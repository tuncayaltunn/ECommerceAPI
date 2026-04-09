using System;
using ETicaretAPI.Application.ViewModels.Products;
using FluentValidation;

namespace ETicaretAPI.Application.Validators.Products
{
	public class CreateProductValidator : AbstractValidator<VM_Create_Product>
	{
		public CreateProductValidator()
		{
			RuleFor(p => p.Name)
				.NotEmpty()
                .WithMessage("Lütfen bir ürün adını boş geçmeyiniz.")
                .NotNull()
				.WithMessage("Lütfen bir ürün adını boş geçmeyiniz.")
				.MaximumLength(150)
				.MinimumLength(5)
				.WithMessage("Lütfen ürün adını 5 ile 150 karakter arasında giriniz");

			RuleFor(p => p.Stock)
				.NotEmpty()
                .WithMessage("Lütfen stok bilgisini boş geçmeyiniz.")
                .NotNull()
				.WithMessage("Lütfen stok bilgisini boş geçmeyiniz")
				.Must(s => s >= 0)
				.WithMessage("Stok bilgisi negatif olamaz");

            RuleFor(p => p.Price)
				.NotEmpty()
                .WithMessage("Lütfen fiyat bilgisini boş geçmeyiniz.")
                .NotNull()
				.WithMessage("Lütfen fiyat bilgisini boş geçmeyiniz")
				.Must(s => s >= 0)
				.WithMessage("Fiyat bilgisi negatif olamaz");
        }
	}
}

