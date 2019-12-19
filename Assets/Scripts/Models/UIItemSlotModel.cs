using Assets.Scripts.Controllers;
using Assets.Scripts.Controllers.Factories;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Controllers.Factories.BlockTypesFactory;

public class UIItemSlotModel: MonoBehaviour
{

    public bool isLinked = false;
    public ItemSlot itemSlot;
    public Image slotImage;
    public Image slotIcon;
    public Text slotAmount;
    private ChunksController chunksController;

    private void Start()
    {
        chunksController = GameObject.Find("Chunks").GetComponent<ChunksController>();
    }

    private void OnDestroy()
    {
        if (itemSlot != null) {
            itemSlot.unLinkUISlot();
        }
    }

    public void Link(ItemSlot _itemSlot)
    {
        itemSlot = _itemSlot;
        isLinked = true;
        itemSlot.LinkUISlot(this);
        UpdateSlot();
    }

    public void UnLink()
    {
        itemSlot.unLinkUISlot();
        itemSlot = null;
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        if (itemSlot != null && itemSlot.HasItem) {
            slotIcon.sprite = chunksController.Blocktypes[itemSlot.stack.blockType].Icon;
            slotAmount.text = itemSlot.stack.amount.ToString();
            slotIcon.enabled = true;
            slotAmount.enabled = true;
        } else if (slotIcon != null) {
            Clear();
        }
    }

    public void Clear()
    {
        slotIcon.sprite = null;
        slotAmount.text = "";
        slotIcon.enabled = false;
        slotAmount.enabled = false;
    }

    internal void AddAmount(int value, BlockTypeKey blockType)
    {
        if (itemSlot.stack != null) {
            itemSlot.stack.amount += value;
            UpdateSlot();
        } else {               
            itemSlot.stack = new ItemStackModel(blockType, 1);
            UpdateSlot();            
        }        
    }

    public bool HasItem
    {
        get {
            return itemSlot == null ? false : itemSlot.HasItem;
        }
    }         
}

public class ItemSlot
{

    public ItemStackModel stack = null;
    private UIItemSlotModel uiItemSlot = null;
    public bool isCreative;

    public ItemSlot(UIItemSlotModel uiItemSlot)
    {
        stack = null;
        this.uiItemSlot = uiItemSlot;
        this.uiItemSlot.Link(this);
    }

    public ItemSlot(UIItemSlotModel uiItemSlot, ItemStackModel stack)
    {
        this.stack = stack;
        this.uiItemSlot = uiItemSlot;
        this.uiItemSlot.Link(this);
    }

    public void LinkUISlot(UIItemSlotModel uiSlot)
    {
        uiItemSlot = uiSlot;
    }

    public void unLinkUISlot()
    {
        uiItemSlot = null;
    }

    public void EmptySlot()
    {
        stack = null;
        if (uiItemSlot != null) {
            uiItemSlot.UpdateSlot();
        }
    }

    public int Take(int amount)
    {
        if (amount > stack.amount) {
            amount = stack.amount;
        } else if (amount < stack.amount) {
            stack.amount -= amount;
            uiItemSlot.UpdateSlot();
            return amount;
        }
        EmptySlot();
        return amount;
    }

    public ItemStackModel TakeAll()
    {
        ItemStackModel handOver = new ItemStackModel(stack.blockType, stack.amount);
        EmptySlot();
        return handOver;
    }

    public void InsertStack(ItemStackModel stack)
    {
        this.stack = stack;
        uiItemSlot.UpdateSlot();
    }

    public bool HasItem
    {
        get {
            return stack != null;
        }
    }

}
