using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: AssemblyTitle("Neumont.Build.VisualStudio.dll")]
[assembly: AssemblyProduct("Neumont Build System")]
[assembly: AssemblyDescription("Neumont Build System - Visual Studio Targets DLL")]

[assembly: Dependency("Microsoft.Build.Framework,", LoadHint.Always)]
[assembly: Dependency("Microsoft.Build.Utilities,", LoadHint.Always)]