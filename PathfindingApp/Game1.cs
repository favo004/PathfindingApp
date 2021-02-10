using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PathfindingApp.Managers;
using PathfindingApp.Settings;

namespace PathfindingApp
{
    /// <summary>
    /// Monogame app to demonstrate pathfinding algorithms
    /// </summary>
    public class Game1 : Game
    {
        public static Texture2D SquareTexture;

        public static float GameSpeed = 1f; // Adjust this for when we want to see up pathfinding visuals

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private AppManager _manager;
        private RenderTarget2D _gameTarget;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = ScreenSettings.WindowWidth;
            _graphics.PreferredBackBufferHeight = ScreenSettings.WindowHeight;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create pixel texture
            SquareTexture = new Texture2D(GraphicsDevice, 1, 1);
            Color[] data = new Color[1];
            data[0] = Color.White;
            SquareTexture.SetData(data);

            _manager = new AppManager(GraphicsDevice, _spriteBatch);
            _manager.LoadContent(Content);
            _manager.DrawMapTarget();

            _gameTarget = new RenderTarget2D(GraphicsDevice, ScreenSettings.GameWidth, ScreenSettings.GameHeight);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _manager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.WhiteSmoke);

            DrawToTarget();

            _spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp);
            _spriteBatch.Draw(_gameTarget, ScreenSettings.WindowRect, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawToTarget()
        {
            GraphicsDevice.SetRenderTarget(_gameTarget);
            GraphicsDevice.Clear(Color.DarkSlateGray);

            _spriteBatch.Begin(
                SpriteSortMode.Immediate, 
                BlendState.AlphaBlend, 
                SamplerState.PointClamp, 
                null, 
                null);

            _manager.Draw(_spriteBatch);

            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
        }
    }
}
