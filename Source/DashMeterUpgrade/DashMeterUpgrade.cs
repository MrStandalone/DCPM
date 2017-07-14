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
	AndroidBattery _battery
	{
		get
		{
			return Android.Instance.GetComponent<AndroidBattery>();
		}
	}
	AndroidDash _dash
	{
		get
		{
			return Android.Instance.GetComponent<AndroidDash>();
		}
	}

	Color _chargedColor, _waitColor, _punishColor;
	#endregion DataMembers

	#region Console Commands
	void Cmd_SetChargedColor(string[] input)
	{
		//No args supplied, display current value
		if (input.Length == 0)
		{
			PluginConsole.WriteLine(string.Format("Charged color = R:{0} G:{1} B:{2}", _chargedColor.r, _chargedColor.g, _chargedColor.b), this);
		}
		//If string array is not a 3 element array
		else if (input.Length != 3)
		{
			PluginConsole.WriteLine("Command requires 3 color values separated by spaces eg: 'dash_charged_color 1 0 0' will set the color to red", this);
		}
		//String array is in fact a 3 element array
		else
		{
			var colorResult = StringArrayToColor(input);

			_chargedColor = colorResult.color;

			if (colorResult.HadConversionError)
			{
				PluginConsole.WriteLine("A conversion error occurred during this command and as such the color value was not assigned to the settings file", this);
			}
			else
			{
				PluginSettings.Instance.SetSetting("dash_charged_color", string.Join(" ", input));
			}
		}
	}

	void Cmd_SetWaitColor(string[] input)
	{
		//No args supplied, display current value
		if (input.Length == 0)
		{
			PluginConsole.WriteLine(string.Format("Wait color = R:{0} G:{1} B:{2}", _waitColor.r, _waitColor.g, _waitColor.b), this);
		}
		//If string array is not a 3 element array
		else if (input.Length != 3)
		{
			PluginConsole.WriteLine("Command requires 3 color values separated by spaces eg: 'dash_wait_color 1 0 0' will set the color to red", this);
		}
		//String array is in fact a 3 element array
		else
		{
			var colorResult = StringArrayToColor(input);

			_waitColor = colorResult.color;

			if (colorResult.HadConversionError)
			{
				PluginConsole.WriteLine("A conversion error occurred during this command and as such the color value was not assigned to the settings file", this);
			}
			else
			{
				PluginSettings.Instance.SetSetting("dash_wait_color", string.Join(" ", input));
			}
		}
	}

	void Cmd_SetPunishmentColor(string[] input)
	{
		//No args supplied, display current value
		if (input.Length == 0)
		{
			PluginConsole.WriteLine(string.Format("Punish color = R:{0} G:{1} B:{2}", _punishColor.r, _punishColor.g, _punishColor.b), this);
		}
		//If string array is not a 3 element array
		else if (input.Length != 3)
		{
			PluginConsole.WriteLine("Command requires 3 color values separated by spaces eg: 'dash_punish_color 1 0 0' will set the color to red", this);
		}
		//String array is in fact a 3 element array
		else
		{
			var colorResult = StringArrayToColor(input);

			_punishColor = colorResult.color;

			if (colorResult.HadConversionError)
			{
				PluginConsole.WriteLine("A conversion error occurred during this command and as such the color value was not assigned to the settings file", this);
			}
			else
			{
				PluginSettings.Instance.SetSetting("dash_punish_color", string.Join(" ", input));
			}
		}
	}
	#endregion Console Commands

	#region Private Methods
	private class MyColor
	{
		public Color color { get; set; }
		public bool HadConversionError { get; set; }

		public MyColor()
		{
			HadConversionError = false;
		}
	}


	//Convert a 3 element string array into a color, if not a 3 element array supplied then use default black as color
	private MyColor StringArrayToColor(string[] input)
	{
		MyColor result = new MyColor();
		float[] colorValues = new float[3] { 0, 0, 0 };

		if (input.Length < 3)
		{
			PluginConsole.WriteLine("Supplied string color array must be at least 3 elements in length, using '0, 0, 0' as default", this);
		}
		else
		{
			float value;

			for (int i = 0; i < 3; i++)
			{
				if (float.TryParse(input[i], out value))
				{
					colorValues[i] = value;
				}
				else
				{
					result.HadConversionError = true;
					PluginConsole.WriteLine("Error converting '" + input[i] + "' into a float value, using default value of '0' instead", this);
				}
			}
		}

		result.color = new Color(colorValues[0], colorValues[1], colorValues[2]);

		return result;
	}
	#endregion Private Methods

	#region Unity Methods
	private void Awake()
	{
		enabled = false;

		//Register Console Commands
		PluginConsole.RegisterConsoleCommand("dash_charged_color", Cmd_SetChargedColor, "Set the Charged color of the Dash interface on the gun, used with no arguments will show the current color", this);
		PluginConsole.RegisterConsoleCommand("dash_wait_color", Cmd_SetWaitColor, "Set the Waiting color of the Dash interface on the gun, used with no arguments will show the current color", this);
		PluginConsole.RegisterConsoleCommand("dash_punish_color", Cmd_SetPunishmentColor, "Set the Punishment color of the Dash interface on the gun, used with no arguments will show the current color", this);

		//Check Settings file for Colors
		_chargedColor = StringArrayToColor(PluginSettings.Instance.GetSetting("dash_charged_color", "0 1 0").Split(' ')).color;
		_waitColor = StringArrayToColor(PluginSettings.Instance.GetSetting("dash_wait_color", "0 0.5 1").Split(' ')).color;
		_punishColor = StringArrayToColor(PluginSettings.Instance.GetSetting("dash_punish_color", "1 0 0").Split(' ')).color;
	}

	private void OnLevelWasLoaded(int level)
	{
		//Only enable this plugin when we are in a playable level
		if (DeadCoreLevels.PlayableLevelIDs.ContainsKey(level))
		{
			enabled = true;
		}
		else
		{
			enabled = false;
		}
	}

	private void LateUpdate()
	{
		//If we have a battery object then we will also have a dash object
		if (_battery != null)
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
				_battery._gaugeRenderer.material.color = _punishColor;
			}
		}
	}
	#endregion Unity Methods
}
