using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerHandler : MonoBehaviour
{
    #region Static Instance Handling
    private static DrawerHandler instance;
    public static DrawerHandler Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<DrawerHandler>();
            if (instance == null)
                Debug.LogError("Could not find DrawerHandler");

            return instance;
        }
    }
    void OnDisable()
    {
        instance = null;
    }
    #endregion


    [SerializeField]
    public List<Drawer> Drawers;

    public bool AutoCloseDrawersOnPointerExit = true;
    public float DrawerAutoCloseDelay = 3f;

    public bool OnlyAllowOpenOneAtATime = true;



    public Item SpawnItemInRandomDrawer(ItemPrefab itemPrefab)
    {
        Item spawnedItem = Drawers[Random.Range(0, Drawers.Count)].SpawnItem(itemPrefab);
        return spawnedItem;
    }
}
