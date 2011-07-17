using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NhMultiTenant.Model
{
	public abstract class EntityBase
	{
		//todo: map as private field - not possible due to MappingByCode limitation
		protected virtual long TenantId { get; set; }

		private static long lastId;
		public virtual long Id { get; protected set; }

		protected EntityBase()
		{
			Id = ++lastId;
		}
	}
}
