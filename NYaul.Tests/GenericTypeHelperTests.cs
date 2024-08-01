namespace NYaul.Tests;

public class GenericTypeHelperTests
{
    [Theory]
    [InlineData(typeof(Provider1), typeof(Config1))]
    [InlineData(typeof(Provider2), typeof(Config2))]
    [InlineData(typeof(Provider3), typeof(Config3))]
    [InlineData(typeof(Provider3_1), typeof(Config3))]
    public void TestSimpleGenericArgument(Type concreteType, params Type[] arguments)
    {
        var container = GenericTypeHelper
                .GetGenericArgumentsOfTypeDefinition(concreteType, typeof(Provider<>))
                .Where(x => x.GenericArguments.Length == arguments.Length)
                .FirstOrDefault()
            ;

        Assert.NotNull(container);

        var args = container.GenericArguments;

        Assert.Equal(arguments.Length, args.Length);
        Assert.True(arguments.SequenceEqual(args));
    }

    [Theory]
    [InlineData(typeof(Provider_06), typeof(Config1), typeof(Config2))]
    public void TestScatteredGenericArguments(Type concreteType, params Type[] arguments)
    {
        var containers = GenericTypeHelper
            .GetGenericArgumentsOfTypeDefinition(concreteType, typeof(Provider_04<,>))
            .Where(x => x.GenericArguments.Length == arguments.Length)
            .ToArray();

        var container = containers.FirstOrDefault();

        Assert.NotNull(container);

        var args = container.GenericArguments;
        Assert.Equal(arguments.Length, args.Length);
        Assert.True(arguments.SequenceEqual(args));
    }

    [Theory]
    [InlineData(typeof(Provider_06), typeof(Config1), typeof(Config2))]
    public void TestScatteredGenericArguments_Interface(Type concreteType, params Type[] arguments)
    {
        var containers = GenericTypeHelper
            .GetGenericArgumentsOfTypeDefinition(concreteType, typeof(IProvider_04<,>))
            .Where(x => x.GenericArguments.Length == arguments.Length)
            .ToArray();

        var container = containers.FirstOrDefault();

        Assert.NotNull(container);

        var args = container.GenericArguments;
        Assert.Equal(arguments.Length, args.Length);
        Assert.True(arguments.SequenceEqual(args));
    }

    public interface IProvider_04<TConfig, TConfig1>
    {
    }

    public class Provider_04<TConfig, TConfig1> : IProvider_04<TConfig, TConfig1>
    {
    }

    public class Provider_05<TConfig> : Provider_04<Config1, TConfig>
    {
    }

    public class Provider_06 : Provider_05<Config2>
    {
    }

    public abstract class ErpSyncProvider
    {
    }

    public abstract class Provider<TConfig> : ErpSyncProvider
        where TConfig : ConfigItem
    {
    }

    public class ConfigItem
    {
    }

    public class Provider1 : Provider<Config1>
    {
    }

    public class Provider2 : Provider<Config2>
    {
    }

    public class Provider3 : Provider<Config3>
    {
    }

    public class Provider3_1 : Provider3
    {
    }

    public class Config1 : ConfigItem
    {
    }

    public class Config2 : ConfigItem
    {
    }

    public class Config3 : ConfigItem
    {
    }
}