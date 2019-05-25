using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using System.IO;
using SimpleJSON;
using System;
using System.Collections.Generic;

namespace CastleDBImporter
{
	[ScriptedImporter(1, "cdb")]
	public class CastleDBImporter : ScriptedImporter
	{
        private CastleDBParser parser = null;

		public override void OnImportAsset(AssetImportContext ctx)
		{
			TextAsset castle = new TextAsset(File.ReadAllText(ctx.assetPath));

			ctx.AddObjectToAsset("main obj", castle);
			ctx.SetMainObject(castle);

			parser = new CastleDBParser(castle);
            EditorApplication.delayCall += new EditorApplication.CallbackFunction(GenerateTypes); // Delay type generation until the asset manager has finished importing
        }

        private void GenerateTypes()
        {
            CastleDBGenerator.GenerateTypes(parser.Root, CastleDBConfig.Instance() );
            parser = null;
        }
    }

}