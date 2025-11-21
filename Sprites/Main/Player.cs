using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class Player : Sprite
    {
        bool pixelatedMode;
        double pixelatedModeTime;

        public bool Isclicked;
        public Rectangle mouseCursorRectangle;
        double startingHealth;
        public Abilitiy ability;

        InfoString infoString;
        Laser laser;
        SecondGun secondGun;

        Texture2D damageTexture; 

        public enum MovementStyle
        {
            movingToUp,
            movingToDown,
            movingToLeft,
            movingToRight
        }
        public MovementStyle movementStyle;

        TimeSpan timeUntilFire;
        bool canFire = false;

        float invincibilityTime;
        const float FLICKER_FREQUENCY = 15f;
        bool flicker;
        double nextFlickerUpdate;

        public bool isInvinsible
        {
            get { return invincibilityTime > 0f; }
        }

        SoundSystem collectCoin;

        public bool Flicker
        {
            set
            {
                this.flicker = value;
                this.visible = !value;
            }
            get { return flicker; }
        }
        public Player()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetName("Space Ship");
            SetHealth(100);
            startingHealth = GetHealth();
            laser = new Laser(this);
            timeUntilFire = TimeSpan.FromSeconds(0.2);
            infoString = new InfoString();
            ability = new Abilitiy(this);
            secondGun = new SecondGun(this);
            pixelatedMode = false;
            pixelatedModeTime = 0;
        }

        public override void Load()
        {
            SetTextures(Globals.Content.Load<Texture2D>("Sprites/Ship/ShipSpriteSheet"));
            this.animation = new Animation(this.texture, 8, 1, 8, true);
            collectCoin = new SoundSystem("Sounds/SoundEffects/collectCoin", 0.2f, 0f, 0f, false, "", "");
        }

        public override void Update()
        {
            base.Update();
            SpeedSettings();
            StayOnScreen();
            VisibilityControl();
            Reload();
            CollisionDetectionWithOthers();
            infoString.Update();
            ability.Update();
            secondGun.Update();
            PixelatedMode();

            if (Globals.AutoAttack)
            {
                Attack();
            }
        }

        public override void Draw()
        {
            base.Draw();
            infoString.Draw();
            ability.Draw();
        }

        public void HandleInput(InputState input)
        {
            Controller(input);
            mouseCursorRectangle = input.cursorRect;
            Isclicked = input.IsMouseLeftPressed();
        }

        private void Controller(InputState input)
        {
            if (input.MoveUp)
            {
                TurnUp();
            }
            if (input.MoveDown)
            {
                TurnDown();
            }
            if (input.MoveRight)
            {
                Accelerate();
            }
            if (input.MoveLeft)
            {
                MoveReverse();
            }
            if (input.Fire)
            {
                Attack();
            }


            if (input.IsMouseLeftPressed()) 
            {
                Attack();
            }
        }

        private void PixelatedMode()
        {
            pixelatedModeTime -= (float)Globals.GameTime.ElapsedGameTime.TotalSeconds;

            if (pixelatedModeTime <= 0)
                pixelatedModeTime = 0;
            else if (pixelatedModeTime >= 3 && pixelatedModeTime < 4)
            {
                invincibilityTime = (int)pixelatedModeTime;
                flicker = true;
            }


            if (pixelatedMode)
            {
                laser.SetTextures(Globals.Content.Load<Texture2D>("Guns/pixelatedGun"));
                laser.animation = new Animation(laser.texture, 1, 1, 1, true);
            }

            if (pixelatedMode && pixelatedModeTime <= 0)
            {
                SetTextures(Globals.Content.Load<Texture2D>("Sprites/Ship/ShipSpriteSheet"));
                this.animation = new Animation(this.texture, 8, 1, 8, true);
                laser.SetTextures(GlobalTexture.laserTexture);
                laser.animation = new Animation(laser.texture, 1, 1, 1, true);
                pixelatedMode = false;
            }
        }

        public void SetActivePixelatedMode(double timeForBeingPixelated)
        {
            pixelatedMode = true;
            pixelatedModeTime = timeForBeingPixelated;
        }
        public void Accelerate()
        {
            velocity.X += 30.0f;
            movementStyle = MovementStyle.movingToRight;
        }
        public void MoveReverse()
        {
            velocity.X -= 30.0f;
            movementStyle = MovementStyle.movingToLeft;
        }
        public void TurnUp()
        {
            position.Y -= 6.0f;
            movementStyle = MovementStyle.movingToUp;
        }
        public void TurnDown()
        {
            position.Y += 6.0f;
            movementStyle = MovementStyle.movingToDown;
        }

        public void Attack()
        {
            if (canFire)
            {
                canFire = false;
                timeUntilFire = TimeSpan.FromSeconds(0.2);
                laser = new Laser(this);
                laser.Fire();
                secondGun.Fire();
            }
        }

        private void Reload()
        {
            timeUntilFire = TimeSpan.FromSeconds(timeUntilFire.TotalSeconds - Globals.GameTime.ElapsedGameTime.TotalSeconds);
            if (timeUntilFire.TotalSeconds <= 0)
                canFire = true;
        }


        public double FetchStartingHealth()
        {
            return startingHealth;
        }

        private void SpeedSettings()
        {
            position.X += velocity.X * (float)Globals.GameTime.ElapsedGameTime.TotalSeconds;
            velocity.X *= 0.95f;
        }
        public void StayOnScreen()
        {
            position.X = MathHelper.Clamp(position.X, animation.Width / 2, Globals.GameSize.X - animation.Width / 2);
            position.Y = MathHelper.Clamp(position.Y, animation.Height / 2, Globals.GameSize.Y - animation.Height / 2);
        }
        public override void SetStartingPosition()
        {
            position = new Vector2(this.animation.Width, Globals.GameSize.Y / 2 - this.animation.Height / 2);
        }

        private void VisibilityControl()
        {
            if (this.flicker)
            {
                if (Globals.GameTime.TotalGameTime.TotalSeconds > nextFlickerUpdate)
                {
                    this.visible = !this.visible;
                    this.nextFlickerUpdate = Globals.GameTime.TotalGameTime.TotalSeconds + 1 / FLICKER_FREQUENCY;
                }
            }
            if (this.isInvinsible)
            {
                this.invincibilityTime -= (float)Globals.GameTime.ElapsedGameTime.TotalSeconds;
                if (!this.isInvinsible) { this.flicker = false; this.visible = true; }
            }
        }

        public void Respawn(float invincibilityTime)
        {
            this.SetStartingPosition();
            this.invincibilityTime = invincibilityTime;
            this.flicker = true;
        }

        public override void SetHealth(double health)
        {
            if (GetHealth() + health > 100)
                health = 100 - GetHealth();

            base.SetHealth(health);
        }

        public override void SetTextures(Texture2D newTexture)
        {
            base.SetTextures(newTexture);
            damageTexture = MakeDamageTexture(normalTexture);
        }

        public override void TakeDamage(Sprite another)
        {

            if (!pixelatedMode)
            {
                texture = damageTexture;
                base.TakeDamage(another);
                Level.AddScore(this.GetPoint());
            }
            else
            {
                texture = damageTexture;
                isTakingDamage = true;

                if(another is Enemy || another is EnemyBullet)
                     SetHealth(-5);

                Level.AddScore(this.GetPoint());
            }


            if (!(another is Boss))
            {
                another.SetHealth(-1000);
            }
        }
        public bool OnKilled()
        {
            return !alive;
        }
        public Bullet GetGun()
        {
            return laser;
        }

        public bool CollisionWithExitDoor(ExitDoor exitDoor) 
        {
            Rectangle rect1 = Sprite.CalculateBoundingRectangle(new Rectangle(0, 0, (int)size.X, (int)size.Y), transformMatrix);

            if (exitDoor != null)
            {

                if (exitDoor.IsOpen())
                {
                    Rectangle rect2 = Sprite.CalculateBoundingRectangle(new Rectangle(0, 0, (int)exitDoor.GetSize().X, (int)exitDoor.GetSize().Y), exitDoor.transformMatrix);

                    if (rect1.Intersects(rect2))
                    {

                        if (Sprite.CollisionDetection(transformMatrix, (int)size.X, (int)size.Y, this.specificTextureData, exitDoor.transformMatrix, (int)exitDoor.GetSize().X, (int)exitDoor.GetSize().Y, exitDoor.specificTextureData))
                        {

                            return true;
                        }
                        else return false;
                    }
                    else return false;
                }
                else return false;

            }
            else return false;
        }

        private void CollisionDetectionWithOthers()
        {
            Rectangle rect1 = Sprite.CalculateBoundingRectangle(new Rectangle(0, 0, (int)size.X, (int)size.Y), transformMatrix);

            //With Enemies
            #region collisionWithEnemies
            for (int i = EnemySystem.enemyList.Count - 1; i >= 0; i--)
            {
                if (isInvinsible) continue;

                Enemy enemy = EnemySystem.enemyList[i];


                if (enemy != null)
                {

                    if (enemy is ChildTrilobit)
                    {
                        if ((enemy as ChildTrilobit).GetMovementStyle() == ChildTrilobit.MovementStyle.spawned)
                            continue;
                    }

                    Rectangle rect2 = Sprite.CalculateBoundingRectangle(new Rectangle(0, 0, (int)enemy.size.X, (int)enemy.size.Y), enemy.transformMatrix);

                    if (rect1.Intersects(rect2))
                    {
                        if (specificTextureData == null || enemy.specificTextureData== null)
                            continue;

                        if (Sprite.CollisionDetection(transformMatrix, (int)size.X, (int)size.Y, this.specificTextureData, enemy.transformMatrix, (int)enemy.size.X, (int)enemy.size.Y, enemy.specificTextureData))
                        {
                            enemy.TakeDamage(this);
                            TakeDamage(enemy);
                            Respawn(2f);
                        }
                    }

                }
            }
            #endregion

            //With Collectables
            #region collisionWithCollectables

            for (int i = CollectableObjectSystem.collactableObjectList.Count - 1; i >= 0; i--)
            {
                CollectableObject co = CollectableObjectSystem.collactableObjectList[i];

                if (co != null)
                {
                    Rectangle rect2 = Sprite.CalculateBoundingRectangle(new Rectangle(0, 0, (int)co.size.X, (int)co.size.Y), co.transformMatrix);
                    if (rect1.Intersects(rect2))
                    {
                        if (this.specificTextureData == null || co.specificTextureData == null)
                            continue;

                        if (Sprite.CollisionDetection(transformMatrix, (int)size.X, (int)size.Y, this.specificTextureData, co.transformMatrix, (int)co.size.X, (int)co.size.Y, co.specificTextureData))
                        {

                            if (co is Bubble)
                            {
                                (co as Bubble).AssaignOwner(this);
                                continue;
                            }
                            else
                            {
                                if (co is Diamond)
                                {
                                    infoString.GetInfo(co.GetPoint() + " Points", co.position);
                                    ability = new Abilitiy(co as Diamond, this);
                                }
                                else if (co is PixelSupply)
                                {
                                    (co as PixelSupply).ChangePlayer(this);

                                    SetTextures(Globals.Content.Load<Texture2D>("Sprites/Ship/pixelShipSpriteSheet"));
                                    this.animation = new Animation(this.texture, 4, 2, 8, true);

                                }
                                else if (co is Wrench)
                                {
                                    infoString.GetInfo((co as Wrench).GiveHealth() + " Health", co.position);
                                    this.SetHealth((co as Wrench).GiveHealth());
                                }
                                else if (co is BouncingFireCollection)
                                {
                                    // BouncingFire bf= new BouncingFire(this,(co as BouncingFireCollection).GiveTime(),co);
                                    // bf.Activate();
                                    //  bf.Fire();
                                    secondGun.AddGun(new BouncingFire(this), Globals.Random.Next(2, 7));
                                }
                                else if (co is RocketSupply)
                                {
                                    secondGun.AddGun(new Rocket(this), Globals.Random.Next(50, 50+Level.LevelCounter*10));
                                }
                                else if(co is TripleFireSupply)
                                {
                                    secondGun.AddGun(new TripleFire(this),Globals.Random.Next(50,50+Level.LevelCounter*10));
                                }
                                else if(co is OrbitalFireSupply)
                                {
                                    secondGun.AddGun(new OrbitalFire(this),Globals.Random.Next(20,50));
                                }
                                else if (co is DiffusedFireSupply)
                                {
                                    secondGun.AddGun(new DiffusedPlayerFire(this), Globals.Random.Next(50, 50+Level.LevelCounter*10));
                                }
                                else if(co is WaveGunSupply)
                                {
                                    secondGun.AddGun(new WaveGun(this),Globals.Random.Next(50,80));
                                }
                                Level.AddScore(co.GetPoint());
                                TakeDamage(co);
                            }
                            co.alive = false;
                            collectCoin.Play();
                        }

                    }
                }
            }

            #endregion


        }


    }
}
