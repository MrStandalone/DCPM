using System;

using DCPM.Common;
using DCPM.PluginBase;

using UnityEngine;

class ServerHosting : MonoBehaviour, IPluginInfo
{
	#region IPluginInfo
	public string Name		{ get { return "DCMP - Server Hosting"; } }
	public string Author	{ get { return "Standalone"; } }
	public string Version	{ get { return "0.0.1"; } }
	public string Desc		{ get { return "Server Hosting UI"; } }
	#endregion IPluginInfo

	#region Data Members
	//Hosting Menu Members
	float hostingWidth = 300;
	float hostingHeight = 150;
	float hostingTop;
	float hostingLeft;

	string hostingGameName = GlobalVars.SteamName + "'s Game";
	string hostingComment = "Single Level / Story Mode";
	string hostingPortString = "Port [0-65535]";
	int hostingPort;
	bool hostingPublic = true;
	bool hostingUseNAT = true;
	string hostingMaxClientsString = "Max Clients [0-32]";
	int hostingMaxClients;

	NetworkConnectionError networkError;
	#endregion

	#region Unity Methods
void OnGUI()
{
	if (MultiplayerMenu.Instance.ActiveUserInterface == MultiplayerMenu.UserInterface.ServerHosting)
	{
		//Hosting UI Setup
		hostingTop = Screen.height / 2 - hostingHeight / 2;
		hostingLeft = Screen.width / 2 - hostingWidth / 2;

		if (Network.isServer || Network.isClient)
		{
			GUI.Box(new Rect(hostingLeft, hostingTop + 50, hostingWidth, 50), "Already Connected to/Hosting a Server");
			GUILayout.BeginArea(new Rect(hostingLeft + 5, hostingTop + 70, hostingWidth - 10, 50));
			if (GUILayout.Button("Disconnect"))
			{
				Network.Disconnect();
			}
			GUILayout.EndArea();
		}
		else
		{
			GUI.Box(new Rect(hostingLeft, hostingTop, hostingWidth, hostingHeight), "Host a Lobby");
			GUILayout.BeginArea(new Rect(hostingLeft + 5, hostingTop + 20, hostingWidth - 10, hostingHeight - 25));

			GUILayout.BeginHorizontal();
			GUILayout.Label("Name:");
			hostingGameName = GUILayout.TextField(hostingGameName, GUILayout.Width(hostingWidth - 85));
			GUILayout.Space(5);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Comment:");
			hostingComment = GUILayout.TextField(hostingComment, GUILayout.Width(hostingWidth - 85));
			GUILayout.Space(5);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Port:");
			hostingPortString = GUILayout.TextField(hostingPortString, GUILayout.Width(hostingWidth - 85));
			GUILayout.Space(5);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Clients:");
			hostingMaxClientsString = GUILayout.TextField(hostingMaxClientsString, GUILayout.Width(hostingWidth - 85));
			GUILayout.Space(5);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Space(5);
			if (GUILayout.Button("Create Lobby"))
			{
				if (int.TryParse(hostingPortString, out hostingPort) && hostingPort > 0 && hostingPort <= 65535)
				{
					if (int.TryParse(hostingMaxClientsString, out hostingMaxClients) && hostingMaxClients > 0 && hostingMaxClients < 32)
					{
						networkError = Network.InitializeServer(hostingMaxClients, hostingPort, hostingUseNAT);

						if (networkError == NetworkConnectionError.NoError)
						{
							PluginConsole.WriteLine("Server Started on " + Network.player.externalIP + ":" + hostingPort, this);
							if (hostingPublic)
							{
								PluginConsole.WriteLine("Registering server with MasterServer", this);
								MasterServer.RegisterHost(NetworkManager.GameModeName, hostingGameName, hostingComment);
							}
						}

							MultiplayerMenu.Instance.ActiveUserInterface = MultiplayerMenu.UserInterface.None;
						//SimpleMultiplayer.showLobby = true;
					}
					else
					{
						hostingMaxClientsString = "Invalid Max Clients";
					}
				}
				else
				{
					hostingPortString = "Invalid Port Number";
				}
			}
			GUILayout.Space(5);
			GUILayout.EndHorizontal();

			GUILayout.EndArea();
		}
	}
}
#endregion Unity Methods

#region Unity Network Methods
void OnMasterServerEvent(MasterServerEvent msEvent)
{
	if (msEvent == MasterServerEvent.RegistrationSucceeded)
		PluginConsole.WriteLine("Server registered with Master Server", this);
	else if (msEvent != MasterServerEvent.RegistrationSucceeded && msEvent != MasterServerEvent.HostListReceived)
		PluginConsole.WriteLine("Failed to register server with Master Server");
}
#endregion Unity Network Methods
}