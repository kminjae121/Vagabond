using System;
using _00.CORE._02.Scripts.Input;
using Code.Core.Debugs;
using UnityEngine;

namespace Code.Dash
{
    public class PlayerDashComponent : MonoBehaviour
    {
        [field: SerializeField] public float dashDistance { get; set; } = 10;
        public bool isCanDash { get; set; } = true;
        
        [SerializeField] private InputReader _inputReader;
        
        private float dashCoolTime;
        private GameObject _dashTrmObject;

        private void Awake()
        {
            _inputReader.DashEvent += Dash;
        }

        private void Start()
        {
            Vector3 trm = gameObject.transform.position;
            
            trm.z += dashDistance;
            
            _dashTrmObject = Instantiate(new GameObject(), trm, Quaternion.identity);

            _dashTrmObject.name = "dashTrmObject";
            _dashTrmObject.transform.parent = gameObject.transform;
        }

        private void Dash()
        {
            if (!isCanDash)
                return;
            
            transform.position = _dashTrmObject.transform.position;
        }
    }
}