using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using System.IO;
using SimpleJSON;
using System;

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

    [ScriptedImporter(1, "img")]
    public class CastleDBImgImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            // Todo: Should probably make sure there's a match cdb file
            CastleDBGenerator.InitPath(CastleDBConfig.Instance().ImagesFolder);

            TextAsset images = new TextAsset(File.ReadAllText(ctx.assetPath));
            ctx.AddObjectToAsset("main obj", images);
            ctx.SetMainObject(images);

            var values = JSON.Parse(images.text);
            foreach( var img in values)
            {
                Debug.Log(img.Key + " " + img.Value);
                string b64 = img.Value.ToString().Split(',')[1];
                b64 = b64.Trim('"');
                switch (b64.Length % 4)
                {
                    case 2: b64 += "=="; break;
                    case 3: b64 += "="; break;
                }

                byte[] imgbytes = Convert.FromBase64String(b64);

                //Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24 , false);

               // if (!tex.LoadImage(imgbytes))
               // {
                //    Debug.Log("Error loading image: " + img.Key);
               // }

               // tex.Apply();

                File.WriteAllBytes( Application.dataPath + "/" + CastleDBConfig.Instance().ImagesFolder + "/" + img.Key + ".png" , imgbytes);
            }

        }
    }
}