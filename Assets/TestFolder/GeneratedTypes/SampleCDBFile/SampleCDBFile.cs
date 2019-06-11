
using UnityEngine;
using CastleDBImporter;
using System.Collections.Generic;
using System;
using CompiledTypes.SampleCDBFile;

namespace CompiledTypes.SampleCDBFile
{
    public class SampleCDBFile
    {
        static CastleDBParser parsedDB;
        public CreaturesType Creatures;
        public ItemsType Items;
        public ModifiersType Modifiers;

        
        public class CreaturesType 
        {
           public Creatures Squid { get { return Get(CompiledTypes.SampleCDBFile.Creatures.RowValues.Squid); } } 
           public Creatures Jellyfish { get { return Get(CompiledTypes.SampleCDBFile.Creatures.RowValues.Jellyfish); } } 
           public Creatures Bear { get { return Get(CompiledTypes.SampleCDBFile.Creatures.RowValues.Bear); } } 
           public Creatures Dragon { get { return Get(CompiledTypes.SampleCDBFile.Creatures.RowValues.Dragon); } } 
           private Creatures Get(Creatures.RowValues line) { return new Creatures(parsedDB.Root, line); }

           public Creatures[] GetAll() 
           {
               var values = (Creatures.RowValues[])Enum.GetValues(typeof(Creatures.RowValues));
               Creatures[] returnList = new Creatures[values.Length];
               for (int i = 0; i < values.Length; i++)
               {
                   returnList[i] = Get(values[i]);
               }
               return returnList;
           }
        } //END OF Creatures 

        public class ItemsType 
        {
           public Items HealingPotion { get { return Get(CompiledTypes.SampleCDBFile.Items.RowValues.HealingPotion); } } 
           public Items PoisonPotion { get { return Get(CompiledTypes.SampleCDBFile.Items.RowValues.PoisonPotion); } } 
           public Items UltraSword { get { return Get(CompiledTypes.SampleCDBFile.Items.RowValues.UltraSword); } } 
           private Items Get(Items.RowValues line) { return new Items(parsedDB.Root, line); }

           public Items[] GetAll() 
           {
               var values = (Items.RowValues[])Enum.GetValues(typeof(Items.RowValues));
               Items[] returnList = new Items[values.Length];
               for (int i = 0; i < values.Length; i++)
               {
                   returnList[i] = Get(values[i]);
               }
               return returnList;
           }
        } //END OF Items 

        public class ModifiersType 
        {
           public Modifiers poison { get { return Get(CompiledTypes.SampleCDBFile.Modifiers.RowValues.poison); } } 
           public Modifiers enchanted { get { return Get(CompiledTypes.SampleCDBFile.Modifiers.RowValues.enchanted); } } 
           private Modifiers Get(Modifiers.RowValues line) { return new Modifiers(parsedDB.Root, line); }

           public Modifiers[] GetAll() 
           {
               var values = (Modifiers.RowValues[])Enum.GetValues(typeof(Modifiers.RowValues));
               Modifiers[] returnList = new Modifiers[values.Length];
               for (int i = 0; i < values.Length; i++)
               {
                   returnList[i] = Get(values[i]);
               }
               return returnList;
           }
        } //END OF Modifiers 



        public SampleCDBFile(TextAsset castleDBAsset)
        {
            parsedDB = new CastleDBParser(castleDBAsset);
            Creatures = new CreaturesType();
            Items = new ItemsType();
            Modifiers = new ModifiersType();

        }
    }
}