using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NhMultiTenant.Model
{
	public abstract class EntityBase
	{
		private long? tenantId;

		public virtual long Id { get; private set; }
	}
}
