using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathfindingApp.Sprites
{
    /// <summary>
    /// 2D graphical image
    /// </summary>
    public class Sprite
    {
        /// <summary>
        /// Matrix coordinates of sprite
        /// Used if sprite is within the map
        /// </summary>
        public Point Coordinates { get; set; }
        /// <summary>
        /// The area that the sprite consumes
        /// </summary>
        public virtual Rectangle Bounds { get; protected set; }

        protected Texture2D _texture;
        protected Rectangle _sourceRect;
        protected Vector2 _position;
        protected Vector2 _origin;
        protected Vector2 _scale;
        protected float _rotation;
        protected Color _color;
        protected Effect _effect;

        public Sprite()
        {

        }
        public Sprite(Vector2 position)
        {
            _position = position;

            _scale = Vector2.One;
            _rotation = 0f;
            _color = Color.White;
        }

        /// <summary>
        /// Loads graphical data
        /// </summary>
        /// <param name="content"></param>
        public virtual void LoadContent(ContentManager content)
        {

        }

        /// <summary>
        /// This is called within the game loop when we want the sprite to be updated
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {

        }
        /// <summary>
        /// Draws sprite to render target
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // Add origin to position to draw sprite to center point
            Vector2 centeredPosition = _position + _origin;

            if (_effect != null)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null,
                null,
                _effect);
            }

            spriteBatch.Draw(_texture, centeredPosition, _sourceRect, _color, _rotation, _origin, _scale, SpriteEffects.None, .5f);
            
            if(_effect != null)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null,
                null);
            }
        }

        /// <summary>
        /// Updates the bounds of the sprite based on position
        /// </summary>
        private void UpdateBounds()
        { 
            Bounds = new Rectangle(_position.ToPoint(), new Point(_sourceRect.Width, _sourceRect.Height));
        }
    }
}
