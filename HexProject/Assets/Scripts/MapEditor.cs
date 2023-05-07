using UnityEngine;
using UnityEngine.EventSystems;

public class MapEditor : MonoBehaviour
{
    public HexGrid grid;

    private Color activeColor;
    private HexType activeType;

    void Awake()
    {
        SelectType(0);
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
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
            grid.EditCell(grid.ColorCell(hit.point), activeType);
        }
    }

    public void SelectType(int type)
    {
        switch (type)
        {
            case 0:
                activeType = HexType.WATER;
                break;
            case 1:
                activeType = HexType.PLAINS;
                break;
            case 2:
                activeType = HexType.WOODS;
                break;
            case 3:
                activeType = HexType.MOUNTAINS;
                break;
            case 4:
                activeType = HexType.HILL;
                break;
        }
    }
}