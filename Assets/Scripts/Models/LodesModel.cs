using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assets.Scripts.Controllers.Factories.BlockTypesFactory;

namespace Assets.Scripts.Models
{
    class LodesModel
    {

        private string name;
        private BlockTypeKey blockTypeKey;
        private int minHeight;
        private int maxHeight;
        private float scale;
        private float threshold;
        private float noiseOffset;

        public string Name
        {
            get => name;
            set => name = value;
        }
        public BlockTypeKey BlockTypeKey
        {
            get => blockTypeKey;
            set => blockTypeKey = value;
        }
        public int MinHeight
        {
            get => minHeight;
            set => minHeight = value;
        }
        public int MaxHeight
        {
            get => maxHeight;
            set => maxHeight = value;
        }
        public float Scale
        {
            get => scale;
            set => scale = value;
        }
        public float Threshold
        {
            get => threshold;
            set => threshold = value;
        }
        public float NoiseOffset
        {
            get => noiseOffset;
            set => noiseOffset = value;
        }
    }
}
