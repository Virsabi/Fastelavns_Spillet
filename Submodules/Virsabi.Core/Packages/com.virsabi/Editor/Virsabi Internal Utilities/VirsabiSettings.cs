using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyBox;

using Object = UnityEngine.Object;
using System.IO;

namespace Virsabi.Internal
{
	/// <summary>
	/// Settings are written to ProjectSettings/VirsabiSettings
	/// If you change this code so that the text-serialized file is longer/shorter you'll have to delete this file in order to regenerate the settings and load correctly.
	/// </summary>
	public class VirsabiSettings : ScriptableObject
	{
        
		/// <summary>
		/// This region holds logic for saving and loading our Virsabi related settings to a VirsabiSettings.asset under the project settings in the unity project.
		/// </summary>
        #region Instance
        private static VirsabiSettings Instance
		{
			get
			{
				if (_instance != null) return _instance;
				_instance = LoadOrCreate();
				return _instance;
			}
		}


		private static readonly string Directory = "ProjectSettings";
		private static readonly string Path = Directory + "/VirsabiSettings.asset";
		private static VirsabiSettings _instance;

		private static void Save()
		{
			var instance = _instance;
			if (!System.IO.Directory.Exists(Directory)) System.IO.Directory.CreateDirectory(Directory);
			try
			{
				UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] { instance }, Path, true);
			}
			catch (Exception ex)
			{
				Debug.LogError("Unable to save VirsabiSettings!\n" + ex);
			}
		}

		private static VirsabiSettings LoadOrCreate()
		{
			var settings = !File.Exists(Path) ? CreateNewSettings() : LoadSettings();
			if (settings == null)
			{
				DeleteFile(Path);
				settings = CreateNewSettings();
			}

			settings.hideFlags = HideFlags.HideAndDontSave;

			return settings;
		}
		private static VirsabiSettings LoadSettings()
		{
			VirsabiSettings settingsInstance;
			try
			{
				settingsInstance = (VirsabiSettings)UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(Path)[0];
			}
			catch (Exception ex)
			{
				Debug.LogError("Unable to read VirsabiSettings, set to defaults" + ex);
				settingsInstance = null;
			}

			return settingsInstance;
		}

		private static VirsabiSettings CreateNewSettings()
		{
			_instance = CreateInstance<VirsabiSettings>();
			Save();

			return _instance;
		}

		private static void DeleteFile(string path)
		{
			if (!File.Exists(path)) return;

			var attributes = File.GetAttributes(path);
			if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);

			File.Delete(path);
		}


		#endregion

		#region Warning supression

		/// <summary>
		/// When changing these suppression definitions you need to delete the virsabi settings file to regenerate
		/// AND delete the Assets/csc file.
		/// </summary>
		[SerializeField]
		private List<SupressDefinition> _warningSupresisons = new List<SupressDefinition>
		{
			new SupressDefinition("0649", "Public or private serialized field not assigned"),
			new SupressDefinition("0414", "The field 'xxxx' is assigned but its value is never used."),
			new SupressDefinition("0618", "Class or API obsolete."),
			new SupressDefinition("0067", "An event was declared but never used in the class in which it was declared."),
			new SupressDefinition("0108", "A variable hides an inherited member. Use the new keyword if hiding was intended.")
		};

		public static List<SupressDefinition> WarningSupresisons
		{
			get { return Instance._warningSupresisons; }
			set
			{
				if (Instance._warningSupresisons == value) return;
				Instance._warningSupresisons = value;
				Save();
			}
		}

		//public static bool Supress0649 = false;

		#endregion

		#region Editor Settings
		[SerializeField] private bool _showHelp = false;

		public static bool ShowHelp
		{
			get { return Instance._showHelp; }
			set
			{
				if (Instance._showHelp == value) return;
				Instance._showHelp = value;
				Save();
			}
		}

		[SerializeField] private string _sceneLoaderShowScenesAt;

		public static string SceneLoaderShowScenesAt
		{
			get { return Instance._sceneLoaderShowScenesAt; }
			set
			{
				if (Instance._sceneLoaderShowScenesAt == value) return;
				Instance._sceneLoaderShowScenesAt = value;
				Save();
			}
		}


		#endregion

		#region Script Define Symbols

		/// <summary>
		/// Simply add any new definitions here and use them like '#if URP'
		/// Then delete ProjectSettings/VirsabiSettings.
		/// The editor will automatically adjust to more.
		/// </summary>
		[SerializeField]
		private List<VirsabiSymbol> _virsabiSymbols = new List<VirsabiSymbol>{ 
			new VirsabiSymbol("OVR", "Loads in scripts that use the OVR plugin", false),
			new VirsabiSymbol("TMPRO", "Imports features using TextMeshPRO", true),
			new VirsabiSymbol("UEViz", "Editor tool for visualizing Unity Events", false),
			new VirsabiSymbol("EXPERIMENTAL", "Experimental features", false),
			new VirsabiSymbol("PLAYMAKER_EXTENSIONS", "Playmaker extensions", false),
			new VirsabiSymbol("URP", "Set true if using Universal Render Pipeline", true)
		};

		public static List<VirsabiSymbol> VirsabiSymbols
		{
			get { return Instance._virsabiSymbols; }
			set
			{
				if (Instance._virsabiSymbols == value) return;
				Instance._virsabiSymbols = value;
				Save();
			}
		}

		


		#endregion

		#region Public Classes
		[Serializable]
		public class VirsabiSymbol
		{

			[SerializeField]
			private string description = "I am a symbol";

			[SerializeField]
			private string symbol;

			[SerializeField]
			public bool Enabled
			{
				get => enabled;

				internal set
				{
					if (Instance._virsabiSymbols.Find(x => x == this).enabled == value) 
						return;
					Instance._virsabiSymbols.Find(x => x == this).enabled = value;
					Save();
				}
			}

			[SerializeField]
			public string Symbol { get => symbol; private set => symbol = value; }
			public string Description { get => description; set => description = value; }

			[SerializeField]
			private bool enabled;

			/// <summary>
			/// 
			/// </summary>
			/// <param name="Symbol">The key</param>
			/// <param name="Description">For people</param>
			/// <param name="defualtState">Is this symbol enabled on install?</param>
			public VirsabiSymbol(string Symbol, string Description, bool defualtState)
			{
				this.Symbol = Symbol;
				this.Description = Description;
				this.enabled = defualtState;
			}
		}

        

		[Serializable]
		public class SupressDefinition
		{
			public string Description = "this is a description";
			public readonly string WarningCode;

			[SerializeField]
			private bool enabled;

			public SupressDefinition(string warningCode, string description)
			{
				Description = description;
				WarningCode = warningCode;
				enabled = false;
			}

			[SerializeField]
			public bool Enabled
			{
				get => enabled;

				set
				{
					if (Instance._warningSupresisons.Find(x => x == this).enabled == value)
						return;
					Instance._warningSupresisons.Find(x => x == this).enabled = value;
					Save();
				}
			}
		}

		#endregion
	}
}
