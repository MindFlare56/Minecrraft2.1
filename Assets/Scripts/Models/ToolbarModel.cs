using Assets.Scripts.Controllers.Factories;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Controllers.Factories.BlockTypesFactory;

public class ToolbarModel: MonoBehaviour
{

    [SerializeField] private UIItemSlotModel[] slots;
    public RectTransform highlight;
    public int slotIndex = 0;
    public static readonly int SIZE = 9;

    private void Start()
    {
       
    }  

    private void Update()
    {
        HandleScroll();
    }

    internal bool CurrentSlotHasItem()
    {
        return CurrentSlot.HasItem;
    }

    internal bool HasEmptyOrAddableSameItem(BlockTypeKey blockType)
    {
        foreach (var slot in Slots) {
            if (!slot.HasItem) {
                return true;
            }
            var stack = slot.itemSlot.stack;
            if (stack.blockType == blockType) {
                //todo handle this with multistack
                //if (stack.amount == 100) { return false; }
                return true;
            }           
        }
        return false;
    }

    internal bool HasSameItem(BlockTypeKey blockType)
    {
        foreach (var slot in Slots) {
            if (slot.itemSlot.stack.blockType == blockType) {
                return true;
            }
        }
        return false;
    }

    internal bool HasEmptySlot()
    {
        foreach (var slot in Slots) {
            if (!slot.HasItem) {
                return true;
            }
        }
        return false;
    }    

    internal UIItemSlotModel CurrentSlot
    {
        get => Slots[slotIndex];
    }
    public UIItemSlotModel[] Slots
    {
        get => slots;
        set => slots = value;
    }

    internal int AddBlock(BlockTypeKey blockType)
    {
        var slot = FindFirstEmptyOrSameSlot(blockType);
        if (slot != null) {
            AddOrCreateBlock(slot, blockType);
        }
        return slot.itemSlot.stack.amount;
    }

    private void AddOrCreateBlock(UIItemSlotModel slot, BlockTypeKey blockType)
    {
        if (slot.itemSlot == null) {
            slot.itemSlot = new ItemSlot(slot, new ItemStackModel(blockType, 1));
        } else {
            slot.AddAmount(1, blockType);
        }        
    }

    private UIItemSlotModel FindFirstEmptyOrSameSlot(BlockTypeKey blockType)
    {
        var slot = FindFirstSameSlot(blockType);
        if (slot == null) {
            slot = FindFirstEmptySlot();
        }
        return slot;
    }

    private UIItemSlotModel FindFirstSameSlot(BlockTypeKey blockType)
    {
        foreach (var slot in Slots) {
            if (slot.itemSlot != null) {
                if (slot.itemSlot.stack != null) {
                    if (slot.itemSlot.stack.blockType == blockType) {
                        return slot;
                    }
                }
            }            
        }
        return null;
    }

    private UIItemSlotModel FindFirstEmptySlot()
    {
        foreach (var slot in Slots) {
            if (!slot.HasItem) {
                return slot;
            }
        }
        return null;
    }

    private void HandleScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0) {
            UpdateSlotIndex(scroll);
            highlight.position = Slots[slotIndex].slotIcon.transform.position;
        }
    }

    private void UpdateSlotIndex(float scroll)
    {
        if (scroll > 0) {
            --slotIndex;
        } else {
            ++slotIndex;
        }
        if (slotIndex > Slots.Length - 1) {
            slotIndex = 0;
        }
        if (slotIndex < 0) {
            slotIndex = Slots.Length - 1;
        }
    }    
}
