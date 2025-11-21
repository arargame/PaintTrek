using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class ItemScreen:MenuScreen
    {
        struct Item
        {
            public Texture2D texture;
            public string name;
            public string info;
        }

        List<Item> items;
        int counter;
        BackButton backButton;

        public ItemScreen() 
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();

            items = ListCreator();
            counter = 0;
            backButton = new BackButton("Back",this,true);
        }

        public override void Load()
        {
            base.Load();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update()
        {
            base.Update();
            backButton.Update();
        }

        public override void Draw()
        {
            Globals.Graphics.GraphicsDevice.Clear(Color.Black);
            base.Draw();

            backButton.Draw();

            Vector2 position = new Vector2(Globals.GameSize.X / 2 - items[counter].texture.Width / 2, Globals.GameSize.Y * 0.1f);

            Globals.SpriteBatch.Begin();
            Vector2 infoPosition = new Vector2(Globals.GameSize.X * 0.1f, Globals.GameSize.Y * 0.35f);
            Globals.SpriteBatch.DrawString(Globals.MenuFont, "Name :    " + items[counter].name, infoPosition, Color.White);
            Globals.SpriteBatch.DrawString(Globals.MenuFont, "Info :    " + items[counter].info, new Vector2(infoPosition.X, infoPosition.Y + 50), Color.White);
            Globals.SpriteBatch.Draw(items[counter].texture, position, Color.White);
            Globals.SpriteBatch.End();
           
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void ExitScreen()
        {
            base.ExitScreen();
        }
        public override void MenuCancel(int selectedEntry)
        {
            base.MenuCancel(selectedEntry);
            ExitScreen();
            ScreenManager.AddScreen(new ExtraScreen());
            ExitScreen();
        }

        public override void MenuLeft(int selectedEntry)
        {
            counter--;
            if (counter < 0)
                counter = items.Count - 1;
        }

        public override void MenuRight(int selectedEntry)
        {
            counter++;
            if (counter > items.Count - 1)
                counter = 0;
        }

        private List<Item> ListCreator()
        {
            items = new List<Item>();

            Item blueDiamond = new Item();
            blueDiamond.name = "Blue Diamond";
            blueDiamond.texture = Globals.Content.Load<Texture2D>("CollectableObjects/blueDiamond");
            blueDiamond.info = "Gives Speedy Attack \n      (+%60 Attaack Speed,+%0-150 Damage).";
            items.Add(blueDiamond);

            Item redDiamond = new Item();
            redDiamond.name = "Red Diamond";
            redDiamond.texture = Globals.Content.Load<Texture2D>("CollectableObjects/redDiamond");
            redDiamond.info = "Gives Power Attack \n    (+% 250-400 Damage).";
            items.Add(redDiamond);

            Item greenDiamond = new Item();
            greenDiamond.name = "Green Diamond";
            greenDiamond.texture = Globals.Content.Load<Texture2D>("CollectableObjects/greenDiamond");
            greenDiamond.info = "Gives Poison Attack \n     (%50 Slow Enemy Speed).";
            items.Add(greenDiamond);

            Item blackDiamond = new Item();
            blackDiamond.name = "Black Diamond";
            blackDiamond.texture = Globals.Content.Load<Texture2D>("CollectableObjects/blackDiamond");
            blackDiamond.info = "Gives Critical Attack \n       (+%33 Attack Speed,+%500-1000 Damage).";
            items.Add(blackDiamond);

            Item bubble = new Item();
            bubble.name = "Bubble";
            bubble.texture = Globals.Content.Load<Texture2D>("CollectableObjects/bubble");
            bubble.info = "Gives Damage Block \n        (+50 Armor).";
            items.Add(bubble);

            Item wrench = new Item();
            wrench.name = "Wrench";
            wrench.texture = Globals.Content.Load<Texture2D>("CollectableObjects/wrenchSpriteSheet");
            wrench.info = "Gives +20 HP.";
            items.Add(wrench);

            Item bouncingBall = new Item();
            bouncingBall.name = "Bouncing Ball";
            bouncingBall.texture = Globals.Content.Load<Texture2D>("CollectableObjects/bouncingBallSupply");
            bouncingBall.info = "Gives a bouncing fire \n       which has 500 HP and 20 damage.";
            items.Add(bouncingBall);

            Item diffusedFireSupply = new Item();
            diffusedFireSupply.name = "Diffused Fire Supply";
            diffusedFireSupply.texture = Globals.Content.Load<Texture2D>("CollectableObjects/diffusedFireSupply");
            diffusedFireSupply.info = "Gives a diffused fire gun\n       which can shoot 8 bullet each time.";
            items.Add(diffusedFireSupply);


            Item rocketSupply = new Item();
            rocketSupply.name = "Rocket Supply";
            rocketSupply.texture = Globals.Content.Load<Texture2D>("CollectableObjects/rocketSupply");
            rocketSupply.info = "Gives a guided rocket gun.\n";
            items.Add(rocketSupply);

            Item pixelSupply = new Item();
            pixelSupply.name = "Pixel Supply";
            pixelSupply.texture = Globals.Content.Load<Texture2D>("CollectableObjects/pixelSupply");
            pixelSupply.info = "Gives an ability to be in pixelated mode.\n ";
            items.Add(pixelSupply);

            Item orbitalFireSupply = new Item();
            orbitalFireSupply.name = "Orbital Fire Supply";
            orbitalFireSupply.texture = Globals.Content.Load<Texture2D>("CollectableObjects/orbitalFireSupply");
            orbitalFireSupply.info = "Gives orbital fire which moves around your ship\n ";
            items.Add(orbitalFireSupply);


            return items;
        }
    }
}
