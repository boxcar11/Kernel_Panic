using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crafting : SingletonClass<Crafting>
{
    public Color haveMaterialColor;
    public Color missingMaterialColor;
    public Color ComponentSelectColor;

    public CanvasGroup craftingCanvasGroup;

    [SerializeField]
    [Tooltip("Chance out of that part will be damaged repairing.")]
    private int chanceToDamage = 50;

    [SerializeField]
    public Recipes[] recipes;
    [SerializeField]
    public Recipes[] repairRecipes;

    private Inventory inventory;
    private bool haveMaterials = false;
    private int toCraft;
    private int toRepair;
    private bool craftWindowVisible;
    private bool playerInRange = false;

    private bool isCrafting = true;

    private List<GameObject> componentUIElements;
    private Color defaultComponentColor;

    private AudioSource audioSource;
    public AudioClip craftClip;
    public AudioClip breakIt;

    // Start is called before the first frame update
    void Start()
    {
        inventory = Inventory.Instance;

        

        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(componentUIElements == null)
        {
            GetUIElements();
        }

        if (Input.GetButtonUp("Fire3") && playerInRange)
        {
            ToggleCraftingWindow();
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            playerInRange = true;
            UpdateUIElements();
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            playerInRange = false;

            craftWindowVisible = false;
            CraftingWindowOpenClose();
        }
    }

    /// <summary>
    /// Crafting if true and repairing if false
    /// </summary>
    /// <returns>isCrafting</returns>
    public bool GetIsCrafting()
    {
        return isCrafting;
    }

    /// <summary>
    /// Returns index of component selected for repair
    /// </summary>
    /// <returns>toRepair</returns>
    public int GetRepairSelect()
    {
        return toRepair;
    }

    public bool GetHaveMaterials()
    {
        return haveMaterials;
    }

    /// <summary>
    /// Selects action based on status of isCrafting
    /// </summary>
    public void ButtonAction()
    {
        if(isCrafting)
        {
            CraftItem();
        }
        else
        {
            RepairItem();
        }

        UpdateUIElements();
        inventory.UpdateUI();
    }

    private void CraftItem()
    {
        Recipes recipe = recipes[toCraft];
        for (int i = 0; i < recipe.requiredMaterials.Length; i++)
        {
            if (inventory.GetMaterialCount(recipe.requiredMaterials[i].materialName) >= recipe.requiredMaterials[i].count)
            {
                haveMaterials = true;
            }
            else
            {
                haveMaterials = false;
                return;
            }
        }

        if(haveMaterials)
        {
            for(int i = 0; i < recipe.requiredMaterials.Length; i++)
            {
                inventory.RemoveMaterial(recipe.requiredMaterials[i].materialName, recipe.requiredMaterials[i].count);
            }

            Components newComp = new Components();
            newComp.componentName = recipe.componentName;
            newComp.spriteImage = recipe.spriteImage;
            newComp.count = 1;

            audioSource.clip = craftClip;
            audioSource.Play();

            inventory.AddComponent(newComp);
        }
    }

    private void RepairItem()
    {
        

        int rand = Random.Range(0, chanceToDamage);

        int init = 0;
        Components comp = inventory.GetComponentAtIndex(toRepair);
        if(rand == chanceToDamage - 1)
        {
            audioSource.clip = breakIt;
            audioSource.Play();
            comp.health = 15;
            return;
        }

        if (comp.health < 100 && comp.health > 35)
        {
            // Find repair recipes index for selected component
            for (int i = 0; i < repairRecipes.Length; i++)
            {
                if (comp.componentName == repairRecipes[i].componentName)
                {
                    init = i;
                }
            }

            CheckForRepairMaterials(init);

            if (haveMaterials)
            {
                for (int i = 0; i < repairRecipes[init].requiredMaterials.Length; i++)
                {
                    inventory.RemoveMaterial(repairRecipes[init].requiredMaterials[i].materialName, repairRecipes[init].requiredMaterials[i].count);
                }

                comp.health = 100;
            }
        }
    }

    public void CheckForRepairMaterials(int init)
    {
        // Check if have enough materials
        for (int i = 0; i < repairRecipes[init].requiredMaterials.Length; i++)
        {
            if (inventory.GetMaterialCount(repairRecipes[init].requiredMaterials[i].materialName) >= repairRecipes[init].requiredMaterials[i].count)
            {
                haveMaterials = true;
            }
            else
            {
                haveMaterials = false;
                return;
            }
        }
    }

    // Called from dropdown window to set which component to craft
    public void CraftingSelect(int val)
    {
        inventory.GetCraftButtonText().text = "Craft";
        isCrafting = true;
        toCraft = val;
        UpdateUIElements();
        inventory.UpdateUI();
    }

    public void RepairSelect(int val)
    {
        inventory.GetCraftButtonText().text = "Repair";
        isCrafting = false;
        toRepair = val;
        UpdateUIElements();
        inventory.UpdateUI();
    }

    public Recipes[] GetRecipes()
    {
        return recipes;
    }

    public Recipes[] GetRepairRecipes()
    {
        return repairRecipes;
    }

    public bool CraftingWindowOpen()
    {
        if(craftWindowVisible)
        {
            return true;
        }

        return false;
    }

    public void GetUIElements()
    {
        componentUIElements = inventory.GetComponentUIElements();
        if (componentUIElements != null)
        {
            defaultComponentColor = componentUIElements[0].GetComponent<Image>().color;
        }
    }

    public void UpdateUIElements()
    {
        if(componentUIElements == null)
        {
            return;
        }
        
        for (int i = 0; i < componentUIElements.Count; i++)
        {
            if(toCraft == i && isCrafting)
            {
                componentUIElements[i].GetComponent<Image>().color = ComponentSelectColor;
            }
            else
            {
                componentUIElements[i].GetComponent<Image>().color = defaultComponentColor;
            }

            Text[] textObj = componentUIElements[i].transform.GetChild(1).GetComponentsInChildren<Text>();
            for (int q = 0; q < textObj.Length; q++)
            {
                for (int j = 0; j < recipes.Length; j++)
                {
                    if (inventory.GetComponentTypeAtIndex(i).componentName == recipes[j].componentName)
                    {
                        // Add a material entry for every required material in the recipe
                        for (int p = 0; p < recipes[i].requiredMaterials.Length; p++)
                        {
                            // Check if q is odd or even and make sure that required material matches up with correct UI text
                            if (q % 2 == 1 && recipes[j].requiredMaterials[p].materialName.ToString() == textObj[q - 1].text)
                            {
                                // If have enough materials set color to haveMaterialColor otherwise set it to missingMaterialColor
                                textObj[q].color = (inventory.GetMaterialCount(recipes[i].requiredMaterials[p].materialName) >= recipes[i].requiredMaterials[p].count) ? haveMaterialColor : missingMaterialColor;
                            }
                        }
                    }
                }
            }
        }
    }

    public void ToggleCraftingWindow()
    {
        craftWindowVisible = !craftWindowVisible;
        CraftingWindowOpenClose();
    }

    private void CraftingWindowOpenClose()
    {
        if (craftWindowVisible)
        {
            craftingCanvasGroup.alpha = 1;
            craftingCanvasGroup.interactable = true;
            craftingCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            craftingCanvasGroup.alpha = 0;
            craftingCanvasGroup.interactable = false;
            craftingCanvasGroup.blocksRaycasts = false;
            toRepair = 0;
        }
    }
}
