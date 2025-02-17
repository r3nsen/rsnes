
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using r3nGUI;





//using SourceGenerator;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace r3nGraphics
{
    public static class EffectType
    {
        public struct EffectStruct
        {
            public string Name;
            public int ID;
        }
        public static string[] sEffectType = {

            Retangulo ,
            Text ,
        };
        static int currID = 0;
        public const string Retangulo = "retângulo";//n        
        public const string Text = "sdfTextRender";//n        

        // ID        
        public static int Retangulo_id = currID++;
        public static int Text_id = currID++;

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MyVertexType : IVertexType
    {
        public Vector3 Position;
        //public Vector3 Data;
        public Vector2 TextureCoordinate;
        public Vector4 Color;
        public Vector4 outlineColor;
        public Vector2 size;


        public static readonly VertexDeclaration VertexDeclaration;
        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public MyVertexType(Vector3 position, Vector2 textureCoordinate, Vector4 color, Vector4 out_color, Vector2 _size)
        {
            Position = position;
            TextureCoordinate = textureCoordinate;
            Color = color;
            outlineColor = out_color;
            size = _size;
        }

        static MyVertexType()
        {
            VertexDeclaration = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                //new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(20, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
                new VertexElement(36, VertexElementFormat.Vector4, VertexElementUsage.Color, 1),
                new VertexElement(52, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1)
                );
        }
    }
    public struct Dimension
    {
        public static GraphicsDeviceManager g;
        public static GraphicsAdapter Adapter;
        public static GameWindow window;
        public static int W => window.ClientBounds.Width;//g.PreferredBackBufferWidth;
        public static int H => window.ClientBounds.Height;//g.PreferredBackBufferHeight;
        public static float zoom = 1;
        public static float zW { get => (zoom * W); }
        public static float zH { get => (zoom * H); }

        public static int screenW => Adapter.CurrentDisplayMode.Width;
        public static int screenH => Adapter.CurrentDisplayMode.Height;
    }    
    public partial class GraphicsManager
    {
        struct Objects
        {
            public MyVertexType[] vertex;
            public int vertexCount, indexCount;
            public short[] index;
        }

        public static GraphicsDevice gd;
        Objects[] sdfObjects;// = new Objects[21];

        //Matrix view;
        //Matrix projection;

        // -- paleta customizavel:
        // --- background
        // --- Player x
        // --- elementos safe
        // --- elementos perigosos
        // ---- Key / door

        //public static int colorIndex = 2;
        static bool changeColorPressed = false;

        RenderTarget2D render, tempscreen;
        public static RenderTarget2D PauseTarget1, PauseTarget2, PauseTarget3, PauseTargetFinal, PauseOut, PauseTargetOut;


        public static SpriteFont sprfont;
        public static ContentManager _content;
        //internal Effect effect;

        public static Dictionary<string, EffectStruct> effectList = new Dictionary<string, EffectStruct>();

        //public static Dictionary<string, EffectStruct<(void,void)>> effectList;

        internal static GameTime gt;
        public void setRenderTargets()
        {
            int w = Dimension.W * 2;
            int h = Dimension.H * 2;
            //int h = gd.Viewport.Height * 2;
            if (w * h == 0)
                return;
            //long a1 = GC.GetTotalMemory(true);
            if (render != null)
                render.Dispose();
            render = null;

            if (PauseTarget1 != null)
                PauseTarget1.Dispose();
            PauseTarget1 = null;
            if (PauseTarget2 != null)
                PauseTarget2.Dispose();
            PauseTarget2 = null;
            if (PauseTarget3 != null)
                PauseTarget3.Dispose();
            PauseTarget3 = null;
            if (PauseTargetFinal != null)
                PauseTargetFinal.Dispose();
            PauseTargetFinal = null;
            //long a2 = GC.GetTotalMemory(true);


            GC.Collect();
            //long a3 = GC.GetTotalMemory(true);

            render = new RenderTarget2D(gd, w, h, false, SurfaceFormat.Vector4, DepthFormat.None);
            float p2r(float value)
            {
                return (float)Math.Pow(2, Math.Ceiling(Math.Log2(value)));
            }
            //float DMax = Math.Max(Dimension.W, Dimension.H);
            Vector2 pauseSize = new Vector2(Dimension.W, Dimension.H);// new Vector2(p2r(Dimension.W), p2r(Dimension.H));
            PauseTarget1 = new RenderTarget2D(gd, Dimension.W, Dimension.H);
            PauseTarget2 = new RenderTarget2D(gd, Dimension.W, Dimension.H);
            PauseTarget3 = new RenderTarget2D(gd, Dimension.W, Dimension.H);
            PauseTargetFinal = new RenderTarget2D(gd, Dimension.W, Dimension.H);
            PauseOut = new RenderTarget2D(gd, Dimension.W, Dimension.H);
            PauseTargetOut = new RenderTarget2D(gd, Dimension.W, Dimension.H);
        }
        public GraphicsManager(GraphicsDevice graphicsDevice)
        {
            gd = graphicsDevice;

            setRenderTargets();

            int objNum = EffectType.sEffectType.Length;//(int)EffectType.SIZE;//44;// - 1;
            sdfObjects = new Objects[objNum];

            //for (int i = 0; i < sdfObjects.Length; i++)
            for (int i = 0; i < objNum; i++)
            {
                sdfObjects[i] = new Objects()
                {
                    vertex = new MyVertexType[4],
                    index = new short[6]
                };                
            }
        }
        public void LoadShader()
        {
            foreach (var s in EffectType.sEffectType)
                effectList.Add(s, new EffectStruct(ShaderManager.ShaderCompiler(gd, s, _content)));
        }
        public void textColor(Color cor)
        {
            effectList[EffectType.Text].effect.Parameters["color"].SetValue(cor.ToVector3());
        }
        public void ReloadShader()
        {
            foreach (var p in effectList)
            {
                Effect temp = ShaderManager.ShaderCompiler(gd, p.Key, _content);

                if (temp != null) p.Value.Set(temp);
            }
        }
        public void SetColors()
        {
            Vector3[] colors = new Vector3[3];
            colors[0] = new Vector3(200.0f / 255.0f, 10.0f / 255.0f, 50.0f / 255.0f);   // Red
            colors[1] = new Vector3(220.0f / 255.0f, 220.0f / 255.0f, 220.0f / 255.0f); // White
            colors[2] = new Vector3(30.0f / 255.0f, 30.0f / 255.0f, 30.0f / 255.0f);    // Black

        }
        double fakeTimer = 0;
        bool oneTime = false;
        float a = 0;
        public void Begin(float x, float y, int w, int h/*, float a = 0*/)
        {
            //w /= 2;
            //h /= 2;
            Matrix view;
            Matrix projection;

            gd.Viewport = new Viewport(100, 100, 100, 100);
            ReloadShader();

            Vector3 pos = new Vector3(0, 0, 1);
            Vector3 target = Vector3.Zero;
            Vector3 up = new Vector3(0, 1, 0);//Vector3.Up;


            Matrix.CreateLookAt(ref pos, ref target, ref up, out view);
            a += .005f;
            //view *= Matrix.CreateRotationZ(a);
            Matrix.CreateOrthographicOffCenter(x - w / 2, x + w / 2, y + h / 2, y - h / 2, -500, 500, out projection);
            Matrix mat = view * projection;
            foreach (var p in effectList)
            {
                p.Value.effect.Parameters["WorldViewProjection"].SetValue(mat);
                //p.Value.effect.Parameters["size"].SetValue(new Vector2(w, h));
                //p.Value.effect.Parameters["size"].SetValue(new Vector2(Dimension.W, Dimension.H));
                //p.Value.effect.Parameters["mapSize"].SetValue(new Vector2(LevelData.mapX, LevelData.mapY));
                //p.Value.effect.Parameters["timer"].SetValue((float)gt.TotalGameTime.TotalMilliseconds);// (float)fakeTimer);//

                //p.Value.effect.Parameters["wpos"].SetValue(
                //    new Vector2((x + Dimension.W / 2) / Dimension.W, (y + Dimension.H / 2) / Dimension.W));
            }
            if (oneTime == false)
            {
                oneTime = true;
                setParameters();
            }
            effectList[EffectType.Retangulo].effect.Parameters["screensize"].SetValue(new Vector2(Dimension.screenW, Dimension.screenH));
            oneTime = false;
        }
        public void Begin(string e, float x, float y, int w, int h)
        {
            Matrix view;
            Matrix projection;

            Vector3 pos = new Vector3(0, 0, 3);
            Vector3 target = Vector3.Zero;
            Vector3 up = Vector3.Up;

            Matrix.CreateLookAt(ref pos, ref target, ref up, out view);
            //Matrix.CreateOrthographicOffCenter(x - w / 2, x + w / 2, y + h / 2, y - h / 2, -500, 500, out projection);
            Matrix.CreateOrthographicOffCenter(x, x + w, y + h, y, -500, 500, out projection);

            effectList[e].effect.Parameters["WorldViewProjection"].SetValue(view * projection);
            //effectList[EffectType.Retangulo].effect.Parameters["screensize"].SetValue(new Vector2(Dimension.screenW, Dimension.screenH));
            //effectList[EffectType.Retangulo].effect.Parameters["screensize"].SetValue();

            //effectList[e].effect.Parameters["size"].SetValue(new Vector2(w, h));
            //effectList[e].effect.Parameters["mapSize"].SetValue(new Vector2(LevelData.mapX, LevelData.mapY));
            //effectList[e].effect.Parameters["timer"].SetValue((float)gt.TotalGameTime.TotalMilliseconds);// (float)fakeTimer);//
        }
        public void setParameters()
        {
            effectList[EffectType.Text].effect.Parameters["tex"].SetValue(FontManager.tex);
        }      
        public enum Pivo
        {
            TopLeft,
            TopCenter,
            TopRight,

            BottomLeft,
            BottomCenter,
            BottomRight,

            MiddleLeft,
            MiddleCenter,
            MiddleRight,
        }
        public class stringAnimation
        {
            public bool enabled; //[Test]
            public float[] waveArray;
            public float[] speedArray;
        }
        public static List<stringAnimation> stringanimation = new List<stringAnimation>();       
        public void DrawTexture(string text, Vector2 pos, float animation, float _size = 1, bool rtl = false, int lineLen = 1000)
        {
            Vector2 offset = Vector2.Zero;
            bool firstGlyph = true;
            float size = FontManager.font.atlas.size;
            int counter = 0;
            for (int i = 0; i < text.Length; i++)
            {

                char c = text[i];
                if (counter >= lineLen)
                {
                    counter -= lineLen;
                    c = '\n';
                    --i;
                }
                ++counter;
                if (c == '\r') continue;
                if (c == '\n')
                {
                    offset.X = 0;
                    //sprfont.LineSpacing
                    //sprfont.LineSpacing * _size;
                    offset.Y += (size + 1) * _size; // hardcoded offset de + 1                    
                    firstGlyph = true;
                    continue;
                }
                if (!FontManager.glyphs.ContainsKey(c)) c = '.';
                var glyph = FontManager.glyphs[c];//sprfont.GetGlyphs()[c];
                if (c == ' ')
                {
                    offset.X += glyph.advance * _size * size;
                    continue;
                }
                if (firstGlyph)
                {
                    //offset.X = Math.Max(glyph.LeftSideBearing, 0) * _size;
                    firstGlyph = false;
                }
                else
                {
                    //offset.X += (sprfont.Spacing /*+ glyph.LeftSideBearing*/) * _size;
                }
                Vector2 currentOff = offset;
                //currentOff.X += glyph.planeBounds.left;//(0/*glyph.Cropping.X*/) * _size;
                //currentOff.Y += glyph.planeBounds.top;//(0/*glyph.Cropping.Y*/) * _size;

                float Size = size * _size;

                Matrix transform = Matrix.Identity;

                transform.M11 = Size;
                transform.M22 = Size;
                transform.M41 = pos.X * 0;
                transform.M42 = pos.Y * 0;

                //offset.X += //(glyph.Width + glyph.RightSideBearing) * _size;

                var o = effectList["text"];
                EnsureSpace(6, 4, o);
                int z = 0;
                //int colorID = 0; float packedData = 0; int z = 0; byte flipX = 0; byte flipY = 0;

                float fw = Size, fh = Size;

                //float _x = glyph.BoundsInTexture.X, _y = glyph.BoundsInTexture.Y,
                //    _w = glyph.BoundsInTexture.Width, _h = glyph.BoundsInTexture.Height;//sprfont.Texture.Width, _h = sprfont.Texture.Height;
                float _x = glyph.atlasBounds.left,
                    _h = FontManager.font.atlas.height - glyph.atlasBounds.bottom,
                    _w = glyph.atlasBounds.right,
                    _y = FontManager.font.atlas.height - glyph.atlasBounds.top;


                //_x /= sprfont.Texture.Width;
                //_y /= sprfont.Texture.Height;
                //_w /= sprfont.Texture.Width;
                //_h /= sprfont.Texture.Height;
                _x /= FontManager.font.atlas.width;
                _y /= FontManager.font.atlas.height;
                _w /= FontManager.font.atlas.width;
                _h /= FontManager.font.atlas.height;
                //////DEBUG
                //float off = .02f;
                _x = 0;
                _y = 0;
                _w = 1;
                _h = 1;
                float f = 1111f;

                ///int indx = colorIndex % colorsManager.backG.Length;
                Vector3 data = new Vector3();//colorsManager.playr.ToVector3();// new Vector3(colorID, size.Y, packedData);

                data.X = 0;
                data.Y = 216216;
                data.Z = animation;

                Vector4 cor = Color.White.ToVector4();

                o.index[o.indexCount++] = (short)(o.vertexCount + 0);
                o.index[o.indexCount++] = (short)(o.vertexCount + 1);
                o.index[o.indexCount++] = (short)(o.vertexCount + 2);
                o.index[o.indexCount++] = (short)(o.vertexCount + 1);
                o.index[o.indexCount++] = (short)(o.vertexCount + 3);
                o.index[o.indexCount++] = (short)(o.vertexCount + 2);

                Vector2 msize = new Vector2();

                o.vertex[o.vertexCount++] = new MyVertexType(new Vector3(0, 0, z), new Vector2(_x, _y), cor, cor, msize);
                o.vertex[o.vertexCount++] = new MyVertexType(new Vector3(1, 0, z), new Vector2(_w, _y), cor, cor, msize);
                o.vertex[o.vertexCount++] = new MyVertexType(new Vector3(0, 1, z), new Vector2(_x, _h), cor, cor, msize);
                o.vertex[o.vertexCount++] = new MyVertexType(new Vector3(1, 1, z), new Vector2(_w, _h), cor, cor, msize);
                float w = glyph.planeBounds.right - glyph.planeBounds.left, h = glyph.planeBounds.top - glyph.planeBounds.bottom;
                currentOff.X += glyph.planeBounds.left * Size;
                currentOff.Y -= glyph.planeBounds.bottom * Size;
                offset.X += glyph.advance * Size;//w *Size;

                Matrix world = Matrix.CreateTranslation(new Vector3(-.0f, -1f, 0))
                     //* Matrix.CreateScale(new Vector3(w * Size, h * Size, 1))
                     * Matrix.CreateScale(new Vector3(FontManager.font.atlas.width * _size, FontManager.font.atlas.height * _size, 1))
                    * Matrix.CreateTranslation(new Vector3(pos + currentOff, 0));

                for (int j = o.vertexCount - 4; j < o.vertexCount; j++)
                    Vector3.Transform(ref o.vertex[j].Position, ref world, out o.vertex[j].Position);
            }
        }
        public static void DrawRect(EffectStruct o, Vector2 pos, Vector2 size, Color col, float rotate = 0, float depth = 0, byte flipX = 0, byte flipY = 0, float _w = 1, float _h = 1)
        {

            //ref var o = ref sdfObjects[id];
            EnsureSpace(6, 4, o);

            float fw = size.X, fh = size.Y;

            //int _w = 1, _h = 1;


            //Vector3 data = new Vector3(colorID, size.Y, packedData);

            Vector4 cor = col.ToVector4();// (col ?? Color.White).ToVector4();

            o.index[o.indexCount++] = (short)(o.vertexCount + 0);
            o.index[o.indexCount++] = (short)(o.vertexCount + 1);

            o.index[o.indexCount++] = (short)(o.vertexCount + 2);
            o.index[o.indexCount++] = (short)(o.vertexCount + 1);
            o.index[o.indexCount++] = (short)(o.vertexCount + 3);
            o.index[o.indexCount++] = (short)(o.vertexCount + 2);
            //o.vertexCount++;
            //return;
            o.vertex[o.vertexCount++] = new MyVertexType(new Vector3(0, 0 + flipY, 0), new Vector2(0, 0), cor, cor, size);

            o.vertex[o.vertexCount++] = new MyVertexType(new Vector3(1, 0 + flipY, 0), new Vector2(_w, 0), cor, cor, size);
            o.vertex[o.vertexCount++] = new MyVertexType(new Vector3(0, 1 - flipY, 0), new Vector2(0, _h), cor, cor, size);
            o.vertex[o.vertexCount++] = new MyVertexType(new Vector3(1, 1 - flipY, 0), new Vector2(_w, _h), cor, cor, size);
            //return;
            Matrix world = Matrix.CreateTranslation(new Vector3(-.5f, -.5f, 0))
                * Matrix.CreateRotationZ(MathHelper.PiOver2 * flipX)
                * Matrix.CreateRotationZ(rotate)
                * Matrix.CreateScale(new Vector3(fw, fh, 1))
                * Matrix.CreateTranslation(new Vector3(pos, depth));

            for (int i = o.vertexCount - 4; i < o.vertexCount; i++)
                Vector3.Transform(ref o.vertex[i].Position, ref world, out o.vertex[i].Position);
        }

        public static void Retangulo(Vector2 pos, Vector2 size, float rotate, Color cor, float depth = 0)
        {
            var o = effectList["retângulo"];
            EnsureSpace(6, 4, o);
            float fw = size.X, fh = size.Y;

            int _w = 1, _h = 1;

            o.index[o.indexCount++] = (short)(o.vertexCount + 0);
            o.index[o.indexCount++] = (short)(o.vertexCount + 1);
            o.index[o.indexCount++] = (short)(o.vertexCount + 2);
            o.index[o.indexCount++] = (short)(o.vertexCount + 1);
            o.index[o.indexCount++] = (short)(o.vertexCount + 3);
            o.index[o.indexCount++] = (short)(o.vertexCount + 2);

            Vector4 col = cor.ToVector4();

            o.vertex[o.vertexCount++] = new MyVertexType(new Vector3(0, 0, 0), new Vector2(0, 0), col, col, size);
            o.vertex[o.vertexCount++] = new MyVertexType(new Vector3(1, 0, 0), new Vector2(_w, 0), col, col, size);
            o.vertex[o.vertexCount++] = new MyVertexType(new Vector3(0, 1, 0), new Vector2(0, _h), col, col, size);
            o.vertex[o.vertexCount++] = new MyVertexType(new Vector3(1, 1, 0), new Vector2(_w, _h), col, col, size);

            Matrix world = Matrix.CreateTranslation(new Vector3(-.5f, -.5f, 0))
                * Matrix.CreateRotationZ(MathHelper.PiOver2)
                * Matrix.CreateRotationZ(rotate)
                //* Matrix.CreateTranslation(new Vector3(-.5f, -.5f, 0))
                * Matrix.CreateScale(new Vector3(fw, fh, 1))
                //* Matrix.CreateTranslation(new Vector3(-.5f, -.5f, 0))
                * Matrix.CreateTranslation(new Vector3(pos, depth));

            for (int i = o.vertexCount - 4; i < o.vertexCount; i++)
                Vector3.Transform(ref o.vertex[i].Position, ref world, out o.vertex[i].Position);
        }
        bool isSaved = false;
        bool printAll = false;

        public float pauseAmount = 0f;
        public void Flush(RenderTarget2D target = null)
        {
            RasterizerState rast = new RasterizerState { CullMode = CullMode.None, FillMode = FillMode.Solid, ScissorTestEnable = true };
            gd.RasterizerState = rast;
            gd.SamplerStates[0] = SamplerState.PointClamp;
            //gd.RasterizerState.ScissorTestEnable= true;

            gd.SetRenderTarget(target);
            ///int indx = colorIndex % colorsManager.backG.Length;// = 3;
            //gd.ScissorRectangle = new Rectangle(100, 100, 100, 100);

            Color bg = Color.Black;//colorsManager.backG;
            bg.A = 255;
            gd.Clear(bg); // LevelData.Black);            

            for (int i = 0; i < sdfObjects.Length; i++)
            {
                string seffect = EffectType.sEffectType[i];

                ref var o = ref sdfObjects[i];
                if (o.vertexCount == 0) continue;
                gd.BlendState = BlendState.NonPremultiplied;
                switch (seffect)
                {
                    case EffectType.Text:
                        gd.SamplerStates[0] = SamplerState.LinearClamp;
                        break;
                }

                try
                {
                    effectList[seffect].effect.CurrentTechnique.Passes[0].Apply();
                }
                catch (Exception e)
                {
                    // LogManager.WriteError($"\nErro: {e.Source}\n{e.StackTrace}\n{e.Message}\n\n");
                    throw;
                }

                gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, o.vertex, 0, o.vertexCount, o.index, 0, o.indexCount / 3);
                sdfObjects[i].vertexCount = sdfObjects[i].indexCount = 0;
            }
            // if (target != null)
            gd.SetRenderTarget(null);
        }     
        private static void EnsureSpace(int indexSpace, int vertexSpace, EffectStruct o)
        {
            //ref var o = ref sdfObjects[id];
            if (o.indexCount + indexSpace >= o.index.Length)
                Array.Resize(ref o.index, Math.Max(o.indexCount + indexSpace, o.index.Length * 2));
            if (o.vertexCount + vertexSpace >= o.vertex.Length)
                Array.Resize(ref o.vertex, Math.Max(o.vertexCount + vertexSpace, o.vertex.Length * 2));
        }
    }
}