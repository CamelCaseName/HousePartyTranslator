using NetDiscordRpc;
using NetDiscordRpc.RPC;
using System.Runtime.Versioning;
using Translator.Core;
using Translator.Core.Helpers;
using Translator.Helpers;
using Settings = Translator.Desktop.InterfaceImpls.WinSettings;

namespace Translator.Desktop.Managers
{
    [SupportedOSPlatform("Windows")]
    public static class DiscordPresenceManager
    {
        public static DiscordRPC? DiscordPresenceClient;
        private static string Character = string.Empty;
        private static string Story = string.Empty;
        private static string ImageKey = Characters.CharacterEnum.rule34.ToString();

        private static void UpdatePresence()
        {
            if (DataBase.IsOnline)
            {
                try
                {
                    DiscordPresenceClient?.SetPresence(new RichPresence()
                    {
                        Details = $"Working on {(Story.Length > 0 ? Story : "nothing")},",
                        State = $"translating {(Character.Length > 0 ? Character : "no one :)")}",
                        Assets = new Assets()
                        {
                            LargeImageKey = ImageKey,
                            LargeImageText = Character
                        }
                    });
                    _ = DiscordPresenceClient?.UpdateStartTime();
                }
                catch
                {
                    LogManager.Log("Couldn't update presence.");
                }
            }
        }

        private static void SetImageKey()
        {
            //try and get the corresponding image key
            if (Characters.CharacterDict.TryGetValue(Character, out Characters.CharacterEnum value))
            {
                ImageKey = value.ToString();
            }
            else
            {
                //else fall back to nippy dick
                ImageKey = Characters.CharacterEnum.rule34.ToString();
            }
        }

        public static void Update()
        {
            if (DataBase.IsOnline)
            {
                if (Settings.EnableDiscordRP)
                {
                    if (!(DiscordPresenceClient?.IsInitialized ?? false) || (DiscordPresenceClient?.IsDisposed ?? false))
                    {
                        Initialize();
                    }
                    if (Story != TabManager.ActiveTranslationManager.StoryName || Character != TabManager.ActiveTranslationManager.FileName)
                    {
                        Story = TabManager.ActiveTranslationManager.StoryName;
                        Character = TabManager.ActiveTranslationManager.FileName;

                        SetImageKey();

                        UpdatePresence();
                    }
                    _ = DiscordPresenceClient?.Invoke();
                }
                else
                {
                    DiscordPresenceClient?.ClearPresence();
                }
            }
        }

        /// <summary>
        /// Call before everything else
        /// </summary>
        private static void Initialize()
        {
            DiscordPresenceClient = new DiscordRPC("940326430703771688")
            {
                SkipIdenticalPresence = true
            };

            //Connect to the RPC
            _ = DiscordPresenceClient?.Initialize();

            //Set the rich presence
            //Call this as many times as you want and anywhere in your code.
            DiscordPresenceClient?.SetPresence(new RichPresence()
            {
                Details = $"Working on {Story},",
                State = $"translating {Character}",
                Assets = new Assets()
                {
                    LargeImageKey = ImageKey,
                    LargeImageText = Character
                }
            });

            _ = DiscordPresenceClient?.UpdateStartTime();
        }

        public static void DeInitialize()
        {
            //Deinit to prevent a c++ side memory leak
            DiscordPresenceClient?.Dispose();
        }
    }
}
