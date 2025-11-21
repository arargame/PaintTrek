using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PaintTrek
{
    /// <summary>
    /// Screenshot utility for MonoGame
    /// </summary>
    public static class ScreenshotHelper
    {
        private static KeyboardState previousKeyboardState;

        /// <summary>
        /// Call this in your Update method to check for screenshot key press
        /// </summary>
        public static void Update(GraphicsDevice graphicsDevice)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();

            // Check for F12 or PrintScreen key press
            bool f12Pressed = currentKeyboardState.IsKeyDown(Keys.F12) && 
                             previousKeyboardState.IsKeyUp(Keys.F12);
            
            bool printScreenPressed = currentKeyboardState.IsKeyDown(Keys.PrintScreen) && 
                                     previousKeyboardState.IsKeyUp(Keys.PrintScreen);

            if (f12Pressed || printScreenPressed)
            {
                TakeScreenshot(graphicsDevice);
            }

            previousKeyboardState = currentKeyboardState;
        }

        /// <summary>
        /// Takes a screenshot and saves it to the Screenshots folder
        /// </summary>
        private static void TakeScreenshot(GraphicsDevice graphicsDevice)
        {
            string fullPath = "";
            try
            {
                // Try Desktop folder (easier to find for debugging)
                string screenshotsFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "PaintTrek Screenshots"
                );

                if (!Directory.Exists(screenshotsFolder))
                {
                    Directory.CreateDirectory(screenshotsFolder);
                    Console.WriteLine($"Created folder: {screenshotsFolder}");
                }

                // Generate filename with timestamp
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string filename = $"PaintTrek_{timestamp}.png";
                fullPath = Path.Combine(screenshotsFolder, filename);

                Console.WriteLine($"Attempting to save screenshot to: {fullPath}");

                // Get current viewport dimensions
                int width = graphicsDevice.Viewport.Width;
                int height = graphicsDevice.Viewport.Height;

                Console.WriteLine($"Viewport size: {width}x{height}");

                // Create array to hold pixel data
                Color[] data = new Color[width * height];

                // Get the data from the backbuffer
                graphicsDevice.GetBackBufferData(data);

                // Create a texture and save it
                using (Texture2D texture = new Texture2D(graphicsDevice, width, height))
                {
                    texture.SetData(data);

                    // Save as PNG
                    using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                    {
                        texture.SaveAsPng(stream, width, height);
                    }
                }

                Console.WriteLine($"✓ Screenshot saved successfully!");
                Console.WriteLine($"  Location: {fullPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ ERROR taking screenshot!");
                Console.WriteLine($"  Path: {fullPath}");
                Console.WriteLine($"  Error: {ex.Message}");
                Console.WriteLine($"  Stack: {ex.StackTrace}");
            }
        }
    }
}
