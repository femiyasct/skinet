namespace Core.Entities.OrderAggregate
{
    public class ProductItemOrdered
    {
        public ProductItemOrdered()
        {
        }

        public ProductItemOrdered(int productItemId, string productName, string pictureurl)
        {
            ProductItemId = productItemId;
            ProductName = productName;
            Pictureurl = pictureurl;
        }

        public int ProductItemId { get; set; }
        public string ProductName { get; set; }
        public string Pictureurl { get; set; }
    }
}