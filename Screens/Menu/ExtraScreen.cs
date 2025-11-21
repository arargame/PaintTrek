using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class ExtraScreen:MenuScreen
    {
        public ExtraScreen() 
        {
            AddEntry(new MenuEntry("Items", true, 0));
            AddEntry(new MenuEntry("Enemies", true, 1));
            AddEntry(new MenuEntry("Musics", true, 2));
            AddEntry(new MenuEntry("Back", true, 3));

            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            screenTitle = "Extra Screen";
            Globals.Window.Title = screenTitle;
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
        }

        public override void Draw()
        {
            Globals.Graphics.GraphicsDevice.Clear(Color.Black);
            base.Draw();
        }

        public override void  HandleInput()
        {
 	         base.HandleInput();
        }

        public override void MenuSelect(int selectedEntry)
        {
            switch (selectedEntry)
            {
                case 0:
                    ExitScreen();
                    ScreenManager.AddScreen(new ItemScreen());
                    break;
                case 1:
                    ExitScreen();
                    ScreenManager.AddScreen(new EnemyScreen());
                    break;
                case 2:
                    ExitScreen();
                    ScreenManager.AddScreen(new MusicScreen());
                    break;
                case 3:
                    MenuCancel(selectedEntry);
                    break;
            }


        }

        public override void MenuCancel(int selectedEntry)
        {
            base.MenuCancel(selectedEntry);
            ScreenManager.AddScreen(new MainMenuScreen());
            ExitScreen();
        }

        public override void ExitScreen()
        {
            base.ExitScreen();
        }

    }
}
