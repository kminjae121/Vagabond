using UnityEngine;

namespace _01.Member.KDH._01.Scripts.Config
{
    [CreateAssetMenu(fileName = "BhopMechanicsUIConfig", menuName = "Config/UI/Bhop Mechanics UI Config")]
    public class BhopMechanicsUIConfig : ScriptableObject
    {
        [Header("Display Settings - 표시 설정")]
        [Tooltip("코요테 타임 UI 표시 여부")]
        public bool showCoyoteTimeUI = true;
        
        [Tooltip("점프 버퍼 UI 표시 여부")]
        public bool showJumpBufferUI = true;
        
        [Tooltip("속도계 표시 여부")]
        public bool showSpeedometer = true;
        
        [Tooltip("연속 Bhop 카운터 표시 여부")]
        public bool showBhopCounter = true;
        
        [Tooltip("최고 속도 기록 표시 여부")]
        public bool showMaxSpeed = true;
        
        [Tooltip("활성화된 기능만 표시 (비활성 시 항상 표시)")]
        public bool showOnlyWhenActive = true;
        
        [Header("Coyote Time Colors - 코요테 타임 색상")]
        [Tooltip("코요테 타임 활성 시 색상 (점프 가능한 상태)")]
        public Color coyoteActiveColor = new Color(0f, 1f, 1f, 1f); // Cyan
        
        [Tooltip("코요테 타임 비활성 시 색상 (대기 상태)")]
        public Color coyoteInactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Gray
        
        [Header("Jump Buffer Colors - 점프 버퍼 색상")]
        [Tooltip("점프 버퍼 활성 시 색상 (입력이 버퍼에 저장된 상태)")]
        public Color bufferActiveColor = new Color(1f, 0f, 1f, 1f); // Magenta
        
        [Tooltip("점프 버퍼 비활성 시 색상 (대기 상태)")]
        public Color bufferInactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Gray
        
        [Header("Speed Colors - 속도 색상")]
        [Tooltip("일반 속도 색상")]
        public Color normalSpeedColor = Color.white;
        
        [Tooltip("중간 속도 색상 (최대 속도의 50% 이상)")]
        public Color mediumSpeedColor = new Color(0.5f, 1f, 0.5f, 1f); // Light Green
        
        [Tooltip("높은 속도 색상 (최대 속도의 80% 이상)")]
        public Color highSpeedColor = Color.yellow;
        
        [Tooltip("초과 속도 색상 (최대 속도의 120% 이상, 스트레이프 점핑)")]
        public Color overSpeedColor = Color.red;
        
        [Header("Speed Thresholds - 속도 임계값")]
        [Tooltip("중간 속도로 간주되는 비율 (0~1)")]
        [Range(0f, 1f)]
        public float mediumSpeedThreshold = 0.5f;
        
        [Tooltip("높은 속도로 간주되는 비율 (0~1)")]
        [Range(0f, 1f)]
        public float highSpeedThreshold = 0.8f;
        
        [Tooltip("초과 속도로 간주되는 비율 (1 이상)")]
        [Range(1f, 2f)]
        public float overSpeedThreshold = 1.2f;
        
        [Header("Bhop Counter Colors - Bhop 카운터 색상")]
        [Tooltip("일반 Bhop 카운터 색상 (0~4회)")]
        public Color bhopNormalColor = Color.white;
        
        [Tooltip("숙련 Bhop 카운터 색상 (5~9회)")]
        public Color bhopExpertColor = Color.yellow;
        
        [Tooltip("마스터 Bhop 카운터 색상 (10회 이상)")]
        public Color bhopMasterColor = Color.red;
        
        [Header("Max Speed Display - 최고 속도 표시")]
        [Tooltip("최고 속도 표시 색상")]
        public Color maxSpeedColor = Color.cyan;
        
        [Header("Text Formats - 텍스트 형식")]
        [Tooltip("코요테 타임 텍스트 형식 ({0} = 남은 시간)")]
        public string coyoteTimeTextFormat = "{0:F2}s";
        
        [Tooltip("코요테 타임 준비 상태 텍스트")]
        public string coyoteReadyText = "Ready";
        
        [Tooltip("점프 버퍼 텍스트 형식 ({0} = 남은 시간)")]
        public string jumpBufferTextFormat = "{0:F2}s";
        
        [Tooltip("점프 버퍼 준비 상태 텍스트")]
        public string bufferReadyText = "Ready";
        
        [Tooltip("속도계 텍스트 형식 ({0} = 현재 속도, {1} = 최대 속도)")]
        public string speedometerFormat = "{0:F1} / {1:F1}";
        
        [Tooltip("속도 단위")]
        public string speedUnit = "m/s";
        
        [Tooltip("Bhop 카운터 텍스트 형식 ({0} = 연속 횟수)")]
        public string bhopCounterFormat = "{0}";
        
        [Tooltip("최고 속도 텍스트 형식 ({0} = 최고 속도)")]
        public string maxSpeedFormat = "{0:F1}";
        
