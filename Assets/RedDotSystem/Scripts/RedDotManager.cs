using RedDotSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RedDotManager
{
    Dictionary<ERedDot, RedDotNode> nodeMap;
    RedDotNode root;
    #region Singleton
    private static RedDotManager _instance;
    public static RedDotManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RedDotManager();
            }
            return _instance;
        }
    }
    #endregion

    private RedDotManager()
    {
        root = BuildRedDotTree();
        InitTree();
    }

    public void Init()
    {
    }

    private RedDotNode BuildRedDotTree()
    {
        nodeMap = new();

        RedDotNode rootNode = new RedDotNode(ERedDot.Root);
        nodeMap[ERedDot.Root] = rootNode;

        foreach (ERedDot redDotType in Enum.GetValues(typeof(ERedDot)))
        {
            if (redDotType == ERedDot.None || redDotType == ERedDot.End || redDotType == ERedDot.Root)
                continue; // None, End, Root�� ����

            // ��� ����
            RedDotNode node = new RedDotNode(redDotType);
            nodeMap[redDotType] = node;

            // �θ� �ִ��� Ȯ���Ͽ� �θ� ��忡 �߰�
            ERedDot parentType = GetParentType(redDotType);
            if (parentType != ERedDot.None && nodeMap.ContainsKey(parentType))
            {
                nodeMap[parentType].AddChild(node);
            }
            else
            {
                // �θ� ������ Root�� �߰�
                rootNode.AddChild(node);
            }

            BindEvaluateFunc(node);
        }

        return rootNode;
    }
    private ERedDot GetParentType(ERedDot redDotType)
    {
        // Enum ������ �θ� ���踦 �߷��ϴ� ����
        string enumName = redDotType.ToString();
        int lastUnderscoreIndex = enumName.LastIndexOf('_');

        if (lastUnderscoreIndex > 0)
        {
            string parentName = enumName.Substring(0, lastUnderscoreIndex);
            if (Enum.TryParse(parentName, out ERedDot parentType))
            {
                return parentType;
            }
        }

        return ERedDot.None; // �θ� ���� ���
    }
    private void BindEvaluateFunc(RedDotNode node)
    {
        // RedDotFunction Ŭ�������� ����� Ÿ���� ������ �κп� �ش��ϴ� �޼��� �̸� ã��
        string enumName = node.Type.ToString();
        int lastUnderscoreIndex = enumName.LastIndexOf('_');

        // ������ھ� ������ �κи� ���� (��: A_B_C -> C)
        string methodName = (lastUnderscoreIndex >= 0)
            ? enumName.Substring(lastUnderscoreIndex + 1)
            : enumName;

        // �ش� �̸��� ����ƽ �޼��� ã��
        var method = typeof(RedDotFunction).GetMethod(methodName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

        if (method != null)
        {
            node.EvaluateFunc = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), method);
            Debug.LogWarning($"Method {methodName} Bound Success.");
        }
        
    }
    private void InitTree()
    {
        Assert.IsNotNull(root);
        root.Evaluate();
    }

    public RedDotNode GetRedDotNode(ERedDot reddot)
    {
        if (nodeMap == null) return null;

        if (nodeMap.TryGetValue(reddot, out RedDotNode node))
        {
            return node;
        }

        return null;
    }
}
