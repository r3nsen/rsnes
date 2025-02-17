using Microsoft.Xna.Framework.Graphics;

//using Newtonsoft.Json;


//using Newtonsoft.Json;

//using SharpDX.Direct2D1.Effects;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace r3nGraphics
{

    public class Atlas
    {
        public string type { get; set; }
        public int distanceRange { get; set; }
        public float size { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string yOrigin { get; set; }
    }
    public class Metrics
    {
        public float emSize { get; set; }
        public float lineHeight { get; set; }
        public float ascender { get; set; }
        public float descender { get; set; }
        public float underlineY { get; set; }
        public float underlineThickness { get; set; }
    }
    public class Bounds
    {
        public float left { get; set; }
        public float bottom { get; set; }
        public float right { get; set; }
        public float top { get; set; }
    }
    public class Glyphs
    {
        public int unicode { get; set; }
        public float advance { get; set; }
        public Bounds planeBounds { get; set; }
        public Bounds atlasBounds { get; set; }
    }

    public class Font
    {
        public Atlas atlas { get; set; }
        public Metrics metrics { get; set; }
        public List<Glyphs> glyphs { get; set; }
        public List<string> kerning { get; set; }
    }

    class FontManager
    {


        public static Font font;
        public static Dictionary<int, Glyphs> glyphs = new Dictionary<int, Glyphs>();
        public static Texture2D tex;
        public static void LoadFont(Texture2D t, string d)
        {
            char c = (char)(int)'y'; // ! = 33, y = 121
            tex = t;

            string text = File.ReadAllText(d);


            //ReadOnlySpan<byte> text = File.ReadAllBytes(d);
            //var reader = new Utf8JsonReader(text, new JsonReaderOptions
            //{
            //    AllowTrailingCommas = true,
            //    CommentHandling = JsonCommentHandling.Skip
            //});

            //while (reader.Read())
            //{
            //    switch (reader.TokenType)
            //    {
            //        case JsonTokenType.String: break;
            //        case JsonTokenType.PropertyName: break;                        

            //    }
            //}

            //font = JsonConvert.DeserializeObject<Font>(text); // Newtonsoft.Json;
            font = JsonSerializer.Deserialize<Font>(text, FontJsonContext.Default.Font); // System.Text.Json;
            //font = _font;
            foreach (var p in font.glyphs) glyphs.Add(p.unicode, p);
        }
    }
}
