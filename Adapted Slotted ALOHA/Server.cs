﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adapted_Slotted_ALOHA.Properties;

namespace Adapted_Slotted_ALOHA
{
    class Server
    {
        public int[,] Frames { get; set; } = new int[Settings.Default.NumberOfStations, 1000];

        public bool CheckCollision(int frame)
        {
            var collision = 0;
            for (var i = 0; i < Settings.Default.NumberOfStations; i++)
            {
                if (Frames[i, frame] == 1)
                {
                    collision++;
                }
            }
            switch (collision)
            {
                case 0:
                    return false;
                case 1:
                    return false;
                default:
                    return true;
            }
        }
    }
}
