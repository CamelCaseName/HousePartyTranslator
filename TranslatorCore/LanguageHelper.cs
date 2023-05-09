﻿using System;
using System.Collections.Generic;

namespace Translator.Core.Helpers
{
	/// <summary>
	/// A Class providing a list of all short keys for languages, as well the String representation of it.
	/// </summary>
	public static class LanguageHelper
	{
		public class LanguageException : Exception
		{
			public LanguageException() : base("You need to enter or select a valid language first") { }
			public LanguageException(string message) : base(message) { }
		}

		//todo add more languages
		/// <summary>
		/// A list of all currently supported languages in short form
		/// </summary>
		//place new languages here, plus add string representation
		public static readonly string[] ShortLanguages = {
			"cs",
			"da",
			"de",
			"fi",
			"fr",
			"hu",
			"it",
			"ja",
			"ko",
			"pl",
			"pt",
			"ptbr",
			"es",
			"esmx",
			"tr",
			"cn",
			"nl",
			"ro",
			"sw",
			"cr"
		};

		/// <summary>
		/// A Dictionary with all String representations of the languages given in short form as the key
		/// </summary>
		public static readonly Dictionary<string, string> Languages = new Dictionary<string, string>() {
			{ShortLanguages[0], "Czech" },
			{ShortLanguages[1], "Danish" },
			{ShortLanguages[2], "German" },
			{ShortLanguages[3], "Finnish" },
			{ShortLanguages[4], "French" },
			{ShortLanguages[5], "Hungarian" },
			{ShortLanguages[6], "Italian" },
			{ShortLanguages[7], "Japanese" },
			{ShortLanguages[8], "Korean" },
			{ShortLanguages[9], "Polish" },
			{ShortLanguages[10], "Purtuguese" },
			{ShortLanguages[11], "Portuguese, Brazilian" },
			{ShortLanguages[12], "Spanish" },
			{ShortLanguages[13], "Spanish, Mexico" },
			{ShortLanguages[14], "Turkish" },
			{ShortLanguages[15], "Chinese" },
			{ShortLanguages[16], "Dutch" },
			{ShortLanguages[17], "Romanian" },
			{ShortLanguages[18], "Swedish" },
			{ShortLanguages[19], "Croatian" }
        };
	}
}
