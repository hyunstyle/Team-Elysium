using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using CP.ProChart;

///<summary>
/// Demo for Canvas based Pie chart
///</summary>
public class CanvasDemoPie : MonoBehaviour 
{
	///<summary>
	/// Reference for pie chart
	///</summary>
	public PieChart pieChart;

	///<summary>
	/// slider for inner radius
	///</summary>
	public Slider innerRadius;

	///<summary>
	/// slider for startAngle
	///</summary>
	public Slider startAngle;

	///<summary>
	/// slider for chartSize
	///</summary>
	public Slider chartSize;

	///<summary>
	/// input for innerRadius
	///</summary>
	public InputField innerRadiusInput;

	///<summary>
	/// input for startAngleInput
	///</summary>
	public InputField startAngleInput;

	///<summary>
	/// input for chartSizeInput
	///</summary>
	public InputField chartSizeInput;

	///<summary>
	/// Reference to Panel 
	///</summary>
	public GameObject dataPanel;

	///<summary>
	/// Text of data info
	///</summary>
	public Text infoData;

	///<summary>
	/// Slider for data value
	///</summary>	
	public Slider data;

	///<summary>
	/// Input for data value
	///</summary>
	public InputField dataInput;

	///<summary>
	/// Info text for data
	///</summary>
	public Text info;

	///<summary>
	/// Boundary of tooltip
	///</summary>
	public RectTransform tooltip;
	
	///<summary>
	/// Text for tooltip
	///</summary>
	public Text tooltipText;

	public Toggle labelToggle;
	public Slider labelPos;

	//label
	public GameObject label;

	///<summary>
	/// Data set for pie chart
	///</summary>
	private ChartData1D dataSet;

	private int column = -1;
	private int over = -1;

	private List<Text> labels = new List<Text>();
	
	///<summary>
	/// Handler for select a data
	///</summary>
	public void OnSelectDelegate(int column)
	{
		this.column = column;
		infoData.gameObject.SetActive(false);
		dataPanel.SetActive(true);
		data.value = dataSet[column];
		info.text = string.Format("Data[{0}]", column);
		dataInput.text = dataSet[column].ToString("0.00");
	}

	///<summary>
	/// Handler for select a data
	///</summary>
	public void OnOverDelegate(int column)
	{
		over = column;
	}

	///<summary>
	/// Initialize chart and data
	///</summary>
	void OnEnable()
	{
		innerRadius.value = pieChart.InnerRadius;
		startAngle.value = pieChart.StartAngle;
		chartSize.value = pieChart.ChartSize;

		dataSet = new ChartData1D();
		dataSet[0] = 50;
		dataSet[1] = 30;
		dataSet[2] = 70;
		dataSet[3] = 10;
		dataSet[4] = 90;

		pieChart.SetValues(ref dataSet);
		pieChart.onSelectDelegate += OnSelectDelegate;
		pieChart.onOverDelegate += OnOverDelegate;

		innerRadiusInput.text = pieChart.InnerRadius.ToString("0.00");
		startAngleInput.text = pieChart.StartAngle.ToString("0.00");
		chartSizeInput.text = pieChart.ChartSize.ToString("0.00");

		label.SetActive(false);

		for (int i = 0; i < dataSet.Columns; i++)
		{
			GameObject obj = (GameObject)Instantiate(label);
			obj.SetActive(labelToggle.isOn);
			obj.name = "Label" + i;
			obj.transform.SetParent(pieChart.transform, false);
			Text t = obj.GetComponentInChildren<Text>();
			labels.Add(t);
		}
	}

	///<summary>
	/// Remove handlers when object is disabled
	///</summary>
	void OnDisable()
	{
		pieChart.onSelectDelegate -= OnSelectDelegate;
		pieChart.onOverDelegate -= OnOverDelegate;
	}

	///<summary>
	/// Manage tooltips
	///</summary>
	void Update ()
	{
		tooltip.gameObject.SetActive(over != -1);
		if (over != -1)
		{
			tooltip.anchoredPosition = (Vector2)Input.mousePosition + tooltip.sizeDelta * tooltip.localScale.x / 2;
			tooltipText.text = string.Format("Data[{0}]\nValue: {1:F2}", over, dataSet[over]);
		}

		UpdateLabels();
	}

	///<summary>
	/// Handler for manage data change
	///</summary>
	public void OnValueChanged(string slider)
	{
		if (slider == "innerRadius")
		{
			pieChart.InnerRadius = innerRadius.value;
			innerRadiusInput.text = pieChart.InnerRadius.ToString("0.00");
		}
		else if (slider == "startAngle")
		{
			pieChart.StartAngle = startAngle.value;
			startAngleInput.text = pieChart.StartAngle.ToString("0.00");
		}
		else if (slider == "chartSize")
		{
			pieChart.ChartSize = chartSize.value;
			chartSizeInput.text = pieChart.ChartSize.ToString("0.00");
		}
		if (slider == "innerRadiusInput")
		{
			pieChart.InnerRadius = float.Parse(innerRadiusInput.text);
			innerRadius.value = pieChart.InnerRadius;
			innerRadiusInput.text = pieChart.InnerRadius.ToString("0.00");
		}
		else if (slider == "startAngleInput")
		{
			pieChart.StartAngle = float.Parse(startAngleInput.text);
			startAngle.value = pieChart.StartAngle;
			startAngleInput.text = pieChart.StartAngle.ToString("0.00");
		}
		else if (slider == "chartSizeInput")
		{
			pieChart.ChartSize = float.Parse(chartSizeInput.text);
			chartSize.value = pieChart.ChartSize;
			chartSizeInput.text = pieChart.ChartSize.ToString("0.00");
		}
		else if (slider == "data")
		{
			dataSet[column] = data.value;
			info.text = string.Format("Data[{0}]", column);
			dataInput.text = dataSet[column].ToString("0.00");
		}
		else if (slider == "dataInput")
		{
			dataSet[column] = Mathf.Clamp(float.Parse(dataInput.text), 0, 100);
			data.value = dataSet[column];
			dataInput.text = dataSet[column].ToString("0.00");
		}
	}

	///<summary>
	/// Update labels
	///</summary>
	private void UpdateLabels()
	{
		for (int i = 0; i < dataSet.Columns; i++)
		{
			labels[i].gameObject.SetActive(labelToggle.isOn);

			LabelPosition labelPosition = pieChart.GetLabelPosition(i, labelPos.value);
			if (labelPosition != null)
			{
				labels[i].rectTransform.anchoredPosition = labelPosition.position;
				labels[i].text = labelPosition.value.ToString("0.00");
			}
		}
		labelPos.interactable = labelToggle.isOn;
	}

} //class
