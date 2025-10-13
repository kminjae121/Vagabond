using System;
using System.Collections;
using System.Collections.Generic;
using _01.Member.KMJ._02.Scripts._01.Player;
using Code.Core.Debugs;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

public class BloodFlowerSystem : MonoBehaviour
{
    private int _flowerCnt = 0;

    [SerializeField] private Player _player;
    
    public event Action _flowerChangeEvent;

    public UnityAction germinationEvent;
    public UnityAction bloomEvent;
    public UnityAction fullBloomEvent;
    public UnityAction fallingFlowerEvent;

    public float fallingFlowerSec { get; set; } = 10f;

    public bool isFallingFlower { get; set; } = false;

    [SerializeField] private List<float> movespeeds;

    private void Awake()
    {
        _flowerChangeEvent += FlowerEvent;
    }

    private void Update()
    {
        FallingFlower();
        print(isFallingFlower);
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
                SetNormal();
                break;
            case <= 3 :
                SetGermination();
                break;
            case <= 6 :
                SetBloomEvent();
                break;
            case <= 9 :
                SetFullBloom();
                break;
            case 10 :
                SetFallingFlower();
                break;
        }
    }

    private void SetNormal()
    {
        SetDash(false);
        _player.movementCompo.maxmoveSpeed = movespeeds[0];
    }

    private void SetGermination()
    {
        germinationEvent?.Invoke();
        SetDash(false);
        _player.movementCompo.maxmoveSpeed = movespeeds[1];
    }

    private void SetBloomEvent()
    {
        bloomEvent?.Invoke();
        SetDash(false);
        _player.movementCompo.maxmoveSpeed = movespeeds[2];
        _player.movementCompo.maxJumpCnt = 3;
    }

    private void SetFullBloom()
    {
        fullBloomEvent?.Invoke();
        SetDash(true);
    }

    private void SetFallingFlower()
    {
        fallingFlowerEvent?.Invoke();
        SetDash(true);
        isFallingFlower = true;
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
