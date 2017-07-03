using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using CP.ProChart;

///<summary>
/// Demo for canvas based bar and line chart using 2D data set
///</summary>
public class CanvasDemo : MonoBehaviour 
{
	//bar chart datas
	public BarChart barChart;
	public Slider spacing;
	public Slider barThickness;
	public InputField distanceInput;
	public InputField marginInput;
	public Toggle barLabelToggle;
	public Slider barLabelPos;

	//line chart datas
	public LineChart lineChart;
	public Slider thickness;
	public Slider pointSize;
	public InputField thicknessInput;
	public InputField pointSizeInput;
	public Toggle lineLabelToggle;
	
	//data value change items
	public GameObject dataPanel;
	public Text infoData;
	public Slider data;
	public InputField dataInput;
	public Text info;

	//tooltip items
	public RectTransform tooltip;
	public Text tooltipText;

	//labels
	public GameObject labelBar;
	public GameObject labelLine;
	public GameObject axisXLabel;
	public GameObject axisYLabel;

	//2D Data set
	private ChartData2D dataSet;

	//selection of data
	private int row = -1;
	private int column = -1;
	private int overRow = -1;
	private int overColumn = -1;

	private List<Text> barLabels = new List<Text>();
	private List<Text> barXLabels = new List<Text>();
	private List<Text> barYLabels = new List<Text>();
	private List<Text> lineLabels = new List<Text>();
	private List<Text> lineXLabels = new List<Text>();
	private List<Text> lineYLabels = new List<Text>();

	///<summary>
	/// Manage selection of data to be able to change it
	///</summary>
	public void OnSelectDelegate(int row, int column)
	{
		this.row = row;
		this.column = column;
		infoData.gameObject.SetActive(false);
		dataPanel.SetActive(true);
		data.value = dataSet[row, column];
		info.text = string.Format("Data[{0},{1}]", row, column);
		dataInput.text = dataSet[row, column].ToString();
	}

	///<summary>
	/// Manage over state of chart
	///</summary>
	public void OnOverDelegate(int row, int column)
	{
		overRow = row;
		overColumn = column;
	}

	///<summary>
	/// Initialize data set and charts
	///</summary>
	void OnEnable() 
	{
		spacing.value = barChart.Spacing;
		barThickness.value = barChart.Thickness;

		thickness.value = lineChart.Thickness;
		pointSize.value = lineChart.PointSize;

		dataSet = new ChartData2D();
		dataSet[0, 0] = 50;
		dataSet[0, 1] = 30;
		dataSet[0, 2] = 70;
		dataSet[0, 3] = 10;
		dataSet[0, 4] = 90;
		dataSet[1, 0] = 40;
		dataSet[1, 1] = 25;
		dataSet[1, 2] = 53;
		dataSet[1, 3] = 12;
		dataSet[1, 4] = 37;
		dataSet[2, 0] = 68;
		dataSet[2, 1] = 91;
		dataSet[2, 2] = 30;
		dataSet[2, 3] = 44;
		dataSet[2, 4] = 63;

		barChart.SetValues(ref dataSet);
		lineChart.SetValues(ref dataSet);

		barChart.onSelectDelegate += OnSelectDelegate;
		barChart.onOverDelegate += OnOverDelegate;
		lineChart.onSelectDelegate += OnSelectDelegate;
		lineChart.onOverDelegate += OnOverDelegate;
	
		distanceInput.text = barChart.Spacing.ToString("0.00");
		marginInput.text = barThickness.value.ToString("0.00");
		thicknessInput.text = thickness.value.ToString("0.00");
		pointSizeInput.text = pointSize.value.ToString("0.00");

		labelBar.SetActive(false);
		labelLine.SetActive(false);
		axisXLabel.SetActive(false);
		axisYLabel.SetActive(false);

		barLabels.Clear();
		lineLabels.Clear();

		for (int i = 0; i < dataSet.Rows; i++)
		{
			for (int j = 0; j < dataSet.Columns; j++)
			{
				GameObject obj = (GameObject)Instantiate(labelBar);
				obj.SetActive(barLabelToggle.isOn);
				obj.name = "Label" + i + "_" + j;
				obj.transform.SetParent(barChart.transform, false);
				Text t = obj.GetComponentInChildren<Text>();
				barLabels.Add(t);

				obj = (GameObject)Instantiate(labelLine);
				obj.SetActive(lineLabelToggle.isOn);
				obj.name = "Label" + i + "_" + j;
				obj.transform.SetParent(lineChart.transform, false);
				t = obj.GetComponent<Text>();
				lineLabels.Add(t);
			}
		}

		barXLabels.Clear();
		lineXLabels.Clear();

		for (int i = 0; i < dataSet.Columns; i++)
		{
			GameObject obj = (GameObject)Instantiate(axisXLabel);
			obj.SetActive(barLabelToggle.isOn);
			obj.name = "Label" + i;
			obj.transform.SetParent(barChart.transform, false);
			Text t = obj.GetComponent<Text>();
			t.text = t.gameObject.name;
			barXLabels.Add(t);

			obj = (GameObject)Instantiate(axisXLabel);
			obj.SetActive(lineLabelToggle.isOn);
			obj.name = "Label" + i;
			obj.transform.SetParent(lineChart.transform, false);
			t = obj.GetComponent<Text>();
			t.text = t.gameObject.name;
			lineXLabels.Add(t);
		}

		barYLabels.Clear();
		lineYLabels.Clear();

		for (int i = 0; i < dataSet.Columns; i++)
		{
			GameObject obj = (GameObject)Instantiate(axisYLabel);
			obj.SetActive(barLabelToggle.isOn);
			obj.name = "Label" + i;
			obj.transform.SetParent(barChart.transform, false);
			Text t = obj.GetComponent<Text>();
			t.text = t.gameObject.name;
			barYLabels.Add(t);

			obj = (GameObject)Instantiate(axisYLabel);
			obj.SetActive(lineLabelToggle.isOn);
			obj.name = "Label" + i;
			obj.transform.SetParent(lineChart.transform, false);
			t = obj.GetComponent<Text>();
			t.text = t.gameObject.name;
			lineYLabels.Add(t);
		}
	}

