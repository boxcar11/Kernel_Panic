using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public List<Sprite> images = new List<Sprite>();
    public List<string> description = new List<string>();

    public TextMeshProUGUI textDesc;
    public Image sprite;

    public Button previous;
    public Button next;
    public GameObject enterGame;

    public int slides;

    // Start is called before the first frame update
    void Start()
    {

        //slides = description.Count-1;
        sprite.sprite = images[slides];
        textDesc.text = description[slides];
    }

    // Update is called once per frame
    void Update()
    {
        if (slides == 0)
        {
            previous.interactable = false;
        }
        else
        {
            previous.interactable = true;
        }

        if (slides == description.Count - 1)
        {
            next.interactable = false;
            enterGame.SetActive(true);
        }
        else
        {
            next.interactable = true;
            enterGame.SetActive(false);
        }

    }

    public void NextSlide()
    {
        slides++;
        sprite.sprite = images[slides];
        textDesc.text = description[slides];
    }

    public void PrevSlide()
    {
        slides--;
        sprite.sprite = images[slides];
        textDesc.text = description[slides];
    }

    public void EnterGame()
    {
        SceneManager.LoadScene("Game");
    }
}
