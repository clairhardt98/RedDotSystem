using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedDot
{
    RedDotTag _redDotTag;
    public RedDotTag RedDotTag => _redDotTag;
    public bool On { get; set; }

    // method that validates its on/off
    private Func<bool> _validationMethod;
    public RedDot(RedDotTag redDotTag)
    {
        _redDotTag = redDotTag;
        _validationMethod = RedDotManager.Instance.GetValidationMethod(_redDotTag);
    }
}

