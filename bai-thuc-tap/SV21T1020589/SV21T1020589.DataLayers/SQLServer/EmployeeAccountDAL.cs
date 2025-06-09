using Dapper;
using SV21T1020589.DomainModels;

namespace SV21T1020589.DataLayers.SQLServer
{
    public class EmployeeAccountDAL : BaseDAL, IUserAccountDAL
    {
        public EmployeeAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string username, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"
                            SELECT EmployeeID as UserId,
                                    Email as UserName ,
                                    FullName as DisplayName,
                                    Photo,
                                    RoleNames
                            FROM Employees WHERE Email = @Email AND Password = @Password ";
                var parameters = new
                {
                    Email = username,
                    Password = password
                };
                data = connection.QueryFirstOrDefault<UserAccount>(sql, parameters ,commandType : System.Data.CommandType.Text);
            }
            return data;
        }

        //public bool ChangePassword(string username, string password)
        //{
        //    throw new NotImplementedException();
        //}

        //public static bool ChangePassword(string username, string oldPassword, string newPassword)
        //{
        //    bool result = false;
        //    using (var connection = OpenConnection())
        //    {
        //        var sql = @"update Employees 
        //            set Password = @newPassword
        //            where Email = @username and Password = @oldPassword";
        //        var parameters = new
        //        {
        //            username = username,
        //            newPassword = newPassword,
        //            oldPassword = oldPassword,
        //        };
        //        result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
        //        connection.Close();
        //    }
        //    return result;
        //}

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update Employees 
                    set Password = @newPassword
                    where Email = @username and Password = @oldPassword";
                var parameters = new
                {
                    username = username,
                    newPassword = newPassword,
                    oldPassword = oldPassword,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
