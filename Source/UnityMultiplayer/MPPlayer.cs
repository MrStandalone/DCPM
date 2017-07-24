using System;

using DCPM.Common;
using DCPM.PluginBase;

using UnityEngine;
using System.Collections.Generic;

class MPPlayer : MonoBehaviour, IPluginInfo
{
	#region IPluginInfo
	public string Name		{ get { return "DCMP - Player Script " + playerGUID; } }
	public string Author	{ get { return "Standalone"; } }
	public string Version	{ get { return "0.0.1"; } }
	public string Desc		{ get { return "Multiplayer Lobby UI & Logic"; } }
	#endregion IPluginInfo


	#region Data Members
	//The multiplayer name of this player
	public string playerName;
	Vector3 newPos;

	public string playerGUID
	{
		get { return _playerGUID; }
		set
		{
			//PluginConsole.WriteLine("Player Script " + _instanceId + " has been assigned GUID '" + value + "'", this);
			_playerGUID = value;
		}
	}
	private string _playerGUID;

	private static int _numInstances = 0;
	private int _instanceId;
	public NetworkViewID ownerViewID;
	private GameObject _playerCapsule;
	private GameObject _floatingText;
	Vector3 textOffset = Vector3.up * 1.5f;
	#endregion
		

	#region Unity Methods
	void Start()
	{
		_numInstances++;
		_instanceId = _numInstances;

		PluginConsole.WriteLine("Player Instantiated", this);
	}

	void OnDestroy()
	{
		PluginConsole.WriteLine("Player '" + playerName + "' Destroyed", this);
		Destroy(_playerCapsule);
		_numInstances--;
	}

	private void OnLevelWasLoaded(int level)
	{
		//If we are loading a playable level, i.e Not the main menu/loading screen/credits
		if (DeadCoreLevels.PlayableLevelIDs[level] != null)
		{
			//TODO - could also destroy and recreate the player capsule here, but if this works then not sure if needed
			_playerCapsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);

			//Should hopefully make the other player capsules not solid
			//_playerCapsule.rigidbody.isKinematic = true;
			_playerCapsule.collider.isTrigger = true;

			_floatingText = new GameObject();
			_floatingText.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			TrackingText namePlate = _floatingText.AddComponent<TrackingText>();
			namePlate.textMesh.text = playerName;
			namePlate.textMesh.anchor = TextAnchor.MiddleCenter;
		}
	}

	Vector3 velocity = Vector3.zero;
	float smoothAmount = 0.02f;
	private void Update()
	{
		if (newPos != null)
			{
				_playerCapsule.transform.position = Vector3.SmoothDamp(_playerCapsule.transform.position, newPos, ref velocity, smoothAmount);
				//_playerCapsule.transform.position = Vector3.Lerp(_playerCapsule.transform.position, newPos, Time.deltaTime * 45);

				_floatingText.transform.position = _playerCapsule.transform.position + textOffset;
				_floatingText.transform.LookAt(Android.Instance.gameObject.transform.position + textOffset, _playerCapsule.transform.up);
				_floatingText.transform.rotation *= Quaternion.Euler(180, 0, 180);
			}
	}
	#endregion

	#region Public Methods
	public void UpdateNetworkPlayerPosition(Vector3 newPos)
	{
		PluginConsole.WriteLine("Updating position", this);
		//TODO - Smooth this movement out over time
		this.newPos = newPos;
		//_playerCapsule.transform.position = newPos;
	}
	#endregion

	#region RPC's
	
	#endregion

	class TrackingText : MonoBehaviour
	{
		private TextMesh tMesh;
		public TextMesh textMesh { get { return tMesh; } }

		void Awake()
		{
			tMesh = gameObject.AddComponent<TextMesh>();
			tMesh.font = GlobalVars.UnityArialFont;
			tMesh.fontSize = 50;
			tMesh.GetComponent<Renderer>().material = tMesh.font.material;
			tMesh.alignment = TextAlignment.Center;
		}
	}
}

