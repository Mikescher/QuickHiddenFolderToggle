using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;
using QuickHiddenFolderToggle.Properties;

namespace QuickHiddenFolderToggle
{
	public class QHFTProgram : Form
	{
		private const string REG_KEY   = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
		private const string REG_IDENT = @"Hidden";

		private const string ABOUT_URL = @"http://www.mikescher.com";
		private const string VERSION = @"1.0";

		[STAThread]
		static void Main() { Application.Run(new QHFTProgram()); }

		private readonly NotifyIcon trayIcon;

		private QHFTProgram()
		{
			var trayMenu = new ContextMenu();
			trayMenu.MenuItems.Add("Show hidden folder", (o, e) => { ShowHiddenFolders();});
			trayMenu.MenuItems.Add("Hide hidden folder", (o, e) => { HideHiddenFolders(); });
			trayMenu.MenuItems.Add("Open Website", (o, e) => { About(); });
			trayMenu.MenuItems.Add("Exit", OnExit);

			trayIcon = new NotifyIcon
			{
				Text = "QuickHiddenFolderToggle v" + VERSION,
				Icon = Resources.eye_open,
				ContextMenu = trayMenu,
				Visible = true
			};

			trayIcon.Click += (o, e) => { ToggleShowFolders(); };

			UpdateIcon();
		}

		private void About()
		{
			Process.Start(ABOUT_URL);
		}

		private void UpdateIcon()
		{
			if (IsShowHiddenFolders())
				trayIcon.Icon = Resources.eye_open;
			else
				trayIcon.Icon = Resources.eye_closed;
		}

		private bool IsShowHiddenFolders()
		{
			try
			{
				using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REG_KEY))
				{
					if (key == null)
					{
						MessageBox.Show(string.Format("Registryvalue '{0}' not found", REG_KEY));
						return false;
					}

					return (int)key.GetValue(REG_IDENT) == 1;
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
				return false;
			}
		}

		private void ToggleShowFolders()
		{
			if (IsShowHiddenFolders())
				HideHiddenFolders();
			else
				ShowHiddenFolders();
		}

		private void HideHiddenFolders()
		{
			try
			{
				using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REG_KEY, true))
				{
					if (key == null)
					{
						MessageBox.Show(string.Format("Registryvalue '{0}' not found", REG_KEY));
						return;
					}

					key.SetValue(REG_IDENT, 0);
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
				return;
			}
			
			RefreshWindows();

			UpdateIcon();
		}

		private void ShowHiddenFolders()
		{
			try
			{
				using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REG_KEY, true))
				{
					if (key == null)
					{
						MessageBox.Show(string.Format("Registryvalue '{0}' not found", REG_KEY));
						return;
					}

					key.SetValue(REG_IDENT, 1);
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
				return;
			}

			RefreshWindows();

			UpdateIcon();
		}

		private static void RefreshWindows()
		{
			Type shellApplicationType = Type.GetTypeFromCLSID(new Guid("13709620-C279-11CE-A49E-444553540000"), true);
			dynamic shellApplication = Activator.CreateInstance(shellApplicationType);
			dynamic windows = shellApplication.Windows();
			for (int i = 0; i < windows.Count; i++)
				windows.Item(i).Refresh();
		}

		protected override void OnLoad(EventArgs e)
		{
			Visible = false;
			ShowInTaskbar = false;

			base.OnLoad(e);
		}

		private void OnExit(object sender, EventArgs e)
		{
			Application.Exit();
		}

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				// Release the icon resource.
				trayIcon.Dispose();
			}

			base.Dispose(isDisposing);
		}
	}
}
