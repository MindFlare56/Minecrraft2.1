using Assets.Scripts.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldModel : MonoBehaviour 
{

    private int seed;
    private ChunksController chunksController;

    private void Start() 
    {
        Random.InitState(seed);
        InitialiseChunks();
        chunksController.GenerateWorld();
    }

    private void InitialiseChunks()
    {
        var chunksObject = new GameObject {
            name = "Chunks",
        };
        chunksObject.transform.SetParent(gameObject.transform);
        var material = GetComponent<MeshRenderer>().material;
        chunksController = chunksObject.AddComponent<ChunksController>().Build(material);
    }    
}
