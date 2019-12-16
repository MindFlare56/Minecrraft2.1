using Assets.Scripts.Controllers.Factories;
using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeModel
{

    private string name = "Default";
    private int solidGroundHeight = 42;
    private int terrainHeight = 42;
    private float terrainScale = 0.25f;
    private List<LodesModel> lodes = LodesFactory.MakeAll();

    public string Name
    {
        get => name;
        set => name = value;
    }
    public int SolidGroundHeight
    {
        get => solidGroundHeight;
        set => solidGroundHeight = value;
    }
    public int TerrainHeight
    {
        get => terrainHeight;
        set => terrainHeight = value;
    }
    public float TerrainScale
    {
        get => terrainScale;
        set => terrainScale = value;
    }
    internal List<LodesModel> Lodes
    {
        get => lodes;
        set => lodes = value;
    }
}
