using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Templates
{
    public class AnimatedSpriteTemplate : SpriteTemplate
    {
        private readonly List<Texture2D> textures = new List<Texture2D>();

        public AnimatedSpriteTemplate(string name, IEnumerable<Texture2D> textures) : base(name)
        {
            float averageWidth = 0, averageHeight = 0;
            foreach (var texture in textures)
            {
                averageWidth += texture.Width;
                averageHeight += texture.Height;
                this.textures.Add(texture);
            }
            averageWidth /= this.textures.Count;
            averageHeight /= this.textures.Count;
            this.Origin = new Vector2(averageWidth / 2, averageHeight / 2);
        }

        public IReadOnlyList<Texture2D> Textures
        {
            get { return this.textures.AsReadOnly(); }
        }

        public override Texture2D Texture
        {
            get { return this.textures.First(); }
            set
            {
                this.textures.Clear();
                this.textures.Add(value);
            }
        }

        public override int NumberOfFrames
        {
            get { return this.textures.Count; }
        }

        public void AddTexture(Texture2D texture)
        {
            this.textures.Add(texture);
        }

        public override void DrawSprite(SpriteBatch sb, int frame, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects, float depth)
        {
            sb.Draw(this.textures[frame], position, null, colour, rotation, this.Origin, scale, effects, depth);
        }
    }
}
