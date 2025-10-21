using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using LoZ.Misc;
using LoZ.Managers;

namespace LoZ.Managers
{
    public static class MapManager
    {
        private static TiledMap currentMap;
        private static TiledMapRenderer renderer;
        private static TiledMapTileLayer collisionLayer;

        public static TiledMap CurrentMap { get { return currentMap; } }

        public static void LoadMap(GraphicsDevice graphics, string mapName, Microsoft.Xna.Framework.Content.ContentManager content) 
        {
            currentMap = content.Load<TiledMap>("Maps/world");
            renderer = new TiledMapRenderer(graphics, currentMap);
            collisionLayer = currentMap.GetLayer<TiledMapTileLayer>("Collision");
        }
        public static bool IsBlocked(Rectangle bounds) 
        {
            int tileSize = GameConfig.tileSize;

            int leftTile = bounds.Left / tileSize;
            int rightTile = (bounds.Right - 1) / tileSize;
            int topTile = bounds.Top / tileSize;
            int bottomTile = (bounds.Bottom - 1) / tileSize;

            for (int y = topTile; y <= bottomTile; y++)
            {
                for (int x = leftTile; x <= rightTile; x++) 
                {
                    if (collisionLayer.TryGetTile((ushort)x, (ushort)y, out var tile)) 
                    {
                        if (tile.HasValue && tile.Value.GlobalIdentifier != 0)
                            return true;
                    }
                }
            }

            return false;
        }

        public static void Update(GameTime gameTime) 
        {
            renderer.Update(gameTime);
        }

        public static void Draw(GameTime gameTime) 
        {
            renderer.Draw(CameraManager.GetViewMatrix());
        }
    }
}
