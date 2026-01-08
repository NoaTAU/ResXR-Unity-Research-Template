using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using System.Collections.Generic;
using System.Linq;

[InitializeOnLoad]
public class MetaSDKInstallationChecker
{
    private const string DONT_SHOW_KEY = "ResXR.MetaSDKChecker.DontShow";
    private const string REQUIRED_VERSION = "78.0.0";
    
    private static readonly string[] REQUIRED_PACKAGES = new string[]
    {
        "com.meta.xr.mrutilitykit",
        "com.meta.xr.sdk.audio",
        "com.meta.xr.sdk.core",
        "com.meta.xr.sdk.haptics",
        "com.meta.xr.sdk.interaction.ovr",
        "com.meta.xr.sdk.platform",
        "com.meta.xr.sdk.voice",
        "com.meta.xr.simulator"
    };

    private static bool _hasChecked = false;

    static MetaSDKInstallationChecker()
    {
        // Use EditorApplication.update to wait for Unity to be ready
        // This works even if there are compilation errors
        EditorApplication.update += InitializeCheck;
    }

    private static void InitializeCheck()
    {
        // Only check once
        if (_hasChecked) return;
        
        // Wait a frame to ensure Unity is initialized
        EditorApplication.update -= InitializeCheck;
        _hasChecked = true;
        
        // Delay the actual check
        EditorApplication.delayCall += CheckMetaSDKInstallation;
    }

    private static void CheckMetaSDKInstallation()
    {
        // Don't check if user dismissed the dialog
        if (EditorPrefs.GetBool(DONT_SHOW_KEY, false))
        {
            return;
        }

        // Try to check packages, but show dialog immediately if checking fails
        // This ensures users always get the installation prompt
        bool checkSucceeded = false;
        
        try
        {
            ListRequest listRequest = Client.List();
            
            // Wait for the request to complete (with timeout)
            int timeout = 0;
            EditorApplication.update += CheckRequest;
            
            void CheckRequest()
            {
                timeout++;
                if (listRequest.IsCompleted)
                {
                    EditorApplication.update -= CheckRequest;
                    checkSucceeded = true;
                    
                    if (listRequest.Status == StatusCode.Success)
                    {
                        CheckPackages(listRequest.Result);
                    }
                    else
                    {
                        // Request failed, show dialog with all packages as missing
                        ShowInstallationDialog(REQUIRED_PACKAGES.ToList(), new List<string>());
                    }
                }
                else if (timeout > 300) // ~5 seconds timeout
                {
                    EditorApplication.update -= CheckRequest;
                    // Timeout - show dialog anyway
                    ShowInstallationDialog(REQUIRED_PACKAGES.ToList(), new List<string>());
                }
            }
        }
        catch
        {
            // If checking fails (e.g., due to compilation errors), show dialog with all packages as missing
            // This ensures users always get the installation prompt
            if (!checkSucceeded)
            {
                ShowInstallationDialog(REQUIRED_PACKAGES.ToList(), new List<string>());
            }
        }
    }

    private static void CheckPackages(PackageCollection packages)
    {
        List<string> missingPackages = new List<string>();
        List<string> wrongVersionPackages = new List<string>();

        foreach (string packageName in REQUIRED_PACKAGES)
        {
            var package = packages.FirstOrDefault(p => p.name == packageName);
            
            if (package == null || string.IsNullOrEmpty(package.version))
            {
                missingPackages.Add(packageName);
            }
            else if (package.version != REQUIRED_VERSION)
            {
                wrongVersionPackages.Add($"{packageName} (installed: {package.version}, required: {REQUIRED_VERSION})");
            }
        }

        if (missingPackages.Count > 0 || wrongVersionPackages.Count > 0)
        {
            ShowInstallationDialog(missingPackages, wrongVersionPackages);
        }
    }

    private static void ShowInstallationDialog(List<string> missingPackages, List<string> wrongVersionPackages)
    {
        string message = "This template requires Meta XR SDK packages to function properly.\n\n";

        if (missingPackages.Count > 0)
        {
            message += "Missing packages:\n";
            foreach (string package in missingPackages)
            {
                message += $"• {package} (v{REQUIRED_VERSION})\n";
            }
            message += "\n";
        }

        if (wrongVersionPackages.Count > 0)
        {
            message += "Wrong version packages:\n";
            foreach (string package in wrongVersionPackages)
            {
                message += $"• {package}\n";
            }
            message += "\n";
        }

        message += "To install:\n";
        message += "1. Open Window > Package Manager\n";
        message += "2. Click the '+' button > Add package by name\n";
        message += "3. Add each package with version " + REQUIRED_VERSION + "\n\n";

        message += "Required packages:\n";
        foreach (string package in REQUIRED_PACKAGES)
        {
            message += $"• {package}@{REQUIRED_VERSION}\n";
        }

        int option = EditorUtility.DisplayDialogComplex(
            "Meta SDK Required",
            message,
            "Open Package Manager",
            "Remind Me Later",
            "Don't Show Again"
        );

        switch (option)
        {
            case 0: // Open Package Manager
                EditorApplication.ExecuteMenuItem("Window/Package Manager");
                break;
            case 1: // Remind Me Later
                // Do nothing, will show again next time
                break;
            case 2: // Don't Show Again
                EditorPrefs.SetBool(DONT_SHOW_KEY, true);
                break;
        }
    }

    [MenuItem("Tools/ResXR/Check Meta SDK Installation")]
    public static void ManualCheck()
    {
        EditorPrefs.DeleteKey(DONT_SHOW_KEY);
        
        // Try to check packages, but if it fails, show dialog anyway
        try
        {
            ListRequest listRequest = Client.List();
            
            // Wait for the request to complete
            EditorApplication.update += () =>
            {
                if (listRequest.IsCompleted)
                {
                    EditorApplication.update -= null;
                    
                    if (listRequest.Status == StatusCode.Success)
                    {
                        CheckPackages(listRequest.Result);
                    }
                    else
                    {
                        // If request failed, show dialog with all packages as missing
                        ShowInstallationDialog(REQUIRED_PACKAGES.ToList(), new List<string>());
                    }
                }
            };
        }
        catch
        {
            // If checking fails (e.g., due to compilation errors), show dialog with all packages as missing
            ShowInstallationDialog(REQUIRED_PACKAGES.ToList(), new List<string>());
        }
    }
}
