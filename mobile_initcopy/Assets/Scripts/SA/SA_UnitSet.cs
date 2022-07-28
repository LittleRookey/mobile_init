using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SA_UnitSet : MonoBehaviour
{
    public enum EnemyType
    {
        Small,
        Normal,
        Elite,
        Boss
    };
    
    public enum UnitType
    {
        none,
        Player, 
        Enemy,
        Object
    };

    public bool isPlayer;

    public UnitType _unitType = UnitType.none;

    public SA_UnitBase _unitST;

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
        if (!isPlayer)
            _unitST = GetComponent<SA_Unit>();
        else
            _unitST = GetComponent<SA_Player>();
        SpriteRenderer[] mats = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer mat in mats)
        {
            mat.material = SA_ResourceManager.Instance.hitMatsDefault;
        }

        if (!_UnitSubset)
        {
            //bool chk = false;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<SA_UnitSubset>() != null)
                {
                    //chk = true;
                    DestroyImmediate(transform.GetChild(i));
                }
                
            }    
            SetUnitToBattle(); // get ref of unitsubset
        }


        if (_UnitSubset != null)
        {
            _UnitSubset._hpList[0].gameObject.SetActive(false);
        }
        if (SA_ResourceManager.Instance.turnOnHPBarAlways) _UnitSubset._hpList[0].gameObject.SetActive(true);

        _UnitSubset._levelText.text = _unitST._level.ToString();

        // Set Material to all sprite renderer of child 
    }


    private void OnEnable()
    {
        if (_unitST.isPlayer)
        {
            Actions.OnPlayerLevelUp += UpdateLevel;
        } 
        else if(!_unitST.isPlayer)
        {
            
            //gameObject.layer = LayerMask.GetMask("Units");

        }
        // TODO set the enemy stat, loot, reset enemy state
        Actions.OnHPChange += AddHP;
    }

    private void OnDisable()
    {
        if (_unitST.isPlayer)
        {
            Actions.OnPlayerLevelUp -= UpdateLevel;
        }
        else if (!_unitST.isPlayer)
        {

        }
        Actions.OnHPChange -= AddHP;
    }



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
            if (_UnitSubset.alwaysTurnOnHPBar && !SA_ResourceManager.Instance.turnOnHPBarAlways && _UnitSubset._hpList[0].gameObject.activeInHierarchy)
            {
                _timerForHP += Time.deltaTime;
                if (_timerForHP > SA_ResourceManager.ENEMYHPTIME)
                {
                    _UnitSubset._hpList[0].gameObject.SetActive(false);  
                }
            }
        }
    }

    void UpdateLevel()
    {
        _UnitSubset._levelText.text = _unitST._level.ToString();
    }

    public void AddHP(float val)
    {
        SA_UnitBase sa = _unitST;
        if (sa._unitState == SA_UnitBase.UnitState.death) return;
        if (!sa.gameObject.activeInHierarchy) return;
        if (sa._unitHP <= 0) return;
        if (val < 0 && _unitST.isPlayer)
        {

        }

        if (sa._unitHP + val > sa._unitMaxHP)
        {
            sa._unitHP = sa._unitMaxHP;
        } else
        {
            sa._unitHP += val;
        }
        UpdateHPBar();
    }

    public void CalcHPState()
    {
        if (gameObject.CompareTag("Object")) return;
        if (_UnitSubset == null) return;
        //hplist[0]은 전체 hp
        // hplist[2]는 pivot 
        _UnitSubset._hpList[0].gameObject.SetActive(true);
        float tValue = _unitST._unitHP * (1/_unitST._unitMaxHP);
        //Debug.Log(_unitST._unitHP / _unitST._unitMaxHP + " " + _unitST.name);
        if (tValue < 0) tValue = 0f;
        
        _UnitSubset._hpList[2].transform.localScale = new Vector3(tValue, 1, 1);

        _timerForHP = 0;
        if (_unitST.isPlayer)
            UIManager.OnUpdateHPBar?.Invoke(_unitST);
    }

    public void UpdateHPBar(bool loseHP=false)
    {
        if (_unitST == null) return; 
        float tValue = _unitST._unitHP / _unitST._unitMaxHP;
        _UnitSubset._hpList[2].transform.localScale = new Vector3(tValue, 1, 1);
        //if (_unitST.isPlayer)
        //    UIManager.OnUpdateHPBar?.Invoke(_unitST);
    }

    public void ShowDmgText(SA_Unit.AttackType type, float value)
    {
        SA_ResourceManager.Instance.GetDamageNumber(type).Spawn(transform.position + _dmgPopupOffset, value);
        //SA_ResourceManager.Instance._normalDamagePrefab.Spawn(transform.position + _dmgPopupOffset, value);
    }

    public void TurnOffHPBar(float sec)
    {
        Invoke("TTurnOffHPBar", sec);
    }

    private void TTurnOffHPBar()
    {
        _UnitSubset._hpList[0].gameObject.SetActive(false);
    }

    void SetUnitType()
    {
        if (_unitType == UnitType.none) return;
        if (_unitType != UnitType.Object)
        {
            bool check = false;

            if (_unitST == null) check = true;
            if (_UnitSubset == null) check = true;
            if (_unitST._spumPrefab == null) check = true;
            if (_rigidBody == null) check = true;
            if (_collider == null) check = true;
            Debug.Log(_unitST == null);
            Debug.Log(_UnitSubset == null);
            Debug.Log(_unitST._spumPrefab == null);
            Debug.Log(_rigidBody == null);
            Debug.Log(_collider == null);
            if (check) UnitTypeProcess();

            switch(_unitType)
            {
                case UnitType.Player:
                    gameObject.tag = "Player";
                    _unitST._spumPrefab._anim.transform.localScale = new Vector3(1, 1, 1);
                    break;

                case UnitType.Enemy:
                    gameObject.tag = "Enemy";
                    
                    //_unitST._spumPrefab._anim.transform.localScale = new Vector3(1, 1, 1);
                    break;

                case UnitType.Object:
                    gameObject.tag = "Object";
                    break;
                case UnitType.none:
                    break;
            }
        }
    }

    public void AddHPBar()
    {
        if (_UnitSubset == null)
        {
            _UnitSubset = Instantiate(SA_ResourceManager.Instance._hpBarWithLevel).GetComponent<SA_UnitSubset>();
            _UnitSubset.gameObject.name = "SA_UnitSubset";
            _UnitSubset.transform.SetParent(transform);
            _UnitSubset.transform.localScale = Vector3.one;
            _UnitSubset.transform.localPosition = SA_ResourceManager.Instance._hpBarPos;
            _UnitSubset._levelText.text = _unitST._level.ToString();
        } else
        { // if hpbar already exists
            

        }
    }
    public void UnitTypeProcess()
    {
        if (!isPlayer)
        {
            _unitST = GetComponent<SA_Unit>();
            if (_unitST != null) DestroyImmediate(_unitST);
            _unitST = GetComponent<SA_Unit>();
            if (_unitST != null) DestroyImmediate(_unitST);
            _unitST = gameObject.AddComponent<SA_Unit>();
            _unitST._spumPrefab = GetComponent<SPUM_Prefabs>();
            _unitST._UnitSet = this;
        } else
        {
            
            DestroyImmediate(GetComponent<SA_Player>());
            DestroyImmediate(GetComponent<SA_Player>());
            _unitST = gameObject.AddComponent<SA_Player>();
            _unitST.isPlayer = true;
            _unitST._spumPrefab = GetComponent<SPUM_Prefabs>();
            _unitST._UnitSet = this;
        }

        //_unitST._mStatContainer = SA_ResourceManager.Instance.GetCharacterStat();
        //UnitInitSet();
        

        SA_AnimationAction tSA = _unitST._spumPrefab._anim.gameObject.GetComponent<SA_AnimationAction>();
        if (tSA != null) DestroyImmediate(tSA);
        tSA = _unitST._spumPrefab._anim.gameObject.GetComponent<SA_AnimationAction>();
        if (tSA != null) DestroyImmediate(tSA);
        
        tSA = _unitST._spumPrefab._anim.gameObject.AddComponent<SA_AnimationAction>();
        tSA._player = _unitST;


        _rigidBody = GetComponent<Rigidbody2D>();
        if (_rigidBody != null) DestroyImmediate(_rigidBody);
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
        _collider = GetComponent<CapsuleCollider2D>();
        if (_collider != null) DestroyImmediate(_collider);

        _collider = gameObject.AddComponent<CapsuleCollider2D>();
        _collider.offset = new Vector2(0, 0.25f);
        _collider.size = new Vector2(0.5f, 0.5f);

        _dmgPopupOffset = new Vector3(0f, 1.3f, 0f);

        for (int i = 0; i < transform.childCount; i++)
        {
            string tName = transform.GetChild(i).name;
            if (tName == "SA_UnitSubset")
            {
                DestroyImmediate(transform.GetChild(i));
            }
        }

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
        
        
        //_UnitSubset = Instantiate(SA_ResourceManager.Instance._hpBarWithLevel).GetComponent<SA_UnitSubset>();
        //_UnitSubset.gameObject.name = "SA_UnitSubset";
        //_UnitSubset.transform.SetParent(transform);
        //_UnitSubset.transform.localScale = Vector3.one;
        //_UnitSubset.transform.localPosition = new Vector3(-.7f, .7f);
        //_UnitSubset._levelText.text = "1";
        

        

        _unitST.enabled = true;
    }

    public void SetUnitToBattle()
    {
        //if (_unitST._ms == null) return;

        Debug.Log("InitMonster");
        Vector3 mPos = new Vector3((int)transform.position.x, (int)transform.position.y, transform.position.y * 0.1f);
        transform.localPosition = mPos;
        
        AddHPBar();
        

    }


    //void UnitInitSet()
    //{
    //    _unitST.InitStat();

    //    //_unitST._unitHP = 100f;
    //    //_unitST._unitMaxHP = _unitST._unitHP;
    //    //_unitST._unitMoveSpeed = 1f;
    //    //_unitST._unitAttack = 10f;
    //    //_unitST._unitAttackSpeed = 1f;
    //    //_unitST._unitHPRegen = 1 / _unitST._unitHP;

        
    //}
}
