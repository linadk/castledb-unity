using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            if (config.Databases == null)
            {
                config.Databases = new List<DatabaseConfigInfo>();
            }

            config.UpdateDBs();

            return config;
        }

        public bool CanLoad(string path)
        {
            path = Path.GetFullPath(path.Replace("Assets", Application.dataPath));

            foreach( var db in Databases)
            {
                if (db.loaded)
                {
                    var loadedPath = Path.GetFullPath(db.path);
                    if (loadedPath == path) { return true; }
                }
            }

            return false;
        }

        public static string[] GetAllDBPaths()
        {
            return Directory.GetFiles(Application.dataPath + "/", "*.cdb", SearchOption.AllDirectories);
        }

        public void UpdateDBs()
        {
            var dbpaths = GetAllDBPaths();

            // Remove deleted dbs
            foreach(var db in Databases)
            {
                //Array no longer exists
                if(!Array.Exists( dbpaths , element => element == db.path)){
                    Databases.Remove(db);
                    Debug.Log("Database " + db.path + " removed!");
                }
            }

            // ADD : DB file exists but is in Databases
            foreach( var dbpath in dbpaths)
            {
                if(!Databases.Exists( element=> element.path == dbpath))
                {
                    Databases.Add(new DatabaseConfigInfo(dbpath, false));
                }
            }

        }

        public static void DeleteDirectory(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory);
            }

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
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