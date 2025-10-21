using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using LoZ.Misc;
using LoZ.Entities;
using SharpDX.Win32;

namespace LoZ.Managers
{
    internal class NPCManager
    {
        private readonly List<NPC> npcs = new List<NPC>();
        private readonly Texture2D cowSheet;
        private readonly Texture2D chickenSheet;

        public NPCManager(Texture2D cowSheet, Texture2D chickenSheet) 
        {
            this.cowSheet = cowSheet;
            this.chickenSheet = chickenSheet;
        }

        public void Reset() 
        {
            npcs.Clear();
        }

        public List<NPC> GetNPCs()
        {
            return npcs;
        }

        public void LoadFromMap(TiledMap map) 
        {
            var npclayer = map.GetLayer<TiledMapObjectLayer>("NPCs");
            if (npclayer == null) return;

            npcs.Clear();

            foreach (var obj in npclayer.Objects)
            {
                Vector2 pos = new Vector2(obj.Position.X, obj.Position.Y - obj.Size.Height);
                string type = obj.Type ?? "";

                bool collidable = false;
                if (obj.Properties.ContainsKey("Collidable"))
                    bool.TryParse(obj.Properties["Collidable"], out collidable);

                NPC npc = null;

                switch (type)
                {
                    case "BlockingCow":
                        npc = new NPC(pos, cowSheet, NPC.NPCType.BlockingCow);
                        break;

                    case "LostCow":
                        npc = new NPC(pos, cowSheet, NPC.NPCType.LostCow);
                        break;

                    case "Chicken":
                        npc = new NPC(pos, chickenSheet, NPC.NPCType.Chicken);
                        break;
                }

                if (npc != null)
                {
                    npc.isCollidable = collidable; 
                    npcs.Add(npc);
                }
            }
        }

        public bool IsBlockedByNPC(Rectangle nextRec) 
        {
            foreach (var npc in npcs)
            {
                if (npc.HasCollision && npc.NPCHitBox.Intersects(nextRec))
                    return true;
            }
            return false;
        }

        public void Update(GameTime gameTime) 
        {
            foreach (var npc in npcs) 
            {
                npc.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            foreach (var npc in npcs) 
            {
                npc.Draw(spriteBatch);
            }
        }
    }
}
