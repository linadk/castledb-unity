using UnityEditor;
using UnityEngine;
 
 namespace CastleDBImporter
 {
    public class CastleDBConfig : ScriptableObject
    {
        public string GUIDColumnName;
        public string GeneratedTypesLocation;
        public string GeneratedTypesNamespace;
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