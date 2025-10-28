using System;
using _01.Member.KMJ._02.Scripts._01.Player;
using Code.Core.Debugs;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts.Test
{
    public class DebuggingTest : MonoBehaviour
    {
        [SerializeField] private ScreenBloodEffect _screenCompo;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                _screenCompo.StartScreenBlood();
            }
        }
    }
}