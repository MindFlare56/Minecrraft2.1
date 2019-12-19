using Assets.Scripts.Controllers.Factories;
using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Controllers.Factories.BlockTypesFactory;

namespace Assets.Scripts.Controllers
{   
    public class ChunksController: MonoBehaviour
    {

        private Material material;
        private ChunkModel[,] chunks;
        private List<ChunkCoord> activeChunks = new List<ChunkCoord>();
        private BiomeModel biome;
        private Transform player;
        private Vector3 spawnPosition;
        private Dictionary<BlockTypeKey, BlockTypesModel> blocktypes;
        private ChunkCoord playerChunkCoord;
        private ChunkCoord playerLastChunkCoord;

        internal ChunksController Build(Material material)
        {
            chunks = new ChunkModel[VoxelModel.WorldSizeInChunks, VoxelModel.WorldSizeInChunks];
            blocktypes = MakeAll();
            biome = CreateBiome();            
            spawnPosition = new Vector3((VoxelModel.WorldSizeInChunks * VoxelModel.ChunkWidth) / 2f, VoxelModel.ChunkHeight - 50f, (VoxelModel.WorldSizeInChunks * VoxelModel.ChunkWidth) / 2f);
            Material = material;
            player = GameObject.Find("Player").GetComponent<PlayerModel>().transform;
            playerLastChunkCoord = GetChunkCoordFromVector3(player.position);
            return this;
        }



        private void Start()
        {   
            
        }

        private void Update()
        {
            if (Chunks != null) {
                playerChunkCoord = GetChunkCoordFromVector3(player.position);
            }                        
        }

        internal ChunkModel GetChunkFromVector3(Vector3 position)
        {
            int x = Mathf.FloorToInt(position.x / VoxelModel.ChunkWidth);
            int z = Mathf.FloorToInt(position.z / VoxelModel.ChunkWidth);
            return chunks[x, z];
        }

        internal ChunkModel GetChunkFromVector3(Vector3Int position)
        {
            int x = Mathf.FloorToInt(position.x / VoxelModel.ChunkWidth);
            int z = Mathf.FloorToInt(position.z / VoxelModel.ChunkWidth);
            return chunks[x, z];
        }

        public BlockTypeKey GetVoxel(Vector3 voxelPosition)
        {
            var blockTypeKey = BlockTypeKey.Air;
            blockTypeKey = ChangeBlockTypeByHeight(voxelPosition, blockTypeKey);
            blockTypeKey = RandomizeTextureHorizontaly(voxelPosition, blockTypeKey);
            return blockTypeKey;
        }

        public bool CheckForVoxel(float x, float y, float z)
        {
            int xCheck = Mathf.FloorToInt(x);
            int yCheck = Mathf.FloorToInt(y);
            int zCheck = Mathf.FloorToInt(z);
            int xChunk = xCheck / VoxelModel.ChunkWidth;
            int zChunk = zCheck / VoxelModel.ChunkWidth;
            xCheck -= (xChunk * VoxelModel.ChunkWidth);
            zCheck -= (zChunk * VoxelModel.ChunkWidth);
            return Blocktypes[chunks[xChunk, zChunk].VoxelMap[xCheck, yCheck, zCheck]].IsSolid;
        }

        public bool CheckForVoxel(Vector3 position)
        {
            return CheckForVoxel(position.x, position.y, position.z);
        }              

        internal void GenerateWorld()
        {
            for (int x = (VoxelModel.WorldSizeInChunks / 2) - VoxelModel.ViewDistanceInChunks; x < (VoxelModel.WorldSizeInChunks / 2) + VoxelModel.ViewDistanceInChunks; x++) {
                for (int z = (VoxelModel.WorldSizeInChunks / 2) - VoxelModel.ViewDistanceInChunks; z < (VoxelModel.WorldSizeInChunks / 2) + VoxelModel.ViewDistanceInChunks; z++) {
                    CreateNewChunk(x, z);
                }
            }
            player.position = spawnPosition;
            IsReady = true;
        }

        internal BlockTypeKey TryDestroyingBlock(Vector3Int voxelPosition)
        {
            var chunk = GetChunkFromVector3(voxelPosition);
            if (chunk != null) {
                return chunk.EditVoxelAtPosition(voxelPosition, BlockTypeKey.Air);
            }
            return BlockTypeKey.Air;
        }

        internal void TryAddingBlock(Vector3Int cursorPosition, BlockTypeKey blockType)
        {
            var chunk = GetChunkFromVector3(cursorPosition);
            if (chunk != null) {
                chunk.EditVoxelAtPosition(cursorPosition, blockType);
            }                
        }

        private void CreateNewChunk(int x, int z)
        {
            Chunks[x, z] = new ChunkModel(new ChunkCoord(x, z), this);
            ActiveChunks.Add(new ChunkCoord(x, z));
        }

