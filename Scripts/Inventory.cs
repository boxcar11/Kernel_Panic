using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Inventory : SingletonClass<Inventory>
{
    public GameObject materialEntryPrefab;
    public GameObject componentEntryPrefab;
    public GameObject recipeEntryPrefab;
    public GameObject inventoryEntryPrefab;

    public GameObject materialPanel;
    public GameObject componentPanel;
    public GameObject inventoryPanel;
    public GameObject repairPanel;

    [SerializeField]
    private Materials[] materialInventory;

    [SerializeField]
    private List<Components> componentInventory;

    private Crafting crafting;

    private List<Components> compTypes = new List<Components>();

    private List<GameObject> materialUIEnteries = new List<GameObject>();
    private List<GameObject> componentUIEnteries = new List<GameObject>();
    private List<GameObject> inventoryUIEnteries = new List<GameObject>();

    private Text craftButtonText;

    // Start is called before the first frame update
    void Start()
    {
        crafting = Crafting.Instance;

        for (int i = 0; i < crafting.recipes.Length; i++)
        {
            Components newComp = new Components();
            newComp.componentName = crafting.recipes[i].componentName;
            compTypes.Add(newComp);
        }

        // Set UI for all materials
        SetupMaterialUI();
        // Set UI for all components
        SetupComponentUI();
        // Set UI for all inventory
        RedrawInventoryUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Components GetComponentTypeAtIndex(int i)
    {
        return compTypes[i];
    }

    public Text GetCraftButtonText()
    {
        return craftButtonText;
    }

    public void TrashComponent()
    {
        int init = crafting.GetRepairSelect();
        RemoveComponent(componentInventory[init]);
    }

    #region Materials
    public void AddMaterial(materialEnum mat, int count)
    {
        for(int i = 0; i < materialInventory.Length; i++)
        {
            if(materialInventory[i].materialName == mat)
            {
                materialInventory[i].count += count;
            }
        }

        UpdateUI();
        crafting.UpdateUIElements();
    }

    public void RemoveMaterial(materialEnum mat, int count)
    {
        for (int i = 0; i < materialInventory.Length; i++)
        {
            if (materialInventory[i].materialName == mat)
            {
                materialInventory[i].count -= count;
            }
        }

        UpdateUI();
        crafting.UpdateUIElements();
    }

    public int GetMaterialCount(materialEnum mat)
    {
        for(int i = 0; i < materialInventory.Length; i++)
        {
            if(materialInventory[i].materialName == mat)
            {
                return materialInventory[i].count;
            }
        }

        // Should never get here
        Debug.LogError("Could not find: " + mat.ToString() + " in Machine.cs");
        return 0;
    }

    public int GetNumberOfMaterials()
    {
        return materialInventory.Length;
    }

    #endregion

    #region Components


    /// <summary>
    /// Add component to player inventory
    /// </summary>
    /// <param name="comp">Component to add</param>
    public void AddComponent(Components comp)
    {
        Components newComp = new Components();
        newComp.componentName = comp.componentName;
        newComp.count = comp.count;
        newComp.spriteImage = comp.spriteImage;
        newComp.health = comp.health;

        componentInventory.Add(newComp);

        UpdateUI();
        crafting.UpdateUIElements();
    }

    /// <summary>
    /// Remove component from player inventory
    /// </summary>
    /// <param name="comp">Component to remove</param>
    public void RemoveComponent(Components comp)
    {
        int temp = componentInventory.IndexOf(comp);
        //inventoryUIEnteries.RemoveAt(temp);
        componentInventory.Remove(comp);

        UpdateUI();
        crafting.UpdateUIElements();
    }

    /// <summary>
    /// Get count of a component
    /// </summary>
    /// <param name="comp"></param>
    /// <returns></returns>
    public int GetComponentCount(Components comp)
    {
        return comp.count;
    }

    /// <summary>
    /// Returns number of components in componentInventory
    /// </summary>
    /// <returns>Component Inventory</returns>
    public int GetNumberOfComponents()
    {
        return componentInventory.Count;
    }

    public Components GetComponentAtIndex(int index)
    {
        if(index > componentInventory.Count - 1)
        {
            return null;
        }
        return componentInventory[index];
    }

    // Returns list of component UI Objects
    public List<GameObject> GetComponentUIElements()
    {
        return componentUIEnteries;
    }


    #endregion

    public void UpdateUI()
    {
            for (int i = 0; i < componentUIEnteries.Count; i++)
            {
                Text[] textObj = componentUIEnteries[i].GetComponentsInChildren<Text>();

            }

            for (int i = 0; i < materialUIEnteries.Count; i++)
            {
                Text[] textObj = materialUIEnteries[i].GetComponentsInChildren<Text>();
                textObj[1].text = materialInventory[i].count.ToString();
            }

            RedrawInventoryUI();
            RedrawRepairPanel();
    }

    private void RedrawInventoryUI()
    {
        // Need to get rid of all the old objects 
        // Get rid of all component entries
        foreach (Transform child in inventoryPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // Clear the list
        inventoryUIEnteries.Clear();

        // Now that old objects are gone, draw new ones
        for (int i = 0; i < componentInventory.Count; i++)
        {
            // Create new inventory entry and parent it to inventory panel
            GameObject go = Instantiate(inventoryEntryPrefab);
            inventoryUIEnteries.Add(go);
            go.transform.SetParent(inventoryPanel.transform, false);

            // Set Images for inventory
            int init = i;
            Button btn = go.transform.GetChild(0).GetComponentInChildren<Button>();
            btn.onClick.AddListener(delegate { crafting.RepairSelect(init); });
            btn.image.sprite = componentInventory[i].spriteImage;

            TextMeshProUGUI txt = go.transform.GetComponentInChildren<TextMeshProUGUI>();
            txt.text = (int)componentInventory[i].health + "/100";

            Slider slide = go.transform.GetComponentInChildren<Slider>();
            slide.value = componentInventory[i].health / 100;

            if (componentInventory[i].health > 35)
            {               
                if (crafting.GetIsCrafting() == false && i == crafting.GetRepairSelect())
                {
                    inventoryUIEnteries[i].GetComponent<Image>().color = crafting.ComponentSelectColor;
                }
                else
                {
                    inventoryUIEnteries[i].GetComponent<Image>().color = Color.white;
                }
            }
            else
            {
                inventoryUIEnteries[i].GetComponent<Image>().color = Color.red;
            }

        }
    }

    private void RedrawRepairPanel()
    {
        string recipeString = "";
        int init = 0;
        int repairSelect = crafting.GetRepairSelect();

        // Check if part is repairable
        if(componentInventory.Count <= 0)
        {
            return;
        }

        if (componentInventory[repairSelect].health > 35)
        {
            // Find repair recipes index for selected component
            for (int i = 0; i < crafting.repairRecipes.Length; i++)
            {
                if (componentInventory[repairSelect].componentName == crafting.repairRecipes[i].componentName)
                {
                    init = i;
                }
            }

            Recipes recipe = crafting.repairRecipes[init];



            for (int i = 0; i < recipe.requiredMaterials.Length; i++)
            {
                recipeString += recipe.requiredMaterials[i].materialName.ToString() + ": " + recipe.requiredMaterials[i].count.ToString();
                if(i != recipe.requiredMaterials.Length - 1)
                {
                    recipeString += "\n";
                }
            }
        }
        else
        {
            recipeString = "Unrepairable";
        }

        Text repairCostText = repairPanel.transform.GetChild(1).GetComponent<Text>();
        repairCostText.text = recipeString;

        crafting.CheckForRepairMaterials(init);

        repairCostText.color = (crafting.GetHaveMaterials() && crafting.GetIsCrafting() == false) ? crafting.haveMaterialColor : crafting.missingMaterialColor;

    }

    #region Setup

    private void SetupMaterialUI()
    {
        for (int i = 0; i < materialInventory.Length; i++)
        {
            // Create new material entry and parent it to material panel
            GameObject go = Instantiate(materialEntryPrefab);
            materialUIEnteries.Add(go);
            go.transform.SetParent(materialPanel.transform, false);

            // Set text for materials
            Text[] textObj = go.GetComponentsInChildren<Text>();
            textObj[0].text = materialInventory[i].materialName.ToString();
            textObj[1].text = materialInventory[i].count.ToString();

            // Set Images for materials
            Image img = go.transform.GetChild(2).GetComponent<Image>();
            img.sprite = materialInventory[i].spriteImage;
        }
    }

    private void SetupComponentUI()
    {
        // Get reference to craft button
        craftButtonText = componentPanel.transform.parent.GetChild(2).GetComponentInChildren<Text>();

        for (int i = 0; i < compTypes.Count; i++)
        {
            // Create new componenet Entry and parent it to componentPanel
            GameObject go = Instantiate(componentEntryPrefab);
            componentUIEnteries.Add(go);
            go.transform.SetParent(componentPanel.transform, false);

            int init = i;
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { crafting.CraftingSelect(init); });

            // Get reciped from crafting scripte
            Recipes[] recipes = crafting.GetRecipes();

            // Run through list of recipes to find the one that matches with the component being set
            for (int j = 0; j < recipes.Length; j++)
            {
                if (compTypes[i].componentName == recipes[j].componentName)
                {
                    // Add a material entry for every required material in the recipe
                    for (int p = 0; p < recipes[j].requiredMaterials.Length; p++)
                    {
                        GameObject recipeGO = Instantiate(recipeEntryPrefab);
                        recipeGO.transform.SetParent(go.transform.GetChild(1), false);

                        Text[] textGO = recipeGO.GetComponentsInChildren<Text>();
                        textGO[0].text = recipes[j].requiredMaterials[p].materialName.ToString();
                        textGO[1].text = recipes[j].requiredMaterials[p].count.ToString();

                        // Set images for components
                        Image img = go.transform.GetChild(0).GetComponentInChildren<Image>();
                        img.sprite = componentInventory[i].spriteImage;
                    }

                    //// Adjust size of recipe list based on number of enteries
                    //RectTransform rt = go.transform.GetChild(1).GetComponent<RectTransform>();
                    //rt.sizeDelta = new Vector2(rt.sizeDelta.x, recipes[j].requiredMaterials.Length * 20);

                    //RectTransform rt2 = go.transform.GetComponent<RectTransform>();
                    //rt2.sizeDelta = new Vector2(rt2.sizeDelta.x, (recipes[j].requiredMaterials.Length * 20) + 40);
                }
            }

            // Adjust the min size of the component panel based of number of required materials
            //LayoutElement le = go.GetComponent<LayoutElement>();
            //le.minHeight = le.minHeight + (recipes.Length * 20);

            Text[] textObj = go.GetComponentsInChildren<Text>();
            textObj[0].text = compTypes[i].componentName.ToString();
            RectTransform textRT = textObj[0].GetComponent<RectTransform>();
            textRT.sizeDelta = new Vector2(textRT.sizeDelta.x, go.GetComponent<RectTransform>().sizeDelta.y);
            //textObj[1].text = compTypes[i].count.ToString();
        }
    }
    #endregion
}
