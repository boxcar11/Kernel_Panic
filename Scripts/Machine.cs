using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Machine : MonoBehaviour
{
    [Header("Component attributes")]
    [Tooltip("Type of component accepted.")]
    public componentEnum componentType;
    [Tooltip("Number of components allowed.")]
    public int numberOfComponents = 1;
    [Tooltip("Rate at which components receive damage.")]
    public float componentDecayRate = 5;

    [Header("Random Chance")]
    [Tooltip("Min number of cycles before next component damage.")]
    public int minChance = 10;
    [Tooltip("Max number of cycles bedore next component damage.")]
    public int maxChance = 20;
    [Tooltip("Amount of damage a component receives at random.")]
    public float randDamage;
    [Tooltip("Max damage applied to a component.")]
    public int maxDamage;
    [Tooltip("Min damage applied to a component.")]
    public int minDamage;
    private int randomValue;

    [Header("Object references")]
    public Slider healthBar;
    public Text machineText;
    public Image fillImage;
    public Color healthyColor;
    public Color damagedColor;

    public bool hasVisibleSprite;
    [Tooltip("List for sprites of component to be toggled as added/removed.")]
    public List<GameObject> componentSprite = new List<GameObject>();

    [Header("Component Canvas Prefabs")]
    public GameObject CDPrefab;
    public CanvasGroup componentCanvasGroup;

    [SerializeField]
    private List<Components> components;

    private Text componentCanvasText;
    private List<Button> componentCanvasButtons = new List<Button>();   
    private List<Slider> componentCanvasSliders = new List<Slider>();
    private List<TextMeshProUGUI> componentCanvasTMPros = new List<TextMeshProUGUI>();

    private List<Button> inventoryButtons = new List<Button>();
    private List<Slider> inventorySliders = new List<Slider>();
    private List<TextMeshProUGUI> inventoryTMPros = new List<TextMeshProUGUI>();

    private Inventory inventory;

    private float currentHealth;
    private float healthPercent;
    
    private bool ComponentWindowVisible;
    private bool playerInRange = false;
    private bool hasComponents;

    private List<AlertTile> alertTiles = new List<AlertTile>();

    // Start is called before the first frame update
    void Start()
    {
        inventory = Inventory.Instance;

        machineText = gameObject.GetComponentInChildren<Text>();
        //machineText.text = gameObject.name;
        randomValue = Random.Range(minChance, maxChance);

        if (components.Count > 0)
        {
            hasComponents = true;
            StartCoroutine(CheckForDamage());
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Damage the component parts  
        for (int i = 0; i < components.Count; i++)
        {
            components[i].health -= (componentDecayRate / 10) * Time.deltaTime;
        }

        if (components.Count > 0 && hasComponents == false)
        {
            hasComponents = true;
            StartCoroutine(CheckForDamage());
        }
        
        if(components.Count <= 0)
        {
            hasComponents = false;
            StopCoroutine(CheckForDamage());
        }

        if (playerInRange && Input.GetButtonUp("Fire3"))
        {
            ToggleComponentWindow();
        }


        updateUI();
    }

    IEnumerator CheckForDamage()
    {
        while (hasComponents)
        {
            randomValue--;
            //Debug.Log(randomValue);
            if (randomValue == 0)
            {
                int random = Random.Range(1, 100);
                if (random >= minChance || random <= maxChance)
                {
                    DamageComponent();
                }
                randomValue = Random.Range(minChance, maxChance);
            }
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }

    public void DamageComponent()
    {
        int rand = Random.Range(0, components.Count);

        randDamage = Random.Range(minDamage, maxDamage);

        components[rand].health -= randDamage;
        //Debug.Log("Damaged " + randDamage);
    }

    public float GetHealthPercent()
    {
        return healthPercent;
    }

    public void AddTile(AlertTile tile)
    {
        alertTiles.Add(tile);
    }

    public AlertTile GetTileAtIndex(int index)
    {
        return alertTiles[index];
    }

    public List<AlertTile> GetTiles()
    {
        return alertTiles;
    }

    /// <summary>
    /// Get number of components in the machine
    /// </summary>
    /// <param name="comp"></param>
    /// <returns></returns>
    public int GetComponentCount()
    {
        return components.Count;
    }

    public Components GetComponentAtIndex(int index)
    {
        return components[index];
    }

    public void AddComponent(componentEnum comp, int count)
    {
        for (int i = 0; i < components.Count; i++)
        {
            if (components[i].componentName == comp)
            {
                components[i].count += count;
            }

        }
    }

    public bool AddComponentWithReturn(componentEnum comp, int count)
    {
        for (int i = 0; i < components.Count; i++)
        {


            if (components[i].componentName == comp)
            {
                components[i].count += count;
                return true;
            }
        }
        return false;
    }

    public void GetComponentFromInventory(Components c, Slider slide, Button btn)
    {
        if (c.componentName == componentType && components.Count < numberOfComponents)
        {
            inventory.RemoveComponent(c);
            components.Add(c);
            inventorySliders.Remove(slide);
            inventoryButtons.Remove(btn);
            if(hasVisibleSprite == true)
                TurnOnSprite();
        }

        ConnectToComponentCanvas();
    }

    public void ConnectToComponentCanvas()
    {
        GameObject cc = GameObject.FindGameObjectWithTag("ComponentCanvas");
        GameObject pic = GameObject.FindGameObjectWithTag("PlayerInventoryCanvas");

        // Get rid of all component entries in componemt Canvas
        foreach(Transform child in cc.transform.GetChild(1))
        {
            GameObject.Destroy(child.gameObject);
        }

        // Get rid all component entries in Inventory Canvas
        foreach(Transform child in pic.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // Clear all the lists
        inventoryButtons.Clear();
        inventorySliders.Clear();
        inventoryTMPros.Clear();
        componentCanvasButtons.Clear();
        componentCanvasSliders.Clear();
        componentCanvasTMPros.Clear();


        DrawMachineInventory(cc);
        DrawPlayerInventory(pic);

        ComponentWindowVisible = true;
    }

    public void DrawMachineInventory(GameObject cc)
    {
        for (int i = 0; i < components.Count; i++)
        {
            GameObject go = Instantiate(CDPrefab);
            go.transform.SetParent(cc.transform.GetChild(1));

            Slider slide = go.transform.GetComponentInChildren<Slider>();
            componentCanvasSliders.Add(slide);

            TextMeshProUGUI txt = go.transform.GetComponentInChildren<TextMeshProUGUI>();
            componentCanvasTMPros.Add(txt);

            int init = i;
            Button btn = go.transform.GetChild(0).GetComponentInChildren<Button>();
            btn.image.sprite = components[i].spriteImage;
            btn.onClick.AddListener(delegate { MoveToInventory(components[init], slide, btn); });
            componentCanvasButtons.Add(btn);

            
        }

        cc.GetComponentInChildren<Text>().text = this.name;
    }

    /// <summary>
    /// Draws Player inventory in Component Canvas
    /// </summary>
    public void DrawPlayerInventory(GameObject pic)
    {
        for (int i = 0; i < inventory.GetNumberOfComponents(); i++)
        {
            GameObject go = Instantiate(CDPrefab);
            go.transform.SetParent(pic.transform);

            Slider slide = go.transform.GetComponentInChildren<Slider>();
            inventorySliders.Add(slide);

            TextMeshProUGUI txt = go.transform.GetComponentInChildren<TextMeshProUGUI>();
            inventoryTMPros.Add(txt);

            Components comp = inventory.GetComponentAtIndex(i);
            Button btn = go.transform.GetChild(0).GetComponentInChildren<Button>();
            btn.image.sprite = comp.spriteImage;
            btn.onClick.AddListener(delegate { GetComponentFromInventory(comp, slide, btn); });
            inventoryButtons.Add(btn);

            
        }
    }

    public void MoveToInventory(Components c, Slider slide, Button btn)
    {
        inventory.AddComponent(c);
        components.Remove(c);
        componentCanvasSliders.Remove(slide);
        componentCanvasButtons.Remove(btn);

        if(hasVisibleSprite == true)
            TurnOffSprite();

        ConnectToComponentCanvas();
    }

    private void TurnOnSprite()
    {
        bool turnedOn = false;
        int i = componentSprite.Count-1;

        while (turnedOn == false)
        {
            if (componentSprite[i].activeSelf == false)
            {
                componentSprite[i].SetActive(true);
                turnedOn = true;
            }
            else
            {
                i--;
            }
        }
    }

    private void TurnOffSprite()
    {
        bool turnedOff = false;
        int i = 0;

        while(turnedOff == false)
        {    
            if (componentSprite[i].activeSelf == true)
            {
                componentSprite[i].SetActive(false);
                turnedOff = true;
            }
            else
            {
                //turnedOff = true;
                i++;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Player has enter trigger for: " + this.gameObject.name);
        if(other.tag == "Player")
        {
            playerInRange = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            playerInRange = false;
            ComponentWindowVisible = false;
            ComponentWindowOpenClose();
        }
    }

    public void ToggleComponentWindow()
    {
        ComponentWindowVisible = !ComponentWindowVisible;
        ComponentWindowOpenClose();
    }

    private void ComponentWindowOpenClose()
    {
        if (ComponentWindowVisible)
        {
            ConnectToComponentCanvas();
            componentCanvasGroup.alpha = 1;
            componentCanvasGroup.interactable = true;
            componentCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            componentCanvasGroup.alpha = 0;
            componentCanvasGroup.interactable = false;
            componentCanvasGroup.blocksRaycasts = false;
        }
    }

    private void updateUI()
    {
        for (int i = 0; i < componentCanvasSliders.Count; i++)
        {
            {
                componentCanvasSliders[i].value = components[i].health / 100;
            }
        }

        for (int i = 0; i < inventorySliders.Count; i++)
        {
            Components comp = inventory.GetComponentAtIndex(i);
            if (comp != null)
            {
                inventorySliders[i].value = comp.health / 100;
            }
        }

        for (int i = 0; i < componentCanvasTMPros.Count; i++)
        {
            componentCanvasTMPros[i].text = (int)components[i].health + "/100";
        }

        for (int i = 0; i < inventoryTMPros.Count; i++)
        {
            Components comp = inventory.GetComponentAtIndex(i);
            if (comp != null)
            {
                inventoryTMPros[i].text = (int)comp.health + "/100";
            }
        }
    }
}
