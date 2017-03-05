using System;
using System.Collections.Generic;

using UnityEngine;
using DCPM.Common;
using DCPM.PluginBase;

public class NoSplash : DeadCorePlugin
{
	#region IPluginInfo
	public override string Name { get { return "No Splash Screens"; } }
	public override string Author { get { return "Standalone"; } }
	public override string Version { get { return "0.1"; } }
	public override string Desc { get { return "Skips the splash screens at the start of the game."; } }
	#endregion

	#region Unity Methods
	void OnLevelWasLoaded(int level)
	{
		if (level == 1)
			SceneLoader.LoadLevel("MainMenu");
	}
	#endregion
}

