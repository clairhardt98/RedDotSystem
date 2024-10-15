using System;
using System.Collections.Generic;
using UnityEngine;

public class RedDotManager : MonoBehaviour
{
    private static RedDotManager _instance;
    public static RedDotManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new RedDotManager();
            }
            return _instance;
        }
    }

    private RedDotManager()
    {

    }

    Dictionary<RedDotTag, Func<bool>> _validateFunctions;

    public Func<bool> GetValidationMethod(RedDotTag redDotTag)
    {
        if(_validateFunctions.TryGetValue(redDotTag, out var func))
        {
            return func;
        }
        return null;
    }

    public void AddValidationMethod(RedDotTag redDotTag, Func<bool> method)
    {
        _validateFunctions[redDotTag] = method;
    }

    // get tag from RedDotTagContainer
    //public RedDotTag RequestRedDotTag(string tagLiteral)
    //{
    //    return null;
    //}

    public void Init()
    {
        // !TODO : Load RedDotTagContainer -> From prefab / server / file

        // !TODO : Initiate RedDotTree -> �±� ������� Ʈ���� ����, ��� ���� �� RedDot ��ü �ν��Ͻ� ����� �Լ� �̸����� ���ε�


        
    }
}
