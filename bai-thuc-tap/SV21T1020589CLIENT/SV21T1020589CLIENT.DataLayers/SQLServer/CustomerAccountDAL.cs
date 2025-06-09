using Dapper;
using SV21T1020589CLIENT.DomainModels;

namespace SV21T1020589CLIENT.DataLayers.SQLServer
{
    public class CustomerAccountDAL : BaseDAL, ICustomerDAL
    {
        public CustomerAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Customer data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Customers where Email = @Email)
                                select -1
                            else 
                                begin
                                    insert into Customers (CustomerName , ContactName , Province ,Address ,Phone , Email , Password,IsLocked)
                                    values(@CustomerName , @ContactName , @Province ,@Address ,@Phone , @Email , @Password, @IsLocked)
                                    select scope_identity();
                                end";
                var parameters = new
                {
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    Password = data.Password ?? "",
                    IsLocked =data.IsLocked
                };
                //thuc thi cau lenh SQL
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public Customer? Authorize(string username, string password)
        {
            Customer? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                            FROM Customers WHERE Email = @Email AND Password = @Password AND IsLocked = 0
                            ";
                var parameters = new
                {
                    Email = username,
                    Password = password
                };
                data = connection.QueryFirstOrDefault<Customer>(sql, parameters, commandType: System.Data.CommandType.Text);
            }
            return data;
        }

        public bool ChangePassword(string CustomerID, string oldPassword, string newPassword)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update Customers 
                            set Password = @newPassword
                            where CustomerID = @CustomerID and Password = @oldPassword";
                var parameters = new       
                {
                    CustomerID = CustomerID,
                    newPassword = newPassword,
                    oldPassword = oldPassword,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Customer? Get(int id)
        {
            Customer? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Customers where CustomerID = @CustomerID";
                var parameters = new
                {
                    CustomerId = id
                };
                data = connection.QueryFirstOrDefault<Customer>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool Update(Customer data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Customers where CustomerId <> @CustomerId and Email = @Email)
                            begin
                                update Customers
                                set  CustomerName = @CustomerName  , 
                                    ContactName= @ContactName , 
                                    Province  =@Province , 
                                    Address=@Address ,
                                    Email=@Email , 
                                    Phone=@Phone ,
                                    Password=@Password
                            where CustomerID = @CustomerID
                            end";
                var parameters = new
                {
                    CustomerID = data.CustomerID,
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    Password = data.Password ?? ""
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

    }
}
