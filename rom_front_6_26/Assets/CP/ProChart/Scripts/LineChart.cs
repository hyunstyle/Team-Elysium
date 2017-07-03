///<summary>
/// Pro Chart is a graph and chart system for Unity3D. It has been designed to 
/// support 2D rendering into Unity Canvas System and 2D/3D rendering as Meshes.
/// The chart system supports multiple type of charts, curves and data formats.
///</summary>
///<version>
/// 1.2, 2015.02.10 by Attila Odry, Tamas Barsony, Laszlo Papp
///</version>
///<remarks>
/// Copyright (beer) 2015, Creative Pudding
/// All rights reserved.
/// 
/// Limitation of redistribution:
/// - Redistribution of the code or part of the code in any form is not allowed,
///   but only by written permission from CreativePudding.
///
/// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
/// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
/// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS
/// BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES 
/// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, 
/// OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
/// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
/// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///</remarks>

#if UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1
	#define PRE_UNITY_5_2
#endif

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace CP.ProChart
{

	///<summary>
	/// Canvas based interactive Line chart
	///</summary>
	// [AddComponentMenu("UI/LineChart", 10)]
	public class LineChart : Chart, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
		///<summary>
		/// Enum to indicate type of chart to render
		///</summary>
		public enum ChartType { LINE, CURVE };
		///<summary>
		/// Enum to indicate type of point to render
		///</summary>
		public enum PointType { NONE, CIRCLE, RECTANGLE, TRIANGLE };

		[SerializeField]
		private ChartType chart = ChartType.LINE;
		///<summary>
		/// Get or set type of chart
		///</summary>
		public ChartType Chart {
			get { return chart; }
			set { if (chart != value) { chart = value; Dirty = true; } }
		}

		[SerializeField]
		private PointType point = PointType.CIRCLE;
		///<summary>
		/// Get or set type of point
		///</summary>
		public PointType Point {
			get { return point; }
			set { if (point != value) { point = value; Dirty = true; } }
		}

		///<summary>
		/// Delegate for click on a data by user
		///</summary>
		public delegate void OnSelectDelegate(int row, int column);
		public OnSelectDelegate onSelectDelegate;

		///<summary>
		/// Delegate for mouse over a data
		///</summary>
		public delegate void OnOverDelegate(int row, int column);
		public OnOverDelegate onOverDelegate;
		
		///<summary>
		/// Delegate for notify when chart is became enabled
		///</summary>
		public delegate void OnEnabledDelegate(bool enabled);
		public OnEnabledDelegate onEnabledDelegate;

		[SerializeField]
		[Range(0f, 0.05f)]
		private float thickness = 0.05f;
		///<summary>
		/// Get or set thickness of line
		///</summary>
		public float Thickness {
			get { return thickness; }
			set { if (thickness != value && value >= 0 && value <= 0.05f) { thickness = value; Dirty = true; } }
		}

		[SerializeField]
		[Range(0f, 0.05f)]
		private float pointSize = 0.05f;
		///<summary>
		/// Get or set size of Points
		///</summary>
		public float PointSize {
			get { return pointSize; }
			set { if (pointSize != value && value >= 0 && value <= 0.05f) { pointSize = value; Dirty = true; } }
		}

		///<summary>
		/// 2D data set associated with this chart
		///</summary>
		private ChartData2D values = null;

		private float min = 0;
		private float max = 0;

		///<summary>
		/// Definition of points, which can be active 
		///</summary>
		private class ActivePoint
		{
			public Rect rect;
			public int firstVertex;
			public int quadsCount;
			public int row;
			public int column;
		}

		///<summary>
		/// List of points, which can be activated
		///</summary>
		private List<ActivePoint> points = new List<ActivePoint>();

		///<summary>
		/// Helper flag indicates when new line required in generating lines
		///</summary>
		private bool newLine = true;

		///<summary>
		/// Update the colors when mouse over or user select data
		///</summary>
		override protected void Update()
		{
			base.Update();

			if (isPointerInside && Interactable && point != PointType.NONE && pointSize > 0.0f)
			{
				Vector2 local;
				Camera cam = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, cam, out local);

				for (int i = points.Count - 1; i >= 0; i--)
				{
					if (local.x > points[i].rect.x && local.x < points[i].rect.x + points[i].rect.width
						&& local.y < points[i].rect.y && local.y > points[i].rect.y - points[i].rect.height)
					{
						ChangeCursor(i);
						return;
					}
				}
				ChangeCursor(-1);
			}
		}

		///<summary>
		/// Enable the chart and create test data to give visuals in editor
		///</summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			//only in editor
			if (!Application.isPlaying)
			{
				ChartData2D testValues = new ChartData2D();
				testValues[0, 0] = -1;
				testValues[0, 1] = 0f;
				testValues[0, 2] = -1.5f;
				testValues[1, 0] = -2;
				testValues[1, 1] = -1;
				testValues[1, 2] = -2.5f;
				testValues[2, 0] = -2;
				testValues[2, 1] = -1;
				testValues[2, 2] = -2.5f;

				SetValues(ref testValues);
			}
			Dirty = true;
			OnEnabled(true);
		}

		///<summary>
		/// Delegate to indicate when chart became enabled or disabled
		///</summary>
		void OnEnabled(bool enabled)
		{
			if (onEnabledDelegate != null)
			{
				onEnabledDelegate(enabled);
			}
		}

		///<summary>
		/// Disable the chart
		///</summary>
		protected override void OnDisable()
		{
			OnEnabled(false);
			base.OnDisable();
		}

		///<summary>
		/// Release data set on destroy
		///</summary>
		protected override void OnDestroy()
		{
			if (this.values != null)
			{
				this.values.onDataChangeDelegate -= OnDataChangeDelegate;
			}
			base.OnDestroy();
		}
		
	#if UNITY_EDITOR
		///<summary>
		/// Set dirty flag to true to force invalidation for editor
		///</summary>
		protected override void OnValidate()
		{
			base.OnValidate();
			Dirty = true;
		}
	#endif

		///<summary>
		/// Set dirty flag to force invalidation
		///</summary>
		public void OnDataChangeDelegate()
		{
			Dirty = true;
		}

		///<summary>
		/// Set data set using reference
		///</summary>
		public void SetValues(ref ChartData2D values)
		{
			if (this.values != null)
			{
				this.values.onDataChangeDelegate -= OnDataChangeDelegate;
			}
			this.values = values;
			this.values.onDataChangeDelegate += OnDataChangeDelegate;

			Dirty = true;
		}

		///<summary>
		/// Generate the chart
		///</summary>
		protected override void Create()
		{
#if !PRE_UNITY_5_2
			Vertices = new List<Vector3>();
			VertexColors = new List<Color32>();
			Triangles = new List<int>();
#else
			vertices = new List<UIVertex>();
#endif
			points.Clear();

			if (values == null || values.isEmpty)
			{
				Dirty = false;
				return;
			}

			float border = ((Thickness > PointSize || Point == PointType.NONE) ? Thickness : PointSize) * rectTransform.rect.width * 2;
			float width = rectTransform.rect.width - border;
			float height = rectTransform.rect.height - border;

			float w = width / (float)(values.Columns - 1);
			max = float.MinValue;
			min = float.MaxValue;
			for (int i = 0; i < values.Rows; i++)
			{
				for (int j = 0; j < values.Columns; j++)
				{
					float val = values[i, j];
					max = val > max ? val : max;
					min = val < min ? val : min;
				}
			}
			if (min > 0) min = 0;
			if (max < 0) max = 0;
			float d = (max != min) ? max - min : 1;

			float x0 = -width / 2;
			float y0 = -height / 2;

			if (values.Columns > 0)
			{
				if (chart == ChartType.LINE)
				{
					for (int j = 0; j < values.Rows; j++)
					{
						Color32 color = colors[j % colorCount, 0];
						for (int i = 0; i < values.Columns - 1; i++)
						{
							float x1 = x0 + w * (float)i;
							float x2 = x1 + w;
							float y1 = y0 + ((values[j, i] + Mathf.Abs(min)) / d) * height;
							float y2 = y0 + ((values[j, i + 1] + Mathf.Abs(min)) / d) * height;

							AddLineSegment(x1, y1, x2, y2, color);
						}
						newLine = true;

						for (int i = 0; i < values.Columns; i++)
						{
							Vector3 p0 = new Vector3(x0 + w * (float)i, y0 + ((values[j, i] + Mathf.Abs(min)) / d) * height, 0);
							CreatePoint(p0, j, i);
						}
					}
				}
				else if (chart == ChartType.CURVE)
				{
					int[, ] segment = new int[values.Columns - 1, 4];

					if (values.Columns == 2) //TODO: column
					{
						segment[0, 0] = 0; segment[0, 1] = 0; segment[0, 2] = 1; segment[0, 3] = 1;
					}
					else
					{
						for (int i = 0; i < (values.Columns - 1); i++)
						{
							if (i == 0) //first segment
							{
								segment[i, 0] = 0;
								segment[i, 1] = 0;
								segment[i, 2] = 1;
								segment[i, 3] = 2;
							}
							else if (i == values.Columns - 2) //last segment
							{
								segment[i, 0] = values.Columns - 3;
								segment[i, 1] = values.Columns - 2;
								segment[i, 2] = values.Columns - 1;
								segment[i, 3] = values.Columns - 1;
							}
							else
							{
								segment[i, 0] = i - 1;
								segment[i, 1] = i;
								segment[i, 2] = i + 1;
								segment[i, 3] = i + 2;
							}
						}
					}

					for (int j = 0; j < values.Rows; j++)
					{
						Color32 color = colors[j % colorCount, 0];
						for (int i = 0; i < segment.Length / 4; i++)
						{
							Vector2 c1 = Vector2.zero;
							Vector2 c2 = Vector2.zero;

							Vector2 p0 = new Vector2(x0 + (w * (float)segment[i, 0]), y0 + ((values[j, segment[i, 0]] + Mathf.Abs(min)) / d) * height);
							Vector2 p1 = new Vector2(x0 + (w * (float)segment[i, 1]), y0 + ((values[j, segment[i, 1]] + Mathf.Abs(min)) / d) * height);
							Vector2 p2 = new Vector2(x0 + (w * (float)segment[i, 2]), y0 + ((values[j, segment[i, 2]] + Mathf.Abs(min)) / d) * height);
							Vector2 p3 = new Vector2(x0 + (w * (float)segment[i, 3]), y0 + ((values[j, segment[i, 3]] + Mathf.Abs(min)) / d) * height);
							
							calcControlPoints(p0, p1, p2, p3, out c1, out c2);
							calcLineSegments(p1, c1, c2, p2, color);
						}
						newLine = true;

						color = colors[j % colorCount, 1];
						for (int i = 0; i < values.Columns; i++)
						{
							Vector3 p0 = new Vector3(x0 + (w * (float)i), y0 + ((values[j, i] + Mathf.Abs(min)) / d) * height, 0);
							CreatePoint(p0, j, i);
						}
					}
				}
			}
			Dirty = false;
		}

		///<summary>
		/// Calculate bzr control points and return as c1, c2
		///</summary>
		void calcControlPoints(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, out Vector2 c1, out Vector2 c2)
		{
			float len0 = (p1.x-p0.x) * (p1.x-p0.x) + (p1.y-p0.y) * (p1.y-p0.y);
			float len1 = (p2.x-p1.x) * (p2.x-p1.x) + (p2.y-p1.y) * (p2.y-p1.y);
			float len2 = (p3.x-p2.x) * (p3.x-p2.x) + (p3.y-p2.y) * (p3.y-p2.y);

			float ratio0 = len1 / (len0 + len1);
			float ratio1 = len1 / (len1 + len2);

			c1.x = ((p2.x - p0.x) / 2) * ratio0 + p1.x;
			c1.y = ((p2.y - p0.y) / 2) * ratio0 + p1.y;
			c2.x = p2.x - ((p3.x - p1.x) / 2) * ratio1;
			c2.y = p2.y - ((p3.y - p1.y) / 2) * ratio1;
		}

		///<summary>
		/// Calculate bzr line segments based on control points
		///</summary>
		void calcLineSegments(Vector2 p0, Vector2 c0, Vector2 c1, Vector2 p1, Color32 color)
		{
			float dx = p1.x - p0.x;
			float dy = p1.y - p0.y;
			float len = Mathf.Sqrt(dx * dx + dy * dy);

			len = 0.5f * len;
			int steps = (int)len;
			float step = 1.0f / (Mathf.Floor(steps) - 1);
			float step2 = step * step;
			float step3 = step2 * step;
		
			float ax = 3.0f * (c0.x - c1.x) + p1.x - p0.x;
			float ay = 3.0f * (c0.y - c1.y) + p1.y - p0.y;
			float bx = 3.0f * (p0.x + c1.x) - 6.0f * c0.x;
			float by = 3.0f * (p0.y + c1.y) - 6.0f * c0.y;
			float cx = 3.0f * (c0.x - p0.x);
			float cy = 3.0f * (c0.y - p0.y);
			
			float xdelta = ax * step3 + bx * step2 + cx * step;
			float ydelta = ay * step3 + by * step2 + cy * step;
			float xdelta2 = 6.0f * ax * step3 + 2.0f * bx * step2;
			float ydelta2 = 6.0f * ay * step3 + 2.0f * by * step2;
			float xdelta3 = 6.0f * ax * step3;
			float ydelta3 = 6.0f * ay * step3;
			
			float x = p0.x;
			float y = p0.y;
			for (int i = 0; i < steps - 1; i++)
			{
				float lastX = x;
				float lastY = y;
				x += xdelta;
				xdelta += xdelta2;
				xdelta2 += xdelta3;
				
				y += ydelta;
				ydelta += ydelta2;
				ydelta2 += ydelta3;

				AddLineSegment(lastX, lastY, x, y, color);
			}
		}

