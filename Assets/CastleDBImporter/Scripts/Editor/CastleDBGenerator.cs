using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;


namespace CastleDBImporter
{
    public class CastleDBGenerator
    {
        public static void GenerateTypes(CastleDBParser.RootNode root, CastleDBConfig configFile)
        {
            // Create scripts
            List<string> scripts = new List<string>();
            CastleDBConfig config = configFile;
            String FieldIndent = "        ";

            InitTypePath(config);

            foreach (CastleDBParser.SheetNode sheet in root.Sheets)
            {
                string scriptPath = $"Assets/{config.GeneratedTypesLocation}/{sheet.Name}.cs";
                scripts.Add(scriptPath);

                //generate fields
                string fieldText = "";
                for (int i = 0; i < sheet.Columns.Count; i++)
                {
                    fieldText += FieldIndent;
                    CastleDBParser.ColumnNode column = sheet.Columns[i];
                    string fieldType = CastleDBUtils.GetTypeFromCastleDBColumn(column);
                    if(fieldType != "Enum") //non-enum, normal field
                    {
                        if(CastleDBUtils.GetTypeNumFromCastleDBTypeString(column.TypeStr) == "8")
                        {
                            fieldText += ($"public List<{fieldType}> {column.Name}List = new List<{fieldType}>();");
                        }
                        else
                        {
                            fieldText += ($"public {fieldType} {column.Name};");
                        }
                    }
                    else //enum type
                    {
                        string[] enumValueNames = CastleDBUtils.GetEnumValuesFromTypeString(column.TypeStr);
                        string enumEntries = "";
                        if(CastleDBUtils.GetTypeNumFromCastleDBTypeString(column.TypeStr) == "10") //flag
                        {
                            fieldText += ($"public {column.Name}Flag {column.Name};");
                            for (int val = 0; val < enumValueNames.Length; val++)
                            {
                                enumEntries += (enumValueNames[val] + " = " + (int)Math.Pow(2, val));
                                if(val + 1 < enumValueNames.Length) {enumEntries += ",";};
                            }
                            fieldText += ($"[FlagsAttribute] public enum {column.Name}Flag {{ {enumEntries} }}");
                        }
                        else
                        {
                            fieldText += ($"public {column.Name}Enum {column.Name};");
                            for (int val = 0; val < enumValueNames.Length; val++)
                            {
                                enumEntries += (enumValueNames[val] + " = " + val);
                                if(val + 1 < enumValueNames.Length) {enumEntries += ",";};
                            }
                            fieldText += ($"public enum {column.Name}Enum {{  {enumEntries} }}");
                        }
                    }

                    fieldText += "\r\n";

                }

                #region("Row Values Field")
                string possibleValuesText = "";
                if (!sheet.NestedType)
                {
                    possibleValuesText += $"public enum RowValues {{";

                    for (int i = 0; i < sheet.Rows.Count; i++)
                    {
                        string rowName = sheet.Rows[i][config.GUIDColumnName];
                        possibleValuesText += rowName;
                        if (i + 1 < sheet.Rows.Count) { possibleValuesText += " , "; }
                    }
                    possibleValuesText += " }\r\n";
                }
                #endregion

                //generate the constructor that sets the fields based on the passed in value
                string constructorText = "";
                string constructorIndent = "            ";
                if (!sheet.NestedType)
                {
                    constructorText += $"{constructorIndent}SimpleJSON.JSONNode node = root.GetSheetWithName(\"{sheet.Name}\").Rows[(int)line];\r\n";
                }
                for (int i = 0; i < sheet.Columns.Count; i++)
                {
                    constructorText += constructorIndent;
                    CastleDBParser.ColumnNode column = sheet.Columns[i];
                    string castText = CastleDBUtils.GetCastStringFromCastleDBTypeStr(column.TypeStr);
                    string enumCast = "";
                    string typeNum = CastleDBUtils.GetTypeNumFromCastleDBTypeString(column.TypeStr);
                    if (typeNum == "8")
                    {
                        //list type
                        constructorText += $"foreach(var item in node[\"{column.Name}\"]) {{ {column.Name}List.Add(new {column.Name}(root, item));}}";
                    }
                    else if (typeNum == "6")
                    {
                        //working area:
                        //ref type
                        string refType = CastleDBUtils.GetTypeFromCastleDBColumn(column);
                        //look up the line based on the passed in row
                        constructorText += $"{column.Name} = new {config.GeneratedTypesNamespace}.{refType}(root,{config.GeneratedTypesNamespace}.{refType}.GetRowValue(node[\"{column.Name}\"]));";
                    }
                    else if (typeNum == "7") // Image
                    {
                        constructorText += $"{column.Name} = Resources.Load<Texture>(node[\"{column.Name}\"]) as Texture;";
                    }
                    else if (typeNum == "11") // Color
                    {
                        constructorText += $"{column.Name} = CastleDB.GetColorFromString( node[\"{column.Name}\"]);";
                    }
                    else
                    {
                        if(typeNum == "10")
                        {
                            enumCast = $"({column.Name}Flag)";
                        }
                        else if(typeNum == "5")
                        {
                            enumCast = $"({column.Name}Enum)";
                        }
                        constructorText += $"{column.Name} = {enumCast}node[\"{column.Name}\"]{castText};";
                    }

                    constructorText += "\r\n";
                }



                string getMethodText = "";
                if(!sheet.NestedType)
                {
                    getMethodText += $@"
        public static {sheet.Name}.RowValues GetRowValue(string name)
        {{
            var values = (RowValues[])Enum.GetValues(typeof(RowValues));
            for (int i = 0; i < values.Length; i++)
            {{
                if(values[i].ToString() == name)
                {{
                    return values[i];
                }}
            }}
            return values[0];
        }}";
                }

                string ctor = "";
                if(!sheet.NestedType)
                {
                    ctor = $"public {sheet.Name} (CastleDBParser.RootNode root, RowValues line)";
                }
                else
                {
                    ctor = $"public {sheet.Name} (CastleDBParser.RootNode root, SimpleJSON.JSONNode node)";
                }
                // string usings = "using UnityEngine;\r\n using System;\r\n using System.Collections.Generic;\r\n using SimpleJSON;\r\n using CastleDBImporter;\r\n";
                string fullClassText = $@"
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
namespace {config.GeneratedTypesNamespace}
{{ 
    public class {sheet.Name}
    {{
{fieldText}
        {possibleValuesText} 
        {ctor} 
        {{
{constructorText}
        }}  
        {getMethodText}
    }}
}}";
                if (!config.SuppressBuildInfo)
                {
                    Debug.Log("Generating CDB Class: " + sheet.Name);
                }

                File.WriteAllText(scriptPath, fullClassText);
            }

            //build the CastleDB file
            string cdbscriptPath = $"Assets/{config.GeneratedTypesLocation}/CastleDB.cs";
            scripts.Add(cdbscriptPath);
            //fields
            string cdbfields = "";
            string cdbconstructorBody = "";
            string classTexts = "";
            foreach (CastleDBParser.SheetNode sheet in root.Sheets)
            {
                if(sheet.NestedType){continue;} //only write main types to CastleDB
                cdbfields += $"        public {sheet.Name}Type {sheet.Name};\r\n";
                cdbconstructorBody += $"        {sheet.Name} = new {sheet.Name}Type();\r\n";

                //get a list of all the row names
                classTexts += $"        public class {sheet.Name}Type \r\n        {{\r\n";
                for (int i = 0; i < sheet.Rows.Count; i++)
                {
                    string rowName = sheet.Rows[i][config.GUIDColumnName];
                    classTexts += $"           public {sheet.Name} {rowName} {{ get {{ return Get({config.GeneratedTypesNamespace}.{sheet.Name}.RowValues.{rowName}); }} }} \r\n";
                }

                classTexts += $"           private {sheet.Name} Get({config.GeneratedTypesNamespace}.{sheet.Name}.RowValues line) {{ return new {sheet.Name}(parsedDB.Root, line); }}\r\n";
                classTexts += $@"
           public {sheet.Name}[] GetAll() 
           {{
               var values = ({config.GeneratedTypesNamespace}.{sheet.Name}.RowValues[])Enum.GetValues(typeof({config.GeneratedTypesNamespace}.{sheet.Name}.RowValues));
               {sheet.Name}[] returnList = new {sheet.Name}[values.Length];
               for (int i = 0; i < values.Length; i++)
               {{
                   returnList[i] = Get(values[i]);
               }}
               return returnList;
           }}";
             classTexts += $"\r\n        }} //END OF {sheet.Name} \r\n\r\n";
            }

            string fullCastle = $@"
using UnityEngine;
using CastleDBImporter;
using System.Collections.Generic;
using System;

namespace {config.GeneratedTypesNamespace}
{{
    public class CastleDB
    {{
        static CastleDBParser parsedDB;
{cdbfields}
        
{classTexts}

        public CastleDB(TextAsset castleDBAsset)
        {{
            parsedDB = new CastleDBParser(castleDBAsset);
            {cdbconstructorBody}
        }}

        // Convert CastleDB color string to Unity Color type.
        public static Color GetColorFromString( string color)
        {{
            int.TryParse(color, out int icolor);
            float blue = ((icolor >> 0) & 255) / 255.0f;
            float green = ((icolor >> 8) & 255) / 255.0f;
            float red = ((icolor >> 16) & 255) / 255.0f;
            return new Color(red, green, blue);
        }}
    }}
}}";
            if (!config.SuppressBuildInfo)
            {
                Debug.Log("Generating CastleDB class");

                if (CastleDBConfig.NamesHaveSpaces)
                {
                    Debug.Log("Your CastleDB file contains spaces in names! These spaces will be automatically converted to '_'.");
                }

                Debug.Log("To suppress log messages from import, check the CastleDBConfig flag 'Suppress Import Messages'");
            }

            File.WriteAllText(cdbscriptPath, fullCastle);
            AssetDatabase.Refresh();
        }

        public static void InitTypePath(CastleDBConfig config)
        {
            InitPath(config.GeneratedTypesLocation);
        }

        public static void InitPath(string path)
        {
            var full_path = $"{ Application.dataPath}/{ path }";
            if (Directory.Exists(full_path)){
                var files = Directory.GetFiles(full_path);
                foreach (var file in files)
                {
                    FileUtil.DeleteFileOrDirectory(file);
                }
                AssetDatabase.Refresh();
            }
            else
            {
                Directory.CreateDirectory(full_path);
                AssetDatabase.Refresh();
            }

        }
    }
}