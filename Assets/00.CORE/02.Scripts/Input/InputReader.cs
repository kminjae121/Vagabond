using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _00.CORE._02.Scripts.Input
{
    [CreateAssetMenu(menuName = "SO/Player")]
    public class InputReader : ScriptableObject, Controls.IPlayerActions
    {
        private Controls _controlls;


        public Vector2 MoveValue;

        public Action<bool> SlidingEvent { get; set; }
        
        public Action JumpKeyEvent { get; set; }
        
        public Action ChargingEvent { get; set; }
        
        public Action ChargingAttackEvent { get; set; }
        
        public Action AttackEvent { get; set; }


        private void OnEnable()
        {
            if (_controlls == null)
            {
                _controlls = new Controls();
            }

            _controlls.Player.SetCallbacks(this);
            _controlls.Player.Enable();
        }

        private void OnDestroy()
        {
            _controlls.Player.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveValue = context.ReadValue<Vector2>();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                AttackEvent?.Invoke();
            }
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            
        }

        public void OnCharging(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                ChargingEvent?.Invoke();
            }
            else if (context.canceled)
            {
                ChargingAttackEvent?.Invoke();
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                JumpKeyEvent.Invoke();
            }
            if (context.started)
            {
                SlidingEvent?.Invoke(true);
            }
            else if (context.canceled) 
            {
                SlidingEvent?.Invoke(false);
            }
        }
    }
}