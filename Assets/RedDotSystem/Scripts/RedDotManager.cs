using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public RedDotTag RequestRedDotTag(string tagLiteral)
    {
        return new RedDotTag(tagLiteral);
    }

    public void Init()
    {
        // initiate RedDotTree


        
    }
}
