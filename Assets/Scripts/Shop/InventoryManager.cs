using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<Item> inventory = new List<Item>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    public List<T> GetInterfaceList<T>()
    {
        List<T> output = new List<T>();
        foreach (Item item in inventory)
        {
            if (item is T interfaceObj)
                output.Add(interfaceObj);
        }

        return output;
    }
}
