using SV21T1020589.DataLayers;
using SV21T1020589.DataLayers.SQLServer;
using SV21T1020589.DomainModels;

namespace SV21T1020589.BusinessLayers
{
    public static class UserAccountService
    {
        private static readonly IUserAccountDAL employeeAccountDB;
        private static readonly IUserAccountDAL customerAccountDB;

        static UserAccountService() 
        {
			string connectionString = @"Server=DESKTOP-IT9GPIL;user id=sa;password=12345678;Database=LiteComerceDB;TrustServerCertificate=True";

			employeeAccountDB = new DataLayers.SQLServer.EmployeeAccountDAL(connectionString);
            customerAccountDB = new DataLayers.SQLServer.CustomerAccountDAL(connectionString);
        }

        public static UserAccount? Authorize(UserTypes userTypes, string username , string password)
        {
            if(userTypes == UserTypes.Employee) 
                return employeeAccountDB.Authorize(username, password);
            else
                return customerAccountDB.Authorize(username, password);
        }

        public static bool ChangePassword(string username, string oldPassword, string newPassword)
        {
           bool result = employeeAccountDB.ChangePassword(username, oldPassword, newPassword);
           return result;
        }

    }

    public enum UserTypes
    {
        Employee,
        Customer
    }
}
