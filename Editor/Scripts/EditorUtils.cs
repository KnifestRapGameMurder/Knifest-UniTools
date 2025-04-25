using System.IO;
using UnityEditor;
using UnityEngine;

namespace Knifest.UniTools.Editor
{
    public static class Utils
    {
        public static void SaveAsset(string path, string name, Object asset)
        {
            // Build the complete path to the file
            string fullPath = $"Assets/{path}";

            // Check if the directory exists, and if not, create it
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            // Build the full path to the asset file
            string assetPath = $"{fullPath}/{name}.asset";

            // Save the asset at the given path
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Focus on the newly created asset in the project window
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        public static void OpenScriptableObjectInInspector(ScriptableObject scriptableObject)
        {
            if (scriptableObject != null)
            {
                // Focus on the Project window to ensure the Inspector updates
                EditorUtility.FocusProjectWindow();

                // Select the scriptable object in the editor
                Selection.activeObject = scriptableObject;

                // Optionally ping the object to highlight it in the project hierarchy
                EditorGUIUtility.PingObject(scriptableObject);
            }
            else
            {
                Debug.LogError("ScriptableObject is null and cannot be opened in the inspector.");
            }
        }
    }
}