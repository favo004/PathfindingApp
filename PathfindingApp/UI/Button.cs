using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PathfindingApp.Sprites;

namespace PathfindingApp.UI
{
    public enum ButtonType
    {
        Normal,
        Toggle,
        Dropdown
    }
    public class Button : Sprite
    {
        public ButtonType ButtonType { get; private set; }
        public bool Highlighted { get; private set; }

        string _text;
        Vector2 _textPosition;

        SpriteFont _font;

        private Effect outlineEffect;  

        public Button(string text, Vector2 position, ButtonType type)
        {
            _text = text;

            _position = position;
            _color = Color.White;
            _scale = Vector2.One;

            ButtonType = type;
        }

        public override void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("UI/Button");
            _font = content.Load<SpriteFont>("UI/Font");

            _sourceRect = new Rectangle(0, 0, 96, 32);
            _origin = new Vector2(32, 16);

            outlineEffect = content.Load<Effect>("Shaders/Outline");
            outlineEffect.Parameters["texelSize"].SetValue(new Vector2(1f / (_sourceRect.Width - 1f), 1f / (_sourceRect.Height - 1f)));
            outlineEffect.Parameters["outlineColor"].SetValue(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));

            Bounds = new Rectangle(_position.ToPoint(), new Point(_sourceRect.Width, _sourceRect.Height));

            SetTextPosition();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap);
            spriteBatch.DrawString(_font, _text, _textPosition, Color.Black);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null,
                null);
        }

        /// <summary>
        /// Highlights cell
        /// </summary>
        public void Highlight()
        {
            if (!Highlighted)
            {
                Highlighted = true;
                //_color = Color.Yellow;
                _effect = outlineEffect;
            }
        }
        public void UnHighlight()
        {
            if (Highlighted)
            {
                Highlighted = false;
                //_color = Color.White;
                _effect = null;
            }
        }

        private void OnClick()
        {
            
        }

        private void SetTextPosition()
        {
            Vector2 textSize = _font.MeasureString(_text);
            float textX = _sourceRect.Width / 2 - textSize.X / 2;
            float textY = _sourceRect.Height / 2 - textSize.Y / 2;
            _textPosition = new Vector2(_position.X + textX, _position.Y + textY);
        }
    }
}
