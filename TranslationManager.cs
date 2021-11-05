using System;
using System.Collections.Generic;
using System.IO;

public class TranslationManager
{
	public static TranslationManager main;

	public string SourceFilePath 
	{
		get
		{
			return sourceFilePath;
		}
		set
		{
			if (sourceFilePath != "") 
			{
				//TODO close opened file here
			} 
			sourceFilePath = value;
			LoadSourceFile(value);
		}
	}
	private string sourceFilePath = "";

	private Dictionary<string, LineData> lineDataDict = new Dictionary<string, LineData>();



	public TranslationManager()
	{
		if (main != null) 
		{
			return;
		}
		main = this;
	}

	private void LoadSourceFile(string path) 
	{
		string folderPath = Path.GetDirectoryName(path);
		string fileName = Path.GetFileNameWithoutExtension(path);
		string checkFilePath = Path.Combine(folderPath, fileName + "_checkFile.txt");

		string[] lines = File.ReadAllLines(path);

		if (!File.Exists(checkFilePath))
		{
			Console.WriteLine("Check file is missing, creating it");
			FileStream checkFileStream = File.Create(checkFilePath);
			StreamWriter checkFileWriter = new StreamWriter(checkFileStream);
			foreach (string line in lines) 
			{
				if (line.Contains("|"))
				{
					string[] parts = line.Split('|');
					checkFileWriter.WriteLine(parts[0] + "|false|false|");
				}
				else 
				{
					checkFileWriter.WriteLine(line);
				}
			}
			checkFileWriter.Flush();
		}
	}
}
