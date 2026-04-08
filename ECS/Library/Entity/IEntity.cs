using System.Collections.Generic;
using ECS.Library.Components;
using ECS.Library.Entity;

namespace ECS.Library.Entity
{
    public interface IEntity
    {
        public int ID {get;}
        void AddComponent<T>() where T : IComponent, new();
        void RemoveComponent<T>() where T : IComponent;
        T GetComponent<T>() where T : IComponent;
    }
}