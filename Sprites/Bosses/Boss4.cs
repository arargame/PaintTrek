using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Boss4 : Boss
    {
        enum MovementStyle
        {
            Left,
            Up,
            Down,
            Starting
        }
        MovementStyle movementStyle;
        Time time;
        TimeSpan timeKeeper;

        InfoString infoString;
        List<SnakeStone> pieces;

        int divisionTime;

        int counter;

        List<Vector2> steps;

        public Boss4()
        {
            Initialize();

            if (Globals.Graphics.IsFullScreen)
            {
                scale = 1.2f;
            }
            else scale = 1f;
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Boss 4", 2000, 30, 4000);
            FetchStartingHealth(GetHealth());
            SetVelocity();
            movementStyle = MovementStyle.Starting;
            time = new Time(3);
            time.Activate();

            divisionTime = 15;
            pieces = new List<SnakeStone>();

            pieces.Add(new SnakeStone());
            pieces.Add(new SnakeStone());
            pieces.Add(new SnakeStone());
            pieces.Add(new SnakeStone());

            timeKeeper = Time.TotalGameTime();

            counter = 0;

            steps = new List<Vector2>();

            infoString = new InfoString();
        }


        public override void Load()
        {
           // SetTextures(Globals.Content.Load<Texture2D>("Sprites/Boss/snakeStoneSpriteSheetLeft"));
            SetTextures(GlobalTexture.snakeStoneLeftTexture);
            animation = new Animation(texture, 1, 3, 3, true);
        }



        public override void Update()
        {
            if (GetHealth() > 2000)
                SetHealth(-(GetHealth() - 2000));

            if (Time.TotalGameTime() - timeKeeper > TimeSpan.FromSeconds(0.25f))
            {
                timeKeeper = Time.TotalGameTime();


                SimpleMovement(velocity);

                steps.Add(position);



                if (steps.Count >= pieces.Count)
                {
                    int k = pieces.Count - 1;
                    for (int i = steps.Count - pieces.Count; i < steps.Count - 1; i++)
                    {
                        pieces[k].position = steps[i];
                        k--;
                    }
                }
            }

            infoString.Update();
            time.Update();

            base.Update();

            counter++;

            if (counter > pieces.Count - 1)
                counter = 1;


            for (int i = 0; i < pieces.Count; i++)
            {
                if (!pieces[i].alive)
                {
                    pieces.Remove(pieces[i]);
                    SetHealth(-100);
                }
            }


            if (divisionTime <= 0)
            {
                divisionTime = 15;
                pieces.Add(new SnakeStone());
                SetHealth(10);
                infoString.GetInfo("+" + 10 + " HP", this.position);
            }

            if (time.GetCounter() == 3)
            {
                divisionTime -= 3;
                time = new Time(3);
                time.Activate();

                int number = Globals.Random.Next(0, 100);

                if (number <= 33)
                {
                    movementStyle = MovementStyle.Left;
                    //SetTextures(Globals.Content.Load<Texture2D>("Sprites/Boss/snakeStoneSpriteSheetLeft"));
                    SetTextures(GlobalTexture.snakeStoneLeftTexture);
                    animation = new Animation(texture, 1, 3, 3, true);
                }
                else if (number > 33 && number <= 66)
                {
                    movementStyle = MovementStyle.Up;
                    //SetTextures(Globals.Content.Load<Texture2D>("Sprites/Boss/snakeStoneSpriteSheetUp"));
                    SetTextures(GlobalTexture.snakeStoneUpTexture);
                    animation = new Animation(texture, 3, 1, 3, true);

                }
                else if (number > 66)
                {
                    movementStyle = MovementStyle.Down;
                    //SetTextures(Globals.Content.Load<Texture2D>("Sprites/Boss/snakeStoneSpriteSheetDown"));
                    SetTextures(GlobalTexture.snakeStoneDownTexture);
                    animation = new Animation(texture, 3, 1, 3, true);
                }
            }



            Rectangle rect = Globals.GameRect;

            if (movementStyle != MovementStyle.Starting)
            {

                if (movementStyle == MovementStyle.Starting)
                {
                    movementStyle = MovementStyle.Left;
                }

                if (position.X <= 0)
                {
                    position = new Vector2(Globals.GameSize.X - size.X, position.Y);
                }

                if (position.Y + size.Y <= 0)
                {
                    position = new Vector2(position.X, Globals.GameSize.Y - size.Y);
                }

                if (position.Y >= Globals.GameSize.Y)
                {
                    position = new Vector2(position.X, 0);
                }

            }
            else
            {
                if (!rect.Intersects(this.destinationRectangle))
                {
                    this.velocity = new Vector2(-50, 0);
                }
            }


            if (movementStyle == MovementStyle.Left)
            {
                velocity = new Vector2(-size.X, 0);
                if (pieces.Count > 0)
                    pieces[0].position = new Vector2(position.X + size.X, position.Y);
            }
            else if (movementStyle == MovementStyle.Up)
            {
                velocity = new Vector2(0, -size.Y);
                if (pieces.Count > 0)
                    pieces[0].position = new Vector2(position.X, position.Y + size.Y);
            }
            else if (movementStyle == MovementStyle.Down)
            {
                velocity = new Vector2(0, size.Y);
                if (pieces.Count > 0)
                    pieces[0].position = new Vector2(position.X, position.Y - size.Y);
            }
        }

        public override void Draw()
        {
            base.Draw();
            infoString.Draw();
        }

        public override void SetVelocity()
        {
            base.SetVelocity();
        }

        internal static Boss4 GetBoss4()
        {
            return new Boss4();
        }


    }
}
