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
using NHibernate.Engine;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl.CustomizersImpl;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Type;
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
				.SetInterceptor(new MultiTenantInterceptor(SaveInterceptorTennantGetter))
				.SetProperty("show_sql", "true")
				;
			cfg.AddFilterDefinition(new FilterDefinition("tenantFilter",
			                                             "TenantId = :tenantId",
			                                             new Dictionary<string, IType> {{"tenantId", TypeFactory.Basic("System.Int64")}},
			                                             false));

			var mapper = new ModelMapper();
			MapEntities(mapper);
			var hbmMappings = mapper.CompileMappingForAllExplicitlyAddedEntities();
			cfg.AddDeserializedMapping(hbmMappings, null);
			sessionFactory = cfg.BuildSessionFactory();
			session = sessionFactory.OpenSession();
			session.EnableFilter("tenantFilter")
				.SetParameter("tenantId", FilterTenantId);

			session.BeginTransaction();
			new SchemaExport(cfg).Execute(false, true, false, session.Connection, null);

			var tenant = new Tenant(123456789, "defaultTenant");
			session.Save(tenant);
			SaveInterceptorTenantId = tenant.Id;
		}

		protected long SaveInterceptorTenantId;
		private Tenant SaveInterceptorTennantGetter()
		{
			return session.Load<Tenant>(SaveInterceptorTenantId);
		}

		protected virtual long FilterTenantId { get { return SaveInterceptorTenantId; } }

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
										ca.Filter("tenantFilter", fm => {});
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
