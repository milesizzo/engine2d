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
    public struct Border
    {
        public static Border Default = new Border()
        {
            Enabled = true,
            Colour = UIDefaults.BorderColour,
            Thickness = 1f,
            Padding = 8f,
        };
        public static Border None = new Border() { Enabled = false };

        public float Padding;
        public bool Enabled;
        public Color Colour;
        public float Thickness;
        /*
        public event EventHandler PaddingChanged;

        public float Padding
        {
            get { return this.padding; }
            set
            {
                this.PaddingChanged?.Invoke(null, EventArgs.Empty);
                this.padding = value;
            }
        }
        */
    }

    public class UIPanel : UIElement
    {
        private Color colour;
        public Border Border;

        public UIPanel(UIElement parent) : base(parent) { }

        public UIPanel() : base() { }

        public override void Initialise()
        {
            base.Initialise();
            this.Colour = UIDefaults.BackgroundColour;
            this.Border = Border.Default;
        }

        public override Vector2 Padding
        {
            get
            {
                return base.Padding + new Vector2(this.Border.Padding);
            }

            set
            {
                base.Padding = value;
            }
        }

        public virtual Color Colour
        {
            get { return this.colour; }
            set { this.colour = value; }
        }

        protected override void Render(SpriteBatch screen)
        {
            var topLeft = this.ScreenTopLeft;
            var size = this.AbsoluteSize;

            this.FillRectangle(screen, topLeft, size, this.Colour);

            if (this.Border.Enabled)
            {
                topLeft += new Vector2(this.Border.Padding / 2);
                size.X -= this.Border.Padding;
                size.Y -= this.Border.Padding;
                this.DrawRectangle(screen, topLeft, size, this.Border.Colour, this.Border.Thickness);
            }
        }
    }
}
