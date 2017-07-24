using System;

using DCPM.Common;
using DCPM.PluginBase;

using UnityEngine;

using UnityMultiplayer.UI;
using System.Collections.Generic;

class NetworkManager : DeadCorePlugin
{
	#region IPluginInfo
	public override string Name { get { return "DCMP - Network Manager"; } }
	public override string Author { get { return "Standalone"; } }
	public override string Version { get { return "0.0.1"; } }
	public override string Desc { get { return "Simple DeadCore Multiplayer Experiment"; } }
	#endregion IPluginInfo

	#region Enums

	#endregion

	#region Data Members
	/// <summary>
	/// Singleton Property Accessor
	/// </summary>
	public static NetworkManager Instance
	{
		get
		{
			return _instance;
		}
	}
	private static NetworkManager _instance;

	TransformTracking transformTracker;

	public const string GameModeName = "DeadCoreMultiplayer-v0.1";
	private MultiplayerLobby mpLobby;
	private MultiplayerChat mpChat;

	public Dictionary<string, MPPlayer> playerGUIDMPPLayerRelation; 
	public Dictionary<NetworkPlayer, NetworkViewID> networkPlayerNetworkViewIDRelation;
	public Dictionary<NetworkViewID, NetworkPlayer> networkViewIDNetworkPlayerRelation;
	public Dictionary<NetworkViewID, MPPlayer> networkViewIDMPPLayerRelation;
	#endregion

	#region Console Commands
	public void Cmd_Test(string[] input)
	{
			
	}
	#endregion

	#region Custom UI Methods

	#endregion

	#region Unity Methods
	private void Start()
	{
		playerGUIDMPPLayerRelation = new Dictionary<string, MPPlayer>();
		networkPlayerNetworkViewIDRelation = new Dictionary<NetworkPlayer, NetworkViewID>();
		networkViewIDMPPLayerRelation = new Dictionary<NetworkViewID, MPPlayer>();
		networkViewIDNetworkPlayerRelation = new Dictionary<NetworkViewID, NetworkPlayer>();

		gameObject.AddComponent<MultiplayerMenu>();
		gameObject.AddComponent<ServerBrowser>();
		gameObject.AddComponent<ServerDirectConnect>();
		gameObject.AddComponent<ServerHosting>();

		gameObject.AddComponent<NetworkView>();
		transformTracker = gameObject.AddComponent<TransformTracking>();
		transformTracker.enabled = false;

		GameManager.Instance.LevelFinished += Instance_LevelFinished;

		_instance = this;
	}

	private void Instance_LevelFinished()
	{
		//TODO - figure out what we want to do for when levels end
		PluginConsole.WriteLine("Level finished, need to do some shit here", this);
		//throw new NotImplementedException();
	}

	private void OnLevelWasLoaded(int level)
	{
		//If we are loading a playable level, i.e Not the main menu/loading screen/credits
		if (DeadCoreLevels.PlayableLevelIDs[level] != null)
		{
			transformTracker.enabled = true;
		}
		else
		{
			transformTracker.enabled = false;
		}
	}
	#endregion

	#region Unity Network Client Methods
	/// <summary>
	/// Unity method called when a client fails to connect to a server for some reason
	/// </summary>
	/// <param name="error"></param>
	void OnFailedToConnect(NetworkConnectionError error)
	{
		PluginConsole.WriteLine("Could not connect to server: " + error.ToString(), this);
	}

	/// <summary>
	/// Method called on the client whenever they connect to a server
	/// </summary>
	void OnConnectedToServer()
	{
		//Inform other clients that we have connected and that they should create an MPPlayer script for us
		//We're sending this from client to client so it is stored as being sent from this player for when they disconnect
		//Rather than send it to server and have the server inform the clients, when the player disconnects the buffered RPC
		//would be made from the server and not be removed
		//networkView.viewID = Network.AllocateViewID();
		PluginConsole.WriteLine("Connected to Server, local GUID: '" + Network.player.guid + "'");

		networkView.RPC("PlayerConnected", RPCMode.OthersBuffered, Network.player.guid, GlobalVars.SteamName);
		//networkView.RPC("PlayerConnected", RPCMode.OthersBuffered, GlobalVars.SteamName);

		//Instantiate a MultiplayerLobby and MultiplayerChat object
		mpLobby = gameObject.AddComponent<MultiplayerLobby>();
		mpChat = gameObject.AddComponent<MultiplayerChat>();
	}

	/// <summary>
	/// Method called on the client when disconnected from a server
	/// </summary>
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		if (Network.isServer)
		{
			PluginConsole.WriteLine("Local server shut down", this);
		}
		else
		{
			if (info == NetworkDisconnection.LostConnection)
			{
				PluginConsole.WriteLine("Lost connection to server", this);
			}
			else
			{
				PluginConsole.WriteLine("Disconnected from server", this);
			}
		}

