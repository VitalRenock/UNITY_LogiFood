using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public OrderManager _orderManager;

    public int _lifePoints;
    [HideInInspector]
    public int _mainScore;

    public int _CountDownValue;
    public Text _CountDownStartText;

    void Start()
    {
        StartCoroutine("StartGame");
    }

    IEnumerator StartGame()
    {
        yield return StartCoroutine("CountDownStart");        
        _orderManager.StartCoroutine("LaunchOrders");

        while (_lifePoints > 0)
        {
            yield return null;
        }

        SceneManager.LoadScene("GameOverScene");
    }

    IEnumerator CountDownStart()
    {
        int countDownCurrent = _CountDownValue;

        while (countDownCurrent > 0)
        {
            _CountDownStartText.text = countDownCurrent.ToString();
            countDownCurrent--;
            yield return new WaitForSeconds(1);
        }
        _CountDownStartText.enabled = false;
    }
}
