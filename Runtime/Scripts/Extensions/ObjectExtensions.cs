using UnityEngine;

namespace Knifest.UniTools.Extensions
{
    public static class ObjectExtensions
    {
        public static void SaveAsset(this Object obj)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(obj);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    }
}