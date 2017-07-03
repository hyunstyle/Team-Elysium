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

	public class BarChartMesh : ChartMesh
	{
		///<summary>
		/// 2D data set associated with this chart
		///</summary>
		private ChartData2D values = null;
		
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
		/// Enable the chart and create test data to give visuals in editor
		///</summary>
		void OnEnable()
		{
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

			if (values == null || values.isEmpty)
			{
				return;
			}

			float w = size.x / (float)(values.Columns * values.Rows + (values.Columns - 1));
			float gap = w * spacing;
			w = (size.x - gap * (float)(values.Columns - 1)) / (float)(values.Columns * values.Rows);

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

			float d = (max != min) ? max - min : 1;
			float x0 = -size.x / 2;
			float y0 = -size.y / 2;

			float z = mode_3d ? -size.z / 2 : 0.0f;

			if (values.Columns > 0)
			{
				for (int i = 0; i < values.Columns; i++)
				{
					Vector2 p1 = Vector2.zero;
					Vector2 p2 = Vector2.zero;

					for (int j = 0; j < values.Rows; j++)
					{
						Color32 colorTop = colors[j % colorCount, 0];
						Color32 colorBottom = colors[j % colorCount, 1];

						p1.x = x0 + (float)((j + i * values.Rows) * w + (i * gap)) + w * (1 - thickness) / 2;
						p2.x = p1.x + w * thickness;
						
						if (max < 0)
						{
							p1.y = size.y / 2;
							p2.y = p1.y - values[j, i] / min * size.y;
						}
						else if (min < 0)
						{
							p1.y = y0 + ((Mathf.Abs(min) + values[j, i]) / d) * size.y;
							p2.y = y0 + (Mathf.Abs(min) / d) * size.y;
						}
						else
						{
							p1.y = y0 + (values[j, i] / max) * size.y;
							p2.y = y0;
						}

						if(min < 0 && max > 0 && values[j,i] < 0 )
						{
							float x = p1.x;
							p1.x = p2.x;
							p2.x = x;						
							AddTriangle(new Vector3(p1.x, p1.y, z), new Vector3(p2.x, p1.y, z), new Vector3(p1.x, p2.y, z), colorTop, colorTop, colorBottom);
							AddTriangle(new Vector3(p2.x, p1.y, z), new Vector3(p2.x, p2.y, z), new Vector3(p1.x, p2.y, z), colorTop, colorBottom, colorBottom);
						}
						else
						{
							AddTriangle(new Vector3(p2.x, p1.y, z), new Vector3(p1.x, p1.y, z), new Vector3(p1.x, p2.y, z), colorTop, colorTop, colorBottom);
							AddTriangle(new Vector3(p2.x, p2.y, z), new Vector3(p2.x, p1.y, z), new Vector3(p1.x, p2.y, z), colorBottom, colorTop, colorBottom);
						}

						if (mode_3d)
						{
							float z1 = size.z + z;
							if(min < 0 && max > 0 && values[j,i] < 0)
							{
								AddTriangle(new Vector3(p1.x, p1.y, z1), new Vector3(p1.x, p2.y, z1), new Vector3(p2.x, p1.y, z1), colorTop, colorBottom, colorTop);
								AddTriangle(new Vector3(p2.x, p1.y, z1), new Vector3(p1.x, p2.y, z1), new Vector3(p2.x, p2.y, z1), colorTop, colorBottom, colorBottom);

								AddTriangle(new Vector3(p2.x, p1.y, z1), new Vector3(p2.x, p2.y, z), new Vector3(p2.x, p1.y, z), colorTop, colorBottom, colorTop);
								AddTriangle(new Vector3(p2.x, p2.y, z1), new Vector3(p2.x, p2.y, z), new Vector3(p2.x, p1.y, z1), colorBottom, colorBottom, colorTop);

								AddTriangle(new Vector3(p1.x, p1.y, z1), new Vector3(p1.x, p1.y, z), new Vector3(p1.x, p2.y, z), colorTop, colorTop, colorBottom);
								AddTriangle(new Vector3(p1.x, p2.y, z1), new Vector3(p1.x, p1.y, z1), new Vector3(p1.x, p2.y, z), colorBottom, colorTop, colorBottom);

								AddTriangle(new Vector3(p2.x, p1.y, z1), new Vector3(p1.x, p1.y, z), new Vector3(p1.x, p1.y, z1), colorTop, colorTop, colorTop);
								AddTriangle(new Vector3(p2.x, p1.y, z1), new Vector3(p2.x, p1.y, z), new Vector3(p1.x, p1.y, z), colorTop, colorTop, colorTop);

								AddTriangle(new Vector3(p2.x, p2.y, z1), new Vector3(p1.x, p2.y, z1), new Vector3(p1.x, p2.y, z), colorBottom, colorBottom, colorBottom);
								AddTriangle(new Vector3(p2.x, p2.y, z1), new Vector3(p1.x, p2.y, z), new Vector3(p2.x, p2.y, z), colorBottom, colorBottom, colorBottom);
							}
							else
							{
								AddTriangle(new Vector3(p1.x, p2.y, z1), new Vector3(p1.x, p1.y, z1), new Vector3(p2.x, p1.y, z1), colorBottom, colorTop, colorTop);
								AddTriangle(new Vector3(p1.x, p2.y, z1), new Vector3(p2.x, p1.y, z1), new Vector3(p2.x, p2.y, z1), colorBottom, colorTop, colorBottom);

								AddTriangle(new Vector3(p2.x, p2.y, z), new Vector3(p2.x, p1.y, z1), new Vector3(p2.x, p1.y, z), colorBottom, colorTop, colorTop);
								AddTriangle(new Vector3(p2.x, p2.y, z), new Vector3(p2.x, p2.y, z1), new Vector3(p2.x, p1.y, z1), colorBottom, colorBottom, colorTop);

								AddTriangle(new Vector3(p1.x, p1.y, z), new Vector3(p1.x, p1.y, z1), new Vector3(p1.x, p2.y, z), colorTop, colorTop, colorBottom);
								AddTriangle(new Vector3(p1.x, p1.y, z1), new Vector3(p1.x, p2.y, z1), new Vector3(p1.x, p2.y, z), colorTop, colorBottom, colorBottom);

								AddTriangle(new Vector3(p1.x, p1.y, z), new Vector3(p2.x, p1.y, z1), new Vector3(p1.x, p1.y, z1), colorTop, colorTop, colorTop);
								AddTriangle(new Vector3(p2.x, p1.y, z), new Vector3(p2.x, p1.y, z1), new Vector3(p1.x, p1.y, z), colorTop, colorTop, colorTop);

								AddTriangle(new Vector3(p1.x, p2.y, z1), new Vector3(p2.x, p2.y, z1), new Vector3(p1.x, p2.y, z), colorBottom, colorBottom, colorBottom);
								AddTriangle(new Vector3(p1.x, p2.y, z), new Vector3(p2.x, p2.y, z1), new Vector3(p2.x, p2.y, z), colorBottom, colorBottom, colorBottom);
							}
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
			meshFilter.sharedMesh.name = "Bar chart";
			meshFilter.sharedMesh.vertices = Vertices.ToArray();
			meshFilter.sharedMesh.triangles = Triangles.ToArray();
			meshFilter.sharedMesh.colors32 = VertexColors.ToArray();

			MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
			Shader shader = Shader.Find("CreativePudding/VertexColor");
			if (renderer.sharedMaterial == null)
			{
				renderer.sharedMaterial = new Material(shader);
			}
			else
			{
				renderer.sharedMaterial.shader = shader;
			}
		}

		///<summary>
		/// Add triangle with colors
		///</summary>
		void AddTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Color32 color1, Color32 color2, Color32 color3)
		{
			Vertices.Add(p1);
			Vertices.Add(p2);
			Vertices.Add(p3);
			VertexColors.Add(color1);
			VertexColors.Add(color2);
			VertexColors.Add(color3);
			Triangles.Add(Vertices.Count - 3);
			Triangles.Add(Vertices.Count - 1);
			Triangles.Add(Vertices.Count - 2);
		}
	}

} //namespace


