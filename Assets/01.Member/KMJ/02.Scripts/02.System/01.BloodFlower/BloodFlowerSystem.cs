using System;
using System.Collections;
using _01.Member.KMJ._02.Scripts._01.Player;
using Code.Core.Debug;
using UnityEngine;

public class BloodFlowerSystem : MonoBehaviour
{
    private int _flowerCnt;

    [SerializeField] private Player _player;
    
    public event Action _flowerChangeEvent;
    
    public float fallingFlowerSec { get; set; }

    private bool _isFallingFlower;

    private void Awake()
    {
        _flowerChangeEvent += FlowerEvent;
    }

    private void Update()
    {
        FallingFlower();
    }

    public void AddFlower(int amount)
    {
        _flowerCnt += 1;
        _flowerChangeEvent?.Invoke();
        UnityLogger.Log(_flowerCnt);
    }

    public void RemoveFlower(int amount)
    {
        _flowerCnt -= 1;
        _flowerChangeEvent?.Invoke();
    }

    public void FlowerEvent()
    {
        switch (_flowerCnt)
        {
            case 0:
                _player.movementCompo.maxmoveSpeed = 20;
                break;
            case <= 1 :
                _player.movementCompo.maxmoveSpeed = 23;
                break;
            case <= 6 :
                _player.movementCompo.maxmoveSpeed = 30;
                _player.movementCompo.maxJumpCnt = 3;
                break;
            case <= 9 :
                break;
            case 10 :
                _isFallingFlower = true;
                break;
        }
    }

    private void FallingFlower()
    {
        if (_isFallingFlower == false)
            return;
        
        fallingFlowerSec -= Time.deltaTime;

        if (fallingFlowerSec <= 0)
        {
            _isFallingFlower = true;
            _flowerCnt = 0;
        }
    }
}
