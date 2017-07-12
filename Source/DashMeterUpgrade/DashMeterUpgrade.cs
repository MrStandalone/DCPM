using System;
using System.Collections.Generic;
using System.Text;
using DCPM.Common;
using DCPM.PluginBase;
using UnityEngine;

public class DashMeterUpgrade : DeadCorePlugin
{
	#region IPluginInfo
	public override string Name		{ get { return "Dash Meter Upgrade"; } }
	public override string Author	{ get { return "Standalone"; } }
	public override string Version	{ get { return "0.1"; } }
	public override string Desc		{ get { return "Changes the dash meter so it shows orange when you can't dash and red when you are being punished"; } }
	#endregion IPluginInfo

	#region DataMembers
	AndroidBattery _battery;
	AndroidDash _dash;

	Color _chargedColor, _waitColor, _punishmentColor, _selectedColor;
	#endregion DataMembers

	#region Console Commands
	void Cmd_SetChargedColor(string[] input)
	{
		if (ResolveColor(input))
		{
			_chargedColor = _selectedColor;

			PluginSettings.Instance.SetSetting("dash_charged_r", _selectedColor.r);
			PluginSettings.Instance.SetSetting("dash_charged_g", _selectedColor.g);
			PluginSettings.Instance.SetSetting("dash_charged_b", _selectedColor.b);

			PluginConsole.WriteLine("Charged Color Changed", this);
		}
	}

	void Cmd_SetWaitColor(string[] input)
	{
		if (ResolveColor(input))
		{
			_waitColor = _selectedColor;

			PluginSettings.Instance.SetSetting("dash_wait_r", _selectedColor.r);
			PluginSettings.Instance.SetSetting("dash_wait_g", _selectedColor.g);
			PluginSettings.Instance.SetSetting("dash_wait_b", _selectedColor.b);

			PluginConsole.WriteLine("Wait Color Changed", this);
		}
	}

	void Cmd_SetPunishmentColor(string[] input)
	{
		if (ResolveColor(input))
		{
			_punishmentColor = _selectedColor;

			PluginSettings.Instance.SetSetting("dash_punish_r", _selectedColor.r);
			PluginSettings.Instance.SetSetting("dash_punish_g", _selectedColor.g);
			PluginSettings.Instance.SetSetting("dash_punish_b", _selectedColor.b);

			PluginConsole.WriteLine("Punishment Color Changed", this);
		}
	}
	#endregion Console Commands

	#region Private Methods
	bool ResolveColor(string[] input)
	{
		bool resolved = true;
		float redValue = 0.0f, greenValue = 0.0f, blueValue = 0.0f;

		if (input.Length >= 1 && !float.TryParse(input[0], out redValue))
		{
			Console.WriteLine("Invalid Red Value, must be between 0.0 and 1.0", this);
			resolved = false;
		}
		else if (input.Length >= 2 && !float.TryParse(input[1], out greenValue))
		{
			Console.WriteLine("Invalid Green Value, must be between 0.0 and 1.0", this);
			resolved = false;
		}
		else if (input.Length >= 3 && !float.TryParse(input[2], out blueValue))
		{
			Console.WriteLine("Invalid Blue Value, must be between 0.0 and 1.0", this);
			resolved = false;
		}

		if (resolved)
		{
			_selectedColor = new Color(redValue, greenValue, blueValue);
		}

		return resolved;
	}
	#endregion Private Methods

	#region Unity Methods
	private void Awake()
	{
		enabled = false;

		//Register Console Commands
		PluginConsole.RegisterConsoleCommand("set_dash_charged_color", Cmd_SetChargedColor, "Set the Charged color of the Dash interface on the gun, eg: 'set_dash_charged_color 1 0 0'", this);
		PluginConsole.RegisterConsoleCommand("set_dash_wait_color", Cmd_SetWaitColor, "Set the Waiting color of the Dash interface on the gun", this);
		PluginConsole.RegisterConsoleCommand("set_dash_punish_color", Cmd_SetPunishmentColor, "Set the Punishment color of the Dash interface on the gun", this);

		float cR, cG, cB;
		float wR, wG, wB;
		float pR, pG, pB;

		//Check Settings file for Colors
		cR = float.Parse(PluginSettings.Instance.GetSetting("dash_charged_r", 0f));
		cG = float.Parse(PluginSettings.Instance.GetSetting("dash_charged_g", 1f));
		cB = float.Parse(PluginSettings.Instance.GetSetting("dash_charged_b", 0f));

		wR = float.Parse(PluginSettings.Instance.GetSetting("dash_wait_r", 0f));
		wG = float.Parse(PluginSettings.Instance.GetSetting("dash_wait_g", 0.5f));
		wB = float.Parse(PluginSettings.Instance.GetSetting("dash_wait_b", 1f));

		pR = float.Parse(PluginSettings.Instance.GetSetting("dash_punish_r", 1f));
		pG = float.Parse(PluginSettings.Instance.GetSetting("dash_punish_g", 0f));
		pB = float.Parse(PluginSettings.Instance.GetSetting("dash_punish_b", 0f));

		//Assign colors to persistent colors
		_chargedColor = new Color(cR, cG, cB);
		_waitColor = new Color(wR, wG, wB);
		_punishmentColor = new Color(pR, pG, pB);
	}

	private void OnLevelWasLoaded(int level)
	{
		if (DeadCoreLevels.PlayableLevelIDs.ContainsKey(level))
		{
			_battery = Android.Instance.GetComponent<AndroidBattery>();
			_dash = Android.Instance.GetComponent<AndroidDash>();
			enabled = true;
		}
		else
		{
			enabled = false;
		}
	}

	private void LateUpdate()
	{
		if (_battery.CurrentBattery < _dash._minBatteryToDash && _battery.CanUseBattery)
		{
			_battery._gaugeRenderer.material.color = _waitColor;
		}
		else if (_battery.CanUseBattery)
		{
			_battery._gaugeRenderer.material.color = _chargedColor;
		}
		else if (!_battery.CanUseBattery)
		{
			_battery._gaugeRenderer.material.color = _punishmentColor;
		}
	}
	#endregion Unity Methods
}
