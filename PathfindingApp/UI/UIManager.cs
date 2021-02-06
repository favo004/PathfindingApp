using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PathfindingApp.Sprites;
using System.Collections.Generic;

namespace PathfindingApp.UI
{
    public class UIManager
    {
        private Cursor _cursor;

        private Button _btn1;
        private Button _btn2;
        private Button _btn3;

        private List<Button> _btns;

        public UIManager(Cursor cursor)
        {
            _cursor = cursor;

            _btns = new List<Button>();
            _btn1 = new Button("Testing", new Vector2(80, 100), ButtonType.Normal);
            _btn2 = new Button("Testing", new Vector2(80, 150), ButtonType.Normal);
            _btn3 = new Button("Testing", new Vector2(80, 200), ButtonType.Normal);
            _btns.Add(_btn1);
            _btns.Add(_btn2);
            _btns.Add(_btn3);
        }

        public void LoadContent(ContentManager content)
        {
            foreach (Button button in _btns)
            {
                button.LoadContent(content);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Button button in _btns)
            {
                if (button.Bounds.Intersects(_cursor.ClickBounds))
                {
                    button.Highlight();
                }
                else
                {
                    if (button.Highlighted)
                        button.UnHighlight();
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
        }
    }
}
