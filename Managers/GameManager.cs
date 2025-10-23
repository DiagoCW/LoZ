using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using LoZ.Entities;
using LoZ.Misc;
using Microsoft.Xna.Framework.Input;
using System;


namespace LoZ.Managers
{
    internal class GameManager
    {
        private Player player;
        private SkeletonManager skeletonManager;
        private NPCManager npcManager;
        private TiledMap map;
        private int score;

        private int playerLives = GameConfig.playerMaxHealth;

        public int Score { get { return score; } }
        public bool IsPlayerDead { get { return player.IsDead; } }

        public Player Player { get { return player; } }

        public void Initialize(Texture2D playerSheet, Texture2D skeletonSheet, Texture2D cowSheet, Texture2D chickenSheet)
        {
            map = MapManager.CurrentMap;

            player = new Player(
                new Vector2(GameConfig.playerStartTileX, GameConfig.playerStartTileY),
                playerSheet
            );

            skeletonManager = new SkeletonManager(skeletonSheet);
            skeletonManager.LoadFromMap( map );
            skeletonManager.OnSkeletonKilled += AddScore;

            npcManager = new NPCManager(cowSheet, chickenSheet);
            npcManager.LoadFromMap( map );

            CameraManager.LookAt(GameConfig.cameraStartPos);
        }

        private void HandleInput() 
        {
            if (DialogueManager.IsOpen) return;
            
            KeyboardState kb = Keyboard.GetState();

            if (kb.IsKeyDown(Keys.Space))
            {
                return;
            }

            Vector2? moveDir = player.GetIntendedMoveDirection(kb);
            if (moveDir.HasValue) 
            {
                Vector2 nextPos = player.playerPos + moveDir.Value * GameConfig.tileSize;
                Rectangle nextRec = new Rectangle(
                    (int)Math.Round(nextPos.X),
                    (int)Math.Round(nextPos.Y),
                    player.playerRec.Width,
                    player.playerRec.Height);

                bool canMove = !MapManager.IsBlocked(nextRec) && !npcManager.IsBlockedByNPC(nextRec);
                if (canMove)
                    player.MoveToNextTile(moveDir.Value);
                else
                    player.Stop();
            }
            if (kb.IsKeyDown(Keys.E) && !DialogueManager.IsOpen && !DialogueManager.RequiresKeyRelease)
            {
                HandleNPCInteraction();
            }
        }


        private void HandleCollisions(GameTime gameTime) 
        {
            if (player.CanDealDamage()) 
            {
                Rectangle attackBox = player.GetAttackHitBox();
                foreach (var skeleton in skeletonManager.GetSkeletons()) 
                {
                    if (!skeleton.IsDead && attackBox.Intersects(skeleton.enemyRec)) 
                    {
                        skeleton.Hit(gameTime, true, attackerPos: player.playerPos);
                    }
                }
            }

            if (!player.IsInvulnerable) 
            {
                foreach (var skeleton in skeletonManager.GetSkeletons()) 
                {
                    if (skeleton.IsDead) continue;

                    if (!player.IsAttacking && IsCloseEnoughForDamage(player.playerRec, skeleton.enemyRec))
                    {
                        player.TakeDamage();
                        break;
                    }
                }
            }
        }

        private bool IsCloseEnoughForDamage(Rectangle playerRec, Rectangle enemyRec) 
        {
            // Slightly shrink rectangles to make the collision less sensitive
            Rectangle p = new Rectangle(playerRec.X + 4, playerRec.Y + 4, playerRec.Width - 8, playerRec.Height - 8);
            Rectangle e = new Rectangle(enemyRec.X + 4, enemyRec.Y + 4, enemyRec.Width - 8, enemyRec.Height - 8);
            return p.Intersects(e);
        }

        private void HandleNPCInteraction()
        {
            foreach (var npc in npcManager.GetNPCs())
            {
                if (!npc.isActive) continue;

                float distance = Vector2.Distance(player.playerPos, npc.NPCPos);
                if (distance < GameConfig.tileSize * 3f) 
                {
                    npc.Interact();
                    break;
                }
            }
        }

        private void AddScore(int amount)
        {
            score += amount;
        }

        public void Reset() 
        {
            player.Revive();

            skeletonManager.Reset();
            npcManager.Reset();

            skeletonManager.LoadFromMap(MapManager.CurrentMap);
            npcManager.LoadFromMap(MapManager.CurrentMap);

            QuestManager.Reset();

            CameraManager.LookAt(GameConfig.cameraStartPos);
        }

        public void ClearScore()
        {
            score = 0;
        }

        public void Update(GameTime gameTime)
        {
            HandleInput();

            player.Update(gameTime);
            skeletonManager.Update(gameTime);
            npcManager.Update(gameTime);

            HandleCollisions(gameTime);

            CameraManager.Update(player.playerPos);
           
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            MapManager.Draw(gameTime);

            spriteBatch.Begin(transformMatrix: CameraManager.GetViewMatrix(), samplerState: SamplerState.PointClamp);
            skeletonManager.Draw(spriteBatch);
            npcManager.Draw(spriteBatch);
            player.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}