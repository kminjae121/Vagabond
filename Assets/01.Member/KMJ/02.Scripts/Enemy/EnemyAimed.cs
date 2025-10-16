using System;
using System.Collections;
using Code.Core.Debugs;
using UnityEngine;
using UnityEngine.Events;

namespace _01.Member.KMJ._02.Scripts.Enemy
{
    public class EnemyAimed : MonoBehaviour
    {
        public bool isAimmed { get; private set; } = false; 
        public bool isTarget { get; private set; } = false;
        public UnityEvent OnAimmedThis;

        private Coroutine aimmingFalseCoroutine;
        

        public float aimmingTime = 0;
        public float maxAimmingTime = 0.6f;

        private void Start()
        {
        }

        private void Update()
        {
            if (isAimmed)
            {
                aimmingTime += Time.deltaTime;
            }

            if (aimmingTime >= maxAimmingTime)
            {
                isTarget = true;
            }
            
            
        }

        public void AimmingThis()
        {
            isAimmed = true;
        }

        public void StartCoroutineInScript()
        {
            if (aimmingFalseCoroutine != null)
            {
                aimmingFalseCoroutine = StartCoroutine(AimmingFalse());
            }
        }
        

        public IEnumerator AimmingFalse()
        {
            yield return new WaitForSeconds(1f);

            UnityLogger.Log("해제됨");
            aimmingTime = 0;
            isTarget = false;
        }
        
    }
}