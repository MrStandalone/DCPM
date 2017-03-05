using System;
using System.Collections.Generic;
using System.Text;

using DCPM.PluginBase;
using UnityEngine;
using System.IO;

namespace DCPM.Common
{
	//This class implements the IPluginInfo however it is not like other plugins in that it is loaded before any other plugin is loaded
    public class PluginConsole : DeadCorePlugin
    {
		#region IPluginInfo Interface
		public override string Name		{ get { return "Plugin Console"; } }
		public override string Author	{ get { return "Standalone"; } }
		public override string Version	{ get { return "1.1"; } }
		public override string Desc		{ get { return "Provides access to a simple in game console (as well as logging to file) and allows plugins to register console commands"; } }
		#endregion
		
		#region Data Members
		static PluginConsole instance;
		public static PluginConsole Instance
		{
			get
			{
				return instance;
			}
		}

		static string logsLocation;

		//Toggleable 
		static bool logToFile = true;

		//Delegate for defining console command callback methods
		public delegate void ConsoleCommandCallback(string[] input);

		//Dictionary for storing and checking/calling registered console commands
		static Dictionary<string, ConsoleCommand> registeredConsoleCommands;

		//If after trimming console input of these characters the input is empty, ignore it
		char[] trimParams = { ' ' };
		#endregion

		#region Data Members for ConsoleUI
		//Console message history
		static Queue<string> textQueue;

		//User input buffer
		string consoleInput;

		//Tracks the scroll position of the scrollablie view
		//Need to figure out a way to make it auto scroll to the bottom, or have the text scrol the opposite way
		static Vector2 scrollPos;

		//Dimensions for the container panel
		Rect consoleRect;

		//Dimensions for the scrollable view in the console
		Rect scrollableRect;

		//Dimensions for the text input field
		Rect textInputRect;

		//Dimensions for the submit button
		Rect submitButtonRect;

		KeyCode consoleKeyBind;
		static bool drawConsole;

		//Custom label style to remove padding - not implemented yet
		GUIStyle textLineStyle;
		#endregion

		#region Registered Console Commands
		void Cmd_Clear(string[] input)
		{
			textQueue.Clear();
		}

		void Cmd_List(string[] input)
		{
			foreach (KeyValuePair<string, ConsoleCommand> kvp in registeredConsoleCommands)
			{
				WriteLine("[" + kvp.Value.Owner.Name + "] '" + kvp.Key + "': " + kvp.Value.Description, this);

				/* Example Output
				[Plugin Console] 'clear': Clear the console history
				[Plugin Console] 'list': Display a list of all registered console commands
				[Plugin Manager] 'plugins list': Display a list of all currently loaded plugins
				[Plugin Manager] 'plugins info': Display detailed information about a specific plugin
				*/
			}
		}
		#endregion	

		#region Unity Methods
		void Awake()
		{
			//Since this class will be instantiated by Unity with AddComponent<DeadCorePlugin>
			//If by chance someone tries to create another GameConsole by adding it to a GameObject
			if (instance != null)
			{
				Destroy(this);
			}
			else
			{
				instance = this;

				registeredConsoleCommands = new Dictionary<string, ConsoleCommand>();
				logsLocation = GlobalVars.RootFolder + "\\dcpm-logs\\";

				#region Initializing Data Members for UnityUI
				textQueue = new Queue<string>();
				consoleInput = "";
				drawConsole = false;
				consoleKeyBind = PluginSettings.Instance.GetKeyCode("TogglePluginConsole", KeyCode.BackQuote);

				consoleRect = new Rect(10, 10, 1000, 600);
				scrollableRect = new Rect(consoleRect.left + 5, consoleRect.top, consoleRect.width - 5, consoleRect.height - 30);
				textInputRect = new Rect(consoleRect.left + 5, consoleRect.top + consoleRect.height - 25, consoleRect.width - 100, 20);
				submitButtonRect = new Rect(consoleRect.left + consoleRect.width - 95, consoleRect.top + consoleRect.height - 25, 85, 20);
				#endregion

				#region Registering Console Commands
				RegisterConsoleCommand("clear", Cmd_Clear, "Clear the console history", this);
				RegisterConsoleCommand("help", Cmd_List, "Display a list of all registered console commands", this);
				#endregion

				WriteLine("Version " + Version + " Initialized", this);
			}
		}

		void Update()
		{
			if (Input.GetKeyDown(consoleKeyBind))
			{
				if (drawConsole)
				{
					drawConsole = false;
					DisableCursor();
				}
				else
				{
					//Enable the cursor when we open the console
					drawConsole = true;
					EnableCursor();
				}
			}
		}

