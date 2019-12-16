using Assets.Scripts.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Controllers.Factories.BlockTypesFactory;

public class ChunkModel
{

    private ChunkCoord coord;
    private GameObject chunkObject;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private int vertexIndex = 0;
	private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
	private BlockTypeKey[,,] voxelMap = new BlockTypeKey[VoxelModel.ChunkWidth, VoxelModel.ChunkHeight, VoxelModel.ChunkWidth];
    private ChunksController chunksController;

    public ChunkModel(ChunkCoord coord, ChunksController chunksController) 
    {
        this.coord = coord;
        ChunksController = chunksController;
        CreateChunkObject();
        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();
    }

    private void CreateChunkObject()
    {
        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshRenderer.material = chunksController.Material;
        chunkObject.transform.SetParent(chunksController.transform);
        chunkObject.transform.position = new Vector3(this.coord.x * VoxelModel.ChunkWidth, 0f, this.coord.z * VoxelModel.ChunkWidth);
        chunkObject.name = "Chunk " + this.coord.x + ", " + this.coord.z;        
    }

    private void LoopThroughChunks(Action<int, int, int> action)
    {
        for (int y = 0; y < VoxelModel.ChunkHeight; y++) {
            for (int x = 0; x < VoxelModel.ChunkWidth; x++) {
                for (int z = 0; z < VoxelModel.ChunkWidth; z++) {
                    action(x, y, z);
                }
            }
        }
    }

    private void PopulateVoxelMap() 
    {
        LoopThroughChunks((x, y, z) => {
            VoxelMap[x, y, z] = chunksController.GetVoxel(new Vector3(x, y, z) + Position);
        });	
	}

	private void CreateMeshData() 
    {
        LoopThroughChunks((x, y, z) => {
            if (chunksController.Blocktypes[VoxelMap[x, y, z]].IsSolid) {
                AddVoxelDataToChunk(new Vector3(x, y, z));
            }                
        });      
	}

    private bool IsVoxelInChunk(int x, int y, int z)
    {
        return !(x < 0 || x > VoxelModel.ChunkWidth - 1 || y < 0 || y > VoxelModel.ChunkHeight - 1 || z < 0 || z > VoxelModel.ChunkWidth - 1);
    }

    private bool CheckVoxel(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x);
        int y = Mathf.FloorToInt(position.y);
        int z = Mathf.FloorToInt(position.z);
        if (!IsVoxelInChunk(x, y, z)) {
            return chunksController.Blocktypes[chunksController.GetVoxel(position + Position)].IsSolid;
        }
        return chunksController.Blocktypes[VoxelMap[x, y, z]].IsSolid;
    }

    private void AddVoxelDataToChunk(Vector3 position)
    {
        for (int faceIndex = 0; faceIndex < 6; ++faceIndex) {
            if (!CheckVoxel(position + VoxelModel.faceChecks[faceIndex])) {
                CreateVoxelFromChunkPosition(position, faceIndex);                
            }
        }
    }

    private void CreateVoxelFromChunkPosition(Vector3 position, int faceIndex)
    {
        BlockTypeKey blockType = VoxelMap[(int) position.x, (int) position.y, (int) position.z];
        AddVertices(position, faceIndex);
        AddTexture(chunksController.Blocktypes[blockType].GetTextureID(faceIndex));
        AddTriangles();
        vertexIndex += 4;
    }

    private void AddVertices(Vector3 position, int faceIndex)
    {        
        Vertices.Add(position + VoxelModel.voxelVerts[VoxelModel.voxelTris[faceIndex, 0]]);
        Vertices.Add(position + VoxelModel.voxelVerts[VoxelModel.voxelTris[faceIndex, 1]]);
        Vertices.Add(position + VoxelModel.voxelVerts[VoxelModel.voxelTris[faceIndex, 2]]);
        Vertices.Add(position + VoxelModel.voxelVerts[VoxelModel.voxelTris[faceIndex, 3]]);
    }

    private void AddTriangles()
    {
        Triangles.Add(vertexIndex);
        Triangles.Add(vertexIndex + 1);
        Triangles.Add(vertexIndex + 2);
        Triangles.Add(vertexIndex + 2);
        Triangles.Add(vertexIndex + 1);
        Triangles.Add(vertexIndex + 3);
    }

    private void CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = Vertices.ToArray();
        mesh.triangles = Triangles.ToArray();
        mesh.uv = Uvs.ToArray();
        mesh.RecalculateNormals();
        MeshFilter.mesh = mesh;
    }

    private void AddTexture(int textureID)
    {
        float y = textureID / VoxelModel.TextureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelModel.TextureAtlasSizeInBlocks);
        x *= VoxelModel.NormalizedBlockTextureSize;
        y *= VoxelModel.NormalizedBlockTextureSize;
        y = 1f - y - VoxelModel.NormalizedBlockTextureSize;
        Uvs.Add(new Vector2(x, y));
        Uvs.Add(new Vector2(x, y + VoxelModel.NormalizedBlockTextureSize));
        Uvs.Add(new Vector2(x + VoxelModel.NormalizedBlockTextureSize, y));
        Uvs.Add(new Vector2(x + VoxelModel.NormalizedBlockTextureSize, y + VoxelModel.NormalizedBlockTextureSize));
    }

    public bool IsActive 
    {
        get { return chunkObject.activeSelf; }
        set { chunkObject.SetActive(value); }
    }
    public Vector3 Position 
    {
        get { return chunkObject.transform.position; }
    }
    internal ChunksController ChunksController
    {
        get => chunksController;
        set => chunksController = value;
    }
    public BlockTypeKey[,,] VoxelMap
    {
        get => voxelMap;
        set => voxelMap = value;
    }
    public List<Vector3> Vertices
    {
        get => vertices;
        set => vertices = value;
    }
    public List<int> Triangles
    {
        get => triangles;
        set => triangles = value;
    }
    public List<Vector2> Uvs
    {
        get => uvs;
        set => uvs = value;
    }
    public MeshFilter MeshFilter
    {
        get => meshFilter;
        set => meshFilter = value;
    }
}

public class ChunkCoord
{

    public int x;
    public int z;

    public ChunkCoord(int x = 0, int z = 0)
    {
        this.x = x;
        this.z = z;
    }

    public bool Equals(ChunkCoord other) 
    {
        if (other == null) {
            return false;
        }
        return (other.x == x && other.z == z);        
    }

}
