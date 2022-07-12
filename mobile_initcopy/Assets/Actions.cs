using UnityEngine;
using UnityEngine.Events;

public static class Actions 
{
    /*
     *  Initializers 
     */
    public static UnityAction<Spawner> OnInitSpawner;

    // Initializes enemy with max hp, stats and loot
    public static UnityAction<SA_Unit> OnEnemySpawn;
    /*
     *  Handles Enemy's Delegates
     */

    public static UnityAction<SA_Unit> OnEnemyKilled;

    
}
