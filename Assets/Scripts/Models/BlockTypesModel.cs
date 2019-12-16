using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class BlockTypesModel
    {

        private string blockName = "Air";
        private bool isSolid = false;
        private int backFaceTexture = 0;
        private int frontFaceTexture = 0;
        private int topFaceTexture = 0;
        private int bottomFaceTexture = 0;
        private int leftFaceTexture = 0;
        private int rightFaceTexture = 0;
        private int textures = 0;
        [SerializeField] private Sprite icon;

        // Back, Front, Top, Bottom, Left, Right
        public int GetTextureID(int faceIndex)
        {

            switch (faceIndex) {
                case 0:
                    return BackFaceTexture;
                case 1:
                    return FrontFaceTexture;
                case 2:
                    return TopFaceTexture;
                case 3:
                    return BottomFaceTexture;
                case 4:
                    return LeftFaceTexture;
                case 5:
                    return RightFaceTexture;
                default:
                    Debug.Log("Error in GetTextureID; invalid face index");
                    return 0;
            }
        }

        internal int Textures
        {
            get => textures;
            set {
                textures = value;
                backFaceTexture = value;
                frontFaceTexture = value;
                topFaceTexture = value;
                bottomFaceTexture = value;
                leftFaceTexture = value;
                rightFaceTexture = value;
            }
        }

        public string BlockName
        {
            get => blockName;
            set => blockName = value;
        }
        public bool IsSolid
        {
            get => isSolid;
            set => isSolid = value;
        }
        public int BackFaceTexture
        {
            get => backFaceTexture;
            set => backFaceTexture = value;
        }
        public int FrontFaceTexture
        {
            get => frontFaceTexture;
            set => frontFaceTexture = value;
        }
        public int TopFaceTexture
        {
            get => topFaceTexture;
            set => topFaceTexture = value;
        }
        public int BottomFaceTexture
        {
            get => bottomFaceTexture;
            set => bottomFaceTexture = value;
        }
        public int LeftFaceTexture
        {
            get => leftFaceTexture;
            set => leftFaceTexture = value;
        }
        public int RightFaceTexture
        {
            get => rightFaceTexture;
            set => rightFaceTexture = value;
        }
        public Sprite Icon
        {
            get => icon;
            set => icon = value;
        }
    }
}
