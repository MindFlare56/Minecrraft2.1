using Assets.Scripts.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour 
{
   
    private ChunksController chunksController;
    private PlayerModel player1;

    private void Start() 
    {
        int seed = 0;        
        Random.InitState(seed);             
        InitialiseChunks();
        chunksController.GenerateWorld();               
        player1 = GameObject.Find("Player").GetComponent<PlayerModel>();        
    }

    private void InitialiseChunks()
    {
        var chunksObject = GameObject.Find("Chunks");
        chunksObject.transform.SetParent(gameObject.transform);
        var material = GetComponent<MeshRenderer>().material;
        chunksController = chunksObject.GetComponent<ChunksController>();        
        chunksController.Build(material);        
    }    
}
