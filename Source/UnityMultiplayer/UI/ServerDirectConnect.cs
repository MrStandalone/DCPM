using System;

using DCPM.Common;
using DCPM.PluginBase;

using UnityEngine;
using System.Net;

namespace UnityMultiplayer.UI
{
	class ServerDirectConnect : MonoBehaviour, IPluginInfo
	{
		#region IPluginInfo
		public string Name		{ get { return "DCMP - Server Direct Connect"; } }
		public string Author	{ get { return "Standalone"; } }
		public string Version	{ get { return "0.0.1"; } }
		public string Desc		{ get { return "Server Direct Connect UI"; } }
		#endregion IPluginInfo

		#region Data Members
		//Direct Connect Menu Members
		float connectWidth = 300;
		float connectHeight = 100;
		float connectTop;
		float connectLeft;

		string connectIPString = "Host Address";
		string connectPortString = "Port [0-65535]";
		int connectPort;
		IPAddress connectIP;
		#endregion Data Members

		#region Unity Methods
		void OnGUI()
		{
			if (MultiplayerMenu.Instance.ActiveUserInterface == MultiplayerMenu.UserInterface.ServerDirectConnect)
			{
				//Direct Connect UI Setup
				connectTop = Screen.height / 2 - connectHeight / 2;
				connectLeft = Screen.width / 2 - connectWidth / 2;

				GUI.Box(new Rect(connectLeft, connectTop, connectWidth, connectHeight), "Direct Connect");
				GUILayout.BeginArea(new Rect(connectLeft + 5, connectTop + 20, connectWidth - 10, connectHeight - 25));

				GUILayout.BeginHorizontal();
				GUILayout.Label("Host:");
				connectIPString = GUILayout.TextField(connectIPString, GUILayout.Width(connectWidth - 85));
				GUILayout.Space(5);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Port:");
				connectPortString = GUILayout.TextField(connectPortString, GUILayout.Width(connectWidth - 85));
				GUILayout.Space(5);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Space(5);
				if (GUILayout.Button("Connect to Lobby"))
				{
					if (int.TryParse(connectPortString, out connectPort) && connectPort > 0 && connectPort <= 65535)
					{
						if (IPAddress.TryParse(connectIPString, out connectIP))
						{
							Network.Connect(connectIPString, connectPort);
							PluginConsole.WriteLine("Attempting to connect to " + connectIPString + ":" + connectPort);
						}
						else
						{
							connectIPString = "Invalid Host Address";
						}
					}
					else
					{
						connectPortString = "Invalid Port Number";
					}
				}
				GUILayout.Space(5);
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
			}
		}
		#endregion Unity Methods
	}
}