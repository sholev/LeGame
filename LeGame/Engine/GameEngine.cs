namespace LeGame.Engine
{
    using System.Linq;

    using LeGame.Engine.Enumerations;
    using LeGame.Handlers;
    using LeGame.Models;
    using LeGame.Models.Characters;
    using LeGame.Models.Characters.Enemies;
    using LeGame.Models.Characters.Player;
    using LeGame.Models.Items.PickableItems;
    using LeGame.Models.Items.Projectiles;
    using LeGame.Screens.StartScreen;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public class GameEngine : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private StartScreen startScreen;
        private StatScreen statScreen;

        private Player testPlayer;
        private Character sampleEnemy;
        private Level testLevel;
        private int oneSec = 1000;
        private int timeSinceLastUpdate = 0;
        private GameStages stage;
       

        public GameEngine()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
            
        }
        
        protected override void LoadContent()
        {
            this.IsMouseVisible = true;

            this.statScreen = new StatScreen();
            //Commented because it disables mouse capturing.
            //this.startScreen = new StartScreen();
            this.stage = GameStages.Game_Stage;

            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
           
            GfxHandler.Load(this.Content);
            
            GoldCoin coin = new GoldCoin(new Vector2(300, 300), "TestObjects/coin");

            // testEnemyTex = Content.Load<Texture2D>(@"TestObjects/cockSprite");
            Vector2 enemyPos = new Vector2(550, 350);
            Vector2 pos = new Vector2(
                GlobalVariables.WindowWidth / 2 - 140,
                GlobalVariables.WindowHeight / 2);

            this.sampleEnemy = new Enemy(enemyPos, @"TestObjects/cockSprite", 100, 100, 2, this.testLevel);
            this.sampleEnemy.Damaged += (sender, args) => GfxHandler.AddBloodEffect(sender);
            this.sampleEnemy.Died += (sender, args) => GfxHandler.AddDeathEffect(sender);

            this.testPlayer = new TestChar(pos, @"Player/p1Rotation", 100, 100, 2, this.testLevel);
            this.testPlayer.Damaged += (sender, args) => GfxHandler.AddBloodEffect(sender);
            this.testPlayer.Died += (sender, args) => GfxHandler.AddDeathEffect(sender);

            this.testLevel = new Level(@"..\..\..\Content\Maps\testMap2.txt", this.testPlayer);
            this.testLevel.Assets.Add(coin);

            this.sampleEnemy.Level = this.testLevel;
            this.testPlayer.Level = this.testLevel;
            this.testLevel.Enemies.Add(this.sampleEnemy);

            // TODO: Get Width and Heignt based on the level size?
            this.graphics.PreferredBackBufferWidth = GlobalVariables.WindowWidth; // set this value to the desired width of your window
            this.graphics.PreferredBackBufferHeight = GlobalVariables.WindowHeight;   // set this value to the desired height of your window
            this.graphics.ApplyChanges();
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
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (this.stage == GameStages.Game_Stage && this.testLevel.Character.CurrentHealth <= 0)
            {
                this.stage = GameStages.Death_Stage;
            }

            //if(stage == GameStages.Start_Stage)
            //{
            //    this.staRtScreen.Update(gameTime);
            //}
            
            if (this.stage == GameStages.Game_Stage)
            {
                this.timeSinceLastUpdate += gameTime.ElapsedGameTime.Milliseconds;
                if (this.timeSinceLastUpdate >= this.oneSec)
                {
                    this.timeSinceLastUpdate = 0;
                    this.testLevel.Character.CooldownTimer += 1;
                }
                this.testPlayer.Move();
                GfxHandler.GetSprite(this.testPlayer).Update(gameTime, this.testPlayer);

                foreach (Character enemy in this.testLevel.Enemies.ToList())
                {
                    enemy.Move();
                    GfxHandler.GetSprite(enemy).Update(gameTime, enemy);
                }

                foreach (Projectile projectile in this.testLevel.Projectiles.ToList())
                {
                    projectile.Move();
                    GfxHandler.GetSprite(projectile).Update(gameTime);
                }

                GfxHandler.UpdateExistingEffects(gameTime);
            }
            else
            {

            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.Black);

            // Vector2 origin = new Vector2(GfxHandler.GetWidth(this.testPlayer) / 2, GfxHandler.GetHeight(this.testPlayer) / 2);
            // TODO: Add your drawing code here
            if (this.stage == GameStages.Game_Stage)
            {
                this.spriteBatch.Begin();


                this.testLevel.Assets.ForEach(t => this.spriteBatch.Draw(GfxHandler.GetTexture(t), t.Position));

                this.spriteBatch.End();
                this.statScreen.DrawHealth(this.testLevel.Character, this.Content, this.spriteBatch);

                foreach (var ememy in this.testLevel.Enemies)
                {
                    GfxHandler.GetSprite(ememy).Draw(this.spriteBatch, this.sampleEnemy.Position);
                }
                
                foreach (var projectile in this.testLevel.Projectiles.ToList())
                {
                    GfxHandler.GetSprite(projectile).Draw(this.spriteBatch, projectile.Position, projectile.Angle);

                    if (projectile.Lifetime > projectile.Range)
                    {
                        this.testLevel.Projectiles.Remove(projectile);
                    }
                }

                GfxHandler.DrawExistingEffects(this.spriteBatch);

                GfxHandler.GetSprite(this.testPlayer).Draw(this.spriteBatch, this.testPlayer.Position, this.testPlayer.FacingAngle, this.testPlayer.MovementAngle);

            }
            else if (this.stage == GameStages.Death_Stage)
            {
                this.statScreen.EndScreen(this.Content, this.spriteBatch);
            }
            else
            {
                //this.startScreen.DrawStartScreen(this.spriteBatch, this.Content);
            }


            base.Draw(gameTime);
        }   
    }
}
