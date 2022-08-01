using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TSpawner : MonoBehaviour
{
    [SerializeField]
    private string ID;

    [SerializeField]
    private float _radius;


    [SerializeField]
    public int _maxEnemySpawnNum;
    
    public bool _showGizmos;

    public SA_Unit _enemy;

    enum SpawnType
    {
        oneByOne,
        groupByGroup
    };
    [SerializeField] private SpawnType spawnType = SpawnType.groupByGroup;

    private ObjectPool<SA_Unit> _pool;

    private int enemiesRemainingAlive;
    public float spawnTimer;
    private float currentTime;
    private int totalSpawnNum = 0;

    /*
     * How Spawner works:
     * 1. Spawn number of normal enemy 
     * 2. once all enemies die, spawn next group of enemy after spawn time interval
     * 
     */

    private void Start()
    {
        //InitPool(_enemy);
        //_pool = new ObjectPool<SA_Unit>(() =>
        //{
        //    return Instantiate(_enemy);
        //}, enemy =>
        //{ // can init on enable getting
        //    enemy.gameObject.SetActive(true);
        //}, enemy =>
        //{
        //    enemy.gameObject.SetActive(false);
        //}, enemy =>
        //{
        //    Destroy(enemy.gameObject);
        //}, false, 10, 20);
    }
    private void OnEnable()
    {
        Actions.OnInitSpawner?.Invoke(this);

    }

    private void OnDisable()
    {

    }

    private void Update()
    {
        if (enemiesRemainingAlive == 0)
            currentTime += Time.deltaTime;

        if (currentTime >= spawnTimer)
        {
            switch(spawnType)
            {
                case SpawnType.oneByOne:
                    if (enemiesRemainingAlive < _maxEnemySpawnNum)
                    {
                        currentTime = 0f;
                        enemiesRemainingAlive += 1;
                        Vector2 spawnPoint = (Vector2)transform.position + Random.insideUnitCircle * _radius;
                        GameObject spawnedEnemy = PoolManager.SpawnObject(_enemy.gameObject, spawnPoint);
                        spawnedEnemy.transform.SetParent(SoonsoonData.Instance.SAM._unitPool[1].transform, false);
                        SA_Unit sa = spawnedEnemy.GetComponent<SA_Unit>();
                        sa.ID = totalSpawnNum;
                        sa._UnitSet._UnitSubset.SetNormal();
                        totalSpawnNum += 1;
                        Actions.OnEnemySpawn?.Invoke(sa);
                        sa.OnDeath += OnEnemyDeath;
                        sa.spawnedPos = transform.position;
                    }
                    break;
                case SpawnType.groupByGroup:
                    if (enemiesRemainingAlive < 1)
                    {
                        currentTime = 0f;
                        for (int i = 0; i < _maxEnemySpawnNum; i++)
                        {
                            enemiesRemainingAlive += 1;
                            Vector2 spawnPoint = (Vector2)transform.position + Random.insideUnitCircle * _radius;
                            GameObject spawnedEnemy = PoolManager.SpawnObject(_enemy.gameObject, spawnPoint);
                            spawnedEnemy.transform.SetParent(SoonsoonData.Instance.SAM._unitPool[1].transform, false);
                            SA_Unit sa = spawnedEnemy.GetComponent<SA_Unit>();
                            sa.ID = totalSpawnNum;
                            totalSpawnNum += 1;
                            Actions.OnEnemySpawn?.Invoke(sa);
                            sa.OnDeath += OnEnemyDeath;
                            sa.spawnedPos = transform.position;
                        } 

                    }
                    break;
            }
           
            
            //SoonsoonData.Instance.SAM.AddEnemy(spawnedEnemy.GetComponent<SA_Unit>());


            //SA_Unit spawnedEnemy = Instantiate(enemies[0], Vector3.zero, Quaternion.identity) as SA_Unit;
            //SA_Unit spawnEnemy = 
            //spawnedEnemy.OnDeath += OnEnemyDeath;

        }
    }

    void OneByOneSpawn()
    {
        currentTime = 0;
        enemiesRemainingAlive += 1;
        Vector2 spawnPoint = (Vector2)transform.position + Random.insideUnitCircle * _radius;
        GameObject spawnedEnemy = PoolManager.SpawnObject(_enemy.gameObject, spawnPoint);
        spawnedEnemy.transform.SetParent(SoonsoonData.Instance.SAM._unitPool[1].transform, false);
        SA_Unit sa = spawnedEnemy.GetComponent<SA_Unit>();
        sa.ID = totalSpawnNum;
        totalSpawnNum += 1;
        Actions.OnEnemySpawn?.Invoke(sa);
        sa.OnDeath += OnEnemyDeath;
        sa.spawnedPos = transform.position;
    }

    void GroupByGroupSpawn()
    {
        currentTime = 0;
        
    }

    public void OnEnemyDeath(SA_UnitBase sa)
    {
        SoonsoonData.Instance.SAM._enemyList.Remove(sa);
        enemiesRemainingAlive -= 1;
        sa.OnDeath -= OnEnemyDeath;
    }


    //public void TurnOffEnemy()
    //{
    //    Invoke()
    //}

    private void OnDrawGizmosSelected()
    {
        if (_showGizmos)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, _radius);

        }
    }
}
