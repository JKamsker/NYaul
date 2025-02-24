using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace NYaul.TypeConversion;

/// <summary>
/// Provides a type converter for enum types that supports string-based conversion with aliases.
/// This converter allows enums to be converted from strings using both their standard names and custom aliases.
/// </summary>
/// <typeparam name="TEnum">The enum type to convert. Must be a struct and an enum.</typeparam>
/// <example>
/// <code>
/// [TypeConverter(typeof(EnumTypeConverter&lt;MyEnum&gt;))]
/// public enum MyEnum
/// {
///     [EnumAlias("one", "first")]
///     One,
///     [EnumAlias("two", "second")]
///     Two
/// }
/// </code>
/// </example>
public class EnumTypeConverter<TEnum> : TypeConverter
    where TEnum : struct, Enum
{
    private Dictionary<string, TEnum>? _enumValues;

    private Lazy<Dictionary<string, TEnum>> _lazyEnumValues = new(LazyInit);

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumTypeConverter{TEnum}"/> class.
    /// Uses lazy initialization for enum value mapping.
    /// </summary>
    public EnumTypeConverter() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumTypeConverter{TEnum}"/> class with predefined enum values.
    /// </summary>
    /// <param name="enumValues">A dictionary mapping string aliases to their corresponding enum values.</param>
    protected EnumTypeConverter(Dictionary<string, TEnum> enumValues)
    {
        _enumValues = enumValues;
    }

    /// <summary>
    /// Returns whether this converter can convert an object of the given type to an enum value.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="sourceType">A <see cref="Type"/> that represents the type to convert from.</param>
    /// <returns>true if the converter can convert from <paramref name="sourceType"/>; otherwise, false.</returns>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string);
    }

    /// <summary>
    /// Converts the given string value to an enum value using both standard enum names and custom aliases.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture.</param>
    /// <param name="value">The object to convert.</param>
    /// <returns>An enum value if the conversion succeeded, or throws an exception if the conversion failed.</returns>
    /// <exception cref="NotSupportedException">Thrown when the conversion is not supported or fails.</exception>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string str)
        {
            return ConvertFromString(context, culture, value, str);
        }

        return base.ConvertFrom(context, culture, value);
    }

    private object? ConvertFromString(ITypeDescriptorContext? context, CultureInfo? culture, object value, string str)
    {
        if (TryConvert(str, out var result))
        {
            return result;
        }

        return base.ConvertFrom(context, culture, value);
    }
    
    public bool TryConvert(string stringValue, out TEnum? result)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if(stringValue == null)
        {
            result = null;
            return false;
        }
        
        if (_enumValues != null && _enumValues.TryGetValue(stringValue, out var res))
        {
            result = res;
            return true;
        }

        if (Enum.TryParse<TEnum>(stringValue, true, out res))
        {
            result = res;
            return true;
        }

        if (_lazyEnumValues.Value.TryGetValue(stringValue, out res))
        {
            result = res;
            return true;
        }

        result = null;
        return false;
    }

    /// <summary>
    /// Attempts to create a lazy-initialized dictionary of enum values from the enum type as a fallback.
    /// </summary>
    /// <returns></returns>
    private static Dictionary<string, TEnum> LazyInit()
    {
        var enumValues = new Dictionary<string, TEnum>(StringComparer.OrdinalIgnoreCase);

        var enumType = typeof(TEnum);
        var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var field in fields)
        {
            var alias = field.GetCustomAttribute<EnumAliasAttribute>();
            if (alias == null)
            {
                continue;
            }

            foreach (var name in alias.Aliases)
            {
                if (field.GetValue(null) is TEnum enumValue)
                {
                    enumValues[name] = enumValue;
                }
            }
        }

        return enumValues;
    }
}

/// <summary>
/// Specifies alternative string representations (aliases) for an enum value.
/// </summary>
/// <remarks>
/// This attribute can be applied to enum members to provide additional string values
/// that can be used when converting strings to enum values using <see cref="EnumTypeConverter{TEnum}"/>.
/// </remarks>
public class EnumAliasAttribute : Attribute
{
    /// <summary>
    /// Gets the collection of aliases for the enum value.
    /// </summary>
    public string[] Aliases { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumAliasAttribute"/> class with the specified aliases.
    /// </summary>
    /// <param name="aliases">One or more string aliases for the enum value.</param>
    public EnumAliasAttribute(params string[] aliases)
    {
        Aliases = aliases;
    }
}
