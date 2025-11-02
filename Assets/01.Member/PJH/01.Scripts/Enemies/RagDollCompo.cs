using System.Collections.Generic;
using System.Linq;
using Code.Entities;
using Code.Interfaces;
using UnityEngine;

namespace Code.Enemies
{
    public class RagDollCompo : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private Transform ragDollParentTrm;
        [SerializeField] private LayerMask bodyLayer;

        private List<RagDollPart> _partList;
        private Collider[] _results;

        private RagDollPart _defaultPart;
        
        public void Initialize(Entity entity)
        {
            _results = new Collider[1];
            _partList = ragDollParentTrm.GetComponentsInChildren<RagDollPart>().ToList();

            foreach (var part in _partList)
                part.InitializePart();
            
            Debug.Assert(_partList.Count > 0, $"{ragDollParentTrm.name}에 레그돌 파트가 없습니다.");
            _defaultPart = _partList[0];
            
            SetRagDollActive(false);
            SetColliderActive(false);
        }
        
        public void SetRagDollActive(bool isActive)
        {
            if (isActive)
            {
                ResetRagDollVelocity();
            }
            
            _partList.ForEach(part => part.SetRagDollActive(isActive));
        }
        
        public void SetColliderActive(bool isActive)
        {
            _partList.ForEach(part => part.SetCollider(isActive));
        }

        public void ResetRagDollVelocity()
        {
            foreach (var part in _partList)
            {
                var rigidBody = part.GetComponent<Rigidbody>();
                if (rigidBody != null)
                {
                    rigidBody.linearVelocity = Vector3.zero;
                    rigidBody.angularVelocity = Vector3.zero;
                }
            }
        }

        public void AddForceToRagDoll(Vector3 force, Vector3 position)
        {
            int count = Physics.OverlapSphereNonAlloc(position, 0.5f, _results, bodyLayer);
            
            if (count > 0)
                _results[0].GetComponent<RagDollPart>().KnockBack(force, position);
            else
                _defaultPart.KnockBack(force, position);
        }
    }
}