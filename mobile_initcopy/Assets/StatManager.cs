using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatManager : MonoBehaviour
{
    public SA_Unit _player;
    [SerializeField] private TextMeshProUGUI strText, dexText, intText, vitText, statPointText;


    private void Start()
    {
        UpdateStatUI();
    }


    public void LevelUp()
    {
        _player._Level += 1;
        _player._statPoint += 1;
        UpdateStatUI();
    }

    public void UpdateStatUI()
    {
        strText.text = "Strength " + _player._mStats._Strength;
        dexText.text = "Dexterity " + _player._mStats._Dexterity;
        intText.text = "Intelligence " + _player._mStats._Intelligence;
        vitText.text = "Vitality " + _player._mStats._Vitality;
        statPointText.text = "Points left: " + _player._statPoint;
    }
    public void AddDexterity(int num=1)
    {
        if (_player._statPoint - num < 0)
        {
            // Doesn't have enough stat point
            return;
        }
        for (int i = 0; i < num; i++)
        {
            _player._unitAttack += 1f;
            _player._unitAttackSpeed -= 0.01f;
        }
        _player._mStats._Dexterity += num;
        _player._statPoint -= num;
        UpdateStatUI();
    }

    public void AddStrength(int num=1)
    {
        if (_player._statPoint - num < 0)
        {
            // Doesn't have enough stat point
            return;
        }

        _player._mStats._Strength += num;
        for (int i = 0; i < num; i++)
        {
            _player._unitAttack += 3.0f;
            _player._unitMaxHP += 10.0f;
        }
        _player._statPoint -= num;
        UpdateStatUI();
    }

    public void AddIntelligence(int num=1)
    {
        if (_player._statPoint - num < 0)
        {
            // Doesn't have enough stat point
            return;
        }
        _player._mStats._Intelligence += num;
        for (int i = 0; i < num; i++)
        {
            _player._unitMagicAttack += 4.0f;
            _player._unitMana += 5.0f;
            _player._unitManaRegen += 0.1f;
        }
        _player._statPoint -= num;
        UpdateStatUI();
    }

    public void AddVitality(int num=1)
    {
        if (_player._statPoint - num < 0)
        {
            // Doesn't have enough stat point
            Debug.Log(_player._statPoint + " " + num);
            return;
        }

        for (int i = 0; i < num; i++)
        {
            _player._unitMaxHP += 25.0f;
            _player._unitHPRegen += 0.5f;
        }
        _player._mStats._Vitality += num;
        _player._statPoint -= num;
        UpdateStatUI();
    }
}
