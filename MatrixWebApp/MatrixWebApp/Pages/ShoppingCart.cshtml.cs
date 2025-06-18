using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MatrixWebApp.Extensions;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MatrixWebApp.Services;
using MatrixWebApp.Models;

namespace MatrixWebApp.Pages
{
    public class ShoppingCartModel : PageModel
    {
        private readonly ShoppingCartService _cartService;

        public ShoppingCartModel(ShoppingCartService cartService)
        { 
            _cartService = cartService;
        }

        public ShoppingCart Cart => _cartService.Cart;

        public IActionResult OnPostUpdateQuantity(int productId, int quantity)
        {
            _cartService.UpdateProduct(productId, quantity);
            return RedirectToPage();
        }

        public IActionResult OnPostRemoveItem(int productId)
        {
            var (removedItem, _) = _cartService.RemoveProduct(productId);

            if (removedItem != null)
            {
                TempData["WarningMessage"] = removedItem.Quantity > 1
                    ? $"{removedItem.Name} (x{removedItem.Quantity}) is verwijderd uit je winkelwagen."
                    : $"{removedItem.Name} is verwijderd uit je winkelwagen.";
            }

            return RedirectToPage();
        }

        public IActionResult OnPostCheckout()
        { 
            return RedirectToPage("Checkout");
        }
    }
}

