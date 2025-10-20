using System;
using System.Collections.Generic;
using _01.Member.KMJ._02.Scripts._01.Player;
using Code.Core.Debugs;
using UnityEngine;
using UnityEngine.Events;

namespace _01.Member.KMJ._02.Scripts._02.System._01.BloodFlower
{
    public class BloodFlowerSystem : MonoBehaviour
    {
        [Header("Player Reference")]
        [SerializeField] private Player _player;
    
        [Header("Flower Settings")]
        [SerializeField] private int _flowerCnt = 1;
        [SerializeField] private List<float> movespeeds;
    
        [Header("Falling Flower Settings")]
        [SerializeField] private float initialFallingFlowerSec = 10f;

        [SerializeField] private BloodFlowerUI _bloodFlowerUI;
    
        public event Action _flowerChangeEvent;

        public UnityAction germinationEvent;
        public UnityAction bloomEvent;
        public UnityAction fullBloomEvent;
        public UnityAction fallingFlowerEvent;

        public float fallingFlowerSec { get; set; }
        public bool isFallingFlower { get; set; } = false;

        private void Awake()
        {
            _flowerChangeEvent += FlowerEvent;
            fallingFlowerSec = initialFallingFlowerSec;
        
            ValidateMoveSpeeds();
        }

        private void Start()
        {
            _flowerChangeEvent?.Invoke();
            _bloodFlowerUI.SetUIValue(1);
        }

        private void Update()
        {
            FallingFlower();
        }

        private void ValidateMoveSpeeds()
        {
            if (movespeeds == null || movespeeds.Count < 5)
            {
                UnityLogger.LogWarning($"[BloodFlowerSystem] movespeeds 리스트가 5개 미만입니다. 현재 개수: {movespeeds?.Count ?? 0}");
            }
        
            if (_player == null)
            {
                UnityLogger.LogError("[BloodFlowerSystem] Player가 할당되지 않았습니다.");
            }
        }

        public void AddFlower(int amount)
        {
            if (_flowerCnt >= 10)
            {
                return;
            }
            
            _flowerCnt += amount;
            _bloodFlowerUI.SetUIValue(_flowerCnt);
            _flowerChangeEvent?.Invoke();
                
            UnityLogger.Log($"[BloodFlowerSystem] 꽃 추가: {_flowerCnt}");
        }

        public void RemoveFlower(int amount)
        {
            _flowerCnt -= amount;
            if (_flowerCnt < 0) _flowerCnt = 0;
            _bloodFlowerUI.SetUIValue(_flowerCnt);
            _flowerChangeEvent?.Invoke();
            UnityLogger.Log($"[BloodFlowerSystem] 꽃 제거: {_flowerCnt}");
        }

        public void FlowerEvent()
        {
            switch (_flowerCnt)
            {
                case 0:
                    SetNormal();
                    break;
                case <= 3:
                    SetGermination();
                    break;
                case <= 6:
                    SetBloomEvent();
                    break;
                case <= 9:
                    SetFullBloom();
                    break;
                case >= 10:
                    SetFallingFlower();
                    break;
            }
        }

        private void SetNormal()
        {
            if (_player == null) return;
        
            if (_player.movementCompo != null && movespeeds != null && movespeeds.Count > 0)
            {
                _player.movementCompo.SetSpeed(movespeeds[0]);
            }
        
            if (_player.movementCompo != null)
            {
                _player.movementCompo.maxJumpCnt = 2;
            }
        }

        private void SetGermination()
        {
            if (_player == null) return;
        
            germinationEvent?.Invoke();
        
            if (_player.movementCompo != null && movespeeds != null && movespeeds.Count > 1)
            {
                _player.movementCompo.SetSpeed(movespeeds[1]);
            }
        }

        private void SetBloomEvent()
        {
            if (_player == null) return;
        
            bloomEvent?.Invoke();
        
            if (_player.movementCompo != null && movespeeds != null && movespeeds.Count > 2)
            {
                _player.movementCompo.SetSpeed(movespeeds[2]);
            }
        
            if (_player.movementCompo != null)
            {
                _player.movementCompo.maxJumpCnt = 3;
            }
        }

        private void SetFullBloom()
        {
            if (_player == null) return;
        
            fullBloomEvent?.Invoke();
        
            if (_player.movementCompo != null && movespeeds != null && movespeeds.Count > 3)
            {
                _player.movementCompo.SetSpeed(movespeeds[3]);
            }
        }

        private void SetFallingFlower()
        {
            if (_player == null) return;
        
            fallingFlowerEvent?.Invoke();
        
            if (_player.movementCompo != null && movespeeds != null && movespeeds.Count > 4)
            {
                _player.movementCompo.SetSpeed(movespeeds[4]);
            }
        
            isFallingFlower = true;
            fallingFlowerSec = initialFallingFlowerSec;
        }

        private void FallingFlower()
        {
            if (!isFallingFlower) return;
        
            fallingFlowerSec -= Time.deltaTime;

            if (fallingFlowerSec <= 0)
            {
                isFallingFlower = false;
                _bloodFlowerUI.SetUIValue(1);
                _flowerCnt = 1;
                fallingFlowerSec = initialFallingFlowerSec;
                _flowerChangeEvent?.Invoke();
            
                UnityLogger.Log("[BloodFlowerSystem] FallingFlower 종료, Germination 상태로 전환");
            }
        }

        public int GetFlowerCount() => _flowerCnt;
    
        public void ResetFlowerCount()
        {
            _flowerCnt = 0;
            isFallingFlower = false;
            fallingFlowerSec = initialFallingFlowerSec;
            _flowerChangeEvent?.Invoke();
        }
    }
}