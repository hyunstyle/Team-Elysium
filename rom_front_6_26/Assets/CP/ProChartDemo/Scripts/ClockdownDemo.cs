using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using CP.ProChart;

///<summary>
/// Clock down demo for pie chart using multiple charts with 1 data set
///</summary>
public class ClockdownDemo : MonoBehaviour {

	//references to canvas based pie charts
	public PieChart pie_canvas_1;
	public PieChart pie_canvas_2;
	public PieChart pie_canvas_3;

	//references to mesh based pie charts
	public PieChartMesh pie2d_1;
	public PieChartMesh pie2d_2;
	public PieChartMesh pie2d_3;
	public PieChartMesh pie3d_1;
	public PieChartMesh pie3d_2;
	public PieChartMesh pie3d_3;

	//text to show
	public Text text;

	//data set
	private ChartData1D dataSet;

	//time of start/end process
	private int startTime;	
	private int endTime;

	///<summary>
	/// Initialize demo
	///</summary>
	void Start ()
	{
		dataSet = new ChartData1D();
		
		InitTimer();

		pie_canvas_1.SetValues(ref dataSet);
		pie_canvas_2.SetValues(ref dataSet);
		pie_canvas_3.SetValues(ref dataSet);
		pie2d_1.SetValues(ref dataSet);
		pie2d_2.SetValues(ref dataSet);
		pie2d_3.SetValues(ref dataSet);
		pie3d_1.SetValues(ref dataSet);
		pie3d_2.SetValues(ref dataSet);
		pie3d_3.SetValues(ref dataSet);
	}
	
	///<summary>
	/// Update data set, so each charts will show the same change
	///</summary>
	void Update ()
	{
		int tickCount = System.Environment.TickCount;
		dataSet[0] = tickCount - startTime;
		dataSet[1] = endTime - dataSet[0];

		text.text = (dataSet[1] / 1000.0f).ToString("0.0");

		if (dataSet[1] <= 0)
		{
			InitTimer();
		}
	}

	///<summary>
	/// Initialize timer
	///</summary>
	void InitTimer()
	{
		int tickCount = System.Environment.TickCount;
		startTime = tickCount;
		endTime = 3000;
		
		dataSet[0] = startTime - tickCount;
		dataSet[1] = endTime - startTime;
	}
}
