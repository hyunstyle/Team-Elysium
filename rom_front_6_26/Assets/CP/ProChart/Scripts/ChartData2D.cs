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
	/// Abstract base class for data set
	/// Each data set must implement this class
	///</summary>
	abstract public class ChartData
	{
		///<summary>
		/// Delegate to indicate changes in data set
		///</summary>
		public delegate void OnDataChangeDelegate();
		public OnDataChangeDelegate onDataChangeDelegate;

		///<summary>
		/// List of data as 2D data set
		///</summary>
		private List<List<float>> values = new List<List<float>>();
		///<summary>
		/// number of rows
		///</summary>
		private int sizeY = 0;
		///<summary>
		/// number of columns
		///</summary>
		private int sizeX = 0;

		///<summary>
		/// flag to indicate if data set is empty
		///</summary>
		public bool isEmpty	{ get { return (sizeX == 0 || sizeY == 0); } }
		///<summary>
		/// get number of rows in data set
		///</summary>
		public int Rows 	{ get { return sizeY; } }
		///<summary>
		/// get number of columns in data set
		///</summary>
		public int Columns	{ get { return sizeX; } }

		///<summary>
		/// Clear the data set
		///</summary>
		public void Clear()
		{
			values.Clear();
			sizeX = 0;
			sizeY = 0;
		}

		///<summary>
		/// Resize the data set to new dimensions
		///</summary>
		public void Resize(int rows, int columns)
		{

			int newSizeX = (columns > sizeX) ? columns : sizeX;
			int newSizeY = (rows > sizeY) ? rows : sizeY;

			for (int i = sizeY; i < newSizeY; i++)
			{
				List<float> newRow = new List<float>();
				values.Add(newRow);
			}

			for (int i = 0; i < newSizeY; i++)
			{
				for (int j = values[i].Count; j < newSizeX; j++)
				{
					values[i].Add(0);
				}
			}

			sizeY = values.Count;
			sizeX = values[0].Count;
		}

		///<summary>
		/// Set one data at given location by value
		///</summary>
		protected void SetInternal(int row, int column, float value)
		{
			if (sizeY <= row || sizeX <= column)
			{
				Resize(row + 1, column + 1);
			}
			values[row][column] = value;

			if (onDataChangeDelegate != null)
			{
				onDataChangeDelegate();
			}
		}

		///<summary>
		/// Get a data at given location
		///</summary>
		protected float GetInternal(int row, int column)
		{
			if (row < sizeY && column < sizeX)
			{
				return values[row][column];
			}
			Debug.LogWarning("Invalid index!!");
			return 0;
		}
	}

	///<summary>
	/// 2D Data set
	///</summary>
	public class ChartData2D : ChartData
	{
		///<summary>
		/// Get or set data with operator[row, column]
		///</summary>
		public float this[int row, int column]
		{
		    get
		    {
		    	return GetInternal(row, column);
		    }
		    set
		    {
		    	SetInternal(row, column, value);
		    }
		}
	}

	///<summary>
	/// 2D Data set
	///</summary>
	public class ChartData1D : ChartData
	{
		///<summary>
		/// Get or set data with operator[column]
		///</summary>
		public float this[int column]
		{
		    get
		    {
		    	return GetInternal(0, column);
		    }
		    set
		    {
		    	SetInternal(0, column, value);
		    }
		}
	}

} //namespace


