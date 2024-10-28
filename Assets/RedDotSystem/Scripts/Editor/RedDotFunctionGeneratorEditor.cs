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
        // ERedDot ������ �� ��������
        var enumValues = Enum.GetValues(typeof(ERedDot)).Cast<ERedDot>().ToArray();

        // ���� ��� Ȯ��
        if (!File.Exists(RedDotFunctionFilePath))
        {
            File.WriteAllText(RedDotFunctionFilePath, "using System;\n\npublic static class RedDotFunction\n{\n}");

        }

        // RedDotFunction.cs ���� ���� �б�
        string existingCode = File.ReadAllText(RedDotFunctionFilePath);

        // ���� ��� ã�� �� �Լ� ����
        foreach (var enumValue in enumValues)
        {
            if(enumValue == ERedDot.None || enumValue == ERedDot.Root || enumValue == ERedDot.End) continue; // None, Root, End�� �Լ� �������� ����

            if (IsLeafNode(enumValue, enumValues))
            {
                string functionName = GetLeafFunctionName(enumValue);

                // �Լ��� �̹� �ִ��� Ȯ��
                if (!FunctionExists(existingCode, functionName))
                {
                    // �Լ� ����
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

        // ���� ������Ʈ
        File.WriteAllText(RedDotFunctionFilePath, existingCode);
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success", "Implement your validation code in function script", "ok");

    }

    private static bool IsLeafNode(ERedDot enumValue, ERedDot[] allEnumValues)
    {
        string currentName = enumValue.ToString();

        // �ٸ� ������ �� �߿��� �� ���� �θ�� ���� ���� �ִ��� Ȯ��
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