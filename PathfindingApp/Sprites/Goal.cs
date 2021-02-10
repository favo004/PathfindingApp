using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PathfindingApp.Sprites
{
    public class Goal : Sprite
    {
        public Goal()
            : base()
        {

        }

        public override void LoadContent(ContentManager content)
        {
            Texture = content.Load<Texture2D>("Graphics/Goal");
            SourceRect = new Rectangle(0, 0, 16, 16);
            Origin = new Vector2(SourceRect.Width / 2, SourceRect.Height / 2);
        }

        public override object Copy()
        {
            Goal copy = new Goal();
            copy.SetAttributes(Texture, SourceRect, Scale, Rotation, Color, Bounds);
            return copy;
        }
    }
}
