using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;
using System;
using UnityEditor;
using System.IO;

namespace CastleDBImporter
{
    public class CastleDBParser
    {
        TextAsset DBTextAsset;
        TextAsset DBImages;

        public RootNode Root {get; private set;}
        public RootNode ImageRoot { get; private set; }
        public CastleDBParser(TextAsset db)
        {
            DBTextAsset = db;

            GetImageDBPath();
            //Debug.Log(AssetDatabase.GetAssetPath(DBTextAsset));
            Root = new RootNode(JSON.Parse(DBTextAsset.text));
            
        }

        public void RegenerateDB()
        {
            Root = new RootNode(JSON.Parse(DBTextAsset.text));
        }

        private void GetImageDBPath()
        {
            string dbpath = AssetDatabase.GetAssetPath(DBTextAsset);
            var typeIndex = dbpath.LastIndexOf(".");
            var path = dbpath.Substring(0, typeIndex) + ".img";
            if (File.Exists(path)) { Debug.Log("IT DOES"); }

            DBImages = AssetDatabase.LoadAssetAtPath(path, (typeof(TextAsset))) as TextAsset;
            Debug.Log(path);
            Debug.Log(DBImages.text);
        }

        public class RootNode
        {
            JSONNode value;
            public List<SheetNode> Sheets { get; protected set;}
            public RootNode (JSONNode root)
            {
                value = root;
                Sheets = new List<SheetNode>();
                foreach (KeyValuePair<string, SimpleJSON.JSONNode> item in value["sheets"])
                {
                    Sheets.Add(new SheetNode(item.Value));
                }
            }
            public SheetNode GetSheetWithName(string name)
            {
                foreach (var item in Sheets)
                {
                    if(item.Name == name)
                    {
                        return item;
                    }
                }
                return null;
            }
        }

        public class SheetNode
        {
            JSONNode value;
            public bool NestedType { get; protected set;}
            public string Name { get; protected set; }
            public List<ColumnNode> Columns { get; protected set; }
            public List<SimpleJSON.JSONNode> Rows { get; protected set; }
            public SheetNode(JSONNode sheetValue)
            {
                value = sheetValue;
                string rawName = value["name"];
                //for list types the name can come in as foo@bar@boo
                Char delimit = '@';
                var splitString = rawName.Split(delimit);
                if(splitString.Length <= 1)
                {
                    Name = value["name"];
                    NestedType = false;
                }
                else
                {
                    Name = splitString[splitString.Length - 1];
                    NestedType = true;
                }
                Columns = new List<ColumnNode>();
                Rows = new List<SimpleJSON.JSONNode>();

                foreach (KeyValuePair<string, SimpleJSON.JSONNode> item in value["columns"])
                {
                    Columns.Add(new ColumnNode(item.Value));
                }

                foreach (KeyValuePair<string, SimpleJSON.JSONNode> item in value["lines"])
                {
                    Rows.Add(item.Value);
                }
            }
        }

        public class ColumnNode
        {
            JSONNode value;
            public string TypeStr { get; protected set;}
            public string Name { get; protected set;}
            public string Display { get; protected set;}
            public ColumnNode(JSONNode sheetValue)
            {

                value = sheetValue;
                Name = value["name"];
                if(!CastleDBConfig.NamesHaveSpaces && Name.Contains(" ")) { CastleDBConfig.NamesHaveSpaces = true; }
                Name = Name.Replace(" ", "_"); // Remove spaces to avoid compiler errors
                Display = value["display"];
                TypeStr = value["typeStr"];
                if(!CastleDBConfig.NamesHaveSpaces && Name.Contains(" ")) { CastleDBConfig.NamesHaveSpaces = true; }
                TypeStr = TypeStr.Replace(" ", "_");
            }
        }
    }

}