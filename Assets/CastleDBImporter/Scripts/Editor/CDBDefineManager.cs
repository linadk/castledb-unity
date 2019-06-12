using System;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace CastleDBImporter
{
    /// <summary>
    /// This class basically makes sure that our defines are in order at any given time so that we can have conditional compilation of database code using. #if DBNAME_CDBIMPORT.
    /// </summary>
    public static class CDBDefineManager
    {
        // On successful recompilation of scripts
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            RefreshDefines();
        }

        /// <summary>
        /// Rebuilds defines based on what dbs are currently built. This allows for conditional compilation directives in the sample, so the sample doesn't cause compile errors, but is probably useful elsewhere.
        /// </summary>
        public static void RefreshDefines()
        {
            // Get all defines
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = definesString.Split(';').ToList();

            // Delete all defines that are CDBIMPORT tags.
            allDefines.RemoveAll(u => u.EndsWith("_CDBIMPORT"));

            // Get all db dirs
            var path = Application.dataPath + Path.DirectorySeparatorChar + CastleDBConfig.Instance().GeneratedTypesLocation;

            // If our dir exists, add import defines
            if (Directory.Exists(path))
            {
                var info = new DirectoryInfo(path);
                var directories = info.GetDirectories();

                // Add each db we currently have imported
                foreach (var d in directories)
                {
                    allDefines.Add(d.Name.ToUpper() + "_CDBIMPORT");
                }
            }

            // Replace our define string
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));
        }
    }

    /// <summary>
    /// Refresh our defines before a build.
    /// </summary>
    class CDBDefineBuildPreprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }

        // Before we run a build
        public void OnPreprocessBuild(BuildReport report)
        {
            CDBDefineManager.RefreshDefines();
        }
    }

    [UnityEditor.InitializeOnLoad]
    public class CDBDefineCompileListener
    {
        // Runs on editor start
        static CDBDefineCompileListener()
        {
            CDBDefineManager.RefreshDefines();
            Application.logMessageReceived += Application_logMessageReceived;
            Application.logMessageReceivedThreaded += Application_logMessageReceived;
        }

        ~CDBDefineCompileListener()
        {
            Application.logMessageReceived -= Application_logMessageReceived;
            Application.logMessageReceivedThreaded -= Application_logMessageReceived;
        }

        // Runs on error log - We do this because sometimes compiler errors can stop the import from working and we want to catch this as soon as possible.
        private static void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Error)
            {
                CDBDefineManager.RefreshDefines();
            }
        }
    }
}
