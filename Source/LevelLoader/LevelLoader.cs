using System;
using System.Collections.Generic;
using System.Text;

using DCPM.Common;
using DCPM.PluginBase;

using UnityEngine;

namespace LevelLoader
{
    public class LevelLoader : DeadCorePlugin
    {
		#region IPluginInfo
		public override string Name { get { return "Level Loader"; } }
		public override string Author { get { return "Standalone"; } }
		public override string Version { get { return "1.0"; } }
		public override string Desc { get { return "Level loading shennanigans"; } }
		#endregion

		void Awake()
		{
			PluginConsole.RegisterConsoleCommand("load_level", Cmd_LoadLevel, "Loads a specified level", this);
		}

		void OnLevelWasLoaded(int index)
		{
			PluginConsole.WriteLine("Level index: " + index, this);
			PluginConsole.WriteLine("Level name: " + Application.loadedLevelName, this);
		}

		void Cmd_LoadLevel(string[] input)
		{
			int index;	
			if (input.Length > 0 && int.TryParse(input[0], out index)) //&& DeadCoreLevels.UnfriendlyNames.ContainsKey(index))
			{
				//PluginConsole.WriteLine("Loading '" + DeadCoreLevels.FriendlyNames[index] + "'", this);
				//SceneLoader.LoadLevel(DeadCoreLevels.UnfriendlyNames[index]);
				Application.LoadLevel(index);
			}
			else
			{
				PluginConsole.WriteLine("Invalid Argument", this);
			}
		}
	}
}
