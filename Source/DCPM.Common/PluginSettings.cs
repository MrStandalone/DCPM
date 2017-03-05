using System;
using System.Collections.Generic;
using System.Text;

using DCPM.PluginBase;

using UnityEngine;
using System.IO;

namespace DCPM.Common
{
	/// <summary>
	/// Simple utility class for adding and retrieving shit from a file
	/// 
	/// TODO:
	/// </summary>
	public class PluginSettings : IPluginInfo
	{
		#region IPluginInfo
		//While not 100% necessary to implement this interface, we are doing so for ease of use with the PluginConsole
		public string Name		{ get { return "Plugin Settings"; } }
		public string Author	{ get { return "Standalone"; } }
		public string Version	{ get { return "1.1"; } }
		public string Desc		{ get { return "Simple settings class, not actually a plugin"; } }
		#endregion

		#region Data Members
		Dictionary<string, string> settingsDictionary;

		string settingsLocation;
		string settingsFile;

		static PluginSettings instance;
		public static PluginSettings Instance
		{
			get
			{
				if (instance == null)
					instance = new PluginSettings();

				return instance;
			}
		}
		#endregion

		PluginSettings()
		{
			settingsDictionary = new Dictionary<string, string>();
			settingsLocation = GlobalVars.RootFolder + "\\dcpm-settings\\";
			settingsFile = "settings.txt";

			LoadSettings();

			PluginConsole.WriteLine("Version " + Version + " Initialized", this);
		}

		#region Public Methods
		/// <summary>
		/// Sets or creates a setting for a UnityEngine.KeyCode identified by a string, returns false if key is already bound
		/// </summary>
		/// <param name="settingName"></param>
		/// <param name="keyCode"></param>
		public bool SetKeyCode(string settingName, KeyCode keyCode)
		{
			bool returnVal = true;
			
			if (settingsDictionary.ContainsValue(keyCode.ToString()))
			{
				returnVal = false;
				PluginConsole.WriteLine("KeyCode '" + keyCode.ToString() + "' is already bound", this);
			}
			else
			{
				if (settingsDictionary.ContainsKey(settingName))
					settingsDictionary[settingName] = keyCode.ToString();
				else
					settingsDictionary.Add(settingName, keyCode.ToString());

				SaveSettings();
			}

			return returnVal;
		}

		/// <summary>
		/// Retrieves a setting from the Dictionary or returns the defaultKeyCode (and creates the setting) if no setting exists
		/// </summary>
		/// <param name="settingName"></param>
		/// <param name="defaultKeyCode"></param>
		/// <returns>Stored KeyCode for the settingName or defaultKeyCode if one does not exist</returns>
		public KeyCode GetKeyCode(string settingName, KeyCode defaultKeyCode)
		{
			KeyCode returnVal = defaultKeyCode;	

			if (settingsDictionary.ContainsKey(settingName))
            {
				try
				{
					returnVal = (KeyCode) Enum.Parse(typeof(KeyCode), settingsDictionary[settingName]);
                }
				catch
				{
					PluginConsole.WriteLine("Error converting '" + settingName + "' = '" + settingsDictionary[settingName] + "' to a UnityEngine.KeyCode", this);
				}
			}
			else
			{
				SetKeyCode(settingName, defaultKeyCode);
			}

			return returnVal;
		}

		/// <summary>
		/// Creates a new setting or edits an existing setting
		/// </summary>
		/// <param name="settingName"></param>
		/// <param name="settingValue"></param>
		/// <returns></returns>
		public void SetSetting(string settingName, object settingValue)
		{
			if (settingsDictionary.ContainsKey(settingName))
				settingsDictionary[settingName] = settingValue.ToString();
			else
				settingsDictionary.Add(settingName, settingValue.ToString());

			SaveSettings();
		}

		/// <summary>
		/// Gets an existing setting or creates it with a default value if it does not exist
		/// </summary>
		/// <param name="settingName"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public string GetSetting(string settingName, object defaultValue)
		{
			string returnVal = defaultValue.ToString();

			if (settingsDictionary.ContainsKey(settingName))
			{
				returnVal = settingsDictionary[settingName];
			}
			else if (defaultValue != null)
			{
				SetSetting(settingName, defaultValue);
			}

			return returnVal;
		}
		#endregion

		#region Private Methods
		void SaveSettings()
		{
			Directory.CreateDirectory(settingsLocation);

			try 
			{
				using (StreamWriter sw = new StreamWriter(new FileStream(settingsLocation + settingsFile, FileMode.Create, FileAccess.Write, FileShare.None)))
				{
					foreach (KeyValuePair<String, String> entry in settingsDictionary)
					{
						sw.WriteLine("{0} = {1}", entry.Key, entry.Value);
					}
				}
			} 
			catch (Exception ex)
			{
				PluginConsole.WriteLine(ex.ToString(), this);
			}
				

			PluginConsole.WriteLine("Settings saved to file", this);
		}

		//Probably need some more error checking in here in case someone royally fuckes up the settings.conf file but for now this will suffice
		void LoadSettings()
		{
			string line;
			string[] strings, separators = { " = " };

			Directory.CreateDirectory(settingsLocation);

			if (File.Exists(settingsLocation + settingsFile))
			{
				PluginConsole.WriteLine("Loading Dictionary from file", this);
				using (StreamReader sr = new StreamReader(new FileStream(settingsLocation + settingsFile, FileMode.OpenOrCreate, FileAccess.Read)))
				{
					while (sr.Peek() >= 0)
					{
						line = sr.ReadLine();

						strings = line.Split(separators, StringSplitOptions.None);

						settingsDictionary.Add(strings[0], strings[1]);
						PluginConsole.WriteLine("Added setting '" + strings[0] + "' = '" + strings[1] + "' to the settings dictionary", this);
					}
				}

				PluginConsole.WriteLine("Settings Dictionary loaded from file", this);
			}
			else
			{
				PluginConsole.WriteLine("No settings file found, creating an empty one", this);
				//File.Create(settingsLocation + settingsFile);
			}
		}
		#endregion
	}
}
