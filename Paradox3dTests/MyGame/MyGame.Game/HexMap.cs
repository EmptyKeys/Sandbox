using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Core.Mathematics;

namespace MyGame
{   
    public class HexMap
    {        
        public static readonly float HexInnerSize = 16f;
        public static readonly float Sqrt3 = (float)Math.Sqrt(3);
        public static readonly float HexInnerSizeSqrt = HexMap.HexInnerSize * HexMap.Sqrt3;

        public static int Distance(int q1, int q2, int r1, int r2)
        {
            return (Math.Abs(q1 - q2) + Math.Abs(r1 - r2) + Math.Abs(q1 + r1 - q2 - r2)) / 2;
        }

        public static Vector2 RoundHex(float q, float r)
        {
            float x = q;
            float y = -q - r;
            float z = r;
            float rx = (float)Math.Round(x);
            float ry = (float)Math.Round(y);
            float rz = (float)Math.Round(z);

            float diffX = Math.Abs(rx - x);
            float diffY = Math.Abs(ry - y);
            float diffZ = Math.Abs(rz - z);

            if (diffX > diffY && diffX > diffZ)
            {
                rx = -ry - rz;
            }
            else if (diffY > diffZ)
            {
                ry = -rx - rz;
            }
            else
            {
                rz = -rx - ry;
            }

            return new Vector2(rx, rz);
        }        
    }
}
