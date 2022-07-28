using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject hpBar;
    [SerializeField] private GameObject mpBar;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI mpText;
    
    [SerializeField] private GameObject expBar;


    public static UnityAction<SA_UnitBase> OnUpdateInfoBar;

    public static UnityAction<SA_Player> OnUpdateExpBar;

    public static UnityAction<SA_UnitBase> OnUpdateHPBar;

    public static UnityAction<int> OnUpdateGold;

    string perc = "%";


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

        
        hpBar.transform.localScale = new Vector3(tValue, 1f, 1f);
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


        expBar.transform.localScale = new Vector3(tValue, 1f, 1f);
        //StartCoroutine(MoveBar(expBar, expBar.transform.localScale, new Vector3(tValue, 1f, 1f), 1f));
    }

    private IEnumerator MoveBar(GameObject ob, Vector3 start, Vector3 end, float dur)
    {
        float timer = 0;
        while (ob.transform.localScale.x != end.x)
        {
            timer += Time.deltaTime;
            ob.transform.localScale = Vector3.Lerp(start, end, 0.5f * Time.deltaTime);
        }
        yield return 1;
    }

    private void Start()
    {
        
        mpText.text = 0.ToString();


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

}
