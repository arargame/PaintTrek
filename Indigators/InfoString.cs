using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class InfoString
    {
        Vector2 position;
        Color color;
        string str;
        bool isActivated;
        byte alpha;

        public InfoString()
        {
            color = Color.Yellow;
            str = "";
            isActivated = false;
            alpha = 255;
        }

        public void Update()
        {
            if (isActivated)
            {
                alpha--;
                position.Y--;
            }

            if (alpha == 0)
            {
                isActivated = false;
            }

            color.A = alpha;
        }

        public void Draw()
        {
            Globals.SpriteBatch.Begin();

            if (isActivated)
                Globals.SpriteBatch.DrawString(Globals.GameFont, str, position, color);

            Globals.SpriteBatch.End();
        }

        public void GetInfo(string info, Vector2 position)
        {
            this.str = info;
            this.position = position;
            SetActive();
        }

        private void SetActive()
        {
            isActivated = true;
        }
    }
}
