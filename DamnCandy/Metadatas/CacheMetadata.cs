using System;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using System.Text.Json.Serialization;
#endif

namespace DamnCandy.Metadatas
{
    [Serializable]
    public class CacheMetadata
    {
        /// <summary>
        /// Guid of cache
        /// </summary>
#if UNITY_5_3_OR_NEWER
        [field: SerializeField]
#else
        [JsonInclude] 
#endif
        public Guid Guid { get; internal set; }
    
        /// <summary>
        /// CacheId of cache
        /// </summary>
        
#if UNITY_5_3_OR_NEWER
        [field: SerializeField]
#else
        [JsonInclude] 
#endif
        public string CacheId { get; internal set; }
    
        /// <summary>
        /// When cache was created
        /// </summary>
#if UNITY_5_3_OR_NEWER
        [field: SerializeField]
#else
        [JsonInclude] 
#endif
        public DateTime CacheDate { get; internal set; }
    
        /// <summary>
        /// Is cache valid and successfully downloaded
        /// </summary>
#if UNITY_5_3_OR_NEWER
        [field: SerializeField]
#else
        [JsonInclude] 
#endif
        public bool IsValid { get; internal set; }
    
        /// <summary>
        /// Does cache has dependencies
        /// </summary>
#if UNITY_5_3_OR_NEWER
        [field: SerializeField]
#else
        [JsonInclude] 
#endif
        public Guid[] Dependencies { get; internal set; }
    
        /// <summary>
        /// Are dependencies valid
        /// </summary>
#if UNITY_5_3_OR_NEWER
        [field: SerializeField]
#else
        [JsonInclude] 
#endif
        public bool IsDependenciesValid { get; internal set; }
    
        public CacheMetadata() { }
    }
}