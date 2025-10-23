using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LoZ.Entities;
using LoZ.Managers;
using LoZ.Misc;
using System.Drawing.Text;
using System.Data;

namespace LoZ
{
    public enum GameState
    {
        StartMenu,
        Playing,
        Win,
        GameOver
    }

    public class Game1 : Game
    {
        private GameState currentState = GameState.StartMenu;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameManager gameManager;

        private SpriteFont hud;

        public static Game1 Instance { get; private set; }

        public Game1()
        {
            Instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 800;


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

            DialogueManager.Initialize(GraphicsDevice);
            hud = TextureManager.hudFont;

            gameManager = new GameManager();
            gameManager.Initialize(TextureManager.playerSheet, TextureManager.skeletonSheet,
                TextureManager.cowSheet, TextureManager.chickenSheet);
        }

        protected override void Update(GameTime gameTime)
        {
            MenuManager.Update(gameTime);
            var result = MenuResult.None;

            switch (currentState)
            {
                case GameState.StartMenu:
                    result = MenuManager.HandleInput(MenuType.MainMenu);
                    if (result == MenuResult.StartGame)
                    {
                        QuestManager.Reset();       
                        gameManager.ClearScore();   
                        gameManager.Reset();        
                        currentState = GameState.Playing;
                    }
                    else if (result == MenuResult.Exit)
                    {
                        Exit();
                    }
                    break;

                case GameState.Playing:
                    
                    KeyboardState kb = Keyboard.GetState();
                    if (kb.IsKeyDown(Keys.Escape)) Exit();

                    DialogueManager.Update(gameTime);
                    gameManager.Update(gameTime);

                    if (gameManager.IsPlayerDead)
                    {
                        currentState = GameState.GameOver;
                    }
                    else if (QuestManager.GameWon)
                    {
                        currentState = GameState.Win;
                    }
                    break;

                case GameState.GameOver:
                    result = MenuManager.HandleInput(MenuType.GameOver);
                    if (result == MenuResult.Restart)
                    {
                        QuestManager.Reset();
                        gameManager.ClearScore(); 
                        gameManager.Reset();
                        currentState = GameState.StartMenu;
                    }
                    else if (result == MenuResult.Exit)
                    {
                        Exit();
                    }
                    break;

                case GameState.Win:
                    result = MenuManager.HandleInput(MenuType.Victory);
                    if (result == MenuResult.Restart)
                    {
                        QuestManager.Reset();
                        gameManager.ClearScore();
                        gameManager.Reset();
                        currentState = GameState.StartMenu;
                    }
                    else if (result == MenuResult.Exit)
                    {
                        Exit();
                    }
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            var previousBlend = GraphicsDevice.BlendState;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            switch (currentState)
            {
                case GameState.StartMenu:
                    MenuManager.Draw(spriteBatch, MenuType.MainMenu);
                    break;

                case GameState.Playing:
                    spriteBatch.End();
                    gameManager.Draw(spriteBatch, gameTime);
                    DialogueManager.Draw(spriteBatch);
                    spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                    Vector2 hudPos = new Vector2(10, 10);
                    spriteBatch.DrawString(hud, $"Score: {gameManager.Score}   Lives: {gameManager.Player.Health}", hudPos, Color.White);
                    break;

                case GameState.GameOver:
                    MenuManager.Draw(spriteBatch, MenuType.GameOver, gameManager.Score);
                    break;

                case GameState.Win:
                    MenuManager.Draw(spriteBatch, MenuType.Victory, gameManager.Score);
                    break;
            }

            spriteBatch.End();

            GraphicsDevice.BlendState = previousBlend;

            base.Draw(gameTime);
        }
    }
}
