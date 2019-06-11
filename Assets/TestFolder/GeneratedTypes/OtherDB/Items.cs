
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
using CompiledTypes.OtherDB;

namespace CompiledTypes.OtherDB
{ 
    public class Items
    {
        public string id;
        public string name;
        public int Weight;

        public enum RowValues {HealingPotion , PoisonPotion , UltraSword }
 
        public Items (CastleDBParser.RootNode root, RowValues line) 
        {
            SimpleJSON.JSONNode node = root.GetSheetWithName("Items").Rows[(int)line];
            id = node["id"];
            name = node["name"];
            Weight = node["Weight"].AsInt;

        }  
        
        public static Items.RowValues GetRowValue(string name)
        {
            var values = (RowValues[])Enum.GetValues(typeof(RowValues));
            for (int i = 0; i < values.Length; i++)
            {
                if(values[i].ToString() == name)
                {
                    return values[i];
                }
            }
            return values[0];
        }
    }
}