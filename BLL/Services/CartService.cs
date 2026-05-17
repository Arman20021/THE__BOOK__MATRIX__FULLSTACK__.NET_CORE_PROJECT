using BLL.DTOs;
using DAL.EF.Tables;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class CartService
    {
        CartRepository cartRepository;

        public CartService(CartRepository cartRepository)
        {
            this.cartRepository = cartRepository;
        }

        public ResultDto AddToCart(int userId, int bookId, int quantity)
        {
            if (quantity <= 0)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "Quantity must be at least 1."
                };
            }

            var book = cartRepository.GetBookById(bookId);

            if (book == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "Book not found."
                };
            }

            if (book.IsActive == false)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "This book is not available."
                };
            }

            if (book.Quantity <= 0)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "This book is out of stock."
                };
            }

            if (quantity > book.Quantity)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "Selected quantity is more than available stock."
                };
            }

            var cart = cartRepository.GetActiveCartByUserId(userId);

            if (cart == null)
            {
                cart = new Cart();

                cart.UserId = userId;
                cart.CartStatus = "Active";
                cart.CreatedAt = DateTime.Now;
                cart.UpdatedAt = DateTime.Now;

                cartRepository.AddCart(cart);
            }

            var cartItem = cartRepository.GetCartItem(cart.CartId, bookId);

            if (cartItem == null)
            {
                CartItem item = new CartItem();

                item.CartId = cart.CartId;
                item.BookId = bookId;
                item.Quantity = quantity;
                item.UnitPrice = book.Price;
                item.LineTotal = book.Price * quantity;

                cartRepository.AddCartItem(item);
            }
            else
            {
                int newQuantity = cartItem.Quantity + quantity;

                if (newQuantity > book.Quantity)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Message = "You cannot add more than available stock."
                    };
                }

                cartItem.Quantity = newQuantity;
                cartItem.UnitPrice = book.Price;
                cartItem.LineTotal = cartItem.UnitPrice * cartItem.Quantity;

                cartRepository.UpdateCartItem(cartItem);
            }

            return new ResultDto
            {
                IsSuccess = true,
                Message = "Book added to cart successfully."
            };
        }

        public CartDto GetCartByUserId(int userId)
        {
            CartDto cartDto = new CartDto();

            var cart = cartRepository.GetActiveCartByUserId(userId);

            if (cart == null)
            {
                return cartDto;
            }

            cartDto.CartId = cart.CartId;

            var items = cartRepository.GetCartItemsByCartId(cart.CartId);

            foreach (var item in items)
            {
                var book = cartRepository.GetBookById(item.BookId);

                CartItemDto dto = new CartItemDto();

                dto.CartItemId = item.CartItemId;
                dto.BookId = item.BookId;
                dto.Quantity = item.Quantity;
                dto.UnitPrice = item.UnitPrice;
                dto.LineTotal = item.LineTotal;

                if (book != null)
                {
                    dto.Title = book.Title;
                    dto.ImagePath = book.ImagePath;
                }

                cartDto.Items.Add(dto);
                cartDto.GrandTotal = cartDto.GrandTotal + item.LineTotal;
            }

            return cartDto;
        }

        public ResultDto UpdateQuantity(int cartItemId, int quantity)
        {
            if (quantity <= 0)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "Quantity must be at least 1."
                };
            }

            var cartItem = cartRepository.GetCartItemById(cartItemId);

            if (cartItem == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "Cart item not found."
                };
            }

            var book = cartRepository.GetBookById(cartItem.BookId);

            if (book == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "Book not found."
                };
            }

            if (quantity > book.Quantity)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "Quantity cannot be more than available stock."
                };
            }

            cartItem.Quantity = quantity;
            cartItem.UnitPrice = book.Price;
            cartItem.LineTotal = book.Price * quantity;

            cartRepository.UpdateCartItem(cartItem);

            return new ResultDto
            {
                IsSuccess = true,
                Message = "Cart quantity updated successfully."
            };
        }

        public void RemoveItem(int cartItemId)
        {
            cartRepository.RemoveCartItem(cartItemId);
        }
    }
}
