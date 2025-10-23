using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LoZ.Misc;

namespace LoZ.Managers
{
    public static class DialogueManager
    {
        private static readonly Queue<string> queue = new Queue<string>();
        private static string currentFull = "";
        private static string currentVisible = "";
        private static int charIndex = 0;

        private static bool open;
        private static bool isTyping;

        private static SpriteFont font;
        private static Texture2D panelTex;
        private static Rectangle panelRec;

        private const float cps = 45f; // Typing speed, Character per second
        private static float typeTimer;

        private static KeyboardState prevKb;

        public static bool RequiresKeyRelease { get; private set; }
        public static bool IsOpen { get { return open; } }

        public static void Initialize(GraphicsDevice graphics) 
        {
            font = TextureManager.dialogueFont;

            panelTex = new Texture2D(graphics, 1, 1);
            panelTex.SetData(new[] { Color.White });

            const int margin = 12;
            int w = GameConfig.virtualWidth - margin * 2;
            int h = 120;
            int x = margin;
            int y = GameConfig.virtualHeight - h - margin;
            panelRec = new Rectangle(x, y, w, h);
        }

        public static void Show(string text) 
        {
            if (string.IsNullOrEmpty(text)) return;

            queue.Enqueue(text);
            if (!open) 
            {
                DequeueNext();
                open = true;
            }
        }

        private static void DequeueNext() 
        {
            if (queue.Count == 0) 
            {
                open = false;
                currentFull = currentVisible = "";
                charIndex = 0;
                typeTimer = 0f;
                isTyping = false;
                RequiresKeyRelease = true;

                return;
            }

            currentFull = queue.Dequeue();
            currentVisible = "";
            charIndex = 0;
            typeTimer = 0f;
            isTyping = true;
        }

        public static void Update(GameTime gameTime) 
        {
            var kb = Keyboard.GetState();

            if (RequiresKeyRelease)
            {
                if (!kb.IsKeyDown(Keys.E) && !kb.IsKeyDown(Keys.Space))
                    RequiresKeyRelease = false;

                prevKb = kb;
                return; 
            }

            if (!open) { prevKb = Keyboard.GetState(); return; }

            bool advancePressed = Pressed(kb, prevKb, Keys.Space) || Pressed(kb, prevKb, Keys.E);

            if (isTyping)
            {
                typeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                int shouldBe = (int)(typeTimer * cps);

                if (shouldBe > charIndex)
                {
                    charIndex = MathHelper.Clamp(shouldBe, 0, currentFull.Length);
                    currentVisible = currentFull.Substring(0, charIndex);
                }

                if (charIndex >= currentFull.Length)
                {
                    isTyping = false;
                }

                if (advancePressed && isTyping)
                {
                    isTyping = false;
                    currentVisible = currentFull;
                    charIndex = currentFull.Length;
                }
            }
            else 
            {
                if (advancePressed)
                    DequeueNext();
            }

            prevKb = kb;
        }

        public static void Draw(SpriteBatch spriteBatch) 
        {
            if (!open) return;

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            spriteBatch.Draw(panelTex, panelRec, Color.Black * 0.7f);

            Vector2 padding = new Vector2(12, 10);

            string wrapped = WrapText(font, currentVisible, panelRec.Width - (int)padding.X * 2);
            spriteBatch.DrawString(font, wrapped, new Vector2(panelRec.X, panelRec.Y) + padding, Color.White);

            if (!isTyping) 
            {
                const string hint = "[E / Space]";
                Vector2 hintSize = font.MeasureString(hint);
                Vector2 hintPos = new Vector2(panelRec.Right - hintSize.X - 10, panelRec.Bottom - hintSize.Y - 6);
                spriteBatch.DrawString(font, hint, hintPos, Color.White * 0.75f);
            }

            spriteBatch.End();
        }

        private static bool Pressed(KeyboardState kb, KeyboardState prev, Keys key)
        {
            return kb.IsKeyDown(key) && prev.IsKeyUp(key);
        }

        private static string WrapText(SpriteFont spriteFont, string text, int maxLineWidth) 
        {
            if (string.IsNullOrEmpty(text)) return "";

            string[] words = text.Split(' ');
            string line = "";
            string result = "";

            foreach (var w in words) 
            {
                string test = (line.Length == 0) ? w : line + " " + w;
                if (spriteFont.MeasureString(test).X <= maxLineWidth) 
                {
                    line = test;
                }
                else
                {
                    if (result.Length > 0) result += "\n";
                    result += line;
                    line = w;
                }
            }

            if (line.Length > 0) 
            {
                if (result.Length > 0) result += "\n";
                result += line;
            }

            return result;
        }
    }
}
