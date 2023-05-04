using UnityEngine;

public class MapEditor : MonoBehaviour
{

    private Color[] colors;

    public HexGrid grid;

    private Color activeColor;

    void Awake()
    {
        //colors = new Color[4];
        colors = grid.colors;
        SelectColor(0);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            grid.ColorCell(hit.point, activeColor);
        }
    }

    public void SelectColor(int index)
    {
        activeColor = colors[index];
    }
}