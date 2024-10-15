using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RedDotTagContainer))]
public class RedDotTagEditor : Editor
{
    [SerializeField] RedDotTagContainer _redDotTagContainer;
    RedDotTagContainer container;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        string path = Application.dataPath + "/RedDotTagContainer.json";
        container = (RedDotTagContainer)target;

        if (GUILayout.Button("Save To File"))
        {
            SaveToFile(path);
        }

        if(GUILayout.Button("Load from File"))
        {
            LoadFromFile(path);
        }
    }

    public void SaveToFile(string filePath)
    {
        try
        {
            string jsonData = JsonUtility.ToJson(container);
            File.WriteAllText(filePath, jsonData);
            Debug.Log("RedDotTag Saved To " + filePath);
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to save data to {filePath}: {e.Message}");

        }
        catch (System.Exception e)
        {
            Debug.LogError($"An error occurred while saving data: {e.Message}");
        }
    }

    public void LoadFromFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);

                JsonUtility.FromJsonOverwrite(jsonData, container);
                Debug.Log("RedDotTag Loaded from " + filePath);
            }
            else
            {
                Debug.LogWarning($"File not found: {filePath}");
            }
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to load data from {filePath}: {e.Message}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"An error occurred while loading data: {e.Message}");
        }
    }
}
