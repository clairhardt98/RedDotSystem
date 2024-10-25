using RedDotSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// �ڷ�ƾ�� ����� ���� MonoBehaviour ���
public class RedDotTimer : MonoBehaviour
{
    private static GameObject thisObj;
    
    private Dictionary<ERedDot, Coroutine> _timerCoroutineMap;

    // �ν��Ͻ� ���� �� ��ȯ���ִ� ���丮 �Լ�. �̹� ������ �� ȣ��� ��� Assert
    public static RedDotTimer CreateTimer(Transform parent = null)
    {
        Assert.IsNull(thisObj, "Timer Already Created");

        thisObj = new GameObject("RedDotTimer");

        if (parent != null)
            thisObj.transform.SetParent(parent);

        return thisObj.AddComponent<RedDotTimer>();
    }

    public void RegisterTimer(ERedDot redDot, float duration, Action onTimerEndCallback = null)
    {
        if (_timerCoroutineMap == null)
            _timerCoroutineMap = new();

        if (_timerCoroutineMap.ContainsKey(redDot))
        {
            StopCoroutine(_timerCoroutineMap[redDot]);
            _timerCoroutineMap.Remove(redDot);

            Debug.LogWarning($"{redDot} timer coroutine already exists. Stopped.");
        }

        _timerCoroutineMap[redDot] = StartCoroutine(TimerCoroutine(redDot, duration, onTimerEndCallback));
    }

    private IEnumerator TimerCoroutine(ERedDot redDot, float duration, Action onTimerEndCallback)
    {
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            yield return null;
        }

        RedDotNode redDotNode = RedDotManager.Instance.GetRedDotNode(redDot);
        if (redDotNode != null)
        {
            Debug.Log($"{redDot} timer coroutine ends. Evaluating {redDot}");
            redDotNode.Evaluate();

            onTimerEndCallback?.Invoke();
        }

        _timerCoroutineMap.Remove(redDot);
    }
}
