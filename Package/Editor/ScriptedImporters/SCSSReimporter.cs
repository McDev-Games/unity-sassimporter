using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class SCSSReimporter : AssetPostprocessor
{
    private static Dictionary<string, List<string>> fileDependencies = new Dictionary<string, List<string>>();

    static bool isForceImporting;

    private static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        var importedSassFiles = importedAssets.Where(a => a.EndsWith(".scss")).ToArray();

        if (importedSassFiles.Length > 0)
        {
            if (isForceImporting)
            {
                //We assume a second forced import attempt and not re-import all sass files. Reset flag.
                isForceImporting = false;
            }
            else
            {
                //This could be optimized by storing dependecy trees, but in fact the amount of root scss files will be low.
                //The SASS importer will not import files that begin with "_" and hence we can define these as dependencies and not consider importing them in Unity.
                ReimportAllSassFiles(importedSassFiles);
            }
        }
    }

    private static void ReimportAllSassFiles(string[] importedSassFiles)
    {
        string assetsPath = Application.dataPath;
        string[] allScssFiles = Directory.GetFiles(assetsPath, "*.scss", SearchOption.AllDirectories);
        var cutoffLength = assetsPath.Remove(assetsPath.Length - "/Assets".Length + 1).Length;

        var filteredScssFiles = allScssFiles
            .Where(s => !Path.GetFileName(s).StartsWith("_"))   //Filter imported files     
            .Select(s => s.Remove(0, cutoffLength)              //Get relative path
            .Replace('\\', '/'));                               //Unity-fy path

        isForceImporting = true;
        foreach (string scssFile in filteredScssFiles)
        {
            //No douple import
            if (!importedSassFiles.Contains(scssFile))
            {
                AssetDatabase.ImportAsset(scssFile, ImportAssetOptions.Default);
            }
        }
    }

    private static void CheckAndReimportDependencies(string changedFile)
    {
        if (fileDependencies.TryGetValue(changedFile, out List<string> dependents))
        {
            foreach (string dependent in dependents)
            {
                AssetDatabase.ImportAsset(dependent, ImportAssetOptions.Default);
            }
        }
    }

    private static void RefreshDependencies()
    {
        fileDependencies.Clear();
        string assetsPath = Application.dataPath;
        string[] allScssFiles = Directory.GetFiles(assetsPath, "*.scss", SearchOption.AllDirectories);

        foreach (string scssFile in allScssFiles)
        {
            string fileContent = File.ReadAllText(scssFile);
            MatchCollection matches = Regex.Matches(fileContent, @"@import\s+""(.*?\.scss)"";");

            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    string relativePath = match.Groups[1].Value;
                    string dependencyPath = Path.Combine(Path.GetDirectoryName(scssFile), relativePath);
                    dependencyPath = Path.GetFullPath(dependencyPath).Replace("\\", "/");

                    if (!fileDependencies.ContainsKey(dependencyPath))
                    {
                        fileDependencies[dependencyPath] = new List<string>();
                    }

                    fileDependencies[dependencyPath].Add(scssFile);
                }
            }
        }
    }
}