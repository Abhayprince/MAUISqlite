using SQLite;

namespace MAUISql.Models
{
    public class Product
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public Product Clone() => MemberwiseClone() as Product;
    }
}
