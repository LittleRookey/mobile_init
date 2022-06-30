using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SA_UnitSet : MonoBehaviour
{

    
    public enum UnitType
    {
        none,
        P1, 
        P2,
        P3
    };

    public UnitType _unitType = UnitType.none;

    public SA_Unit _unitST;

    public SA_UnitSubset _UnitSubset;

    SA_ResourceManager SARM;

    public float _timerForHP;

    public Rigidbody2D _rigidBody;

    public CapsuleCollider2D _collider;

    public Vector3 _dmgPopupOffset;

    Vector3 _myPos;
    // Start is called before the first frame update
    void Start()
    {
        if (_UnitSubset != null)
        {
            _UnitSubset._hpList[0].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!Application.isPlaying)
        {
            Vector3 mPos = new Vector3((int)transform.position.x, (int)transform.position.y, transform.position.y * 0.1f);
            transform.localPosition = mPos;
            SetUnitType();
            // Resource management TODO: Find resource manager or not
            if (SARM == null) SARM = FindObjectOfType<SA_ResourceManager>();
        }
        else
        {
            transform.localPosition = new Vector3(transform.position.x, transform.position.y, transform.position.y * 0.1f);
            if (_UnitSubset == null) return;
            if (_UnitSubset._hpList[0].gameObject.activeInHierarchy)
            {
                _timerForHP += Time.deltaTime;
                if (_timerForHP > 1f)
                {
                    _UnitSubset._hpList[0].gameObject.SetActive(false);  
                }
            }
        }
    }

    public void CalcHPState()
    {
        if (gameObject.CompareTag("P3")) return;
        if (_UnitSubset == null) return;
        //hplist[0]은 전체 hp
        // hplist[2]는 pivot 
        _UnitSubset._hpList[0].gameObject.SetActive(true);
        float tValue = _unitST._unitHP * (1/_unitST._unitMaxHP);
        _UnitSubset._hpList[2].transform.localScale = new Vector3(tValue, 1, 1);

        _timerForHP = 0;
    }

    public void ShowDmgText(SA_Unit.AttackType type, float value)
    {
        SA_ResourceManager.Instance.GetDamageNumber(type).Spawn(transform.position + _dmgPopupOffset, value);
        //SA_ResourceManager.Instance._normalDamagePrefab.Spawn(transform.position + _dmgPopupOffset, value);
    }

    void SetUnitType()
    {
        if (_unitType == UnitType.none) return;
        if (_unitType != UnitType.P3)
        {
            bool check = false;

            if (_unitST == null) check = true;
            if (_unitST._spumPrefab == null) check = true;
            if (_rigidBody == null) check = true;
            if (_collider == null) check = true;

            if (check) UnitTypeProcess();

            switch(_unitType)
            {
                case UnitType.P1:
                    gameObject.tag = "P1";
                    _unitST._spumPrefab._anim.transform.localScale = new Vector3(1, 1, 1);
                    break;

                case UnitType.P2:
                    gameObject.tag = "P2";
                    _unitST._spumPrefab._anim.transform.localScale = new Vector3(-1, 1, 1);
                    break;

                case UnitType.P3:
                    gameObject.tag = "P3";
                    break;
                case UnitType.none:
                    break;
            }
        }
    }

    public void UnitTypeProcess()
    {
        _unitST = GetComponent<SA_Unit>();
        if (_unitST != null) DestroyImmediate(_unitST);
        _unitST = gameObject.AddComponent<SA_Unit>();
        _unitST._spumPrefab = GetComponent<SPUM_Prefabs>();
        _unitST._UnitSet = this;

        _unitST._mStatContainer = SA_ResourceManager.Instance.GetCharacterStat();
        UnitInitSet();
        

        SA_AnimationAction tSA = _unitST._spumPrefab._anim.gameObject.GetComponent<SA_AnimationAction>();
        if (tSA != null) DestroyImmediate(tSA);
        tSA = _unitST._spumPrefab._anim.gameObject.AddComponent<SA_AnimationAction>();
        tSA._player = _unitST;


        _rigidBody = GetComponent<Rigidbody2D>();
        if (_rigidBody != null) DestroyImmediate(_rigidBody);
        _rigidBody = gameObject.AddComponent<Rigidbody2D>();
        _rigidBody.mass = 1;
        _rigidBody.drag = 1;
        _rigidBody.angularDrag = 1;
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rigidBody.gravityScale = 0f;

        _collider = GetComponent<CapsuleCollider2D>();
        if (_collider != null) DestroyImmediate(_collider);
        _collider = gameObject.AddComponent<CapsuleCollider2D>();
        _collider.offset = new Vector2(0, 0.25f);
        _collider.size = new Vector2(0.5f, 0.5f);

        _dmgPopupOffset = new Vector3(0f, 1.3f, 0f);

        if (_UnitSubset != null) DestroyImmediate(_UnitSubset);

        List<GameObject> tObj = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            string tName = transform.GetChild(i).name;
            if (tName == "SA_UnitSubset")
            {
                tObj.Add(transform.GetChild(i).gameObject);
            }
        }

        if (tObj.Count > 0)
        {
            foreach(GameObject obj in tObj)
            {
                DestroyImmediate(obj);
            }
        }
        //Debug.Log(SA_ResourceManager.Instance._hpBar.name);
        _UnitSubset = Instantiate(SA_ResourceManager.Instance._hpBar).GetComponent<SA_UnitSubset>();
        _UnitSubset.gameObject.name = "SA_UnitSubset";
        _UnitSubset.transform.SetParent(transform);
        _UnitSubset.transform.localScale = Vector3.one;
        _UnitSubset.transform.localPosition = Vector3.zero;

    }

    void UnitInitSet()
    {
        _unitST.InitStat();
        _unitST._unitFightRange = 200f;
        _unitST._unitAttackRange = 1f;

        //_unitST._unitHP = 100f;
        //_unitST._unitMaxHP = _unitST._unitHP;
        //_unitST._unitMoveSpeed = 1f;
        //_unitST._unitAttack = 10f;
        //_unitST._unitAttackSpeed = 1f;
        //_unitST._unitHPRegen = 1 / _unitST._unitHP;

        
    }
}
