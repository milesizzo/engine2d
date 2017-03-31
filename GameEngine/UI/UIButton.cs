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
    public class UIButton : UIPanel
    {
        private UILabel label;
        private bool highlighted;
        public Color HighlightColour;

        public UIButton(UIElement parent) : base(parent) { }

        public UIButton() : base() { }

        public override void Initialise()
        {
            base.Initialise();
            this.HighlightColour = Color.DarkRed;
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

            this.MouseEnter += (owner) =>
            {
                (owner as UIButton).highlighted = true;
            };
            this.MouseLeave += (owner) =>
            {
                (owner as UIButton).highlighted = false;
            };
            this.MouseButtonPress += (owner, button, state) =>
            {
                this.Label.Text = $"{button} pressed ({state})";
            };
            this.MouseButtonRelease += (owner, button, state) =>
            {
                this.Label.Text = $"{button} released ({state})";
            };
        }

        public UILabel Label { get { return this.label; } }

        public override Color Colour
        {
            get
            {
                return this.highlighted ? this.HighlightColour : base.Colour;
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
