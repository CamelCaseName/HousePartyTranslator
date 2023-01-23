﻿using NetDiscordRpc;
using NetDiscordRpc.RPC;
using System.Runtime.Versioning;
using Translator.Core;
using Translator.Helpers;
using TranslatorApp.InterfaceImpls;
using DataBase = Translator.Core.DataBase<TranslatorApp.InterfaceImpls.WinLineItem, TranslatorApp.InterfaceImpls.WinUIHandler, TranslatorApp.InterfaceImpls.WinTabController, TranslatorApp.InterfaceImpls.WinTab>;
using Settings = TranslatorApp.InterfaceImpls.WinSettings;

namespace Translator.Managers
{
	[SupportedOSPlatform("Windows")]
	public sealed class DiscordPresenceManager
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
						Details = $"Working on {(Story.Length > 0 ? Story : "nothing")},",
						State = $"translating {(Character.Length > 0 ? Character : "no one :)")}",
						Assets = new Assets()
						{
							LargeImageKey = ImageKey,
							LargeImageText = Character
						}
					});
					_ = DiscordPresenceClient?.UpdateStartTime();
					Update();
				}
				catch
				{
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
				if (((Settings)Settings.Default).EnableDiscordRP)
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
			if (((Settings)Settings.Default).EnableDiscordRP && DataBase.IsOnline)
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
