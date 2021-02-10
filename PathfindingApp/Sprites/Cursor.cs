using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System.Collections.Generic;

namespace PathfindingApp.Sprites
{
    /// <summary>
    /// Cursor functionality
    /// </summary>
    public class Cursor : Sprite
    {
        public Rectangle ClickBounds
        {
            get => new Rectangle(Position.ToPoint() + clickBoundsPositionOffset, _clickDimensions);
        }

        public Sprite HeldItem;

        public int PursuerMax;
        private int _pursuerCount;
        private string _pursuerCountText;
        private Vector2 _pursuerPositionOffset;
        private Color _pursuerTextColor;

        private BitmapFont _font;
        private Vector2 _heldItemPositionOffset = new Vector2(20, 24);
        private bool _holdingPursuer;

        private Point clickBoundsPositionOffset = new Point(7, 0);
        private Point _clickDimensions = new Point(1, 1);

        private Dictionary<string, Rectangle> _cursorRects = new Dictionary<string, Rectangle>
        {
            { "idle", new Rectangle(0, 0, 32, 32) },
            { "click", new Rectangle(0, 32, 32, 32) }
        };

        private MouseState _previousMouseState;
        private bool _clicked;
        private bool _heldClicked;

        private bool _rightClicked;

        public Cursor()
        {          
            Color = Color.White;
            Scale = Vector2.One;

            _previousMouseState = Mouse.GetState();
        }

        public override void LoadContent(ContentManager content)
        {
            Texture = content.Load<Texture2D>("Graphics/Cursor");
            _font = content.Load<BitmapFont>("UI/font");

            SourceRect = _cursorRects["idle"];

            UpdatePursuerCount(0);
            _pursuerPositionOffset = new Vector2(-5, 35);
            _pursuerTextColor = Color.White;
        }

        public override void Update(GameTime gameTime)
        {
            CheckForInput();
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (HeldItem != null)
            {
                spriteBatch.Draw(HeldItem.Texture, Position + _heldItemPositionOffset, HeldItem.SourceRect, HeldItem.Color * .8f);
            }

            if (_holdingPursuer)
            {
                spriteBatch.DrawString(_font, _pursuerCountText, Position + _pursuerPositionOffset, _pursuerTextColor);
            }
        }

        /// <summary>
        /// Returns if user has clicked left mouse button
        /// </summary>
        /// <returns></returns>
        public bool HasClicked()
        {
            return _clicked || _heldClicked;
        }
        /// <summary>
        /// Returns bool if user has clicked on item
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public bool HasClickedOn(Rectangle bounds)
        {
            return _clicked && ClickBounds.Intersects(bounds);
        }
        /// <summary>
        /// Returns bool if user has held click on item
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public bool HasHeldClickOn(Rectangle bounds)
        {
            return _heldClicked && ClickBounds.Intersects(bounds);
        }
        /// <summary>
        /// Returns bool if user has clicked the right mouse button
        /// </summary>
        /// <returns></returns>
        public bool HasRightClicked()
        {
            return _rightClicked;
        }

        /// <summary>
        /// Sets cursors held item to the param
        /// </summary>
        /// <param name="sprite"></param>
        public void SetHeldItem(Sprite sprite)
        {
            HeldItem = sprite;
            if(sprite.GetType() == typeof(Pursuer))
                _holdingPursuer = true;
        }
        /// <summary>
        /// Clears held item
        /// </summary>
        public void ClearHeldItem()
        {
            HeldItem = null;
            if (_holdingPursuer)
                _holdingPursuer = false; 
        }

        public void UpdatePursuerCount(int newCount)
        {
            _pursuerCount = newCount;
            _pursuerCountText = "(" + _pursuerCount + "/" + PursuerMax + ")";

            if (_pursuerCount == PursuerMax)
                _pursuerTextColor = Color.Red;
            else
                _pursuerTextColor = Color.White;
        }
        /// <summary>
        /// Checks for user input on the mouse
        /// </summary>
        private void CheckForInput()
        {
            MouseState mouseState = Mouse.GetState();
            Position = mouseState.Position.ToVector2();

            // Check left click and hold
            if(mouseState.LeftButton == ButtonState.Pressed &&
               _previousMouseState.LeftButton == ButtonState.Pressed)
            {
                SourceRect = _cursorRects["click"];
                _heldClicked = true;
            }
            else if(mouseState.LeftButton == ButtonState.Released &&
                    _previousMouseState.LeftButton == ButtonState.Pressed)
            {
                _clicked = true;
                _heldClicked = false;
            }
            else
            {
                SourceRect = _cursorRects["idle"];
                _clicked = false;
            }

            // Check right click
            if(mouseState.RightButton == ButtonState.Released &&
                _previousMouseState.RightButton == ButtonState.Pressed)
            {
                _rightClicked = true;
            }
            else
            {
                _rightClicked = false;
            }

            _previousMouseState = mouseState;
        }
    }
}
