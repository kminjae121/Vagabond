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
        
        // Stats tracking
        private int consecutiveBhops = 0;
        private float maxSpeedReached = 0f;
        private bool wasGrounded = true;
        
        private void Awake()
        {
            SetupCanvasGroups();
            LoadDefaultConfig();
        }
        
        private void SetupCanvasGroups()
        {
            // Setup canvas groups for fading
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
                Debug.LogWarning("BhopMechanicsUIConfig가 할당되지 않았습니다. 기본값을 사용합니다.");
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
        
        /// <summary>
        /// Coyote Time 시각적 표시 업데이트
        /// </summary>
        private void UpdateCoyoteTimeDisplay()
        {
            float coyoteTime = characterMovement.GetCoyoteTimeRemaining();
            bool isActive = characterMovement.IsCoyoteTimeActive();
            
            float coyoteDuration = config != null ? config.coyoteTimeDuration : 0.15f;
            
            // Update bar fill
            if (coyoteTimeBar != null)
            {
                coyoteTimeBar.fillAmount = coyoteTime / coyoteDuration;
                coyoteTimeBar.color = isActive ? GetCoyoteActiveColor() : GetCoyoteInactiveColor();
            }
            
            // Update text
            if (coyoteTimeText != null)
            {
                string textFormat = config != null ? config.coyoteTimeTextFormat : "Coyote: {0:F2}s";
                
                if (isActive)
                {
                    coyoteTimeText.text = string.Format(textFormat, coyoteTime);
                    coyoteTimeText.color = GetCoyoteActiveColor();
                }
                else
                {
                    coyoteTimeText.text = config != null ? config.coyoteReadyText : "Coyote: Ready";
                    coyoteTimeText.color = GetCoyoteInactiveColor();
                }
            }
            
            // Set target alpha for fading
            bool showOnlyActive = config != null ? config.showOnlyWhenActive : true;
            coyoteTargetAlpha = (showOnlyActive && !isActive) ? 0f : 1f;
        }
        
        /// <summary>
        /// Jump Buffer 시각적 표시 업데이트
        /// </summary>
        private void UpdateJumpBufferDisplay()
        {
            float bufferTime = characterMovement.GetJumpBufferRemaining();
            bool isActive = characterMovement.IsJumpBufferActive();
            
            float bufferDuration = config != null ? config.jumpBufferDuration : 0.2f;
            
            // Update bar fill
            if (jumpBufferBar != null)
            {
                jumpBufferBar.fillAmount = bufferTime / bufferDuration;
                jumpBufferBar.color = isActive ? GetBufferActiveColor() : GetBufferInactiveColor();
            }
            
            // Update text
            if (jumpBufferText != null)
            {
                string textFormat = config != null ? config.jumpBufferTextFormat : "Buffer: {0:F2}s";
                
                if (isActive)
                {
                    jumpBufferText.text = string.Format(textFormat, bufferTime);
                    jumpBufferText.color = GetBufferActiveColor();
                }
                else
                {
                    jumpBufferText.text = config != null ? config.bufferReadyText : "Buffer: Ready";
                    jumpBufferText.color = GetBufferInactiveColor();
                }
            }
            
            // Set target alpha for fading
            bool showOnlyActive = config != null ? config.showOnlyWhenActive : true;
            bufferTargetAlpha = (showOnlyActive && !isActive) ? 0f : 1f;
        }
        
        /// <summary>
        /// 속도계 업데이트
        /// </summary>
        private void UpdateSpeedDisplay()
        {
            float speed = characterMovement.GetHorizontalSpeed();
            float maxSpeed = characterMovement.maxmoveSpeed;
            
            // Update text
            if (speedText != null)
            {
                string format = config != null ? config.speedometerFormat : "Speed: {0:F1} / {1:F1}";
                string unit = config != null ? config.speedUnit : "m/s";
                
                speedText.text = string.Format(format, speed, maxSpeed) + " " + unit;
                speedText.color = GetSpeedColor(speed, maxSpeed);
            }
            
            // Update bar
            if (speedBar != null)
            {
                speedBar.fillAmount = Mathf.Clamp01(speed / maxSpeed);
                speedBar.color = GetSpeedColor(speed, maxSpeed);
            }
            
            // Track max speed
            if (speed > maxSpeedReached)
            {
                maxSpeedReached = speed;
            }
        }
        
        /// <summary>
        /// 연속 Bhop 카운터 업데이트
        /// </summary>
        private void UpdateBhopCounter()
        {
            if (bhopCounterText == null) return;
            
            bool isGrounded = characterMovement.CheckGroundDetected();
            
            // Detect successful bhop
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
            
            // Update display
            string format = config != null ? config.bhopCounterFormat : "Bhops: {0}";
            bhopCounterText.text = string.Format(format, consecutiveBhops);
            
            // Color based on streak
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
        
        /// <summary>
        /// 최고 속도 기록 표시
        /// </summary>
        private void UpdateMaxSpeedDisplay()
        {
            if (maxSpeedText == null) return;
            
            string format = config != null ? config.maxSpeedFormat : "Max: {0:F1}";
            string unit = config != null ? config.speedUnit : "m/s";
            
            maxSpeedText.text = string.Format(format, maxSpeedReached) + " " + unit;
            maxSpeedText.color = config != null ? config.maxSpeedColor : Color.cyan;
        }
        
        /// <summary>
        /// 속도에 따른 색상 반환
        /// </summary>
        private Color GetSpeedColor(float speed, float maxSpeed)
        {
            if (config == null)
            {
                // Default colors
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
        
        /// <summary>
        /// Fade In/Out 애니메이션 처리
        /// </summary>
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
        
        /// <summary>
        /// 최고 속도 기록 초기화 (공개 메서드)
        /// </summary>
        public void ResetMaxSpeed()
        {
            maxSpeedReached = 0f;
        }
        
        /// <summary>
        /// Bhop 카운터 초기화 (공개 메서드)
        /// </summary>
        public void ResetBhopCounter()
        {
            consecutiveBhops = 0;
        }
        
        /// <summary>
        /// 모든 통계 초기화 (공개 메서드)
        /// </summary>
        public void ResetAllStats()
        {
            ResetMaxSpeed();
            ResetBhopCounter();
        }
        
        /// <summary>
        /// Config 런타임 변경 (공개 메서드)
        /// </summary>
        public void SetConfig(BhopMechanicsUIConfig newConfig)
        {
            config = newConfig;
        }
    }
}