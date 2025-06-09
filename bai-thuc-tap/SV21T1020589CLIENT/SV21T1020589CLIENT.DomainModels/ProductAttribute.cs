namespace SV21T1020589CLIENT.DomainModels
{
    public class ProductAttribute
    {
        public long AttributeID { get; set; }
        public long ProductID { get; set; }
        public string AttributeName { get; set; } = "";
        public string AttributeValue { get; set; } = "";
        public int DisplayOrder { get; set; }
    }
}
