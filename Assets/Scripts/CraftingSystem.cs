using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    [SerializeField]
    private RecipeData[] availabeRecipes;

    [SerializeField]
    private GameObject recipeUiPrefab;

    [SerializeField]
    private Transform recipesParent;

    [SerializeField]
    private KeyCode openCraftPanelInput;

    [SerializeField]
    private GameObject craftingPanel;

    void Start()
    {
        UpdateDisplayedRecipes();
    }

    void Update()
    {
        if(Input.GetKeyDown(openCraftPanelInput))
        {
            craftingPanel.SetActive(!craftingPanel.activeSelf);
            UpdateDisplayedRecipes();
        }
    }

    public void UpdateDisplayedRecipes()
    {
        foreach (Transform child in recipesParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < availabeRecipes.Length; i++)
        {
            GameObject recipe = Instantiate(recipeUiPrefab, recipesParent);
            recipe.GetComponent<Recipe>().Configure(availabeRecipes[i]);
        }
    }
}
