using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TalentType
{
    
};

public class Talent 
{
    protected SpriteRenderer _icon;
    protected int _level;
    protected int _exp;
    [TextArea(3, 5)]
    protected string _description;
    protected int _value; // 재능의 값을 나타냄
    
    public Talent(SpriteRenderer sp, string description, int value)
    {
        _icon = sp;
        _level = 1;
        _exp = 0;
        _description = description;
        _value = value;
    }

    public Talent(SpriteRenderer sp, int level, int exp, string description, int value)
    {
        _icon = sp;
        _level = level;
        _exp = exp;
        _description = description;
        _value = value;
    }

    public void SetTalent(int level, int exp, string description, int value)
    {
        this._level = level;
        this._exp = exp;
        this._description = description;
        this._value = value;
    }

    public void SetIcon(SpriteRenderer sp)
    {
        _icon = sp;
    }

    public void Init()
    {
        _level = 1;
        _exp = 0;
        _description = "";
        _value = 0;
    }
    
}

[System.Serializable]
public class SwordsMan : Talent
{
    [SerializeField]
    private int _str;

    public int _Strength
    {
        get
        {
            return _str;
        }
    }
    public SwordsMan(SpriteRenderer sp, string desc, int val, int addStrength=5) : base(sp, desc, val)
    {
        _str = addStrength;

    }

}
