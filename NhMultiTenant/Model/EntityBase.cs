﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NhMultiTenant.Model
{
	public abstract class EntityBase
	{
		private long tenantId;

		private static long lastId;
		public virtual long Id { get; protected set; }

		protected EntityBase()
		{
			Id = ++lastId;
		}
	}
}
