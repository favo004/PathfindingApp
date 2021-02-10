using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathfindingApp.Sprites
{
    public enum OptionType
    {
        Pause,
        SpeedOne,
        SpeedTwo
    }
    public class OptionIcon : Sprite
    {
        private OptionType _type;

        public OptionIcon(OptionType type)
            : base()
        {
            _type = type;
        }

        public override void LoadContent(ContentManager content)
        {
            Texture = content.Load<Texture2D>("Graphics/RunningIcons");
            SourceRect = GetSourceRect();
        }

        private Rectangle GetSourceRect()
        {
            switch (_type)
            {
                case OptionType.Pause:
                    return new Rectangle(16, 0, 16, 16);
                case OptionType.SpeedOne:
                    return new Rectangle(0, 16, 16, 16);
                case OptionType.SpeedTwo:
                    return new Rectangle(16, 16, 16, 16);
            }

            return Rectangle.Empty;
        }
    }
}
