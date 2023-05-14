using UnityEngine;

public class HexCell : MonoBehaviour 
{
    public HexCoordinates coordinates;

    public Color Color
    {
        get
        {
            return color;
        }
        set
        {
            if(color == value)
            {
                return;
            }
            color = value;
            Refresh();
        }
    }
    Color color;

    public HexGridChunk chunk;

    public RectTransform uiRect;

    [SerializeField]
    HexCell[] neighbors;

    public HexType type;
    public int movementCost;

    public int Elevation
    {
        get
        {
            return elevation;
        }
        set
        {
            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = elevation * -HexMetrics.elevationStep - 0.01f;
            uiRect.localPosition = uiPosition;
        }
    }

    private int elevation;

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public void SetType(HexType type) 
    {
        this.type = type;
        switch (type)
        {
            case HexType.WATER:
                this.Color = new Color(0f, 0f, 1f, 1f);
                this.Elevation = 0;
                this.movementCost = 1;
                break;
            case HexType.PLAINS:
                this.Color = new Color(0.1f, 0.7f, 0f, 1f);
                this.Elevation = 1;
                this.movementCost = 1;
                break;
            case HexType.WOODS:
                this.Color = new Color(0f, 0.4f, 0f, 1f);
                this.Elevation = 1;
                this.movementCost = 2;
                break;
            case HexType.MOUNTAINS:
                this.Color = new Color(0.3f, 0.3f, 0.3f, 1f);
                this.Elevation = 3;
                this.movementCost = 3;
                break;
            case HexType.HILL:
                this.Color = new Color(0.5f, 0.4f, 0.3f, 1f);
                this.Elevation = 2;
                this.movementCost = 2;
                break;
        }
    }

    void Refresh()
    {
        if (chunk)
        {
            chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
        }
    }
}