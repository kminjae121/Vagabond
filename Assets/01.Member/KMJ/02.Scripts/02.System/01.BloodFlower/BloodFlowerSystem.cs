using System;
using System.Collections;
using System.Collections.Generic;
using _01.Member.KMJ._02.Scripts._01.Player;
using Code.Core.Debugs;
using NUnit.Framework;
using UnityEngine;

public class BloodFlowerSystem : MonoBehaviour
{
    private int _flowerCnt;

    [SerializeField] private Player _player;
    
    public event Action _flowerChangeEvent;
    
    public float fallingFlowerSec { get; set; }

    public bool isFallingFlower { get; set; } = true;

    [SerializeField] private List<float> movespeeds;

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
                SetDash(false);
                _player.movementCompo.maxmoveSpeed = movespeeds[0];
                break;
            case <= 3 :
                SetDash(false);
                _player.movementCompo.maxmoveSpeed = movespeeds[1];
                break;
            case <= 6 :
                SetDash(false);
                _player.movementCompo.maxmoveSpeed = movespeeds[2];
                _player.movementCompo.maxJumpCnt = 3;
                break;
            case <= 9 :
                SetDash(true);
                break;
            case 10 :
                SetDash(true);
                isFallingFlower = true;
                break;
        }
    }

    public void SetDash(bool IsCanDash)
    {
        _player.dashComponent.isCanDash = IsCanDash;
    }

    private void FallingFlower()
    {
        if (isFallingFlower == false)
            return;
        
        fallingFlowerSec -= Time.deltaTime;

        if (fallingFlowerSec <= 0)
        {
            isFallingFlower = false;
            _flowerCnt = 1;
        }
    }
}
