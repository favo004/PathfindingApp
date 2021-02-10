using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace PathfindingApp.UI
{
    /// <summary>
    /// Standard button for single purpose use
    /// </summary>
    public class StandardButton : Button
    {
        public StandardButton(string text, Vector2 position)
            :base(text, position, ButtonType.Normal)
        {

        }
        public StandardButton(string text, Vector2 position, string message)
            : base(text, position, ButtonType.Normal, message)
        {

        }

        public override void LoadContent(ContentManager content)
        {
            Texture = content.Load<Texture2D>("UI/Button");

            _font = content.Load<BitmapFont>("UI/fontNoOutline");

            SourceRect = new Rectangle(0, 0, 96, 32);
            Origin = new Vector2(48, 16);           

            base.LoadContent(content);

            SetTextPosition();
        }
    }
}
