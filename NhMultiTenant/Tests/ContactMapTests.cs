using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NhMultiTenant.Model;
using Xunit;

namespace NhMultiTenant.Tests
{
	public class ContactMapTests: TestBase
	{
		[Fact]
		public void CanPersistWithDefaultTenantId()
		{
			var c = new Contact("c");
			Session.Save(c);
			
			Session.Flush();
			Session.Clear();

			using(var cmd = Session.Connection.CreateCommand())
			{
				cmd.CommandText = "select tenantid from Contact where name = 'c'";
				Assert.Equal(DefaultTenantId, Convert.ToInt64(cmd.ExecuteScalar()));
			}
		}

		[Fact]
		public void CanPersistWithOtherTenantId()
		{
			CurrentTennantGetter = () => new Tenant(666, "t666");
			var c = new Contact("c");
			Session.Save(c);
			
			Session.Flush();
			Session.Clear();

			using(var cmd = Session.Connection.CreateCommand())
			{
				cmd.CommandText = "select tenantid from Contact where name = 'c'";
				Assert.Equal(666L, Convert.ToInt64(cmd.ExecuteScalar()));
			}
		}
	}
}
