using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assets.Scripts.Controllers.Factories.BlockTypesFactory;

namespace Assets.Scripts.Controllers.Factories
{
    static class LodesFactory
    {

        internal static List<LodesModel> MakeAll()
        {
            List<LodesModel> lodes = new List<LodesModel> {
                MakeDirt(),
                MakeSand()
            };
            return lodes;
        }

        private static LodesModel MakeDirt()
        {
            LodesModel dirt = new LodesModel {
                Name = "Dirt",
                BlockTypeKey = BlockTypeKey.Dirt,
                MinHeight = 1,
                MaxHeight = 255,
                Scale = 1f,
                Threshold = 0.5f,
                NoiseOffset = 0
            };
            return dirt;
        }

        private static LodesModel MakeSand()
        {
            LodesModel sand = new LodesModel {
                Name = "Sand",
                BlockTypeKey = BlockTypeKey.Sand,
                MinHeight = 30,
                MaxHeight = 60,
                Scale = 0.2f,
                Threshold = 0.6f,
                NoiseOffset = 500
            };
            return sand;
        }
    }
}
