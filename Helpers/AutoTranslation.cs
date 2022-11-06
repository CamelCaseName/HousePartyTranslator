using LibreTranslate.Net;

namespace HousePartyTranslator.Helpers
{
    /// <summary>
    /// Callback to extract the completed translation, or a bool indicating an error
    /// </summary>
    /// <param name="successfull">true if the translation succeeded</param>
    /// <param name="data">the updated LineData</param>
    internal delegate void TranslationCopmpletedCallback(bool successfull, LineData data);

    /// <summary>
    /// Provides a simple automatic translation interface
    /// </summary>
    internal static class AutoTranslation
    {
        private static readonly LibreTranslate.Net.LibreTranslate Translator = new LibreTranslate.Net.LibreTranslate("https://translate.rinderha.cc");

        /// <summary>
        /// Starts a webrequest to https://translate.rinderha.cc" to translate the template for the current line into the given langauge if possible
        /// </summary>
        /// <param name="data">the LineData to work on, provides template and translation in return</param>
        /// <param name="language">the language to translate to, in 2 letter code</param>
        /// <param name="OnCompletion">callback to return the completed translation back to the program</param>
        internal static void AutoTranslationAsync(LineData data, string language, TranslationCopmpletedCallback OnCompletion)
        {
            AutoTranslationImpl(data, LanguageCode.English, LanguageCode.FromString(language), OnCompletion);
        }

        /// <summary>
        /// Starts a webrequest to https://translate.rinderha.cc" to translate the template for the current line into the given langauge if possible
        /// </summary>
        /// <param name="data">the LineData to work on, provides template and translation in return</param>
        /// <param name="targetLanguage">the language to translate to, in 2 letter code</param>
        /// <param name="templateLanguage">the language to translate from, in 2 letter code</param>
        /// <param name="OnCompletion">callback to return the completed translation back to the program</param>
        internal static void AutoTranslationAsync(LineData data, string targetLanguage, string templateLanguage, TranslationCopmpletedCallback OnCompletion)
        {
            AutoTranslationImpl(data, LanguageCode.FromString(templateLanguage), LanguageCode.FromString(targetLanguage), OnCompletion);
        }

        private static async void AutoTranslationImpl(LineData data, LanguageCode langCodeTemplate, LanguageCode langCodeTranslation, TranslationCopmpletedCallback OnCompletion)
        {
            try
            {
                string result = "";
                System.Threading.Tasks.Task<string> task = Translator.TranslateAsync(new Translate()
                {
                    ApiKey = "",
                    Source = langCodeTemplate,
                    Target = langCodeTranslation,
                    Text = data.TemplateString
                });
                result = await task;
                if (result.Length > 0)
                {
                    data.TranslationString = result;
                    OnCompletion(true, data);
                }
            }
            catch
            {
                OnCompletion(false, data);
            }
        }
    }
}
