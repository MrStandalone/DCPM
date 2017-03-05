using System;
using System.Collections.Generic;
using UnityEngine;

namespace DCPM.Common
{
	public static class DeadCoreLevels
	{
		static Dictionary<int, string> unfriendlyNames;
		/// <summary>
		/// Internal Unity names of the levels (For use with the DeadCore SceneLoader.LoadLevel([unfriendly name]);
		/// </summary>
		public static Dictionary<int, string> UnfriendlyNames
		{
			get { return unfriendlyNames; }
		}

		static Dictionary<int, string> playableLevelIds;
		public static Dictionary<int, string> PlayableLevelIDs
		{
			get { return playableLevelIds; }
		}

		static Dictionary<int, string> friendlyNames;
		/// <summary>
		/// English-like names of the levels in DeadCore (Used for ouptut)
		/// </summary>
		public static Dictionary<int, string> FriendlyNames
		{
			get { return friendlyNames; }
		}

		/// <summary>
		/// Global public enum of the levels in DeadCore
		/// </summary>
		public enum Levels
		{
			SplashScreen = 1,
			OverCamera = 2,
			Loading_Screen = 3,
			Main_Menu = 4,
			EscapeScene = 5,
			EndStoryMenu = 6,
			EndStorySpeedRunMenu = 7,
			The_Fall = 8,
			The_Tower = 9,
			The_Void = 10,
			The_Storm = 11,
			The_Ascension = 12,
			CinematicIntro = 13,
			CinematicEnd = 14,
			The_Summit = 15,
			Alpha = 16,
			Beta = 17,
			Gamma = 18, 
			Delta = 19,
			Epsilon = 20,
			Zeta = 21,
			Eta = 22, 
			Theta = 23, 
			Iota = 24, 
			Kappa = 25, 
			Nicolas = 26,
			Clement = 27,
			Wandrille = 28
		}

		static DeadCoreLevels()
		{
			unfriendlyNames = new Dictionary<int, string>()
			{
				{ 1, "SplashScreen" },
				{ 2, "OverCamera" },		//No idea
				{ 3, "SceneLoader" },		//Loading Screen
				{ 4, "MainMenu" },			//Main Menu
				{ 5, "EscapeScene" },		//Pretty sure this is just an empty scene
				{ 6, "EndStoryMenu" },
				{ 7, "EndStorySpeedRunMenu" },
				{ 8, "level01" },			//The Fall
				{ 9, "level02" },			//The Tower
				{ 10, "level03" },			//The Void
				{ 11, "level04" },			//The Storm
				{ 12, "level05" },			//The Ascension
				{ 13, "CinematicIntro" },	//Cinematic Into video
				{ 14, "CinematicEnd" },		//Cinematic Ending video
				{ 15, "level06" },			//The Summit (Final level)
				{ 16, "level01_Spark01" },	//Alpha
				{ 17, "level01_Spark02" },	//Beta
				{ 18, "level02_Spark_01" },	//Gamma
				{ 19, "level02_Spark_02" },	//Delta
				{ 20, "level03_Spark_01" },	//Epsilon
				{ 21, "level03_Spark_02" },	//Zeta
				{ 22, "level04_Spark_01" },	//Eta
				{ 23, "level04_Spark_02" },	//Theta
				{ 24, "level05_Spark_01" },	//Iota
				{ 25, "level05_Spark_02" },	//Kappa
				{ 26, "level_nico" },		//Nicolas - Backer
				{ 27, "level_clement" },	//Clement - Backer
				{ 28, "level_wandrille" }	//Wandrille - Backer
			};

			playableLevelIds = new Dictionary<int, string>()
			{
				{ 8, "LEVEL_01" },
				{ 9, "LEVEL_02" },
				{ 10, "LEVEL_03" },
				{ 11, "LEVEL_04" },
				{ 12, "LEVEL_05" },
				{ 16, "LEVEL_01_SPARK_01" },		//Alpha
				{ 17, "LEVEL_01_SPARK_02" },		//Beta
				{ 18, "LEVEL_02_SPARK_01" },		//Gamma
				{ 19, "LEVEL_02_SPARK_02" },		//Delta
				{ 20, "LEVEL_03_SPARK_01" },		//Epsilon
				{ 21, "LEVEL_03_SPARK_02" },		//Zeta
				{ 22, "LEVEL_04_SPARK_01" },		//Eta
				{ 23, "LEVEL_04_SPARK_02" },		//Theta
				{ 24, "LEVEL_05_SPARK_01" },		//Iota
				{ 25, "LEVEL_05_SPARK_02" },		//Kappa
				{ 26, "LEVEL_NICO" },				//Nicolas - Backer
				{ 27, "LEVEL_CLEMENT" },			//Clement - Backer
				{ 28, "LEVEL_WANDRILLE" }			//Wandrille - Backer
			};

			friendlyNames = new Dictionary<int, string>()
			{
				{ 1, "Splash Screen" },
				{ 2, "OverCamera" },		//No idea
				{ 3, "Loading Screen" },	//Loading Screen
				{ 4, "Main Menu" },			//Main Menu
				{ 5, "EscapeScene" },		//Pretty sure this is just an empty scene
				{ 6, "EndStoryMenu" },
				{ 7, "EndStorySpeedRunMenu" },
				{ 8, "The Fall" },			//The Fall
				{ 9, "The Tower" },			//The Tower
				{ 10, "The Void" },			//The Void
				{ 11, "The Storm" },		//The Storm
				{ 12, "The Ascension" },	//The Ascension
				{ 13, "Cinematic Intro" },	//Cinematic Intro Video
				{ 14, "Cinematic End" },		//Cinematic Ending video
				{ 15, "The Summit" },		//The Summit (Final level)
				{ 16, "Spark Alpha" },		//Alpha
				{ 17, "Spark Beta" },		//Beta
				{ 18, "Spark Gamma" },		//Gamma
				{ 19, "Spark Delta" },		//Delta
				{ 20, "Spark Epsilon" },	//Epsilon
				{ 21, "Spark Zeta" },		//Zeta
				{ 22, "Spark Eta" },		//Eta
				{ 23, "Spark Theta" },		//Theta
				{ 24, "Spark Iota" },		//Iota
				{ 25, "Spark Kappa" },		//Kappa
				{ 26, "Backer Nico" },		//Nicolas - Backer
				{ 27, "Backer Clement" },	//Clement - Backer
				{ 28, "Backer Wandrille" }	//Wandrille - Backer
			};
		}
	}

	public static class GlobalVars
	{
		#region Data Members
		/// <summary>
		/// Should always be DeadCore Install Directory\DeadCore_Data\Managed\
		/// </summary>
		public static string RootFolder
		{
			get { return Environment.CurrentDirectory + "\\DeadCore_Data\\Managed"; }
		}

		/// <summary>
		/// Provides access to the Arial font that DeadCore uses, since creating your own fonts is quite difficult without the Unity Editor
		/// TODO - Figure out a way to create a Unity Asset that we can use to load fonts/models etc etc
		/// </summary>
		static Font unityArialFont;
		public static Font UnityArialFont
		{
			get { return unityArialFont; }
		}

		/// <summary>
		/// Returns the profile name of the steam account running the game
		/// </summary>
		public static string SteamName
		{
			get { return ManagedSteam.Steam.Instance.Friends.GetPersonaName(); }
		}
		#endregion

		#region Constructors
		static GlobalVars()
		{
			unityArialFont = Resources.FindObjectsOfTypeAll<Font>()[0];
		}
		#endregion Constructors
	}
}
