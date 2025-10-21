using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using LoZ.Entities;
using LoZ.Misc;
using Microsoft.Xna.Framework.Input;


namespace LoZ.Managers
{
    internal class GameManager
    {
        private Player player;
        private SkeletonManager skeletonManager;
        private NPCManager npcManager;
        private TiledMap map;

        private int playerLives = GameConfig.playerMaxHealth;

        public void Initialize(Texture2D playerSheet, Texture2D skeletonSheet, Texture2D cowSheet, Texture2D chickenSheet)
        {
            map = MapManager.CurrentMap;

            player = new Player(
                new Vector2(GameConfig.playerStartTileX, GameConfig.playerStartTileY),
                playerSheet
            );

            skeletonManager = new SkeletonManager(skeletonSheet);
            skeletonManager.LoadFromMap( map );

            npcManager = new NPCManager(cowSheet, chickenSheet);
            npcManager.LoadFromMap( map );

            CameraManager.LookAt(GameConfig.cameraStartPos);
        }

        private void Combat(GameTime gameTime) 
        {
            Rectangle attackBox = player.GetAttackHitBox();
            
            foreach (var skeleton in skeletonManager.GetSkeletons()) 
            {
                if (skeleton.IsDead) continue;

                if (player.IsAttacking && attackBox.Intersects(skeleton.enemyRec))
                {
                    skeleton.Hit(gameTime, true, attackerPos: player.playerPos);
                    if (skeleton.IsDead) skeleton.enemyRec = Rectangle.Empty;
                }

                else if (!player.IsAttacking && IsCloseEnoughForDamage(player.playerRec, skeleton.enemyRec)) 
                {
                    player.TakeDamage();
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
            KeyboardState kb = Keyboard.GetState();

            if (kb.IsKeyDown(Keys.E)) 
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
        }

        public void Reset() 
        {
            playerLives = GameConfig.playerMaxHealth;
            skeletonManager.Reset();
            npcManager.Reset();
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();

            if (!kb.IsKeyDown(Keys.Space)) 
            {
                Vector2? moveDir = player.GetIntendedMoveDirection(kb);
                if (moveDir.HasValue)
                {
                    Rectangle nextRec = player.GetNextRectangle(moveDir.Value);

                    bool canMove = !MapManager.IsBlocked(nextRec) && !npcManager.IsBlockedByNPC(nextRec);
                    if (canMove)
                        player.MoveToNextTile(moveDir.Value);
                    else
                        player.Stop();
                }
            }

            Combat(gameTime);
            HandleNPCInteraction();

            player.Update(gameTime);
            skeletonManager.Update(gameTime);
            npcManager.Update(gameTime);

            CameraManager.Update(player.playerPos);

            if (Keyboard.GetState().IsKeyDown(Keys.P))
                System.Diagnostics.Debug.WriteLine($"PlayerRect: {player.playerRec}, Width:{player.playerRec.Width}, Height:{player.playerRec.Height}");
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            MapManager.Draw(gameTime);

            spriteBatch.Begin(transformMatrix: CameraManager.GetViewMatrix());
            skeletonManager.Draw(spriteBatch);
            npcManager.Draw(spriteBatch);
            player.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}