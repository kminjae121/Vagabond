using System;
using _01.Member.KMJ._00.Core._01.Entity._02.EntityCompo;
using _01.Member.KMJ._02.Scripts._01.Player;
using Code.Entities;
using Code.Interfaces;
using UnityEngine;


public class BarrierCompo : MonoBehaviour, IEntityComponent
{
    private Player _player;
    [field: SerializeField] public Collider sheldCollider;

    [SerializeField] private float minusAmount;

    [SerializeField] private EntityAnimator _animatorComponent;
    public void Initialize(Entity entity)
    {
        _player = entity as Player;
    }

    private void Start()
    {
        _player.inputReader.BarrierEvent += StartBarrier;
        _player.inputReader.BarrierEndEvent += EndBarrierOnClickUp;
        sheldCollider.enabled = false;
    }

    private void StartBarrier()
    {
        if (_player.bloodSystemCompo.GetCurrentFlowerCnt() - minusAmount < 0)
            return;
        _player.bloodSystemCompo.RemoveFlower(minusAmount);
        _animatorComponent.SetAllBoolParamFalse();
        _animatorComponent.animator.SetBool("SHELD", true);
        Debug.Log("왜 지랄");
        _player.ChangeState("SHELD");
    }

    public void EndBarrier()
    {
        _animatorComponent.SetAllBoolParamFalse();
        _animatorComponent.animator.SetBool("SWORD_IDLE", true);
    }

    public void EndBarrierOnClickUp()
    {
        _animatorComponent.SetAllBoolParamFalse();
        _animatorComponent.animator.SetBool("SWORD_IDLE", true);
        _player.ChangeState("IDLE");
    }

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.SetActive(false);
    }

}