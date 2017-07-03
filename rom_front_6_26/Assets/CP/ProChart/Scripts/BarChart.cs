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
	/// Canvas renderer version of Bar chart.
	/// This class respnosible to render the Bar chart to Canvas
	///</summary>
	// [AddComponentMenu("UI/BarChart", 10)]
	public class BarChart : Chart, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
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

		///<summary>
		/// 2D data set associated with this chart
		///</summary>
		private ChartData2D values = null;

		private float min = 0;
		private float max = 0;

		///<summary>
		/// Definition of bar, which can be active 
		///</summary>
		private class Bar
		{
			public Rect rect;
			public int firstVertex;
			public int row;
			public int column;
		}

		///<summary>
		/// List of bars, which can be activated
		///</summary>
		private List<Bar> bars = new List<Bar>();

		[SerializeField]
		[Range(0.1f, 1.0f)]
		private float spacing = 1;
		///<summary>
		/// Distance between data groups, float in range of 0.1f to 1f, where 0.1f is minimum space
		///</summary>
		public float Spacing {
			get { return spacing; }
			set { if (spacing != value && value >= 0.1f && value <= 1.0f) { spacing = value; Dirty = true; } }
		}

		[SerializeField]
		[Range(0.1f, 1.0f)]
		private float thickness = 1;
		///<summary>
		/// Thickness of bars, float in range of 0.1f to 1f, where 1f is the thickest bar
		///</summary>
		public float Thickness {
			get { return thickness; }
			set { if (thickness != value && value >= 0.1f && value <= 1.0f) { thickness = value; Dirty = true; } }
		}

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

				for (int i = 0; i < bars.Count; i++)
				{
					if (local.x > bars[i].rect.x && local.x < bars[i].rect.x + bars[i].rect.width
						&& local.y < bars[i].rect.y && local.y > bars[i].rect.y + bars[i].rect.height)
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
				testValues[0, 0] = 2;
				testValues[0, 1] = 1;
				testValues[0, 2] = 3;
				testValues[1, 0] = 1;
				testValues[1, 1] = 5;
				testValues[1, 2] = 1;
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
			bars.Clear();
			
			if (values == null || values.isEmpty)
			{
				return;
			}

			float w = rectTransform.rect.width / (float)(values.Columns * values.Rows + (values.Columns - 1));
			float gap = w * spacing;
			w = (rectTransform.rect.width - gap * (float)(values.Columns - 1)) / (float)(values.Columns * values.Rows);

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

			float d = (max != min) ? max - min : 1;

			float x0 = -rectTransform.rect.width / 2;
			float y0 = -rectTransform.rect.height / 2;

			if (values.Columns > 0)
			{
				Vector2 p1 = Vector2.zero;
				Vector2 p2 = Vector2.zero;

				for (int i = 0; i < values.Columns; i++)
				{
					for (int j = 0; j < values.Rows; j++)
					{
						p1.x = x0 + (float)((j + i * values.Rows) * w + (i * gap)) + w * (1 - thickness) / 2;
						p2.x = p1.x + w * thickness;
						
						if (max < 0)
						{
							p1.y = rectTransform.rect.height / 2;
							p2.y = p1.y - values[j, i] / min * rectTransform.rect.height;
						}
						else if (min < 0)
						{
							p1.y = y0 + ((Mathf.Abs(min) + values[j, i]) / d) * rectTransform.rect.height;
							p2.y = y0 + (Mathf.Abs(min) / d) * rectTransform.rect.height;
						}
						else
						{
							p1.y = y0 + (values[j, i] / max) * rectTransform.rect.height;
							p2.y = y0;
						}

						if(min < 0 && max > 0 && values[j,i] < 0 )
						{
							float x = p1.x;
							p1.x = p2.x;
							p2.x = x;						
							AddQuad(p2, p1, j, i);
						}
						else
						{
							AddQuad(p1, p2, j, i);
						}
					}
				}
			}
			Dirty = false;
		}

