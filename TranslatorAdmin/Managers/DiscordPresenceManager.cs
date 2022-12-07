using NetDiscordRpc;
using NetDiscordRpc.RPC;
using Translator.Core;
using Translator.Helpers;
using Settings = TranslatorAdmin.Properties.Settings;
using DataBase = Translator.Core.DataBase<TranslatorAdmin.InterfaceImpls.WinLineItem, TranslatorAdmin.InterfaceImpls.WinUIHandler>;

namespace Translator.Managers
{
    internal sealed class DiscordPresenceManager
    {
        public DiscordRPC? DiscordPresenceClient;
        private string Character = "";
        private string Story = "";
        private string ImageKey = Characters.CharacterEnum.rule34.ToString();

        public DiscordPresenceManager()
        {
            //init first
            if (DataBase.IsOnline) Initialize();

        }

        private void UpdatePresence()
        {
            if (DataBase.IsOnline)
            {
                try
                {
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
                    Update();
                }
                catch {
                    LogManager.Log("Couldn't update presence.");
                }
            }
        }

        private void SetImageKey()
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

        public void Update(string story, string character)
        {
            if (DataBase.IsOnline)
            {
                if (Settings.Default.enableDiscordRP)
                {
                    if (!(DiscordPresenceClient?.IsInitialized ?? false) || (DiscordPresenceClient?.IsDisposed ?? false))
                    {
                        Initialize();
                    }
                    Story = story;
                    Character = character;

                    SetImageKey();

                    UpdatePresence();
                }
                else
                {
                    DiscordPresenceClient?.ClearPresence();
                }
            }
        }

        public void Update()
        {
            if (Settings.Default.enableDiscordRP && DataBase.IsOnline)
            {
                _ = DiscordPresenceClient?.Invoke();
            }
        }

        /// <summary>
        /// Call before everything else
        /// </summary>
        private void Initialize()
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

        public void DeInitialize()
        {
            //Deinit to prevent a c++ side memory leak
            DiscordPresenceClient?.Dispose();
        }
    }
}
