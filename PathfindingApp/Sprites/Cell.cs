using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PathfindingApp.Sprites
{
    public class Cell : Sprite
    {
        public bool Highlighted;
        public Color HighlightColor;

        private readonly string _textureKey;
        private readonly Vector2 _mapPosition;

        private Effect flashEffect;
        private Effect outlineEffect;

        /// <summary>
        /// Sprite that represents map graphics
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="sourceRect"></param>
        /// <param name="position"></param>
        /// <param name="key"></param>
        public Cell(Vector2 position, Vector2 mapPosition, string textureKey)
            : base(position)
        {
            _mapPosition = mapPosition;
            _textureKey = textureKey;
        }

        public override void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("Graphics/Assets");

            switch (_textureKey)
            {
                case "wall":
                    _sourceRect = new Rectangle(16, 0, 16, 16);
                    break;
                case "floor":
                    _sourceRect = new Rectangle(0, 16, 16, 16);
                    break;
                default:
                    _sourceRect = new Rectangle(0, 16, 16, 16);
                    break;
            }
            flashEffect = content.Load<Effect>("Shaders/FlashToWhite");
            outlineEffect = content.Load<Effect>("Shaders/Outline");
            outlineEffect.Parameters["texelSize"].SetValue(new Vector2(1f / (_sourceRect.Width - 1f), 1f / (_sourceRect.Height - 1f)));
            outlineEffect.Parameters["outlineColor"].SetValue(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));

            _origin = new Vector2(_sourceRect.Width / 2, _sourceRect.Height / 2);
            Bounds = new Rectangle(_position.ToPoint() + _mapPosition.ToPoint(), new Point(_sourceRect.Width, _sourceRect.Height));
        }

        /// <summary>
        /// Highlights cell
        /// </summary>
        public void Highlight()
        {
            if (_textureKey != "floor") return;
            if (!Highlighted)
            {
                Highlighted = true;
                //_color = Color.Yellow;
                _effect = flashEffect;
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
    }
}
