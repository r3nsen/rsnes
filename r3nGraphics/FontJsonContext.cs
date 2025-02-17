
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace r3nGraphics
{
    [JsonSerializable(typeof(Font))]
    [JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default)]
    public partial class FontJsonContext : JsonSerializerContext
    {

    }
}
