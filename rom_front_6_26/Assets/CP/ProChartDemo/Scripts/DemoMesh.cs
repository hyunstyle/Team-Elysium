using UnityEngine;
using System.Collections;

using CP.ProChart;

///<summary>
/// Demo for Mesh Chart rendering
///</summary>
public class DemoMesh : MonoBehaviour
{
	///<summary>
	/// Reference for 2D bar chart
	///</summary>
	public BarChartMesh bar2d;

	///<summary>
	/// Reference for 3D bar chart
	///</summary>
	public BarChartMesh bar3d;

	///<summary>
	/// Reference for 2D curve chart
	///</summary>
	public LineChartMesh curve2d;

	///<summary>
	/// Reference for 3D curve chart
	///</summary>
	public LineChartMesh curve3d;

	///<summary>
	/// Reference for 2D line chart
	///</summary>
	public LineChartMesh line2d;

	///<summary>
	/// Reference for 3D line chart
	///</summary>
	public LineChartMesh line3d;

	///<summary>
	/// Reference for 2D pie chart
	///</summary>
	public PieChartMesh pie2d;

	///<summary>
	/// Reference for 3D bar chart
	///</summary>
	public PieChartMesh pie3d;

	///<summary>
	/// default thickness
	///</summary>
	float thickness = 0.00004f;

	///<summary>
	/// delta value to change thickness
	///</summary>
	float delta = 0.0003f;

	///<summary>
	/// 2D data set
	///</summary>
	ChartData2D dataSet;

	///<summary>
	/// 1D data set
	///</summary>
	ChartData1D dataSet2;

	///<summary>
	/// Use this for initialization
	///</summary>
	void Start ()
	{
		bar2d.SetColor(0, Color.green);
		bar2d.SetColor(1, Color.blue);
		bar2d.SetColor(2, Color.red);
		bar2d.SetColor(3, Color.yellow);
		bar2d.SetColor(4, Color.cyan);

		curve2d.SetColor(0, Color.green);
		curve2d.SetColor(1, Color.blue);
		curve2d.SetColor(2, Color.red);
		curve2d.SetColor(3, Color.yellow);
		curve2d.SetColor(4, Color.cyan);

		dataSet = new ChartData2D();

		dataSet[0, 0] = 9f;
		dataSet[0, 1] = 15.23f;
		dataSet[0, 2] = 3.33f;
		dataSet[1, 0] = 2.23f;
		dataSet[1, 1] = 5.23f;
		dataSet[1, 2] = 9.13f;
		dataSet[2, 0] = 7.23f;
		dataSet[2, 1] = 1.23f;
		dataSet[2, 2] = 5.23f;
		dataSet[3, 0] = 7.23f / 3;
		dataSet[3, 1] = 1.23f / 3;
		dataSet[3, 2] = 5.23f / 3;
		dataSet[4, 0] = 2+7.23f;
		dataSet[4, 1] = 2+1.23f;
		dataSet[4, 2] = 2+5.23f;
		bar2d.SetValues(ref dataSet);
		bar3d.SetValues(ref dataSet);

		dataSet2 = new ChartData1D();
		dataSet2[0] = 13f;
		dataSet2[1] = 13f;
		dataSet2[2] = 5.1f;
		dataSet2[3] = 8.1f;
		dataSet2[4] = 12.1f;
		pie2d.SetValues(ref dataSet2);
		pie3d.SetValues(ref dataSet2);

		line2d.SetValues(ref dataSet);
		line3d.SetValues(ref dataSet);
		curve2d.SetValues(ref dataSet);
		curve3d.SetValues(ref dataSet);
	}
	
	///<summary>
	/// Update orientation and params of chart
	///</summary>
	void Update ()
	{

		if (!dataSet2.isEmpty)
			dataSet2[0] = dataSet2[0] + 0.04f;

		thickness += delta;
		if (thickness > 0.03f || thickness <= Mathf.Abs(delta))
		{
			delta = -delta;
		}

		curve3d.Thickness = thickness;
		curve3d.PointSize = thickness + 0.03f;
		
		pie3d.transform.Rotate(0, 40 * Time.deltaTime, 0);
		curve3d.transform.Rotate(0, 40 * Time.deltaTime, 0);
		line3d.transform.Rotate(0, 40 * Time.deltaTime, 0);
		bar3d.transform.Rotate(0, 40 * Time.deltaTime, 0);
	}
}
