using LoZ.Entities;
using LoZ.Misc;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LoZ.Managers
{
    public static class TextureManager
    {
        public static Texture2D playerSheet;
        public static Texture2D skeletonSheet;
        public static Texture2D chickenSheet;
        public static Texture2D cowSheet;

        public static void Load(ContentManager content) 
        {
            playerSheet = content.Load<Texture2D>("Sprites/Player");
            skeletonSheet = content.Load<Texture2D>("Sprites/Skeleton");
            chickenSheet = content.Load<Texture2D>("Sprites/Chicken");
            cowSheet = content.Load<Texture2D>("Sprites/Cow");
        }
        
    }
}
