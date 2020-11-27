using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System.Linq;
using System;

[System.Serializable]
public class GridGenerator : MonoBehaviour
{
    #region Declarations

    public int _rowCount;
    public int _columnCount;
    Vector2 _cellSize;
    Vector2 _cellHalfSize;
    Vector2 _gridHalfSize;

    public Cell[] _cellGrid;
    public OrderManager _ordersManager;
    Camera _camera;
    public List<Sprite> _characters = new List<Sprite>();
    Sprite[] _previousLeft;
    Sprite _previousBelow;
    List<Sprite> _possibleCharacters;
    Sprite _newSprite;
    public float _gridXSize;
    public float _gridYSize;
    public AnimationCurve _switchAnimation;
    public AnimationCurve _fallAnimationCurve;
    public float _switchDuration = 0.5f;
    public float _fallAnimationDuration = 0.5f;
    AudioSource[] _audio;
    AudioClip _player;
    private bool _isSelected = false;
    private static Color _selectedColor = new Color(.5f, .5f, .5f, 1.0f);
    private SpriteRenderer _render;
    private static GameObject _previousSelected = null;
    bool click = false;
    Cell _tempCell;
    bool isFinish;

    List<Cell> _listGood = new List<Cell>();
    List<Cell> _listBad = new List<Cell>();
    List<Cell> _listToCheck = new List<Cell>();

    #endregion

    #region Functions

    private void Start()
    {
        _camera = Camera.main;
        _audio = GetComponents<AudioSource>();
        CreateGrid();
        StartCoroutine(Updating());
    }

    void InitializeCellSize()
    {
        _cellSize.x = _gridXSize / _columnCount;
        _cellSize.y = _gridYSize / _rowCount;
        _cellHalfSize = _cellSize * 0.5f;
        _gridHalfSize = new Vector2(_gridXSize, _gridYSize) * 0.5f;
    }

    void CreateGrid()
    {
        InitializeCellSize();
        _cellGrid = new Cell[_rowCount * _columnCount];
        _previousLeft = new Sprite[_columnCount];
        _previousBelow = null;

        for (int x = 0; x < _rowCount; x++)
        {
            for (int y = 0; y < _columnCount; y++)
            {
                _possibleCharacters = new List<Sprite>();
                _possibleCharacters.AddRange(_characters);
                _possibleCharacters.Remove(_previousLeft[y]);
                _possibleCharacters.Remove(_previousBelow);
                _newSprite = _possibleCharacters[UnityEngine.Random.Range(0, _possibleCharacters.Count)];
                _previousLeft[y] = _newSprite;
                _previousBelow = _newSprite;
                CreateTile(x, y, _newSprite.name);
            }
        }


        for (int i = 0; i < _cellGrid.Length; i++)
        {
            _cellGrid[i].GetAdjacentCells();
        }
    }

    void CreateTile(int x, int y, string name)
    {
        int tileIndex = GridPositionToIndex(x, y);
        GameObject go = new GameObject(name);
        go.transform.position = GridPositonToWorldPosition(x, y);
        //temp
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = _newSprite;
        sr.sortingOrder = 20;
        Cell cell = new Cell(go, x, y, tileIndex, this);
        _cellGrid[tileIndex] = cell;
    }

    Cell[] CheckMatch(Cell cell)
    {
        _listGood.Clear();
        _listBad.Clear();
        _listToCheck.Clear();

        _listToCheck.Add(cell);
        while (_listToCheck.Count > 0 && _listGood.Count < 7)
        {
            for (int i = 0; i < _listToCheck[0]._adjacentCells.Length; i++)
            {
                Cell cellToCheck = _listToCheck[0]._adjacentCells[i];
                try
                {
                    if (cellToCheck == null || _listToCheck.Contains(cellToCheck) || _listGood.Contains(cellToCheck) || _listBad.Contains(cellToCheck))
                    {
                        continue;
                    }
                    if (cell._cellGo != null)
                    {
                        if (cell._cellGo.name == cellToCheck._cellGo.name && cellToCheck != null)
                        {
                            _listToCheck.Add(cellToCheck);
                        }
                        else
                        {
                            _listBad.Add(cellToCheck);
                        }
                    }
                }
                catch (Exception ex)
                {

                }


            }
            _listGood.Add(_listToCheck[0]);
            _listToCheck.RemoveAt(0);
        }
        return _listGood.ToArray();

    }

    void Combo()
    {
        foreach (Cell cell in _cellGrid)
        {
            if (cell._cellGo != null && cell != null)
            {
                Cell[] clearCell = CheckMatch(cell);
                if (clearCell.Length > 2)
                {
                    foreach (Cell cellScore in clearCell)
                    {
                        _ordersManager.ScoredATruck(10,cellScore._cellGo.name);
                    }
                    
                    ClearMatch(clearCell);
                    ItemFallFromTopToBottomBeforeRespawn();
                }
            }

        }
    }

