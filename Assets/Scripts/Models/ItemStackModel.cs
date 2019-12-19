using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Controllers.Factories.BlockTypesFactory;


public class ItemStackModel
{

    public BlockTypeKey blockType;
    public int amount;

    public ItemStackModel(BlockTypeKey blockType, int amount)
    {
        this.blockType = blockType;
        this.amount = amount;
    }

}