#if !PRE_UNITY_5_2
		///<summary>
		/// Add quad
		///</summary>
		void AddQuad(Vector2 topLeft, Vector2 bottomRight, int row, int column)
		{
			bool colorSwap = values[row, column] < 0;
			Color32 colorTop = colors[row % colorCount, colorSwap ? 1 : 0] * color;
			Color32 colorBottom = colors[row % colorCount, colorSwap ? 0 : 1] * color;

			Bar bar = new Bar();
			bar.firstVertex = Vertices.Count;
			bar.row = row;
			bar.column = column;
			bar.rect = new Rect(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
			bars.Add(bar);

			Vertices.Add(topLeft);
			VertexColors.Add(colorTop);

			Vertices.Add(new Vector3(topLeft.x, bottomRight.y, 0));
			VertexColors.Add(colorBottom);

			Vertices.Add(bottomRight);
			VertexColors.Add(colorBottom);
			
			Vertices.Add(new Vector3(bottomRight.x, topLeft.y, 0));
			VertexColors.Add(colorTop);

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
					int firstVertex = bars[cursor].firstVertex;

					Color32 colorTop = colors[bars[cursor].row % colorCount, 0] * color;
					Color32 colorBottom = colors[bars[cursor].row % colorCount, 1] * color;

					VertexColors[firstVertex] = colorTop;
					VertexColors[firstVertex + 1] = colorBottom;
					VertexColors[firstVertex + 2] = colorBottom;
					VertexColors[firstVertex + 3] = colorTop;
				}

				if (newCursor != -1)
				{
					int firstVertex = bars[newCursor].firstVertex;

					Color32 colorTop = selectedColors[bars[newCursor].row % colorCount, 0] * color;
					Color32 colorBottom = selectedColors[bars[newCursor].row % colorCount, 1] * color;

					VertexColors[firstVertex] = colorTop;
					VertexColors[firstVertex + 1] = colorBottom;
					VertexColors[firstVertex + 2] = colorBottom;
					VertexColors[firstVertex + 3] = colorTop;
				}
				cursor = newCursor;
				SetVerticesDirty();

				if(onOverDelegate != null)
				{
					if(cursor != -1)
					{
						onOverDelegate(bars[cursor].row, bars[cursor].column);
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
		/// Add quad
		///</summary>
		void AddQuad(Vector2 topLeft, Vector2 bottomRight, int row, int column)
		{
			UIVertex vert = UIVertex.simpleVert;
			
			bool colorSwap = values[row, column] < 0;
			Color32 colorTop = colors[row % colorCount, colorSwap ? 1 : 0] * color;
			Color32 colorBottom = colors[row % colorCount, colorSwap ? 0 : 1] * color;

			Bar bar = new Bar();
			bar.firstVertex = vertices.Count;
			bar.row = row;
			bar.column = column;
			bar.rect = new Rect(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
			bars.Add(bar);

			vert.position = topLeft;
			vert.color = colorTop;
			vertices.Add(vert);

			vert.position = new Vector3(topLeft.x, bottomRight.y, 0);
			vert.color = colorBottom;
			vertices.Add(vert);

			vert.position = bottomRight;
			vert.color = colorBottom;
			vertices.Add(vert);
			
			vert.position = new Vector3(bottomRight.x, topLeft.y, 0);
			vert.color = colorTop;
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
					int firstVertex = bars[cursor].firstVertex;

					Color32 colorTop = colors[bars[cursor].row % colorCount, 0] * color;
					Color32 colorBottom = colors[bars[cursor].row % colorCount, 1] * color;

					UIVertex vert = vertices[firstVertex];
					vert.color = colorTop;
					vertices[firstVertex] = vert;
					
					vert = vertices[firstVertex + 1];
					vert.color = colorBottom;
					vertices[firstVertex + 1] = vert;
					
					vert = vertices[firstVertex + 2];
					vert.color = colorBottom;
					vertices[firstVertex + 2] = vert;
					
					vert = vertices[firstVertex + 3];
					vert.color = colorTop;
					vertices[firstVertex + 3] = vert;
				}

				if (newCursor != -1)
				{
					int firstVertex = bars[newCursor].firstVertex;

					Color32 colorTop = selectedColors[bars[newCursor].row % colorCount, 0] * color;
					Color32 colorBottom = selectedColors[bars[newCursor].row % colorCount, 1] * color;

					UIVertex vert = vertices[firstVertex];
					vert.color = colorTop;
					vertices[firstVertex] = vert;
					
					vert = vertices[firstVertex + 1];
					vert.color = colorBottom;
					vertices[firstVertex + 1] = vert;
					
					vert = vertices[firstVertex + 2];
					vert.color = colorBottom;
					vertices[firstVertex + 2] = vert;
					
					vert = vertices[firstVertex + 3];
					vert.color = colorTop;
					vertices[firstVertex + 3] = vert;
				}
				cursor = newCursor;
				SetVerticesDirty();

				if(onOverDelegate != null)
				{
					if(cursor != -1)
					{
						onOverDelegate(bars[cursor].row, bars[cursor].column);
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
					onSelectDelegate(bars[cursor].row, bars[cursor].column);
				}
			}
		}

		public LabelPosition GetLabelPosition(int row, int column, float position = 0.5f)
		{
			if (bars.Count <= row * values.Columns + column)
			{
				return null;
			}

			LabelPosition retVal = new LabelPosition();
			retVal.value = values[row, column];

			Rect r = bars[column * values.Rows + row].rect;
			if (values[row, column] < 0)
			{
				retVal.position = new Vector2(r.center.x, r.y + r.height * position);
			}
			else
			{
				retVal.position = new Vector2(r.center.x, (r.y + r.height) - r.height * position);
			}
			return retVal;
		}
	
		public LabelPosition[] GetAxisYPositions()
		{
			if (values.isEmpty)
			{
				return null;
			}

			List<LabelPosition> ret = new List<LabelPosition>();

			float range;
			float y0;
			if (min >= 0)
			{
				range = max;
				y0 = -rectTransform.rect.height / 2.0f;
			}
			else if (max <= 0)
			{
				range = Mathf.Abs(min);
				y0 = rectTransform.rect.height / 2.0f;
			}
			else
			{
				range = max - min;
				y0 = -rectTransform.rect.height / 2.0f + Mathf.Abs(min) / range * rectTransform.rect.height;
			}
			float tickRange = GetTickRange(range, 5);
			float x0 = -rectTransform.rect.width / 2.0f;

			float p = (min > 0) ? 0 : Mathf.Floor(min / tickRange) * tickRange;
			for (int i = 0; i < 5; i++)
			{
				if ((min >= 0 && p <= max) || (max <= 0 && p >= min) || (p >= min && p <= max))
				{
					LabelPosition pos = new LabelPosition();
					pos.position = new Vector2(x0, y0 + p / range * rectTransform.rect.height);
					pos.value = p;
					ret.Add(pos);
				}
				p += tickRange;
			}
			// string s = string.Format("min: {0}, max: {1}, tickRange: {2} [{3}](", min, max, tickRange, ret.Count);
			// for (int i = 0; i < ret.Count; i++)
			// {
			// 	s += ret[i].value + ", ";
			// }
			// s += ")";
			// Debug.Log(s);
			return ret.ToArray();
		}

		public LabelPosition[] GetAxisXPositions()
		{
			if (values.isEmpty)
			{
				return null;
			}

			LabelPosition[] ret = new LabelPosition[values.Columns];

			float w = rectTransform.rect.width / (float)(values.Columns * values.Rows + (values.Columns - 1));
			float gap = w * spacing;
			w = (rectTransform.rect.width - gap * (float)(values.Columns - 1)) / (float)(values.Columns * values.Rows);
			float rows = values.Rows;

			float x0 = -rectTransform.rect.width / 2.0f;
			float y0 = -rectTransform.rect.height / 2.0f;

			for (int i = 0; i < values.Columns; i++)
			{
				LabelPosition pos = new LabelPosition();
				pos.position = new Vector2(x0 + (((i + 0.5f) * rows) * w + (i * gap)), y0);
				ret[i] = pos;
			}
			return ret;
		}

	} //class

} //namespace


