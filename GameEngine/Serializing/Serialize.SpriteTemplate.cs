﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using GameEngine.Templates;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;
using CommonLibrary.Serializing;
using CommonLibrary;

namespace GameEngine.Serializing
{
    public static partial class GameEngineSerialize
    {
        private class FrameSet
        {
            public string Name;
            public int FPS;
            public IList<int> Frames;
            public SpriteEffects Effects;
        }

        public static void Write(ISerializer context, ISpriteTemplate template)
        {
            context.Write("name", template.Name);
            context.Write("type", template.GetType().AssemblyQualifiedName);
            context.Write("origin", template.Origin, CommonSerialize.Write);

            var asSingle = template as SingleSpriteTemplate;
            var asAnimated = template as AnimatedSpriteTemplate;
            var asSheet = template as SpriteSheetTemplate;

            if (asSingle != null)
            {
                context.Write("shape", asSingle.Shape, FarseerSerialize.Write);
                context.Write("texture", template.Texture.Name);
            }
            else if (asAnimated != null)
            {
                context.Write("shape", asAnimated.Shape, FarseerSerialize.Write);
                context.Write("fps", template.FPS);
                context.WriteList("textures", asAnimated.Textures.Select(t => t.Name).ToList());
            }
            else if (asSheet != null)
            {
                context.Write("shape", asSheet.Shape, FarseerSerialize.Write);
                context.Write("texture", asSheet.Texture.Name);
                context.Write("width", asSheet.Width);
                context.Write("height", asSheet.Height);
                context.Write("border", asSheet.Border);
                var asAnimatedSheet = asSheet as AnimatedSpriteSheetTemplate;
                if (asAnimatedSheet != null)
                {
                    context.Write("fps", template.FPS);
                    context.Write("frames", asSheet.NumberOfFrames);
                }
                var asNASS = asSheet as NamedAnimatedSpriteSheetTemplate;
                if (asNASS != null)
                {
                    context.Write("fps", template.FPS);
                    context.Write("frames", asNASS.NumberOfFrames);
                    context.WriteList("animations", asNASS.Keys.Select(k => new FrameSet
                    {
                        Name = k,
                        FPS = asNASS.GetAnimation(k).FPS,
                        Frames = asNASS.Frames(k).ToList(),
                    }).ToList(), Write);
                }
            }
            else
            {
                throw new InvalidOperationException("Unknown SpriteTemplate type");
            }
        }

        public static void Read(ContentManager content, IDeserializer context, out ISpriteTemplate template)
        {
            var name = context.Read<string>("name");
            var typeName = context.Read<string>("type");
            var type = Type.GetType(typeName);
            var origin = Vector2.Zero;
            try
            {
                origin = context.Read<Vector2>("origin", CommonSerialize.Read);
            }
            catch
            {
                //
            }
            Shape shape = null;
            try
            {
                shape = context.Read<Shape>("shape", FarseerSerialize.Read);
            }
            catch
            {
                //
            }
            if (type == typeof(SingleSpriteTemplate))
            {
                var assetName = context.Read<string>("texture");
                template = new SingleSpriteTemplate(name, content.Load<Texture2D>(assetName));
            }
            else if (type == typeof(AnimatedSpriteTemplate))
            {
                var fps = context.Read<int>("fps");
                var textures = context.ReadList<string>("textures").Select(assetName => content.Load<Texture2D>(assetName));
                template = new AnimatedSpriteTemplate(name, textures)
                {
                    FPS = fps,
                };
            }
            else if (type == typeof(SpriteSheetTemplate) || type.IsSubclassOf(typeof(SpriteSheetTemplate)))
            {
                var texture = content.Load<Texture2D>(context.Read<string>("texture"));
                var width = context.Read<int>("width");
                var height = context.Read<int>("height");
                var border = context.ReadOptional("border", 0);
                var parseMethod = context.ReadOptional("method", "HorizontalFirst");
                SpriteSheetTemplate.ParseMethod method;
                if (!Enum.TryParse(parseMethod, out method))
                {
                    method = SpriteSheetTemplate.ParseMethod.HorizontalFirst;
                }

                if (type == typeof(AnimatedSpriteSheetTemplate))
                {
                    var fps = context.Read<int>("fps");
                    var frames = context.Read<int>("frames");
                    template = new AnimatedSpriteSheetTemplate(name, texture, width, height, border, frames, method)
                    {
                        FPS = fps,
                    };
                }
                else if (type == typeof(NamedAnimatedSpriteSheetTemplate))
                {
                    var fps = context.Read<int>("fps");
                    var frames = context.Read<int>("frames");
                    var nass = new NamedAnimatedSpriteSheetTemplate(name, texture, width, height, border, frames, method)
                    {
                        FPS = fps
                    };
                    foreach (var animation in context.ReadList<FrameSet>("animations", Read))
                    {
                        nass.AddAnimation(animation.Name, animation.FPS, animation.Frames, animation.Effects);
                    }
                    template = nass;
                }
                else
                {
                    var numSprites = context.ReadOptional("count", -1);
                    template = new SpriteSheetTemplate(name, texture, width, height, border, numSprites, method);
                }
            }
            else
            {
                throw new InvalidOperationException($"Unknown sprite template type: {typeName}");
            }
            if (shape != null) template.Shape = shape;
            template.Origin = origin;
        }

        private static void Write(ISerializer context, FrameSet frameSet)
        {
            context.Write("key", frameSet.Name);
            context.Write("fps", frameSet.FPS);
            context.Write("frames", frameSet.Frames);
            if (frameSet.Effects != SpriteEffects.None)
            {
                context.Write("effects", Enum.GetName(typeof(SpriteEffects), frameSet.Effects));
            }
        }

        private static void Read(IDeserializer context, out FrameSet frameSet)
        {
            var name = context.Read<string>("key");
            var fps = context.Read<int>("fps");
            var frames = context.ReadList<int>("frames");
            string effects;
            try
            {
                effects = context.Read<string>("effects");
            }
            catch
            {
                effects = "None";
            }
            SpriteEffects e;
            if (!Enum.TryParse(effects, out e))
            {
                e = SpriteEffects.None;
            }
            frameSet = new FrameSet
            {
                Name = name,
                FPS = fps,
                Frames = frames,
                Effects = e
            };
        }
    }
}