    void ClearMatch(Cell[] cellsToClear)
    {
        for (int i = 0; i < cellsToClear.Length; i++)
        {
            Cell cellToClear = cellsToClear[i];
            Destroy(cellToClear._cellGo);
            cellToClear._cellGo = null;

        }
    }

    public void ItemFallFromTopToBottomBeforeRespawn()
    {
        List<Cell> emptyMostBottomCells = new List<Cell>();
        for (int h = 0; h < _columnCount; h++)
        {
            emptyMostBottomCells = _cellGrid.Where(c => c._cellGo == null && c._gridPosition.y == h).OrderBy(c => c._gridPosition.x).ToList();
            foreach (Cell emptyCell in emptyMostBottomCells)
            {
                int y = emptyCell._gridPosition.y;

                for (int j = y + 1; j < _columnCount; j++)
                {
                    Cell tempcomparedCell = GetCellFromPosition(emptyCell._gridPosition.x, j);
                    if (tempcomparedCell._cellGo)
                    {
                        //StartCoroutine(FallAnimation(tempcomparedCell._cellGo,emptyCell._indexGrid, GridPositonToWorldPosition(tempcomparedCell._gridPosition), GridPositonToWorldPosition(emptyCell._gridPosition)));
                        //cellsGameObject.Add(tempcomparedCell);
                        tempcomparedCell._cellGo.transform.position = GridPositonToWorldPosition(emptyCell._gridPosition);
                        emptyCell._cellGo = tempcomparedCell._cellGo;
                        tempcomparedCell._cellGo = null;
                        break;
                    }
                    //StartCoroutine(FallAnimation(tempcomparedCell._cellGo, emptyCell._indexGrid, GridPositonToWorldPosition(tempcomparedCell._gridPosition), GridPositonToWorldPosition(emptyCell._gridPosition)));
                }

            }
        }
        emptyMostBottomCells = _cellGrid.Where(c => c._cellGo == null).ToList();
        //StartCoroutine(FallingPieceEmpty(cellsGameObject, emptyMostBottomCells, 1f));
        RefillingEmpty(emptyMostBottomCells);
    }

    void RefillingEmpty(List<Cell> emptyList)
    {
        _previousLeft = new Sprite[emptyList.Count];
        _previousBelow = null;
        for (int i = 0; i < emptyList.Count; i++)
        {
            _possibleCharacters = new List<Sprite>();
            _possibleCharacters.AddRange(_characters);
            _possibleCharacters.Remove(_previousBelow);
            _newSprite = _possibleCharacters[UnityEngine.Random.Range(0, _possibleCharacters.Count)];

            _previousBelow = _newSprite;
            GameObject newGo = new GameObject(_newSprite.name);
            SpriteRenderer sr = newGo.AddComponent<SpriteRenderer>();
            sr.sprite = _newSprite;
            sr.sortingOrder = 20;

            //animation de chute
            //StartCoroutine(FallAnimation(newGo,GridPositonToWorldPosition(emptyList[i]._gridPosition), 1.5f, newGo));
            newGo.transform.position = GridPositonToWorldPosition(emptyList[i]._gridPosition);
            emptyList[i]._cellGo = newGo;
        }
        Combo();
    }

    #endregion

    #region Coroutines

    private IEnumerator FallAnimation(GameObject go, int objectIndex, Vector3 startposition, Vector3 endposition)
    {

        float t = 0;
        float tRatio;
        float tAnimated;
        while (t < _fallAnimationDuration && go)
        {
            tRatio = t / _fallAnimationDuration;
            tAnimated = _fallAnimationCurve.Evaluate(tRatio);

            go.transform.position = Vector3.Lerp(startposition, endposition, tAnimated);
            t += Time.deltaTime;
            yield return null;
        }
        if (go)
        {
            go.transform.position = endposition;
            _cellGrid[objectIndex]._cellGo.transform.position = go.transform.position;
            //_cellGrid[objectIndex]._cellGo.transform.localScale = go.transform.localScale;
        }

    }

    IEnumerator Updating()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                if (click == false)
                {
                    click = true;
                    _tempCell = GetCellFromPosition(ClampPositionToGrid(WorldToGridPosition(mouseWorldPosition)));
                    _tempCell._cellGo.GetComponent<SpriteRenderer>().color = _selectedColor;
                }
                else
                {
                    click = false;
                    _tempCell._cellGo.GetComponent<SpriteRenderer>().color = Color.white;
                }

