using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField]
    private float _radius;

    [SerializeField]
    public int _maxEnemySpawnNum;

    public bool _showGizmos;

    public SA_Unit _enemy;


    private int enemiesRemainingAlive;
    public float spawnTimer;
    private float currentTime;


    /*
     * How Spawner works:
     * 1. Spawn number of normal enemy 
     * 2. once all enemies die, spawn next group of enemy after spawn time interval
     * 
     */ 

    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= spawnTimer && enemiesRemainingAlive < _maxEnemySpawnNum)

        {
            currentTime = 0;
            enemiesRemainingAlive += 1;
            Vector2 spawnPoint = (Vector2)transform.position + Random.insideUnitCircle * _radius;
            GameObject spawnedEnemy = PoolManager.SpawnObject(_enemy.gameObject, spawnPoint);
            spawnedEnemy.transform.parent = SoonsoonData.Instance.SAM._unitPool[1];
            SoonsoonData.Instance.SAM.AddEnemy(spawnedEnemy.GetComponent<SA_Unit>());


            //SA_Unit spawnedEnemy = Instantiate(enemies[0], Vector3.zero, Quaternion.identity) as SA_Unit;
            //SA_Unit spawnEnemy = 
            //spawnedEnemy.OnDeath += OnEnemyDeath;
            
        }
    }

    public void OnEnemyDeath()
    {

    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
