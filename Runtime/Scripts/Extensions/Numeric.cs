using UnityEngine;

namespace Knifest.UniTools.Extensions
{
    public static class Numeric
    {
        public static Vector3 ToVector3XZ(this Vector2 vector2) => new Vector3(vector2.x, 0, vector2.y);

        /// The Result is in range [0, 1]
        public static float ToPseudoRandom(this float input)
        {
            float randomValue = Mathf.Sin(input * 12.9898f + 78.233f) * 43758.5453f;
            return randomValue - Mathf.Floor(randomValue);
        }

        /// The Result is in range [0, 1]
        public static float ToPseudoRandom(this Vector3 input)
        {
            // Generate a pseudo-random value based on the vector components
            float randomValue = Mathf.Sin(Vector3.Dot(input, new Vector3(12.9898f, 78.233f, 45.654f))) * 43758.5453f;

            // Normalize the value to the range [0, 1]
            return randomValue - Mathf.Floor(randomValue);
        }

        public static Vector2 ToPseudoRandomInsideUnitCircle(this float input)
        {
            // Generate two pseudo-random values using sine
            float randomX = Mathf.Sin(input * 12.9898f + 78.233f) * 43758.5453f;
            float randomY = Mathf.Sin(input * 43.2321f + 19.133f) * 37234.873f;

            // Normalize the values to the range [0, 1]
            randomX -= Mathf.Floor(randomX);
            randomY -= Mathf.Floor(randomY);

            // Map the values to the range [-1, 1]
            randomX = randomX * 2f - 1f;
            randomY = randomY * 2f - 1f;

            // If the point is outside the unit circle, normalize it
            Vector2 point = new Vector2(randomX, randomY);
            if (point.sqrMagnitude > 1f)
            {
                point.Normalize();
            }

            return point;
        }

        public static Vector2 ToPseudoRandomInsideUnitCircle(this Vector3 input)
        {
            // Generate two deterministic random values from the input vector
            float randomX = new Vector3(input.x + 1, input.y, input.z).ToPseudoRandom() * 2f - 1f;
            float randomY = new Vector3(input.x, input.y + 1, input.z).ToPseudoRandom() * 2f - 1f;

            // If the point is outside the unit circle, normalize it
            var point = new Vector2(randomX, randomY);
            if (point.sqrMagnitude > 1f)
            {
                point.Normalize();
            }

            return point;
        }

        public static bool IsInRange(this float value, float min, float max) => value >= min && value <= max;

        public static bool IsInRange(this int value, int min, int max) => value >= min && value <= max;

        public static Vector2 Remap(this Vector2 value, Vector2 fromMin, Vector2 fromMax, Vector2 toMin, Vector2 toMax)
        {
            float remappedX = Remap(value.x, fromMin.x, fromMax.x, toMin.x, toMax.x);
            float remappedY = Remap(value.y, fromMin.y, fromMax.y, toMin.y, toMax.y);

            return new Vector2(remappedX, remappedY);
        }

        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public static int Remap(this int value, int from1, int to1, int from2, int to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
        
        public static string FormatNumber(this int number)
        {
            if (number >= 1000000)
            {
                return (number / 1000000.0).ToString("0.0m");
            }
            if (number >= 1000)
            {
                return (number / 1000.0).ToString("0.0k");
            }

            return number.ToString();
        }
        
        
        public static Vector2Int Abs(this Vector2Int value)
        {
            value.x = Mathf.Abs(value.x);
            value.y = Mathf.Abs(value.y);
            return value;
        }

        public static Vector2Int AtLeast(this Vector2Int value, Vector2Int minValue)
        {
            value.x = Mathf.Max(value.x, minValue.x);
            value.y = Mathf.Max(value.y, minValue.y);
            return value;
        }

        public static Vector2Int AtMost(this Vector2Int value, Vector2Int maxValue)
        {
            value.x = Mathf.Min(value.x, maxValue.x);
            value.y = Mathf.Min(value.y, maxValue.y);
            return value;
        }

        public static int MinAxis(this Vector2Int value)
        {
            return Mathf.Min(value.x, value.y);
        }

        public static int MaxAxis(this Vector2Int value)
        {
            return Mathf.Max(value.x, value.y);
        }
    }
}