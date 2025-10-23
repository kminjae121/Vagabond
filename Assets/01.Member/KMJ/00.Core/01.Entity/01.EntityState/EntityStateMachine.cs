using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Code.Entities;
using UnityEngine;

namespace _01.Member.KMJ._00.Core._01.Entity._01.EntityState
{
    public class EntityStateMachine
    {
        public EntityState CurrentState { get; set; }
        private Dictionary<string, EntityState> _states;

        public EntityStateMachine(Code.Entities.Entity entity, StateDataSO[] stateList)
        {
            _states = new Dictionary<string, EntityState>();
            foreach (StateDataSO state in stateList)
            {
                Type type = FindStateType(state.className);
                
                if (type == null)
                {
                    Debug.LogError($"State 타입을 찾을 수 없습니다: {state.className}");
                    continue;
                }
                
                EntityState entityState = Activator.CreateInstance(type, entity, state.animationHash) as EntityState;
                
                if (entityState == null)
                {
                    Debug.LogError($"State 인스턴스 생성 실패: {state.className}");
                    continue;
                }
                
                _states.Add(state.stateName, entityState);
            }
        }

        private Type FindStateType(string className)
        {
            Type type = Type.GetType(className);
            if (type != null) return type;
            
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                type = assembly.GetTypes().FirstOrDefault(t => 
                    t.Name == className || 
                    t.FullName == className
                );
                
                if (type != null) return type;
            }
            
            return null;
        }

        public void ChangeState(string newStateName, bool forced = false)
        {
            EntityState newState = _states.GetValueOrDefault(newStateName);
            
            if (newState == null)
            {
                Debug.LogError($"State를 찾을 수 없습니다: {newStateName}");
                return;
            }
            
            if (forced == false && CurrentState == newState)
                return;
            
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }

        public void UpdateStateMachine()
        {
            CurrentState?.Update();
        }

        public void FixedUpdateMachine()
        {
            CurrentState?.FixedUpdate();
        }
    }
}