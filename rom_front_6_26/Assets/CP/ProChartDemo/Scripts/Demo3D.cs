using UnityEngine;
using System.Collections;

using CP.ProChart;

///<summary>
/// Demo for 3D Mesh Chart rendering
///</summary>
public class Demo3D : MonoBehaviour
{
	///<summary>
	/// Charts holder game object
	///</summary>
	public GameObject charts;
	
	///<summary>
	/// Reference for bar chart
	///</summary>
	public BarChartMesh bar;
	///<summary>
	/// Reference for line chart
	///</summary>
	public LineChartMesh line;
	///<summary>
	/// Reference for curve chart
	///</summary>
	public LineChartMesh curve;
	///<summary>
	/// Reference for pie chart
	///</summary>
	public PieChartMesh pie;
	///<summary>
	/// Reference for doughnut chart
	///</summary>
	public PieChartMesh doughnut;
	///<summary>
	/// Reference for half doughnut chart
	///</summary>
	public PieChartMesh halfDoughnut;

	private ChartData1D dataSet;
	private ChartData2D dataSet2;

	private float velocity = 0;

	///<summary>
	/// activate bar chart as first and disable the rest
	///</summary>
	void OnEnable()
	{
		bar.gameObject.SetActive(true);
		line.gameObject.SetActive(false);
		curve.gameObject.SetActive(false);
		pie.gameObject.SetActive(false);
		doughnut.gameObject.SetActive(false);
		halfDoughnut.gameObject.SetActive(false);
	}

	///<summary>
	/// Initialize charts with test data
	///</summary>
	void Start()
	{
		dataSet = new ChartData1D();
		dataSet[0] = 50;
		dataSet[1] = 30;
		dataSet[2] = 70;
		dataSet[3] = 10;
		dataSet[4] = 90;

		pie.SetValues(ref dataSet);
		doughnut.SetValues(ref dataSet);
		halfDoughnut.SetValues(ref dataSet);

		dataSet2 = new ChartData2D();
		dataSet2[0, 0] = 50;
		dataSet2[0, 1] = 30;
		dataSet2[0, 2] = 70;
		dataSet2[0, 3] = 10;
		dataSet2[0, 4] = 90;
		dataSet2[1, 0] = 40;
		dataSet2[1, 1] = 25;
		dataSet2[1, 2] = 53;
		dataSet2[1, 3] = 12;
		dataSet2[1, 4] = 37;
		dataSet2[2, 0] = 68;
		dataSet2[2, 1] = 91;
		dataSet2[2, 2] = 30;
		dataSet2[2, 3] = 44;
		dataSet2[2, 4] = 63;

		bar.SetValues(ref dataSet2);
		line.SetValues(ref dataSet2);
		curve.SetValues(ref dataSet2);
	}
	
	///<summary>
	/// Update Chart orientation and params based on mouse movements
	///</summary>
	void Update ()
	{
		if(Input.GetMouseButton(0)) 
		{
			velocity -= Input.GetAxis("Mouse X") / 5.0f;
		}
		else 
		{
			if (Mathf.Abs(velocity) < 0.8f)
			{
				velocity = Mathf.Lerp(velocity, 0.8f, Time.deltaTime);
			}
		}

		charts.transform.localRotation = Quaternion.AngleAxis(charts.transform.localRotation.eulerAngles.y + velocity, Vector3.up);
		velocity = Mathf.Lerp(velocity, 0, Time.deltaTime * 3);
	}

	///<summary>
	/// Click on a chart types to activate the right version
	///</summary>
	public void OnClick(string button)
	{
		bar.gameObject.SetActive(false);
		line.gameObject.SetActive(false);
		curve.gameObject.SetActive(false);
		pie.gameObject.SetActive(false);
		doughnut.gameObject.SetActive(false);
		halfDoughnut.gameObject.SetActive(false);

		switch(button)
		{
			case "bar":
				bar.gameObject.SetActive(true);
				break;

			case "line":
				line.gameObject.SetActive(true);
				break;

			case "curve":
				curve.gameObject.SetActive(true);
				break;

			case "pie":
				pie.gameObject.SetActive(true);
				break;

			case "doughnut":
				doughnut.gameObject.SetActive(true);
				break;

			case "halfDoughnut":
				halfDoughnut.gameObject.SetActive(true);
				break;
		}
	}
} // class
