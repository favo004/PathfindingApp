using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using PathfindingApp.Settings;
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
        public bool HasMessage { get; private set; }

        public delegate void ClickCallback(); // Event for click
        public ClickCallback ClickCallBack;

        public bool Enabled;

        protected string _text;
        protected Vector2 _textPosition;

        protected BitmapFont _font;

        protected Effect _outlineEffect;

        protected string _message;
        protected bool _showMessage;
        protected RenderTarget2D _messageTarget;
        protected Vector2 _messagePositionOffset;
        protected Vector2 _messageSpacing = new Vector2(10, 10);

        protected Vector2 _messagePosition;

        public Button(Vector2 position, ButtonType type)
        {
            Position = position;
            Color = Color.White;
            Scale = Vector2.One;

            ButtonType = type;

            Enabled = true;
        }
        public Button(Vector2 position, ButtonType type, string message)
            :this (position, type)
        {
            HasMessage = true;
            _message = message;
        }
        public Button(string text, Vector2 position, ButtonType type)
            : this(position, type)
        {
            _text = text;
        }
        public Button(string text, Vector2 position, ButtonType type, string message)
            :this(text, position, type)
        {
            HasMessage = true;
            _message = message;
        }

        public override void LoadContent(ContentManager content)
        {
            _outlineEffect = content.Load<Effect>("Shaders/Outline");
            _outlineEffect.Parameters["texelSize"].SetValue(new Vector2(1f / (SourceRect.Width - 1f), 1f / (SourceRect.Height - 1f)));
            _outlineEffect.Parameters["outlineColor"].SetValue(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));

            Bounds = new Rectangle(Position.ToPoint(), new Point(SourceRect.Width, SourceRect.Height));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if(_font != null && _text != null)
                spriteBatch.DrawString(_font, _text, _textPosition, Color.Black);
        }
        public void DrawMessage(SpriteBatch spriteBatch)
        {
            if (_showMessage)
            {
                spriteBatch.Draw(_messageTarget, _messagePosition, Color.White);
            }
        }

        /// <summary>
        /// Sets hover message for button and draws it to the message target
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="spriteBatch"></param>
        public virtual void SetMessage(GraphicsDevice graphics, SpriteBatch spriteBatch)
        {
            Vector2 size = _font.MeasureString(_message);
            size += _messageSpacing;
            _messageTarget = new RenderTarget2D(graphics, (int)size.X, (int)size.Y);

            graphics.SetRenderTarget(_messageTarget);
            graphics.Clear(Color.Transparent);

            spriteBatch.Begin();
            spriteBatch.Draw(Game1.SquareTexture, _messageTarget.Bounds, Color.Black * .7f);
            spriteBatch.DrawString(_font, _message, _messageSpacing / 2, Color.White);
            spriteBatch.End();

            graphics.SetRenderTarget(null);

            _messagePosition = new Vector2(Position.X + SourceRect.Width/2 - _messageTarget.Width/2, Position.Y - _messageTarget.Height);
            if (_messagePosition.X < 0) _messagePosition = new Vector2(0, _messagePosition.Y);
            if (_messagePosition.X + _messageTarget.Width > ScreenSettings.GameWidth) _messagePosition = new Vector2(ScreenSettings.GameWidth - _messageTarget.Width, _messagePosition.Y);
        }

        public virtual void SetClickCallBack()
        {

        }

        /// <summary>
        /// Sets text for the button
        /// </summary>
        public virtual void SetTextPosition()
        {
            Vector2 textSize = _font.MeasureString(_text);
            float textX = SourceRect.Width / 2 - textSize.X / 2;
            float textY = SourceRect.Height / 2 - textSize.Y / 2;
            _textPosition = new Vector2(Position.X + textX, Position.Y + textY);
        }
        /// <summary>
        /// Changes button text
        /// </summary>
        /// <param name="newText">New text for button</param>
        public void ChangeText(string newText)
        {
            _text = newText;
            SetTextPosition();
        }

        /// <summary>
        /// Highlights button
        /// </summary>
        public void Highlight()
        {
            if (!Highlighted)
            {
                Highlighted = true;
                Color = Color.Gainsboro;
                _effect = _outlineEffect;

                if(HasMessage)
                    _showMessage = true;
            }
        }
        /// <summary>
        /// Returns button to normal
        /// </summary>
        public void UnHighlight()
        {
            if (Highlighted)
            {
                Highlighted = false;
                Color = Color.White;
                _effect = null;

                if (HasMessage)
                    _showMessage = false;
            }
        }

        /// <summary>
        /// Event for click
        /// </summary>
        public virtual void OnClick()
        {
            ClickCallBack?.Invoke();
        }
        /// <summary>
        /// Disabled button from being clickable
        /// </summary>
        public virtual void Disable()
        {
            Color = new Color(180, 180, 180);
            Enabled = false;
        }     
        /// <summary>
        /// Enables button
        /// </summary>
        public virtual void Enable()
        {
            Color = Color.White;
            Enabled = true;
        }

        // Toggle button stuff
        /// <summary>
        /// Event for toggled button
        /// </summary>
        protected virtual void OnSelected()
        {
            _outlineEffect.Parameters["outlineColor"].SetValue(new Vector4(1f, 1f, 1f, 1f));
            Highlight();
        }
        /// <summary>
        /// Event for untoggled button
        /// </summary>
        protected virtual void OnDeselected()
        {
            _outlineEffect.Parameters["outlineColor"].SetValue(new Vector4(1f, 1f, 1f, 1f));
            UnHighlight();
        }


    }
}
