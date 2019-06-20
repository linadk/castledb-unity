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
        private static GUIStyle ToggleButtonStyleNormal = null;
        private static GUIStyle ToggleButtonStyleToggled = null;

        public void OnEnable()
        {
            comp = (CastleDBConfig)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // Set up styles
            if (ToggleButtonStyleNormal == null)
            {
                ToggleButtonStyleNormal = "Button";
                ToggleButtonStyleToggled = new GUIStyle(ToggleButtonStyleNormal);
                ToggleButtonStyleToggled.normal.background = ToggleButtonStyleToggled.active.background;
            }

            EditorGUILayout.BeginFoldoutHeaderGroup(true, "Databases");

            var localdb = CastleDBConfig.Instance().Databases;
            for (int i = 0; i < localdb.Count; i++)
            {
                GUI.backgroundColor = localdb[i].loaded ? Color.green : Color.magenta;
                if (GUILayout.Button(localdb[i].path.Replace(Application.dataPath + "/", ""), localdb[i].loaded ? ToggleButtonStyleNormal : ToggleButtonStyleToggled))
                {
                    localdb[i].loaded = !localdb[i].loaded;

                    if (localdb[i].loaded) // LOAD
                    {
                        var assetpath = localdb[i].path.Replace(Application.dataPath, "Assets");

                        AssetDatabase.ImportAsset(assetpath);
                        AssetDatabase.ImportAsset(assetpath.Replace(".cdb", ".img"));
                      
                        AssetDatabase.Refresh();
                    }
                    else // UNLOAD
                    {
                        CastleDBImporter.UndoImport(localdb[i].path);
                        AssetDatabase.Refresh();
                    }
                }

                EditorGUILayout.Space();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            // Make sure we save!
            EditorUtility.SetDirty(comp);
        }
    }
}
