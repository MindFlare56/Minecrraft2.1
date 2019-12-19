using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Models
{

    public class GameSettingModel
    {

        private string version = "0.0.01";
        private int viewDistance = 8;
        [Range(0.1f, 10f)] private float mouseSensitivity = 2.0f;
        private int seed = 89745;

        public string Version
        {
            get => version; set => version = value;
        }
        public int ViewDistance
        {
            get => viewDistance; set => viewDistance = value;
        }
        public float MouseSensitivity
        {
            get => mouseSensitivity; set => mouseSensitivity = value;
        }
        public int Seed
        {
            get => seed; set => seed = value;
        }

    }
}
