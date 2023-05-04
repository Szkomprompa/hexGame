using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    public int chunkCountX = 4, chunkCountZ = 3;
    
    public int typeOfMap = 0;

    int cellCountX, cellCountZ;

    public HexCell cellPrefab;

    HexCell[] cells;

    public Text cellLabelPrefab;

    //Canvas gridCanvas;

    // HexMesh hexMesh;

    [HideInInspector]
    public Color[] colors;

    public HexGridChunk chunkPrefab;

    HexGridChunk[] chunks;

    void Awake()
    {
        SetColors();
        //gridCanvas = GetComponentInChildren<Canvas>();
        //hexMesh = GetComponentInChildren<HexMesh>();

        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        CreateChunks();
        CreateCells();

        //GenerateLands(); Generate lands (random, continents, small amount of water) !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    }

    /*void Start()
    {
        hexMesh.Triangulate(cells);
    }*/

    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        //cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.name = cellPrefab.name + " " + "( " + cell.coordinates.X + ", " + cell.coordinates.Y + ", " + cell.coordinates.Z + ")"; // ZMIENIC NA POPRAWNE
        GenerateColor(cell);
        //cell.Color = colors[DrawColorRandom()];
        //making each tile water                            !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                if (x < cellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }

        Text label = Instantiate<Text>(cellLabelPrefab);
        //label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition =
            new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
        cell.uiRect = label.rectTransform;

        AddCellToChunk(x, z, cell);
    }

    public void ColorCell(Vector3 position, Color color)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        Debug.Log("touched at " + coordinates.ToString());
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        HexCell cell = cells[index];
        cell.Color = color;
        //hexMesh.Triangulate(cells);
    }

    int DrawColorRandom()
    {
        float rand = Random.Range(0.0f, 1.0f);
        if (rand < 0.4f)
        {
            return 0;
        } else if (rand < 0.7f)
        {
            return 1;
        } else if (rand < 0.8f)
        {
            return 2;
        } else 
        { 
            return 3;
        }
    }

    void GenerateColor(HexCell cell)
    {
        if (typeOfMap == 0)
        {
            cell.Color = colors[DrawColorRandom()];
        } else if (typeOfMap == 1)
        {
            cell.Color = colors[0];
        }
    }

    public void SetColors()
    {
        colors = new Color[4];
        colors[0] = new Color(0f,0f,1f,1f);
        colors[1] = new Color(0.1f, 0.7f, 0f, 1f);
        colors[2] = new Color(0.3f, 0.3f, 0.3f, 1f);
        colors[3] = new Color(0f, 0.4f, 0f, 1f);
    }

    void CreateCells()
    {
        cells = new HexCell[cellCountZ * cellCountX];

        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                Debug.Log(i);
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
                chunk.name = "Chunk " + "( " + x + ", " + z + ")";

                if (x > 0)
                {
                    Debug.Log(i + ", " + x + ", " + z);
                    chunk.SetNeighbor( 0, chunks[i - 2]);
                }
                if (z > 0)
                {
                    chunk.SetNeighbor( 1, chunks[i - chunkCountX - 1]);
                }
            }
        }
    }

    void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }
}
