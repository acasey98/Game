using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game3
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        Texture2D dragonTexture;
        Vector2 dragonPosition;
        float dragonSpeed;
        float momentum = 1;
        bool movingUp;
        bool movingDown;
        bool movingLeft;
        bool movingRight;
        bool dashingState;
        float BaseSpeed => 100;
        float MaxSpeed = 400;
        float MaxDashSpeed = 1800;
        float AfterDashSpeed = 350;
        bool Decelerating;
        bool VerticalCollision;
        bool HorizontalCollision;
        float previousPositionX;
        float previousPositionY;
        string previousDirectionY;
        string previousDirectionX;
        bool turning;
        int dashCooldownDefault = 45;
        int dashCooldownCurrent = 0;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            dragonPosition = new Vector2(graphics.PreferredBackBufferWidth / 2,
            graphics.PreferredBackBufferHeight / 2);
            dragonSpeed = 0;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            dragonTexture = Content.Load<Texture2D>("./Graphics/Battlers/Dragon");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 

        public void Accel()
        {
            if (DiagonalMoving() == true)
            {
                if (dashingState == false)
                {
                    if (dragonSpeed >= MaxSpeed * 0.9f)
                    {
                        dragonSpeed = MaxSpeed * 0.9f;
                    }
                    else if (dragonSpeed < BaseSpeed * 0.9f)
                    {
                        dragonSpeed = BaseSpeed * 0.9f;
                    }
                    else if (dragonSpeed >= BaseSpeed * 0.9f)
                    {
                        dragonSpeed *= 1.09f;
                    }
                }
            }
            else
            {
                if (dashingState == false)
                {
                    if (dragonSpeed >= MaxSpeed)
                    {
                        dragonSpeed = MaxSpeed;
                    }
                    else if (dragonSpeed < BaseSpeed)
                    {
                        dragonSpeed = BaseSpeed;
                    }
                    else if (dragonSpeed >= BaseSpeed)
                    {
                        dragonSpeed *= 1.1f;
                    }
                }
            }
        }
        public void DashLogic()
        {
            if (dashingState == true && dashCooldownCurrent == 0)
            {
                if (Decelerating == false && dragonSpeed < MaxDashSpeed)
                {
                    dragonSpeed += 600;
                }
                else if (Decelerating == false && dragonSpeed >= MaxDashSpeed)
                {
                    Decelerating = true;
                }
                else if (Decelerating == true && dragonSpeed > AfterDashSpeed && dragonSpeed > MaxSpeed)
                {
                    dragonSpeed *= 0.9f;
                }
                else if ((Decelerating == true && dragonSpeed > AfterDashSpeed) && dragonSpeed <= MaxSpeed)
                {
                    dragonSpeed -= 50;
                }
                else if (Decelerating == true && dragonSpeed <= AfterDashSpeed)
                {
                    Decelerating = false;
                    dashingState = false;
                    dashCooldownCurrent = dashCooldownDefault; 
                }
            }
            else if (dashCooldownCurrent > 0)
            {
                dashCooldownCurrent -= 1;
            }
        }

        public void CheckBoundaryCollision()
        {
            var kstate = Keyboard.GetState();

            if (dragonPosition.X > graphics.PreferredBackBufferWidth - dragonTexture.Width / 2)
            {
                VerticalCollision = true;
                if (movingUp == false && movingDown == false)
                {
                    dragonSpeed -= dragonSpeed;
                    Decelerating = false;
                    dashingState = false;
                }

            }
            else
            {
                VerticalCollision = false;
            }

            if (dragonPosition.Y > graphics.PreferredBackBufferHeight - dragonTexture.Height / 2)
            {
                HorizontalCollision = true;
                if (movingLeft == false && movingRight == false)
                {
                    dragonSpeed -= dragonSpeed;
                    Decelerating = false;
                    dashingState = false;
                }
            }
            else
            {
                HorizontalCollision = false;
            }
            
        }

        public bool DiagonalMoving()
        {
                        
            if (movingDown && movingLeft || movingDown && movingRight || movingUp && movingLeft || movingUp && movingRight)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetPreviousPositions()
        {
            previousPositionY = dragonPosition.Y;
            previousPositionX = dragonPosition.X;

            if (movingUp == true)
            {
                previousDirectionY = "Up";
            }
            else if (movingDown == true)
            {
                previousDirectionY = "Down";
            }
            else
            {
                previousDirectionY = "None";
            }

            if (movingLeft == true)
            {
                previousDirectionX = "Left";
            }
            else if (movingRight == true)
            {
                previousDirectionX = "Right";
            }
            else
            {
                previousDirectionX = "None";
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            var kstate = Keyboard.GetState();

            


            if (kstate.GetPressedKeys().Length==0 || dashingState == true)
            {
                if (dragonSpeed > 0)
                {
                    if (dashingState == false && dragonSpeed > BaseSpeed)
                    {
                        dragonSpeed *= 0.95f;
                    }
                    else if (dashingState == false && dragonSpeed <= BaseSpeed)
                    {
                        dragonSpeed -= 10;
                    }

                        if (movingUp == true)
                    {
                        dragonPosition.Y -= dragonSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    if (movingDown == true)
                    {
                        dragonPosition.Y += dragonSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    if (movingLeft == true)
                    {
                        dragonPosition.X -= dragonSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    if (movingRight == true)
                    {
                        dragonPosition.X += dragonSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
            }
            
            

            if (kstate.IsKeyDown(Keys.Up) && dashingState == false)
            {
                if (movingDown == true)
                {
                    dragonSpeed = BaseSpeed;
                }
                movingDown = false;
                movingUp = true;
                if (kstate.IsKeyDown(Keys.Left))
                    movingLeft = true;
                else
                    movingLeft = false;
                if (kstate.IsKeyDown(Keys.Right))
                    movingRight = true;
                else
                    movingRight = false;
                
                    dragonPosition.Y -= dragonSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Accel();
                
                
            }

            if (kstate.IsKeyDown(Keys.Down) && dashingState == false)
            {
                if (movingUp == true)
                {
                    dragonSpeed = BaseSpeed;
                }
                movingDown = true;
                    movingUp = false;
                    if (kstate.IsKeyDown(Keys.Left))
                        movingLeft = true;
                    else
                        movingLeft = false;
                    if (kstate.IsKeyDown(Keys.Right))
                        movingRight = true;
                    else
                        movingRight = false;
                
                    dragonPosition.Y += dragonSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Accel();
                
                
            }

            if (kstate.IsKeyDown(Keys.Left) && dashingState == false)
            {
                if (movingRight == true)
                {
                    dragonSpeed = BaseSpeed;
                }
                movingLeft = true;
                movingRight = false;
                if (kstate.IsKeyDown(Keys.Up))
                    movingUp = true;
                else
                    movingUp = false;
                if (kstate.IsKeyDown(Keys.Down))
                    movingDown = true;
                else
                    movingDown = false;
                
                    dragonPosition.X -= dragonSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Accel();
                
                
            }

            if (kstate.IsKeyDown(Keys.Right) && dashingState == false)
            {
                if (movingLeft == true)
                {
                    dragonSpeed = BaseSpeed;
                }
                movingLeft = false;
                movingRight = true;
                if (kstate.IsKeyDown(Keys.Up))
                    movingUp = true;
                else
                    movingUp = false;
                if (kstate.IsKeyDown(Keys.Down))
                    movingDown = true;
                else
                    movingDown = false;
                
                    dragonPosition.X += dragonSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Accel();
                
                
            }

            if (kstate.IsKeyDown(Keys.Z) && dashingState == false && dragonSpeed > 0 && dashCooldownCurrent == 0)
            {
                dashingState = true;                
            }

            DashLogic();

            

            CheckBoundaryCollision();

            dragonPosition.X = Math.Min(Math.Max(dragonTexture.Width / 2, dragonPosition.X), graphics.PreferredBackBufferWidth - dragonTexture.Width / 2);
            dragonPosition.Y = Math.Min(Math.Max(dragonTexture.Height / 2, dragonPosition.Y), graphics.PreferredBackBufferHeight - dragonTexture.Height / 2);

            //SetPreviousPositions();




            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(
                dragonTexture,
                dragonPosition,
                null,
                Color.White,
                0f,
                new Vector2(dragonTexture.Width / 2, dragonTexture.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
                );
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
