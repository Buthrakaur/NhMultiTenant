using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NhMultiTenant.Model
{
	public abstract class EntityBase
	{
		//todo: map as private property - not possible at the moment due to MappingByCode limitation
		protected virtual long TenantId { get; private set; }

		private static long lastId;
		public virtual long Id { get; protected set; }

		protected EntityBase()
		{
			Id = ++lastId;
		}
	}
}
