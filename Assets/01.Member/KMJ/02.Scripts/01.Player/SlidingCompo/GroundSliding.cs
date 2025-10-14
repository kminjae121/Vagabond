
using System;
using System.Collections;
using System.Collections.Generic;
using Code.Core.Debugs;
using Code.Entities;
using Code.Interfaces;
using TMPro;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.SlidingCompo
{
    public class GroundSliding : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private Transform _eyeTrm;


        [SerializeField] private Transform _slideTrm;
        [SerializeField] private Transform _ownTrm;

        [field: SerializeField] public CapsuleCollider playerCollider { get; set; }
        [field: SerializeField] private float camMoveSpeed { get; set; }

        private Rigidbody _rbCompo;
        public Player _player;
        
        public void Initialize(Entity entity)
        {
            _player = entity as Player;
            _rbCompo = GetComponentInParent<Rigidbody>();
        }

        private void Start()
        {
            _player.inputReader.SlideEvent += Slide;
        }

        private void OnDisable()
        {
            _player.inputReader.SlideEvent -= Slide;
        }


        private void Update()
        {
            if (!_player.isSliding)
            {
             //   ReturnSliding();
            }
        }

        public void Slide()
        {
            if (_player.movementCompo.CheckGroundDetected())
            {
                _player.movementCompo.moveSpeed = _player.movementCompo.maxmoveSpeed + 10;
                _player.ChangeState("SLIDE");
            }
        }
        

        //public void Sliding()
        //{
        //    _eyeTrm.position = Vector3.Lerp(_eyeTrm.position, _slideTrm.position, Time.deltaTime * camMoveSpeed);
        //    
        //    Vector3 forwardDir = transform.forward;
        //    
        //    _rbCompo.linearVelocity = new Vector3(forwardDir.x * _player.movementCompo.moveSpeed,
        //        _rbCompo.linearVelocity.y, forwardDir.z * _player.movementCompo.moveSpeed
        //    );
        //}

        //public void ReturnSliding()
        //{
        //    _eyeTrm.position = Vector3.Lerp(_eyeTrm.position, _ownTrm.position, Time.deltaTime * camMoveSpeed);
        //}
    }
}