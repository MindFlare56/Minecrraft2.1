using Assets.Scripts.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugScreen: MonoBehaviour
{

    private ChunksController chunksController;
    private Text text;
    private float frameRate;
    private float timer;
    private int halfWorldSizeInVoxels;
    private int halfWorldSizeInChunks;

    private void Start()
    {
        chunksController = GameObject.Find("Chunks").GetComponent<ChunksController>();
        text = GetComponent<Text>();
        halfWorldSizeInVoxels = VoxelModel.WorldSizeInVoxels / 2;
        halfWorldSizeInChunks = VoxelModel.WorldSizeInChunks / 2;
    }

    void Update()
    {
        string debugText = "Press F3 to close debug.";
        debugText += "\n";
        debugText += frameRate + " fps";
        debugText += "\n\n";
        //debugText += "XYZ: " + (Mathf.FloorToInt(chunksController.Player1.Position.x) - halfWorldSizeInVoxels) + " / " + Mathf.FloorToInt(world.Player1.transform.position.y) + " / " + (Mathf.FloorToInt(world.Player1.Position.z) - halfWorldSizeInVoxels);
        debugText += "\n";
        //debugText += "Chunk: " + (ChunksController.ChunkCoord.x - halfWorldSizeInChunks) + " / " + (chunksController.chunkCoord.z - halfWorldSizeInChunks);
        text.text = debugText;
        if (timer > 1f) {
            frameRate = (int) (1f / Time.unscaledDeltaTime);
            timer = 0;
        } else {
            timer += Time.deltaTime;
        }
    }
}
