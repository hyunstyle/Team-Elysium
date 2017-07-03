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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CP.ProChart
{
	///<summary>
	/// Mesh based Line chart
	///</summary>
	public class LineChartMesh : ChartMesh
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
		/// List of vertices
		///</summary>
		private List<Vector3> vertices;
		///<summary>
		/// Helper for 3D points
		///</summary>
		private float z0;
		///<summary>
		/// Helper for 3D points
		///</summary>
		private float z1;

		///<summary>
		/// 2D data set associated with this chart
		///</summary>
		private ChartData2D values = null;
		
		///<summary>
		/// Enable the chart and create test data to give visuals in editor
		///</summary>
		void OnEnable()
		{
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
		}

		///<summary>
		/// Release data set on destroy
		///</summary>
		void OnDestroy()
		{
			if (this.values != null)
			{
				this.values.onDataChangeDelegate -= OnDataChangeDelegate;
			}
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
			Vertices = new List<Vector3>();
			VertexColors = new List<Color32>();
			Triangles = new List<int>();

			vertices = new List<Vector3>();

			if (values == null || values.isEmpty)
			{
				return;
			}

			float border = ((Thickness > PointSize || Point == PointType.NONE) ? Thickness : PointSize) * size.x * 2;
			float width = size.x - border;
			float height = size.y - border;

			float w = width / (float)(values.Columns - 1);
			float max = float.MinValue;
			float min = float.MaxValue;
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

			z0 = mode_3d ? size.z / 2 : 0.0f;
			z1 = z0 - size.z / (float)values.Rows;
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
						CloseLine(color);

						if (point != PointType.NONE && pointSize > 0.0f)
						{
							if (mode_3d)
							{
								z0 += size.z / (float)values.Rows * 0.1f;
								z1 -= size.z / (float)values.Rows * 0.1f;
							}
		
							color = colors[j % colors.Length, 1];
							for (int i = 0; i < values.Columns; i++)
							{
								Vector3 p0 = new Vector3(x0 + w * (float)i, y0 + ((values[j, i] + Mathf.Abs(min)) / d) * height, z0);
								CreatePoint(p0, j, i);
							}

							if (mode_3d)
							{
								z0 -= size.z / (float)values.Rows * 0.1f;
								z1 += size.z / (float)values.Rows * 0.1f;
							}
						}
						if (mode_3d)
						{
							z0 = z1;
							z1 -= size.z / (float)values.Rows;
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
						CloseLine(color);
						
						if (point != PointType.NONE && pointSize > 0.0f)
						{
							if (mode_3d)
							{
								z0 += size.z / (float)values.Rows * 0.1f;
								z1 -= size.z / (float)values.Rows * 0.1f;
							}
		
							color = colors[j % colors.Length, 1];
							for (int i = 0; i < values.Columns; i++)
							{
								Vector3 p0 = new Vector3(x0 + (w * (float)i), y0 + ((values[j, i] + Mathf.Abs(min)) / d) * height, z0);
								CreatePoint(p0, j, i);
							}
		
							if (mode_3d)
							{
								z0 -= size.z / (float)values.Rows * 0.1f;
								z1 += size.z / (float)values.Rows * 0.1f;
							}
						}
						if (mode_3d)
						{
							z0 = z1;
							z1 -= size.z / (float)values.Rows;
						}
					}
				}
			}

			MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
			if (meshFilter.sharedMesh == null)
			{
				meshFilter.sharedMesh = new Mesh();
			}
			meshFilter.sharedMesh.Clear();
			meshFilter.sharedMesh.name = chart == ChartType.LINE ? "Line chart" : "Curve chart";
			meshFilter.sharedMesh.vertices = Vertices.ToArray();
			meshFilter.sharedMesh.triangles = Triangles.ToArray();
			meshFilter.sharedMesh.colors32 = VertexColors.ToArray();

			MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
			Shader shader = Shader.Find("CreativePudding/VertexColor");
			if (renderer.sharedMaterial == null)
			{
				renderer.sharedMaterial	= new Material(shader);
			}
			else
			{
				renderer.sharedMaterial.shader = shader;
			}
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

			len = 30.0f * len;
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

		///<summary>
		/// Add a segment to chart
		///</summary>
		void AddLineSegment(float x1, float y1, float x2, float y2, Color32 color)
		{
			float px = y2 - y1;
			float py = -(x2 - x1);
			float length = Mathf.Sqrt(px * px + py * py);
			
			float nx = (px / length) * thickness;
			float ny = (py / length) * thickness;

			Vector3 vert0 = new Vector3(x1 + nx, y1 + ny, z0);
			Vector3 vert1 = new Vector3(x1 - nx, y1 - ny, z0);
			Vector3 vert2 = new Vector3(x2 + nx, y2 + ny, z0);
			Vector3 vert3 = new Vector3(x2 - nx, y2 - ny, z0);

			if (vertices.Count == 0)
			{
				vertices.Add(vert0);
				vertices.Add(vert1);
				vertices.Add(vert2);
				vertices.Add(vert3);
				return;
			}

			Vector3 vertP0 = vertices[vertices.Count - 4];
			Vector3 vertP1 = vertices[vertices.Count - 3];
			Vector3 vertP2 = vertices[vertices.Count - 2];
			Vector3 vertP3 = vertices[vertices.Count - 1];

			bool intersect = lineSegmentIntersection(vertP0, vertP2, vert0, vert2) | lineSegmentIntersection(vertP1, vertP3, vert1, vert3);
			if (!intersect)
			{
				if (vectorIntersection(vertP0, vertP2, vert2, vert0))
				{
					vert0 = lineIntersectionPoint(vertP0, vertP2, vert2, vert0);
					vertices[vertices.Count - 2] = vert0;
				}
				if (vectorIntersection(vertP1, vertP3, vert3, vert1))
				{
					vert1 = lineIntersectionPoint(vertP1, vertP3, vert3, vert1);
					vertices[vertices.Count - 1] = vert1;
				}

				CloseLine(color);
				vertices.Add(vert0);
				vertices.Add(vert1);
				vertices.Add(vert2);
				vertices.Add(vert3);
			}
			else
			{
				vertices[vertices.Count - 2] = lineIntersectionPoint(vertP0, vertP2, vert0, vert2);
				vertices[vertices.Count - 1] = lineIntersectionPoint(vertP1, vertP3, vert1, vert3);
				vertices.Add(vert2);
				vertices.Add(vert3);
			}
		}

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
			ret.z = z0;

			return ret; 
		}

		///<summary>
		/// Close the line with given color
		///</summary>
		void CloseLine(Color32 color)
		{
			if (vertices.Count == 0)
				return;

			int offset = Vertices.Count;
			int triangleCount = vertices.Count - 2;
			List<int> triangles = new List<int>();
			for (int i = 0; i < triangleCount / 2; i++)
			{
				triangles.Add(offset + i*2);
				triangles.Add(offset + i*2+2);
				triangles.Add(offset + i*2+1);
				
				triangles.Add(offset + i*2+2);
				triangles.Add(offset + i*2+3);
				triangles.Add(offset + i*2+1);
			}

			for (int i = 0; i < vertices.Count; i++)
			{
				VertexColors.Add(color);
			}

			if (mode_3d)
			{
				List<Vector3> v2 = new List<Vector3>();
				List<int> t2 = new List<int>();

				for (int i = 0; i < vertices.Count; i++)
				{
				 	v2.Add(new Vector3(vertices[i].x, vertices[i].y, z1));
					VertexColors.Add(color);
				}

				for (int i = 0; i < triangleCount * 3; i+=3)
				{
					t2.Add(triangles[i] + vertices.Count);
					t2.Add(triangles[i+2] + vertices.Count);
					t2.Add(triangles[i+1] + vertices.Count);
				}

				for (int i = 0; i < triangles.Count; i+=6)
				{
					t2.Add(triangles[i+4]);
					t2.Add(t2[i+1]);
					t2.Add(triangles[i+2]);

					t2.Add(triangles[i+4]);
					t2.Add(t2[i+5]);
					t2.Add(t2[i+1]);

					t2.Add(triangles[i]);
					t2.Add(t2[i]);
					t2.Add(t2[i+2]);

					t2.Add(triangles[i+1]);
					t2.Add(triangles[i]);
					t2.Add(t2[i+2]);
				}

				t2.Add(offset);
				t2.Add(offset + 1);
				t2.Add(offset + vertices.Count);

				t2.Add(offset + vertices.Count + 1);
				t2.Add(offset + vertices.Count);
				t2.Add(offset + 1);

				t2.Add(offset + vertices.Count - 2);
				t2.Add(offset + vertices.Count * 2 -1);
				t2.Add(offset + vertices.Count - 1);

				t2.Add(offset + vertices.Count * 2 - 1);
				t2.Add(offset + vertices.Count - 2);
				t2.Add(offset + vertices.Count * 2 - 2);

				vertices.AddRange(v2);
				triangles.AddRange(t2);
			}

			Vertices.AddRange(vertices);
			Triangles.AddRange(triangles);

			vertices.Clear();
		}

		///<summary>
		/// Generate a point at given location
		///</summary>
		void CreatePoint(Vector3 center, int row, int column)
		{
			float r = size.x * pointSize;
			Color32 color = colors[row % colorCount, 1];

			switch (point)
			{
				case PointType.CIRCLE:
					{
						Vector3 p1 = new Vector3(r, 0, 0);
						for(int i = 0; i < 360 / 30; i++)
						{
							Vector3 p = p1;
							p1 = Quaternion.Euler(0, 0, 30) * p1;

							Vertices.Add(center);
							Vertices.Add(center + p);
							Vertices.Add(center + p1);
							VertexColors.Add(color);
							VertexColors.Add(color);
							VertexColors.Add(color);
							Triangles.Add(Vertices.Count - 3);
							Triangles.Add(Vertices.Count - 2);
							Triangles.Add(Vertices.Count - 1);

							if (mode_3d)
							{
								Vector3 p3 = center; p3.z = z1;
								Vector3 p4 = center + p; p4.z = z1;
								Vector3 p5 = center + p1; p5.z = z1;
			
								AddTriangle(p3, p5, p4, color, color);
								
								AddTriangle(center + p, p4, center + p1, color, color);
								AddTriangle(center + p1, p4, p5, color, color);
							}
						}
					}
					break;

				case PointType.RECTANGLE:
					{
						Vector3 p0 = center + new Vector3(-r, -r, 0);
						Vector3 p1 = center + new Vector3(-r, r, 0);
						Vector3 p2 = center + new Vector3(r, r, 0);
						Vector3 p3 = center + new Vector3(r, -r, 0);

						// Vector3 p = new Vector3(0, r, 0);
						// Vector3 p0 = center + Quaternion.Euler(0, 0, 45) * p;
						// Vector3 p1 = center + Quaternion.Euler(0, 0, 135) * p;
						// Vector3 p2 = center + Quaternion.Euler(0, 0, 225) * p;
						// Vector3 p3 = center + Quaternion.Euler(0, 0, 315) * p;
		
						AddTriangle(p0, p1, p3, color, color);
						AddTriangle(p3, p1, p2, color, color);

						if (mode_3d)
						{
							Vector3 p4 = p0; p4.z = z1;
							Vector3 p5 = p1; p5.z = z1;
							Vector3 p6 = p2; p6.z = z1;
							Vector3 p7 = p3; p7.z = z1;

							AddTriangle(p4, p7, p5, color, color);
							AddTriangle(p7, p6, p5, color, color);

							AddTriangle(p0, p4, p1, color, color);
							AddTriangle(p5, p1, p4, color, color);

							AddTriangle(p2, p1, p5, color, color);
							AddTriangle(p2, p5, p6, color, color);

							AddTriangle(p2, p6, p3, color, color);
							AddTriangle(p7, p3, p6, color, color);
							
							AddTriangle(p3, p7, p0, color, color);
							AddTriangle(p0, p7, p4, color, color);
						}
					}
					break;

				case PointType.TRIANGLE:
					{
						Vector3 p0 = center + new Vector3(0, r, 0);
						Vector3 p1 = center + new Vector3(r, -r, 0);
						Vector3 p2 = center + new Vector3(-r, -r, 0);

						// Vector3 p = new Vector3(0, r, 0);
						// Vector3 p0 = center + p;
						// Vector3 p1 = center + Quaternion.Euler(0, 0, 120) * p;
						// Vector3 p2 = center + Quaternion.Euler(0, 0, 240) * p;

						AddTriangle(p0, p1, p2, color, color);

						if (mode_3d)
						{
							Vector3 p3 = p0; p3.z = z1;
							Vector3 p4 = p1; p4.z = z1;
							Vector3 p5 = p2; p5.z = z1;

							AddTriangle(p3, p5, p4, color, color);

							AddTriangle(p0, p4, p1, color, color);
							AddTriangle(p0, p3, p4, color, color);

							AddTriangle(p1, p4, p2, color, color);
							AddTriangle(p2, p4, p5, color, color);

							AddTriangle(p2, p5, p0, color, color);
							AddTriangle(p5, p3, p0, color, color);
						}
					}
					break;
			}
		}

		///<summary>
		/// Add triangle with colors
		///</summary>
		void AddTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Color32 color1, Color32 color2)
		{
			Vertices.Add(p1);
			Vertices.Add(p2);
			Vertices.Add(p3);
			VertexColors.Add(color1);
			VertexColors.Add(color2);
			VertexColors.Add(color2);
			Triangles.Add(Vertices.Count - 3);
			Triangles.Add(Vertices.Count - 2);
			Triangles.Add(Vertices.Count - 1);
		}
	}

} //namespace


