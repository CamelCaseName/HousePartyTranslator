using LibreTranslate.Net;
using Translator.Core.Data;

namespace Translator.Core.Helpers
{
    /// <summary>
    /// Callback to extract the completed translation, or a bool indicating an error
    /// </summary>
    /// <param name="successfull">true if the translation succeeded</param>
    /// <param name="data">the updated LineData</param>
    public delegate void TranslationCompletedCallback(bool successfull, LineData data);

    /// <summary>
    /// Provides a simple automatic translation interface
    /// </summary>
    public static class AutoTranslation
    {
        private static readonly LibreTranslate.Net.LibreTranslate Translator = new("https://translate.rinderha.cc");

        /// <summary>
        /// Starts a webrequest to https://translate.rinderha.cc" to translate the template for the current line into the given langauge if possible
        /// </summary>
        /// <param name="data">the LineData to work on, provides template and translation in return</param>
        /// <param name="language">the language to translate to, in 2 letter code</param>
        /// <param name="OnCompletion">callback to return the completed translation back to the program</param>
        public static void AutoTranslationAsync(LineData data, string language, TranslationCompletedCallback OnCompletion)
        {
            LanguageCode code = LanguageCode.AutoDetect;
            try
            {
                code = LanguageCode.FromString(language);
            }
            catch
            {
                code = LanguageCode.AutoDetect;
            }
            finally
            {
                AutoTranslationImpl(data, LanguageCode.English, code, OnCompletion);
            }
        }

        /// <summary>
        /// Starts a webrequest to https://translate.rinderha.cc" to translate the template for the current line into the given langauge if possible
        /// </summary>
        /// <param name="data">the LineData to work on, provides template and translation in return</param>
        /// <param name="targetLanguage">the language to translate to, in 2 letter code</param>
        /// <param name="templateLanguage">the language to translate from, in 2 letter code</param>
        /// <param name="OnCompletion">callback to return the completed translation back to the program</param>
        public static void AutoTranslationAsync(LineData data, string targetLanguage, string templateLanguage, TranslationCompletedCallback OnCompletion)
        {
            LanguageCode codeTo;
            LanguageCode codeFrom = LanguageCode.AutoDetect;
            try
            {
                codeTo = LanguageCode.FromString(targetLanguage);
            }
            catch
            {
                codeTo = LanguageCode.AutoDetect;
            }
            try
            {
                codeFrom = LanguageCode.FromString(templateLanguage);
            }
            catch
            {
                codeFrom = LanguageCode.AutoDetect;
            }
            finally
            {
                AutoTranslationImpl(data, codeFrom, codeTo, OnCompletion);
            }
        }

        private static async void AutoTranslationImpl(LineData data, LanguageCode langCodeTemplate, LanguageCode langCodeTranslation, TranslationCompletedCallback OnCompletion)
        {
            try
            {
                string result = string.Empty;
                System.Threading.Tasks.Task<string> task = Translator.TranslateAsync(new Translate()
                {
                    ApiKey = string.Empty,
                    Source = langCodeTemplate,
                    Target = langCodeTranslation,
                    Text = data.TemplateString
                });
                result = await task;
                if(result == null)
                    OnCompletion(false, data);
                else if (result.Length > 0)
                {
                    data.TranslationString = result;
                    OnCompletion(true, data);
                }
                else
                    OnCompletion(false, data);
            }
            catch
            {
                OnCompletion(false, data);
            }
        }
    }
}
