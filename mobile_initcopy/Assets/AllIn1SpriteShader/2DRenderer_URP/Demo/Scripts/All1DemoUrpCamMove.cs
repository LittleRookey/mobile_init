using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllIn1SpriteShader
{
    public class All1DemoUrpCamMove : MonoBehaviour
    {
        [SerializeField] private float speed = 5;
        private Vector2 input = Vector3.zero;
        private Rigidbody2D rb;

        public void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void FixedUpdate()
        {
            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");
            rb.velocity = input * speed * Time.fixedDeltaTime;
        }
    }
}