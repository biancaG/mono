// Authors:
//      Marek Habersack <mhabersack@novell.com>
//
// Copyright (C) 2010 Novell Inc. http://novell.com
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using static System.Web.HttpApplication;

using NUnit.Framework;
using MonoTests.Common;

namespace MonoTests
{
	[TestFixture]
	public class HttpApplicationTest
	{
#if NET_4_0
		[Test]
		public void GetOutputCacheProviderName ()
		{
			var app = new HttpApplication ();

			Assert.AreEqual ("AspNetInternalProvider", app.GetOutputCacheProviderName (null), "#A1");
		}
#endif

		[Test]
		public void ModuleIsRegistered()
		{
			var httpApp = new HttpApplication();
			var str = "blah";
			HttpApplication.RegisterModule(str.GetType());
			var cfg = WebConfigurationManager.GetWebApplicationSection ("system.web/httpModules") as HttpModulesSection;
			foreach (HttpModuleAction module in cfg.Modules)
			{
				if (module.Name.Contains("__Dynamic_Module_System.String") && module.Type.Contains("System.String"))
					return;
			}
			Assert.Fail("Module not added to the config.");
		}

		[Test]
		public void RegisterModuleFailsGracefully()
		{
			var httpApp = new HttpApplication();
			HttpApplication.RegisterModule(null);
			var cfg = WebConfigurationManager.GetWebApplicationSection ("system.web/httpModules") as HttpModulesSection;
		}

	}
}
