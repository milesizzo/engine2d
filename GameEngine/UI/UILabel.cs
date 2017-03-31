using GameEngine.Templates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.UI
{
    public class UILabel : UIElement
    {
        public enum HTextAlign
        {
            Left,
            Right,
            Centre,
        }

        public enum VTextAlign
        {
            Top,
            Bottom,
            Middle,
        }

        private FontTemplate font;
        private string text;
        public Color TextColour;
        public HTextAlign HorizontalAlignment = HTextAlign.Centre;
        public VTextAlign VerticalAlignment = VTextAlign.Middle;

        public UILabel(UIElement parent) : base(parent) { }

        public UILabel() : base() { }

        public FontTemplate Font
        {
            set
            {
                this.font = value;
                if (this.font != null && this.text != null)
                {
                    this.MinimumSize = this.font.Font.MeasureString(this.text);
                }
            }
            get { return this.font; }
        }

        public string Text
        {
            set
            {
                this.text = value;
                if (this.font != null && this.text != null)
                {
                    this.MinimumSize = this.font.Font.MeasureString(this.text);
                }
            }
            get { return this.text; }
        }

        protected override void Render(SpriteBatch screen)
        {
            var min = this.MinimumSize;
            var topLeft = this.ScreenTopLeft;
            var size = this.Size;
            float x = 0;
            switch (this.HorizontalAlignment)
            {
                case HTextAlign.Left:
                    x = 0;
                    break;
                case HTextAlign.Right:
                    x = topLeft.X + size.Width - min.Width;
                    break;
                case HTextAlign.Centre:
                    x = topLeft.X + (size.Width / 2f) - (min.Width / 2f);
                    break;
            }
            float y = 0;
            switch (this.VerticalAlignment)
            {
                case VTextAlign.Top:
                    y = 0;
                    break;
                case VTextAlign.Bottom:
                    y = topLeft.Y + size.Height - min.Height;
                    break;
                case VTextAlign.Middle:
                    y = topLeft.Y + (size.Height / 2f) - (min.Height / 2f);
                    break;
            }
            this.DrawString(screen, this.Font, this.Text, new Vector2(x, y), this.TextColour);
        }
    }
}
