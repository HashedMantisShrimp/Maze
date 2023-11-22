using UnityEngine;
using AStar;

namespace demo1
{
	public class randomPather : MonoBehaviour
	{
		public float speed = 5f;

		private int currentX, currentY;
		private (int, int)[] path = new (int, int)[0];
		int path_index = 0;
		Vector3 target;

		bool pathfinding = false;

		private async void getPath()
		{
			if (pathfinding) return;

			pathfinding = true;

			bool[,] walkableMap = DemoGrid.Instance.walkableMap;

			for (int i = 0; i < 10; i++)
			{
				int randomY = Random.Range(0, walkableMap.GetLength(0) - 1);
				int randomX = Random.Range(0, walkableMap.GetLength(1) - 1);

				if (!walkableMap[randomY, randomX])
				{
					continue;
				}

				path = await AStarPathfinding.GeneratePath(currentX, currentY, randomX, randomY, walkableMap);

				if (path.Length != 0)
				{
					path_index = 0;
					target = DemoGrid.Instance.cordinateToWorldSpace(path[path_index].Item1, path[path_index].Item2);
					break;
				}
			}

			pathfinding = false;
		}

		// Start is called before the first frame update
		void Start()
		{
			(int, int) currentCord = DemoGrid.Instance.worldSpaceToCordinate(transform.position);
			currentX = currentCord.Item1;
			currentY = currentCord.Item2;
		}

		// Update is called once per frame
		void Update()
		{
			if (path.Length != 0)
			{
				if (Vector3.Distance(target, transform.position) < 0.1f)
				{
					if (path_index >= path.Length - 1)
					{
						getPath();
					}
					else
					{
						currentX = path[path_index].Item1;
						currentY = path[path_index].Item2;
						path_index++;
					}

					target = DemoGrid.Instance.cordinateToWorldSpace(path[path_index].Item1, path[path_index].Item2);
				}

				transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
			}
			else
			{
				getPath();
			}
		}
	}
}