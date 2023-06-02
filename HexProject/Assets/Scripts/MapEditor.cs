using UnityEngine;
using UnityEngine.EventSystems;

public class MapEditor : MonoBehaviour
{
    public HexGrid grid;

    private Color activeColor;
    private HexType activeType;

    public HexUnit unitPrefab;
    public HexCity cityPrefab;
    //bool editMode;

    //  HexCell previousCell, searchFromCell, searchToCell;
    HexCell previousCell;

    void Awake()
    {
        SelectType(0);
        SetEditMode(false);
    }

    void Update()
    {
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
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    DestroyCity();
                }
                else
                {
                    CreateCity();
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
            grid.EditCell(currentCell, activeType);
            /*if (editMode)
            {
                grid.EditCell(currentCell, activeType);
            }
            else if (Input.GetKey(KeyCode.LeftShift) && searchToCell != currentCell)
            {
                if (searchFromCell != currentCell)
                {
                    if (searchFromCell)
                    {
                        searchFromCell.DisableHighlight();
                    }
                    searchFromCell = currentCell;
                    searchFromCell.EnableHighlight(new Color(1f, 1f, 0f, 1f));
                    if (searchToCell)
                    {
                        grid.FindPath(searchFromCell, searchToCell, 4);
                    }
                }
            }
            else if (searchFromCell && searchFromCell != currentCell)
            {
                if (searchFromCell != currentCell)
                {
                    searchToCell = currentCell;
                    grid.FindPath(searchFromCell, searchToCell, 4);
                }
            }*/
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
        enabled = toggle;
    }

    HexCell GetCellUnderCursor()
    {
        return grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
    }

    void CreateUnit()
    {
        HexCell cell = GetCellUnderCursor();
        if (cell && !cell.Unit)
        {
            grid.AddUnit(Instantiate(unitPrefab), cell, Random.Range(0f, 360f));
        }
    }

    public void CreateUnit(HexCell cell)
    {
        if (cell && !cell.Unit)
        {
            grid.AddUnit(Instantiate(unitPrefab), cell, Random.Range(0f, 360f));
        }
    }

    void DestroyUnit()
    {
        HexCell cell = GetCellUnderCursor();
        if (cell && cell.Unit)
        {
            grid.RemoveUnit(cell.Unit);
        }
    }

    void CreateCity()
    {
        HexCell cell = GetCellUnderCursor();
        if (cell && !cell.City)
        {
            grid.AddCity(Instantiate(cityPrefab), cell);
        }
    }

    void DestroyCity()
    {
        HexCell cell = GetCellUnderCursor();
        if (cell && cell.City)
        {
            grid.RemoveCity(cell.City);
        }
    }
}