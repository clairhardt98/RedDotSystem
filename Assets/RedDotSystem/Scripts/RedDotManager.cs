using RedDotSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RedDotManager : MonoBehaviour
{
    Dictionary<ERedDot, RedDotNode> _nodeMap;
    Dictionary<ERedDot, Coroutine> _timerCoroutineMap;
    RedDotNode _root;
    #region Singleton
    private static RedDotManager _instance;
    public static RedDotManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("RedDotManager");
                _instance = obj.AddComponent<RedDotManager>();

                _instance.Init();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    #endregion

    private void Init()
    {
        _root = BuildRedDotTree();
        InitTree();
    }

    private RedDotNode BuildRedDotTree()
    {
        Assert.IsNull(_nodeMap, "NodeMap Already Init");

        _nodeMap = new();

        RedDotNode rootNode = new RedDotNode(ERedDot.Root);

        _nodeMap[ERedDot.Root] = rootNode;

        foreach (ERedDot redDotType in Enum.GetValues(typeof(ERedDot)))
        {
            if (redDotType == ERedDot.None || redDotType == ERedDot.End || redDotType == ERedDot.Root)
                continue; 

            RedDotNode node = new RedDotNode(redDotType);
            _nodeMap[redDotType] = node;

            ERedDot parentType = GetParentType(redDotType);
            if (parentType != ERedDot.None && _nodeMap.ContainsKey(parentType))
            {
                _nodeMap[parentType].AddChild(node);
            }
            else
            {
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
        string enumName = node.RedDotType.ToString();
        int lastUnderscoreIndex = enumName.LastIndexOf('_');

        string methodName = (lastUnderscoreIndex >= 0)
            ? enumName.Substring(lastUnderscoreIndex + 1)
            : enumName;

        var method = typeof(RedDotFunction).GetMethod(methodName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

        if (method != null)
        {
            var evaluateFunc = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), method);
            node.BindEvaluateFunc(evaluateFunc);
            Debug.LogWarning($"Method {methodName} Bound Success.");
        }
    }

    private void InitTree()
    {
        Assert.IsNotNull(_root);
        _root.Evaluate();
    }

    public RedDotNode GetRedDotNode(ERedDot reddot)
    {
        if (_nodeMap == null) return null;

        if (_nodeMap.TryGetValue(reddot, out RedDotNode node))
        {
            return node;
        }

        return null;
    }

    public void RegisterTimer(ERedDot redDot, float duration)
    {
        if (_timerCoroutineMap == null)
            _timerCoroutineMap = new();

        // if an RedDot already has a timer, stop it
        if (_timerCoroutineMap.ContainsKey(redDot))
        {
            StopCoroutine(_timerCoroutineMap[redDot]);
            _timerCoroutineMap.Remove(redDot);

            Debug.LogWarning($"{redDot} timer coroutine already exists. Stopped.");
        }

        _timerCoroutineMap[redDot] = StartCoroutine(TimerCoroutine(redDot, duration)); 
    }

    private IEnumerator TimerCoroutine(ERedDot redDot, float duration)
    {
        float startTime = Time.time;

        while(Time.time - startTime < duration)
        {
            yield return null;
        }
        
        if(_nodeMap.TryGetValue(redDot, out RedDotNode redDotNode))
        {
            Debug.Log($"{redDot} timer coroutine ends. Evaluating {redDot}");
            redDotNode.Evaluate();
        }

        _timerCoroutineMap.Remove(redDot);
    }
}
