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
	/// Abstract base class for Canvas renderer.
	/// Each chart must implement this class, which requires to render into Canvas.
	///</summary>
	public abstract class Chart : MaskableGraphic
	{
		///<summary>
		/// Inidicates if data set changed and need invalidation.
		///</summary>
		public bool Dirty	{ get; set; }

		///<summary>
		/// currently selected item
		///</summary>
		protected int cursor = -1;

#if !PRE_UNITY_5_2
		///<summary>
		/// List of vertices to render in mesh
		///</summary>
		protected List<Vector3> Vertices;
		///<summary>
		/// List of colors for vertices
		///</summary>
		protected List<Color32> VertexColors;
		///<summary>
		/// List of traingles in mesh
		///</summary>
		protected List<int> Triangles;
#else
		///<summary>
		/// list of vertices
		///</summary>
		protected List<UIVertex> vertices;
#endif


		///<summary>
		/// boundary of chart for event handling
		///</summary>
		protected Rect rect = new Rect(0, 0, 0, 0);

		[SerializeField]
		protected bool interactable = true;
		///<summary>
		/// flag indicates if the chart is interactive
		///</summary>
		public bool Interactable { 
			get { return interactable; }             
			set { interactable = value; Dirty = true; }
		}

		///<summary>
		/// flag indicate if the mouse cursor within the chart
		///</summary>
		protected bool isPointerInside = false;

		///<summary>
		/// Default color scheme for normal items
		///</summary>
		protected Color32[,] colors = new Color32[,] {
			{ new Color32(91, 24, 24, 255), 	new Color32(176, 46, 46, 255) },
			{ new Color32(124, 99, 21, 255), 	new Color32(222, 162, 19, 255) },
			{ new Color32(44, 100, 27, 255), 	new Color32(80, 176, 46, 255) },
			{ new Color32(25, 93, 92, 255), 	new Color32(54, 177, 195, 255) },
			{ new Color32(95, 95, 95, 255), 	new Color32(160, 160, 160, 255) },
			{ new Color32(122, 30, 91, 255), 	new Color32(196, 43, 108, 255) },
			{ new Color32(67, 24, 96, 255), 	new Color32(134, 47, 189, 255) },
			{ new Color32(22, 55, 89, 255), 	new Color32(54, 100, 195, 255) },
		};

		///<summary>
		/// Default color scheme for selected items
		///</summary>
		protected Color32[,] selectedColors = new Color32[,] {
			{ new Color32(255, 0, 0, 255),		new Color32(191, 40, 40, 255) },
			{ new Color32(255, 192, 0, 255),	new Color32(209, 150, 9, 255) },
			{ new Color32(60, 255, 0, 255),	 	new Color32(58, 167, 20, 255) },
			{ new Color32(0, 255, 252, 255),	new Color32(44, 169, 188, 255) },
			{ new Color32(222, 222, 222, 255),	new Color32(160, 160, 160, 255) },
			{ new Color32(255, 0, 168, 255),	new Color32(190, 36, 101, 255) },
			{ new Color32(150, 0, 255, 255),	new Color32(124, 28, 185, 255) },
			{ new Color32(0, 126, 255, 255),	new Color32(44, 90, 183, 255) },
		};
		protected int colorCount = 8;

		///<summary>
		/// Generate the chart
		///</summary>
		abstract protected void Create();

		///<summary>
		/// Set a color at given location for normal item
		///</summary>
		public void SetColor(int row, Color color1, Color color2 = default(Color))
		{
			if (color2 == default(Color))
			{
				color2 = color1;
			}
			row = row % colorCount;
			colors[row, 0] = color1;
			colors[row, 1] = color2;
		}

		///<summary>
		/// Set a color at given location for selected item
		///</summary>
		public void SetSelectedColor(int row, Color color1, Color color2 = default(Color))
		{
			if (color2 == default(Color))
			{
				color2 = color1;
			}
			row = row % colorCount;
			selectedColors[row, 0] = color1;
			selectedColors[row, 1] = color2;
		}

#if !PRE_UNITY_5_2

#if UNITY_5_2_0 || UNITY_5_2_1

		///<summary>
		/// UI Canvas invalidation (5.2.0-5.2.1)
		///</summary>
        protected override void OnPopulateMesh(Mesh m)
        {
			if (Dirty || rect != rectTransform.rect)
			{
				Create();
			}
			rect = rectTransform.rect;

			using (var vh = new VertexHelper())
			{
				for (int i = 0; i < Vertices.Count; i++)
				{
					vh.AddVert(Vertices[i], VertexColors[i], Vector2.zero);
				}
				for (int i = 0; i < Triangles.Count; i+=3)
				{
					vh.AddTriangle(Triangles[i], Triangles[i + 1], Triangles[i + 2]);
				}
				vh.FillMesh(m);
			}
		}

#else

		///<summary>
		/// UI Canvas invalidation (5.2.2-)
		///</summary>
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (Dirty || rect != rectTransform.rect)
			{
				Create();
			}
			rect = rectTransform.rect;

			vh.Clear();
			for (int i = 0; i < Vertices.Count; i++)
			{
            	vh.AddVert(Vertices[i], VertexColors[i], Vector2.zero);
			}
			for (int i = 0; i < Triangles.Count; i+=3)
			{
            	vh.AddTriangle(Triangles[i], Triangles[i + 1], Triangles[i + 2]);
			}
        }
#endif

#endif //!PRE_UNITY_5_2

#if PRE_UNITY_5_2
		///<summary>
		/// UI Canvas invalidation  (-5.1.4)
		///</summary>
        // [System.Obsolete("Use OnPopulateMesh instead.", true)]
        protected override void OnFillVBO(List<UIVertex> vbo)
        {
			if (Dirty || rect != rectTransform.rect)
			{
				Create();
			}
			rect = rectTransform.rect;

			vbo.Clear();
			vbo.AddRange(vertices);
        }
#endif

		///<summary>
		/// Refresh the chart if data set changed
		///</summary>
	  	protected virtual void Update()
		{
			if (Dirty)
			{
				SetVerticesDirty();
			}
		}

		protected float GetTickRange(float range, float tickCount)
		{
			float unroundedTickSize = range / (tickCount - 1);
			float x = Mathf.Ceil(Mathf.Log10(unroundedTickSize) - 1);
			float pow10x = Mathf.Pow(10, x);

			return Mathf.Ceil(unroundedTickSize / pow10x) * pow10x;
		}
	
	} // class

	public class LabelPosition
	{
		public Vector2 position = Vector2.zero;
		public float value = 0;
	} // class

} //namespace

