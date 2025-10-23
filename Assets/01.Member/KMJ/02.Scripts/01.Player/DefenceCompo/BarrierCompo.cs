using System;
using _01.Member.KMJ._02.Scripts._01.Player;
using Code.Entities;
using Code.Interfaces;
using UnityEngine;


public class BarrierCompo : MonoBehaviour, IEntityComponent
{
    private Player _player;
    [field: SerializeField] public Collider sheldCollider;

    [SerializeField] private float minusAmount;
    public void Initialize(Entity entity)
    {
        _player = entity as Player;
    }

    private void Start()
    {
        _player.inputReader.BarrierEvent += StartBarrier;
        _player.inputReader.BarrierEndEvent += EndBarrier;
        sheldCollider.enabled = false;
    }

    private void StartBarrier()
    {
        if (_player.bloodSystemCompo.GetCurrentFlowerCnt() - minusAmount < 0 || !_player.swordCompo.GetIsAttacking())
            return;
        _player.bloodSystemCompo.RemoveFlower(minusAmount);
        _player.ChangeState("SHELD");
    }

    private void EndBarrier()
    {
        _player.ChangeState("IDLE");
    }

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.SetActive(false);
    }

}