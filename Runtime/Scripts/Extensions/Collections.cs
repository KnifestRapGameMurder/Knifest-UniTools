using System.Collections.Generic;
using UnityEngine;

namespace Knifest.UniTools.Extensions
{
    public static class Collections
    {
        private static T GetPseudoRandomElement<T>(this IReadOnlyList<T> list, float input)
        {
            var rand = input.ToPseudoRandom() % 1; // % 1 to make sure it's from 0 to 1, but 1 excluding
            int count = list.Count;
            int i = Mathf.FloorToInt(count * rand);
            return list[i];
        }
    }
}