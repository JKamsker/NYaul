# NYaul - .NET Yet Another Utility Library

[![Build and Test](https://github.com/JKamsker/NYaul/actions/workflows/build.yml/badge.svg)](https://github.com/JKamsker/NYaul/actions/workflows/build.yml)
[![NuGet](https://img.shields.io/nuget/v/NYaul.svg)](https://www.nuget.org/packages/NYaul)

NYaul is a .NET utility library designed to simplify common development tasks, providing a collection of helpful extension methods, classes, and utilities. It focuses on I/O operations, reflection, disposable resources, and more.  It's designed to be lightweight, efficient, and easy to use.

## Features

*   **Fluent File and Directory Operations:**
    *   Simplified copying of files and directories (including deep copies and filtering).
    *   Easy creation and management of temporary files and directories (with automatic cleanup).
    *   Extension methods for `FileInfo` and `DirectoryInfo` to check for extensions, traverse parent directories, and more.
    *   `FileEx` and `DirectoryEx` static classes for common operations.

*   **Stream Utilities:**
    *   `ActionStream`: Executes an action on stream disposal.
    *   `TruncateOnDisposeStream`: Truncates a stream to its current position on disposal.
    *   `ProxyStream`: A base class for creating custom stream wrappers.

*   **Disposable Helpers:**
    *   `ActionDisposable`: Executes an action on disposal.
    *   `AsyncActionDisposable`: Executes an asynchronous action on disposal.
    *    Generic versions for actions taking parameters.

*   **Reflection Utilities:**
    *   `DefaultActivator`: Efficiently creates instances of types using optimized activators (prioritizing `CreateDefault` methods, `Default` properties/fields, or parameterless constructors).
    *   `GenericTypeHelper`: Simplifies working with generic types, including extracting generic arguments from complex type hierarchies.

*   **Enum Type Conversion:**
    *   `EnumTypeConverter`: Converts strings to enum values, supporting custom aliases defined via the `EnumAliasAttribute`.

*   **TimeSpan Extensions:**
    *   Allows direct `await` on `TimeSpan` and nullable `TimeSpan` for easy asynchronous delays.

*   **Wait Time Generation:**
     *   `WaitTimeGenerator`:  Generates exponentially increasing wait times for retry logic, with configurable maximum retries, base time, and maximum wait time. Includes a fluent builder.

*   **String Extensions**
    * Provides `Join` methods as polyfills for older .NET versions

*   **File System Abstraction:**
    *   `IFileProvider`: Interface for abstracting file system operations.
    *   `DefaultFileProvider`: Default implementation using `System.IO`.
    *   `VirtualFileProvider`: In-memory file system implementation for testing.

*   **Cross-Platform Compatibility:**
    *   Works with .NET 8.0, .NET Framework 4.8, and .NET Standard 2.0.
    *   Uses polyfills for newer language features when targeting older frameworks.

* **CI/CD**:
    * Full CI/CD is setup with GitHub Actions

## Installation

NYaul is available on NuGet:

```

Install-Package NYaul

```

or

```

dotnet add package NYaul

````

## Usage Examples

**1. Fluent Directory Copy:**

```csharp
using NYaul.IO;

// Copy all .txt and .log files from "sourceDir" to "destDir", including subdirectories.
DirectoryEx.Copy
    .From(@"C:\sourceDir")
    .To(@"C:\destDir")
    .WithFileFilter(file => file.HasExtension(".txt", ".log"))
    .WithSubdirectories() // Or .DeepCopy()
    .Invoke(); // Or .Copy()
````

**2. Temporary File Handling:**

```csharp
using NYaul.IO;

using (var tempFile = TempFile.CreateWithExtension(".txt"))
{
    await tempFile.WriteAllTextAsync("This is some temporary data.");
    string content = await tempFile.ReadAllTextAsync();
    Console.WriteLine(content);
} // tempFile is automatically deleted here.
```

**3.  Asynchronous Delay with TimeSpan:**

```csharp
using System;

// ...
await TimeSpan.FromSeconds(5); // Wait for 5 seconds.
await (TimeSpan?)null;  //  Completes instantly
```

**4.  Default Activator:**

```csharp
using NYaul.Reflection;

public class MyClass
{
    public static MyClass CreateDefault() => new MyClass { Value = 42 };
    public int Value { get; set; }
}

// ...
var instance = DefaultActivator.CreateDefault<MyClass>();
Console.WriteLine(instance.Value); // Output: 42

// Cached delegate for improved performance on subsequent calls:
var activator = DefaultActivator.CreateDefaultActivator<MyClass>();
var instance2 = activator();
```

**5. Enum Conversion with Aliases:**

```csharp
using System.ComponentModel;
using NYaul.TypeConversion;

[TypeConverter(typeof(EnumTypeConverter<MyEnum>))]
public enum MyEnum
{
    [EnumAlias("one", "first")]
    One,
    [EnumAlias("two", "second")]
    Two,
    Three
}

// ...
var converter = new EnumTypeConverter<MyEnum>();
MyEnum value1 = (MyEnum)converter.ConvertFrom("first"); // value1 == MyEnum.One
MyEnum value2 = (MyEnum)converter.ConvertFrom("THREE"); // value2 == MyEnum.Three
```

**6. Exponential Backoff:**

```csharp
using NYaul;

var waitTimeGenerator = WaitTimeGenerator.Create
    .WithMaxRetries(5)
    .WithBaseTime(TimeSpan.FromSeconds(1))
    .WithMaxWaitTime(TimeSpan.FromSeconds(30))
    .Build();

for (int i = 0; i < 10; i++)
{
    TimeSpan? waitTime = waitTimeGenerator.GetWaitTime(i);
    if (waitTime == null)
    {
        Console.WriteLine($"Retry attempt {i}: No more retries.");
        break;
    }
    Console.WriteLine($"Retry attempt {i}: Wait for {waitTime.Value.TotalSeconds} seconds.");
    // await waitTime; // In a real scenario, you'd await here.
}
```

**7. Virtual File System (for Testing):**

```csharp
using NYaul.IO.FileProvider;

var virtualFileSystem = new VirtualFileProvider();
virtualFileSystem.AddFile("test.txt", "Hello, world!");
bool exists = virtualFileSystem.FileExists("test.txt"); // true
string content = virtualFileSystem.ReadAllText("test.txt"); // "Hello, world!"

using (var stream = virtualFileSystem.OpenWrite("test2.txt"))
using (var writer = new StreamWriter(stream))
{
    writer.WriteLine("Line 1");
    writer.WriteLine("Line 2");
}

string content2 = virtualFileSystem.ReadAllText("test2.txt"); // "Line 1\r\nLine 2\r\n"
```

**8. Generic Type Helper**

```csharp
using NYaul.Reflection;
public abstract class Provider<TConfig> where TConfig : ConfigItem { }
public class ConfigItem { }
public class Provider1 : Provider<Config1> { }
public class Config1 : ConfigItem { }

// ...

var result = typeof(Provider1).GetFirstGenericArgumentsOfTypeDefinition(typeof(Provider<>));
// result.GenericArguments[0] == typeof(Config1)
```

## Contributing

Contributions are welcome\! Please feel free to submit pull requests or open issues on the [GitHub repository](https://github.com/JKamsker/NYaul).

## License

This project is licensed under the MIT License - see the [LICENSE.txt](https://www.google.com/url?sa=E&source=gmail&q=LICENSE.txt) file for details.
