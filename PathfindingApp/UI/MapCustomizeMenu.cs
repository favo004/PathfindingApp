using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using PathfindingApp.Sprites;
using System.Collections.Generic;

namespace PathfindingApp.UI
{
    /// <summary>
    /// List of items that can be added to map
    /// </summary>
    public class MapCustomizeMenu
    {
        public RenderTarget2D RenderTarget;
        public bool NeedsUpdate;

        public int NumOfPursuers = 0;

        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;
        private Cursor _cursor;
        private Map _map;

        private BitmapFont _font;
        private Texture2D _backgroundTexture;
        private Vector2 _position;
        private List<MapCustomizeToggleButton> _menuButtons;
        private bool _btnsEnabled;

        private Vector2 _pursuerCountPosition;

        public MapCustomizeMenu(GraphicsDevice graphics, SpriteBatch spriteBatch, Cursor cursor, Map map)
        {
            _position = new Vector2(515, 5);

            _graphics = graphics;
            _spriteBatch = spriteBatch;
            _cursor = cursor;

            _menuButtons = new List<MapCustomizeToggleButton>();
            InitializeButtons();

            _pursuerCountPosition = new Vector2(540, 150);

            _map = map;

            EnableButtons();
        }
        public void LoadContent(ContentManager content)
        {
            _font = content.Load<BitmapFont>("UI/FontNoOutline");
            _backgroundTexture = Game1.SquareTexture;
            RenderTarget = new RenderTarget2D(_graphics, 120, 350);
            DrawTarget();

            foreach (var button in _menuButtons)
            {
                button.LoadContent(content, _graphics, _spriteBatch);
                if (button.HasMessage)
                {
                    button.SetMessage(_graphics, _spriteBatch);
                }
            }
        }
        public void Update(GameTime gameTime)
        {
            if (_map.Running) 
            {
                if(_btnsEnabled)
                    DisableButtons();
            }
            else
            {
                if (!_btnsEnabled)
                    EnableButtons();
            }

            foreach (var button in _menuButtons)
            {
                if (button.Enabled)
                {
                    if (button.Bounds.Intersects(_cursor.ClickBounds))
                    {
                        if (!button.Toggled)
                            button.Highlight();

                        if (_cursor.HasClickedOn(button.Bounds))
                        {
                            // If another button is toggled, we untoggle it
                            ToggleButton prevToggled = IsOtherButtonToggled();
                            if (prevToggled != null && prevToggled != button)
                            {
                                prevToggled.OnClick();
                            }

                            button.OnClick();
                            if (_cursor.HeldItem != null)
                                _cursor.ClearHeldItem();

                            // If button has been toggled, we have the cursor hold the menu item
                            if (button.Toggled)
                            {
                                _cursor.SetHeldItem(button.GetItem());
                            }
                        }
                    }
                    else
                    {
                        if (button.Highlighted && !button.Toggled)
                            button.UnHighlight();
                    }
                }
                else
                {
                    if(button.GetItem() == _cursor.HeldItem)
                    {
                        _cursor.ClearHeldItem();
                    }
                }

                // Check if user right clicks while menu button is toggled
                // If they have, we cancel toggle
                if (button.Toggled)
                {
                    if (_cursor.HasRightClicked())
                    {
                        button.OnClick();
                        _cursor.ClearHeldItem();
                    }
                }

                button.Update(gameTime);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(RenderTarget, _position, Color.White);

            foreach (var button in _menuButtons)
            {
                button.Draw(spriteBatch);
            }

            
            foreach (var button in _menuButtons)
            {
                if (button.HasMessage)
                {
                    if (button.Toggled) continue;

                    button.DrawMessage(spriteBatch);
                }

            }
        }
        public void DrawTarget()
        {
            _graphics.SetRenderTarget(RenderTarget);
            _graphics.Clear(Color.Transparent);

            _spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp);

            _spriteBatch.Draw(_backgroundTexture, RenderTarget.Bounds, Color.Black * .3f);

            _spriteBatch.DrawString(_font, "Customization", new Vector2(5, 10), Color.White);
            _spriteBatch.DrawString(_font, "Pursuers", new Vector2(5, 70), Color.White);

            _spriteBatch.End();

            _graphics.SetRenderTarget(null);
        }

        public void DisableButtons()
        {
            _btnsEnabled = false;
            foreach (var btn in _menuButtons)
            {
                btn.Disable();
            }
        }
        public void EnableButtons()
        {
            _btnsEnabled = true;
            foreach (var btn in _menuButtons)
            {
                btn.Enable();
            }
        }

        private void InitializeButtons()
        {
            Cell wallCell = new Cell(new Vector2(8, 8), Vector2.Zero, "wall");
            Cell floorCell = new Cell(new Vector2(8, 8), Vector2.Zero, "floor");
            Vector2 wallPos = new Vector2(550, 50);
            Vector2 floorPos = new Vector2(600, 50);
            _menuButtons.Add(new MapCustomizeToggleButton(_position, wallPos, wallCell, "Wall"));
            _menuButtons.Add(new MapCustomizeToggleButton(_position, floorPos, floorCell, "Floor"));

            Pursuer bfsPursuer = new Pursuer(new Vector2(8,8), PursueAlgorithm.BFS);
            Pursuer dfsPursuer = new Pursuer(new Vector2(8, 8), PursueAlgorithm.DFS);
            Pursuer aStarPursuer = new Pursuer(new Vector2(8, 8), PursueAlgorithm.AStar);
            Vector2 bfsPos = new Vector2(540, 110);
            Vector2 dfsPos = new Vector2(575, 110);
            Vector2 aStarPos = new Vector2(610, 110);
            _menuButtons.Add(new MapCustomizeToggleButton(_position, bfsPos, bfsPursuer, "Breadth First Search"));
            _menuButtons.Add(new MapCustomizeToggleButton(_position, dfsPos, dfsPursuer, "Depth First Search"));
            _menuButtons.Add(new MapCustomizeToggleButton(_position, aStarPos, aStarPursuer, "A* Search"));

            Goal goal = new Goal();
            goal.SetAttributes(null, new Rectangle(0, 0, 16, 16), Vector2.One, 0f, Color.White, new Rectangle(0, 0, 16, 16));
            goal.Position = new Vector2(8, 8);
            _menuButtons.Add(new MapCustomizeToggleButton(_position, new Vector2(550, 180), goal, "Goal"));

            Eraser eraser = new Eraser();
            eraser.SetAttributes(null, new Rectangle(0, 0, 16, 16), Vector2.One, 0f, Color.White, new Rectangle(0, 0, 16, 16));
            eraser.Position = new Vector2(8, 8);
            _menuButtons.Add(new MapCustomizeToggleButton(_position, new Vector2(600, 180), eraser, "Delete"));
        }
        private ToggleButton IsOtherButtonToggled()
        {
            ToggleButton toggled = _menuButtons.Find(b => b.Toggled);

            return toggled;
        }
    }
}
