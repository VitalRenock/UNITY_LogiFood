using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    public GameManager _gameManager;
    public Image _LifeImageComponent;
    public Sprite[] _spritesLifeBar;

    private void Start()
    {
        _LifeImageComponent.sprite = _spritesLifeBar[_gameManager._lifePoints];
    }

    private void Update()
    {
        _LifeImageComponent.sprite = _spritesLifeBar[_gameManager._lifePoints];
    }
}
