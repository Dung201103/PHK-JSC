using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020589.DataLayers
{
    /// <summary>
    /// Định nghĩa chức năng truy vấn dữ liệu đơn giản
    /// </summary>
    public interface ISimpleQueryDAL<T> where T : class
    {
        /// <summary>
        /// Tuy van don gian va lay toan bo du lieu cua bang
        /// </summary>
        /// <returns></returns>
        List<T> List();
    }
}
