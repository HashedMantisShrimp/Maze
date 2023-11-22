using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace demo1
{
	public class DemoGrid : MonoBehaviour
	{
		static private DemoGrid instance;

		public static DemoGrid Instance { get => instance == null ? makeSingleton() : instance; }

		private void Awake()
		{
			instance = this;
			GenerateGrid();
		}

		static private DemoGrid makeSingleton()
		{
			instance = new DemoGrid();

			return instance;
		}

		public bool[,] walkableMap;
		[SerializeField] private float gridWidth;
		[SerializeField] private float gridHeight;
		[SerializeField] private float cellSize;
		[SerializeField] private GameObject tilePrefab;
		[SerializeField][Range(0f, 1f)] private float nonWalable;
		[SerializeField] float mapScale;

		private Vector2 cellSpacing = Vector2.zero;
		private Vector2 topLeft = Vector2.zero;
		private Texture2D mapSprite;

		private void destoyOldGrid()
		{
			for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
			{
				DestroyImmediate(transform.GetChild(i).gameObject);
			}
		}

		public void createGrid()
		{
			destoyOldGrid();

			topLeft = new Vector2(gameObject.transform.position.x - gridWidth / 2, gameObject.transform.position.y + gridHeight / 2);

			cellSpacing = new Vector2(cellSize, cellSize);

			int cellAmountX = (int)((gridWidth + 0.001f) / cellSpacing.x);
			int cellAmountY = (int)((gridHeight + 0.001f) / cellSpacing.y);
			walkableMap = new bool[cellAmountY, cellAmountX];

			demo2.NoiseGenerator noiseGenerator = new demo2.NoiseGenerator(true, 1 / mapScale);

			for (int y = 0; y < walkableMap.GetLength(0); y++)
			{
				float gridObjectYpos = topLeft.y - cellSpacing.y * y - cellSpacing.y / 2;

				for (int x = 0; x < walkableMap.GetLength(1); x++)
				{
					float gridObjectXPos = topLeft.x + cellSpacing.x * x + cellSpacing.x / 2;

					bool walkable = noiseGenerator.GetPerlinNoise(gridObjectXPos, gridObjectYpos) > nonWalable ? true : false;
					walkableMap[y, x] = walkable;
				}
			}
		}

		private void colorTile(int tileIndexX, int tileIndexY, Color color)
		{

			for (int y = 0; y < 100 * cellSize; y++)
			{
				for (int x = 0; x < 100 * cellSize; x++)
				{
					mapSprite.SetPixel(tileIndexX * Mathf.RoundToInt(cellSize * 100) + x, mapSprite.height - tileIndexY * Mathf.RoundToInt(cellSize * 100) - y, color);
				}
			}
		}

		private void generateMap()
		{
			int cellAmountX = (int)((gridWidth + 0.001f) / cellSpacing.x);
			int cellAmountY = (int)((gridHeight + 0.001f) / cellSpacing.y);

			mapSprite = new Texture2D((int)(gridWidth * 100), (int)(gridHeight * 100));

			for (int y = 0; y < walkableMap.GetLength(0); y++)
			{
				float gridObjectYpos = topLeft.y - cellSpacing.y * y - cellSpacing.y / 2;

				for (int x = 0; x < walkableMap.GetLength(1); x++)
				{
					Color tileColor = walkableMap[y, x] ? Color.white : Color.black;

					colorTile(x, y, tileColor);
				}
			}

			mapSprite.Apply();

			var sprite = Sprite.Create(mapSprite, new Rect(0, 0, gridWidth * 100, gridHeight * 100), new Vector2(0.5f, 0.5f));

			GetComponent<SpriteRenderer>().sprite = sprite;
		}

		public void GenerateGrid()
		{
			createGrid();
			generateMap();
		}

		public bool GetGridCell(int xCodrinate, int yCordinate)
		{
			return walkableMap[yCordinate, xCodrinate];
		}

		public bool GetGridCell(Vector3 worldPos)
		{
			cellSpacing = new Vector2(cellSize, cellSize);

			int xCord = Mathf.RoundToInt((worldPos.x - topLeft.x) / cellSpacing.x);
			int yCord = Mathf.RoundToInt((-worldPos.y + topLeft.y) / cellSpacing.y);

			return walkableMap[yCord, xCord];
		}

		public Vector3 cordinateToWorldSpace(int xCord, int yCord)
		{
			float worldPosX = topLeft.x + cellSpacing.x * xCord + cellSpacing.x / 2;
			float worldPosY = topLeft.y - cellSpacing.y * yCord - cellSpacing.y / 2;

			return new Vector3(worldPosX, worldPosY, 0);
		}

		public (int, int) worldSpaceToCordinate(Vector2 worldPos)
		{
			int xCord = Mathf.RoundToInt((worldPos.x - topLeft.x - cellSpacing.x / 2) / cellSpacing.x);
			int yCord = Mathf.RoundToInt((worldPos.y - topLeft.y + cellSpacing.y / 2) / -cellSpacing.y);

			return (xCord, yCord);
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireCube(transform.position, new Vector3(gridWidth, gridHeight, 0));
		}
	}
}

#if (UNITY_EDITOR)
namespace demo1
{
	[CustomEditor(typeof(demo1.DemoGrid))]
	public class DemoGrid2Editor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			demo1.DemoGrid DemoGrid2 = target as demo1.DemoGrid;

			if (GUILayout.Button("Generate Grid"))
			{
				DemoGrid2.GenerateGrid();
			}
		}
	}
}
#endif