        [Header("Animation Settings - 애니메이션 설정")]
        [Tooltip("UI 페이드 인/아웃 속도")]
        [Range(1f, 20f)]
        public float fadeSpeed = 5f;
        
        [Tooltip("바 채우기 애니메이션 속도")]
        [Range(1f, 20f)]
        public float barFillSpeed = 10f;
        
        [Header("Timing References - 타이밍 참조값")]
        [Tooltip("코요테 타임 지속 시간 (CharacterMovement와 동일하게 설정)")]
        public float coyoteTimeDuration = 0.15f;
        
        [Tooltip("점프 버퍼 지속 시간 (CharacterMovement와 동일하게 설정)")]
        public float jumpBufferDuration = 0.2f;
        
        [Header("Bhop Detection - Bhop 감지")]
        [Tooltip("Bhop으로 인정되는 최소 속도")]
        public float minBhopSpeed = 10f;
        
        [Header("Debug Settings - 디버그 설정")]
        [Tooltip("콘솔에 상세 로그 출력")]
        public bool enableDebugLogs = false;
        
        [Tooltip("UI 요소 테두리 표시 (디버그용)")]
        public bool showDebugBorders = false;
        
        [Header("Presets - 프리셋")]
        [Tooltip("미리 정의된 색상 테마 적용")]
        public ColorTheme colorTheme = ColorTheme.Default;
        
        public enum ColorTheme
        {
            Default,    // 기본 (Cyan/Magenta)
            Neon,       // 네온 (밝은 색상)
            Dark,       // 다크 (어두운 색상)
            Minimal,    // 미니멀 (흑백)
            Retro,      // 레트로 (Quake 3 스타일)
            Custom      // 사용자 정의
        }
        
        /// <summary>
        /// 선택한 테마의 색상을 적용
        /// </summary>
        [ContextMenu("Apply Color Theme")]
        public void ApplyColorTheme()
        {
            switch (colorTheme)
            {
                case ColorTheme.Neon:
                    ApplyNeonTheme();
                    break;
                case ColorTheme.Dark:
                    ApplyDarkTheme();
                    break;
                case ColorTheme.Minimal:
                    ApplyMinimalTheme();
                    break;
                case ColorTheme.Retro:
                    ApplyRetroTheme();
                    break;
                case ColorTheme.Default:
                    ApplyDefaultTheme();
                    break;
            }
        }
        
        private void ApplyDefaultTheme()
        {
            coyoteActiveColor = new Color(0f, 1f, 1f, 1f); // Cyan
            coyoteInactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            bufferActiveColor = new Color(1f, 0f, 1f, 1f); // Magenta
            bufferInactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            normalSpeedColor = Color.white;
            highSpeedColor = Color.yellow;
            overSpeedColor = Color.red;
        }
        
        private void ApplyNeonTheme()
        {
            coyoteActiveColor = new Color(0f, 1f, 0.5f, 1f); // Neon Green
            coyoteInactiveColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            bufferActiveColor = new Color(1f, 0f, 0.5f, 1f); // Neon Pink
            bufferInactiveColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            normalSpeedColor = new Color(0.5f, 0.5f, 1f, 1f); // Neon Blue
            highSpeedColor = new Color(1f, 0.5f, 0f, 1f); // Neon Orange
            overSpeedColor = new Color(1f, 0f, 0.5f, 1f); // Neon Pink
        }
        
        private void ApplyDarkTheme()
        {
            coyoteActiveColor = new Color(0.3f, 0.6f, 0.8f, 1f); // Muted Cyan
            coyoteInactiveColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            bufferActiveColor = new Color(0.6f, 0.3f, 0.6f, 1f); // Muted Purple
            bufferInactiveColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            normalSpeedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
            highSpeedColor = new Color(0.8f, 0.7f, 0.3f, 1f);
            overSpeedColor = new Color(0.8f, 0.3f, 0.3f, 1f);
        }
        
        private void ApplyMinimalTheme()
        {
            coyoteActiveColor = Color.white;
            coyoteInactiveColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            bufferActiveColor = Color.white;
            bufferInactiveColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            normalSpeedColor = Color.white;
            highSpeedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            overSpeedColor = Color.white;
        }
        
        private void ApplyRetroTheme()
        {
            // Quake 3 style colors
            coyoteActiveColor = new Color(1f, 0.5f, 0f, 1f); // Orange
            coyoteInactiveColor = new Color(0.3f, 0.15f, 0f, 1f);
            bufferActiveColor = new Color(1f, 0f, 0f, 1f); // Red
            bufferInactiveColor = new Color(0.3f, 0f, 0f, 1f);
            normalSpeedColor = new Color(0f, 1f, 0f, 1f); // Green
            highSpeedColor = new Color(1f, 1f, 0f, 1f); // Yellow
            overSpeedColor = new Color(1f, 0f, 0f, 1f); // Red
        }
    }
}