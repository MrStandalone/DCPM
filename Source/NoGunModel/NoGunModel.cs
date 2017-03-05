using DCPM.Common;
using DCPM.PluginBase;

using UnityEngine;

namespace NoGunModel
{
	public class NoGunModel : DeadCorePlugin
	{
		#region IPluginInfo
		public override string Name		{ get { return "No Gun Model"; } }
		public override string Author	{ get { return "Standalone"; } }
		public override string Version	{ get { return "1.0"; } }
		public override string Desc		{ get { return "Allows you to toggle drawing of the gun model & aiming reticule (using this will break your aim down sights though)"; } }
		#endregion

		#region Data Members

		#endregion

		#region Registered Console Commands
		void Cmd_ToggleGunDraw(string[] input)
		{
			WeaponScript[] weaponScripts = Resources.FindObjectsOfTypeAll<WeaponScript>();

			HUDScript[] hudScripts = Resources.FindObjectsOfTypeAll<HUDScript>();

			foreach (var script in weaponScripts)
			{
				PluginConsole.WriteLine("Toggling WeaponScript, this will break Iron Sights", this);
				script._weapon.SetActive(!script._weapon.activeSelf);
			}

			foreach (var script in hudScripts)
			{
				PluginConsole.WriteLine("Toggling HUDScript", this);
				script._visible = !script._visible;
			}
		}
		#endregion

		#region Unity Methods
		void Awake()
		{
			PluginConsole.RegisterConsoleCommand("togglegundraw", Cmd_ToggleGunDraw, "Toggles whether the gun model is drawn or not", this);
		}
		#endregion
	}
}
