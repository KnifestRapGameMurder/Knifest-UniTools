using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Knifest.UniTools.Editor
{
    [InitializeOnLoad] // This makes sure the static constructor runs when Unity starts
    public class PlayFromSceneWindow : EditorWindow
    {
        private string[] _scenePaths;
        private string[] _sceneNames;
        private int _selectedSceneIndex = 0;
        private string _activeSceneBeforePlay;
        private List<SceneArgs> _scenesBeforePlay;
        private bool _shouldLoadPreviousScene = false;
        private bool _playedFromThisWindow;

        [Serializable]
        private struct SceneArgs
        {
            public string sceneName;
            public bool isLoaded;
        }

        // // This static constructor will run when Unity is launched
        // static PlayFromSceneWindow()
        // {
        //     // Check if the window is already open, if not, show it
        //     EditorApplication.delayCall += () =>
        //     {
        //         // Check if there is already an instance of this window open
        //         var existingWindow = Resources.FindObjectsOfTypeAll<PlayFromSceneWindow>();
        //         if (existingWindow == null || existingWindow.Length == 0)
        //         {
        //             // This will ensure the window reopens after Unity restarts
        //             ShowWindow();
        //         }
        //     };
        // }

        [MenuItem("Tools/Play From Scene Window")]
        public static void ShowWindow()
        {
            // Open the window (and make it dockable)
            var window = GetWindow<PlayFromSceneWindow>("Play From Scene");
            window.Show();
        }

        private void OnEnable()
        {
            _scenePaths = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
            _sceneNames = new string[_scenePaths.Length];
            for (int i = 0; i < _scenePaths.Length; i++)
            {
                _sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(_scenePaths[i]);
            }

            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Select a scene to play:", EditorStyles.boldLabel);
            _selectedSceneIndex = EditorGUILayout.Popup(_selectedSceneIndex, _sceneNames);

            if (EditorApplication.isPlaying)
            {
                if (GUILayout.Button("Stop")) EditorApplication.isPlaying = false;
            }
            else
            {
                if (GUILayout.Button("Play")) PlaySelectedScene();
            }
        }

        private void PlaySelectedScene()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogError("Game is already playing");
            }
            else
            {
                _activeSceneBeforePlay = SceneManager.GetActiveScene().path;
                int sceneCount = SceneManager.sceneCount;

                _scenesBeforePlay = new();

                for (int i = 0; i < sceneCount; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);

                    if (scene.path == _activeSceneBeforePlay) continue;

                    _scenesBeforePlay.Add(new SceneArgs { sceneName = scene.path, isLoaded = scene.isLoaded });
                }

                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(_scenePaths[_selectedSceneIndex]);
                EditorApplication.isPlaying = true;
                _playedFromThisWindow = true;
            }
        }

        private void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode && _playedFromThisWindow)
            {
                _shouldLoadPreviousScene = true;
                _playedFromThisWindow = false;
            }
        }

        private void OnEditorUpdate()
        {
            if (!_shouldLoadPreviousScene || EditorApplication.isPlaying ||
                string.IsNullOrEmpty(_activeSceneBeforePlay))
                return;

            _shouldLoadPreviousScene = false;
            EditorSceneManager.OpenScene(_activeSceneBeforePlay);

            foreach (var scene in _scenesBeforePlay)
                EditorSceneManager.OpenScene(scene.sceneName,
                    scene.isLoaded ? OpenSceneMode.Additive : OpenSceneMode.AdditiveWithoutLoading);
        }
    }
}