using System;
using _00.CORE._02.Scripts.Input;
using UnityEngine;

namespace Code.Dash
{
    public class PlayerDashComponent : MonoBehaviour
    {
        [field: SerializeField] public float dashDistance { get; set; }

        public bool isCanDash { get; set; } = true;
        private float dashCoolTime;

        [SerializeField] private InputReader _inputReader;

        private void Awake()
        {
            _inputReader.DashEvent += Dash;
        }

        private void Dash()
        {
            if (!isCanDash)
                return;
            
            Vector3 trmPos = transform.position;

            trmPos.z += dashDistance;

            transform.position = trmPos;
        }
    }
}