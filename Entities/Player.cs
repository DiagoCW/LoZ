using LoZ.Managers;
using LoZ.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace LoZ.Entities
{
    public enum Facing 
    {
        Down,
        Up,
        Left,
        Right
    }
    internal class Player
    {
        public Vector2 playerPos;
        public Rectangle playerRec;

        private int health = GameConfig.playerMaxHealth;
        private bool isInvuln;
        private double invulnTimer = 0;
        private double invulnDuration = GameConfig.playerInvulnDuration;

        private Vector2 moveDir;
        private bool isMoving;
        private Vector2 moveTarget;
        private Facing facing = Facing.Down;

        private bool isAttacking;
        private double attackTimer = 0;
        private double attackCooldown = GameConfig.attackCooldown;

        private bool isDead;

        private int width = GameConfig.playerWidth;
        private int height = GameConfig.playerHeight;

        private Animation idleDown, idleRight, idleUp;
        private Animation runDown, runRight, runUp;
        private Animation fightDown, fightRight, fightUp;
        private Animation death;
        private Animation current;

        public bool IsAttacking { get { return isAttacking; } }
        public bool IsInvulnerable { get { return isInvuln; } }
        public bool IsDead { get { return isDead; } }
        public int Health { get { return health; } }

        public Player(Vector2 startPos, Texture2D spriteSheet) 
        {
            

            playerPos = startPos;
            playerRec = new Rectangle((int)playerPos.X, (int)playerPos.Y, width, height);
            moveTarget = playerPos;

            InitializeAnimations(spriteSheet);
            current = idleDown;
        }

        private void InitializeAnimations(Texture2D spriteSheet) 
        {
            // Player spritesheet has 10 rows (0 - 9), each row represents:
            // 0-2: idle (down, right, up)
            // 3-5: run (down, right, up)
            // 6-8: fight (down, right, up)
            // 9: player death
            // Rows 0 - 5 have 6 frames each. Rows 6 - 9 have 4 frames each.

            idleDown = new Animation(spriteSheet, 0, 6, width, height, GameConfig.playerIdleAnimSpeed);
            idleRight = new Animation(spriteSheet, 1, 6, width, height, GameConfig.playerIdleAnimSpeed);
            idleUp = new Animation(spriteSheet, 2, 6, width, height, GameConfig.playerIdleAnimSpeed);

            runDown = new Animation(spriteSheet, 3, 6, width, height, GameConfig.playerRunAnimSpeed);
            runRight = new Animation(spriteSheet, 4, 6, width, height, GameConfig.playerRunAnimSpeed);
            runUp = new Animation(spriteSheet, 5, 6, width, height, GameConfig.playerRunAnimSpeed);

            fightDown = new Animation(spriteSheet, 6, 4, width, height, GameConfig.playerAttackAnimSpeed) { IsLooping = false };
            fightRight = new Animation(spriteSheet, 7, 4, width, height, GameConfig.playerAttackAnimSpeed) { IsLooping = false };
            fightUp = new Animation(spriteSheet, 8, 4, width, height, GameConfig.playerAttackAnimSpeed) { IsLooping = false };

            death = new Animation(spriteSheet, 9, 4, width, height, GameConfig.playerDeathAnimSpeed) { IsLooping = false };
        }

        private void SetIdle()
        {
            switch (facing)
            {
                case Facing.Up: current = idleUp; break;
                case Facing.Down: current = idleDown; break;
                case Facing.Right:
                case Facing.Left: current = idleRight; break;
            }
        }

        public Vector2? GetIntendedMoveDirection(KeyboardState kb)
        {
            if (isMoving || isAttacking) return null;

            Vector2 moveDir = Vector2.Zero;

            if (kb.IsKeyDown(Keys.W)) { moveDir = new Vector2(0, -1); facing = Facing.Up; }
            else if (kb.IsKeyDown(Keys.S)) { moveDir = new Vector2(0, 1); facing = Facing.Down; }
            else if (kb.IsKeyDown(Keys.A)) { moveDir = new Vector2(-1, 0); facing = Facing.Left; }
            else if (kb.IsKeyDown(Keys.D)) { moveDir = new Vector2(1, 0); facing = Facing.Right; }

            if (moveDir == Vector2.Zero) { return null; }
            else return moveDir;
        }

        public Rectangle GetNextRectangle(Vector2 moveDir)
        {
            Vector2 target = playerPos + moveDir * GameConfig.tileSize;
            return new Rectangle((int)Math.Round(target.X), (int)Math.Round(target.Y), width, height);
        }

        public void MoveToNextTile(Vector2 moveDir)
        {
            if (isMoving || isDead) return;

            this.moveDir = moveDir;
            moveTarget = playerPos + moveDir * GameConfig.tileSize;
            isMoving = true;

            switch (facing)
            {
                case Facing.Up: current = runUp; break;
                case Facing.Down: current = runDown; break;
                case Facing.Left:
                case Facing.Right: current = runRight; break;
            }
        }

        public void Stop()
        {
            isMoving = false;
            playerPos.X = (float)Math.Round(playerPos.X / GameConfig.tileSize) * GameConfig.tileSize;
            playerPos.Y = (float)Math.Round(playerPos.Y / GameConfig.tileSize) * GameConfig.tileSize;
            SetIdle();
        }

        private void Attack(KeyboardState kb, GameTime gameTime)
        {
            
            attackTimer -= gameTime.ElapsedGameTime.TotalSeconds;

            if (kb.IsKeyDown(Keys.Space) && attackTimer <= 0)
            {
                isAttacking = true;
                Stop();
                attackTimer = attackCooldown;

                switch (facing)
                {
                    case Facing.Up: current = fightUp; break;
                    case Facing.Down: current = fightDown; break;
                    case Facing.Right:
                    case Facing.Left: current = fightRight; break;
                }

                current.Reset();
            }

            if (isAttacking)
            {
                if (current.IsFinished)
                {
                    isAttacking = false;
                    SetIdle();
                }
            }
        }

        public Rectangle GetAttackHitBox() 
        {

            Rectangle hitBox = playerRec;
            int size = GameConfig.tileSize;

            switch (facing) 
            {
                case Facing.Up: hitBox.Offset(0, -size); break;
                case Facing.Down: hitBox.Offset(0, size); break;
                case Facing.Left: hitBox.Offset(-size, 0); break;
                case Facing.Right: hitBox.Offset(size, 0); break;
            }

            return hitBox;
        }

        public void TakeDamage() 
        {
            if (isInvuln || isDead) return;

            health--;

            if (health <= 0) 
            {
                health = 0;
                Die();
                return;
            }

            isInvuln = true;
            invulnTimer = 0f;
        }

        private void Die() 
        {
            if (isDead) return;

            isDead = true;
            current = death;
            current.Reset();
        }

        public void Update(GameTime gameTime) 
        {
            if (isDead) 
            {
                current.Update(gameTime);
                return;
            }
            
            KeyboardState kb = Keyboard.GetState();
            Attack(kb, gameTime);

            if (isMoving)
            {
                Vector2 newPos = playerPos + moveDir * GameConfig.playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                bool passedTarget = false;
                if (moveDir.X > 0 && newPos.X >= moveTarget.X) passedTarget = true;
                else if (moveDir.X < 0 && newPos.X <= moveTarget.X) passedTarget = true;
                else if (moveDir.Y > 0 && newPos.Y >= moveTarget.Y) passedTarget = true;
                else if (moveDir.Y < 0 && newPos.Y <= moveTarget.Y) passedTarget = true;

                if (passedTarget)
                {
                    playerPos = moveTarget;
                    isMoving = false;
                    SetIdle();
                }
                else 
                {
                    playerPos = newPos;
                }
            }

            current.Update(gameTime);
            playerRec.Location = new Point((int)Math.Round(playerPos.X), (int)Math.Round(playerPos.Y));

            if (isInvuln) 
            {
                invulnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (invulnTimer > invulnDuration)
                    isInvuln = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            SpriteEffects fx = (facing == Facing.Left) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Color tint = (isInvuln && !isDead) ? Color.Red * 0.6f : Color.White;

            current.Draw(spriteBatch, playerPos, tint, fx);
        }
        
    }
}
