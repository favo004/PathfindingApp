using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PathfindingApp.Sprites
{
    public class Eraser : Sprite
    {
        public Eraser()
            : base()
        {

        }

        public override void LoadContent(ContentManager content)
        {
            Texture = content.Load<Texture2D>("Graphics/Eraser");
            SourceRect = new Rectangle(0, 0, 16, 16);
            Origin = new Vector2(SourceRect.Width / 2, SourceRect.Height / 2);
        }
    }
}
