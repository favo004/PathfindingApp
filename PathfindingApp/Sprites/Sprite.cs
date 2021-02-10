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
        public virtual Point Coordinates { get; set; }
        /// <summary>
        /// The area that the sprite consumes
        /// </summary>
        public virtual Rectangle Bounds { get; set; }

        public Texture2D Texture { get; protected set; }
        public Rectangle SourceRect { get; protected set; }
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; protected set; }
        public Vector2 Scale { get; protected set; }
        public float Rotation { get; protected set; }
        private Color _color;
        public Color Color
        {
            get
            {
                return _color * Alpha;
            }
            set
            {
                _color = value;
            }
        }
        public float Alpha { get; set; }
        protected Effect _effect;

        public Sprite()
        {
            Scale = Vector2.One;
            Rotation = 0f;
            Color = Color.White;
            Alpha = 1f;
        }
        public Sprite(Vector2 position)
            : this()
        {
            Position = position;
        }

        /// <summary>
        /// Loads graphical data
        /// </summary>
        /// <param name="content"></param>
        public virtual void LoadContent(ContentManager content)
        {

        }
        public virtual void LoadContent(ContentManager content, GraphicsDevice graphics, SpriteBatch spriteBatch)
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
            Vector2 centeredPosition = Position + Origin;

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

            spriteBatch.Draw(Texture, centeredPosition, SourceRect, Color, Rotation, Origin, Scale, SpriteEffects.None, .5f);


            if (_effect != null)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null,
                null);
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch, RenderTarget2D target = null)
        {
            // Add origin to position to draw sprite to center point
            Vector2 centeredPosition = Position + Origin;

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

            if (target == null)
            {
                spriteBatch.Draw(Texture, centeredPosition, SourceRect, Color, Rotation, Origin, Scale, SpriteEffects.None, .5f);
            }
            else
            {
                spriteBatch.Draw(target, centeredPosition, SourceRect, Color, Rotation, Origin, Scale, SpriteEffects.None, .5f);
            }

            if (_effect != null)
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
        /// Returns a copy of sprite
        /// </summary>
        /// <returns></returns>
        public virtual object Copy()
        {
            Sprite copy = new Sprite(Position);
            copy.SetAttributes(Texture, SourceRect, Scale, Rotation, Color, Bounds);
            return copy;
        }
        public virtual void SetAttributes(Texture2D texture, Rectangle sourceRect, Vector2 scale, float rotation, Color color, Rectangle bounds)
        {
            Texture = texture;
            SourceRect = sourceRect;
            Origin = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2);
            Scale = scale;
            Rotation = rotation;
            Color = color;
            Bounds = bounds;
        }

        /// <summary>
        /// Updates the bounds of the sprite based on position
        /// </summary>
        private void UpdateBounds()
        { 
            Bounds = new Rectangle(Position.ToPoint(), new Point(SourceRect.Width, SourceRect.Height));
        }
    }
}
