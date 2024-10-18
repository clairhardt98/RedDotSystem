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
                continue; // None, End, Root는 제외

            // 노드 생성
            RedDotNode node = new RedDotNode(redDotType);
            nodeMap[redDotType] = node;

            // 부모가 있는지 확인하여 부모 노드에 추가
            ERedDot parentType = GetParentType(redDotType);
            if (parentType != ERedDot.None && nodeMap.ContainsKey(parentType))
            {
                nodeMap[parentType].AddChild(node);
            }
            else
            {
                // 부모가 없으면 Root에 추가
                rootNode.AddChild(node);
            }

            BindEvaluateFunc(node);
        }

        return rootNode;
    }
    private ERedDot GetParentType(ERedDot redDotType)
    {
        // Enum 값에서 부모 관계를 추론하는 로직
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

        return ERedDot.None; // 부모가 없는 경우
    }
    private void BindEvaluateFunc(RedDotNode node)
    {
        // RedDotFunction 클래스에서 노드의 타입의 마지막 부분에 해당하는 메서드 이름 찾기
        string enumName = node.Type.ToString();
        int lastUnderscoreIndex = enumName.LastIndexOf('_');

        // 언더스코어 이후의 부분만 추출 (예: A_B_C -> C)
        string methodName = (lastUnderscoreIndex >= 0)
            ? enumName.Substring(lastUnderscoreIndex + 1)
            : enumName;

        // 해당 이름의 스태틱 메서드 찾기
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
