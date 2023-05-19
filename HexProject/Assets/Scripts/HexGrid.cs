using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class HexGrid : MonoBehaviour
{
    public int chunkCountX = 4, chunkCountZ = 3;
    
    public int typeOfMap = 0;

    int cellCountX, cellCountZ;

    public HexCell cellPrefab;

    HexCell[] cells;

    public Text cellLabelPrefab;

    [HideInInspector]
    public Color[] colors;

    public HexGridChunk chunkPrefab;

    public bool randomSeed = true;
    public int seed = 20;
    public float noiseScale = 1.0f;
    public int octaves = 3;
    [Range(0, 1)]
    public float persistance = 0.5f;
    public float lacunarity = 2.0f;
    public int offsetX, offsetZ;

    float[,] noise;

    HexGridChunk[] chunks;

    HexCellPriorityQueue priorityQueue;

    void Awake()
    {
        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;
        Vector2 offset = new Vector2 (offsetX, offsetZ);

        if(randomSeed)
        {
            seed = Random.Range(0, 999999);
        }
        noise = Noise.GenerateNoiseMap(cellCountX, cellCountZ, seed, noiseScale,octaves, persistance, lacunarity, offset);

        CreateChunks();
        CreateCells();
    }

    void CreateCell(int x, int z, int i, PerlinNoise heightNoise, PerlinNoise forestNoise)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.name = cellPrefab.name + " " + "( " + cell.coordinates.X + ", " + cell.coordinates.Y + ", " + cell.coordinates.Z + ")"; // ZMIENIC NA POPRAWNE

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        //label.text = cell.coordinates.ToStringOnSeparateLines();
        cell.uiRect = label.rectTransform;

        cell.SetType(HexTypeControler(noise[x, z]));
        /*cell.SetType(HexTypeControler(heightNoise.CalculateNoise(x, z, cellCountX * chunkCountX, cellCountZ * chunkCountZ)));
        if (cell.type == HexType.PLAINS && forestNoise.CalculateNoise(x, z, cellCountX * chunkCountX, cellCountZ * chunkCountZ) > 0.65f)
        {
            cell.SetType(HexType.WOODS);
        }
        */
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

        AddCellToChunk(x, z, cell);
    }

    public HexCell GetCell(Vector3 position) // color)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        Debug.Log("touched at " + coordinates.ToString());
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        return cells[index];
    }


    public void EditCell(HexCell cell, HexType type)
    {
        cell.SetType(type);
        cell.chunk.Refresh();
    }

    HexType HexTypeControler(float perlinNoise)
    {
        switch (typeOfMap)
        {
            case 0:
                return Classic(perlinNoise);
            case 1:
                return LowWaterLevel(perlinNoise);
            default:
                return HexType.WATER;
        }
    }

    HexType Classic(float perlinNoise)
    {
        if (perlinNoise < 0.5f)
        {
            return HexType.WATER;
        }
        else if (perlinNoise < 0.75f)
        {
            return HexType.PLAINS;
        }
        else if (perlinNoise < 0.9f)
        {
            return HexType.HILL;
        }
        else
        {
            return HexType.MOUNTAINS;
        }
    }

    HexType LowWaterLevel(float perlinNoise)
    {
        if (perlinNoise < 0.2f)
        {
            return HexType.WATER;
        }
        else if (perlinNoise < 0.75f)
        {
            return HexType.PLAINS;
        }
        else if (perlinNoise < 0.9f)
        {
            return HexType.HILL;
        }
        else
        {
            return HexType.MOUNTAINS;
        }
    }

    void CreateCells()
    {
        cells = new HexCell[cellCountZ * cellCountX];
        PerlinNoise heightNoise = new PerlinNoise();
        heightNoise.SetRandomOffset();
        heightNoise.SetScale(noiseScale);
        PerlinNoise forestNoise = new PerlinNoise();
        forestNoise.SetRandomOffset();
        forestNoise.SetScale(noiseScale);

        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++, heightNoise, forestNoise);
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
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
                chunk.name = "Chunk " + "( " + x + ", " + z + ")";

                if (x > 0)
                {
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

    public void FindPath(HexCell fromCell, HexCell toCell, int speed)
    {
        Search(fromCell, toCell, speed);
    }

    void Search(HexCell fromCell, HexCell toCell, int speed)
    {
        if (priorityQueue == null)
        {
            priorityQueue = new HexCellPriorityQueue();
        }
        else
        {
            priorityQueue.Clear();
        }
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Distance = int.MaxValue;
            cells[i].SetLabel(null);
            cells[i].DisableHighlight();
        }
        fromCell.EnableHighlight(new Color(1f, 1f, 0f, 1f));
        //toCell.EnableHighlight(Color.red);
        fromCell.Distance = 0;
        priorityQueue.Enqueue(fromCell);
        while (priorityQueue.Count > 0)
        {
            HexCell current = priorityQueue.Dequeue();

            if (current == toCell)
            {
                //current = current.Parent;
                while (current != fromCell)
                {
                    int turn = current.Distance / speed;
                    if (turn * speed == current.Distance)
                    {
                        turn -= 1;
                    }
                    current.SetLabel(turn.ToString());
                    current.EnableHighlight(Color.white);
                    current = current.Parent;
                }
                toCell.EnableHighlight(Color.red);
                break;
            }

            int currentTurn = current.Distance / speed;
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor == null)
                {
                    continue;
                }
                if (neighbor.type == HexType.WATER)
                {
                    continue;
                }
                int distance = current.Distance + neighbor.movementCost;
                int turn = distance / speed;
                if (turn * speed == distance)
                {
                    turn -= 1;
                }
                if (neighbor.Distance == int.MaxValue)
                {
                    neighbor.Distance = distance;
                    //neighbor.SetLabel(turn.ToString());
                    neighbor.Parent = current;
                    neighbor.SearchHeuristic = neighbor.coordinates.DistanceTo(toCell.coordinates);
                    priorityQueue.Enqueue(neighbor);
                }
                else if (distance < neighbor.Distance)
                {
                    int oldPriority = neighbor.SearchPriority;
                    neighbor.Distance = distance;
                    //neighbor.SetLabel(turn.ToString());
                    neighbor.Parent = current;
                    priorityQueue.Change(neighbor, oldPriority);
                }
            }
        }
    }
}
