using System;
using System.IO;
using System.Collections.Generic;

using DCPM.Common;
using DCPM.PluginBase;

using UnityEngine;

namespace DCPM
{
	class PluginManager : IPluginInfo
	{
		#region IPluginInfo
		//While not 100% necessary to implement this interface, we are doing so for ease of use with the PluginConsole
		public string Name		{ get { return "Plugin Manager"; } }
		public string Author	{ get { return "Standalone"; } }
		public string Version	{ get { return "1.1"; } }
		public string Desc		{ get { return "Provides a simple platform for creating and deploying plugins for DeadCore"; } }
		#endregion

		#region Data Members
		//Location of the plugins folder
		string pluginsLocation;

		//List that contains references to the MonoBehaviour of the created Unity components
		List<DeadCorePlugin> loadedPlugins;

		//Empty gameobject we'll be attaching our plugin scripts to
		GameObject pluginContainerObject;
		#endregion

		#region Registered Console Commands
		void Cmd_ListPlugins(string[] input)
		{
			int count = 0;
			foreach (IPluginInfo pluginInfo in loadedPlugins)
			{
				PluginConsole.WriteLine(count++ + ": " + pluginInfo.Name + " version " + pluginInfo.Version + " by " + pluginInfo.Author, this);
			}
		}

		void Cmd_PluginInfo(string[] input)
		{
			int index;
			if (input.Length >= 1 && int.TryParse(input[0], out index) && index >= 0 && loadedPlugins.Count > index)
			{
				PluginConsole.WriteLine(loadedPlugins[index].Name + " version " + loadedPlugins[index].Version + " by " + loadedPlugins[index].Author, this);
				PluginConsole.WriteLine(loadedPlugins[index].Name + " Description: " + loadedPlugins[index].Desc, this);
			}
			else
			{
				PluginConsole.WriteLine("Invalid argument", this);
			}
		}

		void Cmd_Exit(string[] input)
		{
			Application.Quit();
			PluginConsole.WriteLine("Exiting...please wait.", this);
		}
		#endregion

		#region Public Methods
		public void Initialize()
		{
			//Create a persistent empty Unity GameObject to attach plugin scripts to
			pluginContainerObject = new GameObject();
			UnityEngine.Object.DontDestroyOnLoad(pluginContainerObject);

			loadedPlugins = new List<DeadCorePlugin>();

			//Ensure the PluginConsole is the first plugin loaded as other plugins require it's use
			loadedPlugins.Add(pluginContainerObject.AddComponent<PluginConsole>());

			//Ensure the watermark BreadSmugged plugin is loaded as well
			loadedPlugins.Add(pluginContainerObject.AddComponent<BreadSmugged>());

			pluginsLocation = GlobalVars.RootFolder + "\\dcpm-plugins\\";
			
			//Ensure the plugins directory exists
			Directory.CreateDirectory(pluginsLocation);

			//Register several helpful console commands
			PluginConsole.RegisterConsoleCommand("plugins_list", Cmd_ListPlugins, "Lists all automatically loaded plugins", this);
			PluginConsole.RegisterConsoleCommand("plugins_info", Cmd_PluginInfo, "Display more detailed information about a loaded plugin, usage: 'plugins_info <plugin id>' get the plugin id by using 'plugins_list'", this);
			PluginConsole.RegisterConsoleCommand("exit", Cmd_Exit, "Exit the game after a short period of time", this);

			PluginConsole.WriteLine("Version " + Version + " Initialized", this);

			LoadAllPlugins();
		}

		//I'll probably change how this all works at some stage in the future
		void LoadAllPlugins()
		{
			PluginConsole.WriteLine("Loading Plugins", this);

			//Do this a neater way
			IEnumerable<Type> pluginTypes = AssemblyManager.Instance.GetTypes<DeadCorePlugin>(AssemblyManager.Instance.LoadAssemblies(pluginsLocation));
			DeadCorePlugin plugin;

			PluginConsole.WriteLine("'DeadCorePlugin' Types loaded from Assemblies", this);

			foreach(Type type in pluginTypes)
			{
				try 
				{
					PluginConsole.WriteLine("Attempting to attach " + type.Name, this);
					plugin = pluginContainerObject.AddComponent(type) as DeadCorePlugin;
					loadedPlugins.Add(plugin);
					PluginConsole.WriteLine(plugin.Name + " version " + plugin.Version + " has been attached to the container", this);
                }
				catch (Exception ex)
				{
					PluginConsole.WriteLine("Error attaching " + type.Name + " to the plugin GameObject container", this);
					PluginConsole.WriteLine(ex.ToString(), this);
				}
			}

			PluginConsole.WriteLine("Plugins Loaded", this);

			PluginConsole.WriteLine("Use 'help' to list available console commands", this);
		}
		#endregion
	}
}
