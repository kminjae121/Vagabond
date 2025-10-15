using UnityEngine;

namespace _01.Member.KDH._01.Scripts.Config
{
    /// <summary>
    /// Bhop (Bunny Hop) 물리 설정을 관리하는 ScriptableObject
    /// 이동, 점프, 스트레이프 점핑 등의 모든 물리 파라미터를 한 곳에서 관리
    /// </summary>
    [CreateAssetMenu(fileName = "BhopPhysicsConfig", menuName = "Config/Physics/Bhop Physics Config")]
    public class BhopPhysicsConfig : ScriptableObject
    {
        [Header("Ground Movement - 지면 이동 설정")]
        [Tooltip("지면에서 가속 속도 (높을수록 빠르게 최대 속도에 도달)")]
        public float groundAcceleration = 14f;
        
        [Tooltip("지면에서의 최대 이동 속도")]
        public float maxGroundSpeed = 8f;
        
        [Tooltip("지면 마찰력 (높을수록 빠르게 감속)")]
        public float friction = 6f;
        
        [Tooltip("마찰이 적용되기 시작하는 최소 속도")]
        public float stopSpeed = 1.5f;
        
        [Header("Air Movement - 공중 이동 설정")]
        [Tooltip("공중에서의 가속도 (낮을수록 정밀한 제어 가능)")]
        public float airAcceleration = 2f;
        
        [Tooltip("공중에서의 최대 속도 배율")]
        public float maxAirSpeed = 0.8f;
        
        [Header("Strafe Jumping - 스트레이프 점핑 설정")]
        [Tooltip("스트레이프 점핑 기능 활성화")]
        public bool enableStrafeJumping = true;
        
        [Tooltip("스트레이프 시 속도 증가 배율")]
        public float strafeMultiplier = 1.2f;
        
        [Tooltip("스트레이프 점핑으로 도달 가능한 최대 속도")]
        public float maxStrafeSpeed = 20f;
        
        [Header("Bunny Hopping - 번니합 설정")]
        [Tooltip("자동 번니합 활성화 (점프 키를 누르고 있으면 자동으로 연속 점프)")]
        public bool enableAutoBhop = false;
        
        [Tooltip("점프 간 속도 유지율 (0~1, 1에 가까울수록 속도 손실 적음)")]
        [Range(0f, 1f)]
        public float bhopSpeedRetention = 0.9f;
        
        [Header("Jump Settings - 점프 기본 설정")]
        [Tooltip("점프 힘/임펄스 크기")]
        public float jumpForce = 8f;
        
        [Tooltip("커스텀 중력 (Unity 기본 중력을 사용하지 않을 경우)")]
        public float gravity = 20f;
        
        [Tooltip("점프 키를 누르고 있으면 자동으로 점프 (번니합용)")]
        public bool autoBhop = true;
    }
}