        private BiomeModel CreateBiome()
        {
            return new BiomeModel();
        }

        private BlockTypeKey RandomizeTextureHorizontaly(Vector3 voxelPosition, BlockTypeKey blockTypeKey)
        {
            if (blockTypeKey == BlockTypeKey.Stone) {
                foreach (LodesModel lode in biome.Lodes) {
                    if (voxelPosition.y > lode.MinHeight && voxelPosition.y < lode.MaxHeight) {
                        if (NoiseModel.Get3DPerlin(voxelPosition, lode.NoiseOffset, lode.Scale, lode.Threshold)) {
                            return lode.BlockTypeKey;
                        }
                    }
                }
            }
            return blockTypeKey;
        }

        private BlockTypeKey ChangeBlockTypeByHeight(Vector3 voxelPosition, BlockTypeKey result)
        {
            int terrainHeight = GenerateRandomTerrainHeight(voxelPosition);
            int voxelYPosition = Mathf.FloorToInt(voxelPosition.y);
            if (voxelYPosition == 0) {
                return BlockTypeKey.Bedrock;
            } else if (voxelYPosition == terrainHeight) {
                result = BlockTypeKey.Grass;
            } else if (voxelYPosition < terrainHeight && voxelYPosition > terrainHeight - 4) {
                result = BlockTypeKey.Dirt;
            } else if (voxelYPosition > terrainHeight) {
                return BlockTypeKey.Air;
            } else {
                result = BlockTypeKey.Stone;
            }
            return result;
        }

        private int GenerateRandomTerrainHeight(Vector3 voxelPosition)
        {
            return Mathf.FloorToInt(biome.TerrainHeight * NoiseModel.Get2DPerlin(new Vector2(voxelPosition.x, voxelPosition.z), 0, biome.TerrainScale)) + biome.SolidGroundHeight;
        }        

        private ChunkCoord GetChunkCoordFromVector3(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x / VoxelModel.ChunkWidth);
            int z = Mathf.FloorToInt(pos.z / VoxelModel.ChunkWidth);
            return new ChunkCoord(x, z);
        }

        /*
        internal void CheckViewDistance(Vector3 position)
        {
            ChunkCoordsModel coord = GetChunkCoordFromVector3(position);
            List<ChunkCoordsModel> previouslyActiveChunks = new List<ChunkCoordsModel>(ActiveChunks);
            for (int x = coord.X - VoxelModel.ViewDistanceInChunks; x < coord.X + VoxelModel.ViewDistanceInChunks; x++) {
                for (int z = coord.Z - VoxelModel.ViewDistanceInChunks; z < coord.Z + VoxelModel.ViewDistanceInChunks; z++) {
                    if (IsChunkInWorld(new ChunkCoordsModel(x, z))) {
                        if (Chunks[x, z] == null) {
                            chunks[x, z] = new ChunksModel(new ChunkCoordsModel(x, z), this, false);
                            ChunksToCreate.Add(new ChunkCoordsModel(x, z));
                        } else if (!Chunks[x, z].IsActive) {
                            Chunks[x, z].IsActive = true;
                            ActiveChunks.Add(new ChunkCoordsModel(x, z));
                        }
                    }
                    for (int i = 0; i < previouslyActiveChunks.Count; i++) {
                        if (previouslyActiveChunks[i].Equals(new ChunkCoordsModel(x, z))) {
                            previouslyActiveChunks.RemoveAt(i);
                        }
                    }
                }
            }
            foreach (ChunkCoordsModel chunkCoord in previouslyActiveChunks) {
                Chunks[chunkCoord.X, chunkCoord.Z].IsActive = false;
            }
        }
        */        

        private bool IsChunkInWorld(ChunkCoord coord)
        {
            return (coord.x > 0 && coord.x < VoxelModel.WorldSizeInChunks - 1 && coord.z > 0 && coord.z < VoxelModel.WorldSizeInChunks - 1);            
        }

        private bool IsVoxelInWorld(Vector3 pos)
        {
            return (pos.x >= 0 && pos.x < VoxelModel.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelModel.ChunkHeight && pos.z >= 0 && pos.z < VoxelModel.WorldSizeInVoxels);
        }        

        public Material Material
        {
            get => material;
            set => material = value;
        }
        public ChunkModel[,] Chunks
        {
            get => chunks;
            set => chunks = value;
        }
        public List<ChunkCoord> ActiveChunks
        {
            get => activeChunks;
            set => activeChunks = value;
        }
        public Dictionary<BlockTypeKey, BlockTypesModel> Blocktypes
        {
            get => blocktypes;
            set => blocktypes = value;
        }
        public bool IsReady
        {
            get;
            internal set;
        } = false;
    }
}
