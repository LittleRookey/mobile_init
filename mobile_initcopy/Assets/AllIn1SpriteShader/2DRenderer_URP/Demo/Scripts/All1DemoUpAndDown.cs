using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllIn1SpriteShader
{
    public class All1DemoUpAndDown : MonoBehaviour
    {
        [SerializeField] private Vector2 reachRange = new Vector2(0.5f, 4f);
        [SerializeField] private Vector2 speedRange = new Vector2(0.5f, 2f);

        private Vector3 newPosition = Vector3.zero;
        private Vector3 startPos;
        private float range = 3f;
        private float positionOffset = 0f;
        private float rand, speed;

        void Start()
        {
            startPos = transform.position;
            range = Random.Range(reachRange.x, reachRange.y);
            speed = Random.Range(speedRange.x, speedRange.y);
            rand = Random.Range(-10f, 10f);
        }

        void Update()
        {
            positionOffset = Mathf.Sin((Time.time + rand) * speed) * range;
            newPosition.y = positionOffset;
            transform.position = startPos + newPosition;
        }
    }
}