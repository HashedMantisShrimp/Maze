using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using AStar;

namespace demo2
{
	public class PathFindTest : MonoBehaviour
	{
		List<(SpriteRenderer, Color)> old_colors = new List<(SpriteRenderer, Color)>();

		private void clearDrawnPath()
		{
			int oldColorCount = old_colors.Count;
			for (int i = oldColorCount - 1; i >= 0; i--)
			{
				(SpriteRenderer, Color) item = old_colors[i];
				if (item.Item1 != null)
				{
					item.Item1.color = item.Item2;
				}

				old_colors.RemoveAt(i);
			}
		}

		private void drawPath((int, int)[] path)
		{
			for (int i = 0; i < path.Length; i++)
			{
				int x = path[i].Item1;
				int y = path[i].Item2;
				GameObject tileObject = GridManager.Instance.GetGridCell(x, y).getTileObject();
				SpriteRenderer spriteRenderer;
				tileObject.TryGetComponent<SpriteRenderer>(out spriteRenderer);

				if (spriteRenderer != null)
				{
					old_colors.Add((spriteRenderer, spriteRenderer.color));

					spriteRenderer.color = Color.green;
				}
			}
		}

		private async void MakePath(int startX, int startY, int goalX, int goalY)
		{
			clearDrawnPath();

			// Generates a walkable and a cost map
			int gridWidth = GridManager.Instance.Width;
			int gridHeight = GridManager.Instance.Height;

			bool[,] walkableMap = new bool[gridHeight, gridWidth];
			float[,] costMap = new float[gridHeight, gridWidth];

			for (int y = 0; y < gridHeight; y++)
			{
				for (int x = 0; x < gridWidth; x++)
				{
					walkableMap[y, x] = GridManager.Instance.GetGridCell(x, y).getWalkable();
					costMap[y, x] = GridManager.Instance.GetGridCell(x, y).getTileCost();
				}
			}


			Stopwatch stopwatch = Stopwatch.StartNew();

			//TODO finish this comment
			// Using the A* asset 
			(int, int)[] path;
			if (useCostMap)
			{
				path = await AStarPathfinding.GeneratePath(startX, startY, goalX, goalY, costMap, manhattanDistance);
			}
			else
			{
				path = await AStarPathfinding.GeneratePath(startX, startY, goalX, goalY, walkableMap, manhattanDistance);
			}

			stopwatch.Stop();

			UnityEngine.Debug.Log("Path len is: " + path.Length + ", It took " + stopwatch.ElapsedMilliseconds + "ms to generate");

			drawPath(path);
		}

		private GridCell firstClick;
		private GridCell secondClick;
		private Color oldColorFirst;
		private Color oldColorSecond;

		private void ClickToDrawPath()
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (firstClick == null)
				{
					Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					firstClick = GridManager.Instance.GetGridCell(mouseWorldPos);

					if (firstClick != null)
					{
						oldColorFirst = firstClick.getTileObject().GetComponent<SpriteRenderer>().color;
						firstClick.getTileObject().GetComponent<SpriteRenderer>().color = Color.cyan;
					}
				}
				else if (secondClick == null)
				{
					Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					secondClick = GridManager.Instance.GetGridCell(mouseWorldPos);

					if (secondClick != null)
					{
						MakePath(firstClick.x, firstClick.y, secondClick.x, secondClick.y);
						oldColorSecond = secondClick.getTileObject().GetComponent<SpriteRenderer>().color;
						secondClick.getTileObject().GetComponent<SpriteRenderer>().color = Color.red;


						if (firstClick.getTileObject() != null)
						{
							firstClick.getTileObject().GetComponent<SpriteRenderer>().color = oldColorFirst;
							secondClick.getTileObject().GetComponent<SpriteRenderer>().color = oldColorSecond;
						}

						firstClick = null;
						secondClick = null;
					}
				}
			}

			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (firstClick != null)
				{
					firstClick.getTileObject().GetComponent<SpriteRenderer>().color = oldColorFirst;
				}
				if (secondClick != null)
				{
					secondClick.getTileObject().GetComponent<SpriteRenderer>().color = oldColorSecond;
				}
				firstClick = null;
				secondClick = null;
			}
		}

		public bool ClickPath;
		public bool manhattanDistance = true;
		public bool useCostMap = false;

		// Update is called once per frame
		void Update()
		{
			if (ClickPath)
			{
				ClickToDrawPath();
			}
		}
	}
}