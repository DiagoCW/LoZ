using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LoZ.Entities;
using LoZ.Managers;
using LoZ.Misc;

namespace LoZ
{
    //public enum GameState 
    //{
    //Menu,
    //Playing,
    //Win,
    //GameOver
    //}

    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameManager gameManager;

        //private GameState currentState = GameState.Playing;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = 800; //GameConfig.virtualWidth;
            graphics.PreferredBackBufferHeight = 800; //GameConfig.virtualHeight;
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            CameraManager.Initialize(Window, GraphicsDevice);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            MapManager.LoadMap(GraphicsDevice, "world", Content);
            TextureManager.Load(Content);

            gameManager = new GameManager();
            gameManager.Initialize(TextureManager.playerSheet, TextureManager.skeletonSheet,
                TextureManager.cowSheet, TextureManager.chickenSheet);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            gameManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            
            var previousBlend = GraphicsDevice.BlendState;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            gameManager.Draw(spriteBatch, gameTime);

            GraphicsDevice.BlendState = previousBlend;

            base.Draw(gameTime);
        }
    }
}
