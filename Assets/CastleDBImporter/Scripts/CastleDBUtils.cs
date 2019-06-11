using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace CastleDBImporter
{
    class CastleDBUtils
    {
        // Convert CastleDB color string to Unity Color type.
        public static Color GetColorFromString(string color)
        {
            int.TryParse(color, out int icolor);
            float blue = ((icolor >> 0) & 255) / 255.0f;
            float green = ((icolor >> 8) & 255) / 255.0f;
            float red = ((icolor >> 16) & 255) / 255.0f;
            return new Color(red, green, blue);
        }
    }
}

