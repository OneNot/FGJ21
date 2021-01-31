using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drawer : MonoBehaviour
{
    public bool IsOpen { get; private set; }

    [SerializeField]
    private GameObject DrawerClosedGO, DrawerOpenGO;
    [SerializeField]
    private Button DrawerClosedButton;
    [SerializeField]
    public RectTransform ItemContainer;

    public int DrawerID;

    private RectTransform rectTransform;
    private bool hovering;
    private Coroutine closingOperation;

    public Item SpawnItem(ItemPrefab itemPrefab)
    {
        Vector3[] corners = new Vector3[4];
        ItemContainer.GetWorldCorners(corners);
        Item spawnedItem = Instantiate(
        itemPrefab.Prefab,
            new Vector3(
                Random.Range(corners[0].x, corners[3].x),
                Random.Range(corners[0].y, corners[1].y),
                0f
            ),
            Quaternion.identity,
            ItemContainer
        ).GetComponent<Item>();

        RectTransform rt = spawnedItem.GetComponent<RectTransform>();
        float maxY = ((ItemContainer.position.y + (ItemContainer.rect.height / 2))) - (rt.rect.height / 2);
        float minY = ((ItemContainer.position.y - (ItemContainer.rect.height / 2))) + (rt.rect.height / 2);
        float maxX = ((ItemContainer.position.x + (ItemContainer.rect.width / 2))) - (rt.rect.width / 2);
        float minX = ((ItemContainer.position.x - (ItemContainer.rect.width / 2))) + (rt.rect.width / 2);
        if (rt.position.y > maxY)
            rt.position = new Vector3(rt.position.x, maxY, rt.position.z);
        if (rt.position.y < minY)
            rt.position = new Vector3(rt.position.x, minY, rt.position.z);
        if (rt.position.x > maxX)
            rt.position = new Vector3(maxX, rt.position.y, rt.position.z);
        if (rt.position.x < minX)
            rt.position = new Vector3(minX, rt.position.y, rt.position.z);

        rt.rotation = Quaternion.Euler(0f, 0f, Random.rotation.eulerAngles.z);

        spawnedItem.ContainingDrawer = this;
        spawnedItem.Name = itemPrefab.Name;

        //StartCoroutine(TryToFitItemInBetter(spawnedItem.GetComponent<RectTransform>()));

        return spawnedItem;
    }

    public bool IsMouseHoveringOnItemContainer()
    {
        Vector2 localMousePosition = ItemContainer.InverseTransformPoint(Input.mousePosition);
        return ItemContainer.rect.Contains(localMousePosition);
    }

    private void Awake()
    {
        Close();
        rectTransform = GetComponent<RectTransform>();
    }

    // IEnumerator TryToFitItemInBetter(RectTransform itemRectT)
    // {
    //     for (int y = 0; y < 25; y++)
    //     {
    //         for (int i = 0; i < 5; i++)
    //         {
    //             Vector3[] itemContainerCorners = new Vector3[4];
    //             ItemContainer.GetWorldCorners(itemContainerCorners);
    //             // Vector3[] itemRectTCorners = new Vector3[4];
    //             // itemRectT.GetWorldCorners(itemRectTCorners);

    //             if (!AllCornersInsideRect(itemContainerCorners, ItemContainer.rect))
    //             {
    //                 itemRectT.position = new Vector3(
    //                     Random.Range(itemContainerCorners[0].x, itemContainerCorners[3].x),
    //                     Random.Range(itemContainerCorners[0].y, itemContainerCorners[1].y),
    //                     0f
    //                 );
    //                 itemRectT.rotation = Quaternion.Euler(0f, 0f, Random.rotation.eulerAngles.z);
    //             }
    //             else
    //             {
    //                 Debug.Log("Did it?");
    //                 yield break;
    //             }
    //         }
    //         yield return null;
    //     }
    //     Debug.Log("Failed?");
    // }

    // private bool AllCornersInsideRect(Vector3[] corners, Rect rect)
    // {
    //     return (rect.Contains(corners[0]) && rect.Contains(corners[1]) && rect.Contains(corners[2]) && rect.Contains(corners[3]));
    // }

    private void OnEnable() {
        DrawerClosedButton.onClick.AddListener(Open);
    }
    private void OnDisable() {
        DrawerClosedButton.onClick.RemoveListener(Open);
    }

    public void Close()
    {
        DrawerOpenGO.SetActive(false);
        DrawerClosedGO.SetActive(true);
        IsOpen = false;
    }

    public void Open()
    {
        if (!DrawerHandler.Instance.OnlyAllowOpenOneAtATime || !DrawerHandler.Instance.Drawers.Exists(x => x.IsOpen))
        {
            if (closingOperation != null)
            StopCoroutine(closingOperation);

        DrawerOpenGO.SetActive(true);
        DrawerClosedGO.SetActive(false);
        IsOpen = true;
        }
    }

    private void OnPointerEnter()
    {
        if (closingOperation != null)
            StopCoroutine(closingOperation);
    }
    private void OnPointerExit()
    {
        if (IsOpen && DrawerHandler.Instance.AutoCloseDrawersOnPointerExit)
        {
            if (closingOperation != null)
                StopCoroutine(closingOperation);

            closingOperation = StartCoroutine(DelayedClose());
        }
    }

    private IEnumerator DelayedClose()
    {
        yield return new WaitForSeconds(DrawerHandler.Instance.DrawerAutoCloseDelay);
        Close();
    }

    private void Update()
    {
        Vector2 localMousePosition = rectTransform.InverseTransformPoint(Input.mousePosition);
        if (rectTransform.rect.Contains(localMousePosition))
        {
            if (!hovering)
                OnPointerEnter();
            hovering = true;
        }
        else
        {
            if (hovering)
                OnPointerExit();
            hovering = false;
        }
    }
}
