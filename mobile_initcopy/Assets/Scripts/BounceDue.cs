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

    void OnEnable()
    {
        moveToPlayer = false;
        randomYDrop = Random.Range(-6f, 3f);
        firstYPos = transform.position.y;
        Set(Vector3.right * Random.Range(-1, 2) * Random.Range(1f, 3f), Random.Range(2f, 5f));
        Invoke("MoveToPlayer", 2f);
    }

    void Update()
    {
        if (moveToPlayer)
        {
            Actions.OnCoinDrop?.Invoke(this);
            
            return;
        }
        UPosition();
        CheckGroundHit();
        
    }

    public void MoveToPlayer()
    {
        
        moveToPlayer = true;
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
}