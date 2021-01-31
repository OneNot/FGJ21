using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragger : EventTrigger {

    private bool dragging;
    private Vector3 dragStartPoint;
    private Transform dragStartParent;
    private Item item;

    private void Awake() {
        item = GetComponent<Item>();
    }

    public void Update() {
        if (dragging) {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }
    public override void OnPointerDown(PointerEventData eventData) {
        dragging = true;
        dragStartPoint = transform.position;
        dragStartParent = transform.parent;
        transform.SetParent(ItemHandler.Instance.ItemDraggingLayer);
    }

    public override void OnPointerUp(PointerEventData eventData) {
        dragging = false;
        bool validDropPoint = false;

        foreach (Drawer d in DrawerHandler.Instance.Drawers)
        {
            if (d.IsMouseHoveringOnItemContainer())
            {
                transform.SetParent(d.ItemContainer);
                item.ContainingDrawer = d;
                validDropPoint = true;
                break;
            }
        }
        if (!validDropPoint)
        {
            foreach (Person p in PersonSpawner.Instance.ActivePersons)
            {
                if (p.IsMouseHoveringOnPersonWithRightItem(item))
                {
                    p.GivePersonItem(item);
                    return;
                }
            }

            transform.position = dragStartPoint;
            transform.SetParent(dragStartParent);
        }
    }
}