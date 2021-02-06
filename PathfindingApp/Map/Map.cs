using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PathfindingApp.Settings;
using PathfindingApp.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathfindingApp
{
    /// <summary>
    /// This class reads in a text file and converts it into a 2D array that represents 
    /// the map
    /// </summary>
    public class Map : Sprite
    {
        public RenderTarget2D MapTarget;
        public char[][] MapKeys;
        public bool NeedsUpdate;

        private GraphicsDevice _graphics;
        private List<List<Cell>> _mapCells;  

        private Cursor _cursor;

        public Map(string filePath, Cursor cursor, GraphicsDevice graphics)
        {
            _graphics = graphics;
            _cursor = cursor;

            GenerateMapFromFile(filePath);

            _scale = Vector2.One;
            _origin = new Vector2(MapTarget.Bounds.Width / 2, MapTarget.Bounds.Height / 2);
        }

        /// <summary>
        /// Loads map data graphically
        /// </summary>
        /// <param name="content"></param>
        public void LoadMap(ContentManager content)
        {
            _position = new Vector2(ScreenSettings.GameWidth / 2 - MapTarget.Width / 2, ScreenSettings.GameHeight / 2 - MapTarget.Height / 2);
            Bounds = new Rectangle(_position.ToPoint(), new Point(MapTarget.Bounds.Width, MapTarget.Bounds.Height));

            _mapCells = new List<List<Cell>>();

            for (int i = 0; i < MapKeys.Length; i++)
            {
                List<Cell> rowCells = new List<Cell>();
                for (int j = 0; j < MapKeys[i].Length; j++)
                {
                    Vector2 position = new Vector2(j * 16, i * 16);
                    string key = MapKeys[i][j] == '*' ? "wall" : "floor";

                    Cell cell = new Cell(position, _position, key);
                    cell.LoadContent(content);

                    rowCells.Add(cell);
                }
                _mapCells.Add(rowCells);
            }
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < _mapCells.Count; i++)
            {
                for (int j = 0; j < _mapCells[i].Count; j++)
                {
                    if (Bounds.Intersects(_cursor.ClickBounds))
                    {
                        CheckForHighlight(_mapCells[i][j]);
                    }
                }
            }      
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            // If map has been changed, we need to redraw the map to the target before
            // we draw the target again
            if (NeedsUpdate)
            {
                DrawMapToTarget(spriteBatch);
            }
            Vector2 centerPosition = _position + _origin;
            spriteBatch.Draw(MapTarget, centerPosition, MapTarget.Bounds, Color.White, _rotation, _origin, _scale, SpriteEffects.None, 0.5f);
        }

        public void DrawMapToTarget(SpriteBatch spriteBatch)
        {
            _graphics.SetRenderTarget(MapTarget);
            _graphics.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    null,
                    null);

            for (int i = 0; i < _mapCells.Count; i++)
            {
                for (int j = 0; j < _mapCells[i].Count; j++)
                {
                    _mapCells[i][j].Draw(spriteBatch);
                }
            }

            spriteBatch.End();

            _graphics.SetRenderTarget(null);
        }

        public void CheckForHighlight(Cell cell)
        {
            if (cell.Bounds.Intersects(_cursor.ClickBounds))
            {
                if (!cell.Highlighted)
                {
                    cell.Highlight();
                    NeedsUpdate = true;
                }
            }
            else
            {
                if (cell.Highlighted)
                {
                    cell.UnHighlight();
                    NeedsUpdate = true;
                }
            }
        }

        /// <summary>
        /// Reads in map data from text file and converts it to 2D array
        /// </summary>
        /// <param name="filePath"></param>
        private void GenerateMapFromFile(string filePath)
        {
            try
            {
                string[] lines = System.IO.File.ReadAllLines(filePath);

                // Convert text data into 2d char array
                MapKeys = lines.Select(line => line.ToArray()).ToArray();
                InitializeTarget();
            }
            catch (Exception e)
            {
                // Log error
                System.Diagnostics.Debug.Write("Error with generating map from text file! ", e.ToString());

            }

        }
        /// <summary>
        /// Initialize render target to the size of the map
        /// </summary>
        private void InitializeTarget()
        {
            int height = MapKeys.Length * 16;
            int width = MapKeys[0].Length * 16;

            MapTarget = new RenderTarget2D(_graphics, width, height);
        }


    }
}
