using UnityEditor;
using UnityEngine;

namespace demo2
{
	public class GridCell
	{
		public int x, y;
		private GameObject tileObject;
		private bool walkable;
		private float tileCost;
		private SpriteRenderer spriteRenderer;

		public GridCell()
		{
		}

		public GridCell(int x, int y, float tileCost = 1f, GameObject tileObject = null)
		{
			this.x = x;
			this.y = y;
			this.tileCost = tileCost;
			setTileObject(tileObject);
			setWalkable(tileCost != -1f);
		}

		private void updateColor()
		{
			if (spriteRenderer == null)
			{
				return;
			}

			spriteRenderer.color = walkable ? Color.Lerp(Color.white, Color.black, tileCost / 10) : Color.black;
		}

		public void setTileObject(GameObject tileObject)
		{
			this.tileObject = tileObject;

			if (tileObject == null)
			{
				return;
			}

			tileObject.TryGetComponent<SpriteRenderer>(out spriteRenderer);
			updateColor();
		}

		public GameObject getTileObject()
		{
			return tileObject;
		}

		public void setWalkable(bool walkable)
		{
			this.walkable = walkable;
			updateColor();
		}

		public bool getWalkable()
		{
			return walkable;
		}

		public void setTileCost(float tileCost)
		{
			this.tileCost = tileCost;
			setWalkable(tileCost != -1f);
		}

		public float getTileCost()
		{
			return tileCost;
		}
	}

	public class GridManager : MonoBehaviour
	{
		static private GridManager instance;

		public static GridManager Instance { get => instance == null ? makeSingleton() : instance; }

		private void Awake()
		{
			instance = this;
		}

		static private GridManager makeSingleton()
		{
			instance = new GridManager();

			return instance;
		}

		public int Width { get => grid == null ? -1 : grid.Width; }
		public int Height { get => grid == null ? -1 : grid.Height; }

		[SerializeField] private float gridWidth;
		[SerializeField] private float gridHeight;
		[SerializeField] private float cellSize;
		[SerializeField] private GameObject tilePrefab;
		[SerializeField] private Vector2 tileSpacing;
		[SerializeField][Range(0f, 1f)] private float nonWalable;
		[SerializeField] float mapScale;

		private Vector2 cellSpacing = Vector2.zero;
		private Vector2 topLeft = Vector2.zero;
		Grid<GridCell> grid;

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

			if (grid == null)
			{
				grid = new Grid<GridCell>();
			}

			topLeft = new Vector2(gameObject.transform.position.x - gridWidth / 2, gameObject.transform.position.y + gridHeight / 2);

			cellSpacing = new Vector2(cellSize + tileSpacing.x, cellSize + tileSpacing.y);

			int cellAmountX = (int)((gridWidth + tileSpacing.x + 0.001f) / cellSpacing.x);
			int cellAmountY = (int)((gridHeight + tileSpacing.y + 0.001f) / cellSpacing.y);

			grid.createGrid(cellAmountX, cellAmountY);

			for (int y = 0; y < Height; y++)
			{
				float gridObjectYpos = topLeft.y - cellSpacing.y * y - cellSpacing.y / 2;

				for (int x = 0; x < Width; x++)
				{
					float gridObjectXPos = topLeft.x + cellSpacing.x * x + cellSpacing.x / 2;
					GameObject gridObject = Instantiate(tilePrefab, new Vector2(gridObjectXPos, gridObjectYpos), Quaternion.identity, gameObject.transform);
					gridObject.transform.localScale *= cellSize;
					grid.setCellSecure(x, y, new GridCell(x, y, 1f, gridObject));
				}
			}
		}

		public void generateMap()
		{
			NoiseGenerator noiseGenerator = new NoiseGenerator(true, 1 / mapScale);

			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					Vector2 gridCellPos = grid.getCell(x, y).getTileObject().transform.position;
					float tileCost = noiseGenerator.GetPerlinNoise(gridCellPos.x, gridCellPos.y);
					tileCost = tileCost > nonWalable ? tileCost *= 10 : -1f;
					grid.getCellSecure(x, y).setTileCost(tileCost);
				}
			}
		}

		public void GenerateGrid()
		{
			createGrid();
			generateMap();
			Debug.Log($"Grid generation done, gird height is: {Height}, grid width is: {Width}, total numbers of cells is: {Height * Width}");
		}

		public GridCell GetGridCell(int xCodrinate, int yCordinate)
		{
			return grid.getCell(xCodrinate, yCordinate);
		}

		public GridCell GetGridCell(Vector3 worldPos)
		{
			int xCord = Mathf.RoundToInt((worldPos.x - topLeft.x - cellSpacing.x / 2) / cellSpacing.x);
			int yCord = Mathf.RoundToInt((worldPos.y - topLeft.y + cellSpacing.y / 2) / -cellSpacing.y);

			if (!grid.isInBounds(xCord, yCord))
			{
				return null;
			}

			return GetGridCell(xCord, yCord);
		}

		private void Start()
		{
			GenerateGrid();
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireCube(transform.position, new Vector3(gridWidth, gridHeight, 0));
		}
	}
}

#if (UNITY_EDITOR)
namespace demo2
{
	[CustomEditor(typeof(GridManager))]
	public class GridManagerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			GridManager gridManager = target as GridManager;

			if (GUILayout.Button("Generate Grid"))
			{
				gridManager.GenerateGrid();
			}
		}
	}
}
#endif