using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace PaintTrek
{
    public enum ScreenState 
    {
        Active,
        Inactive,
        Frozen
    }

    abstract class Screen
    {
        public double timeToActivateInput;
        public bool inputActivated;

        public InputState inputState;

        public ContentManager contentManager;

        public ScreenState screenState;

        public string screenTitle;

        public abstract void Initialize();

        public abstract void Load();

        public abstract void UnloadContent();

        public abstract void Update();

        public abstract void Draw();

        public abstract ScreenState GetScreenState();

        public abstract void HandleInput();

        public abstract void ExitScreen();
    }
}
