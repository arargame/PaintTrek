using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class MobileController
    {
        Texture2D shootAreaTexture;
        Rectangle shootAreaRect;
        Color shootAreaColor;
        Texture2D controllerArea;
        Vector2 controllerAreaPosition;
        Texture2D[] controllerButtons = new Texture2D[4];
        Vector2[] controllerButttonsPositions = new Vector2[4];
        Vector2 controllerButtonSize;

        public bool pressedUp;
        public bool pressedDown;
        public bool pressedLeft;
        public bool pressedRight;
        public bool pressedFire;

        public MobileController() 
        {
            Initalize();
        }

        ~MobileController() 
        {
        }

        private void Initalize() 
        {
            shootAreaTexture = Globals.Content.Load<Texture2D>("Textures/shootArea");
            shootAreaRect = new Rectangle((int)Globals.GameSize.X - shootAreaTexture.Width, (int)Globals.GameSize.Y - shootAreaTexture.Height, shootAreaTexture.Width, shootAreaTexture.Height);
            shootAreaColor = Color.White;
            controllerArea = Globals.Content.Load<Texture2D>("Textures/controllerArea");
            controllerAreaPosition = new Vector2(2, Globals.GameSize.Y / 2 - controllerArea.Height / 2);
            controllerButtonSize = new Vector2(controllerArea.Width / 3, controllerArea.Height / 3);
            for (int i = 0; i < controllerButtons.Length; i++)
            {
                controllerButtons[i] = Globals.Content.Load<Texture2D>("Textures/cButton");
            }

            //Up Button
            controllerButttonsPositions[0] = new Vector2(controllerAreaPosition.X + controllerButtonSize.X, controllerAreaPosition.Y);
            //Left Button
            controllerButttonsPositions[1] = new Vector2(controllerAreaPosition.X, controllerAreaPosition.Y + controllerButtonSize.Y);
            //Right Button
            controllerButttonsPositions[2] = new Vector2(controllerAreaPosition.X + (controllerButtonSize.Y * 2), controllerAreaPosition.Y + controllerButtonSize.Y);
            //Down Button
            controllerButttonsPositions[3] = new Vector2(controllerAreaPosition.X + controllerButtonSize.X, controllerAreaPosition.Y + (controllerButtonSize.Y * 2));

        }

        public void Update(Player player)
        {

            Controller(player);
            IsShooted(player);
        }

        public void Draw() 
        {
            Globals.SpriteBatch.Begin(SpriteSortMode.Deferred,BlendState.NonPremultiplied);

            //Shoot Area Drawing
          //  Globals.SpriteBatch.Draw(shootAreaTexture, shootAreaRect, shootAreaColor);

            //Controller Area Drawing
         //   Globals.SpriteBatch.Draw(controllerArea, controllerAreaPosition, new Color(255,255,255,100));

           /* for (int i = 0; i < controllerButtons.Length; i++)
            {
                Globals.SpriteBatch.Draw(controllerButtons[i], new Rectangle((int)controllerButttonsPositions[i].X, (int)controllerButttonsPositions[i].Y, (int)controllerButtonSize.X, (int)controllerButtonSize.Y), Color.White);
            }*/

            Globals.SpriteBatch.End();
        }

        private void IsShooted(Player player)
        {
            bool b = player.mouseCursorRectangle.Intersects(shootAreaRect);

            if (b)
            {
                shootAreaColor = Color.White;
                player.Attack();
                pressedFire = true;
            }
            else
            {
                shootAreaColor = new Color((byte)shootAreaColor.R, (byte)shootAreaColor.G, (byte)shootAreaColor.B, (byte)124);
                pressedFire = false;
            }
        }

        private void Controller(Player player)
        {
            Rectangle rect = new Rectangle((int)controllerAreaPosition.X, (int)controllerAreaPosition.Y, controllerArea.Width, controllerArea.Height);
            bool b = player.mouseCursorRectangle.Intersects(rect) && player.Isclicked;
            Rectangle[] rects = new Rectangle[4];
            for (int i = 0; i < controllerButtons.Length; i++)
            {
                rects[i] = new Rectangle((int)controllerButttonsPositions[i].X, (int)controllerButttonsPositions[i].Y, (int)controllerButtonSize.X, (int)controllerButtonSize.Y);
            }
            if (rects[0].Intersects(player.mouseCursorRectangle) )
            {
                player.TurnUp();
                pressedUp = true;
            }
            else pressedUp = false;

            if (rects[1].Intersects(player.mouseCursorRectangle))
            {
                player.MoveReverse();
                pressedLeft = true;
            }
            else pressedLeft = false;

            if (rects[2].Intersects(player.mouseCursorRectangle))
            {
                player.Accelerate();
                pressedRight = true;
            }
            else pressedRight = false;

            if (rects[3].Intersects(player.mouseCursorRectangle))
            {
                player.TurnDown();
                pressedDown = true;
            }
            else pressedDown = false;
        }
    }
}
