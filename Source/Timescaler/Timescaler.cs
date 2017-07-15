using System;
using System.Collections.Generic;
using System.Text;
using DCPM.Common;
using DCPM.PluginBase;
using UnityEngine;

public class Timescaler : DeadCorePlugin
{
    #region IPluginInfo
    public override string Name { get { return "Timescaler"; } }
    public override string Author { get { return "HaselLoyance"; } }
    public override string Version { get { return "1.0"; } }
    public override string Desc { get { return "Allows to modify Unity Time.timeScale"; } }
    #endregion IPluginInfo
    
    #region Console Commands
    void Cmd_Timescale(string[] input)
    {
        float timescaleValue = 1.0f;

        if (input.Length == 0)
        {
            PluginConsole.WriteLine(string.Format("Timescale: {0}", Time.timeScale), this);
        }
        else if (input.Length != 1)
        {
            PluginConsole.WriteLine("Command requires 1 timescale value eg: 'timescale 2'", this);
        }
        else if (float.TryParse(input[0], out timescaleValue))
        {
            Time.timeScale = timescaleValue;
            PluginConsole.WriteLine(string.Format("New timescale: {0}", Time.timeScale), this);
        }
        else
        {
            PluginConsole.WriteLine("A conversion error occurred during this command and as such the timescale value was not changed", this);
        }
    }
    #endregion Console Commands

    #region Unity Methods
    private void Awake()
    {
        PluginConsole.RegisterConsoleCommand("timescale", Cmd_Timescale, "Sets Unity Time.timeScale", this);
    }
    #endregion Unity Methods
}
