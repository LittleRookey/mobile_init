using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.StatsSystem;
public enum TalentType
{
    베기, 
    찌르기
};

[CreateAssetMenu(fileName = "Talent", menuName = "Litkey/Talent")]
public class Talent : ScriptableObject
{
    public SpriteRenderer _icon;

    public string id;
    public string _name;
    public int _level;
    public int _exp;
    [TextArea(3, 5)]
    public string _description;
    public int _value; // 재능의 값을 나타냄

    public TalentType _talentType;
    public void Init()
    {
        _level = 1;
        _exp = 0;
        _description = "";
        _value = 0;
    }

    
}

public static class ScriptableObjectExtension
{
    /// <summary>
    /// Creates and returns a clone of any given scriptable object.
    /// </summary>
    public static T Clone<T>(this T scriptableObject) where T : ScriptableObject
    {
        if (scriptableObject == null)
        {
            Debug.LogError($"ScriptableObject was null. Returning default {typeof(T)} object.");
            return (T)ScriptableObject.CreateInstance(typeof(T));
        }

        T instance = Object.Instantiate(scriptableObject);
        instance.name = scriptableObject.name; // remove (Clone) from name
        return instance;
    }
}
