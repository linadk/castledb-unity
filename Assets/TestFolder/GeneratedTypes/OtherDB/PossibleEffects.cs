
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using CastleDBImporter;
using CompiledTypes.OtherDB;

namespace CompiledTypes.OtherDB
{ 
    public class PossibleEffects
    {
        public Modifiers effect;
        public int EffectChance;

         
        public PossibleEffects (CastleDBParser.RootNode root, SimpleJSON.JSONNode node) 
        {
            effect = new Modifiers(root,Modifiers.GetRowValue(node["effect"]));
            EffectChance = node["EffectChance"].AsInt;

        }  
        
    }
}