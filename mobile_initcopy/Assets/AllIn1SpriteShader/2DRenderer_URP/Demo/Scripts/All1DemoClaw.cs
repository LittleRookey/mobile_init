using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllIn1SpriteShader
{
    public class All1DemoClaw : MonoBehaviour
    {
        [SerializeField] private Transform claw1, claw2;
        [SerializeField] private float startAngle = 30f;
        [SerializeField] private Vector2 speedRange = new Vector2(3f, 7f);
        [SerializeField] private Vector2 maxRange = new Vector2(15f, 35f);

        private Vector3 eulerClaw1, eulerClaw2;
        private float rotationAmount = 0;
        private float rand, range, speed;

        void Start()
        {
            speed = Random.Range(speedRange.x, speedRange.y);
            range = Random.Range(maxRange.x, maxRange.y);
            rand = Random.Range(-10f, 10f);
        }

        void Update()
        {
            rotationAmount = Mathf.Sin((Time.time + rand) * speed) * range;
            eulerClaw1.z = -startAngle + rotationAmount;
            eulerClaw2.z = startAngle - rotationAmount;
            claw1.rotation = Quaternion.Euler(eulerClaw1);
            claw2.rotation = Quaternion.Euler(eulerClaw2);
        }
    }
}