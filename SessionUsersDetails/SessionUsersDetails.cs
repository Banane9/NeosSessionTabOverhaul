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
        private static readonly ModConfigurationKey<color> FirstRowColorKey = new("FirstRowColor", "Background color of the first row in the Session user lists.", () => new color(0, .85f));

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> HideCustomBadgesKey = new("HideCustomBadges", "Hide Custom Badges in the Session Users list.", () => false);

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> HidePatreonBadgeKey = new("HidePatreonBadge", "Hide the Patreon Badge in the Session Users list.", () => false);

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<color> SecondRowColorKey = new("SecondRowColor", "Background color of the second row in the Session user lists.", () => new color(1, .15f));

        public override string Author => "Banane9";
        public override string Link => "https://github.com/Banane9/NeosSessionUsersDetails";
        public override string Name => "SessionUsersDetails";
        public override string Version => "1.0.0";

        internal static color FirstRowColor => Config.GetValue(FirstRowColorKey);
        internal static bool HideCustomBadges => Config.GetValue(HideCustomBadgesKey);
        internal static bool HidePatreonBadge => Config.GetValue(HidePatreonBadgeKey);
        internal static color SecondRowColor => Config.GetValue(SecondRowColorKey);
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