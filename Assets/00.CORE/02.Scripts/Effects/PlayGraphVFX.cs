using UnityEngine;
using UnityEngine.VFX;

namespace Code.Core.Effects
{
    public class PlayGraphVFX : MonoBehaviour, IPlayableVFX
    {
        [field:SerializeField] public string VFXName { get; private set; }
        
        [SerializeField] private bool isOnPosition;
        [SerializeField] private VisualEffect[] effects;
        
        public void PlayVFX(Vector3 position, Quaternion rotation)
        {
            if (!isOnPosition)
                transform.SetPositionAndRotation(position, rotation);
            
            foreach(var effect in effects)
                effect.Play();
        }

        public void StopVFX()
        {
            foreach(var effect in effects)
                effect.Stop();
        }

        private void OnValidate()
        {
            if(!string.IsNullOrEmpty(VFXName))
                gameObject.name = VFXName;
        }
    }
}