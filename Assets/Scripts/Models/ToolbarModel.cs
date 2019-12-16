using Assets.Scripts.Controllers.Factories;
using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Controllers.Factories.BlockTypesFactory;

public class ToolbarModel: MonoBehaviour
{

    public UIItemSlotModel[] slots;
    public RectTransform highlight;
    public int slotIndex = 0;

    private void Start()
    {
        BlockTypeKey blockType = BlockTypeKey.Bedrock;
        foreach (UIItemSlotModel s in slots) {            
            ItemStackModel stack = new ItemStackModel(blockType, Random.Range(2, 65));
            ItemSlot slot = new ItemSlot(s, stack);
            ++blockType;
        }
    }

    private void Update()
    {
        HandleScroll();
    }

    private void HandleScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0) {
            UpdateSlotIndex(scroll);
            highlight.position = slots[slotIndex].slotIcon.transform.position;
        }
    }

    private void UpdateSlotIndex(float scroll)
    {
        if (scroll > 0) {
            --slotIndex;
        } else {
            ++slotIndex;
        }
        if (slotIndex > slots.Length - 1) {
            slotIndex = 0;
        }
        if (slotIndex < 0) {
            slotIndex = slots.Length - 1;
        }
    }

    public UIItemSlotModel CurrentSlot
    {
        get => slots[slotIndex];
    }

    public bool CurrentSlotHasItem()
    {
        return CurrentSlot.HasItem;
    }

}
