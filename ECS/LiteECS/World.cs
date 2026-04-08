using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteECS
{
    public class World
    {
        private int[] generations = new int[1024];
        private int nextId;
        private readonly Stack<int> freeIds = new Stack<int>();

        private readonly Dictionary<Type, Dictionary<int, Component>> componentPools = new();
        private readonly Dictionary<int, List<Type>> entityComponentTypes = new();

        private readonly List<ISystem> systems = new();

        private ISystem[] updateSystems = Array.Empty<ISystem>();
        private IDrawSystem[] drawSystems = Array.Empty<IDrawSystem>();
        private bool systemsDirty = true;

        public World()
        {
            for (int i = 0; i < generations.Length; i++)
                generations[i] = 0;
        }

        #region Entity Management

        public Entity CreateEntity()
        {
            int id;
            if (freeIds.Count > 0)
            {
                id = freeIds.Pop();
                generations[id]++;
            }
            else
            {
                id = nextId++;
                if (id >= generations.Length)
                    Array.Resize(ref generations, generations.Length * 2);
                generations[id] = 1;
            }

            entityComponentTypes[id] = new List<Type>();
            return new Entity(id, generations[id]);
        }

        public void DestroyEntity(Entity entity)
        {
            if (!entity.IsValid || entity.Id >= generations.Length || generations[entity.Id] != entity.Generation)
                return;

            int id = entity.Id;

            // 移除所有组件
            if (entityComponentTypes.TryGetValue(id, out var types))
            {
                foreach (var type in types.ToArray())
                    RemoveComponentInternal(entity, type);
                entityComponentTypes.Remove(id);
            }

            freeIds.Push(id);
        }

        #endregion

        #region Component Management

        public T AddComponent<T>(Entity entity, T component) where T : Component
        {
            if (!entity.IsValid || entity.Id >= generations.Length || generations[entity.Id] != entity.Generation)
                throw new ArgumentException("Invalid entity");

            var type = typeof(T);
            if (!componentPools.TryGetValue(type, out var pool))
            {
                pool = new Dictionary<int, Component>();
                componentPools[type] = pool;
            }

            if (pool.ContainsKey(entity.Id))
                throw new InvalidOperationException($"Entity already has component {type.Name}");

            component.Entity = entity;
            pool[entity.Id] = component;
            entityComponentTypes[entity.Id].Add(type);

            return component;
        }

        public T GetComponent<T>(Entity entity) where T : Component
        {
            if (!entity.IsValid) return null;
            var type = typeof(T);
            if (componentPools.TryGetValue(type, out var pool) && pool.TryGetValue(entity.Id, out var comp))
                return (T)comp;
            return null;
        }

        public bool HasComponent<T>(Entity entity) where T : Component
        {
            return GetComponent<T>(entity) != null;
        }

        public void RemoveComponent<T>(Entity entity) where T : Component
        {
            if (!entity.IsValid) return;
            RemoveComponentInternal(entity, typeof(T));
        }

        private void RemoveComponentInternal(Entity entity, Type type)
        {
            if (componentPools.TryGetValue(type, out var pool) && pool.Remove(entity.Id))
                entityComponentTypes[entity.Id].Remove(type);
        }

        #endregion

        #region Query

        /// <summary>
        /// 获取包含指定组件的所有实体（返回元组以便遍历）
        /// </summary>
        public IEnumerable<(Entity entity, T comp)> Query<T>() where T : Component
        {
            var type = typeof(T);
            if (componentPools.TryGetValue(type, out var pool))
            {
                foreach (var kv in pool)
                    yield return (new Entity(kv.Key, generations[kv.Key]), (T)kv.Value);
            }
        }

        /// <summary>
        /// 获取同时包含两个组件的实体
        /// </summary>
        public IEnumerable<(Entity entity, T1 comp1, T2 comp2)> Query<T1, T2>()
            where T1 : Component
            where T2 : Component
        {
            var t1Type = typeof(T1);
            var t2Type = typeof(T2);
            if (!componentPools.TryGetValue(t1Type, out var pool1) ||
                !componentPools.TryGetValue(t2Type, out var pool2))
                yield break;

            foreach (var kv in pool1)
            {
                if (pool2.ContainsKey(kv.Key))
                    yield return (
                        new Entity(kv.Key, generations[kv.Key]),
                        (T1)kv.Value,
                        (T2)pool2[kv.Key]
                    );
            }
        }

        #endregion

        #region System Management

        public World AddSystem(ISystem system)
        {
            systems.Add(system);
            system.Initialize(this);
            systemsDirty = true;
            return this;
        }

        public void Update(GameTime gameTime)
        {
            if (systemsDirty) RebuildSystemLists();
            foreach (var sys in updateSystems)
                if (sys.Enabled)
                    sys.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            if (systemsDirty) RebuildSystemLists();
            foreach (var sys in drawSystems)
                if (sys.Enabled)
                    sys.Draw(gameTime);
        }

        private void RebuildSystemLists()
        {
            updateSystems = systems.Where(s => s is not IDrawSystem).ToArray();
            drawSystems = systems.OfType<IDrawSystem>().ToArray();
            systemsDirty = false;
        }

        #endregion
    }
}