		//CleanupObjects();

		//Destroy the Lobby and Chat objects
		Destroy(mpLobby);
		Destroy(mpChat);


		//TODO - Cleanup, remove player objects, stop trying to communicate with server etc etc
		//TODO - Cleanup the MultiplayerLobby and MultiplayerChat object

		networkViewIDNetworkPlayerRelation.Clear();
		networkViewIDMPPLayerRelation.Clear();

		//_networkPlayerNameRelations.Clear();
		//_networkPlayerObjectRelations.Clear();
	}
	#endregion

	#region Unity Network Server Methods
	/// <summary>
	/// Method called on the server when a player disconnects
	/// </summary>
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		//Tell clients and server that this player has disconnected
		PluginConsole.WriteLine("Player " + player.guid + " has disconnected, informing clients", this);
		networkView.RPC("PlayerDisconnected", RPCMode.All, player);
	}

	/// <summary>
	/// Method called on the server when a player connects
	/// </summary>
	/*
	void OnPlayerConnected(NetworkPlayer player)
	{
		networkView.RPC("PlayerConnected", RPCMode.AllBuffered, )
	}
	*/

	void OnServerInitialized()
	{
		//networkView.viewID = Network.AllocateViewID();
		PluginConsole.WriteLine("Server Initialized, local GUID '" + Network.player.guid + "'", this);

		networkView.RPC("PlayerConnected", RPCMode.OthersBuffered, Network.player.guid, GlobalVars.SteamName);

		//Add yourself to the player list
		//_networkPlayerNameRelations.Add(Network.player, GlobalVars.SteamName);

		//Instantiate a MultiplayerLobby and MultiplayerChat object
		mpLobby = gameObject.AddComponent<MultiplayerLobby>();
		mpChat = gameObject.AddComponent<MultiplayerChat>();
	}
	#endregion

	#region RPC Methods
	/// <summary>
	/// Networked RPC method called when a player connects
	/// The source of this call will be the server itself
	/// </summary>
	[RPC]
	public void PlayerConnected(string senderGUID, string playerName, NetworkMessageInfo info)
	{
		PluginConsole.WriteLine("Player '" + playerName + "' connected with GUID '" + senderGUID + "'", this);
		//TODO - Instantiate a client version of the MPPlayer script with the viewId and playerName

		MPPlayer player = gameObject.AddComponent<MPPlayer>();
		player.playerGUID = senderGUID;
		player.playerName = playerName;

		playerGUIDMPPLayerRelation.Add(senderGUID, player);
		//networkViewIDMPPLayerRelation.Add(viewID, player);
		//networkPlayerNetworkViewIDRelation.Add(info.sender, viewID);

		//MPPlayer player = gameObject.AddComponent<MPPlayer>();
		//player.playerName = playerName;
		//player.ownerViewID = viewID;

		//_networkPlayerObjectRelations.Add(viewID, player);
		//Possibly not needed anymore if we simply have a null reference in the MPPlayer spot for the local player
		//_networkPlayerNameRelations.Add(info.sender, playerName);
	}

	/// <summary>
	/// Networked RPC method called by the server to clients when a player disconnects
	/// 
	/// Used to clean up disconnected player on the clients
	/// </summary>
	/// <param name="player"></param>
	[RPC]
	public void PlayerDisconnected(NetworkPlayer player)
	{
		PluginConsole.WriteLine("Server has informed us that '" + player.guid + "' has disconnected", this);

		RemovePlayer(player);
	}

	[RPC]
	public void UpdatePlayerPosition(string playerGUID, Vector3 newPos)
	{
		//PluginConsole.WriteLine("Call was made to update position by '" + playerGUID + "'", this);

		if (playerGUIDMPPLayerRelation[playerGUID] != null)
		{
			playerGUIDMPPLayerRelation[playerGUID].UpdateNetworkPlayerPosition(newPos);
		}
		else
		{
			PluginConsole.WriteLine("For some reason that player does not exist", this);
		}
		
	}
	#endregion

	#region Private Methods
	void RemovePlayer(NetworkPlayer player)
	{
		PluginConsole.WriteLine("Removing objects and RPC's owned by '" + player.guid + "'", this);

		playerGUIDMPPLayerRelation.Remove(player.guid);
		//_networkPlayerNameRelations.Remove(player);
		//Destroy(_networkPlayerObjectRelations[player]);
		//_networkPlayerObjectRelations.Remove(player);
		Network.RemoveRPCs(player);
		//TODO - Delete the corresponding player object
	}
	#endregion
}
