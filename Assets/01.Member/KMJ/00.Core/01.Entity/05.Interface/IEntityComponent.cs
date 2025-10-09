using Code.Entities;
using UnityEngine;

namespace Code.Interfaces
{
    public interface IEntityComponent
    {
        public void Initialize(Entity entity);
    }
}