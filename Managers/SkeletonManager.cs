using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using System.Collections.Generic;
using LoZ.Entities;
using LoZ.Misc;
using System;

namespace LoZ.Managers
{
    internal class SkeletonManager
    {
        private readonly List<Skeleton> skeletons = new List<Skeleton>();
        private readonly Texture2D skeletonSheet;

        private Random skeletonRandom = new Random();
        
        private int tileSize = GameConfig.tileSize;

        public SkeletonManager(Texture2D skeletonSheet) 
        {
            this.skeletonSheet = skeletonSheet;
        }

        public void Reset() 
        {
            skeletons.Clear();
        }

        public List<Skeleton> GetSkeletons()
        {
            return skeletons;
        }

        public void LoadFromMap(TiledMap map) 
        {
            var enemyLayer = map.GetLayer<TiledMapObjectLayer>("Enemies");
            if (enemyLayer == null) return;

            skeletons.Clear();

            foreach (var obj in enemyLayer.Objects) 
            {
                Vector2 pos = new Vector2(obj.Position.X, obj.Position.Y - obj.Size.Height);
                
                skeletons.Add(new Skeleton(pos, skeletonSheet, skeletonRandom));
            }
        }

        public void Update(GameTime gameTime) 
        {
            foreach (var skeleton in skeletons) 
            {
                skeleton.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            foreach (var skeleton in skeletons)
            {
                skeleton.Draw(spriteBatch);
            }
        }


    }
}
