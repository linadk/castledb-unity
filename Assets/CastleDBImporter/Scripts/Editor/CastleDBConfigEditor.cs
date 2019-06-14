using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace CastleDBImporter
{
    [CustomEditor(typeof(CastleDBConfig))]
    class CastleDBConfigEditor : Editor
    {
        CastleDBConfig comp;

        public void OnEnable()
        {
            comp = (CastleDBConfig)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            var dbs = GetDatabases();
            EditorGUILayout.BeginFoldoutHeaderGroup(true, "Databases");

            foreach( var db in dbs)
            {
                Rect r = EditorGUILayout.BeginHorizontal("Button");
                EditorGUILayout.Toggle(true);
                EditorGUILayout.LabelField(db.Replace(Application.dataPath +"/" , ""));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            // Make sure we save!
            EditorUtility.SetDirty(comp);
        }

        public string[] GetDatabases()
        {
            var dbs = new Dictionary<string, string>();

            return Directory.GetFiles(Application.dataPath + "/", "*.cdb" , SearchOption.AllDirectories);
        }

        public GUIStyle GetDBLabelStyle(string db)
        {
            GUIStyle s = new GUIStyle(EditorStyles.textField);
            s.normal.textColor = Color.green;

            if (AssetDatabase.AssetPathToGUID(db.Replace(Application.dataPath , "Assets")) != "")
            {
                s.normal.textColor = Color.green;
            }
            else
            {
                s.normal.textColor = Color.red;
            }

            return s;
        }
    }
}
