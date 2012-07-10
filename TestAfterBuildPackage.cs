using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace DeltaEngine.TestAfterBuild
{
	[PackageRegistration(UseManagedResourcesOnly = true),
	 InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400),
	 ProvideMenuResource("Menus.ctmenu", 1), Guid(GuidList.guidTestAfterBuildPkgString)]
	public sealed class TestAfterBuildPackage : Package
	{
		private DTE2 dte;
		private bool buildSucceeded;

		protected override void Initialize()
		{
			base.Initialize();
			Command.Load(UserDataPath);
			InitializeDte();
			InitializeMenus();
			AttachToBuildEvents();
			AttachToDotNetDemonEvents();
		}

		private void InitializeDte()
		{
			dte = (DTE2)GetService(typeof (DTE));
		}

		private void InitializeMenus()
		{
			var mcs = GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
			if (mcs != null)
				CreateMenuCommand(mcs, PkgCmdIDList.cmdidSetupTestsAfterBuilding,
					(sender, args) => new SettingsDialog(dte).ShowDialog());
		}

		private void CreateMenuCommand(OleMenuCommandService mcs, uint commandId, EventHandler runCode)
		{
			var menuCommandId = new CommandID(GuidList.guidTestAfterBuildCmdSet, (int)commandId);
			mcs.AddCommand(new MenuCommand(runCode, menuCommandId));
		}

		private void AttachToBuildEvents()
		{
			dte.Events.BuildEvents.OnBuildBegin += (scope, action) => buildSucceeded = true;
			dte.Events.BuildEvents.OnBuildProjConfigDone +=
				(project, projectConfig, platform, solutionConfig, success) =>
				{
					if (success == false)
						buildSucceeded = false;
				};
			dte.Events.BuildEvents.OnBuildDone += BuildEvents_OnBuildDone;
		}

		private void BuildEvents_OnBuildDone(vsBuildScope scope, vsBuildAction action)
		{
			if (action != vsBuildAction.vsBuildActionBuild ||
				buildSucceeded == false)
				return;

			OnBuildSucceeded();
		}

		private void OnBuildSucceeded()
		{
			if (Command.IsDisabled)
				return;

			try
			{
				ExecuteCommandButDoNotLoseFocus(Command.Current);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Failed to execute command '" + Command.Current + "': " + ex);
			}
			finally
			{
				PreventRunningTwice();
			}
		}

		private void ExecuteCommandButDoNotLoseFocus(string command)
		{
			var storeActiveWindow = dte.ActiveWindow;
			Command.LastTimeExecuted = DateTime.Now;
			dte.ExecuteCommand(command);
			storeActiveWindow.Activate();
		}

		private void PreventRunningTwice()
		{
			buildSucceeded = false;
		}

		private void AttachToDotNetDemonEvents()
		{
			List<Action> buildSucceededHandlers = AppDomain.CurrentDomain.GetData(
				"RedGate.Neptune.BuildSucceeded") as List<Action> ?? new List<Action>();
			buildSucceededHandlers.Add(OnBuildSucceeded);
			AppDomain.CurrentDomain.SetData("RedGate.Neptune.BuildSucceeded", buildSucceededHandlers);
		}
	}
}