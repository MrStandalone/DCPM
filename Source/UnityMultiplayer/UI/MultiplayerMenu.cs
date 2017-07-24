using System;

using DCPM.Common;
using DCPM.PluginBase;

using UnityEngine;

class MultiplayerMenu : MonoBehaviour, IPluginInfo
{
	#region IPluginInfo
	public string Name		{ get { return "DCMP - Multiplayer Main Menu"; } }
	public string Author	{ get { return "Standalone"; } }
	public string Version	{ get { return "0.0.1"; } }
	public string Desc		{ get { return "Multiplayer Main Menu UI"; } }
	#endregion IPluginInfo

	#region Enums
	public enum UserInterface
	{
		None,
		ServerHosting,
		ServerDirectConnect,
		ServerBrowser
	}
	#endregion

	#region Data Members
	/// <summary>
	/// Singleton Property Accessor
	/// </summary>
	public static MultiplayerMenu Instance
	{
		get { return _instance; }
	}
	private static MultiplayerMenu _instance;

	//Main Menu UI Members
	float serverMenuWidth = 160;
	float serverMenuHeight = 200;
	float serverMenuTop = 120;
	float serverMenuLeft;

	/// <summary>
	/// Property accessor for what the current UI to display is
	/// </summary>
	public UserInterface ActiveUserInterface
	{
		get { return _activeUserInterface; }
		set { _activeUserInterface = value; }
	}
	static UserInterface _activeUserInterface = UserInterface.None;
	#endregion

	#region Unity Methods
	void Start()
	{
		ActiveUserInterface = UserInterface.None;

		_instance = this;
	}

	void OnGUI()
	{
		if (GameManager.Instance.CurrentGameState == GameManager.GameState.MainMenu)
		{
			//Main Menu UI Setup
			serverMenuLeft = Screen.width - (serverMenuWidth + 20);

			//Draw the Main Menu on the right hand side of the screen
			GUI.Box(new Rect(serverMenuLeft, serverMenuTop, serverMenuWidth, serverMenuHeight), "Multiplayer Menu");

			if (GUI.Button(new Rect(serverMenuLeft + 10, serverMenuTop + 20, serverMenuWidth - 20, (serverMenuHeight - 30) / 3), "Server Browser"))
			{
				if (ActiveUserInterface == UserInterface.ServerBrowser)
					ActiveUserInterface = UserInterface.None;
				else
					ActiveUserInterface = UserInterface.ServerBrowser;
			}

			if (GUI.Button(new Rect(serverMenuLeft + 10, serverMenuTop + (serverMenuHeight - 30) / 3 + 20, serverMenuWidth - 20, (serverMenuHeight - 30) / 3), "Host Server"))
			{
				if (ActiveUserInterface == UserInterface.ServerHosting)
					ActiveUserInterface = UserInterface.None;
				else
					ActiveUserInterface = UserInterface.ServerHosting;
			}

			if (GUI.Button(new Rect(serverMenuLeft + 10, serverMenuTop + (serverMenuHeight - 30) / 3 + 20 + (serverMenuHeight - 30) / 3, serverMenuWidth - 20, (serverMenuHeight - 30) / 3), "Direct Connect"))
			{
				if (ActiveUserInterface == UserInterface.ServerDirectConnect)
					ActiveUserInterface = UserInterface.None;
				else
					ActiveUserInterface = UserInterface.ServerDirectConnect;
			}
		}
		else
		{
			ActiveUserInterface = UserInterface.None;
		}
	}
	#endregion
}