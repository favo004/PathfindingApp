using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PathfindingApp.Sprites;
using System.Collections.Generic;

namespace PathfindingApp.UI
{
    /// <summary>
    /// Manages all UI elements
    /// </summary>
    public class UIManager
    {
        private Cursor _cursor;
        private Map _map;
        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;

        private MapMenu _mapMenu;
        private MapCustomizeMenu _customizeMenu;

        private bool _mapRunning;

        public UIManager(Cursor cursor, GraphicsDevice graphics, SpriteBatch spriteBatch, Map map)
        {
            _cursor = cursor;
            _map = map;
            _graphics = graphics;
            _spriteBatch = spriteBatch;

            _mapMenu = new MapMenu(graphics, spriteBatch, cursor, map);
            _customizeMenu = new MapCustomizeMenu(graphics, spriteBatch, cursor, map);
        }

        public void LoadContent(ContentManager content)
        {
            _mapMenu.LoadContent(content);
            _customizeMenu.LoadContent(content);
        }

        public void Update(GameTime gameTime)
        {
            _mapMenu.Update(gameTime);
            _customizeMenu.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _mapMenu.Draw(spriteBatch);
            _customizeMenu.Draw(spriteBatch);
        }

        private bool CheckMapRunning()
        {
            if(_map.Running && !_mapRunning)
            {
                _mapRunning = true;
                _customizeMenu.DisableButtons();
            }

            return false;
        }
    }
}
