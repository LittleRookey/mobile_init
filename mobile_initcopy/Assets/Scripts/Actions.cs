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
    public static UnityAction<SA_Player> OnOpenCharacterStat;

    public static UnityAction<float> OnHPChange;

    /// <summary>
    /// lerps the hp bar and enables the effect
    /// </summary>
    public static UnityAction OnPlayerLoseHP;

    // Manages visual coin drop
    public static UnityAction<BounceDue> OnCoinDrop;

    // give rewards to player
    // manages exp, gold, proficiency, + etc
    public static UnityAction<SA_UnitBase> OnRewardEnemyKill;


    public static UnityAction OnPlayerLevelUp;

    // when enemy got hit by player
    public static UnityAction<SA_UnitBase> OnEnemyHit;

    public static UnityAction OnEnemyDeath;

    public static void DrawRay(Vector3 startPos, Vector3 endPos)
    {

        Gizmos.DrawRay(startPos, (endPos - startPos).normalized);
    }
}
