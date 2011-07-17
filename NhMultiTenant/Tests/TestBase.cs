using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl.CustomizersImpl;
using NHibernate.Tool.hbm2ddl;
using NhMultiTenant.Infrastructure;
using NhMultiTenant.Model;

namespace NhMultiTenant.Tests
{
	public class TestBase: IDisposable
	{
		private readonly ISessionFactory sessionFactory;
		private readonly ISession session;

		protected ISession Session
		{
			get { return session; }
		}

		public TestBase()
		{
			var cfg = new Configuration()
				.DataBaseIntegration(x =>
				                     	{
				                     		x.Driver<SQLite20Driver>();
				                     		x.Dialect<SQLiteDialect>();
				                     		x.ConnectionString = "Data Source=:memory:;Version=3;New=True";
				                     	})
				.SetInterceptor(new MultiTenantInterceptor(() => GetCurrentTenant()))
				.SetProperty("show_sql", "true")
				;

			var mapper = new ModelMapper();
			MapEntities(mapper);
			var hbmMappings = mapper.CompileMappingForAllExplicitlyAddedEntities();
			cfg.AddDeserializedMapping(hbmMappings, null);
			sessionFactory = cfg.BuildSessionFactory();
			session = sessionFactory.OpenSession();

			session.BeginTransaction();
			new SchemaExport(cfg).Execute(false, true, false, session.Connection, null);

			var tenant = new Tenant("defaultTenant");
			session.Save(tenant);
			DefaultTenantId = tenant.Id;
		}

		protected readonly long DefaultTenantId;
		protected virtual Tenant GetCurrentTenant()
		{
			return session.Load<Tenant>(DefaultTenantId);
		}

		private void MapEntities(ModelMapper mapper)
		{
			mapper.Class<Tenant>(ca =>
			                     	{
			                     		ca.Id(x => x.Id,
			                     		      id => id.Generator(Generators.Assigned));
			                     		ca.Property(x => x.Name,
			                     		            p =>
			                     		            	{
			                     		            		p.NotNullable(true);
			                     		            		p.Unique(true);
			                     		            	});
			                     	});
			mapper.Class<Contact>(ca =>
			                     	{
			                     		ca.Id(x => x.Id,
			                     		      id => id.Generator(Generators.Assigned));
			                     		ca.Property(x => x.Name,
			                     		            p =>
			                     		            	{
			                     		            		p.NotNullable(true);
			                     		            		p.Unique(true);
			                     		            	});
			                     		ca.Property("TenantId",
			                     		            p => p.NotNullable(true));
			                     	});
		}

		public void Dispose()
		{
			session.Transaction.Rollback();
			session.Dispose();
			sessionFactory.Dispose();
		}
	}
}
