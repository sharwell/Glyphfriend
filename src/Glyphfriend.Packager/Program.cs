using ProtoBuf;
using System.Collections.Generic;
using System.IO;

namespace Glyphfriend.Packager
{
    class Program
    {
        static void Main(string[] args)
        {
            GenerateBinaryGlyphsFile();
        }

        private static void GenerateBinaryGlyphsFile()
        {
            var glyphDictionary = ConvertGlyphsToDictionary();
            BinarySerializeGlyphsToFile(glyphDictionary);
        }

        private static Dictionary<string, Dictionary<string, byte[]>> ConvertGlyphsToDictionary()
        {
            var glyphs = new Dictionary<string, Dictionary<string,byte[]>>();
            var glyphDirectory = new DirectoryInfo("../../Glyphs");
            foreach (var glyph in glyphDirectory.EnumerateFiles("*.png", SearchOption.AllDirectories))
            {
                var directory = glyph.Directory.Name;
                if (!glyphs.ContainsKey(directory))
                {
                    glyphs.Add(directory, new Dictionary<string, byte[]>());
                }
                var glyphContent = StreamHelpers.ReadFully(new FileStream(glyph.FullName, FileMode.Open));
                glyphs[directory].Add(Path.GetFileNameWithoutExtension(glyph.Name), glyphContent);
            }
            return glyphs;
        }

        private static void BinarySerializeGlyphsToFile(Dictionary<string, Dictionary<string, byte[]>> glyphs)
        {
            using (var ms = new FileStream("../../Binary/glyphs.bin", FileMode.Create))
            {
                Serializer.Serialize(ms, glyphs);
                ms.Flush();
            }
        }
    }
}