using GameEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.UI
{
    public class UISystem
    {
        private readonly List<UIElement> elements = new List<UIElement>();
        private readonly GraphicsDevice graphics;
        private bool enabled;
        public Action<MouseState, Renderer> DrawMouseCursor;

        public UISystem(GraphicsDevice graphics)
        {
            this.graphics = graphics;
            UIElement.ScreenDimensions = new Size2(this.graphics.Viewport.Width, this.graphics.Viewport.Height);
            this.enabled = false;
        }

        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }

        public void Clear()
        {
            this.elements.Clear();
        }

        public void Add(UIElement element)
        {
            this.elements.Add(element);
        }

        public void Update(GameTime gameTime)
        {
            if (!this.Enabled) return;
            foreach (var element in this.elements)
            {
                element.Update(gameTime);
            }
        }

        public void Draw(Renderer renderer)
        {
            if (!this.Enabled) return;
            foreach (var element in this.elements)
            {
                element.Draw(renderer.Screen);
            }
            var mouse = Mouse.GetState();
            if (this.DrawMouseCursor == null)
            {
                renderer.Screen.DrawLine(mouse.X, mouse.Y, mouse.X + 16, mouse.Y + 16, Color.White, 2);
                renderer.Screen.DrawLine(mouse.X, mouse.Y, mouse.X + 8, mouse.Y, Color.White, 2);
                renderer.Screen.DrawLine(mouse.X, mouse.Y, mouse.X, mouse.Y + 8, Color.White, 2);
            }
            else
            {
                this.DrawMouseCursor(mouse, renderer);
            }
        }
    }
}
