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

    public int elevation;

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
                this.elevation = 0;
                break;
            case HexType.PLAINS:
                this.Color = new Color(0.1f, 0.7f, 0f, 1f);
                this.elevation = 1;
                break;
            case HexType.WOODS:
                this.Color = new Color(0f, 0.4f, 0f, 1f);
                this.elevation = 1;
                break;
            case HexType.MOUNTAINS:
                this.Color = new Color(0.3f, 0.3f, 0.3f, 1f);
                this.elevation = 2;
                break;
        }
    }

    void Refresh()
    {
        if (chunk)
        {
            chunk.Refresh();
        }
    }
}