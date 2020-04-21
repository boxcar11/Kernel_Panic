using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequestController : MonoBehaviour
{
    public GameObject requestPrefab;
    public GameObject requestPanel;
    public CanvasGroup requestCanvasGroup;
    public GameObject closeWindow;
    

    [SerializeField]
    private MaterialRequest[] mr;

    private List<GameObject> materialPanels = new List<GameObject>();

    private RequestDropoff requestDropoff;
    
    private bool requestWindowVisible;
    private bool playerInRange = false;

    // Start is called before the first frame update
    void Start()
    {
        requestDropoff = FindObjectOfType<RequestDropoff>();

        for (int j = 0; j < 3; j++)
        {
            GameObject go = Instantiate(requestPrefab);
            materialPanels.Add(go);
            go.transform.SetParent(requestPanel.transform, false);

            Text[] textObj = go.GetComponentsInChildren<Text>();
            textObj[0].text = mr[j].materials.materialName.ToString();
            textObj[1].text = "Stock: " + mr[j].GetCount() + "/" + mr[j].GetMaxCount();
            textObj[2].text =mr[j].materials.count.ToString();

            go.transform.GetChild(0).GetComponent<Image>().sprite = mr[j].materials.spriteImage;

            //int childCount = go.transform.childCount;
            Button[] btns = go.transform.GetComponentsInChildren<Button>();

            // Create variable to hold value of j at the point (copy held foreach set of buttons
            int init = j;

            // Set buttons to call increase/decrease of count
            btns[0].onClick.AddListener(delegate { IncreaseCount(mr[init].materials.materialName); });
            btns[1].onClick.AddListener(delegate { DecreaseCount(mr[init].materials.materialName); });
        }  
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonUp("Fire3") && playerInRange)
        {
            ToggleRequestWindow();
        }

        // Call update on MaterialRequest objects since they don't inhereit monobehavior
        for (int i = 0; i < mr.Length; i++)
        {
            mr[i].UpdateProgress();
        }

        UpdateUI();
    }

    // Increase count of material
    public void IncreaseCount(materialEnum mat)
    {
        for (int i = 0; i < mr.Length; i++)
        {
            if(mr[i].materials.materialName == mat)
            {
                if (mr[i].GetCount() > mr[i].materials.count)
                {
                    mr[i].materials.count++;
                }
            }
        }

        UpdateUI();
    }

    // Decrease count of material
    public void DecreaseCount(materialEnum mat)
    {
        for (int i = 0; i < mr.Length; i++)
        {
            if (mr[i].materials.materialName == mat)
            {
                mr[i].materials.count--;
            }
        }

        UpdateUI();
    }

    // Get count of materials
    public int GetCount(materialEnum mat)
    {
        for (int i = 0; i < mr.Length; i++)
        {
            if (mr[i].materials.materialName == mat)
            {
                return mr[i].materials.count;
            }
        }

        // Should never get here
        Debug.LogError("Could not find: " + mat.ToString() + " in Machine.cs");
        return 0;
    }

    // Sends a request for delivery of requested items
    public void Request()
    {
        List<Materials> matList = new List<Materials>();

        for (int i = 0; i < mr.Length; i++)
        {
            if (mr[i].materials.count > 0)
            {
                matList.Add(mr[i].materials);
                mr[i].SubtractCount(mr[i].materials.count);
            }
        }

        // Send the list of requested materials to RequestDropoff
        requestDropoff.SetMaterialRequest(matList);

        // Set material request counts to zero
        for (int i = 0; i < mr.Length; i++)
        {
            mr[i].materials.count = 0;
        }

        requestWindowVisible = false;
        RequestWindowOpenClose();
    }

    public void UpdateUI()
    {
        for (int i = 0; i < mr.Length; i++)
        {
            Text[] txt = materialPanels[i].GetComponentsInChildren<Text>();
            txt[1].text = "Stock: " + mr[i].GetCount() + "/" + mr[i].GetMaxCount();
            txt[2].text = mr[i].materials.count.ToString();

            materialPanels[i].GetComponentInChildren<Slider>().value = mr[i].GetProgress() / 100;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            playerInRange = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            playerInRange = false;

            requestWindowVisible = false;
            RequestWindowOpenClose();
        }
    }

    public void ToggleRequestWindow()
    {
        requestWindowVisible = !requestWindowVisible;
        RequestWindowOpenClose();
    }

    private void RequestWindowOpenClose()
    {
        if (requestWindowVisible)
        {
            requestCanvasGroup.alpha = 1;
            requestCanvasGroup.interactable = true;
            requestCanvasGroup.blocksRaycasts = true;
            closeWindow.SetActive(true);
        }
        else
        {
            requestCanvasGroup.alpha = 0;
            requestCanvasGroup.interactable = false;
            requestCanvasGroup.blocksRaycasts = false;
            closeWindow.SetActive(false);
        }
    }
}
