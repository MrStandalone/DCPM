using System;

using DCPM.Common;
using DCPM.PluginBase;

using UnityEngine;
using System.Collections.Generic;

class MultiplayerLobby : MonoBehaviour, IPluginInfo
{
	#region IPluginInfo
	public string Name		{ get { return "DCMP - Multiplayer Lobby"; } }
	public string Author	{ get { return "Standalone"; } }
	public string Version	{ get { return "0.0.1"; } }
	public string Desc		{ get { return "Multiplayer Lobby UI & Logic"; } }
	#endregion IPluginInfo

	#region Data Members
	//Lobby Menu Members
	float lobbyWidth = 500;
	float lobbyHeight = 250;
	float lobbyTop = 20;
	float lobbyLeft;

	public static CurrentGameState currentGameState = CurrentGameState.NotConnected;
	public enum CurrentGameState
	{
		NotConnected,
		InLobby,
		LoadingMap,
		WaitingForLoaders,
		Racing,
		RaceFinished,
		ReturningToLobby
	}

	public enum GameMode
	{
		Single,
		Story
	}
	public GameMode lobbyGameMode = GameMode.Story;

	//Dictionary<string, MPPlayerScript> connectedPlayerObjects = new Dictionary<string, MPPlayerScript>();
	//Dictionary<string, string> connectedPlayerNames = new Dictionary<string, string>();

	Vector2 lobbyPlayersScrollPos;
	Vector2 lobbyMapSelectionScrollPos;

	int lobbySelectedMap = -1;

	Dictionary<int, string> playableLevelIds = new Dictionary<int, string>()
	{
		{ 8, "LEVEL_01" },
		{ 9, "LEVEL_02" },
		{ 10, "LEVEL_03" },
		{ 11, "LEVEL_04" },
		{ 12, "LEVEL_05" },
		{ 16, "LEVEL_01_SPARK_01" },		//Alpha
		{ 17, "LEVEL_01_SPARK_02" },		//Beta
		{ 18, "LEVEL_02_SPARK_01" },		//Gamma
		{ 19, "LEVEL_02_SPARK_02" },		//Delta
		{ 20, "LEVEL_03_SPARK_01" },		//Epsilon
		{ 21, "LEVEL_03_SPARK_02" },		//Zeta
		{ 22, "LEVEL_04_SPARK_01" },		//Eta
		{ 23, "LEVEL_04_SPARK_02" },		//Theta
		{ 24, "LEVEL_05_SPARK_01" },		//Iota
		{ 25, "LEVEL_05_SPARK_02" },		//Kappa
		{ 26, "LEVEL_NICO" },				//Nicolas - Backer
		{ 27, "LEVEL_CLEMENT" },			//Clement - Backer
		{ 28, "LEVEL_WANDRILLE" }			//Wandrille - Backer
	};

	DateTime raceStartTime;
	DateTime raceEndTime;
	DateTime raceDelayStartTime;

	int raceEndDelay = 15;
	int raceStartDelay = 10;
	#endregion Data Members

	#region Data Members

	#endregion

	#region Unity Methods
	void Update()
	{
		if (Input.GetKey(KeyCode.Tab))
		{
			//TODO - Draw the In Game Player status board
		}
	}

