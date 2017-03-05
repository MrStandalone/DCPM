using System;
using System.Collections.Generic;
using System.Text;

namespace DCPM.PluginBase
{
    public interface IPluginInfo
    {
		string Name { get; }
		string Author { get; }
		string Version { get; }
		string Desc { get; }
    }
}
