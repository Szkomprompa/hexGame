using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HexGameUI : MonoBehaviour
{
    public HexGrid grid;

    HexCell currentCell;

    HexUnit selectedUnit;

    HexCity selectedCity;

    public GameObject CityMenuPanel;

    public MapEditor mapEditor;

    public int money;
    public int gain;

    public Text moneyText;

    public void SetEditMode(bool toggle)
    {
        enabled = !toggle;
        grid.ShowUI(!toggle);
        grid.ClearPath();
    }

    bool UpdateCurrentCell()
    {
        HexCell cell = grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (cell != currentCell) 
        {
            currentCell = cell;
            return true;
        }
        return false;
    }

    void DoSelection()
    {
        grid.ClearPath();
        UpdateCurrentCell();
        if (currentCell)
        {
            selectedUnit = currentCell.Unit;
            if (!selectedUnit) 
            {
                selectedCity = currentCell.City;
            }
        }
    }

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                DoSelection();
            }
            else if (selectedUnit)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    DoMove();
                }
                else
                {
                    DoPathfinding();
                }
            }
        }
        if (selectedCity && !selectedUnit)
        {
            OpenCityMenu();
        }
        else
        {
            CloseCityMenu();
        }
    }

    void DoPathfinding()
    {
        if (UpdateCurrentCell())
        {
            if (currentCell && selectedUnit.IsValidDestination(currentCell))
            {
                grid.FindPath(selectedUnit.Location, currentCell, 4);
            }
            else
            {
                grid.ClearPath();
            }
        }
    }

    void DoMove()
    {
        if (grid.HasPath)
        {
            List<HexCell> path = grid.GetPath();
            foreach (HexCell cell in path)
            {
                Debug.Log(cell.owner);
                if (cell.owner != 0)
                {
                    gain += 1;
                    cell.SetOwner(0);
                }
            }
            selectedUnit.Travel(grid.GetPath());    //selectedUnit.Location = currentCell;
            UpdateMoney();
            grid.ClearPath();
        }
    }

    public void OpenCityMenu()
    {
        if (CityMenuPanel != null && !CityMenuPanel.activeSelf)
        {
            CityMenuPanel.SetActive(true);
        }
    }

    public void CloseCityMenu()
    {
        if (CityMenuPanel != null && CityMenuPanel.activeSelf)
        {
            CityMenuPanel.SetActive(false);
        }
    }

    public void AddUnit()
    {
        if (!currentCell.Unit && money >= 10)
        {
            mapEditor.CreateUnit(currentCell);      // DO ZMIANY (BEZPOŒREDNIO Z HEXGRID)
            money -= 10;
        }
    }

    public void UpdateMoney()
    {
        if (gain == 0)
        {
            gain = grid.gain;
        }
        moneyText.text = "Monety: " + money + "  (+" + gain + ")";
    }

    public void AddMoney()
    {
        money += gain;
        UpdateMoney();
    }
}