	///<summary>
	/// Remove hanlders when object is disabled
	///</summary>
	void OnDisable()
	{
		barChart.onSelectDelegate -= OnSelectDelegate;
		barChart.onOverDelegate -= OnOverDelegate;
		lineChart.onSelectDelegate -= OnSelectDelegate;
		lineChart.onOverDelegate -= OnOverDelegate;
	}

	///<summary>
	/// manage tooltip
	///</summary>
	void Update ()
	{
		tooltip.gameObject.SetActive(overRow != -1);
		if (overRow != -1)
		{
			tooltip.anchoredPosition = (Vector2)Input.mousePosition + tooltip.sizeDelta * tooltip.localScale.x / 2;
			tooltipText.text = string.Format("Data[{0},{1}]\nValue: {2:F2}", overRow, overColumn, dataSet[overRow, overColumn]);
		}

		UpdateLabels();	
	}

	///<summary>
	/// Change the type of chart with click on buttons
	///</summary>
	public void OnClick(string button)
	{
		if (button == "curve")
		{
			lineChart.Chart = LineChart.ChartType.CURVE;
		}
		else if (button == "line")
		{
			lineChart.Chart = LineChart.ChartType.LINE;
		}
		else if (button == "none")
		{
			lineChart.Point = LineChart.PointType.NONE;
		}
		else if (button == "rectangle")
		{
			lineChart.Point = LineChart.PointType.RECTANGLE;
		}
		else if (button == "circle")
		{
			lineChart.Point = LineChart.PointType.CIRCLE;
		}
		else if (button == "triangle")
		{
			lineChart.Point = LineChart.PointType.TRIANGLE;
		}
	}
	
