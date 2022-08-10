using UnityEngine;
using UnityEngine.Events;

public class BounceDue : MonoBehaviour
{
    public UnityEvent onGroundHitEvent;
    public Transform trnsObject;
    public Transform trnsBody;
    public float gravity = -10;
    public Vector2 groundVelocity;
    public float verticalVelocity;
    private float lastVerticalVelocity;
    public bool isGrounded;
    private float randomYDrop;
    float firstYPos;

    bool moveToPlayer;
    bool isCalled;

    SA_Player player;

    CircleCollider2D circleCollider;
    DropItem dropItem;
    string goldName = "��ȭ";
    public UnityAction<DropItem> OnPlayerHit;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        dropItem = GetComponent<DropItem>();
    }

    private void Start()
    {
        player = StatManager.Instance._player;
        
    }
    void OnEnable()
    {
        OnPlayerHit += WhenReachPlayer;
        circleCollider.enabled = false;
        moveToPlayer = false;
        isCalled = false;
        randomYDrop = Random.Range(-3f, 3f);
        firstYPos = transform.position.y;
        Set(Vector3.right * Random.Range(-1, 1) * Random.Range(1f, 2f), Random.Range(4f, 5f));
        Invoke("MoveToPlayer", 3f);
    }

    private void OnDisable()
    {
        OnPlayerHit -= WhenReachPlayer;
    }

    void Update()
    {
        if (moveToPlayer)
        {
            Actions.OnCoinDrop?.Invoke(this);
            moveToPlayer = false;
            isCalled = true;
            return;
        }
        if (isCalled) return;
        UPosition();
        CheckGroundHit();
        
    }

    void WhenReachPlayer(DropItem dp)
    {
        // handles gold
        if (dp.itemName == goldName)
        {
            UIManager.OnUpdateGold?.Invoke(dp.itemCount);
            return;
        }

        Litkey.InventorySystem.Inventory.AddToInventory?.Invoke(dropItem);
    }
    public void MoveToPlayer()
    {
        
        moveToPlayer = true;
        circleCollider.enabled = true;
    }

    public void Set(Vector2 groundVelocity, float verticalVelocity)
    {
        isGrounded = false;
        this.groundVelocity = groundVelocity;
        this.verticalVelocity = verticalVelocity;
        lastVerticalVelocity = verticalVelocity;

    }
    public void UPosition()
    {
        if (!isGrounded)
        {
            verticalVelocity += gravity * Time.deltaTime;
            trnsBody.position += new Vector3(0, verticalVelocity, 0) * Time.deltaTime;
        }
        trnsObject.position += (Vector3)groundVelocity * Time.deltaTime;

    }
    void CheckGroundHit()
    {
        if (trnsBody.position.y < firstYPos - randomYDrop && !isGrounded)
        {
            trnsBody.position = new Vector2(trnsObject.position.x, firstYPos - randomYDrop);
            isGrounded = true;
            GroundHit();
        }
    }
    void GroundHit()
    {
        onGroundHitEvent.Invoke();
    }
    public void Bounce(float division)
    {
        Set(groundVelocity, lastVerticalVelocity / division);
    }
    public void SlowDownVelocity(float division)
    {
        groundVelocity = groundVelocity / division;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Add to player inventory
            OnPlayerHit?.Invoke(dropItem);
            PoolManager.ReleaseObject(gameObject);
        }
        //if((player.transform.position - bd.transform.position).sqrMagnitude <= 0.1f)
        //    PoolManager.ReleaseObject(bd.gameObject);
    }
}