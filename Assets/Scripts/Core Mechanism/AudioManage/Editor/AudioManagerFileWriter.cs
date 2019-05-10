using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace AudioManaging {
    public static class AudioManagerFileWriter
    {
        public static void WriteEnumFile(string filePath, string[] enumList)
        {
            string copyPath = Application.dataPath + filePath + "/AudioEnum.cs";
            Debug.Log("Creating AudioEnum: " + copyPath);

            using (StreamWriter outfile =
                new StreamWriter(copyPath, false))
            {

                outfile.WriteLine("namespace AudioManaging");
                outfile.WriteLine("{");

                outfile.WriteLine("    public enum AudioEnum");
                outfile.WriteLine("    { ");
                for (int i = 0; i < enumList.Length; i++)
                {
                    if (i != enumList.Length - 1)
                        outfile.WriteLine("        " + enumList[i] + ",");
                    else
                        outfile.WriteLine("        " + enumList[i]);
                }
                outfile.WriteLine("    }");
                outfile.WriteLine("}");
            }
        }
        public static bool IsFileCheckedOut(string filePath)
        {
            string copyPath = "Assets" + filePath + "/AudioEnum.cs";
            UnityEditor.VersionControl.Asset asset = UnityEditor.VersionControl.Provider.GetAssetByPath(copyPath);
            return UnityEditor.VersionControl.Provider.isActive && UnityEditor.VersionControl.Provider.IsOpenForEdit(asset);
        }
    }
}
