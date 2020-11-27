using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public enum Direction
{ Right, Up, Left, Down }
//[System.Serializable]
public class Cell
{
    #region Declarations

    public GameObject _cellGo;
    public int _xPosition;
    public int _yPosition;
    public int2 _gridPosition;
    public int _indexGrid;
    public GridGenerator _manager;
    public Cell[] _adjacentCells = new Cell[4];
    public bool _isEmpty;

    private SpriteRenderer _render;
    private bool _isSelected = false;
    private bool _matchFound = false;

    #endregion

    #region Constructor

    public Cell(GameObject cellGo, int xPosition, int yPosition, int indexGrid, GridGenerator manager)
    {
        //if (cellGo)
        //{
        //    _name = cellGo.name;
        //}
        //else
        //{
        //    _name = "empty";
        //}
        _cellGo = cellGo;
        _xPosition = xPosition;
        _yPosition = yPosition;
        _gridPosition.x = xPosition;
        _gridPosition.y = yPosition;
        _indexGrid = indexGrid;
        _manager = manager;
    }

    #endregion


    #region Functions
    public void GetAdjacentCells()
    {
        _adjacentCells[(int)Direction.Right] = _manager.GetCellFromPosition(_xPosition + 1, _yPosition); //0
        _adjacentCells[(int)Direction.Up] = _manager.GetCellFromPosition(_xPosition, _yPosition + 1); //1
        _adjacentCells[(int)Direction.Left] = _manager.GetCellFromPosition(_xPosition - 1, _yPosition); //2
        _adjacentCells[(int)Direction.Down] = _manager.GetCellFromPosition(_xPosition, _yPosition - 1); //3

    }
    //void GetMatch()
    //{

    //}


    /*
    public void SwapSprite(GameObject renderToSwap)
    {
        if (_render.sprite == renderToSwap.GetComponent<SpriteRenderer>().sprite)
        {
            return;
        }

        //modification visuel
        Sprite tempSprite = renderToSwap.GetComponent<SpriteRenderer>().sprite;
        renderToSwap.GetComponent<SpriteRenderer>().sprite = _render.sprite;
        _render.sprite = tempSprite;

    }

    private Cell GetTravel(Vector2 castDir)
    {
        if (_xPosition == _previousSelected._xPosition + castDir.x && _yPosition == _previousSelected._yPosition + castDir.y)
        {
            return _previousSelected;
        }
        return null;
    }

    private List<Cell> GetAllTravelCell()
    {
        Vector2[] _adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        List<Cell> adjacentTiles = new List<Cell>();
        for (int i = 0; i < _adjacentDirections.Length; i++)
        {
            adjacentTiles.Add(GetTravel(_adjacentDirections[i]));

        }
        return adjacentTiles;
    }

    private void ClearMatch(List<Cell> matchCell)
    {

        if (matchCell.Count >= 2)
        {
            for (int i = 0; i < matchCell.Count; i++)
            {
                matchCell[i].GetComponent<SpriteRenderer>().sprite = null;
            }
            _matchFound = true;
        }
    }

    public void ClearAllMatches(List<Cell> matchCell)
    {
        if (_render.sprite != null)
        {
            ClearMatch(matchCell);
            if (_matchFound)
            {
                _render.sprite = null;
                _matchFound = false;
            }
        }
    }
    */


    #endregion

}
