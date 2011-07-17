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
		private const string TenantPropertyName = "TenantId";
		private readonly Func<Tenant> currentTenantProvider;

		public MultiTenantInterceptor(Func<Tenant> currentTenantProvider)
		{
			this.currentTenantProvider = currentTenantProvider;
		}

		public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
		{
			var index = Array.IndexOf(propertyNames, TenantPropertyName);

			if (index == -1)
				return false;

			var tenantId = currentTenantProvider().Id;

			state[index] = tenantId;

			typeof(EntityBase)
				.GetProperty(TenantPropertyName, BindingFlags.Instance | BindingFlags.NonPublic)
				.SetValue(entity, tenantId,
				          BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.FlattenHierarchy,
				          null,
				          null,
				          null
				);

			return false;
		}

		public override bool OnLoad(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
		{
			var index = Array.IndexOf(propertyNames, TenantPropertyName);

			if (index == -1)
				return false;

			var entityTenantId = Convert.ToInt64(state[index]);

			var currentTenantId = currentTenantProvider().Id;

			if (entityTenantId != currentTenantId)
			{
				throw new UnauthorizedAccessException(string.Format("Tenant violation on {0}#{1}", entity.GetType().Name, id));
			}

			return false;
		}
	}
}
