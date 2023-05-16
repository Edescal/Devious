using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Edescal;

public class Inventory : MonoBehaviour
{
    public IEntity Owner { get; private set; }
    public UnityEvent<List<Item>> onInventoryUpdate;
    public List<Item> items = new List<Item>();

    public void AddItem(Item item)
    {
        if (item == null) return;

        if (!items.Contains(item))
        {
            items.Add(item);
            onInventoryUpdate?.Invoke(items);
        }
    }

    public void RemoveItem(Item item)
    {
        if (item == null) return;

        if (items.Contains(item))
        {
            items.Remove(item);
            onInventoryUpdate?.Invoke(items);
        }
    }

    public bool HasItem(Item item)
    {
        if (items.Contains(item))
        {
            return true;
        }

        return false;
    }
}