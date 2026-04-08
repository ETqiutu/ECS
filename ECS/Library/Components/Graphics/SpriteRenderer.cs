using ECS.Library.Components.Base;
using ECS.Library.Entity;
using ECS.Library.Render;

namespace ECS.Library.Components.Graphics
{
    public class SpriteRenderer
    {
        public Sprite Texture;
        public GameObject gameObject;
        public Transform transform;
        public SpriteRenderer()
        {
            Texture = null;
        }
    }
}