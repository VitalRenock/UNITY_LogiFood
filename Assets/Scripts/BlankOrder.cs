using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TypeOfOrders
{
    Apple,
    Banana,
    Eggplant,
    Meat,
    Carrot,
    maxValue
}

public class BlankOrder : MonoBehaviour
{
    public GameObject _prefabTruck;
    GameObject _gameObjectTruck;
    public bool _isEmpty;

    public TypeOfOrders _typeOfThisOrder;

    public int _scoreWinForOrder;
    public int _currentScoreOrder;

    public int _orderTimer;
    public int _minOrderTimer;
    public int _maxOrderTimer;
    public Text _orderTextTimer;

    public GameManager _gameManager;

    void Start()
    {
        _orderTimer = Random.Range(_minOrderTimer, _maxOrderTimer);
        _isEmpty = true;
        _orderTextTimer.enabled = false;
    }

    IEnumerator CountDownOrder()
    {
        _isEmpty = false;
        _orderTextTimer.enabled = true;
        _gameObjectTruck = Instantiate(_prefabTruck, transform.position, Quaternion.identity, transform);

        _typeOfThisOrder = (TypeOfOrders)Random.Range(0, (int)TypeOfOrders.maxValue);
        _orderTimer = Random.Range(_minOrderTimer, _maxOrderTimer);
        int currentTimer = _orderTimer;

        while (currentTimer > 0)
        {
            if (_currentScoreOrder >= _scoreWinForOrder)
            {

                TruckValidate();
                CleanOrder();

                yield break;
            }

            currentTimer--;
            _orderTextTimer.text = _typeOfThisOrder.ToString() + "\n" + currentTimer.ToString();
            yield return new WaitForSeconds(1f);
        }


        TruckLose();
        CleanOrder();
        yield return null;
    }

    public void CleanOrder()
    {
        _isEmpty = true;
        _orderTextTimer.enabled = false;
        Destroy(_gameObjectTruck);
    }

    public void TruckScored(int score, string test)
    {
        Debug.LogError("Début TruckScored() - Score recu =  " + score.ToString() + " / Type = " + test);
        _currentScoreOrder += score;
    }

    public void TruckValidate()
    {
        //Debug.Log("CAMION GAGNE");
        _gameManager._mainScore += _currentScoreOrder;
        _currentScoreOrder = 0;
    }

    public void TruckLose()
    {
        //Debug.Log("CAMION PERDU");
        _gameManager._lifePoints--;
    }

}
