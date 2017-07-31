using DCPM.PluginBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace DCPM.Common
{
	public class BreadSmugged : DeadCorePlugin
	{
		#region IPluginInfo
		public override string Name { get { return "Bread Smug"; } }
		public override string Author { get { return "Standalone"; } }
		public override string Version { get { return "1.0"; } }
		public override string Desc { get { return "Adds a 'watermark' into the Game to indicate that the Plugin Manager is indeed running"; } }
		#endregion

		#region Data Members
		Texture2D _breadTex;
		#endregion

		void Start()
		{
			try
			{
				Assembly myAssembly = Assembly.GetExecutingAssembly();
				Stream myStream = myAssembly.GetManifestResourceStream("DCPM.Common.breadcore.png");

				byte[] byteArray = new byte[myStream.Length];
				myStream.Read(byteArray, 0, byteArray.Length);

				_breadTex = new Texture2D(128, 128, TextureFormat.DXT1, false);
				_breadTex.LoadImage(byteArray);

				PluginConsole.WriteLine("BreadSmug texture loaded from Assembly", this);
			}
			catch (Exception ex)
			{
				PluginConsole.WriteLine(ex.ToString(), this);
			}
		}

		void OnLevelWasLoaded(int level)
		{
			//If we failed to load the BreadSmug texture then there's no point continuing
			//TODO - Do we possibly stop any plugins loading if this doesn't work? Probably not, effort
			if (_breadTex == null)
				return;

			if (DeadCoreLevels.PlayableLevelIDs.ContainsKey(level))
			{
				var checkpoints = Resources.FindObjectsOfTypeAll<CheckPointScript>();
				var clouds = Resources.FindObjectsOfTypeAll<ParticleSystem>();
				foreach (var cloud in clouds)
				{
					//If the cloud is named 'cloud-lightning' in Croissant language
					if (cloud.name == "nuage-eclair")
					{
						cloud.renderer.material.mainTexture = _breadTex;
						cloud.emissionRate = cloud.emissionRate * 1.5f;
					}	
				}
			}
		}
	}
}
