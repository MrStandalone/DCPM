using System;

using DCPM.Common;
using DCPM.PluginBase;

using UnityEngine;

class MultiplayerChat : MonoBehaviour, IPluginInfo
{
	#region IPluginInfo
	public string Name		{ get { return "DCMP - Multiplayer Chat"; } }
	public string Author	{ get { return "Standalone"; } }
	public string Version	{ get { return "0.0.1"; } }
	public string Desc		{ get { return "Multiplayer Chat UI"; } }
	#endregion IPluginInfo

	#region Data Members

	#endregion
}
