using System.Reflection;
using FluentAssertions;
using NYaul.Reflection;

namespace NYaul.Tests;
// Assuming DefaultActivator is in the DefaultActivatorNamespace

public class DefaultActivatorTests
{
    #region Test Classes

    // 1. Type with static CreateDefault method
    public class TypeWithCreateDefault
    {
        public int Value { get; set; }

        public static TypeWithCreateDefault CreateDefault()
        {
            return new TypeWithCreateDefault { Value = 10 };
        }
    }

    // 2. Type with static Default property
    public class TypeWithDefaultProperty
    {
        public int Value { get; set; }

        public static TypeWithDefaultProperty Default => new TypeWithDefaultProperty { Value = 20 };
    }

    // 3. Type with static readonly Default field
    public class TypeWithDefaultField
    {
        public int Value { get; set; }

        public static readonly TypeWithDefaultField Default = new TypeWithDefaultField { Value = 30 };
    }

    // 4. Type with parameterless constructor
    public class TypeWithParameterlessConstructor
    {
        public int Value { get; set; }

        public TypeWithParameterlessConstructor()
        {
            Value = 40;
        }
    }

    // 5. Type without parameterless constructor
    public class TypeWithoutParameterlessConstructor
    {
        public int Value { get; set; }

        public TypeWithoutParameterlessConstructor(int value)
        {
            Value = value;
        }
    }

    // 6. Type with multiple default providers (CreateDefault and Default property)
    public class TypeWithMultipleDefaults
    {
        public int Value { get; set; }

        public static TypeWithMultipleDefaults CreateDefault()
        {
            return new TypeWithMultipleDefaults { Value = 50 };
        }

        public static TypeWithMultipleDefaults Default => new TypeWithMultipleDefaults { Value = 60 };
    }

    // 7. Type with incompatible CreateDefault method (wrong return type)
    public class TypeWithIncompatibleCreateDefault
    {
        public int Value { get; set; }

        public static string CreateDefault()
        {
            return "Not the correct type";
        }
    }

    // 8. Type with no default mechanisms and no parameterless constructor
    public class TypeWithNoDefaultMechanism
    {
        public int Value { get; set; }

        private TypeWithNoDefaultMechanism()
        {
            Value = 70;
        }

        public TypeWithNoDefaultMechanism(int value)
        {
            Value = value;
        }
    }
    
    private void ActivateAndAssert<T>(Action<Func<T>> funcIn)
    {
        funcIn(DefaultActivator.CreateDefaultActivator<T>());
        funcIn(() => DefaultActivator.CreateDefault<T>());
    }


    #endregion

    #region Unit Tests

    [Fact]
    public void ActivateDefault_WithCreateDefaultMethod_ShouldUseCreateDefault()
    {
        ActivateAndAssert<TypeWithCreateDefault>(func =>
        {
            // Act
            var instance = func();

            // Assert
            instance.Should().NotBeNull();
            instance.Value.Should().Be(10);
        });
    }

    [Fact]
    public void ActivateDefault_WithDefaultProperty_ShouldUseDefaultProperty()
    {
        ActivateAndAssert<TypeWithDefaultProperty>(func =>
        {
            // Act
            var instance = func();

            // Assert
            instance.Should().NotBeNull();
            instance.Value.Should().Be(20);
        });
    }

    [Fact]
    public void ActivateDefault_WithDefaultField_ShouldUseDefaultField()
    {
        ActivateAndAssert<TypeWithDefaultField>(func =>
        {
            // Act
            var instance = func();

            // Assert
            instance.Should().NotBeNull();
            instance.Value.Should().Be(30);
        });
    }

    [Fact]
    public void ActivateDefault_WithParameterlessConstructor_ShouldUseConstructor()
    {
        ActivateAndAssert<TypeWithParameterlessConstructor>(func =>
        {
            // Act
            var instance = func();

            // Assert
            instance.Should().NotBeNull();
            instance.Value.Should().Be(40);
        });
    }

    [Fact]
    public void ActivateDefault_WithoutParameterlessConstructor_ShouldUseActivator()
    {
        ActivateAndAssert<TypeWithoutParameterlessConstructor>(func =>
        {
            // Act
            Action act = () => func();

            // Assert
            act.Should().Throw<MissingMethodException>();
        });
    }

    [Fact]
    public void ActivateDefault_WithMultipleDefaults_ShouldPrioritizeCreateDefault()
    {
        ActivateAndAssert<TypeWithMultipleDefaults>(func =>
        {
            // Act
            var instance = func();

            // Assert
            instance.Should().NotBeNull();
            instance.Value.Should().Be(50); // Should use CreateDefault, not Default property
        });
    }

    [Fact]
    public void ActivateDefault_WithIncompatibleCreateDefault_ShouldFallback()
    {
        ActivateAndAssert<TypeWithIncompatibleCreateDefault>(func =>
        {
            // Act
            var instance = func();

            // Assert
            // Since CreateDefault has incompatible return type, it should fallback to parameterless constructor
            instance.Should().NotBeNull();
            instance.Value.Should().Be(0); // Default int value
        });
    }

    [Fact]
    public void ActivateDefault_WithNoDefaultMechanismAndNoParameterlessConstructor_ShouldUseActivator()
    {
        ActivateAndAssert<TypeWithNoDefaultMechanism>(func =>
        {
            // Act
            Action act = () => func();

            // Assert
            // Since there's no parameterless constructor, Activator.CreateInstance<T>() should be used
            // However, Activator.CreateInstance<T>() requires a parameterless constructor, so it should throw.
            act.Should().Throw<MissingMethodException>();
        });
    }

    [Fact]
    public void ActivateDefault_ShouldCacheDelegates()
    {
        // Arrange
        var func1 = DefaultActivator.CreateDefaultActivator<TypeWithCreateDefault>();
        var func2 = DefaultActivator.CreateDefaultActivator<TypeWithCreateDefault>();
        
        // Act & Assert
        func1.Should().BeSameAs(func2); // The cached delegate should be the same instance
    }

    [Fact]
    public void ActivateDefault_ShouldReturnDifferentInstances()
    {
        // Arrange
        ActivateAndAssert<TypeWithCreateDefault>(func =>
        {
            // Act
            var instance1 = func();
            var instance2 = func();

            // Assert
            instance1.Should().NotBeSameAs(instance2);
            instance1.Value.Should().Be(10);
            instance2.Value.Should().Be(10);
        });

    }

   
    #endregion
}