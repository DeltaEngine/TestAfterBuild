using System;
using System.IO;

namespace DeltaEngine.TestAfterBuild
{
	public static class Command
	{
		private const string CommandFilename = "TestAfterBuild.Command";
		private static string userPath;
		private static string CommandFilePath
		{
			get { return Path.Combine(userPath, CommandFilename); }
		}

		public static string Current = "";
		public static int WaitTimeInSeconds = 0;
		public static DateTime LastTimeExecuted = new DateTime(2000, 1, 1);

		public static bool IsDisabled
		{
			get
			{
				return String.IsNullOrEmpty(Current) ||
					(DateTime.Now - LastTimeExecuted).TotalSeconds < WaitTimeInSeconds;
			}
		}

		public static void Load(string setUserPath)
		{
			userPath = setUserPath;
			if (File.Exists(CommandFilePath))
				ReadTextFromFile();
		}

		private static void ReadTextFromFile()
		{
			var file = File.OpenText(CommandFilePath);
			Current = file.ReadLine();
			WaitTimeInSeconds = 0;
			int.TryParse(file.ReadLine(), out WaitTimeInSeconds);
			file.Close();
		}

		public static void Save()
		{
			var file = File.CreateText(CommandFilePath);
			file.WriteLine(Current);
			file.WriteLine(WaitTimeInSeconds.ToString());
			file.Close();
		}

		public static void DeleteSaveFile()
		{
			if (File.Exists(CommandFilePath))
				File.Delete(CommandFilePath);
		}
	}
}
