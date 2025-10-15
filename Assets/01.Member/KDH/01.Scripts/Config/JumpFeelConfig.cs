using UnityEngine;

namespace _01.Member.KDH._01.Scripts.Config
{
    /// <summary>
    /// 점프 느낌(Feel)과 조작감을 향상시키는 기능들의 설정
    /// Coyote Time, Jump Buffer 등 플레이어 경험 개선 기능 포함
    /// </summary>
    [CreateAssetMenu(fileName = "JumpFeelConfig", menuName = "Config/Player/Jump Feel Config")]
    public class JumpFeelConfig : ScriptableObject
    {
        [Header("Basic Jump - 기본 점프 설정")]
        [Tooltip("점프 힘/임펄스")]
        public float jumpForce = 8f;
        
        [Tooltip("최대 점프 횟수 (1=단일 점프, 2=더블 점프)")]
        public int maxJumpCount = 2;
        
        [Header("Coyote Time - 코요테 타임")]
        [Tooltip("코요테 타임 활성화 (플랫폼을 떠난 직후에도 점프 가능)")]
        public bool enableCoyoteTime = true;
        
        [Tooltip("플랫폼을 떠난 후 점프 가능한 시간 (초)")]
        [Range(0f, 0.5f)]
        public float coyoteTimeDuration = 0.15f;
        
        [Header("Jump Buffer - 점프 버퍼")]
        [Tooltip("점프 입력 버퍼링 활성화 (착지 전 입력한 점프를 착지 시 자동 실행)")]
        public bool enableJumpBuffer = true;
        
        [Tooltip("착지 전 점프 입력이 버퍼되는 시간 (초)")]
        [Range(0f, 0.5f)]
        public float jumpBufferDuration = 0.2f;
        
        [Header("Variable Jump Height - 가변 점프 높이")]
        [Tooltip("가변 점프 높이 활성화 (점프 키를 일찍 떼면 낮게 점프)")]
        public bool enableVariableJumpHeight = false;
        
        [Tooltip("점프 키를 일찍 뗐을 때 적용되는 중력 배율")]
        [Range(1f, 5f)]
        public float jumpCutMultiplier = 2f;
        
        [Header("Landing - 착지")]
        [Tooltip("점프 간 최소 간격 (연속 점프 방지, 초)")]
        public float minTimeBetweenJumps = 0.1f;
        
        [Tooltip("착지 시 속도 유지율 (번니합용, 0~1)")]
        [Range(0f, 1f)]
        public float landingSpeedRetention = 0.9f;
        
        [Header("Debug - 디버그")]
        [Tooltip("콘솔에 디버그 정보 표시")]
        public bool showDebugInfo = false;
        
        [Tooltip("씬 뷰에 시각적 표시 활성화")]
        public bool showGizmos = true;
    }
}