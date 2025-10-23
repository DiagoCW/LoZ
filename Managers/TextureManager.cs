using LoZ.Entities;
using LoZ.Misc;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LoZ.Managers
{
    public static class TextureManager
    {
        public static Texture2D carrotTex { get; private set; }

        // Sprites
        public static Texture2D playerSheet { get; private set; }
        public static Texture2D skeletonSheet { get; private set; }
        public static Texture2D chickenSheet { get; private set; }
        public static Texture2D cowSheet { get; private set; }

        // Fonts
        public static SpriteFont dialogueFont { get; private set; }
        public static SpriteFont menuFont { get; private set; }
        public static SpriteFont hudFont { get; private set; }

        // Menu Backgrounds
        public static Texture2D MenuBackground { get; private set; }
        public static Texture2D GameOverBackground { get; private set; }
        public static Texture2D VictoryBackground { get; private set; }

        public static void Load(ContentManager content) 
        {
            carrotTex = content.Load<Texture2D>("carrot");
            
            // Sprites
            playerSheet = content.Load<Texture2D>("Sprites/Player");
            skeletonSheet = content.Load<Texture2D>("Sprites/Skeleton");
            chickenSheet = content.Load<Texture2D>("Sprites/Chicken");
            cowSheet = content.Load<Texture2D>("Sprites/Cow");

            // Fonts
            dialogueFont = content.Load<SpriteFont>("Fonts/Dialogue");
            menuFont = content.Load<SpriteFont>("Fonts/menuFont");
            hudFont = content.Load<SpriteFont>("Fonts/HUD");

            // Menu Backgrounds
            MenuBackground = content.Load<Texture2D>("Backgrounds/MenuBackground");
            GameOverBackground = content.Load<Texture2D>("Backgrounds/GameOverBackground");
            VictoryBackground = content.Load<Texture2D>("Backgrounds/VictoryBackground");
        }
        
    }
}
