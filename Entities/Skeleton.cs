using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LoZ.Misc;
using LoZ.Managers;
using System;

namespace LoZ.Entities
{
    internal class Skeleton
    {
        public Vector2 enemyPos;
        public Rectangle enemyRec;
        
        private int health = GameConfig.skeletonHealth;

        private bool isMoving;
        private bool isDead;
        private bool isHit;
        private Facing facing = Facing.Down;

        private Vector2 moveTarget;
        private Vector2 direction;

        // Sprite height / width
        private int height = GameConfig.skeletonHeight;
        private int width = GameConfig.skeletonWidth;

        private float moveSpeed = GameConfig.skeletonSpeed;
        private float moveTimer;
        private float moveInterval = GameConfig.skeletonMoveInterval;

        private float hitTimer;
        private float hitDuration = GameConfig.skeletonHitDuration;

        private Animation idleDown, idleRight, idleUp;
        private Animation runDown, runRight, runUp;
        private Animation hitDown, hitRight, hitUp;
        private Animation death;
        private Animation current;

        public event Action<Skeleton> OnDied;

        private Random rnd;

        public bool IsDead { get { return isDead; } }
        public bool IsHit { get { return isHit; } }

        public Skeleton(Vector2 startPos, Texture2D spriteSheet, Random random)
        {
            rnd = random;
            
            enemyPos = startPos;
            enemyRec = new Rectangle((int)enemyPos.X, (int)enemyPos.Y, width, height);
            
            InitializeAnimations(spriteSheet);
            current = idleDown;
            moveTarget = enemyPos;

            moveTimer = (float)rnd.NextDouble() * moveInterval;
            moveInterval = GameConfig.skeletonMoveInterval + (float)rnd.NextDouble();

            float speedVariation = (float)rnd.NextDouble() * 0.4f + 1f;
            moveSpeed = GameConfig.skeletonSpeed * speedVariation;
        }

