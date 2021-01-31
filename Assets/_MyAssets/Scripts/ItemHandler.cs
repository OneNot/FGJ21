using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    #region Static Instance Handling
    private static ItemHandler instance;
    public static ItemHandler Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<ItemHandler>();
            if (instance == null)
                Debug.LogError("Could not find ItemHandler");

            return instance;
        }
    }
    void OnDisable()
    {
        instance = null;
    }
    #endregion


    [SerializeField]
    public List<ItemPrefab> ItemPrefs;

    [Header("=== Don't edit below ===")]

    [SerializeField]
    private List<Item> ActiveItems = new List<Item>();
    public int NumberOfActiveItemsBeforeNoHerrings = 50;

    public Transform ItemDraggingLayer;

    public int RedHerringsForEachItem = 3;

    public int NumOfActiveItems {get { return ActiveItems.Count; }}

    public Item SpawnRandomItem()
    {
        Item spawnedItem = DrawerHandler.Instance.SpawnItemInRandomDrawer(GetRandomItemPrefab());
        ActiveItems.Add(spawnedItem);

        //spawn red herring items
        if (NumOfActiveItems < NumberOfActiveItemsBeforeNoHerrings)
            for (int i = 0; i <= Random.Range(0, RedHerringsForEachItem); i++)
                ActiveItems.Add(DrawerHandler.Instance.SpawnItemInRandomDrawer(GetRandomItemPrefab()));

        return spawnedItem;
    }

    public void RemoveItemFromActiveItems(Item item)
    {
        ActiveItems.Remove(item);
        Destroy(item.gameObject);
    }

    // public void AskForItem(ItemPrefabs item)
    // {
    //     AskedItems.Add(item);
    // }

    // public void GivePersonItemFromAskedItems(ItemPrefabs item)
    // {
    //     item.Owner.GetItemAndLeave();
    //     AskedItems.Remove(item);
    // }

    public ItemPrefab GetRandomItemPrefab()
    {
        return ItemPrefs[Random.Range(0, ItemPrefs.Count)];
    }

    public Item FindFromActiveItemsByName(string name)
    {
        return ActiveItems.Find(x => x.Name == name);
    }
}
