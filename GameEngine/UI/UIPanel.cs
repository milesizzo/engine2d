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
            Colour = Color.DarkGray,
            Thickness = 1f,
            Padding = 8f,
        };

        public bool Enabled;
        public Color Colour;
        public float Thickness;
        public float Padding;
    }

    public class UIPanel : UIElement
    {
        public Color Colour;
        public Border Border;

        public UIPanel(UIElement parent) : base(parent) { }

        public UIPanel() : base() { }

        public override void Initialise()
        {
            base.Initialise();
            this.Border = Border.Default;
        }

        protected override void Render(SpriteBatch screen)
        {
            var topLeft = this.ScreenTopLeft;
            var size = this.Size;

            this.FillRectangle(screen, topLeft, size, this.Colour);

            if (this.Border.Enabled)
            {
                topLeft += new Vector2(this.Border.Padding);
                size.Width -= this.Border.Padding * 2;
                size.Height -= this.Border.Padding * 2;
                this.DrawRectangle(screen, topLeft, size, this.Border.Colour, this.Border.Thickness);
            }
        }
    }
}
