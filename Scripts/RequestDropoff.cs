using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequestDropoff : MonoBehaviour
{
    public GameObject itemPrefab;

    public List<GameObject> warningLights = new List<GameObject>();
    

    [SerializeField]
    private float coolDownTimer = 15;
    private float coolDown;
    private bool timerRunning = false;

    private bool dropLocationsFull = false;
    private GameObject[] dropLocations;

    [SerializeField]
    private List<Materials> mList = new List<Materials>();
    private List<int> requestSizeList = new List<int>();

    // If request drop off was full use this
    private int requestSizeOverflow;


    // Start is called before the first frame update
    void Start()
    {
        dropLocations = GameObject.FindGameObjectsWithTag("DropoffLocation");
        
        coolDown = coolDownTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerRunning)
        {
            coolDown -= Time.deltaTime;
            
            // Make sure there is something requested
            if (mList.Count > 0)
            {


                if (coolDown <= 0)
                {
                    coolDown = coolDownTimer;

                    // Make sure we know how many different materials were requested
                    if (requestSizeOverflow > 0)
                    {
                        for (int i = 0; i < requestSizeOverflow; i++)
                        {
                            GameObject dropSpot = GetDropoffLocation();
                            if (dropSpot == null) // All spots are full just wait
                            {
                                timerRunning = false;
                                dropLocationsFull = true;
                                requestSizeOverflow = requestSizeList[0] - i;
                                return;
                            }
                            DropItem(dropSpot);
                        }
                        requestSizeOverflow = 0;
                    }
                    else if (requestSizeList.Count > 0)
                    {
                        for (int i = 0; i < requestSizeList[0]; i++)
                        {
                            GameObject dropSpot = GetDropoffLocation();
                            if (dropSpot == null) // All spots are full just wait
                            {
                                timerRunning = false;
                                dropLocationsFull = true;
                                requestSizeOverflow = requestSizeList[0] - i;
                                requestSizeList.RemoveAt(0);
                                return;
                            }
                            DropItem(dropSpot);
                        }
                        requestSizeList.RemoveAt(0);
                    }
                    else
                    {
                        Debug.LogError("Something is wrong mList was greater than zero but requestSizeList was not. RequestSizeOverFlow is at zero also");
                    }
                }               
            }
            else
            {
                timerRunning = false;
            }
        }
    }

    public void DropItem(GameObject dropSpot)
    {

        Vector3 newPostion = new Vector3(dropSpot.transform.position.x, dropSpot.transform.position.y, 0);
        GameObject go = Instantiate(itemPrefab, newPostion, this.transform.rotation);
        go.transform.SetParent(dropSpot.transform);

        Item item = go.GetComponent<Item>();
        item.materials.materialName = mList[0].materialName;
        item.materials.count = mList[0].count;
        item.materials.spriteImage = mList[0].spriteImage;
        mList.RemoveAt(0);

        go.GetComponent<SpriteRenderer>().sprite = item.materials.spriteImage;
        go.GetComponentInChildren<Text>().text = item.materials.count.ToString();
    }

    /// <summary>
    /// Set list of requested of materials
    /// </summary>
    /// <param name="mat">List of materials to request</param>
    public void SetMaterialRequest(List<Materials> mat)
    {
        List<Materials> tempList = new List<Materials>();

        requestSizeList.Add(mat.Count);

        for (int i = 0; i < mat.Count; i++)
        {
            tempList.Add(new Materials());
            tempList[i].materialName = mat[i].materialName;
            tempList[i].count = mat[i].count;
            tempList[i].spriteImage = mat[i].spriteImage;
        }

        mList.AddRange(tempList);

        timerRunning = true;

    }

    /// <summary>
    /// Request a material 
    /// </summary>
    /// <param name="mat">Material requested</param>
    public void SetMaterialRequest(Materials mat)
    {
        Materials newMat = new Materials();
        newMat.materialName = mat.materialName;
        newMat.count = mat.count;
        newMat.spriteImage = mat.spriteImage;
        mList.Add(newMat);

        timerRunning = true;
    }

    /// <summary>
    /// Returns first empty dropoff location
    /// </summary>
    /// <returns>dropLocation</returns>
    private GameObject GetDropoffLocation()
    {
        for (int i = 0; i < dropLocations.Length; i++)
        {
            if(dropLocations[i].transform.childCount == 0)
            {
                return dropLocations[i];
            }
        }

        // If get here all spots are full
        return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (dropLocationsFull)
            {
                timerRunning = true;
                dropLocationsFull = false;
            }
        }
    }
}
