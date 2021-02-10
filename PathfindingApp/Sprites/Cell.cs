using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace PathfindingApp.Sprites
{
    public enum CellType
    {
        Wall,
        Floor
    }
    public class Cell : Sprite
    {
        public bool Highlighted;

        public List<Color> HighlightedColors;
        public Color HighlightedColor
        {
            get
            {
                Color highlight = Color.White;

                foreach (Color color in HighlightedColors)
                {
                    highlight = new Color((highlight.R + color.R) / 2, (highlight.G + color.B)/2, (highlight.B + color.B) / 2);
                }

                return highlight;
            }
        }

        public CellType CellType;

        private readonly string _textureKey;
        private readonly Vector2 _mapPosition;

        private Effect flashEffect;
        private Effect outlineEffect;

        public Cell()
        {

        }
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
            Texture = content.Load<Texture2D>("Graphics/Assets");

            switch (_textureKey)
            {
                case "wall":
                    SourceRect = new Rectangle(16, 0, 16, 16);
                    CellType = CellType.Wall;
                    break;
                case "floor":
                    SourceRect = new Rectangle(0, 16, 16, 16);
                    CellType = CellType.Floor;
                    break;
                default:
                    SourceRect = new Rectangle(0, 16, 16, 16);
                    CellType = CellType.Floor;
                    break;
            }
            flashEffect = content.Load<Effect>("Shaders/FlashToWhite");
            outlineEffect = content.Load<Effect>("Shaders/Outline");
            outlineEffect.Parameters["texelSize"].SetValue(new Vector2(1f / (SourceRect.Width - 1f), 1f / (SourceRect.Height - 1f)));
            outlineEffect.Parameters["outlineColor"].SetValue(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));

            Origin = new Vector2(SourceRect.Width / 2, SourceRect.Height / 2);
            Bounds = new Rectangle(Position.ToPoint() + _mapPosition.ToPoint(), new Point(SourceRect.Width, SourceRect.Height));
        }

        public override object Copy()
        {
            Cell cell = new Cell(Position, _mapPosition, _textureKey);
            cell.SetAttributes(Texture, SourceRect, Scale, Rotation, Color, Bounds, CellType);
            return cell;
        }
        public void SetAttributes(Texture2D texture, Rectangle sourceRect, Vector2 scale, float rotation, Color color, Rectangle bounds, CellType cellType)
        {
            base.SetAttributes(texture, sourceRect, scale, rotation, color, bounds);
            CellType = cellType;
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
                Color = Color.Yellow;
            }
        }
        public void UnHighlight()
        {
            if (Highlighted)
            {
                Highlighted = false;
                Color = Color.White;
            }
        }

        public void ChangeCellType(CellType type)
        {
            CellType = type;
            switch (type)
            {
                case CellType.Floor:
                    SourceRect = new Rectangle(0, 16, 16, 16);
                    break;
                case CellType.Wall:
                    SourceRect = new Rectangle(16, 0, 16, 16);
                    break;
            }
        }
    }
}
