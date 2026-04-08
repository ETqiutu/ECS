using Microsoft.Xna.Framework;
using ECS.Library.Entity;

namespace ECS.Library.Components.Base
{
    public class Transform : IComponent
    {
        public ComponentType Type {get; private set;}
        public Vector2 Position;
        public float Rotation;
        public Vector2 Scale;
        public GameObject gameObject;

        public Transform()
        {
            Type = ComponentType.Transform;
            Position = new Vector2(0f, 0f);
            Rotation = 0f;
            Scale = new Vector2(1f, 1f);
        }
    }
}