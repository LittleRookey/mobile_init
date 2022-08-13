using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    TSpawner currentSpawner;

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

    public void InitSpawner(TSpawner spawner)
    {
        currentSpawner = spawner;
        PoolManager.WarmPool(spawner._enemy.gameObject, spawner._maxEnemySpawnNum);
    }

    
}
