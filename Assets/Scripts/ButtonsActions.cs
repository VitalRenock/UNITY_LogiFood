using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonsActions : MonoBehaviour
{
    AudioSource[] _audio;
    AudioClip _player;
    public Text _highScoreText;

    private void Start()
    {
        _audio = GetComponents<AudioSource>();
        _highScoreText.text = "HighScore Start()";
    }

    public void StartGame()
    {
        _audio[0].Play();
        Debug.Log("Start Game");
        SceneManager.LoadScene("GameScene");
    }

    public void MuteGame()
    {
        _audio[0].Play();
        Debug.Log("Mute Game");
    }

    public void QuitGameButton()
    {
        _audio[0].Play();
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void HighScoreUpdate()
    {
        // Mise à jour du score.
    }

    public void MenuButton()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
