using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBar : MonoBehaviour
{
    public GameManager _gameManager;
    public Text _scoreTextComponent;

    private void Start()
    {
        _scoreTextComponent.text = "Score\n" + _gameManager._mainScore.ToString();
    }

    private void Update()
    {
        _scoreTextComponent.text = "Score\n" + _gameManager._mainScore.ToString();
    }
}
