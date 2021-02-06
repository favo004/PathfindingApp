using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PathfindingApp.Sprites;
using PathfindingApp.UI;

namespace PathfindingApp.Managers
{
    /// <summary>
    /// Manages how the application runs
    /// </summary>
    public class AppManager
    {
        private Config _config;

        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;

        private Map _map;
        private Cursor _cursor;

        private UIManager _uIManager;

        public AppManager(GraphicsDevice graphics, SpriteBatch spriteBatch)
        {
            _config = new Config();
            _graphics = graphics;
            _spriteBatch = spriteBatch;

            _cursor = new Cursor();
            _map = new Map(_config.MapPath, _cursor, _graphics);
            _uIManager = new UIManager(_cursor);
        }

        /// <summary>
        /// Loads graphical data for map, sets up map target
        /// </summary>
        /// <param name="content"></param>
        public void LoadContent(ContentManager content)
        {
            _map.LoadMap(content);
            _cursor.LoadContent(content);
            _uIManager.LoadContent(content);
        }
        public void DrawMapTarget()
        {
            _map.DrawMapToTarget(_spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            _map.Update(gameTime);
            _cursor.Update(gameTime);
            _uIManager.Update(gameTime);

            if (_map.NeedsUpdate)
            {
                _map.DrawMapToTarget(_spriteBatch);
                _map.NeedsUpdate = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_map.NeedsUpdate)
            {
                spriteBatch.End();

                _map.DrawMapToTarget(spriteBatch);
                _map.NeedsUpdate = false;

                spriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    DepthStencilState.Default,
                    RasterizerState.CullNone);
            }

            _map.Draw(spriteBatch);          
            _uIManager.Draw(spriteBatch);

            _cursor.Draw(spriteBatch);
        }
    }

    /// <summary>
    /// Settings that configure map details
    /// </summary>
    public class Config
    {
        private string _mapPath = @"C:\Users\JFavo\source\repos\PathfindingApp\PathfindingApp\MapData\TestMap.txt";
        public string MapPath
        {
            get => _mapPath;
        }

    }
}
