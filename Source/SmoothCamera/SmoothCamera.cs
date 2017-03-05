using System;
using System.Collections.Generic;

using UnityEngine;
using DCPM.Common;
using DCPM.PluginBase;

public class SmoothCamera : DeadCorePlugin
{
	#region IPluginInfo
	public override string Name { get { return "Smooth Camera"; } }
	public override string Author { get { return "Standalone"; } }
	public override string Version { get { return "1.0"; } }
	public override string Desc { get { return "Attempts to fix the micro stuttering that plagues the game"; } }
	#endregion

	#region Data Members
	bool smooth = true;

	float smoothAmount = 0.015f;

	GameObject smoothMovementGO;
	#endregion

	#region Private Methods
	void EnableSmoothing()
	{
		smoothMovementGO = new GameObject();
		PluginConsole.WriteLine("Smooth Camera Enabled", this);
		PluginSettings.Instance.SetSetting("SmoothCameraToggle", smooth);
	}

	void DisableSmoothing()
	{
		Destroy(smoothMovementGO);
		PluginConsole.WriteLine("Smooth Camera Disabled", this);
		PluginSettings.Instance.SetSetting("SmoothCameraToggle", smooth);
	}
	#endregion

	#region Console Commands
	void Cmd_Toggle(string[] input)
	{
		smooth = !smooth;

		if (smooth)
			EnableSmoothing();
		else
			DisableSmoothing();
	}

	void Cmd_SmoothFactor(string[] input)
	{
		float value;
		if (input.Length >= 1 && float.TryParse(input[0], out value))
		{
			smoothAmount = value;
			PluginConsole.WriteLine("Smooth amount changed to " + smoothAmount, this);
			PluginSettings.Instance.SetSetting("SmoothCameraAmount", smoothAmount);
		}
		else
		{
			PluginConsole.WriteLine("Smooth amount: " + smoothAmount, this);
		}
	}
	#endregion

	#region Unity Methods
	void Awake()
	{
		PluginConsole.RegisterConsoleCommand("sc_toggle", Cmd_Toggle, "Toggles smooth camera movement", this);
		PluginConsole.RegisterConsoleCommand("sc_smooth_amount", Cmd_SmoothFactor, "Set the camera smooth factor", this);

		if (bool.TryParse(PluginSettings.Instance.GetSetting("SmoothCameraToggle", smooth), out smooth))
			PluginConsole.WriteLine("Smooth toggle setting loaded, smoothing enabled = " + smooth, this);
		else
		{
			PluginConsole.WriteLine("Could not load smooth toggle setting from file, using default: true");
		}

		if (float.TryParse(PluginSettings.Instance.GetSetting("SmoothCameraAmount", smoothAmount), out smoothAmount))
			PluginConsole.WriteLine("Smooth amount setting loaded, smooth amount = " + smoothAmount, this);
		else
		{
			PluginConsole.WriteLine("Could not load smooth amount setting from file, using default: 0.015");
		}
	}

	void OnLevelWasLoaded(int level)
	{
		if (smooth && level != (int) DeadCoreLevels.Levels.Main_Menu && level!= (int) DeadCoreLevels.Levels.Loading_Screen && level != (int) DeadCoreLevels.Levels.SplashScreen)
			EnableSmoothing();
	}

	void Update()
	{
		if (smooth && GameManager.Instance.CurrentGameState == GameManager.GameState.InGame)
		{
			//Update the mouse rotation per frame
			smoothMovementGO.transform.rotation = Camera.main.transform.parent.rotation;
		}
	}

	Vector3 velocity = Vector3.zero;
	void LateUpdate()
	{
		if (smooth && GameManager.Instance.CurrentGameState == GameManager.GameState.InGame)
		{
			Vector3 targetPosition = Camera.main.transform.parent.position;
			smoothMovementGO.transform.position = Vector3.SmoothDamp(smoothMovementGO.transform.position, targetPosition, ref velocity, smoothAmount);
			Camera.main.transform.position = smoothMovementGO.transform.position;
		}
	}
	#endregion
}

