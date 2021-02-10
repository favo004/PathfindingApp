using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using PathfindingApp.Sprites;
using System.Collections.Generic;

namespace PathfindingApp.UI
{
    public class MapMenu
    {
        private Cursor _cursor;
        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;
        private Map _map;

        private BitmapFont _font;
        private string _errorMessage = "";
        private Vector2 _errorMessagePosition = new Vector2(5, 10);

        private StandardButton _runButton;
        private StandardButton _resetButton;

        private MapCustomizeToggleButton _pauseButton;
        private MapCustomizeToggleButton _speedOneButton;
        private MapCustomizeToggleButton _speedTwoButton;
        private List<MapCustomizeToggleButton> _toggleButtons;

        private List<Button> _btns;

        public MapMenu(GraphicsDevice graphics, SpriteBatch spriteBatch, Cursor cursor, Map map)
        {
            _cursor = cursor;
            _map = map;

            _btns = new List<Button>();
            _runButton = new StandardButton("Run", new Vector2(17, 110), "Runs the pathfinding algorithms");
            _runButton.ClickCallBack = RunMap;
            _resetButton = new StandardButton("Reset", new Vector2(17, 160), "Resets map back to original");
            _resetButton.ClickCallBack = ClearMap;

            _btns.Add(_runButton);
            _btns.Add(_resetButton);

            OptionIcon pause = new OptionIcon(OptionType.Pause);
            pause.Position = new Vector2(8, 8);
            _pauseButton = new MapCustomizeToggleButton(Vector2.Zero, new Vector2(25, 80), pause, "Pause");
            _pauseButton.ClickCallBack = PauseMap;

            OptionIcon speedOne = new OptionIcon(OptionType.SpeedOne);
            speedOne.Position = new Vector2(8, 8);
            _speedOneButton = new MapCustomizeToggleButton(Vector2.Zero, new Vector2(65, 80), speedOne, "Normal Speed");
            _speedOneButton.ClickCallBack = MapSpeedOne;

            OptionIcon speedTwo = new OptionIcon(OptionType.SpeedTwo);
            speedTwo.Position = new Vector2(8, 8);
            _speedTwoButton = new MapCustomizeToggleButton(Vector2.Zero, new Vector2(105, 80), speedTwo, "2x Speed");
            _speedTwoButton.ClickCallBack = MapSpeedTwo;

            _btns.Add(_pauseButton);
            _btns.Add(_speedOneButton);
            _btns.Add(_speedTwoButton);

            _toggleButtons = new List<MapCustomizeToggleButton>();
            _toggleButtons.Add(_pauseButton);
            _toggleButtons.Add(_speedOneButton);
            _toggleButtons.Add(_speedTwoButton);

            _graphics = graphics;
            _spriteBatch = spriteBatch;


        }

        public void LoadContent(ContentManager content)
        {
            _font = content.Load<BitmapFont>("UI/fontNoOutline");
            foreach (var button in _btns)
            {
                if (button.GetType() == typeof(MapCustomizeToggleButton))
                    button.LoadContent(content, _graphics, _spriteBatch);
                if (button.GetType() == typeof(StandardButton))
                    button.LoadContent(content);

                if (button.HasMessage)
                {
                    button.SetMessage(_graphics, _spriteBatch);
                }
            }

            DisableToggles();

            
        }

        public void Update(GameTime gameTime)
        {
            foreach (Button button in _btns)
            {
                if (button.Enabled)
                {
                    if (button.Bounds.Intersects(_cursor.ClickBounds))
                    {
                        button.Highlight();
                        if (_cursor.HasClickedOn(button.Bounds))
                        {                 
                            if (button.ButtonType == ButtonType.Toggle)
                            {
                                // Check to see if another toggle button is selected
                                ToggleButton btn = IsOtherButtonToggled();
                                if(btn != null && button != btn)
                                {
                                    btn.OnClick();
                                }
                            }
                            button.OnClick();
                        }
                    }
                    else
                    {
                        if (button.Highlighted)
                        {
                            if(button.ButtonType == ButtonType.Toggle)
                            {
                                ToggleButton btn = button as ToggleButton;

                                if (!btn.Toggled)
                                    button.UnHighlight();
                            }
                            else
                                button.UnHighlight();
                        }
                    }
                }

                button.Update(gameTime);
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Button button in _btns)
            {
                button.Draw(spriteBatch);
            }
            foreach (Button button in _btns)
            {
                if (button.HasMessage)
                {
                    button.DrawMessage(spriteBatch);
                }
            }

            if(_errorMessage != "")
            {
                spriteBatch.DrawString(_font, _errorMessage, _errorMessagePosition, Color.Red);
            }
        }

        private void RunMap()
        {
            string msg = _map.ToggleRun();

            _errorMessage = FormatErrorMessage(msg);


            if (_errorMessage == "")
            {
                if (_map.Running)
                {
                    _runButton.ChangeText("Stop");
                    EnableToggles();
                }
                else
                {
                    _runButton.ChangeText("Run");
                    DisableToggles();
                }
            }
        }
        private void ClearMap()
        {
            if (_map.Running)
                RunMap();

            _map.Reset();
            _cursor.UpdatePursuerCount(0);
            _errorMessage = "";
        }
        private void PauseMap()
        {
            _map.Paused = !_map.Paused;
        }
        private void MapSpeedOne()
        {
            Game1.GameSpeed = 1f;
        }
        private void MapSpeedTwo()
        {
            Game1.GameSpeed = 2f;
        }

        private void DisableToggles()
        {
            _pauseButton.Disable();
            _speedOneButton.Disable();
            _speedTwoButton.Disable();
        }
        private void EnableToggles()
        {
            _pauseButton.Enable();
            _speedOneButton.Enable();
            _speedTwoButton.Enable();

            _speedOneButton.OnClick();
        }
        private ToggleButton IsOtherButtonToggled()
        {
            ToggleButton toggled = _toggleButtons.Find(b => b.Toggled);

            return toggled;
        }

        private string FormatErrorMessage(string msg)
        {
            if(msg != "")
            {
                string[] split = msg.Split(' ');
                msg = "";
                for (int i = 0; i < split.Length; i++)
                {
                    if (i == split.Length / 2)
                        msg += "\n";
                    msg += split[i] + " ";
                }
            }

            return msg;
        }
    }
}
