using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Templates
{
    public class SingleSpriteTemplate : SpriteTemplate
    {
        private Texture2D texture;

        public SingleSpriteTemplate(string name, Texture2D texture) : base(name)
        {
            this.Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            this.texture = texture;
        }

        public override Texture2D Texture
        {
            get { return this.texture; }
            set { this.texture = value; }
        }

        public override int NumberOfFrames
        {
            get
            {
                return 1;
            }
        }

        public override void DrawSprite(SpriteBatch sb, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects, float depth)
        {
            sb.Draw(this.texture, position, null, colour, rotation, this.Origin, scale, effects, depth);
        }
    }
}
