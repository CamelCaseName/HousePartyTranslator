using DiscordRPC;
using HousePartyTranslator.Helpers;
using System;


namespace HousePartyTranslator.Managers
{
    public class DiscordPresenceManager
    {
        public DiscordRpcClient DiscordPresenceClient;
        private string Character;
        private string Story;
        private string ImageKey = Characters.CharacterEnum.rule34.ToString();

        public DiscordPresenceManager()
        {
            //init first
            Initialize();

        }

        private void UpdatePresence()
        {
            DiscordPresenceClient.SetPresence(new RichPresence()
            {
                Details = $"Working on {Story},",
                State = $"translating {Character}",
                Assets = new Assets()
                {
                    LargeImageKey = ImageKey,
                    LargeImageText = Character
                }
            });
            DiscordPresenceClient.UpdateStartTime();
            Update();
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
            if (Properties.Settings.Default.enableDiscordRP)
            {
                if (!DiscordPresenceClient.IsInitialized || DiscordPresenceClient.IsDisposed)
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
                DiscordPresenceClient.ClearPresence();
            }
        }

        public void Update()
        {
            if (Properties.Settings.Default.enableDiscordRP)
            {
                DiscordPresenceClient.Invoke();
            }
        }

        /// <summary>
        /// Call before everything else
        /// </summary>
        private void Initialize()
        {
            DiscordPresenceClient = new DiscordRpcClient("940326430703771688")
            {
                SkipIdenticalPresence = true
            };

            //Connect to the RPC
            DiscordPresenceClient.Initialize();

            //Set the rich presence
            //Call this as many times as you want and anywhere in your code.
            DiscordPresenceClient.SetPresence(new RichPresence()
            {
                Details = $"Working on {Story},",
                State = $"translating {Character}",
                Assets = new Assets()
                {
                    LargeImageKey = ImageKey,
                    LargeImageText = Character
                }
            });

            DiscordPresenceClient.UpdateStartTime();
        }

        public void DeInitialize()
        {
            //Deinit to prevent a c++ side memory leak
            DiscordPresenceClient.Dispose();
        }
    }
}
