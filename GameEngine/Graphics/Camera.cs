using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace GameEngine.Graphics
{
    public class Camera : Camera2D
    {
        private readonly GraphicsDevice graphics;
        public SamplerState SamplerState = SamplerState.LinearClamp;

        public Camera(GraphicsDevice graphics) : base(graphics)
        {
            this.graphics = graphics;
        }
        
        public Viewport Viewport
        {
            get { return this.graphics.Viewport; }
        }

        public void Clear(Color colour)
        {
            this.graphics.Clear(colour);
        }
    }
}
