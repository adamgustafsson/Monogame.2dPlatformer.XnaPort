using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FuncWorks.XNA.XTiled;

namespace Controller
{
    /// <summary>
    /// MasterController
    /// Main controller class, initializing and running all seperate game controllers as well
    /// as the main model and view classes
    /// </summary>
    public class MasterController : Microsoft.Xna.Framework.Game
    {
        //Variables
        private GraphicsDeviceManager m_graphics;
        private SpriteBatch m_spriteBatch;
        private List<Map> m_TMXMaps;
        //private PerformanceUtility.PerformanceUtility m_performanceTool;

        private GameController m_gameController;
        private View.InputHandler m_inputHandler;
        private View.GameView m_gameView;
        private Model.GameModel m_gameModel;

        //Constructor
        public MasterController()
        {
            this.m_graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.m_graphics.PreferredBackBufferHeight = 720;
            this.m_graphics.PreferredBackBufferWidth = 1280;
            this.m_graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            LoadTMXMaps(Content);//Loading all the TMX maps
            m_spriteBatch = new SpriteBatch(GraphicsDevice);

            //Debugging tools, showing FPS and other relevant information. Press X while debugging to enable
            #region PerformanceTool
            //m_performanceTool = new PerformanceUtility.PerformanceUtility(m_graphics, this);
            //m_performanceTool.LoadContent(Content, m_spriteBatch);
            #endregion

            //Initializing the games main MVC classes
            m_gameModel = new Model.GameModel(m_TMXMaps);
            m_inputHandler = new View.InputHandler();
            m_gameView = new View.GameView(m_gameModel, m_inputHandler, m_spriteBatch, GraphicsDevice);
            m_gameController = new GameController(m_gameModel, m_gameView);

            m_gameView.LoadContent(Content);
        }

        protected void LoadTMXMaps(ContentManager a_content)
        {
            //Initializing Object Drawing for TMX files/maps
            Map.InitObjectDrawing(GraphicsDevice);

            //Loading the TMX maps
            m_TMXMaps = new List<Map>();
            var map = TMXContentProcessor.LoadTMX("Content/Levels/Level-3_org.tmx", a_content);
            m_TMXMaps.Add(map);
        }

        protected override void UnloadContent()
        {
            this.Content.Unload();
        }

        protected override void Update(GameTime a_gameTime)
        {
            //Starting performance watch for update in debug mode (Debug-tool)
            //m_performanceTool.RecordUpdateStart(a_gameTime);

            //Setting keyboard, gamepad and mouse states
            m_inputHandler.SetKeyboardState();
            m_inputHandler.SetMouseState();

            //Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //Updating the Game Controller
            m_gameController.UpdateSimulation((float)a_gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(a_gameTime);

            //Ending performance watch for update in debug mode (Debug-tool)
            //m_performanceTool.RecordUpdateEnd(a_gameTime);

        }

        protected override void Draw(GameTime a_gameTime)
        {
            //Starting performance watch for draw in debug mode (Debug-tool)
            //m_performanceTool.RecordDrawStart(a_gameTime);
            
            //Drawing graphics
            GraphicsDevice.Clear(Color.CornflowerBlue);
            m_gameController.DrawGraphics((float)a_gameTime.ElapsedGameTime.TotalSeconds);
            base.Draw(a_gameTime);

            //Ending performance watch for draw in debug mode (Debug-tool)
            //m_performanceTool.RecordDrawEnd(a_gameTime);
        }

    }
}
