using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class BackButton
    {
        string text;
        Vector2 position;
        Rectangle rect;
        ClickableArea clickableArea;
        Color color;
        Screen screen;
        bool enable;
        SoundSystem menuUporDownSound;
        bool isOverlapped;

        public BackButton(string text,Screen screen,bool enable) 
        {
            this.text = text;
            position = new Vector2((Globals.GameSize.X - (Globals.GameFont.MeasureString(text).X+20)), (Globals.GameSize.Y - (Globals.GameFont.MeasureString(text).Y * 2)));
            rect = new Rectangle((int)position.X,(int) position.Y,(int) Globals.GameFont.MeasureString(text).X,(int) Globals.GameFont.MeasureString(text).Y);
            clickableArea = new ClickableArea(rect);
            color = Color.White;
            this.screen = screen;
            this.enable =enable;
            menuUporDownSound = new SoundSystem("Sounds/SoundEffects/menu-click", 1f, 0f, 0f, false, "Unknown", "Unknown");
        }

        ~BackButton() 
        {
            
        }

        public void Update() 
        {
            if(enable)
            {
                if (clickableArea.IsOverlapped)
                {
                    color = Color.Yellow;
                    if(!isOverlapped)
                    menuUporDownSound.Play();
                    isOverlapped = true;
                }
                else 
                {
                    color = Color.White;
                    isOverlapped = false;
                }

                if (clickableArea.IsClicked)
                {
                    if (screen is GameScreen)
                        screen.ExitScreen();
                    else if (screen is MenuScreen)
                        (screen as MenuScreen).MenuCancel((screen as MenuScreen).SelectedEntry);
                }
            }
        }

        public void Draw() 
        {
            if (enable)
            {
                Globals.SpriteBatch.Begin();
                Globals.SpriteBatch.DrawString(Globals.GameFont, text, position, color);
                Globals.SpriteBatch.End();
            }
        }

        public void SetPosition(Vector2 position) 
        {
            this.position = position;
            this.rect = new Rectangle((int)position.X,(int)position.Y,(int)this.rect.Width,(int)this.rect.Height);
            clickableArea.SetRect(rect);
        }

        public void Enabled()
        {
            enable = true;
        }


    }
}
