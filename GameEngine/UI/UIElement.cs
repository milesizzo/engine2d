using GameEngine.Templates;
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
    public enum UIOrigin
    {
        TopLeft,
        TopCentre,
        TopRight,
        BottomLeft,
        BottomCentre,
        BottomRight,
        Centre
    }

    public struct UIPlacement
    {
        private float? x;
        private float? y;
        private float? relativeX;
        private float? relativeY;

        public float ToAbsoluteX(float scale)
        {
            if (this.x == null)
            {
                if (this.relativeX == null) return 0;
                return scale * this.relativeX.Value;
            }
            return this.x.Value;
        }

        public float ToAbsoluteY(float scale)
        {
            if (this.y == null)
            {
                if (this.relativeY == null) return 0;
                return scale * this.relativeY.Value;
            }
            return this.y.Value;
        }

        public bool IsEmpty
        {
            get { return this.x == null && this.y == null && this.relativeX == null && this.relativeY == null; }
        }

        public float X { get { return this.x ?? 0; } set { this.x = value; } }
        public float Y { get { return this.y ?? 0; } set { this.y = value; } }
        public float RelativeX { get { return this.relativeX ?? 0; } set { this.relativeX = value; } }
        public float RelativeY { get { return this.relativeY ?? 0; } set { this.relativeY = value; } }
    }

    public static class UIDefaults
    {
        public static Color BackgroundColour = new Color(0.1f, 0.1f, 0.1f);
        public static Color BorderColour = new Color(0.2f, 0.2f, 0.2f);
        public static Vector2 Padding = Vector2.Zero;
    }

    public enum UIMouseButton
    {
        Left,
        Right,
        Middle,
    }

    public enum UIMouseButtonState
    {
        Pressed,
        Released,
        LostFocus,
    }

    public class UIMouseState
    {
        public UIElement Element;
    }

    public delegate void UIEventHandler(UIElement owner);
    public delegate void MouseButtonEventHandler(UIElement owner, UIMouseButton button, UIMouseButtonState state);

    public abstract class UIElement
    {
        public static Size2 ScreenDimensions;
        private readonly List<UIElement> elements = new List<UIElement>();
        private readonly UIElement parent;
        public UIPlacement Placement;
        public UIPlacement Size;
        private Vector2? padding;
        private UIOrigin? origin;
        private Size2? minimumSize = null;
        public bool AllowSubPixelRendering = false;
        public bool AcceptsInput = true;
        public event UIEventHandler MouseEnter;
        public event UIEventHandler MouseLeave;
        public event MouseButtonEventHandler MouseButtonDown;
        //public event MouseButtonEventHandler MouseButtonUp;
        public event MouseButtonEventHandler MouseButtonPress;
        public event MouseButtonEventHandler MouseButtonRelease;
        private bool mouseColliding = false;
        private Dictionary<UIMouseButton, UIMouseButtonState> mouseButtonState = new Dictionary<UIMouseButton, UIMouseButtonState>
        {
            {UIMouseButton.Left, UIMouseButtonState.Released},
            {UIMouseButton.Right, UIMouseButtonState.Released},
            {UIMouseButton.Middle, UIMouseButtonState.Released},
        };

        protected UIElement() : this(null) { }

        protected UIElement(UIElement parent)
        {
            this.parent = parent;
            if (this.parent != null) this.parent.elements.Add(this);
            this.Initialise();
        }

        public virtual void Initialise()
        {
            //
        }

        public void Draw(SpriteBatch screen)
        {
            this.Render(screen);
            //this.DrawRectangle(screen, this.ScreenTopLeft + this.Padding, new Size2(20, 20), Color.White, 1f);
            foreach (var child in this.elements)
            {
                child.Draw(screen);
            }
            var mouse = Mouse.GetState();
            screen.DrawLine(mouse.X, mouse.Y, mouse.X + 16, mouse.Y + 16, Color.White, 2);
            screen.DrawLine(mouse.X, mouse.Y, mouse.X + 16, mouse.Y, Color.White, 2);
            screen.DrawLine(mouse.X, mouse.Y, mouse.X, mouse.Y + 16, Color.White, 2);
        }

        private void HandleMouseButton(UIMouseButton button, UIMouseButtonState state)
        {
            if (this.mouseButtonState[button] != state)
            {
                // state change
                switch (state)
                {
                    case UIMouseButtonState.Pressed:
                        this.MouseButtonPress?.Invoke(this, button, state);
                        break;
                    case UIMouseButtonState.Released:
                    case UIMouseButtonState.LostFocus:
                        this.MouseButtonRelease?.Invoke(this, button, state);
                        break;
                }
                this.mouseButtonState[button] = state;
            }
            else if (this.mouseButtonState[button] == UIMouseButtonState.Pressed)
            {
                this.MouseButtonDown?.Invoke(this, button, state);
            }
        }

        private bool IsMouseColliding
        {
            get
            {
                var mouse = Mouse.GetState();
                var mousePos = new Vector2(mouse.X, mouse.Y);
                var stl = this.ScreenTopLeft;
                var size = this.AbsoluteSize;
                return (mousePos.X >= stl.X && mousePos.X < stl.X + size.X && mousePos.Y >= stl.Y && mousePos.Y < stl.Y + size.Y);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            var childMouseCollision = false;
            foreach (var child in this.elements)
            {
                child.Update(gameTime);
                childMouseCollision = childMouseCollision || child.mouseColliding;
            }
            if (childMouseCollision && this.mouseColliding)
            {
                this.mouseColliding = false;
                this.MouseLeave?.Invoke(this);
            }
            else if (!childMouseCollision && this.AcceptsInput)
            {
                var localMouseColliding = this.IsMouseColliding;
                if (this.mouseColliding)
                {
                    if (!localMouseColliding) 
                    {
                        this.mouseColliding = false;
                        this.MouseLeave?.Invoke(this);
                    }
                }
                else
                {
                    if (localMouseColliding) 
                    {
                        this.mouseColliding = true;
                        this.MouseEnter?.Invoke(this);
                    }
                }
            }
        }
        
        public virtual Vector2 Padding
        {
            get { return this.padding ?? UIDefaults.Padding; }
            set { this.padding = value; }
        }

        protected Vector2 ScreenTopLeft
        {
            get
            {
                if (this.parent == null)
                {
                    return this.TopLeft;
                }
                var parentTopLeft = this.parent.ScreenTopLeft + this.parent.Padding;
                return parentTopLeft + this.TopLeft;
            }
        }

        public Size2 MinimumSize
        {
            get
            {
                var padding = this.Padding;
                if (this.elements.Any())
                {
                    var max = new Size2();
                    foreach (var element in this.elements)
                    {
                        // get the minimum size of the child element (plus our padding)
                        var min = element.MinimumSize;
                        min.Width += padding.X * 2;
                        min.Height += padding.Y * 2;

                        // get the current absolute size of the child element (plus our padding)
                        var currX = element.Size.X + padding.X * 2;
                        var currY = element.Size.Y + padding.Y * 2;

                        // if the child size plus padding is bigger than the current max, set it
                        if (min.Width > max.Width) max.Width = min.Width;
                        if (currX > max.Width) max.Width = currX;
                        if (min.Height > max.Height) max.Height = min.Height;
                        if (currY > max.Height) max.Height = currY;
                    }
                    if (this.minimumSize != null)
                    {
                        var min = this.minimumSize.Value;
                        min.Width += padding.X * 2;
                        min.Height += padding.Y * 2;

                        if (min.Width > max.Width) max.Width = min.Width;
                        if (min.Height > max.Height) max.Height = min.Height;
                    }
                    return max;
                }
                else
                {
                    var result = new Size2(padding.X * 2, padding.Y * 2);
                    if (this.minimumSize != null)
                    {
                        result += this.minimumSize.Value;
                    }
                    return result;
                }
            }
            protected set { this.minimumSize = value; }
        }

        public UIOrigin Origin
        {
            get { return this.origin ?? UIOrigin.TopLeft; }
            set { this.origin = value; }
        }

        private static float RelativeToAbsolute(float parent, float? relative, float? absolute)
        {
            if (absolute == null)
            {
                if (relative == null) return 0;
                return parent * relative.Value;
            }
            return absolute.Value;
        }

        private Vector2 GetAbsolute(ref UIPlacement placement)
        {
            Vector2 parentSize;
            if (this.parent == null)
            {
                parentSize = UIElement.ScreenDimensions;
            }
            else
            {
                parentSize = this.parent.AbsoluteSize;
                parentSize -= this.parent.Padding * 2;
            }
            return new Vector2(placement.ToAbsoluteX(parentSize.X), placement.ToAbsoluteY(parentSize.Y));
        }

        public Vector2 AbsolutePosition
        {
            get { return this.GetAbsolute(ref this.Placement); }
        }

        public Vector2 AbsoluteSize
        {
            get
            {
                var actual = this.GetAbsolute(ref this.Size);
                var min = this.MinimumSize;
                if (actual.X < min.Width) actual.X = min.Width;
                if (actual.Y < min.Height) actual.Y = min.Height;
                return actual;
            }
        }

        public Vector2 TopLeft
        {
            get
            {
                var size = this.AbsoluteSize;
                var pos = this.AbsolutePosition;
                switch (this.Origin)
                {
                    case UIOrigin.TopLeft:
                        return pos;
                    case UIOrigin.TopCentre:
                        return new Vector2(pos.X - (size.X / 2f), pos.Y);
                    case UIOrigin.TopRight:
                        return new Vector2(pos.X - size.X, pos.Y);
                    case UIOrigin.BottomLeft:
                        return new Vector2(pos.X, pos.Y - size.Y);
                    case UIOrigin.BottomCentre:
                        return new Vector2(pos.X - (size.X / 2f), pos.Y - size.Y);
                    case UIOrigin.BottomRight:
                        return new Vector2(pos.X - size.X, pos.Y - size.Y);
                    case UIOrigin.Centre:
                        return new Vector2(pos.X - (size.X / 2f), pos.Y - (size.Y / 2f));
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
