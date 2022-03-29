using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Nirast.Pcms.Api.Logger")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Nirast.Pcms.Api.Logger")]
[assembly: AssemblyCopyright("Copyright ©  2018")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("d074a54a-cb45-4b1c-b7ef-bd6c1a1e44db")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: ImplicitDependency(typeof(Serilog.Sinks.File.FileSink))]

/// <summary>
/// Indicates that the marked assembly depends on the type that is specified in the constructor.
/// Typically used to force a compile-time dependency to the assembly that contains the type.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class ImplicitDependencyAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImplicitDependencyAttribute"/> class.
    /// </summary>
    /// <param name="dependencyType">A type from the assembly that is used dynamically.</param>
    public ImplicitDependencyAttribute(Type dependencyType)
    {
        DependencyType = dependencyType;
    }

    /// <summary>
    /// Gets the dependent type reference.
    /// </summary>
    /// <value>The dependent type reference.</value>
    public Type DependencyType { get; private set; }
}
