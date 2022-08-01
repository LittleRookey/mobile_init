using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections;
using DG.Tweening;


public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject hpBar;
    [SerializeField] private GameObject mpBar;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI mpText;
    
    [SerializeField] private GameObject expBar;

    public ParticleSystem hploseEffect;

    public static UnityAction<SA_UnitBase> OnUpdateInfoBar;

    public static UnityAction<SA_Player> OnUpdateExpBar;

    public static UnityAction<SA_UnitBase> OnUpdateHPBar;

    public static UnityAction<int> OnUpdateGold;

    string perc = "%";

    public bool hpChanging;

    Color alphafull;
    Color alphazero;

    
    public void UpdateInfoBox(SA_UnitBase sa)
    {

        UpdateHPBar(sa);

        if (sa._unitMagicForce > 0f)
        {
            float val = sa._unitMana / 100;
            mpBar.transform.localScale = new Vector2(val, 1f);
        }

        UpdateEXPBar(sa);
    }

    void UpdateHPBar(SA_UnitBase sa)
    {
        float tValue = sa._unitHP / sa._unitMaxHP;
        if (tValue < 0f)
            tValue = 0f;
        else if (tValue > 1f)
            tValue = 1f;

        //hploseEffect.gameObject.SetActive(true);

        MoveAlphaParticle(hploseEffect, alphafull, .01f);
        //MoveBar(hpBar, tValue, 1f);
        //StartCoroutine(MoveBarWithEffect(hpBar, tValue, 1f, hploseEffect));
        MoveBar(hpBar, tValue, 1f);
        MoveAlphaParticle(hploseEffect, alphazero, .9f);
        //DOTween.To(() => hpBar.transform.localScale.x, x => hpBar.transform.localScale = new Vector3(x, 1f, 1f), tValue, 1f);
        //hpBar.transform.localScale.
        //hpBar.transform.localScale = new Vector3(tValue, 1f, 1f);
        hpText.text = (tValue* 100).ToString("F1") + perc;
    }

    void UpdateEXPBar(SA_UnitBase sa)
    {
        //Debug.Log("EXP UPdated");
        float tValue = (float)sa._exp / sa._maxExp;

        if (tValue == expBar.transform.localScale.x) return;
        if (tValue >= 1f)
        {
            tValue = 1f;
        }
        //Debug.Log(sa._exp + " / " + sa._maxExp);
        //Debug.Log(sa.gameObject.name);
        //Debug.Log(tValue);

        MoveBar(expBar, tValue, 1f);
        //expBar.transform.localScale = new Vector3(tValue, 1f, 1f);
        //StartCoroutine(MoveBar(expBar, expBar.transform.localScale, new Vector3(tValue, 1f, 1f), 1f));
    }

    void MoveAlphaParticle(ParticleSystem targObject, Color tValue, float dur=1f)
    {
        ParticleSystem.MainModule sett = targObject.main;
        DOTween.To(() => targObject.main.startColor.color, x => sett.startColor = x, tValue, dur);
    }

    public static void MoveBar(GameObject targObject, float tValue, float dur=1f)
    {
        
        DOTween.To(() => targObject.transform.localScale.x, x => targObject.transform.localScale = new Vector3(x, 1f, 1f), tValue, dur);
        
    }

    IEnumerator MoveBarWithEffect(GameObject targObject, float tValue, float dur = 1f, ParticleSystem effects = null)
    {
        
        DOTween.To(() => targObject.transform.localScale.x, x => targObject.transform.localScale = new Vector3(x, 1f, 1f), tValue, dur);
        if (effects != null)
            effects.gameObject.SetActive(true);
        yield return new WaitForSeconds(dur);
        if (effects != null)
            effects.gameObject.SetActive(false);

    }

    //private IEnumerator MoveBar(GameObject ob, Vector3 start, Vector3 end, float dur)
    //{
    //    float timer = 0;
    //    while (ob.transform.localScale.x != end.x)
    //    {
    //        timer += Time.deltaTime;
    //        ob.transform.localScale = Vector3.Lerp(start, end, 0.5f * Time.deltaTime);
    //    }
    //    yield return 1;
    //}

    private void Start()
    {
        hploseEffect.gameObject.SetActive(true);
        MoveAlphaParticle(hploseEffect, alphazero, .5f);
        mpText.text = 0.ToString();
        ParticleSystem.MainModule sett = hploseEffect.main;
        alphafull = sett.startColor.color;
        alphazero = new Color(alphafull.r, alphafull.g, alphafull.b, 0f);
        //expBar.transform.localScale = tempVec;
        //UpdateInfoBox(StatManager.Instance._player);
        //UpdateHPBar(StatManager.Instance._player);
        UpdateHPBar(StatManager.Instance._player);
        UpdateEXPBar(StatManager.Instance._player);
    }

    private void OnEnable()
    {
        OnUpdateInfoBar += UpdateInfoBox;
        OnUpdateHPBar += UpdateHPBar;
        OnUpdateExpBar += UpdateEXPBar;
    }

    private void OnDisable()
    {
        OnUpdateInfoBar -= UpdateInfoBox;
        OnUpdateHPBar -= UpdateHPBar;
        OnUpdateExpBar -= UpdateEXPBar;
    }

    private void Update()
    {
        
    }
}
