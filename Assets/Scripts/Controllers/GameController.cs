using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    
    private void Start()
    {
        GameObject world;
        var savePrefab = Resources.Load("Prefab/SavedWorld(Clone)");
        if (savePrefab != null) {            
            Instantiate(savePrefab, gameObject.transform.position, gameObject.transform.rotation);
            world = GameObject.Find("SavedWorld(Clone)");
        } else {
            var prefab = Resources.Load("Prefab/World");
            Instantiate(prefab, gameObject.transform.position, gameObject.transform.rotation);
            world = GameObject.Find("World(Clone)");            
        }
        world.name = "World";
        world.gameObject.transform.SetParent(gameObject.transform);
    }
   
}
