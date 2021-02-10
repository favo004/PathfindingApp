using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PathfindingApp.UI
{
    /// <summary>
    /// Toggle button that allows the user to add certain cells to map
    /// </summary>
    public class ToggleButton : Button
    {
        public bool Toggled;    

        public ToggleButton(Vector2 position)
            : base(position, ButtonType.Toggle)
        {

        }
        public ToggleButton(Vector2 position, string message)
            : base(position, ButtonType.Toggle, message)
        {

        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
        public override void DrawMessage(SpriteBatch spriteBatch)
        {
            if (!Toggled)
                base.DrawMessage(spriteBatch);
        }

        public override void OnClick()
        {
            base.OnClick();

            if (!Toggled)
            {
                OnSelected();
            }
            else
            {
                OnDeselected();
            }

        }
        protected override void OnSelected()
        {
            Toggled = true;
            base.OnSelected();
        }
        protected override void OnDeselected()
        {
            Toggled = false;
            base.OnDeselected();
        }
        public override void Disable()
        {
            if (Toggled)
            {
                OnClick();
            }
            base.Disable();
        }
    }
}