#if !PRE_UNITY_5_2

		///<summary>
		/// Add a segment to chart
		///</summary>
		void AddLineSegment(float x1, float y1, float x2, float y2, Color32 color)
		{
			float px = y2 - y1;
			float py = -(x2 - x1);
			float length = Mathf.Sqrt(px * px + py * py);
			
			float nx = (px / length) * rectTransform.rect.width * thickness;
			float ny = (py / length) * rectTransform.rect.width * thickness;

			Vector3 vert0 = new Vector3(x1 + nx, y1 + ny, 0);
			Vector3 vert1 = new Vector3(x1 - nx, y1 - ny, 0);
			Vector3 vert2 = new Vector3(x2 + nx, y2 + ny, 0);
			Vector3 vert3 = new Vector3(x2 - nx, y2 - ny, 0);

			if (Vertices.Count == 0 || newLine)
			{
				Vertices.Add(vert0);
				Vertices.Add(vert1);
				Vertices.Add(vert2);
				Vertices.Add(vert3);
				VertexColors.Add(color);
				VertexColors.Add(color);
				VertexColors.Add(color);
				VertexColors.Add(color);
				
				Triangles.Add(Vertices.Count - 4);
				Triangles.Add(Vertices.Count - 2);
				Triangles.Add(Vertices.Count - 3);

				Triangles.Add(Vertices.Count - 2);
				Triangles.Add(Vertices.Count - 1);
				Triangles.Add(Vertices.Count - 3);
				newLine = false;
				return;
			}

			Vector3 vertP0 = Vertices[Vertices.Count - 4];
			Vector3 vertP1 = Vertices[Vertices.Count - 3];
			Vector3 vertP2 = Vertices[Vertices.Count - 2];
			Vector3 vertP3 = Vertices[Vertices.Count - 1];

			bool intersect = lineSegmentIntersection(vertP0, vertP2, vert0, vert2) | lineSegmentIntersection(vertP1, vertP3, vert1, vert3);
			if (!intersect)
			{
				if (vectorIntersection(vertP0, vertP2, vert2, vert0))
				{
					vert0 = lineIntersectionPoint(vertP0, vertP2, vert2, vert0);
					Vertices[Vertices.Count - 2] = vert0;
				}
				if (vectorIntersection(vertP1, vertP3, vert3, vert1))
				{
					vert1 = lineIntersectionPoint(vertP1, vertP3, vert3, vert1);
					Vertices[Vertices.Count - 1] = vert1;
				}

				Vertices.Add(vert0);
				Vertices.Add(vert1);
				Vertices.Add(vert2);
				Vertices.Add(vert3);
				VertexColors.Add(color);
				VertexColors.Add(color);
				VertexColors.Add(color);
				VertexColors.Add(color);

				Triangles.Add(Vertices.Count - 4);
				Triangles.Add(Vertices.Count - 2);
				Triangles.Add(Vertices.Count - 3);

				Triangles.Add(Vertices.Count - 2);
				Triangles.Add(Vertices.Count - 1);
				Triangles.Add(Vertices.Count - 3);
			}
			else
			{
				Vertices[Vertices.Count - 2] = lineIntersectionPoint(vertP0, vertP2, vert0, vert2);
				Vertices[Vertices.Count - 1] = lineIntersectionPoint(vertP1, vertP3, vert1, vert3);
				Vertices.Add(vert2);
				Vertices.Add(vert3);
				VertexColors.Add(color);
				VertexColors.Add(color);
				Triangles.Add(Vertices.Count - 4);
				Triangles.Add(Vertices.Count - 2);
				Triangles.Add(Vertices.Count - 3);

				Triangles.Add(Vertices.Count - 2);
				Triangles.Add(Vertices.Count - 1);
				Triangles.Add(Vertices.Count - 3);
			}
		}

