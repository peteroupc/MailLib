using System;
using System.Collections.Generic;
using System.Reflection;

namespace MailLibTest {
    /// <summary>Utility methods for accessing internal APIs via
    /// reflection.</summary>
  public static class Reflect {
    private static readonly IDictionary<string, Type> ValueTypeCache = new
      Dictionary<string, Type>();

    // Check every assembly in the AppDomain; by default,
    // Type.GetType looks only in the current assembly.
    private static Type FindType(string type) {
      lock (ValueTypeCache) {
      if (ValueTypeCache.ContainsKey(type)) {
        return ValueTypeCache[type];
      }
      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
        var typeObject = Type.GetType(type + "," + assembly.FullName);
        if (typeObject != null) {
          ValueTypeCache[type] = typeObject;
          return typeObject;
        }
      }
      return null;
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='type'>The parameter <paramref name='type'/> is not
    /// documented yet.</param>
    /// <param name='parameters'>The parameter <paramref
    /// name='parameters'/> is not documented yet.</param>
    /// <returns>An arbitrary object.</returns>
    public static object Construct(string type, params object[] parameters) {
      try {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public |
 BindingFlags.NonPublic | BindingFlags.CreateInstance;
        return Activator.CreateInstance(
  FindType(type),
          flags,
 null,
 parameters,
          null);
      } catch (TargetInvocationException ex) {
        throw ex.InnerException;
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is not
    /// documented yet.</param>
    /// <param name='name'>The parameter <paramref name='name'/> is not
    /// documented yet.</param>
    /// <returns>An arbitrary object.</returns>
    public static object GetMethod(object obj, string name) {
      return obj.GetType().GetMethod(
  name,
  BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
    }

    private static object GetMethodExtended(
  IReflect type,
  string name,
  bool staticMethod,
  int parameterCount) {
      var haveMethodName = false;
      BindingFlags flags = (staticMethod ? BindingFlags.Static :
        BindingFlags.Instance) | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.InvokeMethod;
      foreach (var method in type.GetMethods(flags)) {
        if (method.Name.Equals(name)) {
          haveMethodName = true;
          if (method.GetParameters().Length == parameterCount) {
            return method;
          }
        }
      }
      return haveMethodName ? type.GetMethod(name, flags) : null;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is not
    /// documented yet.</param>
    /// <param name='method'>The parameter <paramref name='method'/> is not
    /// documented yet.</param>
    /// <param name='parameters'>The parameter <paramref
    /// name='parameters'/> is not documented yet.</param>
    /// <returns>An arbitrary object.</returns>
    public static object InvokeMethod(
  object obj,
  object method,
  params object[] parameters) {
      try {
        return ((MethodInfo)method).Invoke(obj, parameters);
      } catch (TargetInvocationException ex) {
        throw ex.InnerException;
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is not
    /// documented yet.</param>
    /// <param name='name'>The parameter <paramref name='name'/> is not
    /// documented yet.</param>
    /// <param name='parameters'>The parameter <paramref
    /// name='parameters'/> is not documented yet.</param>
    /// <returns>An arbitrary object.</returns>
    public static object Invoke(
  object obj,
  string name,
  params object[] parameters) {
      return InvokeMethod(
  obj,
  GetMethodExtended(obj.GetType(), name, false, parameters.Length),
  parameters);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='type'>The parameter <paramref name='type'/> is not
    /// documented yet.</param>
    /// <param name='name'>The parameter <paramref name='name'/> is not
    /// documented yet.</param>
    /// <param name='parameters'>The parameter <paramref
    /// name='parameters'/> is not documented yet.</param>
    /// <returns>An arbitrary object.</returns>
    public static object InvokeStatic(
  string type,
  string name,
  params object[] parameters) {
      return InvokeMethod(
  null,
  GetMethodExtended(FindType(type), name, true, parameters.Length),
  parameters);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is not
    /// documented yet.</param>
    /// <param name='name'>The parameter <paramref name='name'/> is not
    /// documented yet.</param>
    /// <returns>An arbitrary object.</returns>
    public static object GetProperty(object obj, string name) {
  object method = obj.GetType().GetProperty(
  name,
  BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty)
      .GetGetMethod();
      return InvokeMethod(
  obj,
  method);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='type'>The parameter <paramref name='type'/> is not
    /// documented yet.</param>
    /// <param name='name'>The parameter <paramref name='name'/> is not
    /// documented yet.</param>
    /// <returns>An arbitrary object.</returns>
    public static object GetPropertyStatic(string type, string name) {
      return InvokeMethod(
  null,
 FindType(
  type).GetProperty(
  name,
  BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty)
      .GetGetMethod());
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is not
    /// documented yet.</param>
    /// <param name='name'>The parameter <paramref name='name'/> is not
    /// documented yet.</param>
    /// <returns>An arbitrary object.</returns>
    public static object GetField(object obj, string name) {
      BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.GetField;
      return obj.GetType().GetField(
  name,
  flags).GetValue(obj);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='type'>The parameter <paramref name='type'/> is not
    /// documented yet.</param>
    /// <param name='name'>The parameter <paramref name='name'/> is not
    /// documented yet.</param>
    /// <returns>An arbitrary object.</returns>
    public static object GetFieldStatic(string type, string name) {
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
          BindingFlags.Instance | BindingFlags.GetField;
       return FindType(
  type).GetField(
  name,
  flags).GetValue(null);
    }
  }
}
