using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllIn1SpriteShader
{
    public class All1DemoRandomZ : MonoBehaviour
    {
        [SerializeField] private Vector2 randomZRange = Vector3.zero;

        public void Start()
        {
            transform.Translate(0f, 0f, Random.Range(randomZRange.x, randomZRange.y));
        }
    }
}