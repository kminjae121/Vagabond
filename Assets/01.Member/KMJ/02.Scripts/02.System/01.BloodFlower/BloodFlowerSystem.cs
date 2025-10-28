using System;
using System.Collections;
using System.Collections.Generic;
using _01.Member.KMJ._02.Scripts._01.Player;
using _01.Member.KMJ._02.Scripts._01.Player.AttackCompo;
using Code.Core._02.Sound;
using Code.Core.Debugs;
using Code.Core.GameEvent;
using GameEvents;
using UnityEngine;
using UnityEngine.Events;

namespace _01.Member.KMJ._02.Scripts._02.System._01.BloodFlower
{
    public class BloodFlowerSystem : MonoBehaviour
    {
        [Header("Player Reference")]
        [SerializeField] private Player _player;
    
        [Header("Flower Settings")]
        [SerializeField] private float _flowerCnt;
        [SerializeField] private List<float> movespeeds;
    
        [Header("Falling Flower Settings")]
        [SerializeField] private float initialFallingFlowerSec = 10f;

        [SerializeField] private BloodFlowerUI _bloodFlowerUI;

        [SerializeField] private List<float> chargingTimeList;

        [SerializeField] private float minusAmount = 0.1f;
        
        [SerializeField] private GameEventChannelSO _soundChannel;
        [SerializeField] private SoundSO ScreamSound;

        private Coroutine _minusflowerCoroutine;
        public UnityEvent _flowerChangeEvent;
        public UnityEvent OnPlayerDeathEvent;

        public UnityAction germinationEvent;
        public UnityAction bloomEvent;
        public UnityAction fullBloomEvent;
        public UnityAction fallingFlowerEvent;

        public float fallingFlowerSec { get; set; }
        public bool isFallingFlower { get; set; } = false;

        private void Awake()
        {   
            _flowerChangeEvent.AddListener(FlowerEvent);
            fallingFlowerSec = initialFallingFlowerSec;
        
            ValidateMoveSpeeds();
            AddFlower(100);
        }

        private void Start()
        {
            _flowerChangeEvent?.Invoke();
            _bloodFlowerUI.SetUIValue(1);

            _minusflowerCoroutine = StartCoroutine(MinusValue());
        }

        private void Update()
        {
            FallingFlower();
        }

        private void MinusFlowerCnt()
        {
            if (_flowerCnt <= 0)
                return;
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
            if (_flowerCnt >= 1000)
            {
                return;
            }
            
            _flowerCnt += amount;
            _bloodFlowerUI.SetUIValue(_flowerCnt);
            _flowerChangeEvent?.Invoke();

            if (_minusflowerCoroutine == null)
            {
                _minusflowerCoroutine = StartCoroutine(MinusValue());
            }
                
            UnityLogger.Log($"[BloodFlowerSystem] 꽃 추가: {_flowerCnt}");
        }

        public void RemoveFlower(float amount)
        {
            _flowerCnt -= amount;
            if (_flowerCnt < 0) _flowerCnt = 0;
            _bloodFlowerUI.SetUIValue(_flowerCnt);
            _flowerChangeEvent?.Invoke();
        }

        public void Scream()
        {
            var sfxEvt = SoundEvents.PlaySFXEvent.Initializer(transform.position,ScreamSound);
            _soundChannel.RaiseEvent(sfxEvt);
        }

        public void GetDamage(float damage)
        {
            _flowerCnt -= damage;
            _bloodFlowerUI.SetUIValue(_flowerCnt);
            
            Scream();
            if (_flowerCnt < 0)
            {
                _flowerCnt = 0;
                OnPlayerDeathEvent?.Invoke();
            }
        }

        public float GetCurrentFlowerCnt()
        {
            return _flowerCnt;
        }

        public void FlowerEvent()
        {
            switch (_flowerCnt)
            {
                case <= 100:
                    SetNormal();
                    break;
                case <= 200:
                    break;
                case <= 300:
                    break;
                case <= 400:
                    SetGermination();
                    break;
                case <= 500:
                    break;
                case <= 600:
                    SetBloomEvent();
                    break;
                case <= 700:
                    break;
                case <= 800:
                    SetFullBloom();
                    break;
                case <= 900:
                    break;
                default:
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
            _player.atkComponent.SetChargingAttackTime(chargingTimeList[0]);
        }

        private void SetGermination()
        {
            if (_player == null) return;
        
            germinationEvent?.Invoke();
        
            if (_player.movementCompo != null && movespeeds != null && movespeeds.Count > 1)
            {
                _player.movementCompo.SetSpeed(movespeeds[1]);
            }
            _player.atkComponent.SetChargingAttackTime(chargingTimeList[1]);
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
            _player.atkComponent.SetChargingAttackTime(chargingTimeList[2]);
        }

        private void SetFullBloom()
        {
            if (_player == null) return;
        
            fullBloomEvent?.Invoke();
        
            if (_player.movementCompo != null && movespeeds != null && movespeeds.Count > 3)
            {
                _player.movementCompo.SetSpeed(movespeeds[3]);
            }
            _player.atkComponent.SetChargingAttackTime(chargingTimeList[3]);
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
            _player.atkComponent.SetChargingAttackTime(chargingTimeList[4]);
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

        public float GetFlowerCount() => _flowerCnt;
    
        public void ResetFlowerCount()
        {
            _flowerCnt = 0;
            isFallingFlower = false;
            fallingFlowerSec = initialFallingFlowerSec;
            _flowerChangeEvent?.Invoke();
        }

        private IEnumerator MinusValue()
        {
            while (_flowerCnt > 0)
            {
                yield return new WaitForSeconds(0.1f);
                
                RemoveFlower(minusAmount);
            }

            _minusflowerCoroutine = null;
        }
    }
}