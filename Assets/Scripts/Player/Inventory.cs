
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDataArr : EventArgs
{
    public ItemData[] dataArray { get; set; }

    public ItemDataArr(ItemData[] dataArr)
    {
        this.dataArray = dataArr;
    }
}

[CreateAssetMenu(fileName = "InventoryObject", menuName = "Scriptable Objects/Inventory Object")]
public class Inventory : ScriptableObject
{

    [SerializeField]private ItemData[] inventory = new ItemData[4];

    public event EventHandler<ItemDataArr> InventoryChanged; 

    public bool AddInInventory(int position, ItemData item)
    {
        if(position >= 0 && position < inventory.Length)
        {
            if (inventory[position] == null)
            {
                inventory[position] = item;
                InventoryChanged?.Invoke(null, new ItemDataArr(inventory));
                return true;
            }

        }
        return false;
    }

    public GameObject RemoveFromInventory(int position)
    {
        if (position >= 0 && position < inventory.Length)
        {
            if (inventory[position] != null)
            {
                GameObject item = inventory[position].pefabOfItself;
                inventory[position] = null;
                InventoryChanged?.Invoke(null, new ItemDataArr(inventory));

                return item;
            }

        }
        return null;
    }

    public void ClearInventory()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] != null)
            {
                inventory[i] = null;
            }
        }
        InventoryChanged?.Invoke(null, new ItemDataArr(inventory));
    }
}
