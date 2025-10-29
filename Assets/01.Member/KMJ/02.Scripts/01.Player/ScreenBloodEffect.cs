using System;
using System.Collections;
using Code.Core.Debugs;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace _01.Member.KMJ._02.Scripts._01.Player
{
    public class ScreenBloodEffect : MonoBehaviour
    {
        private bool isActive = false;

        private Coroutine _screenBloodCoroutine = null;

        [SerializeField] private Image screenImage;
        
        
        private float _imageValue;
        private float _imageModifierValue;
        private float _deadModifierValue;

        private float  _ppGValue;
        private float  _ppBValue;
        private float _ppValue;
        private float _ppModifierValue;

        [SerializeField] private float lerpModiferValue = 1;
        [SerializeField] private float deadLerpModiferValue = 1;
        
        [SerializeField] private Volume volume;
        private ColorAdjustments _colorAdjustments;
        private Vignette _vignette;

        private bool _isDead;


        private void Awake()
        {
            Color color = screenImage.color;
            color.a = 0;
            screenImage.color = color;
            
            if (volume.profile.TryGet(out _vignette))
            {
                _vignette.intensity.value = 0;
            }

            if (volume.profile.TryGet(out _colorAdjustments))
            {
                _colorAdjustments.colorFilter.value = Color.white;
            }

            _isDead = false;
        }

        private void Update()
        {
            
            if (isActive)
            {
                LerpValues();
                Color color = screenImage.color;
                color.a = _imageValue;
                screenImage.color = color;
                _vignette.intensity.value = _ppValue;
                
                if (screenImage.color.a <= 0.01f)
                {
                    isActive = false;
                }
            }

            if (_isDead)
            {
                Time.timeScale = 0.3f;
                LerpDead();
                
            }
        }

        public void PlayerDeadTrue()
        {
            _isDead = true;
        }

        public void SetTime()
        {
            _imageModifierValue = 0;
        }

        public void SetScreenPP()
        {
            _ppModifierValue = 0;
        }

        public void LerpValues()
        {
            _imageValue = Mathf.Lerp(_imageValue, _imageModifierValue, Time.deltaTime * lerpModiferValue);
            _ppValue = Mathf.Lerp(_ppValue, _ppModifierValue, Time.deltaTime * lerpModiferValue);
        }

        public void LerpDead()
        {
            Color currentColor = _colorAdjustments.colorFilter.value;
            
            float r = currentColor.r;
            
            _ppGValue = Mathf.Lerp(currentColor.g, _deadModifierValue, Time.deltaTime * deadLerpModiferValue);
            _ppBValue = Mathf.Lerp(currentColor.b, _deadModifierValue, Time.deltaTime * deadLerpModiferValue);
            
            _colorAdjustments.colorFilter.value = new Color(r, _ppGValue, _ppBValue);
        }

        public void StartScreenBlood()
        {
            Color color = screenImage.color;
            color.a = 1;
            screenImage.color = color;
            _vignette.intensity.value = 0.56f;
            _imageValue = 1;
            _ppValue = 0.56f;
            isActive = true;
            SetTime();
            SetScreenPP();  
        }
    }
}