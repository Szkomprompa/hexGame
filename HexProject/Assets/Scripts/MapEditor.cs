using UnityEngine;
using UnityEngine.EventSystems;

public class MapEditor : MonoBehaviour
{
    public HexGrid grid;

    private Color activeColor;
    private HexType activeType;

    public HexUnit unitPrefab;
    bool editMode;


    void Awake()
    {
        SelectType(0);
    }

    void Update()
    {
        //if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        //{
        //    HandleInput();
        //}

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButton(0))
            {
                HandleInput();
                return;
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    DestroyUnit();
                }
                else
                {
                    CreateUnit();
                }
                return;
            }
        }
    }

    void HandleInput()
    {
        HexCell currentCell = GetCellUnderCursor();
        if (currentCell)
        {
            if (editMode)
            {
                grid.EditCell(currentCell, activeType);
            }
            else
            {
                grid.FindDistancesTo(currentCell);
            }
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

    public void SetEditMode(bool toggle)
    {
        editMode = !editMode;//toggle;
    }

    HexCell GetCellUnderCursor()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            return grid.GetCell(hit.point);
        }
        return null;
    }

    void CreateUnit()
    {
        HexCell cell = GetCellUnderCursor();
        if (cell && !cell.Unit)
        {
            HexUnit unit = Instantiate(unitPrefab);
            unit.transform.SetParent(grid.transform, false);
            unit.Location = cell;
        }
    }

    void DestroyUnit()
    {
        HexCell cell = GetCellUnderCursor();
        if (cell && cell.Unit)
        {
            cell.Unit.Die();
        }
    }
}