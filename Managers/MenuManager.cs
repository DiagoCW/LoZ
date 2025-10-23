using LoZ.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LoZ.Managers
{
    public enum MenuType
    {
        MainMenu,
        GameOver,
        Victory
    }

    public enum MenuResult
    {
        None,
        StartGame,
        Restart,
        Exit
    }
    public static class MenuManager
    {
        private static double inputCooldown = 0;
        private const double inputDelay = 0.3;

        public static void Update(GameTime gameTime)
        {
            if (inputCooldown > 0)
                inputCooldown -= gameTime.ElapsedGameTime.TotalSeconds;
        }

        public static MenuResult HandleInput(MenuType type)
        {
            KeyboardState kb = Keyboard.GetState();

            if (inputCooldown > 0)
                return MenuResult.None;

            switch (type)
            {
                case MenuType.MainMenu:
                    if (kb.IsKeyDown(Keys.Enter))
                    {
                        inputCooldown = inputDelay;
                        return MenuResult.StartGame;
                    }
                    if (kb.IsKeyDown(Keys.Escape))
                    {
                        inputCooldown = inputDelay;
                        return MenuResult.Exit;
                    }
                    break;

                case MenuType.GameOver:
                case MenuType.Victory:
                    if (kb.IsKeyDown(Keys.Enter))
                    {
                        inputCooldown = inputDelay;
                        return MenuResult.Restart;
                    }
                    if (kb.IsKeyDown(Keys.Escape))
                    {
                        inputCooldown = inputDelay;
                        return MenuResult.Exit;
                    }
                    break;
            }
            return MenuResult.None;
        }

        public static void Draw(SpriteBatch spriteBatch, MenuType type, int score = 0)
        {
            Texture2D bg = GetBackground(type);

            Rectangle screenRect = new Rectangle(0, 0,
            Game1.Instance.GraphicsDevice.Viewport.Width,
            Game1.Instance.GraphicsDevice.Viewport.Height);

            spriteBatch.Draw(bg, screenRect, Color.White);

            // --- existing text drawing ---
            switch (type)
            {
                case MenuType.MainMenu:
                    DrawCenteredText(spriteBatch, "Press ENTER to Start", Game1.Instance.GraphicsDevice.Viewport.Height * 0.70f, Color.White);
                    DrawCenteredText(spriteBatch, "Press ESCAPE to Exit", Game1.Instance.GraphicsDevice.Viewport.Height * 0.78f, Color.White);
                    break;

                case MenuType.GameOver:
                    DrawCenteredText(spriteBatch, "Game Over", Game1.Instance.GraphicsDevice.Viewport.Height * 0.45f, Color.Red);
                    DrawCenteredText(spriteBatch, $"Score: {score}", Game1.Instance.GraphicsDevice.Viewport.Height * 0.55f, Color.Yellow);
                    DrawCenteredText(spriteBatch, "Press ENTER to Return to Start", Game1.Instance.GraphicsDevice.Viewport.Height * 0.70f, Color.White);
                    DrawCenteredText(spriteBatch, "Press ESCAPE to Exit", Game1.Instance.GraphicsDevice.Viewport.Height * 0.78f, Color.White);
                    break;

                case MenuType.Victory:
                    DrawCenteredText(spriteBatch, "You Won!", Game1.Instance.GraphicsDevice.Viewport.Height * 0.45f, Color.Green);
                    DrawCenteredText(spriteBatch, $"Score: {score}", Game1.Instance.GraphicsDevice.Viewport.Height * 0.55f, Color.Yellow);
                    DrawCenteredText(spriteBatch, "Press ENTER to Return to Start", Game1.Instance.GraphicsDevice.Viewport.Height * 0.70f, Color.White);
                    DrawCenteredText(spriteBatch, "Press ESCAPE to Exit", Game1.Instance.GraphicsDevice.Viewport.Height * 0.78f, Color.White);
                    break;
            }
        }

        private static Texture2D GetBackground(MenuType type)
        {
            return type switch
            {
                MenuType.MainMenu => TextureManager.MenuBackground,
                MenuType.GameOver => TextureManager.GameOverBackground,
                MenuType.Victory => TextureManager.VictoryBackground,
                _ => TextureManager.MenuBackground
            };
        }

        private static void DrawCenteredText(SpriteBatch sb, string text, float y, Color color)
        {
            var font = TextureManager.menuFont;
            float screenWidth = Game1.Instance.GraphicsDevice.Viewport.Width;
            Vector2 size = font.MeasureString(text);
            float x = (screenWidth - size.X) / 2f;
            sb.DrawString(font, text, new Vector2(x, y), color);
        }

    }
}