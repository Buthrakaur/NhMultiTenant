using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;

namespace NhMultiTenant.Tests
{
	public class TestBase: IDisposable
	{
		private readonly ISessionFactory sessionFactory;
		private readonly ISession session;

		public TestBase()
		{
			var cfg = new Configuration()
				.DataBaseIntegration(x =>
					{
						x.Driver<SQLite20Driver>();
						x.Dialect<SQLiteDialect>();
						x.ConnectionString = "Data Source=:memory:;Version=3;New=True";
					});

			var mapper = new ModelMapper();
			SetupMappings(mapper);
			var hbmMappings = mapper.CompileMappingForAllExplicitlyAddedEntities();
			cfg.AddDeserializedMapping(hbmMappings, null);
			sessionFactory = cfg.BuildSessionFactory();
			session = sessionFactory.OpenSession();
		}

		private void SetupMappings(ModelMapper mapper)
		{
			todo
		}

		public void Dispose()
		{
			session.Dispose();
			sessionFactory.Dispose();
		}
	}
}
