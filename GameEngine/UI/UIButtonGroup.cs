using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Templates;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace GameEngine.UI
{
    public class UIButtonGroup : UIPanel
    {
        private readonly List<UIButton> buttons = new List<UIButton>();
        public float Spacing;

        public UIButtonGroup(UIElement parent) : base(parent) { }

        public UIButtonGroup() : base() { }

        public override void Initialise()
        {
            base.Initialise();
            this.Colour = new Color(5, 5, 5, 64);
            this.Spacing = 8f;
            this.Padding = new Vector2(8f);
        }

        public UIButton AddButton(FontTemplate font, string text, Color buttonColour, Color textColour)
        {
            var button = new UIButton(this);
            button.Origin = UIOrigin.TopCentre;
            button.Placement.RelativeX = 0.5f;
            button.Size.RelativeX = 0.8f;
            button.Label.Text = text;
            button.Label.Font = font;
            button.Label.TextColour = textColour;
            button.Colour = buttonColour;
            this.buttons.Add(button);

            var count = (float)this.buttons.Count;
            var spacing = 1 / (count + 1);
            var y = 0f;
            for (var i = 0; i < this.buttons.Count; i++)
            {
                var child = this.buttons[i];
                child.Placement.Y = y;
                //RelativeY = spacing * (i+1); //((float)i / (float)this.buttons.Count) * 0.75f;
                //child.Size.RelativeY = 1 / (float)this.buttons.Count;
                child.Size.Y = 32;
                y += child.AbsoluteSize.Y + this.Spacing;
                child.Label.Text = $"{child.TopLeft}";
            }
            //height += this.buttons.Count * this.Spacing;
            if (y > 0)
            {
                this.MinimumSize = new Size2(0, y - this.Spacing);
            }
            return button;
        }

        protected override void Render(SpriteBatch screen)
        {
            base.Render(screen);
        }
    }
}
