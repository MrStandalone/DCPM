using System;

using DCPM.Common;
using DCPM.PluginBase;

using UnityEngine;


class TransformTracking : MonoBehaviour, IPluginInfo
{
	#region IPluginInfo
	public string Name		{ get { return "DCMP - Transform Tracker " + Network.player.guid; } }
	public string Author	{ get { return "Standalone"; } }
	public string Version	{ get { return ""; } }
	public string Desc		{ get { return ""; } }
	#endregion IPluginInfo

	#region Data Members
	//Player must move/rotate more than this in order for it to be transmitted over the network
	float positionThreshold = 0.2f;
	float rotationThreshold = 0.2f;

	Vector3 lastTransmittedPosition = new Vector3();
	Quaternion lastTransmittedRotation = new Quaternion();

	Transform playerTransform
	{
		get { return Android.Instance.gameObject.transform; }
	}
	#endregion

	#region Unity Methods
	void OnEnable()
	{
		PluginConsole.WriteLine("Enabled", this);
	}

	void OnDisable()
	{
		PluginConsole.WriteLine("Disabled", this);
	}

	//Should be transmitted at 50hz?
	void FixedUpdate()
	{
		/// <summary>
		/// Method that checks to see if a player has moved/rotated > some threshold
		/// </summary>
		if (Vector3.Distance(playerTransform.position, lastTransmittedPosition) > positionThreshold)
		{
			//Are we connected to an MP game?
			if (Network.isServer)
			{
				//Inform all clients of your movement
				PluginConsole.WriteLine("Informing clients of movement.", this);
				networkView.RPC("UpdatePlayerPosition", RPCMode.Others, Network.player.guid, playerTransform.position);
				lastTransmittedPosition = playerTransform.position;
			}
			else if (Network.isClient)
			{
				//Inform all clients of your movement
				PluginConsole.WriteLine("Informing other clients of movement.", this);
				networkView.RPC("UpdatePlayerPosition", RPCMode.Others, Network.player.guid, playerTransform.position);
				lastTransmittedPosition = playerTransform.position;
			}
		}
		/*
		//Check to see if the player has rotated past a certain threshold since the last transmission
		if (Quaternion.Angle(playerTransform.rotation, lastTransmittedRotation) > rotationThreshold)
		{
			if (Network.isServer)
			{
				//Inform all clients of your rotation
				networkView.RPC("InformClientsOfRotation", RPCMode.Others, playerTransform.rotation, Network.player);
				lastTransmittedRotation = playerTransform.rotation;
			} 
			else if (Network.isClient)
			{
				//Inform the server of your rotation
				networkView.RPC("InformServerOfRotation", RPCMode.Server, playerTransform.rotation);
				lastTransmittedRotation = playerTransform.rotation;
			}
		}
		*/
	}
	#endregion

	#region RPC's
	[RPC]
	public void InformServerOfMovement(string senderGUID, Vector3 newPos)
	{
		//Ensure we are the server
		if (Network.isServer)
		{
			PluginConsole.WriteLine("Player '" + senderGUID + "' has informed server of movement, relaying to clients");
			networkView.RPC("UpdatePlayerPosition", RPCMode.All, senderGUID, newPos);
		}
	}
	#endregion
}
