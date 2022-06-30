using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CleverCrow.Fluid.StatsSystem;
using CleverCrow.Fluid.StatsSystem.StatsContainers;

public class EquipmentManager : MonoBehaviour
{

    //public Equipment chainmail;
    public StatsAdjustment armor;

    public StatsContainer originalStats; // original stat
    [SerializeField]
    protected StatsContainer runtimeStats;
    // Start is called before the first frame update
    void Start()
    {
        runtimeStats = originalStats.CreateRuntimeCopy();
        Debug.Log("before: " + runtimeStats.GetStatInt("health"));
        armor.ApplyAdjustment(runtimeStats);
        Debug.Log("after: " +runtimeStats.GetStatInt("health", 10));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PrintCharacterStat()
    {
        
        runtimeStats.records.records.ForEach(record => Debug.Log(record));
    }

    public void PrintEquipmentStat()
    {
         //records.records.ForEach(record => Debug.Log(record));
    }
}
