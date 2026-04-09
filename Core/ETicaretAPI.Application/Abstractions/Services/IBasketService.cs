
using System;
using ETicaretAPI.Application.ViewModels.Baskets;
using ETicaretAPI.Domain.Entities;

namespace ETicaretAPI.Application.Abstractions.Services
{
	public interface IBasketService
	{
		Task<List<BasketItem>> GetBasketItemsAsync();
		Task AddItemToBasketAsync(VM_Create_BasketItem basketItem);
		Task UpdateQuantityAsync(VM_Update_BasketItem basketItem);
		Task RemoveBasketItemAsync(string basketItemId);
		Basket? GetUserActiveBasket { get; }
	}
}

