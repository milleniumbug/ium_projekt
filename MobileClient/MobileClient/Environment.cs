using System;
using System.Collections.Generic;
using System.Text;

namespace MobileClient
{
    public class Environment
    {
		public string DataBasePath { get; }

	    public Environment(string dataBasePath)
	    {
		    DataBasePath = dataBasePath;
	    }
    }
}
