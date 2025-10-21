using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using LoZ.Misc;
using LoZ.Managers;

namespace LoZ.Entities
{
    internal class NPC
    {
        public enum NPCType
        {
            BlockingCow,
            LostCow,
            Chicken
        }

        public Vector2 NPCPos;
        public Rectangle NPCHitBox;
        public bool isActive = true;
        public bool hasBeenTalkedTo = false;
        public bool isCollidable = false;

        private NPCType Type;
        private Animation idle;
        private Animation current;

        private int width = GameConfig.NPCWidth;
        private int height = GameConfig.NPCHeight;

        public bool HasCollision
        {
            get
            {
                return isActive && isCollidable && (Type == NPCType.BlockingCow && !QuestManager.LostCowFound);
            }
        }

        public NPC(Vector2 pos, Texture2D sprite, NPCType type)
        {
            Type = type;
            NPCPos = pos;
            NPCHitBox = new Rectangle((int)NPCPos.X, (int)NPCPos.Y, width, height);

            InitializeAnimation(sprite);
        }

        private void InitializeAnimation(Texture2D sprite)
        {
            // Each NPC sprite has an idle animation on row 0 with 2 frames.
            idle = new Animation(sprite, 0, 2, width, height, GameConfig.NPCIdleAnimSpeed);
            current = idle;
        }

        private void DisplayDialogue(string text)
        {
            System.Diagnostics.Debug.WriteLine($"[NPC says]: {text}");
        }

        public void Interact()
        {
            if (!isActive) return;

            switch (Type)
            {
                case NPCType.LostCow:
                    if (!hasBeenTalkedTo)
                    {
                        hasBeenTalkedTo = true;
                        QuestManager.FoundLostCow();
                        DisplayDialogue("Moo! Thanks for finding me!");
                    }
                    else
                    {
                        DisplayDialogue("Thanks again, good luck on your journey!");
                    }
                    break;

                case NPCType.BlockingCow:
                    if (QuestManager.LostCowFound)
                    {
                        DisplayDialogue("Moo! Thanks for helping my friend, you can pass!");
                        isActive = false;
                    }
                    else
                    {
                        DisplayDialogue("Moo! You can't pass until you find my friend!");
                    }
                    break;

                case NPCType.Chicken:
                    if (QuestManager.LostCowFound)
                    {
                        DisplayDialogue("Bawk! Bawk! You saved me! <3");
                        QuestManager.WinGame();
                    }
                    else
                    {
                        DisplayDialogue("Bawk? Have you seen the lost cow?");
                    }
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (!isActive) return;
            current.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isActive) return;
            current.Draw(spriteBatch, NPCPos, Color.White);
        }
    }
}
