using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace demo1
{
	public class AntSpawner : MonoBehaviour
	{
		[SerializeField] private GameObject antPrefab;
		[SerializeField] private int spawnAmount;

		private (int, int) findFreeGridPos()
		{
			bool[,] walkableMap = DemoGrid.Instance.walkableMap;
			int maxTry = 1000;

			for (int i = 0; i < maxTry; i++)
			{
				int yCord = Random.Range(0, walkableMap.GetLength(0));
				int xCord = Random.Range(0, walkableMap.GetLength(1));

				if (walkableMap[yCord, xCord])
				{
					return (xCord, yCord);
				}
			}

			return (-1, -1);
		}

		private void spawnAnts(int xCord, int yCord)
		{
			Vector3 spawnPos = DemoGrid.Instance.cordinateToWorldSpace(xCord, yCord);

			for (int i = 0; i < spawnAmount; i++)
			{
				Instantiate(antPrefab, spawnPos, Quaternion.identity, gameObject.transform);
			}
		}

		// Start is called before the first frame update
		void Start()
		{
			(int, int) spawCord = findFreeGridPos();

			if (spawCord == (-1, -1)) return;

			spawnAnts(spawCord.Item1, spawCord.Item2);
		}
	}
}