using System;
using _01.Member.KMJ._00.Core._01.Entity._02.EntityCompo;
using _01.Member.KMJ._02.Scripts._01.Player;
using _01.Member.KMJ._02.Scripts._02.System._01.BloodFlower;
using Code.Core._02.Sound;
using Code.Core.GameEvent;
using Code.Entities;
using Code.Interfaces;
using GameEvents;
using UnityEngine;


public class BarrierCompo : MonoBehaviour, IEntityComponent
{
    private Player _player;
    [field: SerializeField] public Collider sheldCollider;

    [SerializeField] private float minusAmount;
    [SerializeField] private float plusAmount;

    [SerializeField] private EntityAnimator _animatorComponent;
    [SerializeField] private BloodFlowerSystem _bloodFlowerSystem;
    [SerializeField] private LayerMask whatIsArrow;

    [SerializeField] private GameEventChannelSO _soundChannel;
    [SerializeField] private SoundSO sheldSound;
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
        if(((1 << other.gameObject.layer) & whatIsArrow) != 0)
        {
            var sfxEvt = SoundEvents.PlaySFXEvent.Initializer(transform.position,sheldSound);
            _soundChannel.RaiseEvent(sfxEvt);
            
            _bloodFlowerSystem.AddFlower((int)plusAmount);
            other.gameObject.SetActive(false);
        }
    }

}