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
	/// Mesh renderer version of PieChart
	/// This class respnosible to render the Pie chart to 2D or 3D mesh
	///</summary>
	public class PieChartMesh : ChartMesh
	{
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
		///degree of zero position in clock-wise direction, float in range of 0f to 360f, where 0f = top (12 hr)
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
		/// Enable the chart and create test data to give visuals in editor
		///</summary>
		void OnEnable()
		{
			//only in editor
			if (!Application.isPlaying)
			{
				ChartData1D testValues = new ChartData1D();
				testValues[0] = 10.85f;
				testValues[1] = -90.2f;
				testValues[2] = 35;
				testValues[3] = 40.85f;
				testValues[4] = 60.0f;
				SetValues(ref testValues);
			}
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

		///<summary>
		/// Generate the chart
		///</summary>
		protected override void Create()
		{
			Vertices = new List<Vector3>();
			VertexColors = new List<Color32>();
			Triangles = new List<int>();

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

				Color32 color1 = colors[i % colorCount, 1];
				Color32 color2 = colors[i % colorCount, 0];

				int j = Mathf.CeilToInt(angle / 5);
				Quaternion step = Quaternion.identity;
				if (j != 0)
				{
					step = Quaternion.Euler(0, 0, -Mathf.Sign(chartSize) * angle / (float)j);
				}

				int start = Vertices.Count;
				for (; j > 0; j--)
				{
					Vector3 pp1 = step * pp0;
					Vector3 pp1i = step * pp0i;

					AddQuad(pp0i, pp0, pp1, pp1i, color1, color2);

					pp0 = pp1;
					pp0i = pp1i;
				}
				int cnt = Vertices.Count - start;

				if (mode_3d && step != Quaternion.identity)
				{
					for (int k = 0; k < cnt; k++)
					{
						Vertices.Add(new Vector3(Vertices[start + k].x, Vertices[start + k].y, -Vertices[start + k].z));
					}
					for (int k = 0; k < cnt; k++)
					{
						VertexColors.Add(VertexColors[start + k]);
					}
					int quads = cnt / 4;
					for (int k = 0; k < quads; k++)
					{
						Triangles.Add(start + cnt + k * 4);
						Triangles.Add(start + cnt + k * 4 + 2);
						Triangles.Add(start + cnt + k * 4 + 1);

						Triangles.Add(start + cnt + k * 4 + 2);
						Triangles.Add(start + cnt + k * 4 + 0);
						Triangles.Add(start + cnt + k * 4 + 3);
					}

					for (int k = 0; k < quads; k++)
					{
						int offset = start + k * 4;
						Triangles.Add(offset + 1);
						Triangles.Add(offset + cnt + 2);
						Triangles.Add(offset + 2);

						Triangles.Add(offset + cnt + 1);
						Triangles.Add(offset + cnt + 2);
						Triangles.Add(offset + 1);

						Triangles.Add(offset);
						Triangles.Add(offset + 3);
						Triangles.Add(offset + cnt + 3);

						Triangles.Add(offset + cnt);
						Triangles.Add(offset);
						Triangles.Add(offset + cnt + 3);
					}
					
					Triangles.Add(start + 1);
					Triangles.Add(start);
					Triangles.Add(start + cnt + 1);

					Triangles.Add(start + cnt + 1);
					Triangles.Add(start);
					Triangles.Add(start + cnt);

					Triangles.Add(start + (quads - 1) * 4 + 2);
					Triangles.Add(start + (quads - 1) * 4 + cnt + 2);
					Triangles.Add(start + (quads - 1) * 4 + 3);

					Triangles.Add(start + (quads - 1) * 4 + cnt + 3);
					Triangles.Add(start + (quads - 1) * 4 + 3);
					Triangles.Add(start + (quads - 1) * 4 + cnt + 2);
				}
			}

			if (chartSize < 0)
			{
				for (int i = 0; i < Triangles.Count; i+=3)
				{
					int j = Triangles[i];
					Triangles[i] = Triangles[i + 1];
					Triangles[i + 1] = j;
				}
			}

			MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
			if (meshFilter.sharedMesh == null)
			{
				meshFilter.sharedMesh = new Mesh();
			}
			meshFilter.sharedMesh.Clear();
			meshFilter.sharedMesh.name = "Pie chart";
			meshFilter.sharedMesh.vertices = Vertices.ToArray();
			meshFilter.sharedMesh.triangles = Triangles.ToArray();
			meshFilter.sharedMesh.colors32 = VertexColors.ToArray();
			meshFilter.sharedMesh.RecalculateNormals();

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
		/// Add quad
		///</summary>
		void AddQuad(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, Color32 color1, Color32 color2)
		{
			float z = mode_3d ? -size.z / 2 : 0.0f;
			Vertices.Add(new Vector3(p0.x * size.x, p0.y * size.y, z));
			Vertices.Add(new Vector3(p1.x * size.x, p1.y * size.y, z));
			Vertices.Add(new Vector3(p2.x * size.x, p2.y * size.y, z));
			Vertices.Add(new Vector3(p3.x * size.x, p3.y * size.y, z));
			VertexColors.Add(color1);
			VertexColors.Add(color2);
			VertexColors.Add(color2);
			VertexColors.Add(color1);
			Triangles.Add(Vertices.Count - 4);
			Triangles.Add(Vertices.Count - 3);
			Triangles.Add(Vertices.Count - 2);
			Triangles.Add(Vertices.Count - 4);
			Triangles.Add(Vertices.Count - 2);
			Triangles.Add(Vertices.Count - 1);
		}

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
	}

} //namespace

