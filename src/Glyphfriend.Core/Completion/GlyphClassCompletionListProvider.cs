using Microsoft.Html.Editor.Completion;
using Microsoft.Html.Editor.Completion.Def;
using Microsoft.VisualStudio.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Glyphfriend
{
    [HtmlCompletionProvider(CompletionTypes.Values, "*", "class")]
    [ContentType("htmlx")]
    class GlyphClassCompletionListProvider : BaseClassCompletionProvider
    {
        public override string CompletionType
        {
            get { return CompletionTypes.Values; }
        }

        public override IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
        {
            var completionItems = new List<HtmlCompletion>();
            var enabledGlyphs = GetEnabledGlyphs();
            foreach (var glyph in enabledGlyphs)
            {
                completionItems.Add(CreateItem(glyph.Key, glyph.Value, context.Session));
            }
            return completionItems;
        }

        private IEnumerable<KeyValuePair<string, System.Windows.Media.ImageSource>> GetEnabledGlyphs()
        {
            return VSPackage.Glyphs.Where(g => VSPackage.EnabledLibraries[g.Key])
                                   .SelectMany(g => g.Value);
        }
    }
}