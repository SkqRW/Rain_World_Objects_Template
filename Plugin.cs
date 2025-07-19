using System;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using RWCustom;
using BepInEx;
using Debug = UnityEngine.Debug;
using DevInterface;
using CustomEdible;

#pragma warning disable CS0618

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace Plugin;

[BepInPlugin(GUID, Name, Version)]
public partial class Plugin : BaseUnityPlugin
{
    public const string GUID = "Autor.CustomObjects";
    public const string Name = "CustomObjects";
    public const string Version = "0.0.1";


    public void OnEnable()
    {
        InitObjects();
        DevTools.Init();
    }

    public void OnDisble()
    {
        TerminateObjects();
        DevTools.Terminate();
    }

    public static void InitObjects()
    {
        CustomEdible.Object.Init(GUID); 
        DevTools.Init();
    }
    public static void TerminateObjects() 
    {
        CustomEdible.Object.Terminate();
        DevTools.Terminate();
    }
}
