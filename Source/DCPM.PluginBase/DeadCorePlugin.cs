using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace DCPM.PluginBase
{
	public abstract class DeadCorePlugin : MonoBehaviour, IPluginInfo
	{
		public abstract string Name { get; }
		public abstract string Author { get; }
		public abstract string Version { get; }
		public abstract string Desc { get; }
	}
}
