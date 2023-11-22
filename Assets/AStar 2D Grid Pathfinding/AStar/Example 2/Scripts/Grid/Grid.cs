using System;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

namespace demo2
{
	public class Grid<T>
	{
		private T[,] gridArray;
		private int gridWidth;
		private int gridHeight;

		public int Width { get => gridWidth; }
		public int Height { get => gridHeight; }

		public Grid()
		{

		}

		public Grid(int gridWidth, int gridHeight, T default_cell = default)
		{
			createGrid(gridWidth, gridHeight, default_cell);
		}

		public Grid(T[,] grid)
		{
			gridArray = grid;
			gridWidth = grid.GetLength(1);
			gridHeight = grid.GetLength(0);
		}

		public void createGrid(int gridWidth, int gridHeight, T default_cell = default)
		{
			this.gridWidth = gridWidth;
			this.gridHeight = gridHeight;

			gridArray = new T[gridHeight, gridWidth];

			if (default_cell == null)
			{
				return;
			}

			for (int y = 0; y < gridHeight; y++)
			{
				for (int x = 0; x < gridWidth; x++)
				{
					gridArray[y, x] = default_cell;
				}
			}
		}

		public bool isInBounds(int xCordinate, int yCordinate)
		{
			return Height > yCordinate && yCordinate >= 0 && Width > xCordinate && xCordinate >= 0;
		}

		public T getCell(int xCordinate, int yCordinate)
		{
			return gridArray[yCordinate, xCordinate];
		}

		public void setCell(int xCordinate, int yCordinate, T newValue)
		{
			gridArray[yCordinate, xCordinate] = newValue;
		}

		private void validateGridAccess(int xCordinate, int yCordinate)
		{
			if (gridArray == null)
			{
				throw new NullReferenceException($"Grid has not been created");
			}

			if (!isInBounds(xCordinate, yCordinate))
			{
				throw new ArgumentOutOfRangeException($"Cordinates out of bound, xCordinate: {xCordinate}, yCordinate: {yCordinate}, Grid y dimention: {Height}, Grid x dimention: {Width}");
			}
		}

		public T getCellSecure(int xCordinate, int yCordinate)
		{
			try
			{
				validateGridAccess(xCordinate, yCordinate);
			}
			catch (Exception e)
			{
				throw e;
			}

			return gridArray[yCordinate, xCordinate];
		}

		public void setCellSecure(int xCordinate, int yCordinate, T newValue)
		{
			try
			{
				validateGridAccess(xCordinate, yCordinate);
			}
			catch (Exception e)
			{
				throw e;
			}

			gridArray[yCordinate, xCordinate] = newValue;
		}

		public T[] getNeighbours(int xCordinate, int yCordinate, int range = 1)
		{
			return getNeighbours(xCordinate, yCordinate, range, range, range, range);
		}

		public T[] getNeighbours(int xCordinate, int yCordinate, int xRange = 1, int yRange = 1)
		{
			return getNeighbours(xCordinate, yCordinate, xRange, xRange, yRange, yRange);
		}

		public T[] getNeighbours(int xCordinate, int yCordinate, int xStartOffset = 1, int xEndOffset = 1, int yStartOffset = 1, int yEndOffset = 1)
		{
			List<T> neighbourCells = new List<T>();

			int yStart = (int)MathF.Max(0, yCordinate - yStartOffset);
			int yEnd = (int)MathF.Min(Height - 1, yCordinate + yEndOffset);

			int xStart = (int)MathF.Max(0, xCordinate - xStartOffset);
			int xEnd = (int)MathF.Min(Width - 1, xCordinate + xEndOffset);

			for (int y = yStart; y <= yEnd; y++)
			{
				for (int x = xStart; x <= xEnd; x++)
				{
					if (x == xCordinate && y == yCordinate)
					{
						continue;
					}

					neighbourCells.Add(getCell(x, y));
				}
			}

			return neighbourCells.ToArray();
		}

		public (T, (int, int))[] getNeighboursWithCodinate(int xCordinate, int yCordinate, int range = 1)
		{
			return getNeighboursWithCodinate(xCordinate, yCordinate, range, range, range, range);
		}

		public (T, (int, int))[] getNeighboursWithCodinate(int xCordinate, int yCordinate, int xRange = 1, int yRange = 1)
		{
			return getNeighboursWithCodinate(xCordinate, yCordinate, xRange, xRange, yRange, yRange);
		}

		public (T, (int, int))[] getNeighboursWithCodinate(int xCordinate, int yCordinate, int xStartOffset = 1, int xEndOffset = 1, int yStartOffset = 1, int yEndOffset = 1)
		{
			List<(T, (int, int))> neighbourCells = new List<(T, (int, int))>();

			int yStart = (int)MathF.Max(0, yCordinate - yStartOffset);
			int yEnd = (int)MathF.Min(Height - 1, yCordinate + yEndOffset);

			int xStart = (int)MathF.Max(0, xCordinate - xStartOffset);
			int xEnd = (int)MathF.Min(Width - 1, xCordinate + xEndOffset);

			for (int y = yStart; y <= yEnd; y++)
			{
				for (int x = xStart; x <= xEnd; x++)
				{
					if (x == xCordinate && y == yCordinate)
					{
						continue;
					}

					neighbourCells.Add((getCell(x, y), (x, y)));
				}
			}

			return neighbourCells.ToArray();
		}

		public IEnumerable<T> GetIterator()
		{
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					yield return gridArray[y, x];
				}
			}
		}

		public override string ToString()
		{
			string output = "[\n";

			for (int y = 0; y < Height; y++)
			{
				output += "[ ";

				for (int x = 0; x < Width; x++)
				{
					output += "" + getCell(x, y).ToString() + ", ";
				}

				output = output.Remove(output.Length - 2, 1);

				output += "]\n";
			}
			output += "]";

			return output;
		}
	}
}