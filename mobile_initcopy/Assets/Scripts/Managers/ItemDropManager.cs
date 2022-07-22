using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemDropManager : MonoBehaviour
{
    [SerializeField] private SA_Unit player;

    public int smallParticlesNum = 5;
    public float magnetSpeed = 3f;
    //public float dropRadius = 2f;

    [SerializeField] private GameObject coinDrop;
    [SerializeField] private GameObject expDrop;

    /*
     *  Manages drop visual items
     */
    public static UnityAction<SA_Unit> DropItems;

    private void Awake()
    {
        //if (player == null) player = GameObject.FindGameObjectWithTag("Player").GetComponent<SA_Unit>();
    }
    private void Start()
    {
        PoolManager.WarmPool(coinDrop, 20);
        //PoolManager.WarmPool(expDrop, 20);
    }
    public void DropExps()
    {
        //for (int i = 0; i < smallParticlesNum; i++)
        //{
        //    float _x = Random.Range(0.2f, dropRadius);
        //    float _y = Random.Range(0.2f, dropRadius);
        //    GameObject exp = PoolManager.SpawnObject(expDrop);
        //    exp.transform.localPosition += new Vector3(_x, _y, 0f);
        //}
    }

    public void DropCoin(SA_Unit sa)
    {
        StartCoroutine(SpawnCoin(sa));
        
    }

    IEnumerator SpawnCoin(SA_Unit sa)
    {
        WaitForSeconds sec = new WaitForSeconds(.1f);
        for (int i = 0; i < smallParticlesNum; i++)
        {
            GameObject coin = PoolManager.SpawnObject(coinDrop);
            //coin.transform.localPosition += (sa.transform.position + (Vector3)Random.insideUnitCircle * dropRadius);
            coin.transform.position = sa.transform.position;
            yield return sec;
        }
    }

    public void MoveToPlayer(BounceDue bd)
    {
        bd.transform.position = Vector3.Lerp(bd.transform.position, player.transform.position, magnetSpeed * Time.deltaTime);
        if ((player.transform.position - bd.transform.position).sqrMagnitude <= 0.1f)
        {
            PoolManager.ReleaseObject(bd.gameObject);
        }
    }

    private void OnEnable()
    {
        DropItems += DropCoin;
        Actions.OnCoinDrop += MoveToPlayer;
    }

    private void OnDisable()
    {
        DropItems -= DropCoin;
    }
}
