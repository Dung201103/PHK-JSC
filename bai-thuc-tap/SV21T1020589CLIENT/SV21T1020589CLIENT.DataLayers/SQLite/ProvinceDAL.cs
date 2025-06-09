using Dapper;
using SV21T1020589CLIENT.DomainModels;
using System.Collections.Generic;
using System.Linq;

namespace SV21T1020589CLIENT.DataLayers.SQLite
{
	public class ProvinceDAL : BaseDAL, ISimpleQueryDAL<Province>
	{
		public ProvinceDAL(string connectionString) : base(connectionString) { }

		public List<Province> List()
		{
			using (var connection = OpenConnection())
			{
				var sql = "SELECT * FROM Provinces";
				return connection.Query<Province>(sql).ToList();
			}
		}
	}
}
