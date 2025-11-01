using System;
using Code.Core.Debugs;
using UnityEngine;
using UnityEngine.Events;

namespace Map
{
    public class Door : MonoBehaviour
    {
        public UnityEvent openEvent;
        public UnityEvent closeEvent;

        private void Start()
        {
            openEvent.AddListener(Open);
            closeEvent.AddListener(Close);
        }


        public void SetDoor(bool isOpen)
        {
            if(isOpen)
                openEvent?.Invoke();
            else if(isOpen == false)
            {
                UnityLogger.Log("뭐함");
                closeEvent?.Invoke();
            }
        }

        private void Close()
        {
            gameObject.SetActive(true);
        }

        private void Open()
        {
            gameObject.SetActive(false);
        }
    }
}