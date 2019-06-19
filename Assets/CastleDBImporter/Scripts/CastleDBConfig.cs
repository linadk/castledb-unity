using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CastleDBImporter
{
    public class CastleDBConfig : ScriptableObject
    {
        [Tooltip("The CastleDB column name that is used to generate variable names/types. If this has spaces they will be replaced with '_'s")]
        public string GUIDColumnName;

        [Tooltip("The folder that the generated types will be located in.")]
        public string GeneratedTypesLocation;

        [Tooltip("The folder where database images will be stored")]
        public string ImagesFolder;

        [Tooltip("The namespace that the generated types will be wrappe din.")]
        public string GeneratedTypesNamespace;

        [Tooltip("Suppresses import/generation information in the console.")]
        public bool SuppressBuildInfo;

        [Tooltip("Holds data about what databases in the assets folder are loaded.")]
        public List<DatabaseConfigInfo> Databases;

        public static bool NamesHaveSpaces = false;

        public static CastleDBConfig Instance()
        {
            var guids = AssetDatabase.FindAssets("CastleDBConfig t:CastleDBConfig");
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            var config = AssetDatabase.LoadAssetAtPath(path, typeof(CastleDBConfig)) as CastleDBConfig;

            // Load our db if we haven't
            if(config.Databases == null || config.Databases.Count < 1)
            {
                config.Databases = new List<DatabaseConfigInfo>();
                var dbpaths = GetAllDBPaths();
                foreach(var dbpath in dbpaths)
                {
                    config.Databases.Add(new DatabaseConfigInfo(dbpath, false));
                }
            }

            return config;

        }

        public static string[] GetAllDBPaths()
        {
            return Directory.GetFiles(Application.dataPath + "/", "*.cdb", SearchOption.AllDirectories);
        }
    }

    [System.Serializable]
    public class DatabaseConfigInfo
    {
        [SerializeField]
        public string path;

        [SerializeField]
        public bool loaded;

        public DatabaseConfigInfo( string dbpath , bool isloaded)
        {
            path = dbpath;
            loaded = isloaded;
        }
    }
 }