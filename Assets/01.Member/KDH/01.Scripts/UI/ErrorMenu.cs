using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Member.KDH._02.Scripts.UI
{
    public class ErrorMenu : Panel
    {

        [SerializeField] private TextMeshProUGUI errorText = null;
        [SerializeField] private TextMeshProUGUI buttonText = null;
        [SerializeField] private Button actionButton = null;

        public override void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }
            actionButton.onClick.AddListener(ButtonAction);
            base.Initialize();
        }

        public void Open(string error, string button)
        {
            Open();
            if (string.IsNullOrEmpty(error) == false)
            {
                errorText.text = error;
            }
            if (string.IsNullOrEmpty(button) == false)
            {
                buttonText.text = button;
            }
        }
    
        private void ButtonAction()
        {
            Close();
        }
    
    }
}