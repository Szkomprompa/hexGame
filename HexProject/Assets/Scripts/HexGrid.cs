using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HexGrid : MonoBehaviour
{
    public int chunkCountX = 4, chunkCountZ = 3;
    
    public int typeOfMap = 0;

    int cellCountX, cellCountZ;

    public HexCell cellPrefab;

    HexCell[] cells;

    public Text cellLabelPrefab;

    public float noiseScale = 20.0f;
    public float xNoiseOffset = 0f;
    public float zNoiseOffset = 0f;

    [HideInInspector]
    public Color[] colors;

    public HexGridChunk chunkPrefab;

    HexGridChunk[] chunks;

    void Awake()
    {
        SetOffset();

        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        CreateChunks();
        CreateCells();

        //GenerateLands(); Generate lands (random, continents, small amount of water) !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    }

    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.name = cellPrefab.name + " " + "( " + cell.coordinates.X + ", " + cell.coordinates.Y + ", " + cell.coordinates.Z + ")"; // ZMIENIC NA POPRAWNE
        cell.SetType(NoiseCellType(CalculateNoise(x,z)));

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
        label.rectTransform.anchoredPosition =
            new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
        cell.uiRect = label.rectTransform;

        AddCellToChunk(x, z, cell);
    }

    public void ColorCell(Vector3 position, HexType type) // color)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        Debug.Log("touched at " + coordinates.ToString());
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        HexCell cell = cells[index];
        cell.SetType(type);
    }

    HexType NoiseCellType(float perlinNoise)
    {
        if (perlinNoise < 0.4f)
        {
            return HexType.WATER;
        }
        else if (perlinNoise < 0.7f)
        {
            return HexType.PLAINS;
        }
        else if (perlinNoise < 0.8f)
        {
            return HexType.MOUNTAINS;
        }
        else
        {
            return HexType.WOODS;
        }
    }

    float CalculateNoise(int x, int z)
    {
        float xCord = (float)x / cellCountX * noiseScale + xNoiseOffset;
        float zCord = (float)z / cellCountZ * noiseScale + zNoiseOffset;

        float perlinValue = Mathf.PerlinNoise(xCord, zCord);
        return perlinValue;
    }

    void SetOffset()
    {
        xNoiseOffset = Random.Range(0f, 999999f);
        zNoiseOffset = Random.Range(0f, 999999f);
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
