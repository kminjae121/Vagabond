using _01.Member.KDH._01.Scripts.Config;
using _01.Member.KMJ._02.Scripts._01.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Member.KDH._01.Scripts.Physics
{
    public class BhopMechanicsUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterMovement characterMovement;
        [SerializeField] private BhopMechanicsUIConfig config;
        
        [Header("Coyote Time Indicator")]
        [SerializeField] private Image coyoteTimeBar;
        [SerializeField] private TextMeshProUGUI coyoteTimeText;
        
        [Header("Jump Buffer Indicator")]
        [SerializeField] private Image jumpBufferBar;
        [SerializeField] private TextMeshProUGUI jumpBufferText;
        
        [Header("Speed Display")]
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private Image speedBar;
        
        [Header("Additional Displays")]
        [SerializeField] private TextMeshProUGUI bhopCounterText;
        [SerializeField] private TextMeshProUGUI maxSpeedText;
        
        private CanvasGroup coyoteGroup;
        private CanvasGroup bufferGroup;
        private CanvasGroup speedGroup;
        private float coyoteTargetAlpha;
        private float bufferTargetAlpha;
        
        private int consecutiveBhops = 0;
        private float maxSpeedReached = 0f;
        private bool wasGrounded = true;
        
        private void Awake()
        {
            SetupCanvasGroups();
            LoadDefaultConfig();
            ValidateReferences();
        }
        
        private void SetupCanvasGroups()
        {
            if (coyoteTimeBar != null)
            {
                coyoteGroup = GetOrAddCanvasGroup(coyoteTimeBar.transform.parent);
            }
            
            if (jumpBufferBar != null)
            {
                bufferGroup = GetOrAddCanvasGroup(jumpBufferBar.transform.parent);
            }
            
            if (speedText != null)
            {
                speedGroup = GetOrAddCanvasGroup(speedText.transform.parent);
            }
        }
        
        private CanvasGroup GetOrAddCanvasGroup(Transform parent)
        {
            if (parent == null) return null;
            
            CanvasGroup group = parent.GetComponent<CanvasGroup>();
            if (group == null)
            {
                group = parent.gameObject.AddComponent<CanvasGroup>();
            }
            return group;
        }
        
        private void LoadDefaultConfig()
        {
            if (config == null)
            {
                Debug.LogWarning("[BhopMechanicsUI] BhopMechanicsUIConfig가 할당되지 않았습니다. 기본값을 사용합니다.");
            }
        }
        
        private void ValidateReferences()
        {
            if (characterMovement == null)
            {
               // Debug.LogError("[BhopMechanicsUI] CharacterMovement가 할당되지 않았습니다.");
            }
        }
        
        private void Update()
        {
            if (characterMovement == null) return;
            
            if (config == null || config.showCoyoteTimeUI)
            {
                UpdateCoyoteTimeDisplay();
            }
            
            if (config == null || config.showJumpBufferUI)
            {
                UpdateJumpBufferDisplay();
            }
            
            if (config == null || config.showSpeedometer)
            {
                UpdateSpeedDisplay();
            }
            
            if (config == null || config.showBhopCounter)
            {
                UpdateBhopCounter();
            }
            
            if (config == null || config.showMaxSpeed)
            {
                UpdateMaxSpeedDisplay();
            }
            
            UpdateFading();
        }
        
        private void UpdateCoyoteTimeDisplay()
        {
            float coyoteTime = characterMovement.GetCoyoteTimeRemaining();
            bool isActive = characterMovement.IsCoyoteTimeActive();
            
            float coyoteDuration = config != null ? config.coyoteTimeDuration : 0.15f;
            
            if (coyoteTimeBar != null)
            {
                coyoteTimeBar.fillAmount = coyoteTime / coyoteDuration;
                coyoteTimeBar.color = isActive ? GetCoyoteActiveColor() : GetCoyoteInactiveColor();
            }
            
            if (coyoteTimeText != null)
            {
                string textFormat = config != null ? config.coyoteTimeTextFormat : "{0:F2}s";
                
                if (isActive)
                {
                    coyoteTimeText.text = string.Format(textFormat, coyoteTime);
                    coyoteTimeText.color = GetCoyoteActiveColor();
                }
                else
                {
                    coyoteTimeText.text = config != null ? config.coyoteReadyText : "Ready";
                    coyoteTimeText.color = GetCoyoteInactiveColor();
                }
            }
            
            bool showOnlyActive = config != null ? config.showOnlyWhenActive : true;
            coyoteTargetAlpha = (showOnlyActive && !isActive) ? 0f : 1f;
        }
        
        private void UpdateJumpBufferDisplay()
        {
            float bufferTime = characterMovement.GetJumpBufferRemaining();
            bool isActive = characterMovement.IsJumpBufferActive();
            
            float bufferDuration = config != null ? config.jumpBufferDuration : 0.2f;
            
            if (jumpBufferBar != null)
            {
                jumpBufferBar.fillAmount = bufferTime / bufferDuration;
                jumpBufferBar.color = isActive ? GetBufferActiveColor() : GetBufferInactiveColor();
            }
            
            if (jumpBufferText != null)
            {
                string textFormat = config != null ? config.jumpBufferTextFormat : "{0:F2}s";
                
                if (isActive)
                {
                    jumpBufferText.text = string.Format(textFormat, bufferTime);
                    jumpBufferText.color = GetBufferActiveColor();
                }
                else
                {
                    jumpBufferText.text = config != null ? config.bufferReadyText : "Ready";
                    jumpBufferText.color = GetBufferInactiveColor();
                }
            }
            
            bool showOnlyActive = config != null ? config.showOnlyWhenActive : true;
            bufferTargetAlpha = (showOnlyActive && !isActive) ? 0f : 1f;
        }
        
        private void UpdateSpeedDisplay()
        {
            float speed = characterMovement.GetHorizontalSpeed();
            float maxSpeed = characterMovement.maxmoveSpeed;
            
            if (speedText != null)
            {
                string format = config != null ? config.speedometerFormat : "{0:F1} / {1:F1}";
                string unit = config != null ? config.speedUnit : "m/s";
                
                speedText.text = string.Format(format, speed, maxSpeed) + " " + unit;
                speedText.color = GetSpeedColor(speed, maxSpeed);
            }
            
            if (speedBar != null)
            {
                speedBar.fillAmount = Mathf.Clamp01(speed / maxSpeed);
                speedBar.color = GetSpeedColor(speed, maxSpeed);
            }
            
            if (speed > maxSpeedReached)
            {
                maxSpeedReached = speed;
            }
        }
        
        private void UpdateBhopCounter()
        {
            if (bhopCounterText == null) return;
            
            bool isGrounded = characterMovement.isGrounded;
            
            if (!wasGrounded && isGrounded)
            {
                float speed = characterMovement.GetHorizontalSpeed();
                float minBhopSpeed = config != null ? config.minBhopSpeed : 10f;
                
                if (speed > minBhopSpeed)
                {
                    consecutiveBhops++;
                }
                else
                {
                    consecutiveBhops = 0;
                }
            }
            
            wasGrounded = isGrounded;
            
            string format = config != null ? config.bhopCounterFormat : "{0}";
            bhopCounterText.text = string.Format(format, consecutiveBhops);
            
            if (consecutiveBhops >= 10)
            {
                bhopCounterText.color = config != null ? config.bhopMasterColor : Color.red;
            }
            else if (consecutiveBhops >= 5)
            {
                bhopCounterText.color = config != null ? config.bhopExpertColor : Color.yellow;
            }
            else
            {
                bhopCounterText.color = config != null ? config.bhopNormalColor : Color.white;
            }
        }
        
        private void UpdateMaxSpeedDisplay()
        {
            if (maxSpeedText == null) return;
            
            string format = config != null ? config.maxSpeedFormat : "{0:F1}";
            string unit = config != null ? config.speedUnit : "m/s";
            
            maxSpeedText.text = string.Format(format, maxSpeedReached) + " " + unit;
            maxSpeedText.color = config != null ? config.maxSpeedColor : Color.cyan;
        }
        
        private Color GetSpeedColor(float speed, float maxSpeed)
        {
            if (config == null)
            {
                float speedPercent = speed / maxSpeed;
                if (speedPercent > 1.2f) return Color.red;
                if (speedPercent > 0.8f) return Color.yellow;
                return Color.white;
            }
            
            float percent = speed / maxSpeed;
            
            if (percent > config.overSpeedThreshold)
                return config.overSpeedColor;
            if (percent > config.highSpeedThreshold)
                return config.highSpeedColor;
            if (percent > config.mediumSpeedThreshold)
                return config.mediumSpeedColor;
            
            return config.normalSpeedColor;
        }
        
        private Color GetCoyoteActiveColor()
        {
            return config != null ? config.coyoteActiveColor : Color.cyan;
        }
        
        private Color GetCoyoteInactiveColor()
        {
            return config != null ? config.coyoteInactiveColor : Color.gray;
        }
        
        private Color GetBufferActiveColor()
        {
            return config != null ? config.bufferActiveColor : Color.magenta;
        }
        
        private Color GetBufferInactiveColor()
        {
            return config != null ? config.bufferInactiveColor : Color.gray;
        }
        
        private void UpdateFading()
        {
            float fadeSpeed = config != null ? config.fadeSpeed : 5f;
            
            if (coyoteGroup != null)
            {
                coyoteGroup.alpha = Mathf.Lerp(
                    coyoteGroup.alpha, 
                    coyoteTargetAlpha, 
                    Time.deltaTime * fadeSpeed
                );
            }
            
            if (bufferGroup != null)
            {
                bufferGroup.alpha = Mathf.Lerp(
                    bufferGroup.alpha, 
                    bufferTargetAlpha, 
                    Time.deltaTime * fadeSpeed
                );
            }
        }
        
        public void ResetMaxSpeed()
        {
            maxSpeedReached = 0f;
        }
        
        public void ResetBhopCounter()
        {
            consecutiveBhops = 0;
        }
        
        public void ResetAllStats()
        {
            ResetMaxSpeed();
            ResetBhopCounter();
        }
        
        public void SetConfig(BhopMechanicsUIConfig newConfig)
        {
            config = newConfig;
        }
        
        public int GetConsecutiveBhops() => consecutiveBhops;
        
        public float GetMaxSpeedReached() => maxSpeedReached;
    }
}