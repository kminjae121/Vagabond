using System;
using System.Collections;
using Code.Core.Debugs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

        private void Awake()
        {
        }

        private void Update()
        {
            if (isAimmed)
            {
                OnAimmedThis?.Invoke();
                aimmingTime += Time.deltaTime;
                
                if (aimmingTime >= maxAimmingTime)
                {
                    //uiImage.color = Color.red;
                    aimmingTime = maxAimmingTime;
                    isTarget = true;
                }
            }
        }
        
        

        public void AimmingThis()
        {
            isAimmed = true;
        }

        public void StartCoroutineInScript()
        {
            if (aimmingFalseCoroutine == null)
            {
                aimmingFalseCoroutine = StartCoroutine(AimmingFalse());
            }
        }
        

        public IEnumerator AimmingFalse()
        {
            yield return new WaitForSeconds(0.35f);

            isAimmed = false;
            aimmingTime = 0;
            isTarget = false;

            aimmingFalseCoroutine = null;
        }
        
    }
}