	void OnGUI()
	{
		if (GameManager.Instance.CurrentGameState == GameManager.GameState.MainMenu)
		{
			lobbyLeft = Screen.width / 2 - lobbyWidth / 2;

			if (Network.isServer)
			{
				//Draw Server Lobby
				GUI.Box(new Rect(lobbyLeft, lobbyTop, lobbyWidth, lobbyHeight), "Multiplayer Lobby - Hosting");

				GUI.Box(new Rect(lobbyLeft + 5, lobbyTop + 20, lobbyWidth - 210, lobbyHeight - 50), "Player List");
				GUILayout.BeginArea(new Rect(lobbyLeft + 5, lobbyTop + 45, lobbyWidth - 210, lobbyHeight - 80));
				lobbyPlayersScrollPos = GUILayout.BeginScrollView(lobbyPlayersScrollPos);
				GUILayout.BeginHorizontal();
				GUILayout.Label(GlobalVars.SteamName, GUILayout.Height(20));
				GUILayout.EndHorizontal();

				foreach (KeyValuePair<string, MPPlayer> kvp in NetworkManager.Instance.playerGUIDMPPLayerRelation)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label(kvp.Value.playerName, GUILayout.Height(20));
					GUILayout.EndHorizontal();
				}
				GUILayout.EndScrollView();
				GUILayout.EndArea();

				GUI.Box(new Rect(lobbyLeft + lobbyWidth - 200, lobbyTop + 20, 195, lobbyHeight - 50), "Map Selection");
				GUILayout.BeginArea(new Rect(lobbyLeft + lobbyWidth - 200, lobbyTop + 45, 195, lobbyHeight - 75));
				lobbyMapSelectionScrollPos = GUILayout.BeginScrollView(lobbyMapSelectionScrollPos);
				//Draw the button for the Campaign story mode
				GUILayout.BeginHorizontal();
				if (lobbySelectedMap == -1)
						GUI.color = Color.green;
				if (GUILayout.Button("Campaign Story Race"))
				{
						

					lobbySelectedMap = -1;
					lobbyGameMode = GameMode.Story;

					//TODO - RPC Clients and Inform them of the change
				}
				GUI.color = Color.white;
				GUILayout.EndHorizontal();

				foreach (KeyValuePair<int, string> kvp in playableLevelIds)
				{
					GUILayout.BeginHorizontal();
					if (kvp.Key == lobbySelectedMap)
						GUI.color = Color.green;

					if (GUILayout.Button(DeadCoreLevels.FriendlyNames[kvp.Key]))
					{
						lobbySelectedMap = kvp.Key;
						lobbyGameMode = GameMode.Single;
							
						networkView.RPC("NetworkMessage", RPCMode.All, "[Lobby] Host has changed the selected map to " + DeadCoreLevels.FriendlyNames[lobbySelectedMap]);
					}
					GUILayout.EndHorizontal();

					GUI.color = Color.white;
				}
				GUILayout.EndScrollView();
				GUILayout.EndArea();

				if (GUI.Button(new Rect(lobbyLeft + 5, lobbyTop + lobbyHeight - 30, lobbyWidth - 10, 25), "Start Match"))
				{
					PluginConsole.WriteLine("Attempting to start match", this);
					//PluginConsole.WriteLine(networkView.ToString());
					//networkView.RPC("NetworkMessage", RPCMode.All, "[Lobby] Host has started the game, map will load shortly");
					gameObject.networkView.RPC("LoadLevel", RPCMode.All, lobbySelectedMap, (int) lobbyGameMode);
				}
			}
			else if (Network.isClient)
			{
				//Draw Client Lobby
				GUI.Box(new Rect(lobbyLeft, lobbyTop, lobbyWidth, lobbyHeight), "Multiplayer Lobby - Client");
				GUI.Box(new Rect(lobbyLeft + 5, lobbyTop + 20, lobbyWidth - 10, lobbyHeight - 25), "Player List");
				GUILayout.BeginArea(new Rect(lobbyLeft + 5, lobbyTop + 45, lobbyWidth - 10, lobbyHeight - 50));
				lobbyPlayersScrollPos = GUILayout.BeginScrollView(lobbyPlayersScrollPos);
				GUILayout.BeginHorizontal();
				GUILayout.Label(GlobalVars.SteamName, GUILayout.Height(20));
				GUILayout.EndHorizontal();
				
				foreach (KeyValuePair<string, MPPlayer> kvp in NetworkManager.Instance.playerGUIDMPPLayerRelation)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label(kvp.Value.playerName, GUILayout.Height(20));
					GUILayout.EndHorizontal();
				}
				GUILayout.EndScrollView();
				GUILayout.EndArea();
			}
		}
	}
	#endregion Unity Methods

	#region RPC Methods
	[RPC]
	public void LoadLevel(int selectedMap, int gameMode)
	{
		//Load The_Fall in Story mode
		if ((GameMode) gameMode == GameMode.Story)
		{
			PluginConsole.WriteLine("Attempting to load story mode", this);
			GameManager.Instance.StartLevel(playableLevelIds[8], GameManager.GameMode.Story);
		}
		//Load the selected map in Speedrun mode
		else
		{
			PluginConsole.WriteLine("Attempting to load map: " + selectedMap + " with gamemode: " + gameMode, this);
			GameManager.Instance.StartLevel(playableLevelIds[selectedMap], GameManager.GameMode.SpeedRun);
		}

		PluginConsole.WriteLine("Map should be loading", this);
	}
	#endregion
}
