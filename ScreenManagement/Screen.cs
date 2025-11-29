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
        
        // Track this screen's clickable areas
        protected List<ClickableArea> screenClickableAreas = new List<ClickableArea>();

        public abstract void Initialize();

        public abstract void Load();

        public abstract void UnloadContent();

        public abstract void Update();

        public abstract void Draw();

        public abstract ScreenState GetScreenState();

        public abstract void HandleInput();

        public abstract void ExitScreen();
        
        // Register a clickable area for this screen
        protected void RegisterClickableArea(ClickableArea ca)
        {
            if (!screenClickableAreas.Contains(ca))
            {
                screenClickableAreas.Add(ca);
            }
        }
        
        // Clean up this screen's clickable areas
        protected void CleanupClickableAreas()
        {
            foreach (var ca in screenClickableAreas)
            {
                ClickableAreaSystem.Remove(ca);
            }
            screenClickableAreas.Clear();
        }
    }
}
