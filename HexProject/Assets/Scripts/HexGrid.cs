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

    public HexUnit unitPrefab;

    HexCellPriorityQueue priorityQueue;

    int searchFrontierPhase;

    public bool HasPath
    {
        get
        {
            return currentPathExists;
        }
    }
    bool currentPathExists;
    HexCell currentPathFrom, currentPathTo;

    List<HexUnit> units = new List<HexUnit>();

    void Awake()
    {
        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;
        HexUnit.unitPrefab = unitPrefab;
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
        //Debug.Log("touched at " + coordinates.ToString());
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

    public HexCell GetCell(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return GetCell(hit.point);
        }
        return null;
    }

    public void ShowUI(bool visible)
    {
        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i].ShowUI(visible);
        }
    }

    void ClearUnits()
    {
        for (int i = 0; i < units.Count; i++)
        {
            units[i].Die();
        }
        units.Clear();
    }

    public void AddUnit(HexUnit unit, HexCell location, float orientation)
    {
        units.Add(unit);
        unit.transform.SetParent(transform, false);
        unit.Location = location;
        unit.Orientation = orientation;
    }

    public void RemoveUnit(HexUnit unit)
    {
        units.Remove(unit);
        unit.Die();
    }

    public void ClearPath()
    {
        if (currentPathExists)
        {
            HexCell current = currentPathTo;
            while (current != currentPathFrom)
            {
                current.SetLabel(null);
                current.DisableHighlight();
                current = current.Parent;
            }
            current.DisableHighlight();
            currentPathExists = false;
        }
        else if (currentPathFrom)
        {
            currentPathFrom.DisableHighlight();
            currentPathTo.DisableHighlight();
        }
        currentPathFrom = currentPathTo = null;
    }

    void ShowPath(int speed)
    {
        if (currentPathExists)
        {
            HexCell current = currentPathTo;
            while (current != currentPathFrom)
            {
                int turn = (current.Distance - 1) / speed;
                /*if (turn * speed == current.Distance)
                {
                    turn -= 1;
                }*/
                current.SetLabel(turn.ToString());
                current.EnableHighlight(Color.white);
                current = current.Parent;
            }
        }
        currentPathFrom.EnableHighlight(new Color(1f, 1f, 0f, 1f));
        currentPathTo.EnableHighlight(Color.red);
    }

    public void FindPath(HexCell fromCell, HexCell toCell, int speed)
    {
        ClearPath();
        currentPathFrom = fromCell;
        currentPathTo = toCell;
        currentPathExists = Search(fromCell, toCell, speed);
        ShowPath(speed);
    }

    bool Search(HexCell fromCell, HexCell toCell, int speed)
    {
        searchFrontierPhase += 2;

        if (priorityQueue == null)
        {
            priorityQueue = new HexCellPriorityQueue();
        }
        else
        {
            priorityQueue.Clear();
        }

        fromCell.SearchPhase = searchFrontierPhase;
        fromCell.Distance = 0;
        priorityQueue.Enqueue(fromCell);
        while (priorityQueue.Count > 0)
        {
            HexCell current = priorityQueue.Dequeue();

            if (current == toCell)
            {
                return true;
            }

            int currentTurn = (current.Distance - 1) / speed;
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor == null || neighbor.SearchPhase > searchFrontierPhase)
                {
                    continue;
                }
                if (neighbor.type == HexType.WATER || neighbor.Unit)
                {
                    continue;
                }
                int distance = current.Distance + neighbor.movementCost;
                int turn = (distance - 1) / speed;
                /*if (turn * speed == distance)
                {
                    turn -= 1;
                }*/
                if (turn > currentTurn)
                {
                    distance = turn * speed + neighbor.movementCost;
                }
                if (neighbor.SearchPhase < searchFrontierPhase)        // == int.MaxValue
                {
                    neighbor.SearchPhase = searchFrontierPhase;
                    neighbor.Distance = distance;
                    neighbor.Parent = current;
                    neighbor.SearchHeuristic = neighbor.coordinates.DistanceTo(toCell.coordinates);
                    priorityQueue.Enqueue(neighbor);
                }
                else if (distance < neighbor.Distance)
                {
                    int oldPriority = neighbor.SearchPriority;
                    neighbor.Distance = distance;
                    neighbor.Parent = current;
                    priorityQueue.Change(neighbor, oldPriority);
                }
            }
        }
        return false;
    }

    public List<HexCell> GetPath()
    {
        if (!currentPathExists)
        {
            return null;
        }
        List<HexCell> path = ListPool<HexCell>.Get();
        for (HexCell c = currentPathTo; c != currentPathFrom; c = c.Parent)
        {
            path.Add(c);
        }
        path.Add(currentPathFrom);
        path.Reverse();
        return path;
    }
}
