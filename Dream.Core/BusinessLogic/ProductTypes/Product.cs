namespace Dream.Core.BusinessLogic.ProductTypes
{
    public class Product
    {
        public int IntegerId { get; set; }
        public string StringId { get; set; }

        public Product() { }

        public Product(Product product)
        {
            IntegerId = product.IntegerId;
            StringId = product.StringId;
        }
    }
}
