using Dapper;
using SV21T1020589.DomainModels;

namespace SV21T1020589.DataLayers.SQLServer
{
    public class CustomerAccountDAL : BaseDAL, IUserAccountDAL
    {
        public CustomerAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string username, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"
                            SELECT CustomerID as UserId,
                                    Email as UserName ,
                                    CustomerName as DisplayName
                            FROM Customers WHERE Email = @Email AND Password = @Password";
                var parameters = new
                {
                    Email = username,
                    Password = password
                };
                data = connection.QueryFirstOrDefault<UserAccount>(sql, parameters, commandType: System.Data.CommandType.Text);
            }
            return data;
        }


        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }
    }
}
