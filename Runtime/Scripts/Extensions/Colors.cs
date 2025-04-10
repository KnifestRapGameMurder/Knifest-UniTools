using UnityEngine;

namespace Knifest.UniTools.Extensions
{
    public static class Colors
    {
        // Compare two colors with tolerance
        public static bool CompareColorsWithTolerance(this Color color1, Color color2, float tolerance)
        {
            // Calculate the squared difference between the colors for performance
            float diffR = (color1.r - color2.r) * (color1.r - color2.r);
            float diffG = (color1.g - color2.g) * (color1.g - color2.g);
            float diffB = (color1.b - color2.b) * (color1.b - color2.b);
            //float diffA = (color1.a - color2.a) * (color1.a - color2.a);

            // Calculate the squared distance
            float distanceSquared = diffR + diffG + diffB /*+ diffA*/;

            // Return true if the squared distance is within the squared tolerance
            return distanceSquared <= tolerance * tolerance;
        }

        public static Vector3 ToVector3(this Color color)
        {
            return new Vector3(color.r, color.g, color.b);
        }
    }
}