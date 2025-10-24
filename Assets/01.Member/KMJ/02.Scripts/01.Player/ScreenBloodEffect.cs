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

        private float _ppValue;
        private float _ppModifierValue;

        [SerializeField] private float lerpModiferValue = 1;


        private void Awake()
        {
            Color color = screenImage.color;
            color.a = 0;
            screenImage.color = color;
        }

        private void Update()
        {
            
            if (isActive)
            {
                LerpValues();
                UnityLogger.Log(_imageValue);
                Color color = screenImage.color;
                color.a = _imageValue;
                screenImage.color = color;
                
                if (screenImage.color.a <= 0.01f)
                {
                    isActive = false;
                }
            }
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

        public void StartScreenBlood()
        {
            Color color = screenImage.color;
            color.a = 1;
            screenImage.color = color;
            
            _imageValue = 1;
            _ppValue = 0.65f;
            isActive = true;
            SetTime();
            SetScreenPP();  
        }
    }
}