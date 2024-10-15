using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RedDotTag
{
    public RedDotTag(string tagLiteral, string validateFuncName, string devComment = "")
    {
        TagLiteral = tagLiteral;
        DevComment = devComment;
        ValidateFuncName = validateFuncName;
    }
    [SerializeField] public string TagLiteral;
    [SerializeField] public string DevComment;
    [SerializeField] public string ValidateFuncName;
}

// container class for RedDotTags
[System.Serializable]
public class RedDotTagContainer : MonoBehaviour
{
    [SerializeField] public List<RedDotTag> redDotTags;
}