#else

		///<summary>
		/// Add a segment to chart
		///</summary>
		void AddLineSegment(float x1, float y1, float x2, float y2, Color32 color)
		{
			float px = y2 - y1;
			float py = -(x2 - x1);
			float length = Mathf.Sqrt(px * px + py * py);
			
			float nx = (px / length) * rectTransform.rect.width * thickness;
			float ny = (py / length) * rectTransform.rect.width * thickness;

			Vector3 vert0 = new Vector3(x1 + nx, y1 + ny, 0);
			Vector3 vert1 = new Vector3(x1 - nx, y1 - ny, 0);
			Vector3 vert2 = new Vector3(x2 - nx, y2 - ny, 0);
			Vector3 vert3 = new Vector3(x2 + nx, y2 + ny, 0);

			UIVertex vert = UIVertex.simpleVert;
			vert.color = color;

			if (vertices.Count == 0 || newLine)
			{
				AddQuad(vert0, vert1, vert2, vert3, color);
				newLine = false;
				return;
			}

			Vector3 vertP0 = vertices[vertices.Count - 4].position;
			Vector3 vertP1 = vertices[vertices.Count - 3].position;
			Vector3 vertP2 = vertices[vertices.Count - 2].position;
			Vector3 vertP3 = vertices[vertices.Count - 1].position;

			bool intersect = lineSegmentIntersection(vertP0, vertP3, vert3, vert0) | lineSegmentIntersection(vertP1, vertP2, vert2, vert1);
			if (!intersect)
			{
				if (vectorIntersection(vertP0, vertP3, vert3, vert0))
				{
					vert0 = lineIntersectionPoint(vertP0, vertP3, vert3, vert0);
					vert.position = vert0;
					vertices[vertices.Count - 1] = vert;
				}
				if (vectorIntersection(vertP1, vertP2, vert2, vert1))
				{
					vert1 = lineIntersectionPoint(vertP1, vertP2, vert2, vert1);
					vert.position = vert1;
					vertices[vertices.Count - 2] = vert;
				}
				AddQuad(vert0, vert1, vert2, vert3, color);
			}
			else
			{
				vert0 = lineIntersectionPoint(vertP0, vertP3, vert3, vert0);
				vert.position = vert0;
				vertices[vertices.Count - 1] = vert;

				vert1 = lineIntersectionPoint(vertP1, vertP2, vert2, vert1);
				vert.position = vert1;
				vertices[vertices.Count - 2] = vert;

				AddQuad(vert0, vert1, vert2, vert3, color);
			}
		}

