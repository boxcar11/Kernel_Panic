using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : SingletonClass<GameManager>
{
    [Tooltip("Level at which component is no longer counted (Inclusive).")]
    public float componentDeath = 15;
    [Tooltip("Number of dead components for instant death")]
    public float instantDeath = 15;
    [Tooltip("Number of dead components for slow death.")]
    public float slowDeath = 5;
    [Tooltip("Number of seconds for slow death")]
    public float slowTime = 30;

    public GameObject menu;
    
    public GameObject loseCanvas;

    public Player player;

    public Text uptimeText;

    private List<Components> componentSlots = new List<Components>();
    private int totalNumOfComponents;
    private int numOfComponents;

    private float timer;
    private float gameTimer;

    // Start is called before the first frame update
    void Start()
    {
        timer = slowTime;

        // Get total number of parts in computer
        GameObject[] tempObj = GameObject.FindGameObjectsWithTag("ComputerComponent");
        for (int i = 0; i < tempObj.Length; i++)
        {
            totalNumOfComponents += tempObj[i].GetComponent<Machine>().numberOfComponents;
        }
    }

    // Update is called once per frame
    void Update()
    {
        gameTimer += Time.deltaTime;

        CheckComputerCondition();

        if(Input.GetButtonUp("Cancel"))
        {
            ToggleMenu();
        }
    }

    // Check how many working components are in computer
    private void CheckComputerCondition()
    {
        numOfComponents = 0;
        GameObject[] tempObj = GameObject.FindGameObjectsWithTag("ComputerComponent");
        for (int i = 0; i < tempObj.Length; i++)
        {
            Machine mach = tempObj[i].GetComponent<Machine>();

            int tempCount = mach.GetComponentCount();
            numOfComponents += tempCount;

            for (int j = 0; j < tempCount; j++)
            {
                // Check if part is bad
                if(mach.GetComponentAtIndex(j).health <= componentDeath)
                {
                    numOfComponents--;
                }
            }
        }

        // Check if lost to many parts
        if((totalNumOfComponents - numOfComponents) >= instantDeath)
        {
            Lose();
        }
        else if((totalNumOfComponents - numOfComponents) >= slowDeath)
        {
            if(timer <= 0)
            {
                Lose();
            }
            timer -= Time.deltaTime;
        }
        else
        {   // Reset timer
            timer = slowTime;
        }
    }

    private void ToggleMenu()
    {
        menu.SetActive(!menu.activeSelf);
    }

    private int CalculateMinute(float uptime)
    {
        return (int)(uptime / 60);
    }

    private int CalulateSecond(float uptime)
    {
        return (int)(uptime % 60);
    }

    #region Lose
    public void LoseButton()
    {
        // Load main menu
        SceneManager.LoadScene(0);
    }

    private void Lose()
    {
        if (loseCanvas != null)
        {
            loseCanvas.SetActive(true);
        }
        if (player != null)
        {
            player.enabled = false;
        }
        Time.timeScale = 0;
        float uptime = gameTimer;
        int min = CalculateMinute(uptime);
        int sec = CalulateSecond(uptime);
        if (uptimeText != null)
        {
            uptimeText.text = " UPTIME: " + min + ":" + sec;
        }
    }
    #endregion
}
