using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using BaseX;
using CodeX;
using FrooxEngine;
using FrooxEngine.LogiX;
using FrooxEngine.LogiX.Data;
using FrooxEngine.LogiX.ProgramFlow;
using FrooxEngine.UIX;
using HarmonyLib;
using NeosModLoader;

namespace SessionUsersDetails
{
    public class SessionUsersDetails : NeosMod
    {
        public static ModConfiguration Config;

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> SpritesInjectedKey = new("SpritesInjectedKey", "Whether the necessary sprites have been added already.", () => false);

        public override string Author => "Banane9";
        public override string Link => "https://github.com/Banane9/NeosSessionUsersDetails";
        public override string Name => "SessionUsersDetails";
        public override string Version => "1.0.0";

        internal static bool SpritesInjected { get; set; } = false;

        public override void OnEngineInit()
        {
            Harmony harmony = new($"{Author}.{Name}");
            Config = GetConfiguration();
            Config.Save(true);
            harmony.PatchAll();
        }
    }
}