#endif 
		///<summary>
		/// Check intersection of lines
		///</summary>
		bool lineSegmentIntersection(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
		{
			if (v0 == v2 || v0 == v3 || v1 == v2 || v1 == v3)
				return false;

			float l1x = v1.x - v0.x;
			float l1y = v1.y - v0.y;
			float l2x = v3.x - v2.x;
			float l2y = v3.y - v2.y;
			
			float u_b = l2y * l1x - l2x * l1y;
			if (u_b == 0)
			{
				return false;
			}
			
			float ua = ((v3.x - v2.x) * (v0.y - v2.y) - (v3.y - v2.y) * (v0.x - v2.x)) / u_b;
			float ub = ((v1.x - v0.x) * (v0.y - v2.y) - (v1.y - v0.y) * (v0.x - v2.x)) / u_b;

			if (0.0 <= ua && ua <= 1.0 && 0.0 <= ub && ub <= 1.0)
				return true;
			return false;
		}

		///<summary>
		/// Check intersection of vectors
		///</summary>
		bool vectorIntersection(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
		{
			if (v0 == v2 || v0 == v3 || v1 == v2 || v1 == v3)
			{
				return false;
			}
			
			float l1x = v1.x - v0.x;
			float l1y = v1.y - v0.y;
			float l2x = v3.x - v2.x;
			float l2y = v3.y - v2.y;
			
			float u_b = l2y * l1x - l2x * l1y;
			if (u_b == 0)
			{
				return false;
			}
			
			float ua = (l2x * (v0.y - v2.y) - l2y * (v0.x - v2.x)) / u_b;
			float ub = (l1x * (v0.y - v2.y) - l1y * (v0.x - v2.x)) / u_b;
			
			if (0.0f <= ua && 0.0f <= ub)
			{
				return true;
			}
			return false;
		}

		///<summary>
		/// Get point of intersection of lines
		///</summary>
		Vector3 lineIntersectionPoint(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
		{
			Vector3 ret = Vector3.zero;
			float distAB, theCos, theSin, newX, ABpos;

			v1.x-=v0.x; v1.y-=v0.y;
			v2.x-=v0.x; v2.y-=v0.y;
			v3.x-=v0.x; v3.y-=v0.y;

			distAB = Mathf.Sqrt(v1.x * v1.x + v1.y * v1.y);

			theCos = v1.x / distAB;
			theSin = v1.y / distAB;
			newX = v2.x * theCos + v2.y * theSin;
			v2.y = v2.y * theCos - v2.x * theSin;
			v2.x = newX;
			newX = v3.x * theCos + v3.y * theSin;
			v3.y = v3.y * theCos - v3.x * theSin;
			v3.x = newX;

			ABpos=v3.x+(v2.x-v3.x)*v3.y/(v3.y-v2.y);

			ret.x=v0.x + ABpos * theCos;
			ret.y=v0.y + ABpos * theSin;

			return ret; 
		}

#if !PRE_UNITY_5_2

		///<summary>
		/// Generate a point at given location
		///</summary>
		void CreatePoint(Vector3 center, int row, int column)
		{
			float r = rectTransform.rect.width * pointSize;
			Color32 color = colors[row % colorCount, 1];
			switch (point)
			{
				case PointType.CIRCLE:
					{
						Vector3 p1 = new Vector3(r, 0, 0);

						ActivePoint ap = new ActivePoint();
						ap.firstVertex = Vertices.Count;
						ap.quadsCount = 36;
						ap.row = row;
						ap.column = column;
						ap.rect = new Rect(center.x - r, center.y + r, r * 2, r * 2);
						points.Add(ap);

						for (int j = 0; j < 36; j++)
						{
							Vector3 p = p1;
							Vector3 p0 = Quaternion.Euler(0, 0, -(360/72)) * p;
							p1 = Quaternion.Euler(0, 0, -(360/72)) * p0;
							AddQuad(center, center + p, center + p0, center + p1, color);
						}
					}
					break;

				case PointType.RECTANGLE:
					{
						ActivePoint ap = new ActivePoint();
						ap.firstVertex = Vertices.Count;
						ap.quadsCount = 1;
						ap.row = row;
						ap.column = column;
						ap.rect = new Rect(center.x - r, center.y + r, r * 2, r * 2);
						points.Add(ap);

						Vector3 p0 = center + new Vector3(-r, -r, 0);
						Vector3 p1 = center + new Vector3(-r, r, 0);
						Vector3 p2 = center + new Vector3(r, r, 0);
						Vector3 p3 = center + new Vector3(r, -r, 0);
		
						AddQuad(p0, p1, p2, p3, color);
					}
					break;

				case PointType.TRIANGLE:
					{
						ActivePoint ap = new ActivePoint();
						ap.firstVertex = Vertices.Count;
						ap.quadsCount = 1;
						ap.row = row;
						ap.column = column;
						ap.rect = new Rect(center.x - r, center.y + r, r * 2, r * 2);
						points.Add(ap);

						Vector3 p0 = center + new Vector3(0, r, 0);
						Vector3 p1 = center + new Vector3(r, -r, 0);
						Vector3 p2 = center + new Vector3(0, -r, 0);
						Vector3 p3 = center + new Vector3(-r, -r, 0);

						AddQuad(p0, p1, p2, p3, color);
					}
					break;

				case PointType.NONE:
					{
						ActivePoint ap = new ActivePoint();
						ap.firstVertex = 0;
						ap.quadsCount = 0;
						ap.row = row;
						ap.column = column;
						ap.rect = new Rect(center.x - r, center.y + r, r * 2, r * 2);
						points.Add(ap);
					}
					break;
			}
		}

		///<summary>
		/// Add quad
		///</summary>
		void AddQuad(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, Color32 color)
		{
			Vertices.Add(p0);
			VertexColors.Add(color);
			
			Vertices.Add(p1);
			VertexColors.Add(color);
			
			Vertices.Add(p2);
			VertexColors.Add(color);
			
			Vertices.Add(p3);
			VertexColors.Add(color);

			Triangles.Add(Vertices.Count - 4);
			Triangles.Add(Vertices.Count - 3);
			Triangles.Add(Vertices.Count - 1);

			Triangles.Add(Vertices.Count - 1);
			Triangles.Add(Vertices.Count - 3);
			Triangles.Add(Vertices.Count - 2);
		}


		///<summary>
		/// Set cursor color for over state and manage cursor
		///</summary>
		void ChangeCursor(int newCursor)
		{
			if (newCursor != cursor)
			{
				if (cursor != -1)
				{
					int firstVertex = points[cursor].firstVertex;
					int quadsCount = points[cursor].quadsCount;

					Color32 c = colors[points[cursor].row % colorCount, 1] * color;

					for (int i = 0; i < quadsCount; i++)
					{
						VertexColors[firstVertex] = c;
						VertexColors[firstVertex + 1] = c;
						VertexColors[firstVertex + 2] = c;
						VertexColors[firstVertex + 3] = c;
					
						firstVertex += 4;
					}
				}

				if (newCursor != -1)
				{
					int firstVertex = points[newCursor].firstVertex;
					int quadsCount = points[newCursor].quadsCount;

					Color32 c = Color.yellow * color;

					for (int i = 0; i < quadsCount; i++)
					{
						VertexColors[firstVertex] = c;
						VertexColors[firstVertex + 1] = c;
						VertexColors[firstVertex + 2] = c;
						VertexColors[firstVertex + 3] = c;

						firstVertex += 4;
					}
				}
				cursor = newCursor;
				SetVerticesDirty();

				if(onOverDelegate != null)
				{
					if(cursor != -1)
					{
						onOverDelegate(points[cursor].row, points[cursor].column);
					}
					else
					{
						onOverDelegate(-1, -1);
					}
				}
			}
	    }
		


#else

		///<summary>
		/// Generate a point at given location
		///</summary>
		void CreatePoint(Vector3 center, int row, int column)
		{
			float r = rectTransform.rect.width * pointSize;
			Color32 color = colors[row % colorCount, 1];
			switch (point)
			{
				case PointType.CIRCLE:
					{
						Vector3 p1 = new Vector3(r, 0, 0);

						ActivePoint ap = new ActivePoint();
						ap.firstVertex = vertices.Count;
						ap.quadsCount = 36;
						ap.row = row;
						ap.column = column;
						ap.rect = new Rect(center.x - r, center.y + r, r * 2, r * 2);
						points.Add(ap);

						for (int j = 0; j < 36; j++)
						{
							Vector3 p = p1;
							Vector3 p0 = Quaternion.Euler(0, 0, -(360/72)) * p;
							p1 = Quaternion.Euler(0, 0, -(360/72)) * p0;
							AddQuad(center, center + p, center + p0, center + p1, color);
						}
					}
					break;

				case PointType.RECTANGLE:
					{
						ActivePoint ap = new ActivePoint();
						ap.firstVertex = vertices.Count;
						ap.quadsCount = 1;
						ap.row = row;
						ap.column = column;
						ap.rect = new Rect(center.x - r, center.y + r, r * 2, r * 2);
						points.Add(ap);

						Vector3 p0 = center + new Vector3(-r, -r, 0);
						Vector3 p1 = center + new Vector3(-r, r, 0);
						Vector3 p2 = center + new Vector3(r, r, 0);
						Vector3 p3 = center + new Vector3(r, -r, 0);
		
						AddQuad(p0, p1, p2, p3, color);
					}
					break;

				case PointType.TRIANGLE:
					{
						ActivePoint ap = new ActivePoint();
						ap.firstVertex = vertices.Count;
						ap.quadsCount = 1;
						ap.row = row;
						ap.column = column;
						ap.rect = new Rect(center.x - r, center.y + r, r * 2, r * 2);
						points.Add(ap);

						Vector3 p0 = center + new Vector3(0, r, 0);
						Vector3 p1 = center + new Vector3(r, -r, 0);
						Vector3 p2 = center + new Vector3(0, -r, 0);
						Vector3 p3 = center + new Vector3(-r, -r, 0);

						AddQuad(p0, p1, p2, p3, color);
					}
					break;

				case PointType.NONE:
					{
						ActivePoint ap = new ActivePoint();
						ap.firstVertex = 0;
						ap.quadsCount = 0;
						ap.row = row;
						ap.column = column;
						ap.rect = new Rect(center.x - r, center.y + r, r * 2, r * 2);
						points.Add(ap);
					}
					break;
			}
		}

		///<summary>
		/// Add quad
		///</summary>
		void AddQuad(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, Color32 color)
		{
			UIVertex vert = UIVertex.simpleVert;
			vert.color = color;

			vert.position = p0;
			vertices.Add(vert);
			
			vert.position = p1;
			vertices.Add(vert);
			
			vert.position = p2;
			vertices.Add(vert);
			
			vert.position = p3;
			vertices.Add(vert);
		}

		///<summary>
		/// Set cursor color for over state and manage cursor
		///</summary>
		void ChangeCursor(int newCursor)
		{
			if (newCursor != cursor)
			{
				if (cursor != -1)
				{
					int firstVertex = points[cursor].firstVertex;
					int quadsCount = points[cursor].quadsCount;

					Color32 c = colors[points[cursor].row % colorCount, 1] * color;

					for (int i = 0; i < quadsCount; i++)
					{
						UIVertex vert = vertices[firstVertex];
						vert.color = c;
						vertices[firstVertex] = vert;
						
						vert = vertices[firstVertex + 1];
						vert.color = c;
						vertices[firstVertex + 1] = vert;
						
						vert = vertices[firstVertex + 2];
						vert.color = c;
						vertices[firstVertex + 2] = vert;
						
						vert = vertices[firstVertex + 3];
						vert.color = c;
						vertices[firstVertex + 3] = vert;
					
						firstVertex += 4;
					}
				}

				if (newCursor != -1)
				{
					int firstVertex = points[newCursor].firstVertex;
					int quadsCount = points[newCursor].quadsCount;

					for (int i = 0; i < quadsCount; i++)
					{
						UIVertex vert = vertices[firstVertex];
						vert.color = Color.yellow;
						vertices[firstVertex] = vert;
						
						vert = vertices[firstVertex + 1];
						vert.color = Color.yellow;
						vertices[firstVertex + 1] = vert;
						
						vert = vertices[firstVertex + 2];
						vert.color = Color.yellow;
						vertices[firstVertex + 2] = vert;
						
						vert = vertices[firstVertex + 3];
						vert.color = Color.yellow;
						vertices[firstVertex + 3] = vert;

						firstVertex += 4;
					}
				}
				cursor = newCursor;
				SetVerticesDirty();

				if(onOverDelegate != null)
				{
					if(cursor != -1)
					{
						onOverDelegate(points[cursor].row, points[cursor].column);
					}
					else
					{
						onOverDelegate(-1, -1);
					}
				}
			}
	    }
		

#endif

		///<summary>
		/// Implement Canvas handler for IPointerEnterHandler
		///</summary>
		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			isPointerInside = true;
		}

		///<summary>
		/// Implement Canvas handler for IPointerExitHandler
		///</summary>
		public virtual void OnPointerExit(PointerEventData eventData)
		{
			ChangeCursor(-1);
			isPointerInside = false;
		}

		///<summary>
		/// Implement Canvas handler for IPointerClickHandler
		///</summary>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			if (cursor != -1)
			{
				if (onSelectDelegate != null)
				{
					onSelectDelegate(points[cursor].row, points[cursor].column);
				}
			}
		}

		public LabelPosition GetLabelPosition(int row, int column)
		{
			if (points.Count <= row * values.Columns + column)
			{
				return null;
			}

			LabelPosition retVal = new LabelPosition();
			retVal.value = values[row, column];

			Rect r = points[row * values.Columns + column].rect;
			retVal.position = new Vector2(r.center.x, r.center.y - r.height);
			return retVal;
		}

		public LabelPosition[] GetAxisYPositions()
		{
			if (values.isEmpty)
			{
				return null;
			}

			List<LabelPosition> ret = new List<LabelPosition>();

			float border = ((Thickness > PointSize || Point == PointType.NONE) ? Thickness : PointSize) * rectTransform.rect.width * 2;
			float height = rectTransform.rect.height - border;

			float range;
			float y0;
			if (min >= 0)
			{
				range = max;
				y0 = -height / 2.0f;
			}
			else if (max <= 0)
			{
				range = Mathf.Abs(min);
				y0 = height / 2.0f;
			}
			else
			{
				range = max - min;
				y0 = -height / 2.0f + Mathf.Abs(min) / range * height;
			}
			float tickRange = GetTickRange(range, 5);
			float x0 = -rectTransform.rect.width / 2.0f;

			float p = (min > 0) ? 0 : Mathf.Floor(min / tickRange) * tickRange;
			for (int i = 0; i < 5; i++)
			{
				if ((min >= 0 && p <= max) || (max <= 0 && p >= min) || (p >= min && p <= max))
				{
					LabelPosition pos = new LabelPosition();
					pos.position = new Vector2(x0, y0 + p / range * height);
					pos.value = p;
					ret.Add(pos);
				}
				p += tickRange;
			}
			return ret.ToArray();
		}

		public LabelPosition[] GetAxisXPositions()
		{
			if (points.Count == 0)
			{
				return null;
			}

			LabelPosition[] ret = new LabelPosition[values.Columns];

			float y0 = -rectTransform.rect.height / 2.0f;

			for (int i = 0; i < values.Columns; i++)
			{
				LabelPosition pos = new LabelPosition();
				pos.position = new Vector2(points[i].rect.center.x, y0);
				ret[i] = pos;
			}
			return ret;
		}

	} //class

} //namespace

