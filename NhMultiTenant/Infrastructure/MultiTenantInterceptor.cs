using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NHibernate;
using NHibernate.Event;
using NhMultiTenant.Model;

namespace NhMultiTenant.Infrastructure
{
	public class MultiTenantInterceptor: EmptyInterceptor
	{
		private readonly Func<Tenant> currentTenantProvider;

		public MultiTenantInterceptor(Func<Tenant> currentTenantProvider)
		{
			this.currentTenantProvider = currentTenantProvider;
		}

		public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
		{
			var index = Array.IndexOf(propertyNames, "TenantId");

			if (index == -1)
				return false;

			var tenantId = currentTenantProvider().Id;

			state[index] = tenantId;

			typeof(EntityBase)
				.GetProperty("TenantId", BindingFlags.Instance | BindingFlags.NonPublic)
				.SetValue(entity, tenantId,
				          BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.FlattenHierarchy,
				          null,
				          null,
				          null
				);

			return false;
		}
	}
}
