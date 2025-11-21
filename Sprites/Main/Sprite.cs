using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    abstract class Sprite
    {
        float rotationAngle;
        string name;
        public Texture2D texture;
        //public Texture2D damageTexture;
        public Texture2D normalTexture;
        public Vector2 position;
        public Vector2 velocity;
        public Color color;
        public Rectangle destinationRectangle;
        public Rectangle sourceRectangle;
        public float scale;
        public float rotation;
        public bool visible;
        public bool alive;
        public Vector2 origin;
        public SpriteEffects spriteEffect;
        public float layerDepth;
        public Vector2 size;
        public Color[,] textureData2D;
        public Color[] textureData;
        public Color[] specificTextureData;

        public Animation animation;

        public Matrix transformMatrix;

        double health;
        double damage;
        bool IsKilled;
        public bool originChanged;

        int point;

        double takingDamageTime = 0;
        public bool isTakingDamage = false;

        SoundSystem deathSound = new SoundSystem("Sounds/SoundEffects/explosion", 1f, 0f, 0f, false, "Unknown", "Unknown");


        public virtual void Initialize()
        {
            SpriteSystem.spriteList.Add(this);


            Load();

            SetCharacterInfo("Sprite", 10, 10, 10);

            position = new Vector2(800, 200);
            size = new Vector2(50, 50);
            scale = 1f;
            Vector2 origin = new Vector2(position.X, position.Y);
            spriteEffect = SpriteEffects.None;
            layerDepth = 0f;
            rotation = 0f;
            color = Color.White;

            velocity = Vector2.Zero;

            SetStartingPosition();

            sourceRectangle = new Rectangle(0, 0, (int)size.X, (int)size.Y);
            destinationRectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);

            alive = true;
            visible = true;

            transformMatrix = Matrix.Identity;
            rotationAngle = 0f;
            IsKilled = false;
            originChanged = false;
        }

        public virtual void Load()
        {
            normalTexture = Globals.Content.Load<Texture2D>("Sprites/Default/SmilemanTexture");
            //damageTexture = MakeDamageTexture(normalTexture);
            texture = normalTexture;
            animation = new Animation(texture, 1, 1, 1, false);
            SetTextureData();
        }

        public virtual void UnloadContent() 
        {

        }

        public virtual void Update()
        {
            if (alive)
            {
                SetSize();
                SetOrigin();
                SetRectangle();
                SetSpecificTextureData();
                animation.Update();
                CalculateTransformMatrix();
                Rotation();

                if (isTakingDamage)
                    takingDamageTime += Globals.GameTime.ElapsedGameTime.TotalSeconds;

                if (isTakingDamage && takingDamageTime >= 0.1)
                {
                    texture = normalTexture;
                    takingDamageTime = 0;
                    isTakingDamage = false;
                }
            }

            CheckHealth();
        }

        public virtual void Draw()
        {
            Globals.SpriteBatch.Begin(SpriteSortMode.Deferred,BlendState.NonPremultiplied);

            if (alive && visible && !texture.IsDisposed)
            {
                Globals.SpriteBatch.Draw(texture,
                    position,
                    sourceRectangle,
                    color,
                    rotation,
                    origin,
                    scale,
                    spriteEffect,
                    layerDepth);
            }
            Globals.SpriteBatch.End();
        }

        public virtual void SetStartingPosition() { }

        public virtual void SetHealth(double health)
        {
            this.health += health;

            if (this.health <= 0)
                IsKilled = true;
        }

        public double GetDamage()
        {
            return -damage;
        }

        public void SetDamage(double damage)
        {
            this.damage = damage;
        }

        public void CheckHealth()
        {
            if (alive && health <= 0)
            {
                this.IsKilled = true;
                Level.AddScore(GetPoint() * Level.LevelCounter);
                alive = false;
                ExplosionSystem.AddExplosion(this);
                deathSound.Play();
            }

            if (!alive)
            {
                SpriteSystem.Remove(this);
            }
        }

        public void SetCharacterInfo(string name, double health, double damage, int point)
        {
            SetName(name);
            this.health = health;
            this.damage = damage;
            this.point = point;
        }

        public virtual void SetVelocity()
        {
        }
        public virtual void SimpleMovement(Vector2 amount)
        {
            position += amount;
        }

        public double GetHealth()
        {
            return health;
        }

        public virtual int GetPoint()
        {
            return point;
        }
        private void SetRectangle()
        {
            destinationRectangle = new Rectangle((int)(position.X - origin.X), (int)(position.Y - origin.Y), (int)size.X, (int)size.Y);
            sourceRectangle = new Rectangle(animation.FrameBounds.X, animation.FrameBounds.Y, (int)size.X, (int)size.Y);
        }

        private void SetSize()
        {
            size = new Vector2(animation.Width, animation.Height);
        }

        private void SetOrigin()
        {
            if(!originChanged)
            origin = new Vector2(size.X / 2, size.Y / 2);
        }

        public void Rotate(float degree)
        {
            rotationAngle = MathHelper.ToRadians(degree);
        }

        private void Rotation()
        {
            rotation += rotationAngle;
        }

        public void ResetRotation()
        {

            if (rotation > 0 && rotation < 1 || rotation > -1 && rotation < 0)
            {
                rotation = 0;
            }
            else if (rotation > 0)
            {
                rotation -= 0.05f;
            }
            else if (rotation < 0)
            {
                rotation += 0.05f;
            }
            else rotation = 0;
        }

        public void SetName(string newName)
        {
            name = newName;
        }
        public string GetName()
        {
            return name;
        }

        private void SetTextureData()
        {
            int pixelCount = texture.Width * texture.Height;

            if (pixelCount <= 0)
            {
                textureData = new Color[1];
                textureData2D = new Color[1, 1];
            }
            else
            {
                textureData = new Color[pixelCount];
                texture.GetData(textureData);
                textureData2D = TextureTo2DArray(texture);
            }

        }

        private void SetSpecificTextureData()
        {
            specificTextureData = GetSpecificAreaColorArray();
        }

        private void CalculateTransformMatrix()
        {
            //Vector3 scale = new Vector3(Globals.GameSize.X/800,Globals.GameSize.Y/600,1f);


            transformMatrix = Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateScale(scale) *
                Matrix.CreateTranslation(new Vector3(position, 0.0f));
        }



        public virtual void SetTextures(Texture2D newTexture)
        {
            texture = newTexture;
            normalTexture = texture;
            //damageTexture = MakeDamageTexture(newTexture);
            SetTextureData();
        }


        public virtual void TakeDamage(Sprite another)
        {
            //texture = damageTexture;
            isTakingDamage = true;
            SetHealth(another.GetDamage());
        }


        public Texture2D MakeDamageTexture(Texture2D texture)
        {
            this.scale = this.scale+0.0f;
            int pixelCount = texture.Width * texture.Height;
            Color[] pixels = new Color[pixelCount];
            texture.GetData<Color>(pixels);
            for (int i = 0; i < pixels.Length; i++)
            {
                byte offset = 200;
                byte r = (byte)Math.Min(pixels[i].R + offset, 255);
                byte g = (byte)Math.Min(pixels[i].R + offset, 255);
                byte b = (byte)Math.Min(pixels[i].R + offset, 255);
                pixels[i] = new Color(r, g, b, pixels[i].A);
            }

            Texture2D outTexture = new Texture2D(Globals.Graphics.GraphicsDevice, texture.Width, texture.Height, false, SurfaceFormat.Color);
            outTexture.SetData<Color>(pixels);
            return outTexture;
        }

        private Color GetColorAt(int x, int y)
        {
            // we need to take the source rectangle into account
            var transformedX = x + animation.FrameBounds.X;
            var transformedY = y + animation.FrameBounds.Y;

            // calculate the offset and return the color from
            // the one-dimensional texture data returned by the "GetData" method
            return textureData[transformedX + transformedY * texture.Width];
        }


        private Color[] GetSpecificAreaColorArray()
        {
            int width = (int)size.X;
            int height = (int)size.Y;

            Color[] specificAreaColorArray = new Color[width * height];

            int counter = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    specificAreaColorArray[counter] = textureData2D[j, i];
                    counter++;
                }
            }

            return specificAreaColorArray;
        }

        public static Color[,] TextureTo2DArray(Texture2D Texture)
        {
            Color[] TextureColor1D = new Color[Texture.Width * Texture.Height];
            Texture.GetData(TextureColor1D);
            Color[,] TextureColor2D = new Color[Texture.Width, Texture.Height];
            for (int x = 0; x < Texture.Width; x++)
                for (int y = 0; y < Texture.Height; y++)
                    TextureColor2D[x, y] = TextureColor1D[x + y * Texture.Width];
            return TextureColor2D;
        }


        public static Rectangle CalculateBoundingRectangle(Rectangle rectangle,
                                                         Matrix transform)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        public static Rectangle GetOverlappingRectangle(Rectangle boundsA, Rectangle boundsB)
        {
            int x1 = Math.Max(boundsA.Left, boundsB.Left);
            int y1 = Math.Max(boundsA.Top, boundsB.Top);
            int x2 = Math.Min(boundsA.Right, boundsB.Right);
            int y2 = Math.Min(boundsA.Bottom, boundsB.Bottom);

            int width = x2 - x1;
            int height = y2 - y1;

            if (width > 0 && height > 0)
                return new Rectangle(x1, y1, width, height);
            else return Rectangle.Empty;
        }

        #region Collision Detection

        //Per Pixel Collision Detection with Transform Matrix on Scaled,Rotated,Animated Object
        public static bool CollisionDetection(
                           Matrix transformA, int widthA, int heightA, Color[] dataA,
                           Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }
        //Per Pixel Collision Detection on Animated Object
        public static bool CollisionDetection(Sprite spriteA, Sprite spriteB)
        {
            // get the bounds in screen coordinates
            var rectangleA = spriteA.destinationRectangle;
            var rectangleB = spriteB.destinationRectangle;

            // find the bounds of the rectangle intersection in screen space
            var intersection = GetOverlappingRectangle(rectangleA, rectangleB);
            if (intersection == Rectangle.Empty)
            {
                return false;
            }

            // Check every point within the intersection bounds
            for (int y = intersection.Top; y < intersection.Bottom; y++)
            {
                for (int x = intersection.Left; x < intersection.Right; x++)
                {
                    // to retrieve the color of a pixel
                    // we need to transform the coordinates back
                    // to the sprite's local space
                    int xA = x - rectangleA.Left;
                    int yA = y - rectangleA.Top;
                    Color colorA = spriteA.GetColorAt(xA, yA);

                    int xB = x - rectangleB.Left;
                    int yB = y - rectangleB.Top;
                    Color colorB = spriteB.GetColorAt(xB, yB);

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool SimpleCollisionDetection(Sprite spriteA, Sprite spriteB)
        {
            return spriteA.destinationRectangle.Intersects(spriteB.destinationRectangle);
        }

        #endregion

    }
}
