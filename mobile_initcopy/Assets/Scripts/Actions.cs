using UnityEngine;
using UnityEngine.Events;

public static class Actions 
{
    /*
     *  Initializers 
     */
    
    public static UnityAction<TSpawner> OnInitSpawner;

    // Initializes enemy with max hp, stats and loot

    public static UnityAction<SA_Unit> OnEnemySpawn;
    /*
     *  Handles Enemy's Delegates
     */


    // When the character stat open button is pressed,
    // load player info to the stat window
    public static UnityAction<SA_Unit> OnOpenCharacterStat;

    public static UnityAction<float> OnHPChange;


    // Manages visual coin drop
    public static UnityAction<BounceDue> OnCoinDrop;

    // give rewards to player
    // manages exp, gold, proficiency, + etc
    public static UnityAction<SA_Unit> OnRewardEnemyKill;


    public static UnityAction OnPlayerLevelUp;


}
