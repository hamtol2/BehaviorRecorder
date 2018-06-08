using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static bool CompareTwoStrings(string str1, string str2)
    {
        return str1.Equals(str2, StringComparison.CurrentCultureIgnoreCase);
    }

    public static float GetAngleBetween(Vector2 start, Vector2 end)
    {
        Vector2 target = end - start;
        return Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
    }

    public static float GetDistanceBetween(Vector2 start, Vector2 end)
    {
        Vector2 target = end - start;
        return target.magnitude;
    }

    public static T ReadFromJson<T>(TextAsset jsonText)
    {
        return SimpleJson.SimpleJson.DeserializeObject<T>(jsonText.text);
    }

    public static string RemoveAllWhiteSpace(string targetString)
    {
        return targetString.Replace(" ", string.Empty);
    }
}