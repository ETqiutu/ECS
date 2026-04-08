using System;
using System.Collections.Generic;
using ECS.Library.Components;

namespace ECS.Library.Entity
{
    public class GameObject : IEntity
    {
        public int ID { get; private set; }
        private Dictionary<ComponentType, IComponent> components = new Dictionary<ComponentType, IComponent>();

        public GameObject(int ID)
        {
            this.ID = ID;
        }

        public void AddComponent<T>() where T : IComponent, new()
        {
            T component = new T();
            ComponentType type = component.Type;
            
            if (components.ContainsKey(type))
            {
                throw new InvalidOperationException($"Component of type {type} already exists on GameObject {ID}");
            }
            
            components[type] = component;
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            ComponentType? typeToRemove = null;
            
            foreach (var kvp in components)
            {
                if (kvp.Value is T)
                {
                    typeToRemove = kvp.Key;
                    break;
                }
            }
            
            if (typeToRemove.HasValue)
            {
                components.Remove(typeToRemove.Value);
            }
        }

        public T GetComponent<T>() where T : IComponent
        {
            foreach (var component in components.Values)
            {
                if (component is T typedComponent)
                {
                    return typedComponent;
                }
            }
            
            return default(T);
        }
        
        public IComponent GetComponentByType(ComponentType type)
        {
            components.TryGetValue(type, out IComponent component);
            return component;
        }
        
        public bool HasComponent<T>() where T : IComponent
        {
            foreach (var component in components.Values)
            {
                if (component is T)
                {
                    return true;
                }
            }
            return false;
        }
        
        public bool HasComponent(ComponentType type)
        {
            return components.ContainsKey(type);
        }
    }
}