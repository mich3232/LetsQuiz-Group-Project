using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GameDataEditor : EditorWindow
{
    [Header("Component")]
    public GameData gameData;

    private string _gameDataFilePath = "/StreamingAssets/data.json";

    [MenuItem("Window/ Game Data Editor")]
    private static void Init()
    {
        EditorWindow.GetWindow(typeof(GameDataEditor)).Show();
    }

    private void OnGUI()
    {
        if (gameData != null)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("gameData");
            EditorGUILayout.PropertyField(serializedProperty, true);

            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Save Game Data"))
                SaveGameData();
        }

        if (GUILayout.Button("Load Game Data"))
            LoadGameData();
    }

    private void LoadGameData()
    {
        string filePath = Application.dataPath + _gameDataFilePath;

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            GameData loadedData = JsonUtility.FromJson<GameData>(jsonData);

            if (loadedData != null)
                gameData = loadedData;
            else
                Debug.LogError("GameDataEditor : LoadGameData() - Could not find data to load.");
        }
        else
            gameData = new GameData();
    }

    private void SaveGameData()
    {
        string stringData = JsonUtility.ToJson(gameData);

        string filePath = Application.dataPath + _gameDataFilePath;

        if (File.Exists(filePath))
        {
            if (!String.IsNullOrEmpty(stringData) || stringData != null)
                File.WriteAllText(filePath, stringData);
        }
    }
}