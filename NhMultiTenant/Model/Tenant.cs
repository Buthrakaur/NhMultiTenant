using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NhMultiTenant.Model
{
	public class Tenant
	{
		public virtual long Id { get; protected set; }
		public virtual string Name { get; protected set; }

		protected Tenant()
		{
		}

		public Tenant(long id, string name)
		{
			Id = id;
			Name = name;
		}
	}
}
