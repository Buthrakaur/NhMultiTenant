using System;
using System.Collections.Generic;
using NHibernate.Exceptions;
using NhMultiTenant.Model;
using Xunit;
using System.Linq;

namespace NhMultiTenant.Tests
{
	public class QueryTenantDataTests: TestBase
	{
		private Contact c1;
		private Contact c2;

		public QueryTenantDataTests()
		{
			Session.Save(new Tenant(1, "t1"));
			Session.Save(new Tenant(2, "t2"));
			Session.Flush();
			Session.Clear();

			SaveInterceptorTenantId = 1;
			Session.Save((c1 = new Contact("t1c")));
			Session.Flush();
			Session.Clear();

			SaveInterceptorTenantId = 2;
			Session.Save((c2 = new Contact("t2c")));
			Session.Flush();
			Session.Clear();
		}

		protected override long FilterTenantId { get { return 2; } }

		[Fact]
		public void CanLoadCurrentTenantData()
		{
			var c = Session.Load<Contact>(c2.Id);
			Assert.Equal("t2c", c.Name);
		}

		[Fact]
		public void CanNotReachOtherTenantDataByLoad()
		{
			var c = Session.Load<Contact>(c1.Id);
			AssertTenantViolationExceptionThrown(() => { var x1 = c.Name; });
		}

		private static void AssertTenantViolationExceptionThrown(Action exAction)
		{
			Exception ex = null;
			try
			{
				exAction();
			}
			catch (Exception x)
			{
				ex = x;
			}
			Assert.NotNull(ex);
			Assert.IsType<GenericADOException>(ex);
			Assert.IsType<UnauthorizedAccessException>(ex.InnerException);
		}

		[Fact]
		public void CanGetCurrentTenantData()
		{
			var c = Session.Get<Contact>(c2.Id);
			Assert.Equal("t2c", c.Name);
		}

		[Fact]
		public void CanNotReachOtherTenantDataByGet()
		{
			AssertTenantViolationExceptionThrown(() => Session.Get<Contact>(c1.Id));
		}

		[Fact]
		public void CanQueryCurrentTenantData()
		{
			var contacts = Session.QueryOver<Contact>().List();
			Assert.Single(contacts);
			Assert.Equal("t2c", contacts.Single().Name);
		}

		[Fact]
		public void CanNotReachOtherTenantDataByQuery()
		{
			Assert.Empty(Session.QueryOver<Contact>().Where(x => x.Name == "t1c").List());
			Assert.Empty(Session.QueryOver<Contact>().Where(x => x.Id == c1.Id).List());
		}
	}
}