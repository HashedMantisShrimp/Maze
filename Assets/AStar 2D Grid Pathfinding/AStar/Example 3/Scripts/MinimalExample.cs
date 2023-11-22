using UnityEngine;
using AStar;

namespace demo3
{
	public class MinimalExample : MonoBehaviour
	{
		// This function generates a path on a boolean map and prints it to the console.
		// It first defines the walkable map, which is a 2D boolean array representing a simple grid.
		// It then calls the GeneratePath() method of the AStarPathfinding class to generate a path from (0,0) to (2,0) on the walkable map.
		// The resulting path is printed to the console using the printPath() function.
		private async void simpleBoolMap()
		{
			bool[,] walkableMap = {
		{true, false, true, true},
		{true, false, true, true},
		{true, false, true, true},
		{true, true, true, true}
		};

			(int, int)[] path = await AStarPathfinding.GeneratePath(0, 0, 2, 0, walkableMap);

			printPath(path);
		}

		// This function generates a path on a float map and prints it to the console.
		// It first defines the cost map, which is a 2D float array assigning a cost value to each cell on the map.
		// It then calls the GeneratePath() method of the AStarPathfinding class to generate a path from (0,0) to (2,0) on the cost map.
		// The resulting path is printed to the console using the printPath() function.
		private async void simpleFloatMap()
		{
			float[,] costMap = {
		{1, -1f, 1, 1},
		{1, -1f, 9, 1},
		{1, -1f, 9, 1},
		{1, 1, 1, 1}
		};

			(int, int)[] path = await AStarPathfinding.GeneratePath(0, 0, 2, 0, costMap);

			printPath(path);
		}

		// This function takes an array of (int, int) tuples representing the coordinates of each step in a path.
		// It converts the array to a string and prints it to the console.
		private void printPath((int, int)[] path)
		{
			string output = "[";

			foreach ((int, int) cordinate in path)
			{
				output += $"({cordinate.Item1}, {cordinate.Item2}), ";
			}

			output = output.Remove(output.Length - 2, 2);
			output += "]";

			Debug.Log(output);
		}

		// This function is called when the script is started.
		// It runs both simpleBoolMap() and simpleFloatMap() in sequence.
		private void Start()
		{
			simpleBoolMap();
			simpleFloatMap();
		}
	}
}