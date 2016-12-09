#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class TestAssetGUID : MonoBehaviour {

    public Material[] materials;
    public Texture[] textures;
    public Sprite[] sprites;
	// Use this for initialization
	void Start () 
    {
	
	}
	
	void Update () {
	
	}


    /** 首字母小写 */
    public static string FirstLower(string str)
    {
        return char.ToLower(str[0]) + str.Substring(1); 
    }


	[ContextMenu("EchoAll")]
	public void EchoAll()
	{

		StringWriter sw = new StringWriter();
		sw.WriteLine(EchoMaterial());
		sw.WriteLine("\n\n");


		sw.WriteLine(EchoSprite());
		sw.WriteLine("\n\n");

		sw.WriteLine(EchoTexure());
		sw.WriteLine("\n\n");

		Debug.Log(sw.ToString());
	}


	[ContextMenu("EchoMaterial")]
	public StringWriter EchoMaterial()
	{
		StringWriter sw = new StringWriter();
		sw.WriteLine("#region Material");
		sw.WriteLine("string MATERIAL_GUID_NEW = \"{fileID: 2100000, guid: __GUID__, type: 2}\";");
		sw.WriteLine("");

		string[] oldGuids = new string[]{ 
			"{fileID: 10754, guid: 0000000000000000f000000000000000, type: 0}",
			"{fileID: 10304, guid: 0000000000000000f000000000000000, type: 0}",
			"{fileID: 10301, guid: 0000000000000000f000000000000000, type: 0}",
			"{fileID: 10303, guid: 0000000000000000f000000000000000, type: 0}",
			"{fileID: 10302, guid: 0000000000000000f000000000000000, type: 0}",
		};

		for(int i = 0; i < materials.Length; i ++)
		{
			if(materials[i] == null) continue;

			string fieldName = "material_" + (materials[i].name.Replace(" ", "_").Replace("-", "_"));
			sw.WriteLine("Material {0};", fieldName);
		}


		for(int i = 0; i < materials.Length; i ++)
		{
			if(materials[i] == null) continue;

			string fieldName = "material_" + (materials[i].name.Replace(" ", "_").Replace("-", "_"));
			string fieldName_oldUid = fieldName + "_oldUid" ;
			sw.WriteLine("string {0}\t\t\t\t= \"{1}\";", fieldName_oldUid, oldGuids[i]);
		}

		sw.WriteLine("");
		sw.WriteLine("void GUI_MaterialList()");
		sw.WriteLine("{");

		for(int i = 0; i < materials.Length; i ++)
		{
			if(materials[i] == null) continue;

			string fieldName = "material_" + (materials[i].name.Replace(" ", "_").Replace("-", "_"));
			string path = AssetDatabase.GetAssetPath(materials[i]);

			sw.WriteLine("\tif ({0} == null)", fieldName);
			sw.WriteLine("\t{");
			sw.WriteLine("\t\t{0} = AssetDatabase.LoadAssetAtPath<Material>(\"{1}\");", fieldName, path);
			sw.WriteLine("\t}");
		}

		sw.WriteLine("");
		sw.WriteLine("");

		for(int i = 0; i < materials.Length; i ++)
		{
			if(materials[i] == null) continue;

			string fieldName = "material_" + (materials[i].name.Replace(" ", "_").Replace("-", "_"));
			string fieldName_oldUid = fieldName + "_oldUid" ;

			sw.WriteLine("\tGUI_ObjItem<Material>(\"{0}\", {1}, {2}, MATERIAL_GUID_NEW);", materials[i].name, fieldName, fieldName_oldUid);
			if(i < materials.Length - 1) sw.WriteLine("\tGUILayout.Space(20);");
		}


		sw.WriteLine("}");
		sw.WriteLine("#endregion");


		Debug.Log(sw.ToString());
		return sw;
	}


    [ContextMenu("EchoSprite")]
	public StringWriter EchoSprite()
    {
		StringWriter sw = new StringWriter();
		sw.WriteLine("#region Sprite");
		sw.WriteLine("string SPRITE_GUID_NEW = \"{fileID: 21300000, guid: __GUID__, type: 3}\";");
        sw.WriteLine("");

        string[] oldGuids = new string[]{ 
            "{fileID: 10907, guid: 0000000000000000f000000000000000, type: 0}",
            "{fileID: 10901, guid: 0000000000000000f000000000000000, type: 0}",
            "{fileID: 10915, guid: 0000000000000000f000000000000000, type: 0}",
            "{fileID: 10911, guid: 0000000000000000f000000000000000, type: 0}",
            "{fileID: 10913, guid: 0000000000000000f000000000000000, type: 0}",
            "{fileID: 10917, guid: 0000000000000000f000000000000000, type: 0}",
            "{fileID: 10905, guid: 0000000000000000f000000000000000, type: 0}",
        };

		for(int i = 0; i < sprites.Length; i ++)
		{
			if(sprites[i] == null) continue;

			string fieldName = "sprite_" + (sprites[i].name.Replace(" ", "_").Replace("-", "_"));
			sw.WriteLine("Sprite {0};", fieldName);
		}


		for(int i = 0; i < sprites.Length; i ++)
		{
			if(sprites[i] == null) continue;

			string fieldName = "sprite_" + (sprites[i].name.Replace(" ", "_").Replace("-", "_"));
			string fieldName_oldUid = fieldName + "_oldUid" ;
			sw.WriteLine("string {0}\t\t\t\t= \"{1}\";", fieldName_oldUid, oldGuids[i]);
		}

		sw.WriteLine("");
		sw.WriteLine("void GUI_SpriteList()");
		sw.WriteLine("{");

		for(int i = 0; i < sprites.Length; i ++)
		{
			if(sprites[i] == null) continue;

			string fieldName = "sprite_" + (sprites[i].name.Replace(" ", "_").Replace("-", "_"));
			string path = AssetDatabase.GetAssetPath(sprites[i]);

			sw.WriteLine("\tif ({0} == null)", fieldName);
			sw.WriteLine("\t{");
			sw.WriteLine("\t\t{0} = AssetDatabase.LoadAssetAtPath<Sprite>(\"{1}\");", fieldName, path);
			sw.WriteLine("\t}");
		}

		sw.WriteLine("");
		sw.WriteLine("");

		for(int i = 0; i < sprites.Length; i ++)
		{
			if(sprites[i] == null) continue;

			string fieldName = "sprite_" + (sprites[i].name.Replace(" ", "_").Replace("-", "_"));
			string fieldName_oldUid = fieldName + "_oldUid" ;

			sw.WriteLine("\tGUI_ObjItem<Sprite>(\"{0}\", {1}, {2}, SPRITE_GUID_NEW);", sprites[i].name, fieldName, fieldName_oldUid);
			if(i < sprites.Length - 1) sw.WriteLine("\tGUILayout.Space(20);");
		}


		sw.WriteLine("}");
		sw.WriteLine("#endregion");


        Debug.Log(sw.ToString());
		return sw;
    }



	[ContextMenu("EchoTexture")]
	public StringWriter EchoTexure()
	{
		StringWriter sw = new StringWriter();
		sw.WriteLine("#region Texture");
		sw.WriteLine("string TEXTURE_GUID_NEW = \"{fileID: 2800000, guid: __GUID__, type: 3}\";");
		sw.WriteLine("");

		string[] oldGuids = new string[]{ 
			"{fileID: 10906, guid: 0000000000000000f000000000000000, type: 0}",
			"{fileID: 10900, guid: 0000000000000000f000000000000000, type: 0}",
			"{fileID: 10914, guid: 0000000000000000f000000000000000, type: 0}",
			"{fileID: 10910, guid: 0000000000000000f000000000000000, type: 0}",
			"{fileID: 10912, guid: 0000000000000000f000000000000000, type: 0}",
			"{fileID: 10916, guid: 0000000000000000f000000000000000, type: 0}",
			"{fileID: 10904, guid: 0000000000000000f000000000000000, type: 0}",
			"{fileID: 10300, guid: 0000000000000000f000000000000000, type: 0}",
		};

		for(int i = 0; i < textures.Length; i ++)
		{
			if(textures[i] == null) continue;

			string fieldName = "texture_" + (textures[i].name.Replace(" ", "_").Replace("-", "_"));
			sw.WriteLine("Texture {0};", fieldName);
		}


		for(int i = 0; i < textures.Length; i ++)
		{
			if(textures[i] == null) continue;

			string fieldName = "texture_" + (textures[i].name.Replace(" ", "_").Replace("-", "_"));
			string fieldName_oldUid = fieldName + "_oldUid" ;
			sw.WriteLine("string {0}\t\t\t\t= \"{1}\";", fieldName_oldUid, oldGuids[i]);
		}

		sw.WriteLine("");
		sw.WriteLine("void GUI_TextureList()");
		sw.WriteLine("{");

		for(int i = 0; i < textures.Length; i ++)
		{
			if(textures[i] == null) continue;

			string fieldName = "texture_" + (textures[i].name.Replace(" ", "_").Replace("-", "_"));
			string path = AssetDatabase.GetAssetPath(textures[i]);

			sw.WriteLine("\tif ({0} == null)", fieldName);
			sw.WriteLine("\t{");
			sw.WriteLine("\t\t{0} = AssetDatabase.LoadAssetAtPath<Texture>(\"{1}\");", fieldName, path);
			sw.WriteLine("\t}");
		}

		sw.WriteLine("");
		sw.WriteLine("");

		for(int i = 0; i < textures.Length; i ++)
		{
			if(textures[i] == null) continue;

			string fieldName = "texture_" + (textures[i].name.Replace(" ", "_").Replace("-", "_"));
			string fieldName_oldUid = fieldName + "_oldUid" ;

			sw.WriteLine("\tGUI_ObjItem<Texture>(\"{0}\", {1}, {2}, TEXTURE_GUID_NEW);", textures[i].name, fieldName, fieldName_oldUid);
			if(i < textures.Length - 1) sw.WriteLine("\tGUILayout.Space(20);");
		}


		sw.WriteLine("}");
		sw.WriteLine("#endregion");


		Debug.Log(sw.ToString());

		return sw;
	}
}
#endif