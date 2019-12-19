using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controllers.Factories
{
    [Serializable]
    public class BlockTypesFactory
    {

        public enum BlockTypeKey: byte
        {
            Air = 0,
            Bedrock = 1,
            Stone = 2,
            Sand = 3,
            Grass = 4,
            Dirt = 5
        }

        public static Dictionary<BlockTypeKey, BlockTypesModel> MakeAll()
        {                        
            return new Dictionary<BlockTypeKey, BlockTypesModel> {
                {BlockTypeKey.Air, MakeAir()},
                {BlockTypeKey.Bedrock, MakeBedrock()},
                {BlockTypeKey.Stone, MakeStone()},
                {BlockTypeKey.Sand, MakeSand()},
                {BlockTypeKey.Grass, MakeGrass()},
                {BlockTypeKey.Dirt, MakeDirt()}
            };
        }

        private static Sprite FindSprite(string name)
        {
            return GameObject.Find(name).GetComponent<SpriteRenderer>().sprite;
        }

        private static BlockTypesModel MakeBedrock()
        {
            return new BlockTypesModel {
                BlockName = "Bedrock",
                IsSolid = true,
                Textures = 9,
                Icon = FindSprite("bedrock_icon")
            };
        }        

        private static BlockTypesModel MakeStone()
        {
            return new BlockTypesModel {
                BlockName = "Stone",
                IsSolid = true,
                Textures = 0,
                Icon = FindSprite("stone_icon")
            };
        }

        private static BlockTypesModel MakeGrass()
        {
            return new BlockTypesModel {
                BlockName = "Grass",
                IsSolid = true,
                FrontFaceTexture = 2,
                BackFaceTexture = 2,
                LeftFaceTexture = 2,
                RightFaceTexture = 2,
                TopFaceTexture = 7,
                BottomFaceTexture = 1,
                Icon = FindSprite("grass_icon")
            };
        }

        private static BlockTypesModel MakeSand()
        {
            return new BlockTypesModel {
                BlockName = "Sand",
                IsSolid = true,
                Textures = 10,
                Icon = FindSprite("sand_icon")
            };
        }

        private static BlockTypesModel MakeDirt()
        {
            return new BlockTypesModel {
                BlockName = "Dirt",
                IsSolid = true,
                Textures = 1,
                Icon = FindSprite("dirt_icon")
            };
        }

        private static BlockTypesModel MakeAir()
        {
            return new BlockTypesModel();
        }
    }
}
