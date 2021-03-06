using System;
using System.IO;
using System.Linq;
using System.Reflection;

#nullable enable

namespace H.Runners.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public static class ResourcesUtilities
    {
        /// <summary>
        /// Searches for a file among Embedded resources <br/>
        /// Throws an <see cref="ArgumentException"/> if nothing is found or more than one match is found <br/>
        /// <![CDATA[Version: 1.0.0.4]]> <br/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="assembly"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static Stream ReadFileAsStream(string name, Assembly? assembly = null)
        {
            name = name ?? throw new ArgumentNullException(nameof(name));
            assembly ??= Assembly.GetExecutingAssembly();

            try
            {
                return assembly.GetManifestResourceStream(
                           assembly
                               .GetManifestResourceNames()
                               .Single(resourceName => resourceName.EndsWith(name, StringComparison.InvariantCultureIgnoreCase)))
                       ?? throw new ArgumentException($"\"{name}\" is not found in embedded resources");
            }
            catch (InvalidOperationException exception)
            {
                throw new ArgumentException(
                    "Not a single one was found or more than one resource with the given name was found. " +
                    "Make sure there are no collisions and the required file has the attribute \"Embedded resource\"", 
                    exception);
            }
        }
    }
}