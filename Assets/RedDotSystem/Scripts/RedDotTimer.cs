using RedDotSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// 코루틴을 사용을 위한 MonoBehaviour 상속
public class RedDotTimer : MonoBehaviour
{
    private static GameObject thisObj;
    
    private Dictionary<ERedDot, Coroutine> _timerCoroutineMap;

    // 인스턴스 생성 및 반환해주는 팩토리 함수. 이미 생성된 후 호출될 경우 Assert
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
