using System.Media;

namespace DCPM
{
	/// <summary>
	/// This is a static initializer class whose only purpose is to provide a place to call a method from the module initializer code injected into the Assembly-CSharp.dll game assembly
	/// I've done it this way for now because I was making a few changes and didn't want to keep editing the Assembly-CSharp assembly
	/// This way I only make changes to my VS Project instead of having to constantly inject new code into the Assembly-CSharp assembly
	/// 
	/// Could possibly do away with this if we had a standalone executable that waits and listens for the DeadCore.exe to start up and then inject
	/// our assemblies however this way is easier in my opinion.
	/// </summary>
    public static class Initializer
    {
		public static void Initialize()
		{
			new PluginManager().Initialize();

			SystemSounds.Beep.Play();
		}
    }
}
