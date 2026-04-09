using ETicaretAPI.Application.Validators.Products;
using ETicaretAPI.Application.ViewModels.Products;
using FluentValidation.TestHelper;
using Xunit;

namespace ETicaretAPI.Application.Tests.Validators.Products;

public class CreateProductValidatorTests
{
    private readonly CreateProductValidator _validator = new();

    private static VM_Create_Product ValidModel() => new()
    {
        Name = "Geçerli Ürün Adı",
        Stock = 10,
        Price = 99.90f
    };

    [Fact]
    public void Validate_WithValidModel_PassesValidation()
    {
        // Arrange
        var model = ValidModel();

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("abc")]
    public void Validate_WithInvalidName_HasValidationError(string invalidName)
    {
        // Arrange
        var model = ValidModel();
        model.Name = invalidName;

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(p => p.Name);
    }

    [Fact]
    public void Validate_WithNameLongerThan150Chars_HasValidationError()
    {
        // Arrange
        var model = ValidModel();
        model.Name = new string('A', 151);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(p => p.Name);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_WithNegativeStock_HasValidationError(int negativeStock)
    {
        // Arrange
        var model = ValidModel();
        model.Stock = negativeStock;

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(p => p.Stock);
    }

    [Theory]
    [InlineData(-0.01f)]
    [InlineData(-99f)]
    public void Validate_WithNegativePrice_HasValidationError(float negativePrice)
    {
        // Arrange
        var model = ValidModel();
        model.Price = negativePrice;

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(p => p.Price);
    }
}
