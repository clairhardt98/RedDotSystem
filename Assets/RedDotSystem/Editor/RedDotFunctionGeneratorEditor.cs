using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class RedDotFunctionGeneratorEditor : EditorWindow
{
    private string RedDotFunctionFilePath = "";
    private GameObject redDotTagContainerPrefab;
    private RedDotTagContainer redDotTagContainerInstance;

    private string prefabPath = "";

    [MenuItem("RedDot/Generate RedDot Validate Functions")]
    public static void ShowWindow()
    {
        GetWindow<RedDotFunctionGeneratorEditor>("RedDot Function Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select RedDotTagContainer Prefab", EditorStyles.boldLabel);

        if(GUILayout.Button("Select Prefab"))
        {
            SelectPrefab();
        }

        GUILayout.Label($"Selected Prefab Path: {prefabPath}");

        GUILayout.Space(20);


        GUILayout.Label("Select Function Script File", EditorStyles.boldLabel);


        if (GUILayout.Button("Select Function Script"))
        {
            SelectScript();
        }

        GUILayout.Label($"Function Script Path: {RedDotFunctionFilePath}");



        if (redDotTagContainerInstance != null && !string.IsNullOrEmpty(RedDotFunctionFilePath))
        {
            GUILayout.Space(20);
            if(GUILayout.Button("Generate Functions"))
            {
                GenerateRedDotFunctions();
            }
        }
    }

    private void SelectPrefab()
    {
        string path = EditorUtility.OpenFilePanel("Select RedDotTagContainer Prefab", "Assets", "prefab");

        if(!string.IsNullOrEmpty(path))
        {
            path = path.Replace(Application.dataPath, "Assets");

            redDotTagContainerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if(redDotTagContainerPrefab != null)
            {
                prefabPath = path;

                redDotTagContainerInstance = redDotTagContainerPrefab.GetComponent<RedDotTagContainer>();

                if(redDotTagContainerInstance == null)
                {
                    Debug.LogError("RedDotTagContainer Instance is null");
                }
            }
            else
            {
                Debug.LogError("Unable to load Prefab");
            }
        }
    }

    private void SelectScript()
    {
        RedDotFunctionFilePath = EditorUtility.OpenFilePanel("Select RedDotFunction Script", "Assets", "cs");
    }
    private void GenerateRedDotFunctions()
    {
        if (redDotTagContainerInstance == null) return;
        if (!File.Exists(RedDotFunctionFilePath))
        {
            File.WriteAllText(RedDotFunctionFilePath, "using System;\n\npublic static class RedDotFunction\n{\n}");
        }

        string existingCode = File.ReadAllText(RedDotFunctionFilePath);

        List<RedDotTag> redDotTags = redDotTagContainerInstance.redDotTags;

        foreach (var tag in redDotTags)
        {
            if (!existingCode.Contains($"public static bool {tag.ValidateFuncName}()"))
            {
                string newFunction = $"\n    public static bool {tag.ValidateFuncName}()\n    {{\n        // {tag.DevComment}\n        throw new System.NotImplementedException();\n    }}\n";

                int insertIndex = existingCode.LastIndexOf('}');
                existingCode = existingCode.Insert(insertIndex, newFunction);
            }
        }

        File.WriteAllText(RedDotFunctionFilePath, existingCode);

        EditorUtility.DisplayDialog("Success", "Implement your validation code in function script", "ok");
        AssetDatabase.Refresh();
        Debug.Log("RedDotFunction.cs 파일에 함수가 생성되었습니다.");
    }
}
