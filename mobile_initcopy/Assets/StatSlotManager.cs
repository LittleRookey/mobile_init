using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatSlotManager : MonoBehaviour
{
    public StatSlot _MaxHPSlot;
    public StatSlot _AttackSlot;
    public StatSlot _HPRegenSlot;
    public StatSlot _MagicForceSlot;
    

    public void UpdateSlots()
    {
        if (StatManager.Instance._player != null)
        {
            _MaxHPSlot.UpdateStat(StatManager.Instance._player);
            _AttackSlot.UpdateStat(StatManager.Instance._player);
            _HPRegenSlot.UpdateStat(StatManager.Instance._player);
            _MagicForceSlot.UpdateStat(StatManager.Instance._player);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
