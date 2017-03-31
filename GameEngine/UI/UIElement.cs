using GameEngine.Templates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.UI
{
    public enum UIOrigin
    {
        TopLeft,
        BottomRight,
        TopRight,
        BottomLeft,
        Centre
    }

    public abstract class UIElement
    {
        private readonly List<UIElement> elements = new List<UIElement>();
        private UIElement parent;
        private Vector2? position;
        private Size2? size;
        private UIOrigin? origin;
        private Size2? minimumSize = null;
        public bool AllowSubPixelRendering = false;

        protected UIElement()
        {
            this.Initialise();
        }

        protected UIElement(UIElement parent)
        {
            this.parent = parent;
            this.parent.elements.Add(this);
            this.Initialise();
        }

        public virtual void Initialise()
        {
            //
        }

        public void Draw(SpriteBatch screen)
        {
            this.Render(screen);
            foreach (var child in this.elements)
            {
                child.Draw(screen);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (var child in this.elements)
            {
                child.Update(gameTime);
            }
        }

        protected Vector2 ScreenTopLeft
        {
            get
            {
                if (this.parent == null)
                {
                    return this.TopLeft;
                }
                var parentTopLeft = this.parent.ScreenTopLeft;
                var localTopLeft = this.TopLeft;
                return new Vector2(parentTopLeft.X + localTopLeft.X, parentTopLeft.Y + localTopLeft.Y);
            }
        }

        public Size2 MinimumSize
        {
            get
            {
                if (this.elements.Any())
                {
                    var max = this.elements.Max(e => e.MinimumSize);
                    if (this.minimumSize != null)
                    {
                        var min = this.minimumSize.Value;
                        var width = (max.Width < min.Width) ? min.Width : max.Width;
                        var height = (max.Height < min.Height) ? min.Height : max.Height;
                        return new Size2(width, height);
                    }
                    return max;
                }
                else
                {
                    return this.minimumSize ?? new Size2();
                }
            }
            protected set { this.minimumSize = value; }
        }

        public UIOrigin Origin
        {
            get { return this.origin ?? UIOrigin.TopLeft; }
            set { this.origin = value; }
        }

        public Vector2 Position
        {
            get { return this.position ?? Vector2.Zero; }
            set { this.position = value; }
        }

        public Size2 Size
        {
            get
            {
                var min = this.MinimumSize;
                if (this.size == null) return min;
                var size = this.size.Value;
                var width = (size.Width < min.Width) ? min.Width : size.Width;
                var height = (size.Height < min.Height) ? min.Height : size.Height;
                return new Size2(width, height);
            }
            set { this.size = value; }
        }

        public float Width { get { return this.Size.Width; } }

        public float Height { get { return this.Size.Height; } }

        public Vector2 TopLeft
        {
            get
            {
                var size = this.Size;
                var pos = this.Position;
                switch (this.Origin)
                {
                    case UIOrigin.TopLeft:
                        return pos;
                    case UIOrigin.TopRight:
                        return new Vector2(pos.X - size.Width, pos.Y);
                    case UIOrigin.BottomRight:
                        return new Vector2(pos.X - size.Width, pos.Y - size.Height);
                    case UIOrigin.BottomLeft:
                        return new Vector2(pos.X, pos.Y - size.Height);
                    case UIOrigin.Centre:
                        return new Vector2(pos.X - (size.Width / 2f), pos.Y - (size.Height / 2f));
                }
                return Vector2.Zero; // just to shut the compiler up
            }
        }

        protected void ApplySubPixel(ref Vector2 topLeft)
        {
            if (!this.AllowSubPixelRendering)
            {
                topLeft.X = (float)Math.Truncate(topLeft.X);
                topLeft.Y = (float)Math.Truncate(topLeft.Y);
            }
        }

        protected void ApplySubPixel(ref Size2 size)
        {
            if (!this.AllowSubPixelRendering)
            {
                size.Width = (float)Math.Truncate(size.Width);
                size.Height = (float)Math.Truncate(size.Height);
            }
        }

        protected void DrawRectangle(SpriteBatch screen, Vector2 topLeft, Size2 size, Color colour, float thickness)
        {
            this.ApplySubPixel(ref topLeft);
            this.ApplySubPixel(ref size);
            screen.DrawRectangle(topLeft, size, colour, thickness);
        }

        protected void FillRectangle(SpriteBatch screen, Vector2 topLeft, Size2 size, Color colour)
        {
            this.ApplySubPixel(ref topLeft);
            this.ApplySubPixel(ref size);
            screen.FillRectangle(topLeft, size, colour);
        }

        protected void DrawString(SpriteBatch screen, FontTemplate font, string text, Vector2 pos, Color colour)
        {
            this.ApplySubPixel(ref pos);
            screen.DrawString(font.Font, text, pos, colour);
        }

        protected abstract void Render(SpriteBatch screen);

        public UIElement Parent { get { return this.parent; } }

        public IReadOnlyList<UIElement> Elements { get { return this.elements; } }
    }
}
