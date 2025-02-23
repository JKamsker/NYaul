using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace NYaul.Reflection
{
    /// <summary>
    /// Provides methods to create default instances of types using optimized activators.
    /// </summary>
    public static class DefaultActivator
    {
        /// <summary>
        /// Cache to store the compiled delegates for each type <typeparamref name="T"/>.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, object> _defaultFuncs = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// Retrieves the optimized <see cref="Func{T}"/> to create a default instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type for which to create the default instance.</typeparam>
        /// <returns>A <see cref="Func{T}"/> delegate that creates the default instance.</returns>
        public static Func<T> CreateDefaultActivator<T>()
        {
            // Try to get the cached delegate
            if (_defaultFuncs.TryGetValue(typeof(T), out var cachedFunc))
            {
                return (Func<T>)cachedFunc;
            }

            // Build the delegate
            var func = BuildActivateDefaultFunc<T>();

            // Cache the delegate for future use
            _defaultFuncs[typeof(T)] = func;

            return func;
        }

        /// <summary>
        /// Creates a default instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to instantiate.</typeparam>
        /// <returns>A default instance of type <typeparamref name="T"/>.</returns>
        /// <exception cref="MissingMethodException">
        /// Thrown if no suitable constructor, method, property, or field is found to create an instance.
        /// </exception>
        public static T CreateDefault<T>()
        {
            var type = typeof(T);
            var context = new TypeContext(type);

            // 1. Try to use CreateDefault method
            var createDefaultMethod = context.CreateDefaultMethod;
            if 
            (
                createDefaultMethod != null 
                && type.IsAssignableFrom(createDefaultMethod.ReturnType)
                && createDefaultMethod.Invoke(null, null) is T result
            )
            {
                return result;
            }

            // 2. Try to use Default property
            var defaultProperty = context.DefaultProperty;
            if 
            (
                defaultProperty != null 
                && type.IsAssignableFrom(defaultProperty.PropertyType)
                && defaultProperty.GetValue(null) is T result2
            )
            {
                return result2;
            }

            // 3. Try to use Default field
            var defaultField = context.DefaultField;
            if
            (
                defaultField != null 
                && type.IsAssignableFrom(defaultField.FieldType)
                && defaultField.GetValue(null) is T result3
            )
            {
                return result3;
            }

            // 4. Fallback to parameterless constructor or Activator.CreateInstance<T>()
            var parameterlessConstructor = context.ParameterlessConstructor;
            if (parameterlessConstructor != null)
            {
                return (T)parameterlessConstructor.Invoke(null);
            }

            return Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Builds the optimized <see cref="Func{T}"/> delegate based on the <see cref="TypeContext"/>.
        /// </summary>
        /// <typeparam name="T">The type for which to build the delegate.</typeparam>
        /// <returns>A compiled <see cref="Func{T}"/> delegate.</returns>
        private static Func<T> BuildActivateDefaultFunc<T>()
        {
            var type = typeof(T);
            var context = new TypeContext(type);

            // 1. Try to use CreateDefault method
            var createDefaultMethod = context.CreateDefaultMethod;
            if (createDefaultMethod != null && type.IsAssignableFrom(createDefaultMethod.ReturnType))
            {
                // Create a delegate that invokes the static method
                var methodCall = Expression.Call(createDefaultMethod);
                return Expression.Lambda<Func<T>>(methodCall).Compile();
            }

            // 2. Try to use Default property
            var defaultProperty = context.DefaultProperty;
            if (defaultProperty != null && type.IsAssignableFrom(defaultProperty.PropertyType))
            {
                // Create a delegate that accesses the static property
                var propertyAccess = Expression.Property(null, defaultProperty);
                return Expression.Lambda<Func<T>>(propertyAccess).Compile();
            }

            // 3. Try to use Default field
            var defaultField = context.DefaultField;
            if (defaultField != null && type.IsAssignableFrom(defaultField.FieldType))
            {
                // Create a delegate that accesses the static field
                var fieldAccess = Expression.Field(null, defaultField);
                return Expression.Lambda<Func<T>>(fieldAccess).Compile();
            }

            // 4. Fallback to parameterless constructor or Activator.CreateInstance<T>()
            var parameterlessConstructor = context.ParameterlessConstructor;
            if (parameterlessConstructor != null)
            {
                var newExpression = Expression.New(parameterlessConstructor);
                return Expression.Lambda<Func<T>>(newExpression).Compile();
            }

            return () => Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Encapsulates reflection information for a specific type using lazy initialization.
        /// </summary>
        private class TypeContext
        {
            private readonly Type _type;

            // Lazy backing fields for reflection members
            private readonly Lazy<MethodInfo?> _createDefaultMethod;
            private readonly Lazy<PropertyInfo?> _defaultProperty;
            private readonly Lazy<FieldInfo?> _defaultField;
            private readonly Lazy<ConstructorInfo?> _parameterlessConstructor;

            /// <summary>
            /// Initializes a new instance of the <see cref="TypeContext"/> class for the specified type.
            /// </summary>
            /// <param name="type">The type to analyze.</param>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null.</exception>
            public TypeContext(Type type)
            {
                _type = type ?? throw new ArgumentNullException(nameof(type));

                // Initialize Lazy fields with thread-safe publication
                _createDefaultMethod = new Lazy<MethodInfo?>(() =>
                        _type.GetMethod("CreateDefault", BindingFlags.Static | BindingFlags.Public, null, Type.EmptyTypes, null),
                    LazyThreadSafetyMode.ExecutionAndPublication);

                _defaultProperty = new Lazy<PropertyInfo?>(() =>
                        _type.GetProperty("Default", BindingFlags.Static | BindingFlags.Public),
                    LazyThreadSafetyMode.ExecutionAndPublication);

                _defaultField = new Lazy<FieldInfo?>(() =>
                        _type.GetField("Default", BindingFlags.Static | BindingFlags.Public),
                    LazyThreadSafetyMode.ExecutionAndPublication);

                _parameterlessConstructor = new Lazy<ConstructorInfo?>(() =>
                        _type.GetConstructor(Type.EmptyTypes),
                    LazyThreadSafetyMode.ExecutionAndPublication);
            }

            /// <summary>
            /// Gets the <see cref="MethodInfo"/> for the static <c>CreateDefault</c> method, if it exists.
            /// </summary>
            public MethodInfo? CreateDefaultMethod => _createDefaultMethod.Value;

            /// <summary>
            /// Gets the <see cref="PropertyInfo"/> for the static <c>Default</c> property, if it exists.
            /// </summary>
            public PropertyInfo? DefaultProperty => _defaultProperty.Value;

            /// <summary>
            /// Gets the <see cref="FieldInfo"/> for the static <c>Default</c> field, if it exists.
            /// </summary>
            public FieldInfo? DefaultField => _defaultField.Value;

            /// <summary>
            /// Gets the <see cref="ConstructorInfo"/> for the parameterless constructor, if it exists.
            /// </summary>
            public ConstructorInfo? ParameterlessConstructor => _parameterlessConstructor.Value;
        }
    }
}
