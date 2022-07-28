using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EffectManager : MonoBehaviour
{
    [Header("VFX objects")]
    [SerializeField] private GameObject swordSlash_normal;
    [SerializeField] private GameObject swordhit_vfx;
    [SerializeField] private ParticleSystem hploseEffect;
    [SerializeField] private ParticleSystem mploseEffect;

    [Header("VFX settings")]
    public Vector2 effectSpawnPos;
    public static UnityAction<SA_UnitBase> OnPlayerAttackV1;
    public static UnityAction<SA_UnitBase> OnPlayerAttackV2;

    public static UnityAction<GameObject> OnEffectDisabled;

    // Start is called before the first frame update
    void Start()
    {
        PoolManager.WarmPool(swordSlash_normal, 4);
        PoolManager.WarmPool(swordhit_vfx, 3);
    }

    void PlaySlashV1(SA_UnitBase targ)
    {
        if (!targ.isPlayer) return;
        GameObject go = PoolManager.SpawnObject(swordSlash_normal, (Vector2)targ.transform.position);
        go.transform.localScale = new Vector3(targ._spumPrefab._anim.transform.localScale.x * -1, -1, 1);
        
    }

    void PlaySlashV2(SA_UnitBase targ)
    {
        if (!targ.isPlayer) return;
        GameObject go = PoolManager.SpawnObject(swordSlash_normal, (Vector2)targ.transform.position);
        go.transform.localScale = new Vector3(targ._spumPrefab._anim.transform.localScale.x * -1, 1, 1);
    }

    void PlaySwordHit(SA_UnitBase targ)
    {
        
        GameObject go = PoolManager.SpawnObject(swordhit_vfx, (Vector2)targ.transform.position + Vector2.up * 0.5f);
        go.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
    }

    public void ReleaseObject(GameObject go, float sec)
    {
        PoolManager.ReleaseObject(go);
    }
    
    public void TurnHPLoseParticle(int val)
    {
        var emiss = hploseEffect.emission;
        emiss.rateOverTime = 10f;
        
    }

    void loseeffect()
    {
        var emiss = hploseEffect.emission;
        emiss.rateOverTime = 0f;
    }

    private void OnEnable()
    {
        OnPlayerAttackV1 += PlaySlashV1;
        OnPlayerAttackV2 += PlaySlashV2;
        Actions.OnEnemyHit += PlaySwordHit;
    }

    private void OnDisable()
    {
        OnPlayerAttackV1 -= PlaySlashV1;
        OnPlayerAttackV2 -= PlaySlashV2;
        Actions.OnEnemyHit -= PlaySwordHit;
    }
}
