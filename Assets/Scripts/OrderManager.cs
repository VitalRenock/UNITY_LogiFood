using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public GameObject[] _orders;
    public int _totalOrders;
    int _remainingOrders;
    public int _minTimeOrders;
    public int _maxTimeOrders;
    public float _timePast;
    public float _timeToRemove;
    public float _timeBeforeOrder;
    public float _lastTime;

    public bool _gameIsRunning;

    private void Start()
    {
        _timeBeforeOrder = _maxTimeOrders;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("I try to scored");
            ScoredATruck(3, "Milk");
        }
    }

    IEnumerator LaunchOrders()
    {
        _gameIsRunning = true;
        _remainingOrders = _totalOrders;

        StartCoroutine(DecreasingTimeOrder());

        while (_gameIsRunning)
        {
            if(_remainingOrders > 0) 
            {
                CreateOrder();
            }
            else
            {
                _gameIsRunning = false;
            }
            yield return new WaitForSeconds(_timeBeforeOrder);
        }
    }

    IEnumerator DecreasingTimeOrder()
    {
        while (true)
        {
            yield return new WaitForSeconds(_timePast);
            _timeBeforeOrder = Mathf.Clamp(_timeBeforeOrder - _timeToRemove, _minTimeOrders, _maxTimeOrders);
        }
    }

    public void CreateOrder()
    {
        for (int i = 0; i < _orders.Length; i++)
        {
            BlankOrder tempBlankOrder = _orders[i].GetComponent<BlankOrder>();

            if (tempBlankOrder._isEmpty == true)
            {
                tempBlankOrder.StartCoroutine("CountDownOrder");
                break;
            }
        }
    }

    public void ScoredATruck(int score, string typeOfTheTruck)
    {
        Debug.Log("Début de ScoredAtTruck(): Score = " + score + " / Type = " + typeOfTheTruck);
        for (int i = 0; i < _orders.Length; i++)
        {
            BlankOrder tempBlankOrder = _orders[i].GetComponent<BlankOrder>();

            if (tempBlankOrder._typeOfThisOrder.ToString() == typeOfTheTruck && tempBlankOrder._isEmpty == false)
            {
                tempBlankOrder.TruckScored(score, typeOfTheTruck);
                break;
            }
            else { Debug.LogWarning("Score ignoré"); }
        }
    }
}
