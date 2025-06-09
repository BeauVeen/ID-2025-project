using MatrixWebApp.Models;
using Microsoft.AspNetCore.Http;
using MatrixWebApp.Extensions;
using MatrixWebApp.Services;

namespace MatrixWebApp.Services
{
    public class ShoppingCartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession Session => _httpContextAccessor.HttpContext.Session;
        private const string CartSessionKey = "Cart";

        public ShoppingCartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ShoppingCart Cart
        {
            get
            {
                var cart = Session.Get<ShoppingCart>(CartSessionKey);
                if (cart == null)
                {
                    cart = new ShoppingCart();
                    Session.Set(CartSessionKey, cart);
                }
                return cart;
            }
            private set
            {
                Session.Set(CartSessionKey, value);
            }
        }

        public void AddProducts(CartItem item)
        {
            var cart = Cart;
            cart.AddItem(item);
            Cart = cart; // opslaan in sessie
        }

        public void UpdateProduct(int productId, int quantity)
        {
            var cart = Cart;
            cart.UpdateQuantity(productId, quantity);
            Cart = cart;
        }

        public void RemoveProduct(int productId)
        {
            var cart = Cart;
            cart.RemoveItem(productId);
            Cart = cart;
        }

        public void ClearCart()
        {
            Cart = new ShoppingCart();
        }
    }
}
