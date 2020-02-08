using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public static class UploadNPMPackage
{
    [MenuItem("Export Package/Upload NPM package")]
    public static void Upload()
    {
        char projectPathDriveLetter = Path.GetPathRoot(Application.dataPath).First(innerChar => char.IsLetter(innerChar));
        string command = projectPathDriveLetter + ": && cd " + Application.dataPath + " && cd Package && npm publish";

        ExecuteCMDCommand(command);
    }

    private static void ExecuteCMDCommand(string command)
    {
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments = "/C " + command;
        process.StartInfo = startInfo;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();

        string error = process.StandardError.ReadToEnd();
        string output = process.StandardOutput.ReadToEnd();

        if (string.IsNullOrEmpty(output))
            Debug.Log("<color=blue>No output</color>");
        else
            Debug.Log("<color=blue>Output: " + output + "</color>");

        if (string.IsNullOrEmpty(error))
            Debug.Log("<color=green>No errors</color>");
        else
            Debug.LogError("Error: " + error);

        process.Close();
    }
}
