using LibreTranslate.Net;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Translator.Core.Data;
using Translator.Core.UICompatibilityLayer;

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
        private static readonly LibreTranslate.Net.LibreTranslate? Translator;
        private static readonly DeepL.Translator? DeepLTranslator;
        private static readonly CancellationTokenSource tokenSource = new();

        static AutoTranslation()
        {
            if (Settings.Default.Translator == 0)
            {
                Translator = new("https://translate.rinderha.cc");
            }
            else if (Settings.Default.Translator == 1)
            {
                DeepLTranslator = new(Settings.Default.DeeplApiKey, new() { sendPlatformInfo = false });
            }
        }

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
                return;
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

        /// <summary>
        /// Aborts all running translation tasks
        /// </summary>
        public static void AbortAllrunningTranslations()
        {
            tokenSource.Cancel();
        }

        private static async void AutoTranslationImpl(LineData data, LanguageCode langCodeTemplate, LanguageCode langCodeTranslation, TranslationCompletedCallback OnCompletion)
        {
            if (Translator is null)
            {
                if (DeepLTranslator is null)
                {
                    return;
                }
                else
                {
                    await DeepLImpl(data, langCodeTemplate, langCodeTranslation, OnCompletion);
                }
            }
            else
            {
                await LibreTranslateImpl(data, langCodeTemplate, langCodeTranslation, OnCompletion);
            }
        }

        private static async Task DeepLImpl(LineData data, LanguageCode langCodeTemplate, LanguageCode langCodeTranslation, TranslationCompletedCallback OnCompletion)
        {
            try
            {
                string result = string.Empty;
                Task<DeepL.Model.TextResult> task = DeepLTranslator!.TranslateTextAsync(
                    data.TemplateString.RemoveVAHints(),
                    langCodeTemplate == LanguageCode.AutoDetect ? string.Empty : langCodeTemplate.ToString(),
                    langCodeTranslation.ToString(),
                    cancellationToken: tokenSource.Token
                    );
                result = (await task).Text;
                if (result == null)
                {
                    OnCompletion(false, data);
                }
                else if (result.Length > 0)
                {
                    data.TranslationString = result;
                    data.WasChanged = true;
                    data.IsTranslated = true;
                    OnCompletion(true, data);
                }
                else
                {
                    OnCompletion(false, data);
                }
            }
            catch
            {
                OnCompletion(false, data);
            }
        }

        private static async Task LibreTranslateImpl(LineData data, LanguageCode langCodeTemplate, LanguageCode langCodeTranslation, TranslationCompletedCallback OnCompletion)
        {
            try
            {
                string result = string.Empty;
                Task<string> task = Translator!.TranslateAsync(new Translate()
                {
                    ApiKey = string.Empty,
                    Source = langCodeTemplate,
                    Target = langCodeTranslation,
                    Text = data.TemplateString.RemoveVAHints()
                });
                result = await task;
                if (result == null)
                {
                    OnCompletion(false, data);
                }
                else if (result.Length > 0)
                {
                    data.TranslationString = result;
                    data.WasChanged = true;
                    data.IsTranslated = true;
                    OnCompletion(true, data);
                }
                else
                {
                    OnCompletion(false, data);
                }
            }
            catch
            {
                OnCompletion(false, data);
            }
        }
    }
}
