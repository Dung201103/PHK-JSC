using Microsoft.Data.SqlClient;

namespace SV21T1020589.DataLayers.SQLServer
{
    /// <summary>
    /// Lóp cơ sở (lớp cha) của các lớp cài đặt các phép xử lý dữ liệu trên SQL server
    /// </summary>
    public abstract class BaseDAL
    {
        /// <summary>
        /// Chuỗi tham số kết nối đến CSDL SQL Server
        /// </summary>
        protected string connectionString = "";
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectionString"></param>
        public BaseDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        /// <summary>
        /// Tạo và mở 1 kết nối đến CSDL (SQL server)
        /// </summary>
        /// <returns></returns>
        protected SqlConnection OpenConnection()
        {
            SqlConnection connection =  new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
