using UnityEngine;
using UnityEditor;
using System.IO;

public class ChangeTextureSize
{
    [MenuItem("Assets/Change Texture Sizes")]
    private static void ChangeTexturesSizes()
    {

        for (char c = 'A'; c <= 'Z'; c++)
        {
            string[] aFilePaths = Directory.GetFiles(@"G:\UnityProjects\HeccClubDescribeGuess\HeccClubTest\Assets\DescribeAndGuess" + @"\" + c);
            foreach(string s in aFilePaths)
            {
                if(Path.GetExtension(s) == ".jpg" || Path.GetExtension(s) == ".png" || Path.GetExtension(s) == ".gif")
                {
                    Debug.Log("TEST");
                    TextureImporter ti = new TextureImporter();
                    TextureImporterSettings tis = new TextureImporterSettings();
                    ti.ReadTextureSettings(tis);
                    ti.maxTextureSize = 64;
                    ti.SetTextureSettings(tis);
                    AssetDatabase.WriteImportSettingsIfDirty(s);
                    AssetDatabase.ImportAsset(s, ImportAssetOptions.ForceUpdate);
                }
            }
        }
            
    }
}