using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlertPanelManager : MonoBehaviour
{
    public GameObject alertPanelPrefab;

    [Header("Attention")]
    public float attentionValue = .5f;
    public Color attentionBackgroundColor;

    [Header("Critical")]
    public float criticalValue = .25f;
    public Color criticalBackgroundColor;

    //private GameObject[] componentArray;
    private List<Machine> componentList = new List<Machine>();
    private List<AlertTile> alertTiles = new List<AlertTile>();

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] tempArray = GameObject.FindGameObjectsWithTag("ComputerComponent");


        // Create alert window for every object with tag of ComputerComponent
        for (int i = 0; i < tempArray.Length; i++)
        {
            Machine curMachine = tempArray[i].GetComponent<Machine>();
            componentList.Add(curMachine);
            for (int j = 0; j < curMachine.GetComponentCount(); j++)
            {
                GameObject temp = Instantiate(alertPanelPrefab);
                temp.transform.SetParent(this.transform, false);               
                
                AlertTile tile = temp.GetComponent<AlertTile>();
                // Give each machine its tile so that it has reference to it
                curMachine.AddTile(tile);

                // Add tile to alertTiles list
                alertTiles.Add(tile);

                // Hide new alert
                temp.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < componentList.Count; i++)
        {
            Machine currentMachine = componentList[i];

            for (int j = 0; j < currentMachine.GetComponentCount(); j++)
            {
                GameObject tileObj = currentMachine.GetTileAtIndex(j).gameObject;
                Components comp = currentMachine.GetComponentAtIndex(j);
                if ((comp.health / 100) <= criticalValue)
                {
                    tileObj.GetComponentInChildren<TextMeshProUGUI>().SetText(currentMachine.name + ": is critical");
                    tileObj.GetComponent<Image>().color = criticalBackgroundColor;
                }

                else if ((comp.health / 100) <= attentionValue)
                {
                    tileObj.GetComponentInChildren<TextMeshProUGUI>().SetText(currentMachine.name + ": needs attention");
                    tileObj.GetComponent<Image>().color = attentionBackgroundColor;

                    tileObj.SetActive(true);
                }
                else
                {
                    tileObj.SetActive(false);
                }

                currentMachine.GetTileAtIndex(j).health = comp.health;
            }
        }

        alertTiles.Sort(SortAlerts);
        ReorderUIAlerts();
    }

    static int SortAlerts(AlertTile alert1, AlertTile alert2)
    {
        return alert1.health.CompareTo(alert2.health);
    }

    private void ReorderUIAlerts()
    {
        for (int i = 0; i < alertTiles.Count; i++)
        {
            alertTiles[i].gameObject.transform.SetSiblingIndex(i);
        }
    }
}
