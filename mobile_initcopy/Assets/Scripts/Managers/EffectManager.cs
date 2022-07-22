using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private GameObject swordSlash_normal;

    public Vector2 effectSpawnPos;
    public static UnityAction<SA_Unit> OnPlayerAttackV1;
    public static UnityAction<SA_Unit> OnPlayerAttackV2;


    // Start is called before the first frame update
    void Start()
    {
        PoolManager.WarmPool(swordSlash_normal, 4);
    }

    void PlaySlashV1(SA_Unit targ)
    {
        if (!targ.isPlayer) return;
        GameObject go = PoolManager.SpawnObject(swordSlash_normal, (Vector2)targ.transform.position);
        go.transform.localScale = new Vector3(targ._spumPrefab._anim.transform.localScale.x * -1, -1, 1);
    }

    void PlaySlashV2(SA_Unit targ)
    {
        if (!targ.isPlayer) return;
        GameObject go = PoolManager.SpawnObject(swordSlash_normal, (Vector2)targ.transform.position);
        go.transform.localScale = new Vector3(targ._spumPrefab._anim.transform.localScale.x * -1, 1, 1);
    }


    private void OnEnable()
    {
        OnPlayerAttackV1 += PlaySlashV1;
        OnPlayerAttackV2 += PlaySlashV2;
    }

    private void OnDisable()
    {
        OnPlayerAttackV1 -= PlaySlashV1;
        OnPlayerAttackV2 -= PlaySlashV2;
    }
}
