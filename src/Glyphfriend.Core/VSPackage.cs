using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ProtoBuf;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Glyphfriend
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideAutoLoad(HtmlFileLoadedContext)]
    [ProvideUIContextRule(HtmlFileLoadedContext,
        name: "HTML File Loaded",
        expression: "HtmlConfig",
        termNames: new[] { "HtmlConfig" },
        termValues: new[] { "ActiveEditorContentType:htmlx" })]
    public sealed class VSPackage : Package
    {
        public const string HtmlFileLoadedContext = "21F5568E-A5DE-4821-AF39-F4F1049BB9CF";

        internal static Dictionary<string,bool> EnabledLibraries { get; private set; }
        internal static Dictionary<string, Dictionary<string, ImageSource>> Glyphs { get; private set; }
        internal static string AssemblyLocation => Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        protected override void Initialize()
        {
            DeserializeGlyphsFromBinary();
            SetEnabledLibraries();
        }

        private void DeserializeGlyphsFromBinary()
        {
            var binaryGlyphDictionary = DeserializeBinaryGlyphs();
            Glyphs = ConvertBinaryGlyphDictionaryToGlyphDictionary(binaryGlyphDictionary);
        }

        private Dictionary<string, Dictionary<string, byte[]>> DeserializeBinaryGlyphs()
        {
            var binaryPath = Path.Combine(AssemblyLocation, "glyphs.bin");
            using (var fs = File.Open(binaryPath, FileMode.Open))
            {
                return Serializer.Deserialize< Dictionary<string, Dictionary<string, byte[]>>>(fs);
            }
        }

        private Dictionary<string, Dictionary<string,ImageSource>> ConvertBinaryGlyphDictionaryToGlyphDictionary(Dictionary<string, Dictionary<string, byte[]>> dictionary)
        {
            // Convert the Dictionary from a mapping of byte[] values to one that uses ImageSources
            return dictionary.ToDictionary(k => k.Key, v => v.Value.ToDictionary(x => x.Key, y => BytesToImage(y.Value)));
        }

        private ImageSource BytesToImage(byte[] imageData)
        {
            var image = new BitmapImage();
            using (var ms = new MemoryStream(imageData))
            {
                image.BeginInit();
                image.StreamSource = ms;
                // This is required for images to be loaded by Visual Studio on-demand
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
            }
            return image as ImageSource;
        }

        private void SetEnabledLibraries()
        {
            // Default each library to enabled
            EnabledLibraries = Glyphs.ToDictionary(k => k.Key, v => false);
            EnabledLibraries["FontAwesome"] = true;
            // Consider reading settings here
        }
    }
}
