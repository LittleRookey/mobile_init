using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    
    private void OnEnable()
    {
        Actions.OnInitSpawner += InitSpawner;
    }

    private void OnDisable()
    {
        Actions.OnInitSpawner -= InitSpawner;
    }
    
    public void SetupEnemy(SA_Unit unit)
    {
        
    }

    public void InitSpawner(Spawner spawner)
    {
        PoolManager.WarmPool(spawner._enemy.gameObject, spawner._maxEnemySpawnNum);
    }

    
}
