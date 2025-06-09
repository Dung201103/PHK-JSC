using Microsoft.Data.Sqlite;

namespace SV21T1020589CLIENT.DataLayers.SQLite
{
	/// <summary>
	/// Lớp cơ sở (lớp cha) của các lớp thao tác dữ liệu sử dụng SQLite
	/// </summary>
	public abstract class BaseDAL
	{
		/// <summary>
		/// Chuỗi kết nối đến CSDL SQLite
		/// </summary>
		protected string connectionString = "";

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="connectionString">Chuỗi kết nối đến file .db</param>
		public BaseDAL(string connectionString)
		{
			this.connectionString = connectionString;
		}

		/// <summary>
		/// Tạo và mở 1 kết nối đến CSDL (SQLite)
		/// </summary>
		/// <returns></returns>
		protected SqliteConnection OpenConnection()
		{
			var connection = new SqliteConnection(connectionString);
			connection.Open();
			return connection;
		}
	}
}