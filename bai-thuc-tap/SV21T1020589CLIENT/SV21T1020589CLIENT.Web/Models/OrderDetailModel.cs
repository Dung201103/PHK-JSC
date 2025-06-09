using SV21T1020589CLIENT.DomainModels;

namespace SV21T1020589CLIENT.Web.Models
{
    public class OrderDetailModel
    {
        public Order? Order { get; set; }
        public required List<OrderDetail> Details { get; set; }
    }
}
