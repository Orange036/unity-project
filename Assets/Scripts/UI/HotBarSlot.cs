using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotBarSlot : MonoBehaviour
{

    [SerializeField] private int slotIndex;
    [SerializeField] private Inventory inventory;

    private void Awake()
    {
        inventory.InventoryChanged += Inventory_InventoryChanged;
    }

    private void Inventory_InventoryChanged(object o, ItemDataArr arr)
    {
        Image image = GetComponent<Image>();
        if (arr.dataArray[slotIndex] != null)
        {
            image.enabled = true;
            image.sprite = arr.dataArray[slotIndex].spriteOfItself;
        }
        else
        {
            image.enabled = false;
        }
    }
}
