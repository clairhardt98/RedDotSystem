using RedDotSystem;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class RedDotFunctionGeneratorEditor : EditorWindow
{
    private const string RedDotFunctionFilePath = "Assets/RedDotSystem/Scripts/RedDotFunction.cs";

    [MenuItem("RedDotSystem/Generate RedDotFunction")]
    public static void ShowWindow()
    {
        GetWindow<RedDotFunctionGeneratorEditor>("RedDot Function Generator");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Generate RedDot Functions"))
        {
            GenerateRedDotFunctions();
        }
    }

    private static void GenerateRedDotFunctions()
    {
        // ERedDot 열거형 값 가져오기
        var enumValues = Enum.GetValues(typeof(ERedDot)).Cast<ERedDot>().ToArray();

        // 파일 경로 확인
        if (!File.Exists(RedDotFunctionFilePath))
        {
            File.WriteAllText(RedDotFunctionFilePath, "using System;\n\npublic static class RedDotFunction\n{\n}");

        }

        // RedDotFunction.cs 파일 내용 읽기
        string existingCode = File.ReadAllText(RedDotFunctionFilePath);

        // 리프 노드 찾기 및 함수 생성
        foreach (var enumValue in enumValues)
        {
            if(enumValue == ERedDot.None || enumValue == ERedDot.Root || enumValue == ERedDot.End) continue; // None, Root, End는 함수 생성하지 않음

            if (IsLeafNode(enumValue, enumValues))
            {
                string functionName = GetLeafFunctionName(enumValue);

                // 함수가 이미 있는지 확인
                if (!FunctionExists(existingCode, functionName))
                {
                    // 함수 생성
                    string functionCode = GenerateFunctionCode(functionName);
                    int insertIndex = existingCode.LastIndexOf('}');
                    existingCode = existingCode.Insert(insertIndex, functionCode);
                    Debug.Log($"Generated function: {functionName}");
                }
                else
                {
                    Debug.Log($"Function {functionName} already exists.");
                }
            }
        }

        // 파일 업데이트
        File.WriteAllText(RedDotFunctionFilePath, existingCode);
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success", "Implement your validation code in function script", "ok");

    }

    private static bool IsLeafNode(ERedDot enumValue, ERedDot[] allEnumValues)
    {
        string currentName = enumValue.ToString();

        // 다른 열거형 값 중에서 이 값을 부모로 가진 값이 있는지 확인
        return !allEnumValues.Any(otherEnum => otherEnum != enumValue && otherEnum.ToString().StartsWith(currentName + "_"));
    }

    private static string GetLeafFunctionName(ERedDot enumValue)
    {
        string enumName = enumValue.ToString();
        int lastUnderscoreIndex = enumName.LastIndexOf('_');

        return lastUnderscoreIndex >= 0
            ? enumName.Substring(lastUnderscoreIndex + 1)
            : enumName;
    }

    private static bool FunctionExists(string existingCode, string functionName)
    {
        return existingCode.Contains($"public static bool {functionName}()");
    }

    private static string GenerateFunctionCode(string functionName)
    {
        return $"\n    public static bool {functionName}()\n    {{\n        throw new System.NotImplementedException();\n    }}\n";
    }
}