namespace MatrixWebApp.Models
{
    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; } = new();

        public int TotalItems => Items.Sum(i => i.Quantity);
        public decimal TotalPrice => Items.Sum(i => i.Quantity * i.Price);

        public void AddItem(CartItem item)
        {
            var existing = Items.FirstOrDefault(p => p.ProductId == item.ProductId);
            if (existing != null)
            {
                existing.Quantity = item.Quantity;
            }
            else
            { 
                Items.Add(item);
            }
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var item = Items.FirstOrDefault(p => p.ProductId == productId);
            if (item != null)
            { 
                item.Quantity = quantity;
            }
        }

        public void RemoveItem(int productId)
        { 
            Items.RemoveAll(p => p.ProductId == productId);
        }

        public void Clear()
        {
            Items.Clear();
        }
    }
}