		void OnGUI()
		{
			if (drawConsole)
			{
				if (textLineStyle == null)
				{
					textLineStyle = new GUIStyle(GUI.skin.label);
					textLineStyle.margin = new RectOffset(0, 0, 0, 0);
					textLineStyle.padding = new RectOffset(0, 0, 0, 0);
				}

				GUI.Box(consoleRect, "");
				GUILayout.BeginArea(scrollableRect);
				scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(scrollableRect.width), GUILayout.Height(scrollableRect.height));

				foreach (string line in textQueue)
				{
					GUILayout.Label(line, textLineStyle);
				}

				GUILayout.EndScrollView();
				GUILayout.EndArea();

				if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.Return)
				{
					Submit(consoleInput);
				}

				consoleInput = GUI.TextField(textInputRect, consoleInput);


				if (GUI.Button(submitButtonRect, "Submit"))
				{
					Submit(consoleInput);
				}
			}
		}
		#endregion

		#region Private Methods
		void Submit(string input)
		{
			WriteLine(input);

			List<string> inputList = new List<string>(input.Split(' '));

			if (inputList.Count > 0)
			{
				foreach (KeyValuePair<string, ConsoleCommand> kvp in registeredConsoleCommands)
				{
					if (inputList[0].Equals(kvp.Key))
					{
						inputList.RemoveAt(0);

						try
						{
							kvp.Value.Callback.Invoke(inputList.ToArray());
						}
						catch (Exception ex)
						{
							WriteLine("Error: Console command '" + kvp.Key + "' caused an exception and has been removed from the registered console commands to prevent further errors", this);
							WriteLine(ex.ToString(), this);
							registeredConsoleCommands.Remove(kvp.Key);
						}
						
					}
				}
			}
		}

		void DisableCursor()
		{
			if (GameManager.Instance.CurrentGameState == GameManager.GameState.InGame)
			{
				Screen.showCursor = false;
				Screen.lockCursor = true;

				MouseLook[] lookArray = UnityEngine.Object.FindObjectsOfType(typeof(MouseLook)) as MouseLook[];
				foreach (MouseLook look in lookArray)
				{
					look.enabled = true;
				}
			}
		}

		void EnableCursor()
		{
			if (GameManager.Instance.CurrentGameState == GameManager.GameState.InGame)
			{
				Screen.showCursor = true;
				Screen.lockCursor = false;

				MouseLook[] lookArray = UnityEngine.Object.FindObjectsOfType(typeof(MouseLook)) as MouseLook[];
				foreach (MouseLook look in lookArray)
				{
					look.enabled = false;
				}
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Registers a console command with the Plugin Console, whenever user input starts with the 'command' parameter supplied, a callback will occur on the supplied callback with the input in an array split by ' ' (spaces)
		/// </summary>
		/// <param name="command">String that the command will bind to</param>
		/// <param name="callback">Callback method</param>
		/// <param name="description"></param>
		/// <param name="owner"></param>
		public static void RegisterConsoleCommand(string command, ConsoleCommandCallback callback, string description, IPluginInfo owner)
		{
			registeredConsoleCommands.Add(command, new ConsoleCommand(callback, description, owner));
		}

		/// <summary>
		/// Allows plugins to forcefully hide the console
		/// </summary>
		public static void HideConsole()
		{
			drawConsole = false;
		}

		/// <summary>
		/// Allows plugins to write a message into the console and into the default logging file
		/// </summary>
		/// <param name="message"></param>
		/// <param name="plugin"></param>
		/// <param name="logFile"></param>
		/// <param name="args"></param>
		public static void WriteLine(string message, IPluginInfo plugin = null, string logFile = "default.log", params object[] args)
		{
			//Ensure the directory exisits
			Directory.CreateDirectory(logsLocation);

			message = string.Format(message, args);

			//If an IPluginInfo has been passed in with the mssage
			if (plugin != null)
			{
				message = string.Format("[{0}] {1}", plugin.Name, message);
			}

			textQueue.Enqueue(message);

			//If logging to file is enabled (it is by default)
			if (logToFile)
			{
				message = string.Format("[{0}] {1}", DateTime.Now.ToString("dd/MM/yyyy - HH:mm:ss"), message);
				using (StreamWriter sw = new StreamWriter(new FileStream(logsLocation + logFile, FileMode.Append, FileAccess.Write)))
				{
					sw.WriteLine(message, args);
				}
			}

			scrollPos.y = Mathf.Infinity;
		}
		#endregion

		//Private inner class for defining console commands with a description & an owner
		private class ConsoleCommand
		{
			public IPluginInfo Owner;
			public ConsoleCommandCallback Callback;
			public string Description;

			public ConsoleCommand(ConsoleCommandCallback callback, string description, IPluginInfo owner)
			{
				this.Callback = callback;
				this.Description = description;
				this.Owner = owner;
			}
		}
	}
}
