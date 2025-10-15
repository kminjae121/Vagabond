using System;
using Code.Core.Debugs;
using UnityEngine;
using UnityEngine.Events;

namespace _01.Member.KMJ._02.Scripts.Enemy
{
    public class EnemyAimed : MonoBehaviour
    {
        public bool isAimmed { get; private set; } = false;
        public UnityEvent OnAimmedThis;

        private void Start()
        {
        }

        private void Update()
        {
        }

        public void AimmingThis(bool isAim)
        {
            if(isAim)
                OnAimmedThis?.Invoke();
            
            isAimmed = isAim;
        }
    }
}