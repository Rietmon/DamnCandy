using System;
using System.IO;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace DamnCandy
{
    public static class CacheSettings
    {
        #if UNITY_5_3_OR_NEWER
        /// <summary>
        /// Root directory of application data (default: Application.persistentDataPath
        /// </summary>
        public static string RootPath { get; set; } = Application.persistentDataPath;
        #else
        /// <summary>
        /// Root directory of application data (default: Environment.CurrentDirectory)
        /// </summary>
        public static string RootPath { get; set; } = Environment.CurrentDirectory;
        #endif
    
        /// <summary>
        /// Subdirectory of RootPath where cache data will be stored (default: "__CacheData")
        /// </summary>
        public static string CacheDataFolder { get; set; } = "__CacheData";
    
        /// <summary>
        /// Default binary file extension (default: "bin")
        /// Use without dot (".")
        /// </summary>
        public static string DefaultBinaryFileExtension { get; set; } = "bin";
    
        internal static string CacheDataPath => Path.Combine(RootPath, CacheDataFolder);
    }
}