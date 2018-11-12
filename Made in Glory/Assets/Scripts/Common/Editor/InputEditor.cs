using MIG.Scripts.Common;
using MIG.Scripts.Common.Input;
using MIG.Scripts.Common.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MIG.Scripts.Common.Input
{
    public class InputEditor : EditorWindow
    {
        public ControlsMapping[] Keys;

        private string _inputFilePath = "/StreamingAssets/Configuration/Input.json";
        private Vector2 _scrollPosition = Vector2.zero;

        [MenuItem("Window/Input Editor")]
        static void Init()
        {
            GetWindow(typeof(InputEditor)).Show();
        }

        void OnGUI()
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, true, true, GUILayout.Width(position.width), GUILayout.Height(position.height));
            if (Keys != null)
            {
                SerializedObject serializedObject = new SerializedObject(this);
                SerializedProperty serializedProperty = serializedObject.FindProperty("Keys");
                EditorGUILayout.PropertyField(serializedProperty, true);

                serializedObject.ApplyModifiedProperties();

                if (GUILayout.Button("Save data"))
                {
                    SaveGameData();
                }
            }

            if (GUILayout.Button("Load data"))
            {
                LoadGameData();
            }
            GUILayout.EndScrollView();
        }

        private void LoadGameData()
        {
            string filePath = Application.dataPath + _inputFilePath;

            if (File.Exists(filePath))
            {
                string dataAsJson = File.ReadAllText(filePath);

                if (string.IsNullOrEmpty(dataAsJson))
                    Keys = new ControlsMapping[0];
                else
                    Keys = JsonHelper.GetJsonArray<ControlsMapping>(dataAsJson);
            }
            else
            {
                Keys = new ControlsMapping[0];
            }
        }

        private void SaveGameData()
        {
            string dataAsJson = JsonHelper.SetJsonArray(Keys);

            string filePath = Application.dataPath + _inputFilePath;
            File.WriteAllText(filePath, dataAsJson);
        }
    }

    
}
