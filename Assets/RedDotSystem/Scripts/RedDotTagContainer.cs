using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RedDotTag
{
    public RedDotTag(string tagLiteral, string devComment = "")
    {
        TagLiteral = tagLiteral;

        if (string.IsNullOrEmpty(tagLiteral))
        {
            Depth = 0;
        }
        string[] parts = tagLiteral.Split('.');
        Depth = parts.Length;
        DevComment = devComment;
    }

    public static bool operator ==(RedDotTag left, RedDotTag right) => System.Object.Equals(left, right);

    public static bool operator !=(RedDotTag left, RedDotTag right) => !System.Object.Equals(left, right);
    public override bool Equals(object obj)
    {
        return obj is RedDotTag tag &&
               TagLiteral == tag.TagLiteral &&
               Depth == tag.Depth;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(TagLiteral, Depth);
    }

    private string TagLiteral;
    private string DevComment;
    public int Depth;
}
public class RedDotTagContainer : MonoBehaviour
{
    [SerializeField] List<RedDotTag> redDotTags;
}