                Cell tempCell = GetCellFromPosition(ClampPositionToGrid(WorldToGridPosition(mouseWorldPosition)));
                yield return StartCoroutine(Switching(tempCell, _switchDuration));
            }
            yield return null;
        }
    }

    IEnumerator Switching(Cell cell, float switchDuration)
    {

        Vector3 startPosition = _tempCell._cellGo.transform.position;
        Vector3 endPosition = cell._cellGo.transform.position;
        float t = 0;
        float tRatio;
        float tAnimated;
        for (int i = 0; i < cell._adjacentCells.Length; i++)
        {
            if (click == false && cell._adjacentCells[i] == _tempCell)
            {
                _audio[1].Play();
                while (t < switchDuration)
                {
                    tRatio = t / switchDuration;
                    tAnimated = _switchAnimation.Evaluate(tRatio);
                    cell._cellGo.transform.position = Vector3.Lerp(endPosition, startPosition, tAnimated);
                    _tempCell._cellGo.transform.position = Vector3.Lerp(startPosition, endPosition, tAnimated);
                    t = t + Time.deltaTime;
                    yield return null;
                }

                GameObject tempGo = _tempCell._cellGo;

                _tempCell._cellGo.transform.position = endPosition;
                cell._cellGo.transform.position = startPosition;

                _tempCell._cellGo = cell._cellGo;
                cell._cellGo = tempGo;

                if (CheckMatch(cell).Count() > 2 || CheckMatch(_tempCell).Count() > 2)
                {
                    Cell[] clearCell = CheckMatch(_tempCell);
                    if (clearCell.Length > 2)
                    {
                        if(clearCell.Length>3)
                        {
                            _audio[3].Play();
                        }
                        else
                        {
                            _audio[2].Play();
                        }
                        foreach (Cell cellScore in clearCell)
                        {
                            _ordersManager.ScoredATruck(10, cellScore._cellGo.name);
                        }
                        ClearMatch(clearCell);
                        ItemFallFromTopToBottomBeforeRespawn();
                    }
                    clearCell = CheckMatch(cell);
                    if (clearCell.Length > 2)
                    {
                        if (clearCell.Length > 3)
                        {
                            _audio[3].Play();
                        }
                        else
                        {
                            _audio[2].Play();
                        }
                        foreach (Cell cellScore in clearCell)
                        {
                            _ordersManager.ScoredATruck(10, cellScore._cellGo.name);
                        }
                        ClearMatch(clearCell);
                        ItemFallFromTopToBottomBeforeRespawn();
                    }
                    break;

                }

                else
                {
                    _audio[4].Play();
                    t = 0;
                    while (t < switchDuration)
                    {
                        tRatio = t / switchDuration;
                        tAnimated = _switchAnimation.Evaluate(tRatio);
                        cell._cellGo.transform.position = Vector3.Lerp(endPosition, startPosition, tAnimated);
                        _tempCell._cellGo.transform.position = Vector3.Lerp(startPosition, endPosition, tAnimated);
                        t = t + Time.deltaTime;
                        yield return null;
                    }

                    _tempCell._cellGo.transform.position = endPosition;
                    cell._cellGo.transform.position = startPosition;

                    tempGo = cell._cellGo;

                    cell._cellGo = _tempCell._cellGo;
                    _tempCell._cellGo = tempGo;
                }
            }
        }
        yield return null;
    }

    #endregion

    #region Helpers

    public int GridPositionToIndex(int x, int y)
    {
        return (x * _rowCount + y);
    }

    public Cell GetCellFromPosition(int xPosition, int yPosition)
    {
        if (xPosition >= 0 && xPosition < _columnCount && yPosition >= 0 && yPosition < _rowCount)
        {
            return _cellGrid[GridPositionToIndex(xPosition, yPosition)];
        }
        return null;
    }

    public Cell GetCellFromPosition(int2 position)
    {
        /*if (position.x < 0 || position.x >= _columnCount || position.y < 0 || position.y >= _rowCount)
        {

        }*/

        return GetCellFromPosition(position.x, position.y);
    }

    int2 ClampPositionToGrid(int xPosition, int yPosition)
    {
        return new int2(Mathf.Clamp(xPosition, 0, _columnCount - 1), Mathf.Clamp(yPosition, 0, _rowCount - 1));
    }

    public Vector3 GridPositonToWorldPosition(int xPosition, int yPosition)
    {
        return (Vector3)((new Vector2(xPosition, yPosition) * _cellSize) + _cellHalfSize - _gridHalfSize) + transform.position;
    }

    public Vector3 GridPositonToWorldPosition(int2 transformPosition)
    {
        return GridPositonToWorldPosition(transformPosition.x, transformPosition.y);
    }

    int2 WorldToGridPosition(Vector2 position)
    {
        Vector2 localposition = (Vector2)transform.InverseTransformPoint(position) + _gridHalfSize;
        return new int2(Mathf.FloorToInt(localposition.x / _cellSize.x), Mathf.FloorToInt(localposition.y / _cellSize.y));
    }

    int2 ClampPositionToGrid(int2 position)
    {
        return (ClampPositionToGrid(position.x, position.y));
    }

    #endregion
}

