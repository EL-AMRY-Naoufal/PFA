using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Item",menuName="Items/New item")]

public class ItemData : ScriptableObject
{
    [Header("Data")]
    public string Name;
    public string Description;
    public Sprite Visual;
    public GameObject Prefab;
    public bool stackable;
    public int maxStack;

    [Header("Effects")]
    public float healthEffect;
    public float hungerEffect;
    public float thirstEffect;

    [Header("Armor Stats")]
    public float armorPoints;

    [Header("Types")]
    public ItemType itemtype;
    public EquipmentType equipmentType;

    [Header("Atack Stats")]
    public float attackPoints;
}

public enum ItemType
{
    Ressource,
    Equipment,
    Consumable,
}

public enum EquipmentType
{
    Head,
    Chest,
    Hands,
    Legs,
    Feet,
    Weapon
}