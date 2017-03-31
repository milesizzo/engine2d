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

        public UIButton(UIElement parent) : base(parent) { }

        public UIButton() : base() { }

        public override void Initialise()
        {
            base.Initialise();
            this.Padding = Vector2.Zero;
            this.Border.Padding = 2;
            this.label = new UILabel(this);
            this.label.Placement.RelativeX = 0.5f;
            this.label.Placement.RelativeY = 0.5f;
            //this.label.Size.RelativeX = 0.5f;
            //this.label.Size.RelativeY = 0.5f;
            this.label.Origin = UIOrigin.Centre;
            this.label.HorizontalAlignment = UILabel.HTextAlign.Centre;
            this.label.VerticalAlignment = UILabel.VTextAlign.Middle;
        }

        public UILabel Label { get { return this.label; } }

        protected override void Render(SpriteBatch screen)
        {
            base.Render(screen);
        }
    }
}
