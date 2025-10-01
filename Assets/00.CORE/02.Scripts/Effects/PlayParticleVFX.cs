using UnityEngine;

namespace Code.Core.Effects
{
    public class PlayParticleVFX : MonoBehaviour, IPlayableVFX
    {
        [field: SerializeField] public string VFXName { get; private set; }
        
        [SerializeField] private bool isOnPosition;
        [SerializeField] private ParticleSystem particle;
        
        public void PlayVFX(Vector3 position, Quaternion rotation)
        {
            if (!isOnPosition)
                transform.SetPositionAndRotation(position, rotation);
            
            particle.Play(true);
        }

        public void StopVFX()
        {
            particle.Stop(true);
        }

        private void OnValidate()
        {
            gameObject.name = VFXName;
        }
    }
}