using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class BackButton : Button
    {
        Screen screen;
        bool enable;

        public BackButton(string text, Screen screen, bool enable) : base()
        {
            this.text = text;
            this.screen = screen;
            this.enable = enable;

            // Calculate offset for BottomRight anchor
            // Original pos: GameSize.X - (Width + 20), GameSize.Y - (Height * 2)
            // Offset X = Width + 20
            // Offset Y = Height * 2
            float width = Globals.GameFont.MeasureString(text).X;
            float height = Globals.GameFont.MeasureString(text).Y;
            
            // Create rect and clickableArea before setting anchor
            Vector2 size = new Vector2(width, height);
            this.rect = new Rectangle(0, 0, (int)size.X, (int)size.Y);
            this.clickableArea = new ClickableArea(this.rect);
            
            SetAnchor(Anchor.BottomRight, new Vector2(width + 20, height * 2));
        }

        public override void Update()
        {
            if (enable)
            {
                base.Update();

                if (IsClicked)
                {
                    if (screen is GameScreen)
                        screen.ExitScreen();
                    else if (screen is MenuScreen)
                        (screen as MenuScreen).MenuCancel((screen as MenuScreen).SelectedEntry);
                }
            }
        }

        public override void Draw()
        {
            if (enable)
            {
                base.Draw();
            }
        }

        public void SetPosition(Vector2 position)
        {
            this.position = position;
            this.rect = new Rectangle((int)position.X, (int)position.Y, (int)this.rect.Width, (int)this.rect.Height);
            clickableArea.SetRect(rect);
        }

        public void Enabled()
        {
            enable = true;
        }
    }
}
