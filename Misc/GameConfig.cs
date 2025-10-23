
using Microsoft.Xna.Framework;

namespace LoZ.Misc
{
    public static class GameConfig
    {
        // Tile and world settings

        public const int tileSize = 16; // size of one tile in pixels
        public const int tilesVisibleX = 40; // how many tiles fit horizontally on screen
        public const int tilesVisibleY = 40; // how many tiles fit vertically on screen

        // Map

        public static int virtualWidth = tileSize * tilesVisibleX; // 640 px
        public static int virtualHeight = tileSize * tilesVisibleY; // 640 px

        // Player settings

        public const int playerMaxHealth = 100;
        public const double playerInvulnDuration = 1f;
        public const int playerWidth = 32;
        public const int playerHeight = 32;
        public const float playerSpeed = 400f;
        public const int playerStartTileX = 4 * 16;
        public const int playerStartTileY = 115 * 16;
        public const double attackCooldown = 0.7f;
        public const float playerIdleAnimSpeed = 0.15f;
        public const float playerRunAnimSpeed = 0.10f;
        public const float playerAttackAnimSpeed = 0.2f;
        public const float playerDeathAnimSpeed = 0.12f;

        // Zone logic

        public const int zones = 3;
        public const int zoneHeightPixels = 640;
        
        // Camera settings

        public static readonly Vector2[] zoneCenters = // Zone camera center positions
        {
            new Vector2(virtualWidth / 2f, zoneHeightPixels * 2.5f), // Zone 1 (bottom)
            new Vector2(virtualWidth / 2f, zoneHeightPixels * 1.5f), // Zone 2 (middle)
            new Vector2(virtualWidth / 2f, zoneHeightPixels * 0.5f)  // Zone 3 (top)
        };

        public static readonly Vector2 cameraStartPos 
            = zoneCenters[0];

        // Enemy Skeleton settings

        public const int skeletonHealth = 2;
        public const float skeletonSpeed = 50f;
        public const float skeletonMoveInterval = 2f;
        public const float skeletonIdleAnimSpeed = 0.15f;
        public const float skeletonRunAnimSpeed = 0.10f;
        public const float skeletonDeathAnimSpeed = 0.12f;
        public const float skeletonHitAnimSpeed = 0.14f;
        public const float skeletonHitDuration = 1f;
        public const int skeletonWidth = 32;
        public const int skeletonHeight = 32;

        // NPC settings

        public const int NPCWidth = 32;
        public const int NPCHeight = 32;
        public const float NPCIdleAnimSpeed = 0.25f;
    }
}
