using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Image muteImage;
    public Sprite unmute;
    public Sprite mute;
    private AudioListener listener;
    private bool muted = false;

    private void Start()
    {
        listener = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioListener>();
    }

    public void NewGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Mute()
    {
        if (muted)
        {
            listener.enabled = true;
            muteImage.sprite = unmute;
        }
        else
        {
            listener.enabled = false;
            muteImage.sprite = mute;
        }
        muted = !muted;
    }
}
