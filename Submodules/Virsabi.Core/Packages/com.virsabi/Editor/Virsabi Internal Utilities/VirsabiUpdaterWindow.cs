#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Virsabi.Internal
{
	public class VirsabiUpdaterWindow : MonoBehaviour
	{
		[InitializeOnLoad]
		public class VirsabiUpdateWindow : EditorWindow
		{
			public static bool AutoUpdateCheckIsEnabled = true;

			private static VirsabiVersion _installedVersion;
			private static VirsabiVersion _latestVersion;

			private static EditorWindow _windowInstance;


			static VirsabiUpdateWindow()
			{
				if (AutoUpdateCheckIsEnabled)
				{
					VirsabiInternalUtilities.GetVirsabiLatestVersionAsync(version =>
					{
						_installedVersion = VirsabiInternalUtilities.GetVirsabiInstalledVersion();
						_latestVersion = version;
						if (!_installedVersion.VersionsMatch(_latestVersion))
						{
							var versions = "Installed version: " + _installedVersion.AsSting + ". Latest version: " + _latestVersion.AsSting;
							Debug.Log("It's time to update Virsabi internal scripts :)! Use \"Virsabi Tools/Update Virsabi Tools\". " + versions);
						}
					});
				}
			}

			[MenuItem("Virsabi Tools/Update Virsabi Tools", false, 10000)]
			private static void VirsabiUpdateMenuItem()
			{
				_windowInstance = GetWindow<VirsabiUpdateWindow>();
				_windowInstance.titleContent = new GUIContent("Update Virsabi Library");
			}

			private void OnEnable()
			{
				_windowInstance = this;

				_installedVersion = VirsabiInternalUtilities.GetVirsabiInstalledVersion();
				VirsabiInternalUtilities.GetVirsabiLatestVersionAsync(version =>
				{
					_latestVersion = version;
					if (_windowInstance != null) _windowInstance.Repaint();
				});
			}

			private string GetPackageInstallDescription()
			{
				string desc = "";

				switch (VirsabiInternalUtilities._InstallMethod)
				{
					case VirsabiInstallMethod.masterUpm:
						desc = "Your Virsabi Tools package is installed from the master UPM branch! Version is controlled by the Package Manager!";
						break;
					case VirsabiInstallMethod.upmDev:
						desc = "Your Virsabi Tools package is installed from the development UPM branch! Version can be controlled by the Package Manager, but the upmDev branch does not create releases so the below 'latest version on upm repo' may have an update for you!";
						break;
					case VirsabiInstallMethod.fromDisk:
						desc = "Your Virsabi Tools package is installed and from disk - You can develop directly in the package folder. Version is externally controlled by git!";
						break;
					default:
						break;
				}

				return desc;
			}


			private void OnGUI()
			{
				EditorGUILayout.HelpBox(GetPackageInstallDescription(), MessageType.Info);

				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Current version: " + (_installedVersion == null ? "..." : _installedVersion.AsSting));
				
				if(VirsabiInternalUtilities._InstallMethod != VirsabiInstallMethod.fromDisk)
					EditorGUILayout.LabelField("Latest version on upm repo: " + (_latestVersion == null ? "..." : _latestVersion.AsSting));
				else
					EditorGUILayout.LabelField("Installed on disk - Latest version on upm repo: " + (_latestVersion == null ? "..." : _latestVersion.AsSting));

				using (new EditorGUILayout.HorizontalScope())
				{
					if (VirsabiInternalUtilities._InstallMethod != VirsabiInstallMethod.fromDisk)
					{
						if (GUILayout.Button("Update GIT packages", EditorStyles.toolbarButton))
						{
							if (!VirsabiInternalUtilities.UpdateGitPackages())
								ShowNotification(new GUIContent("There is no git packages installed"));
						}
					}
					

					if (GUILayout.Button("Open Git releases page", EditorStyles.toolbarButton))
					{
						VirsabiInternalUtilities.OpenVirsabiGitInBrowser();
					}
				}
			}
		}
	}
}
#endif