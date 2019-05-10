using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;

public class NotificationFileWriter
{
    public static string WriteViewClass(Utility.App app, string templateId, string folderPath)
    {
        // remove whitespace and minus
        string name = templateId + "_" + (int)app + "_" + "NotificationView";
        name = name.Replace(" ", "_");
        name = name.Replace("-", "_");
        string copyPath = Application.dataPath + folderPath + "/ViewScripts/" + name + ".cs";
        Debug.Log("Creating Classfile: " + copyPath);
        string dataTypeName = templateId + "_" + (int)app + "_" + "NotificationData";
        if (File.Exists(copyPath) == false)
        { // do not overwrite
            using (StreamWriter outfile =
                new StreamWriter(copyPath))
            {
                outfile.WriteLine("using UnityEngine;");
                outfile.WriteLine("using System.Collections;");
                outfile.WriteLine("using System.Collections.Generic;");
                outfile.WriteLine("public class " + name + " : NotificationView");
                outfile.WriteLine("{ ");
                outfile.WriteLine(" ");
                outfile.WriteLine("     public override void ResolveNotificationData(NotificationData notifiData)");
                outfile.WriteLine("    {");
                outfile.WriteLine("        " + dataTypeName + " data = (" + dataTypeName + ")notifiData;");
                outfile.WriteLine("    }");
                outfile.WriteLine("}");

            }//File written
        }
        return name;
    }

    public static void DeleteViewClass(Utility.App app, string templateId, string folderPath)
    {
        // remove whitespace and minus
        string name = templateId + "_" + (int)app + "_" + "NotificationView";
        name = name.Replace(" ", "_");
        name = name.Replace("-", "_");
        string copyPath = Application.dataPath + folderPath + "/ViewScripts/" + name + ".cs";
        Debug.Log("Delete Classfile: " + copyPath);
        if (File.Exists(copyPath))
        { // do not overwrite
            UnityEditor.VersionControl.Provider.Delete(copyPath);
        }
    }

    public static string WriteDataClass(Utility.App app, string templateId, string folderPath)
    {
        // remove whitespace and minus
        string name = templateId + "_" + (int)app + "_" + "NotificationData";
        name = name.Replace(" ", "_");
        name = name.Replace("-", "_");
        string copyPath = Application.dataPath + folderPath + "/DataScripts/" + name + ".cs";
        Debug.Log("Creating Classfile: " + copyPath);
        if (File.Exists(copyPath) == false)
        { // do not overwrite
            using (StreamWriter outfile =
                new StreamWriter(copyPath))
            {
                outfile.WriteLine("using UnityEngine;");
                outfile.WriteLine("using System.Collections;");
                outfile.WriteLine("using System.Collections.Generic;");
                outfile.WriteLine("public class " + name + " : NotificationData");
                outfile.WriteLine("{ ");
                outfile.WriteLine(" ");
                outfile.WriteLine("     //Auto generated content");
                outfile.WriteLine("     protected override void initialize()");
                outfile.WriteLine("    {");
                outfile.WriteLine("        int appInt = " + (int)app + ";");
                outfile.WriteLine("        app = (Utility.App)appInt;");
                outfile.WriteLine("        templateId = \"" + templateId + "\";");
                outfile.WriteLine("    }");
                outfile.WriteLine(" ");
                outfile.WriteLine("}");

            }//File written
        }
        return name;
    }

    public static void DeleteDataClass(Utility.App app, string templateId, string folderPath)
    {
        string name = templateId + "_" + (int)app + "_" + "NotificationData";
        name = name.Replace(" ", "_");
        name = name.Replace("-", "_");
        string copyPath = Application.dataPath + folderPath + "/DataScripts/" + name + ".cs";
        Debug.Log("Delete Classfile: " + copyPath);
        if (File.Exists(copyPath))
        { // do not overwrite
            UnityEditor.VersionControl.Provider.Delete(copyPath);
        }
    }
    public static void DeletePrefab(Utility.App app, string templateId, string folderPath)
    {
        string name = templateId + "_" + (int)app + "_" + "NotificationView";
        name = name.Replace(" ", "_");
        name = name.Replace("-", "_");
        string copyPath = Application.dataPath + folderPath + "/" + name + ".prefab";
        Debug.Log("Delete Classfile: " + copyPath);
        if (File.Exists(copyPath))
        { // do not overwrite
            UnityEditor.VersionControl.Provider.Delete(copyPath);
        }
    }
}
