using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace PathfindingApp.Sprites
{
    public class Cursor : Sprite
    {
        public Rectangle ClickBounds
        {
            get => new Rectangle(_position.ToPoint() + clickBoundsPositionOffset, _clickDimensions);
        }

        private Point clickBoundsPositionOffset = new Point(7, 0);
        private Point _clickDimensions = new Point(1, 1);

        private Dictionary<string, Rectangle> _cursorRects = new Dictionary<string, Rectangle>
        {
            { "idle", new Rectangle(0, 0, 32, 32) },
            { "click", new Rectangle(0, 32, 32, 32) }
        };

        private MouseState _previousMouseState;
        private bool _clicked;

        public Cursor()
        {          
            _color = Color.White;
            _scale = Vector2.One;

            _previousMouseState = Mouse.GetState();
        }

        public void SetPosition(Vector2 newPos)
        {
            _position = newPos;
        }

        public override void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("Graphics/Cursor");

            _sourceRect = _cursorRects["idle"];
        }

        public override void Update(GameTime gameTime)
        {
            CheckForInput();
            base.Update(gameTime);
        }

        /// <summary>
        /// Checks to see if cursor has clicked on the bounds
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public bool HasClickOn(Rectangle bounds)
        {
            return _clicked && ClickBounds.Intersects(bounds);
        }

        private void CheckForInput()
        {
            MouseState mouseState = Mouse.GetState();
            SetPosition(mouseState.Position.ToVector2());

            if(mouseState.LeftButton == ButtonState.Pressed &&
               _previousMouseState.LeftButton == ButtonState.Pressed)
            {
                _sourceRect = _cursorRects["click"];
                _clicked = true;
            }
            else
            {
                _sourceRect = _cursorRects["idle"];
                _clicked = false;
            }

            _previousMouseState = mouseState;
        }
    }
}
