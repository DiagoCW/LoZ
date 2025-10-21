using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using LoZ.Misc;


namespace LoZ.Managers
{
    public static class CameraManager
    {
        public static OrthographicCamera camera { get; private set; }
        private static int currentZone = 0;

        public static void Initialize(GameWindow window, GraphicsDevice graphics) 
        {
            var viewportAdapter = new BoxingViewportAdapter(
                window, graphics, GameConfig.virtualWidth, GameConfig.virtualHeight);

            camera = new OrthographicCamera(viewportAdapter);
        }

        public static void LookAt(Vector2 position) 
        {
            camera.LookAt(position);
        }

        public static void Update(Vector2 playerPos) 
        {
            float y = playerPos.Y;
            int zoneHeight = GameConfig.zoneHeightPixels;
            int newZone;

            if (y < zoneHeight)
            {
                newZone = 2; // top
            }
            else if (y < zoneHeight * 2)
            {
                newZone = 1; // middle
            }
            else
            {
                newZone = 0; // bottom
            }

            if (newZone != currentZone) 
            {
                currentZone = newZone;
                camera.LookAt(GameConfig.zoneCenters[currentZone]);
            }
        }

        public static Matrix GetViewMatrix() 
        {
            return camera.GetViewMatrix();  
        }
    }
}
