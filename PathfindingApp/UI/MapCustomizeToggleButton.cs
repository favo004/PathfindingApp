using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using PathfindingApp.Sprites;

namespace PathfindingApp.UI
{
    public class MapCustomizeToggleButton : ToggleButton
    {
        public RenderTarget2D RenderTarget;

        private Sprite _menuItem; // Item to be selected for map customization
        private Vector2 _menuPosition; // Position of the parent menu on screen
        private Vector2 _menuItemPosition; // Position of button based off of menu position

        public MapCustomizeToggleButton(Vector2 menuPosition, Vector2 menuItemPosition, Sprite menuItem)
            : base(Vector2.Zero)
        {
            _menuItem = menuItem;
            _menuPosition = menuPosition;
            _menuItemPosition = menuItemPosition;
        }
        public MapCustomizeToggleButton(Vector2 menuPosition, Vector2 menuItemPosition, Sprite menuItem, string message)
            : base(Vector2.Zero, message)
        {
            _menuItem = menuItem;
            _menuItemPosition = menuItemPosition;
        }

        public override void LoadContent(ContentManager content, GraphicsDevice graphics, SpriteBatch spriteBatch)
        {
            Texture = content.Load<Texture2D>("UI/MenuItemBackground");

            if (HasMessage)
                _font = content.Load<BitmapFont>("UI/fontNoOutline");

            SourceRect = new Rectangle(0, 0, 32, 32);
            Origin = new Vector2(16, 16);
            Color = Color.White * .8f;

            _menuItem.LoadContent(content);

            RenderTarget = new RenderTarget2D(graphics, 32, 32);
            DrawTarget(graphics, spriteBatch);

            Position = _menuItemPosition - Origin; // Set draw position to center of image

            Bounds = RenderTarget.Bounds;

            base.LoadContent(content);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch, RenderTarget);
        }
        public void DrawTarget(GraphicsDevice graphics, SpriteBatch spriteBatch)
        {
            graphics.SetRenderTarget(RenderTarget);
            graphics.Clear(Color.Transparent);

            spriteBatch.Begin();

            base.Draw(spriteBatch);
            _menuItem.Draw(spriteBatch);

            spriteBatch.End();

            graphics.SetRenderTarget(null);
        }

        /// <summary>
        /// Returns copy of menus item
        /// </summary>
        /// <returns></returns>
        public Sprite GetItem()
        {
            return _menuItem;
        }

    }
}
