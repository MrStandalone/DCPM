using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

//Don't want to do this but we will need to for console output
using DCPM.Common;
using DCPM.PluginBase;

namespace DCPM
{
	/// <summary>
	/// TODO: Implement unloading of Assemblies/Plugins
	/// TODO: Implement Plugin Folder structure so plugins can have accompanying files etc
	/// TODO: Implement better error detection/assembly checking to make sure that loading the assembly doesn't break things
	/// TODO: Remove singleton as there should be no need to keep an instance of this alive, although if we do unloading we may need to keep it around
	/// </summary>

	class AssemblyManager : IPluginInfo
	{
		#region IPluginInfo
		//While not 100% necessary to implement this interface, we are doing so for ease of use with the PluginConsole
		public string Name		{ get { return "Assembly Manager"; } }
		public string Author	{ get { return "Standalone"; } }
		public string Version	{ get { return "1.0"; } }
		public string Desc		{ get { return "Simple assembly loader, not actually a plugin"; } }
		#endregion

		#region Data Members
		//Singleton instance, not really needed, could do this as a static class and have the same effect
		private static AssemblyManager instance;
		public static AssemblyManager Instance
		{
			get
			{
				if (instance == null)
					instance = new AssemblyManager();

				return instance;
			}
		}

		//static AppDomain pluginDomain;
		#endregion

		//Private constructor so this class cannot be instantiated elsewhere
		private AssemblyManager()
		{

		}

		#region Public Methods
		
		/// <summary>
		/// Returns an enumerable list of a specific class type found in a list of assemblies
		/// </summary>
		/// <typeparam name="T">The type of class that you want returned in the enumerable list</typeparam>
		/// <param name="assemblies">The list of assembles to look through</param>
		/// <returns>Returns an enumerable list of a specific class type found in a list of assemblies</returns>
		/// 
		/// TODO: Rename this, horrible name
		public IEnumerable<Type> GetTypes<T>(IEnumerable<Assembly> assemblies)
		{
			List<Type> types = new List<Type>();

			foreach (Assembly assembly in assemblies)
			{
				try
				{
					IEnumerable<Type> assemblyTypes = assembly.GetTypes();

					foreach (Type type in assemblyTypes)
					{
						if (type.IsAssignableFrom(typeof(T)) || type.IsSubclassOf(typeof(T)))
						{
							types.Add(type);
						}
					}
				}
				catch (Exception ex)
				{
					PluginConsole.WriteLine("Failed to get types from assembly: '" + assembly.FullName + "'", this);
					PluginConsole.WriteLine(ex.Message);
				}
			}

			return types;
		}

		/// <summary>
		/// Returns an enumerable list of assembles in a directory
		/// </summary>
		/// <param name="path">File directory location</param>
		/// <returns>Returns an enumerable list of assembles in a directory</returns>
		public IEnumerable<Assembly> LoadAssemblies(string path)
		{
			AssemblyName assemblyName = null;
			Assembly assembly = null;
			List<Assembly> assemblies = new List<Assembly>();

			foreach(string dllPath in Directory.GetFiles(path, "*.dll"))
			{
				try 
				{
					assemblyName = AssemblyName.GetAssemblyName(dllPath);
					assembly = Assembly.Load(assemblyName);

					PluginConsole.WriteLine("Assembly loaded: "+ assembly.ToString(), this);

					assemblies.Add(assembly);
				}
				catch (Exception ex)
				{
					PluginConsole.WriteLine("Error loading assembly: '" + assemblyName.FullName + "'", this);
					PluginConsole.WriteLine(ex.Message, this);
				}
			}

			return assemblies;
		}
		#endregion
	}
}
