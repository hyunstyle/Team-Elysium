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
	/// Abstract base class for Mesh renderer.
	/// Each charts should implement this base class, 
	/// which requires mesh rendering.
	///</summary>
	[RequireComponent (typeof(MeshRenderer))]
	[RequireComponent (typeof(MeshFilter))]
	[ExecuteInEditMode]
	public abstract class ChartMesh : MonoBehaviour
	{
		///<summary>
		/// Boundary of the chart
		///</summary>
		public Vector3 size = new Vector3(1, 1, 1);
		///<summary>
		/// flag indicate if chart renders in 2D or 3D mode
		///</summary>
		public bool mode_3d = false;

		///<summary>
		/// flag indicate if data set changed
		///</summary>
		public bool Dirty	{ set; get; }

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
		
		///<summary>
		/// Default color scheme
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
		protected int colorCount = 8;
		
		///<summary>
		/// Refresh if data changed
		///</summary>
		void Update()
		{
			if (Dirty)
			{
				Create();
			}
		}

		///<summary>
		/// Generate chart
		///</summary>
		abstract protected void Create();

		///<summary>
		/// Set a color at given location
		///</summary>
		public void SetColor(int row, Color color1, Color color2 = default(Color))
		{
			if (color2 == default(Color))
			{
				color2 = color1;
			}
			row = row % colorCount;
			colors[row % colorCount, 0] = color1;
			colors[row % colorCount, 1] = color2;
		}

		///<summary>
		/// Set dirty flag to force invalidation
		///</summary>
		public void OnDataChangeDelegate()
		{
			Dirty = true;
		}

		///<summary>
		/// Draw gizmos
		///</summary>
		void OnDrawGizmos()
		{
			Vector3 scale = transform.lossyScale;
			Vector3 pos = transform.position;
			Quaternion rot = transform.rotation;

			float sx = size.x / 2;
			float sy = size.y / 2;
			float sz = mode_3d ? (size.z / 2) : 0;
		
			Vector3 s1 = pos + rot * Vector3.Scale(new Vector3(-sx, sy, -sz), scale);
			Vector3 s2 = pos + rot * Vector3.Scale(new Vector3(sx, sy, -sz), scale);
			Vector3 s3 = pos + rot * Vector3.Scale(new Vector3(sx, -sy, -sz), scale);
			Vector3 s4 = pos + rot * Vector3.Scale(new Vector3(-sx, -sy, -sz), scale);
			Gizmos.color = Color.grey;

			if(mode_3d == false)
			{
				Gizmos.DrawLine(s1, s2);
				Gizmos.DrawLine(s2, s3);
				Gizmos.DrawLine(s3, s4);
				Gizmos.DrawLine(s4, s1);
			}
			else
			{
				Vector3 s5 = pos + rot * Vector3.Scale(new Vector3(-sx, sy, sz), scale);
				Vector3 s6 = pos + rot * Vector3.Scale(new Vector3(sx, sy, sz), scale);
				Vector3 s7 = pos + rot * Vector3.Scale(new Vector3(sx, -sy, sz), scale);
				Vector3 s8 = pos + rot * Vector3.Scale(new Vector3(-sx, -sy, sz), scale);
				
				Gizmos.DrawLine(s1, s2);
				Gizmos.DrawLine(s2, s3);
				Gizmos.DrawLine(s3, s4);
				Gizmos.DrawLine(s4, s1);

				Gizmos.DrawLine(s5, s6);
				Gizmos.DrawLine(s6, s7);
				Gizmos.DrawLine(s7, s8);
				Gizmos.DrawLine(s8, s5);

				Gizmos.DrawLine(s1, s5);
				Gizmos.DrawLine(s2, s6);
				Gizmos.DrawLine(s3, s7);
				Gizmos.DrawLine(s4, s8);
			}
		}
	}

} //namespace