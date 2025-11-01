using Code.Entities;

namespace _01.Member.KMJ._02.Scripts.Test
{
    public class TestEnemy : Entity
    {
        protected override void Awake()
        {
            OnDeathEvent.AddListener(Death);
            base.Awake();
        }

        private void OnDisable()
        {
            OnDeathEvent.RemoveListener(Death);
        }

        private void Death()
        {
            gameObject.SetActive(false);
        }
    }
}