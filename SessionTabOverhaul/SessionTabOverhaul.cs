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

namespace SessionTabOverhaul
{
    public class SessionTabOverhaul : NeosMod
    {
        public static ModConfiguration Config;

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> ColorHostNameKey = new("ColorHostName", "Color the Host's username like the host icon.", () => true);

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<color> FirstRowColorKey = new("FirstRowColor", "Background color of the first row in the Session user lists.", () => new color(0, .85f));

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> HideAllBadgesKey = new("HideAllBadges", "Hide all Badges in the Session Users list.", () => false);

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> HideCustomBadgesKey = new("HideCustomBadges", "Hide Custom Badges in the Session Users list.", () => false);

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> HidePatreonBadgeKey = new("HidePatreonBadge", "Hide the Patreon Badge in the Session Users list.", () => false);

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<color> SecondRowColorKey = new("SecondRowColor", "Background color of the second row in the Session user lists.", () => new color(1, .15f));

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> ShowBringButtonKey = new("ShowBringButton", "Show the Bring button in the Session Users list.", () => true);

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> ShowDeviceLabelKey = new("ShowDeviceLabel", "Show the Device label in the Session Users list.", () => true);

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> ShowFPSOrQueuedMessagesKey = new("ShowFPSOrQueuedMessages", "Show the FPS / Queued messages in the Session Users list.", () => true);

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> ShowSteamButtonKey = new("ShowSteamButton", "Show the Steam button in the Session Users list.", () => true);

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> ShowVoiceModeKey = new("ShowVoiceMode", "Show the Voice mode in the Session Users list.", () => true);

        public override string Author => "Banane9";
        public override string Link => "https://github.com/Banane9/NeosSessionTabOverhaul";
        public override string Name => "SessionTabOverhaul";
        public override string Version => "1.1.0";

        internal static bool ColorHostName => Config.GetValue(ColorHostNameKey);
        internal static color FirstRowColor => Config.GetValue(FirstRowColorKey);
        internal static bool HideAllBadges => Config.GetValue(HideAllBadgesKey);
        internal static bool HideCustomBadges => Config.GetValue(HideCustomBadgesKey);
        internal static bool HidePatreonBadge => Config.GetValue(HidePatreonBadgeKey);
        internal static color SecondRowColor => Config.GetValue(SecondRowColorKey);
        internal static bool ShowBringButton => Config.GetValue(ShowBringButtonKey);
        internal static bool ShowDeviceLabel => Config.GetValue(ShowDeviceLabelKey);
        internal static bool ShowFPSOrQueuedMessages => Config.GetValue(ShowFPSOrQueuedMessagesKey);
        internal static bool ShowSteamButton => Config.GetValue(ShowSteamButtonKey);
        internal static bool ShowVoiceMode => Config.GetValue(ShowVoiceModeKey);
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