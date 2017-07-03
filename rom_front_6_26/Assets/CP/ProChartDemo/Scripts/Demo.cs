using UnityEngine;
using System.Collections;

using CP.ProChart;

///<summary>
/// Demo for Canvas charts
///</summary>
public class Demo : MonoBehaviour
{
	///<summary>
	/// Reference for bar chart
	///</summary>
	public BarChart barChart;

	///<summary>
	/// Reference for bar2 chart
	///</summary>
	public BarChart barChart2;

	///<summary>
	/// Reference for pie chart
	///</summary>
	public PieChart pieChart;

	///<summary>
	/// Reference for line chart
	///</summary>
	public LineChart lineChart;

	///<summary>
	/// Reference for line 2 chart
	///</summary>
	public LineChart lineChart2;
	
	///<summary>
	/// 2D test data set
	///</summary>
	ChartData2D dataSet;

	///<summary>
	/// 1D test data set
	///</summary>
	ChartData1D dataSet2;

	///<summary>
	/// Initialize and set up chart with test data
	///</summary>
	void OnEnable()
	{
		//create data set
		dataSet = new ChartData2D();

		//fill in values
		dataSet[0, 0] = 1;
		dataSet[0, 1] = 2;
		dataSet[0, 2] = 1.5f;
		dataSet[0, 3] = 1.5f;
		dataSet[0, 4] = 2.5f;
		dataSet[0, 5] = 3.1f;
		dataSet[0, 6] = 2.3f;
		dataSet[0, 7] = 0.5f;
		dataSet[0, 8] = 1.8f;
		dataSet[1, 0] = 2;
		dataSet[1, 1] = 1;
		dataSet[1, 2] = 2.5f;
		dataSet[1, 3] = 0.5f;
		dataSet[1, 4] = 1.4f;
		dataSet[1, 5] = 3.5f;
		dataSet[1, 6] = 2.1f;
		dataSet[1, 7] = 0.9f;
		dataSet[1, 8] = 2.2f;

		//bind same data set to charts
		barChart.SetValues(ref dataSet);
		barChart2.SetValues(ref dataSet);
		lineChart.SetValues(ref dataSet);
		lineChart2.SetValues(ref dataSet);

		//do it again for 1D data
		dataSet2 = new ChartData1D();
		dataSet2[0] = 3f;
		dataSet2[1] = 13f;
		dataSet2[2] = 5.1f;
		dataSet2[3] = 8.1f;
		dataSet2[4] = 2.1f;
		dataSet2[5] = 4.1f;
		dataSet2[6] = 5.1f;
		dataSet2[7] = 7.8f;
		
		//bind 1D data to pie chart
		pieChart.SetValues(ref dataSet2);
	}
}
