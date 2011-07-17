using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NhMultiTenant.Model;
using Xunit;

namespace NhMultiTenant.Tests
{
	public class TenantMapTests: TestBase
	{
		[Fact]
		public void CanPersistTenant()
		{
			var t = new Tenant(123, "t1");
			Session.Save(t);

			Session.Flush();
			Session.Clear();

			var t1 = Session.Load<Tenant>(123L);

			Assert.Equal("t1", t1.Name);
		}
	}
}
