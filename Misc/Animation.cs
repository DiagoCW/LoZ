using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LoZ.Misc
{
    public class Animation
    {
        private Texture2D texture;
        private int row;
        private int frameCount;
        private int frameWidth;
        private int frameHeight;
        private float frameTime;
        private int currentFrame;
        private float timer;
        private bool isLooping;
        public bool IsFinished
        {
            get
            {
                return !isLooping && currentFrame >= frameCount - 1;
            }
        }

        public Animation(Texture2D texture, int row, int frameCount, int frameWidth, int frameHeight, float frameTime, bool isLooping = true) 
        {
            this.texture = texture;
            this.row = row;
            this.frameCount = frameCount;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.frameTime = frameTime;
            this.isLooping = isLooping;
        }

        public void Reset() 
        {
            currentFrame = 0;
            timer = 0f;
        }

        public void Update(GameTime gameTime) 
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timer >= frameTime) 
            {
                timer = 0f;
                currentFrame++;
                if (currentFrame >= frameCount) 
                {
                    if (isLooping) currentFrame = 0;
                    else currentFrame = frameCount - 1;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, SpriteEffects fx = SpriteEffects.None) 
        {
            Rectangle srcRec = new Rectangle(currentFrame * frameWidth, row * frameHeight, frameWidth, frameHeight);
            spriteBatch.Draw(texture, position, srcRec, color, 0f, Vector2.Zero, 1f, fx, 0f);
        }

    }
}
