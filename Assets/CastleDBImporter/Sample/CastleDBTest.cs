using UnityEngine;
using System.Collections.Generic;
using System;

#if SAMPLECDBFILE_CDBIMPORT
using CompiledTypes.SampleCDBFile;
#endif

[ExecuteInEditMode]
public class CastleDBTest : MonoBehaviour
{
    public TextAsset CastleDBAsset;
    [SerializeField]
    public Texture dragonTex;
    public bool test;
    void Update()
    {
        if(test)
        {
                #if SAMPLECDBFILE_CDBIMPORT
                var DB = new SampleCDBFile(CastleDBAsset);
                Creatures creature = DB.Creatures.Dragon;
                Debug.Log("[string] name: " + creature.Name);
                Debug.Log("[bool] attacks player: " + creature.attacksPlayer);
                Debug.Log("[int] base damage: " + creature.BaseDamage);
                Debug.Log("[float] damage modifier: " + creature.DamageModifier);
                Debug.Log("[enum] death sound: " + creature.DeathSound);
                Debug.Log("[flag enum] spawn areas: " + creature.Spawn_Areas);
                Debug.Log("[color] color: <color=#" + ColorUtility.ToHtmlStringRGBA(creature.Color) + ">" + creature.Color.ToString() + "</color>");
                Debug.Log("[img] image : " + creature.Icon.name);
                dragonTex = creature.Icon;

                GetComponent<MeshRenderer>().material.SetTexture("_MainTex" , creature.Icon );
                foreach (var item in creature.DropsList)
                {
                    Debug.Log($"{creature.Name} drops item {item.item} at rate {item.DropChance}");
                    foreach (var effect in item.PossibleEffectsList)
                    {
                        Debug.Log($"item has effect {effect.effect} with chase {effect.EffectChance}");
                    }
                }
#endif
        }


        test = false;
        }
    }

