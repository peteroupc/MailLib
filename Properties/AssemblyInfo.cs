#region Using directives

using System;
using System.Reflection;
using System.Runtime.InteropServices;

#endregion

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("MailLib")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: CLSCompliant(true)]
[assembly: AssemblyCompany("")]
[assembly: AssemblyCopyright("Written by Peter O. in 2014. Any copyright is dedicated to the Public Domain. <http://creativecommons.org/publicdomain/zero/1.0/>")]
[assembly: AssemblyProduct("MailLib")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
// The assembly version has following format :
//
// Major.Minor.Build.Revision
//
// You can specify all the values or you can use the default the Revision and
// Build Numbers by using the '*' as shown below:
[assembly: AssemblyVersion("0.5.*")]
#if DEBUG
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("MailLibTest")]
#endif
