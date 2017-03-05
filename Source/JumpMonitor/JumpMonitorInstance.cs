using System;
using System.Collections.Generic;

using UnityEngine;
using DCPM.Common;
using DCPM.PluginBase;

/// <summary>
/// 
/// </summary>
public class JumpMonitorInstance : DeadCorePlugin
{
	#region IPluginInfo
	public override string Name { get { return "Super Jump Monitor"; } }
	public override string Author { get { return "Standalone"; } }
	public override string Version { get { return "0.1"; } }
	public override string Desc { get { return "Attempts to monitor how close you were to making a super jump"; } }
	#endregion IPluginInfo

	#region Data Members
	float lastJumpPadTime = 0.0f;
	float lastJumpRequestTime = 0.0f;
	float lastUIDisplayTime = 0.0f;
	string monitor, labelText = "Herpa Derp Derp";
	Color labelBGColor = Color.cyan;
	#endregion Data Members

	#region Console Commands
	//Enable UI
	//Enable/Disable
	#endregion Console Commands

	#region Unity Methods
	/// <summary>
	/// On Script initialization hook into the static event of the Jump Pads
	/// </summary>
	void Awake()
	{
		monitor =  PluginSettings.Instance.GetSetting("sjm_enabled", "1");
		JumperAttachment.PlayerUsedJumpPad += PlayerUsedJumpPad;
	}

	/// <summary>
	/// On Level load attach the JumperAttachment script to all of the Jump Pads in a level
	/// Hook into the Player Jumped event
	/// </summary>
	void OnLevelWasLoaded(int level)
	{
		if (GameManager.Instance.CurrentGameState == GameManager.GameState.InGame && monitor == "1")
		{
			PluginConsole.WriteLine("Attaching Monitor to Jumpers", this);
			foreach (Jumper jumper in GameObject.FindObjectsOfType<Jumper>())
            {
				jumper.gameObject.AddComponent<JumperAttachment>();
				PluginConsole.WriteLine("Monitor attached to " + jumper.name, this);
			}
		}
		
		Android.Instance.Jumped += Player_Jumped;
	}

	void Update()
	{
		if (InputManager.Instance.GetButtonDown("Jump") && monitor == "1")
		{
			PluginConsole.WriteLine("Player Requested Jump @ " + Time.time, this);
			lastJumpRequestTime = Time.time;

			if (Time.time - lastJumpPadTime < Time.fixedDeltaTime)
			{
				PluginConsole.WriteLine("Super Jump! - Jump was requested in the fixed physics time step", this);
				labelBGColor = Color.green;
				labelText = "Super Jump!";
				lastUIDisplayTime = Time.time;
			}
			else if (Time.time - lastJumpPadTime < 0.5f)
			{
				PluginConsole.WriteLine("Too Late! - Jump was requested " + (lastJumpRequestTime - (lastJumpPadTime + Time.fixedDeltaTime)) + "s too late", this);
				labelBGColor = Color.red;
				labelText = "Too Late! - " + (lastJumpRequestTime - (lastJumpPadTime + Time.fixedDeltaTime)).ToString("N3") + "s too late";
				lastUIDisplayTime = Time.time;
			}
		}
	}

	void OnGUI()
	{

		//AndroidJumper jumper = Android.Instance.gameObject.GetComponent<AndroidJumper>();
		//jumper._canDoubleJump;
		if (GameManager.Instance.CurrentGameState == GameManager.GameState.InGame)
		{
			if (Time.time - lastUIDisplayTime < 2.0f)
			{
				GUI.backgroundColor = labelBGColor;
				GUI.Button(new Rect(Screen.width / 2 - 250 / 2, Screen.height / 2 - 100, 250, 50), labelText);
			}
		}
	}
	#endregion Unity Methods

	#region Events
	void PlayerUsedJumpPad(object sender, JumperAttachment.PlayerUsedJumpPadEventArgs args)
	{
		if (monitor == "1")
		{
			PluginConsole.WriteLine("Player Used Jump Pad @ " + args.timeUsed, this);
			lastJumpPadTime = args.timeUsed;

			if (args.timeUsed - lastJumpRequestTime < 0.5f)
			{
				PluginConsole.WriteLine("Too Early! - Jump was requested " + (args.timeUsed - lastJumpRequestTime) + "s too early", this);
				labelBGColor = Color.red;
				labelText = "Too Early! - " + (args.timeUsed - lastJumpRequestTime).ToString("N3") + "s too early";
				lastUIDisplayTime = Time.time;
			}
		}
	}

	private void Player_Jumped(int obj)
	{
		if (monitor == "1")
		{
			PluginConsole.WriteLine("Player Jump Called @ " + Time.time, this);
		}
		
	}
# endregion Events
}

