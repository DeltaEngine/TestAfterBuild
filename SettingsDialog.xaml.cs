using System.Windows;
using EnvDTE80;

namespace DeltaEngine.TestAfterBuild
{
	public partial class SettingsDialog
	{
		private static readonly string[] CommonCommands = new[]
		{
			"ReSharper.ReSharper_UnitTestSession_RepeatPreviousRun",
			"ReSharper.ReSharper_UnitTest_RunSolution",
			"ReSharper.ReSharper_UnitTest_RunContext",
			"ReSharper.ReSharper_UnitTest_RunCurrentSession",
			"TestDriven.NET.RerunTests",
			"TestDriven.NET.Client",
			"TestDriven.NET.RunTests",
			"Test.RunTestsInClass",
			"Test.RunTestsInCurrentContext",
			"Test.RunTestsInNamespace",
			"TestExplorer.RunNotRunTests",
			"EditorContextMenus.CodeWindow.RunTests",
			"NCrunch.RuntestspinnedtoTestsWindow",
			"NCrunch.RuntestsvisibleinTestsWindow"
		};
		private readonly DTE2 dte;

		public SettingsDialog(DTE2 setDte)
		{
			dte = setDte;
			InitializeComponent();
			SelectedCommand.ItemsSource = CommonCommands;
			SelectedCommand.Text = Command.Current;
			UpdateWaitMessage();
		}

		private void UpdateWaitMessage()
		{
			if (Command.WaitTimeInSeconds > 0)
				SetWaitMessage();
			else
				UseWaitTime.IsChecked = false;
		}

		private void SetWaitMessage()
		{
			WaitTime.Text = Command.WaitTimeInSeconds.ToString();
			UseWaitTime.IsChecked = true;
			UseWaitTime.Content = "After executing the command wait " + Command.WaitTimeInSeconds +
				" seconds before executing again";
		}

		private void SaveClicked(object sender, RoutedEventArgs e)
		{
			Command.WaitTimeInSeconds = 0;
			GetWaitTime();
			Command.Current = SelectedCommand.Text;
			Command.Save();
			Close();
		}

		private void GetWaitTime()
		{
			if (UseWaitTime.IsChecked == true)
				int.TryParse(WaitTime.Text, out Command.WaitTimeInSeconds);
		}

		private void DisableClicked(object sender, RoutedEventArgs e)
		{
			Command.DeleteSaveFile();
			Close();
		}

		private void DisableResharperBuildSettingsClicked(object sender, RoutedEventArgs e)
		{
			dte.ExecuteCommand("ReSharper_UnitTestSession_BuildPolicy_Never");
		}

		private void WaitTimeChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			GetWaitTime();
			UpdateWaitMessage();
		}
	}
}
