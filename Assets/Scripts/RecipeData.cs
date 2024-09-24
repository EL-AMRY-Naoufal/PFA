using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Recipe",menuName="Recipe/New Recipe")]
public class RecipeData : ScriptableObject
{
    public ItemData craftableItem;
    public ItemInInventory[] requiredItems;
}
