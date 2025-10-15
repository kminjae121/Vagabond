 using System;
 using _01.Member.KMJ._02.Scripts._01.Player;
 using Code.Core.Debugs;
 using Code.Entities;
 using Code.Interfaces;
 using UnityEngine;

public class WallClimbingCompo : MonoBehaviour, IEntityComponent
{
    [SerializeField] private Vector3 _climingSize;
    [SerializeField] private Vector3 _detectedOutSize;

    [SerializeField] private Transform _detectedTrm;
        
    [SerializeField] private Transform _endTrm;
    
    [SerializeField] private LayerMask _detectedLayer;

    [SerializeField] private float speed = 5;

    private Rigidbody _rbComponent;

    private float _waitTime = 0.06f;

    private float _currentTime = 0;
    
    private Player _player;
    
    public void Initialize(Entity entity)
    {
        _player = entity as Player;
        _rbComponent = entity.GetComponent<Rigidbody>();
    }

    public bool CanClimbWall()
    {
        Collider[] hits = Physics.OverlapBox(_detectedTrm.position, _climingSize, Quaternion.identity, _detectedLayer);

        if (hits.Length > 0)
        {
            return true;
        }
        else return false;
    }

    public void ClimingWall()
    {
        Collider[] hits = Physics.OverlapBox(_detectedTrm.position, _climingSize, Quaternion.identity, _detectedLayer);
        
        
        if (hits.Length > 0)
        {
            _player.movementCompo.StopMoving();
            _rbComponent.useGravity = false;
            _player.ChangeState("CLIMBWALL");       
        }
    }

    public void Climbing()
    {
        Collider[] hits = Physics.OverlapBox(_endTrm.position, _detectedOutSize, Quaternion.identity, _detectedLayer);

        UnityLogger.Log(hits.Length);
        
        if (hits.Length != 0)
        {
            _currentTime = 0;
            _rbComponent.linearVelocity = transform.up * speed; 
        }
        else
        {
            _player.movementCompo.StopMoving();
            _currentTime += Time.fixedDeltaTime;

            if (_currentTime >= _waitTime)
            {
                _rbComponent.useGravity = true;
                _rbComponent.AddForce(transform.up * speed / 2, ForceMode.Impulse);
                _rbComponent.AddForce(transform.forward * speed * 1.5f, ForceMode.Impulse);
                _player.ChangeState("IDLE");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(_detectedTrm.position, _climingSize);
        Gizmos.DrawWireCube(_endTrm.position, _detectedOutSize);
        
        Gizmos.color = Color.red;
        Gizmos.color = Color.white;
    }
}
