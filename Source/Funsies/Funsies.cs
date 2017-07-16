using DCPM.Common;
using DCPM.PluginBase;
using UnityEngine;

public class Funsies : DeadCorePlugin
{
    #region IPluginInfo
    public override string Name { get { return "Funsies"; } }
    public override string Author { get { return "HaselLoyance"; } }
    public override string Version { get { return "1.0"; } }
    public override string Desc { get { return "Commands for having fun"; } }
    #endregion IPluginInfo

    #region Data Members
    bool enableCheckpoints = true;
    Vector3 cameraScale = new Vector3(1,1,1);
    Camera cam = null;
    #endregion Data Members

    #region Console Commands

    //Timescale
    void Cmd_CameraScale(string[] input)
    {

        if (input.Length == 0)
        {
            PluginConsole.WriteLine(string.Format("Camera scale: {0}, Default: 1 1 1", cameraScale.ToString()), this);
            return;
        }

        if (input.Length != 3)
        {
            PluginConsole.WriteLine("Command requires 1 scale values eg: 'camera_scale 1 1 1'", this);
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            float scaleValue = 1.0f;

            if (!float.TryParse(input[i], out scaleValue))
            {
                PluginConsole.WriteLine("A conversion error occurred during this command and as such the camera scale values were not changed", this);
                return;
            }

            cameraScale[i] = scaleValue;
        }

        ResetCamera();
        PluginConsole.WriteLine(string.Format("New camera scale: {0}", cameraScale.ToString()), this);
    }

    //Toggle checkpoints
    void Cmd_ToggleCheckpoints(string[] input)
    {
        if (input.Length != 0)
        {
            PluginConsole.WriteLine("toggle_checkpoints does not take any arguments", this);
            return;
        }

        enableCheckpoints = !enableCheckpoints;

        PluginConsole.WriteLine("Checkpoints " + (enableCheckpoints ? "enabled" : "disabled"), this);

        ToggleCheckpoints();
    }
    #endregion Console Commands

    #region Private Methods
    void ResetCamera()
    {
        if (cam == null)
        {
            return;
        }

        cam.projectionMatrix = cam.projectionMatrix * Matrix4x4.Scale(cameraScale);
    }

    void ToggleCheckpoints()
    {
        foreach (CheckPointScript cp in FindObjectsOfType<CheckPointScript>())
        {
            PluginConsole.WriteLine(cp.name + "   " + cp._id, this);
            if (cp.name != "Checkpoint_0" && cp.name != "Game-object teleporter01" 
                && cp.name != "Checkpoint_00" && !(cp.name == "_Checkpoint_Start" && cp._id == 0)
                && cp.name != "_Checkpoint_0" && cp.name != "Checkpoint_Start"
                && cp.name != "__Checkpoint_START" && cp.name != "__Checkpoint_Start")
            {
                cp.active = enableCheckpoints;
            }
        }
    }
    #endregion Private Methods

    #region Unity Methods
    void OnLevelWasLoaded(int level)
    {
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.InGame)
        {
            return;
        }
        
        cam = FindObjectsOfType<Camera>()[1];


        ResetCamera();
        ToggleCheckpoints();
    }

    void Awake()
    {
        PluginConsole.RegisterConsoleCommand("camera_scale", Cmd_CameraScale, "Applis matrix scale to camera", this);
        PluginConsole.RegisterConsoleCommand("toggle_checkpoints", Cmd_ToggleCheckpoints, "Enables/disables checkpoints", this);
    }

    void Update()
    {
    }
    #endregion Unity Methods
}
