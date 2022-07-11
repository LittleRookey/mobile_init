using UnityEngine;
using UnityEngine.Events;

public static class Actions 
{
    /*
     *  Handles Enemy's Delegates
     */
    public static UnityAction<SA_Unit> OnEnemySpawned;

    public static UnityAction<SA_Unit> OnEnemyKilled;
}
