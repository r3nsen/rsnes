using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using r3nGraphics;

using r3nGUI;
using r3nGraphics;
using System.IO;

namespace rsnes
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Texture2D tex;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            string fontpath = "C:\\Users\\renat\\OneDrive\\Documentos\\programas\\msdf-atlas-gen-1.2.2-win64\\msdf-atlas-gen\\test.png";
            tex = Content.Load<Texture2D>("724911");
            InterfaceElement.Load(Content, GraphicsDevice, Texture2D.FromStream(GraphicsDevice, File.Open(fontpath, FileMode.Open)), _graphics, Window);
            InterfaceElement.LoadEffects(tex);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(new Color(.05f, .05f, .05f));
            //_gm.Begin(0, 0, Dimension.W, Dimension.H);
            //InterfaceElement.Example();
            InterfaceElement.Begin(0, 0, Dimension.W, Dimension.H);

            InterfaceElement.BeginLayout(dir: LayoutDir.Horizontal);
            InterfaceElement.BeginLayout(dir: LayoutDir.Horizontal, "MarginLayout");//StyleID.Camp);
            InterfaceElement.EndLayout();
            InterfaceElement.BeginLayout(dir: LayoutDir.Horizontal, "LayoutNoRes");// StyleID.noresLay);
            InterfaceElement.Button();
            InterfaceElement.Button();
            InterfaceElement.Button();
            InterfaceElement.EndLayout();
            InterfaceElement.EndLayout();

            InterfaceElement.End();
            GraphicsManager.FlushUI(InterfaceElement.drawStack.Span, _gd: GraphicsDevice);
            InterfaceElement.drawStack = InterfaceElement.drawStack.Slice(0, 0);

            base.Draw(gameTime);
        }
    }
}