	///<summary>
	/// Update values when slider moved
	///</summary>
	public void OnValueChanged(string slider)
	{
		if (slider == "distance")
		{
			barChart.Spacing = spacing.value;
			distanceInput.text = barChart.Spacing.ToString("0.00");
		}
		else if (slider == "distanceInput")
		{
			barChart.Spacing = float.Parse(distanceInput.text);
			spacing.value = barChart.Spacing;
			distanceInput.text = spacing.value.ToString("0.00");
		}		
		else if (slider == "margin")
		{
			barChart.Thickness = barThickness.value;
			marginInput.text = barThickness.value.ToString("0.00");
		}
		else if (slider == "marginInput")
		{
			barChart.Thickness = float.Parse(marginInput.text);
			barThickness.value = barChart.Thickness;
			marginInput.text = barThickness.value.ToString("0.00");
		}		
		else if (slider == "thickness")
		{
			lineChart.Thickness = thickness.value;
			thicknessInput.text = thickness.value.ToString("0.00");
		}
		else if (slider == "thicknessInput")
		{
			lineChart.Thickness = float.Parse(thicknessInput.text);
			thickness.value = lineChart.Thickness;
			thicknessInput.text = thickness.value.ToString("0.00");
		}
		else if (slider == "pointSize")
		{
			lineChart.PointSize = pointSize.value;
			pointSizeInput.text = pointSize.value.ToString("0.00");
		}
		else if (slider == "pointSizeInput")
		{
			lineChart.PointSize = float.Parse(pointSizeInput.text);
			pointSize.value = lineChart.PointSize;
			pointSizeInput.text = pointSize.value.ToString("0.00");
		}
		else if (slider == "data")
		{
			dataSet[row, column] = data.value;
			info.text = string.Format("Data[{0},{1}]", row, column);
			dataInput.text = dataSet[row, column].ToString("0.00");
		}
		else if (slider == "dataInput")
		{
			dataSet[row, column] = Mathf.Clamp(float.Parse(dataInput.text), -100, 100);
			data.value = dataSet[row, column];
			dataInput.text = dataSet[row, column].ToString();
		}
	}

	///<summary>
	/// Update labels
	///</summary>
	public void UpdateLabels()
	{
		for (int i = 0; i < dataSet.Rows; i++)
		{
			for (int j = 0; j < dataSet.Columns; j++)
			{
				LabelPosition labelPos = barChart.GetLabelPosition(i, j, barLabelPos.value);
				if (labelPos != null)
				{
					barLabels[i * dataSet.Columns + j].transform.parent.gameObject.SetActive(barLabelToggle.isOn);
					barLabels[i * dataSet.Columns + j].text = labelPos.value.ToString("0.00");
					barLabels[i * dataSet.Columns + j].transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition = labelPos.position;
				}

				labelPos = lineChart.GetLabelPosition(i, j);
				if (labelPos != null)
				{
					lineLabels[i * dataSet.Columns + j].gameObject.SetActive(lineLabelToggle.isOn);
					lineLabels[i * dataSet.Columns + j].text = labelPos.value.ToString("0.00");
					lineLabels[i * dataSet.Columns + j].rectTransform.anchoredPosition = labelPos.position;
				}
			}
		}
		barLabelPos.interactable = barLabelToggle.isOn;
		
		LabelPosition[] positions = barChart.GetAxisXPositions();
		if (positions != null)
		{
			for (int i = 0; i < positions.Length; i++)
			{
				barXLabels[i].gameObject.SetActive(barLabelToggle.isOn);
				barXLabels[i].GetComponent<RectTransform>().anchoredPosition = positions[i].position;
			}
		}

		positions = barChart.GetAxisYPositions();
		if (positions != null)
		{
			for (int i = 0; i < 5; i++)
			{
				if (positions.Length - 1 < i)
				{
					barYLabels[i].gameObject.SetActive(false);
				}
				else
				{
					barYLabels[i].gameObject.SetActive(barLabelToggle.isOn);
					barYLabels[i].text = positions[i].value.ToString("0.0");
					barYLabels[i].GetComponent<RectTransform>().anchoredPosition = positions[i].position;
				}
			}
		}

		positions = lineChart.GetAxisXPositions();
		if (positions != null)
		{
			for (int i = 0; i < positions.Length; i++)
			{
				lineXLabels[i].gameObject.SetActive(lineLabelToggle.isOn);
				lineXLabels[i].GetComponent<RectTransform>().anchoredPosition = positions[i].position;
			}
		}

		positions = lineChart.GetAxisYPositions();
		if (positions != null)
		{
			for (int i = 0; i < 5; i++)
			{
				if (positions.Length - 1 < i)
				{
					lineYLabels[i].gameObject.SetActive(false);
				}
				else
				{
					lineYLabels[i].gameObject.SetActive(lineLabelToggle.isOn);
					lineYLabels[i].text = positions[i].value.ToString("0.0");
					lineYLabels[i].GetComponent<RectTransform>().anchoredPosition = positions[i].position;
				}
			}
		}
	}

} //class
