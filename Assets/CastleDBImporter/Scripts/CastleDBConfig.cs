using UnityEditor;
using UnityEngine;
 
 namespace CastleDBImporter
 {
    public class CastleDBConfig : ScriptableObject
    {
        [Tooltip("The CastleDB column name that is used to generate variable names/types. If this has spaces they will be replaced with '_'s") ]
        public string GUIDColumnName;

        [Tooltip("The folder that the generated types will be located in.")]
        public string GeneratedTypesLocation;

        [Tooltip("The folder where database images will be stored")]
        public string ImagesFolder;

        [Tooltip("The namespace that the generated types will be wrappe din.")]
        public string GeneratedTypesNamespace;

        [Tooltip("Suppresses import/generation information in the console.")]
        public bool SuppressBuildInfo;

        public static bool NamesHaveSpaces = false;

        public static CastleDBConfig Instance()
        {
            var guids = AssetDatabase.FindAssets("CastleDBConfig t:CastleDBConfig");
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath(path, typeof(CastleDBConfig)) as CastleDBConfig;
        }
    }
 }