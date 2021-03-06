﻿using System;
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
        public float ButtonHeight;

        public UIButtonGroup(UIElement parent) : base(parent) { }

        public UIButtonGroup() : base() { }

        public override void Initialise()
        {
            base.Initialise();
            this.Colour = new Color(5, 5, 5, 64);
            this.Spacing = 8f;
            this.ButtonHeight = 32f;
            this.Padding = new Vector2(8f);
        }

        public UIButton AddButton(FontTemplate font, string text, Action onClick)
        {
            var button = new UIButton(this);
            button.Origin = UIOrigin.Centre;
            button.Placement.RelativeX = 0.5f;
            button.Size.RelativeX = 0.8f;
            button.Label.Text = text;
            button.Label.Font = font;
            button.ButtonClick += (owner) => onClick();
            this.buttons.Add(button);

            var count = this.buttons.Count;
            var buffer = (this.AbsoluteSize.Y - (count * this.ButtonHeight) - (count - 1) * this.Spacing) / 2;
            var y = buffer;
            for (var i = 0; i < this.buttons.Count; i++)
            {
                var child = this.buttons[i];
                child.Placement.Y = y;
                child.Size.Y = this.ButtonHeight;
                y += child.AbsoluteSize.Y + this.Spacing;
            }
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
