using System.ComponentModel;
using FluentAssertions;
using NYaul.TypeConversion;

namespace NYaul.Tests.TypeConversion;

public class EnumTypeConverterTests
{
    private readonly EnumTypeConverter<TestEnum> _converter;

    [TypeConverter(typeof(EnumTypeConverter<TestEnum>))]
    public enum TestEnum
    {
        [EnumAlias("one", "first")] One,

        [EnumAlias("two", "second")] Two,

        Three // No alias
    }


    public EnumTypeConverterTests()
    {
        _converter = (EnumTypeConverter<TestEnum>)TypeDescriptor.GetConverter(typeof(TestEnum));
        
        var a  = TypeDescriptor.GetConverter(typeof(TestEnum)).TryConvert("two", out TestEnum converted);
    }

    [Theory]
    [InlineData("one", TestEnum.One)]
    [InlineData("ONE", TestEnum.One)]
    [InlineData("first", TestEnum.One)]
    [InlineData("FIRST", TestEnum.One)]
    [InlineData("two", TestEnum.Two)]
    [InlineData("second", TestEnum.Two)]
    [InlineData("Three", TestEnum.Three)]
    [InlineData("THREE", TestEnum.Three)]
    public void ConvertFrom_ValidStrings_ShouldReturnCorrectEnum(string input, TestEnum expected)
    {
        // Act
        var result = _converter.ConvertFrom(input);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("")]
    [InlineData(null)]
    public void ConvertFrom_InvalidStrings_ShouldThrowException(string input)
    {
        // Act
        var action = () => _converter.ConvertFrom(input);

        // Assert
        action.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void CanConvertFrom_String_ShouldReturnTrue()
    {
        // Act
        var result = _converter.CanConvertFrom(typeof(string));

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(double))]
    public void CanConvertFrom_NonStringTypes_ShouldReturnFalse(Type sourceType)
    {
        // Act
        var result = _converter.CanConvertFrom(sourceType);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("one", TestEnum.One)]
    [InlineData("ONE", TestEnum.One)]
    [InlineData("first", TestEnum.One)]
    [InlineData("FIRST", TestEnum.One)]
    [InlineData("two", TestEnum.Two)]
    [InlineData("second", TestEnum.Two)]
    [InlineData("Three", TestEnum.Three)]
    [InlineData("THREE", TestEnum.Three)]
    public void TryConvert_ValidStrings_ShouldReturnTrueAndCorrectEnum(string input, TestEnum expected)
    {
        // Act
        var result = _converter.TryConvert(input, out var converted);

        // Assert
        result.Should().BeTrue();
        converted.Should().Be(expected);
    }
    
    [Theory]
    [InlineData("invalid")]
    [InlineData("")]
    [InlineData(null)]
    public void TryConvert_InvalidStrings_ShouldReturnFalse(string input)
    {
        // Act
        var result = _converter.TryConvert(input, out _);

        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void TryConvert_InvalidStrings_ShouldReturnFalseAndDefaultEnum()
    {
        // Act
        var result = _converter.TryConvert("invalid", out var converted);

        // Assert
        result.Should().BeFalse();
        converted.Should().Be(null);
    }
}