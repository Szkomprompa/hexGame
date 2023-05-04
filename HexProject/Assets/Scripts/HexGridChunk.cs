using UnityEngine;
using UnityEngine.UI;

public class HexGridChunk : MonoBehaviour
{
    HexCell[] cells;

    HexMesh hexMesh;
    Canvas gridCanvas;

    [SerializeField]
    HexGridChunk[] neighbors;

    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();

        cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
        neighbors = new HexGridChunk[4];
    }

    void Start()
    {
        hexMesh.Triangulate(cells);
    }

    public void AddCell(int index, HexCell cell)
    {
        cells[index] = cell;
        cell.chunk = this;
        cell.transform.SetParent(transform, false);
        cell.uiRect.SetParent(gridCanvas.transform, false);
    }

    public void Refresh()
    {
        hexMesh.Triangulate(cells);
    }

    public HexGridChunk GetNeighbor(int direction)
    {
        return neighbors[direction];
    }

    public void SetNeighbor( int direction, HexGridChunk chunk)
    {
        neighbors[direction] = chunk;
        int opposite;
        switch (direction)
        {
            case 0:
                opposite = 2;
                break;
            case 1: 
                opposite = 3;
                break;
            default:
                opposite = 0;
                break;
        }
        chunk.neighbors[opposite] = this;
    }
}
