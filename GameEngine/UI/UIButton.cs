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
    public delegate void UIButtonClicked(UIButton button);

    public class UIButton : UIPanel
    {
        private UILabel label;
        private bool highlighted;
        private bool clicked;
        public Color HighlightColour;
        public Color ClickedColour;
        public event UIButtonClicked ButtonClick;

        public UIButton(UIElement parent) : base(parent) { }

        public UIButton() : base() { }

        public override void Initialise()
        {
            base.Initialise();
            this.HighlightColour = Color.DarkRed;
            this.ClickedColour = Color.Red;
            this.Colour = Color.DarkBlue;
            this.Padding = Vector2.Zero;
            this.Border.Padding = 2;
            this.label = new UILabel(this);
            this.label.Placement.RelativeX = 0.5f;
            this.label.Placement.RelativeY = 0.5f;
            this.label.Origin = UIOrigin.Centre;
            this.label.HorizontalAlignment = UILabel.HTextAlign.Centre;
            this.label.VerticalAlignment = UILabel.VTextAlign.Middle;
            this.label.AcceptsInput = false;
            this.label.TextColour = Color.Yellow;

            this.MouseEnter += (owner) =>
            {
                this.highlighted = true;
            };
            this.MouseLeave += (owner) =>
            {
                this.highlighted = false;
                this.clicked = false;
            };
            this.MouseFocus += (owner) =>
            {
                var mouse = Mouse.GetState();
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    if (!this.clicked)
                    {
                        this.clicked = true;
                    }
                }
                else
                {
                    if (this.clicked)
                    {
                        this.clicked = false;
                        this.ButtonClick?.Invoke(this);
                    }
                }
            };
        }

        public UILabel Label { get { return this.label; } }

        public override Color Colour
        {
            get
            {
                if (this.clicked)
                {
                    return this.ClickedColour;
                }
                else if (this.highlighted)
                {
                    return this.HighlightColour;
                }
                return base.Colour;
            }

            set
            {
                base.Colour = value;
            }
        }

        protected override void Render(SpriteBatch screen)
        {
            base.Render(screen);
        }
    }
}
