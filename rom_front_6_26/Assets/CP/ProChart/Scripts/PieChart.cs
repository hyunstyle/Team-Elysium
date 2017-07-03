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
	/// Canvas renderer version of PieChart
	/// This class respnosible to render the Pie chart to Canvas
	///</summary>
	// [AddComponentMenu("UI/PieChart", 10)]
	public class PieChart : Chart, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{

		///<summary>
		/// Delegate for click on a data by user
		///</summary>
		public delegate void OnSelectDelegate(int column);
		public OnSelectDelegate onSelectDelegate;

		///<summary>
		/// Delegate for mouse over a data
		///</summary>
		public delegate void OnOverDelegate(int column);
		public OnOverDelegate onOverDelegate;

		///<summary>
		/// Delegate for notify when chart is became enabled
		///</summary>
		public delegate void OnEnabledDelegate(bool enabled);
		public OnEnabledDelegate onEnabledDelegate;

		[SerializeField]
		[Range(0, 0.9f)]
		private float innerRadius = 0;
		///<summary>
		/// size of inner hole in percentage of diameter of chart, float in range 0f to 0.9f
		///</summary>
		public float InnerRadius {
			get { return innerRadius; }
			set { if (innerRadius != value && value >= 0.0f && value <= 0.9f) { innerRadius = value; Dirty = true; } }
		}

		[SerializeField]
		[Range(0, 360)]
		private float startAngle = 0;
		///<summary>
		/// degree of zero position in clock-wise direction, float in range of 0f to 360f, where 0f = top (12 hr)
		///</summary>
		public float StartAngle {
			get { return startAngle; }
			set { if (startAngle != value && value >= 0.0f && value <= 360.0f) { startAngle = value; Dirty = true; } }
		}

		[SerializeField]
		[Range(-360, 360)]
		private float chartSize = 360;
		///<summary>
		/// degree of size, float in range of -360f to +360f
		///</summary>
		public float ChartSize {
			get { return chartSize; }
			set { if (chartSize != value && value >= -360.0f && value <= 360.0f) { chartSize = value; Dirty = true; } }
		}

		///<summary>
		/// 1D data set associated with this chart
		///</summary>
		private ChartData1D values = null;

		///<summary>
		/// Definition of sector, which can be active 
		///</summary>
		private class Sector
		{
			public int firstVertex;
			public int quadsCount;
			public int column;
			public float start;
			public float center;
			public float end;
		}

		///<summary>
		/// List of sectors, which can be activated
		///</summary>
		private List<Sector> sectors = new List<Sector>();
		
		///<summary>
		/// Update the colors when mouse over or user select data
		///</summary>
		override protected void Update()
		{
			base.Update();
			if (isPointerInside && Interactable)
			{
				Vector2 local;
				Camera cam = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, cam, out local);

				float s = Mathf.Pow(local.y, 2) / Mathf.Pow(rectTransform.rect.height / 2, 2) + Mathf.Pow(local.x, 2) / Mathf.Pow(rectTransform.rect.width / 2, 2);
				float si = Mathf.Pow(local.y, 2) / Mathf.Pow(rectTransform.rect.height / 2 * innerRadius, 2) + Mathf.Pow(local.x, 2) / Mathf.Pow(rectTransform.rect.width * innerRadius / 2, 2);
				if (s < 1 && si > 1)
				{
					float a = Angle(Vector3.up, local);

					for (int i = 0; i < sectors.Count; i++)
					{
						float start = sectors[i].start;
						float end = sectors[i].end;

						if ((start < end && (a >= start && a < end)) || (start > end && (a >= start || a < end)))
						{
							ChangeCursor(i);
							return;
						}
					}
				}			
				ChangeCursor(-1);
			}
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
		/// Enable the chart and create test data to give visuals in editor
		///</summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			//only in editor
			if (!Application.isPlaying)
			{
				ChartData1D testValues = new ChartData1D();
				testValues[0] = 120.85f;
				testValues[1] = -90.2f;
				testValues[2] = 35;
				testValues[3] = 40.85f;
				testValues[4] = 60.0f;
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
		public void SetValues(ref ChartData1D values)
		{
			if (this.values != null)
			{
				this.values.onDataChangeDelegate -= OnDataChangeDelegate;
			}
			this.values = values;
			this.values.onDataChangeDelegate += OnDataChangeDelegate;

			Dirty = true;
		}

#if !PRE_UNITY_5_2
		///<summary>
		/// Add quad
		///</summary>
		void AddQuad(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, Color32 color1, Color32 color2)
		{
			Vertices.Add(new Vector3(p0.x * rectTransform.rect.width, p0.y * rectTransform.rect.height, 0));
			VertexColors.Add(color1);

			Vertices.Add(new Vector3(p1.x * rectTransform.rect.width, p1.y * rectTransform.rect.height, 0));
			VertexColors.Add(color2);

			Vertices.Add(new Vector3(p2.x * rectTransform.rect.width, p2.y * rectTransform.rect.height, 0));
			VertexColors.Add(color2);
			
			Vertices.Add(new Vector3(p3.x * rectTransform.rect.width, p3.y * rectTransform.rect.height, 0));
			VertexColors.Add(color1);

			Triangles.Add(Vertices.Count - 4);
			Triangles.Add(Vertices.Count - 3);
			Triangles.Add(Vertices.Count - 1);

			Triangles.Add(Vertices.Count - 1);
			Triangles.Add(Vertices.Count - 3);
			Triangles.Add(Vertices.Count - 2);
		}
		
#else

		///<summary>
		/// Add quad
		///</summary>
		void AddQuad(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, Color32 color1, Color32 color2)
		{
			UIVertex vert = UIVertex.simpleVert;

			vert.position = new Vector3(p0.x * rectTransform.rect.width, p0.y * rectTransform.rect.height, 0);
			vert.color = color1;
			vertices.Add(vert);

			vert.position = new Vector3(p1.x * rectTransform.rect.width, p1.y * rectTransform.rect.height, 0);
			vert.color = color2;
			vertices.Add(vert);

			vert.position = new Vector3(p2.x * rectTransform.rect.width, p2.y * rectTransform.rect.height, 0);
			vert.color = color2;
			vertices.Add(vert);
			
			vert.position = new Vector3(p3.x * rectTransform.rect.width, p3.y * rectTransform.rect.height, 0);
			vert.color = color1;
			vertices.Add(vert);
		}
#endif

		///<summary>
		/// Get angle from two vector
		///</summary>
		private float Angle(Vector3 a, Vector3 b)
		{
			float angle = (Mathf.Atan2(a.y, a.x) - Mathf.Atan2(b.y, b.x)) * Mathf.Rad2Deg;
			if (angle < 0.0f)
				angle += 360.0f;
			return angle;
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
			sectors.Clear();

			if (values == null || values.isEmpty)
			{
				return;
			}

			float total = 0.0f;
			float percent = 0.0f;
			for (int i = 0; i < values.Columns; i++)
			{
				total += Mathf.Abs(values[i]);
			}

			Vector3 pp0 = Quaternion.Euler(0, 0, -startAngle) * Vector3.up * 0.5f;
			Vector3 pp0i = pp0 * innerRadius;

			for (int i = 0; i < values.Columns; i++)
			{
				float val = Mathf.Abs(values[i]);
				
				percent += val / total;

				float angle = Mathf.Abs(chartSize) * val / total;

				Color32 color1 = colors[i % colorCount, 1] * color;
				Color32 color2 = colors[i % colorCount, 0] * color;

				int j = Mathf.CeilToInt(angle / 5);
				Quaternion step = Quaternion.identity;
				if (j != 0)
				{
					step = Quaternion.Euler(0, 0, -Mathf.Sign(chartSize) * angle / (float)j);
				}

				Sector sector = new Sector();
#if !PRE_UNITY_5_2
				sector.firstVertex = Vertices.Count;
#else
				sector.firstVertex = vertices.Count;
#endif
				sector.column = i;
				Vector3 p = new Vector3(pp0.x * rectTransform.rect.width, pp0.y * rectTransform.rect.height);
				if (chartSize < 0)
				{
					sector.end = Angle(Vector3.up, p);
					sector.center = sector.end - angle / 2;
				}
				else
				{
					sector.start = Angle(Vector3.up, p);
					sector.center = sector.start + angle / 2;
				}

				for (; j > 0; j--)
				{
					Vector3 pp1 = step * pp0;
					Vector3 pp1i = step * pp0i;

					AddQuad(pp0i, pp0, pp1, pp1i, color1, color2);

					pp0 = pp1;
					pp0i = pp1i;
				}

				p = new Vector3(pp0.x * rectTransform.rect.width, pp0.y * rectTransform.rect.height);
				if (chartSize < 0)
				{
					sector.start = Angle(Vector3.up, p);
				}
				else
				{
					sector.end = Angle(Vector3.up, p);
				}
#if !PRE_UNITY_5_2
				sector.quadsCount = (Vertices.Count - sector.firstVertex) / 4;
#else
				sector.quadsCount = (vertices.Count - sector.firstVertex) / 4;
#endif
				sectors.Add(sector);
			}

			ChangeCursor(cursor, true);
			Dirty = false;
		}


#if !PRE_UNITY_5_2
		///<summary>
		/// Set cursor color for over state and manage cursor
		///</summary>
		void ChangeCursor(int newCursor, bool create = false)
		{
			if (newCursor != cursor || create)
			{
				if (cursor != -1)
				{
					int firstVertex = sectors[cursor].firstVertex;
					int quadsCount = sectors[cursor].quadsCount;

					Color32 color1 = colors[sectors[cursor].column % colorCount, 1] * color;
					Color32 color2 = colors[sectors[cursor].column % colorCount, 0] * color;

					for (int i = 0; i < quadsCount; i++)
					{
						VertexColors[firstVertex] = color1;
						VertexColors[firstVertex + 1] = color2;
						VertexColors[firstVertex + 2] = color2;
						VertexColors[firstVertex + 3] = color1;
					
						firstVertex += 4;
					}
				}

				if (newCursor != -1)
				{
					int firstVertex = sectors[newCursor].firstVertex;
					int quadsCount = sectors[newCursor].quadsCount;

					Color32 color1 = selectedColors[sectors[newCursor].column % colorCount, 1] * color;
					Color32 color2 = selectedColors[sectors[newCursor].column % colorCount, 0] * color;

					for (int i = 0; i < quadsCount; i++)
					{
						VertexColors[firstVertex] = color1;
						VertexColors[firstVertex + 1] = color2;
						VertexColors[firstVertex + 2] = color2;
						VertexColors[firstVertex + 3] = color1;

						firstVertex += 4;
					}
				}
				cursor = newCursor;
				if (!create)
				{
					SetVerticesDirty();
				}

				if(onOverDelegate != null)
				{
					if(cursor != -1)
					{
						onOverDelegate(sectors[cursor].column);
					}
					else
					{
						onOverDelegate(-1);
					}
				}
			}
	    }

#else

		///<summary>
		/// Set cursor color for over state and manage cursor
		///</summary>
		void ChangeCursor(int newCursor, bool create = false)
		{
			if (newCursor != cursor || create)
			{
				if (cursor != -1)
				{
					int firstVertex = sectors[cursor].firstVertex;
					int quadsCount = sectors[cursor].quadsCount;

					Color32 color1 = colors[sectors[cursor].column % colorCount, 1] * color;
					Color32 color2 = colors[sectors[cursor].column % colorCount, 0] * color;

					for (int i = 0; i < quadsCount; i++)
					{
						UIVertex vert = vertices[firstVertex];
						vert.color = color1;
						vertices[firstVertex] = vert;
						
						vert = vertices[firstVertex + 1];
						vert.color = color2;
						vertices[firstVertex + 1] = vert;
						
						vert = vertices[firstVertex + 2];
						vert.color = color2;
						vertices[firstVertex + 2] = vert;
						
						vert = vertices[firstVertex + 3];
						vert.color = color1;
						vertices[firstVertex + 3] = vert;
					
						firstVertex += 4;
					}
				}

				if (newCursor != -1)
				{
					int firstVertex = sectors[newCursor].firstVertex;
					int quadsCount = sectors[newCursor].quadsCount;

					Color32 color1 = selectedColors[sectors[newCursor].column % colorCount, 1] * color;
					Color32 color2 = selectedColors[sectors[newCursor].column % colorCount, 0] * color;

					for (int i = 0; i < quadsCount; i++)
					{
						UIVertex vert = vertices[firstVertex];
						vert.color = color1;
						vertices[firstVertex] = vert;
						
						vert = vertices[firstVertex + 1];
						vert.color = color2;
						vertices[firstVertex + 1] = vert;
						
						vert = vertices[firstVertex + 2];
						vert.color = color2;
						vertices[firstVertex + 2] = vert;
						
						vert = vertices[firstVertex + 3];
						vert.color = color1;
						vertices[firstVertex + 3] = vert;

						firstVertex += 4;
					}
				}
				cursor = newCursor;
				if (!create)
				{
					SetVerticesDirty();
				}

				if(onOverDelegate != null)
				{
					if(cursor != -1)
					{
						onOverDelegate(sectors[cursor].column);
					}
					else
					{
						onOverDelegate(-1);
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
					onSelectDelegate(sectors[cursor].column);
				}
			}
		}

		public LabelPosition GetLabelPosition(int column, float position = 0.5f)
		{
			if (sectors.Count <= column)
			{
				return null;
			}

			LabelPosition retVal = new LabelPosition();
			retVal.value = values[column];

			Vector3 v = Quaternion.Euler(0, 0, -sectors[column].center) * Vector3.up * 0.5f;
			retVal.position = new Vector2(v.x * rectTransform.rect.width * position, v.y * rectTransform.rect.height * position);
			return retVal;
		}


	} //class

} //namespace

