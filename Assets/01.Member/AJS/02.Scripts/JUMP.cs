using UnityEngine;

// ▼▼▼ IDE가 제안한 네임스페이스를 여기에 추가 ▼▼▼
namespace _01.Member.AJS._02.Scripts
{
    // 기존 스크립트 내용은 이 중괄호 안에 그대로 둡니다.
    [RequireComponent(typeof(Rigidbody))]
    public class Jetpack : MonoBehaviour
    {
        [Header("제트팩 설정")]
        [SerializeField]
        private float jetpackForce = 30f;

        [SerializeField]
        private ParticleSystem jetpackEffect;

        private Rigidbody rb;
        private bool isJetpacking = false;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            if (jetpackEffect != null)
            {
                jetpackEffect.Stop();
            }
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                isJetpacking = true;
            }
            else
            {
                isJetpacking = false;
            }
            HandleEffect();
        }

        void FixedUpdate()
        {
            if (isJetpacking)
            {
                rb.AddForce(Vector3.up * jetpackForce, ForceMode.Force);
            }
        }

        void HandleEffect()
        {
            if (jetpackEffect == null) return;

            if (isJetpacking && !jetpackEffect.isPlaying)
            {
                jetpackEffect.Play();
            }
            else if (!isJetpacking && jetpackEffect.isPlaying)
            {
                jetpackEffect.Stop();
            }
        }
    }
} // <-- 네임스페이스 닫는 중괄호