﻿namespace LeGame.Graphics
{
    using System.Linq;

    using Interfaces;
    using Models.Characters.Player;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public class PlayerRotationSprite : Sprite
    {
        private const int PlayerRotationSpriteUpdateTime = 50;
        private const int LegsRow = 0;
        private const int TorsoRow = 1;
        private bool reverse;

        public PlayerRotationSprite(Texture2D texture) 
            : base(texture, PlayerRotationSpriteUpdateTime)
        {
            this.CurrentFrame = 5;
            this.TotalFrames = this.Columns;
        }

        public override void Update(GameTime gameTime, ICharacter character)
        {
            this.TimeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (this.TimeSinceLastFrame < this.TimePerFrame)
            {
                return;
            }
            this.TimeSinceLastFrame = gameTime.ElapsedGameTime.Milliseconds;
            
            KeyboardState keyState = Keyboard.GetState();

            bool moving = ((Player)character).KbKeys.Any(key => keyState.IsKeyDown(key));

            if (moving || this.CurrentFrame != 5)
            {
                if (this.reverse)
                {
                    this.CurrentFrame--;
                }
                else
                {
                    this.CurrentFrame++;
                }
                
                if (this.CurrentFrame == this.TotalFrames - 1 || this.CurrentFrame == 0)
                {
                    this.reverse = !this.reverse;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 location, float torsoRotation = 0, float legRotation = 0)
        {
            int width = this.Texture.Width / this.Columns;
            int height = this.Texture.Height / this.Rows;
            int column = this.CurrentFrame % this.Columns;
            var origin = new Vector2(width / 2f, height / 2f);

            Rectangle torsoSource = new Rectangle(width * column, height * TorsoRow, width, height);
            Rectangle legsSource = new Rectangle(width * column, height * LegsRow, width, height);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(this.Texture, null, destinationRectangle, legsSource, origin, legRotation);
            spriteBatch.Draw(this.Texture, null, destinationRectangle, torsoSource, origin, torsoRotation);
            spriteBatch.End();
        }
    }
}
