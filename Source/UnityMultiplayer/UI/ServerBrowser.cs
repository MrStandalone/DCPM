using System;

using DCPM.Common;
using DCPM.PluginBase;

using UnityEngine;

namespace UnityMultiplayer.UI
{
	class ServerBrowser : MonoBehaviour, IPluginInfo
	{
		#region IPluginInfo
		public string Name		{ get { return "DCMP - Server Browser"; } }
		public string Author	{ get { return "Standalone"; } }
		public string Version	{ get { return "0.0.1"; } }
		public string Desc		{ get { return "Server Browser UI"; } }
		#endregion IPluginInfo

		#region Data Members
		HostData[] hostData;

		//UI Layout
		float browserWidth = 800;
		float browserHeight = 250;
		float browserTop;
		float browserLeft;

		Vector2 scrollPos;

		HostListStatus hostListStatus = HostListStatus.NoHostList;
		enum HostListStatus
		{
			NoHostList,
			WaitingForMasterServer,
			RecievedHostList,
		}

		DateTime timeHostListRequested;
		#endregion

		#region Unity Methods
		void OnGUI()
		{
			if (MultiplayerMenu.Instance.ActiveUserInterface == MultiplayerMenu.UserInterface.ServerBrowser)
			{
				//Browser UI Setup
				browserTop = Screen.height / 2 - browserHeight / 2;
				browserLeft = Screen.width / 2 - browserWidth / 2;

				//Check for Host list
				if (hostListStatus == HostListStatus.NoHostList)
				{
					MasterServer.RequestHostList(NetworkManager.GameModeName);
					timeHostListRequested = DateTime.Now;
					hostListStatus = HostListStatus.WaitingForMasterServer;
				}

				GUI.Box(new Rect(browserLeft, browserTop, browserWidth, browserHeight), "Lobby Browser");

				GUI.Box(new Rect(browserLeft + 5, browserTop + 20, browserWidth - 10, browserHeight - 55), "");
				GUILayout.BeginArea(new Rect(browserLeft + 5, browserTop + 20, browserWidth - 10, browserHeight - 20));

				GUILayout.Space(5);
				scrollPos = GUILayout.BeginScrollView(scrollPos);

				if (hostListStatus != HostListStatus.RecievedHostList)
				{
					GUILayout.FlexibleSpace();
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if (hostListStatus == HostListStatus.NoHostList)
					{
						GUILayout.Label("No Host List available from Master Server");
					}
					else
					{
						if ((DateTime.Now - timeHostListRequested).TotalSeconds > 10)
							GUILayout.Label("Waiting for Host List from Master Server... (Taking longer than usual, Master Server may be down)");
						else
							GUILayout.Label("Waiting for Host List from Master Server...");
					}
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					GUILayout.FlexibleSpace();
				}
				else
				{
					if (hostData.Length == 0)
					{
						GUILayout.FlexibleSpace();
						GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label("No Servers Found");
						GUILayout.FlexibleSpace();
						GUILayout.EndHorizontal();
						GUILayout.FlexibleSpace();
					}
					else
					{
						foreach (HostData host in hostData)
						{
							GUILayout.BeginHorizontal();
							GUILayout.Label(host.gameName, GUILayout.Width(400), GUILayout.Height(20));
							GUILayout.Label(host.connectedPlayers + " / " + host.playerLimit, GUILayout.Width(40));
							GUILayout.Label(host.comment, GUILayout.Width(150), GUILayout.Height(20));
							if (GUILayout.Button("Connect"))
							{
								Network.Connect(host);
							}
							GUILayout.EndHorizontal();
						}
					}
				}
				GUILayout.EndScrollView();
				GUILayout.FlexibleSpace();

				GUILayout.BeginHorizontal();
				GUILayout.Space(5);
				if (GUILayout.Button("Refresh Lobby List"))
				{
					MasterServer.ClearHostList();
					MasterServer.RequestHostList(NetworkManager.GameModeName);
					timeHostListRequested = DateTime.Now;
					hostListStatus = HostListStatus.WaitingForMasterServer;
				}
				GUILayout.Space(5);
				GUILayout.EndHorizontal();

				GUILayout.Space(10);
				GUILayout.EndArea();
			}
		}
		#endregion Unity Methods

		#region Unity Networked Methods
		void OnMasterServerEvent(MasterServerEvent msEvent)
		{
			if (msEvent == MasterServerEvent.HostListReceived)
			{
				hostListStatus = HostListStatus.RecievedHostList;
				hostData = MasterServer.PollHostList();
			}
		}
		#endregion Unity Networked Methods
	}
}