        private void InitializeAnimations(Texture2D sheet)
        {
            // Skeleton spritesheet has 10 rows (0 - 9), each row represents:
            // 0-2: idle (down, right, up)
            // 3-5: run (down, right, up)
            // 6: Skeleton death 
            // 7-9: Skeleton hit (down, right, up)
            // Rows 0 - 5 have 6 frames each. Rows 6 - 9 have 4 frames each.

            float idleSpeed = GameConfig.skeletonIdleAnimSpeed;
            float runSpeed = GameConfig.skeletonRunAnimSpeed;
            float deathSpeed = GameConfig.skeletonDeathAnimSpeed;
            float hitSpeed = GameConfig.skeletonHitAnimSpeed;

            idleDown = new Animation(sheet, 0, 6, width, height, idleSpeed);
            idleRight = new Animation(sheet, 1, 6, width, height, idleSpeed);
            idleUp = new Animation(sheet, 2, 6, width, height, idleSpeed);

            runDown = new Animation(sheet, 3, 6, width, height, runSpeed);
            runRight = new Animation(sheet, 4, 6, width, height, runSpeed);
            runUp = new Animation(sheet, 5, 6 , width, height, runSpeed);

            death = new Animation(sheet, 6, 4, width, height, deathSpeed, false);

            hitDown = new Animation(sheet, 7, 4, width, height, hitSpeed, false);
            hitRight = new Animation(sheet, 8, 4, width, height, hitSpeed, false);
            hitUp = new Animation(sheet, 9, 4, width, height, hitSpeed, false); 
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

        private void ChooseNewDirection() 
        {
            int dir = rnd.Next(4);
            switch (dir)
            {
                case 0: direction = new Vector2(0, -1); facing = Facing.Up; break;
                case 1: direction = new Vector2(0, 1); facing = Facing.Down; break;
                case 2: direction = new Vector2(-1, 0); facing = Facing.Left; break;
                case 3: direction = new Vector2(1, 0); facing = Facing.Right; break;
            }
            moveTarget = enemyPos + direction * GameConfig.tileSize;
            switch (facing) 
            {
                case Facing.Up:
                    current = runUp;
                    break;
                case Facing.Down:
                    current = runDown;
                    break;
                case Facing.Right:
                case Facing.Left:
                    current = runRight;
                    break;
            }
        }

        private void Movement(GameTime gameTime) 
        {
            if (enemyPos == moveTarget) 
            {
                if (isMoving) 
                {
                    isMoving = false;
                    SetIdle();
                }
                return;
            }

            Vector2 moveDir = moveTarget - enemyPos;
            if (moveDir != Vector2.Zero) 
            {
                moveDir.Normalize();
            }

            Vector2 newPos = enemyPos + moveDir * moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            bool passedTarget = false;
            if (moveDir.X > 0 && newPos.X >= moveTarget.X) passedTarget = true;
            else if (moveDir.X < 0 && newPos.X <= moveTarget.X) passedTarget = true;
            else if (moveDir.Y > 0 && newPos.Y >= moveTarget.Y) passedTarget = true;
            else if (moveDir.Y < 0 && newPos.Y <= moveTarget.Y) passedTarget = true;

            Vector2 finalPos;
            if (passedTarget)
            {
                finalPos = moveTarget;
                isMoving = false;
                SetIdle();
            }
            else 
            {
                finalPos = newPos;
                if (!isMoving) isMoving = true;
            }

            Rectangle targetRec = new Rectangle((int)Math.Round(finalPos.X), (int)Math.Round(finalPos.Y), enemyRec.Width, enemyRec.Height);

            if (!MapManager.IsBlocked(targetRec))
            {
                enemyPos = finalPos;
            }
            else 
            {
                moveTarget = enemyPos;
                isMoving = false;
                SetIdle();
            }
        }

        public void Hit(GameTime gameTime, bool wasAttacked = false, Vector2? attackerPos = null) 
        {
            if (isDead) return;

            if (wasAttacked && !isHit)
            {
                isHit = true;
                hitTimer = 0f;
                health--;

                if (attackerPos.HasValue)
                {
                    Vector2 tileOffset = enemyPos - attackerPos.Value;

                    if (tileOffset != Vector2.Zero)
                    {
                        tileOffset.Normalize();
                        Vector2 target = enemyPos + tileOffset * GameConfig.tileSize;

                        Rectangle targetRec = new Rectangle((int)target.X, (int)target.Y, width, height);
                        if (!MapManager.IsBlocked(targetRec))
                        {
                            moveTarget = target;
                            isMoving = true;
                        }
                    }
                }

                if (health <= 0) 
                {
                    Die();
                    return;
                }

                switch (facing)
                {
                    case Facing.Up:
                        current = hitUp;
                        break;
                    case Facing.Down:
                        current = hitDown;
                        break;
                    case Facing.Right:
                    case Facing.Left:
                        current = hitRight;
                        break;
                }
                current.Reset();
                return;
            }

            if (isHit) 
            {
                if (hitTimer >= hitDuration && current.IsFinished) 
                {
                    isHit = false;
                    SetIdle();
                }
            }
        }

        public void Die() 
        {
            if (isDead) return;

            isDead = true;
            current = death;
            current.Reset();
            enemyRec = Rectangle.Empty;
            OnDied?.Invoke(this);
        }

        public void Update(GameTime gameTime) 
        {
            if (isDead)
            {
                current.Update(gameTime);
                return;
            }

            if (isHit)
            {
                hitTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (hitTimer >= hitDuration || current.IsFinished)
                {
                    isHit = false;
                    SetIdle();
                }
            }
            else if (!isHit)
            {
                moveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (moveTimer >= moveInterval && !isMoving)
                {
                    moveTimer = 0;
                    ChooseNewDirection();
                }
            }

            Movement(gameTime);

            current.Update(gameTime);

            enemyRec.Location = enemyPos.ToPoint();
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            SpriteEffects fx = (facing == Facing.Left) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            current.Draw(spriteBatch, enemyPos, Color.White, fx);
        }

    }
}
