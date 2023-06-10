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
        private static readonly ModConfigurationKey<bool> EnableLinkedVariablesList = new("EnableLinkedVariablesList", "Allow generating a list of dynamic variable definitions for a space.", () => true);

        public override string Author => "Banane9";
        public override string Link => "https://github.com/Banane9/NeosSessionUsersDetails";
        public override string Name => "SessionUsersDetails";
        public override string Version => "1.0.0";

        public override void OnEngineInit()
        {
            Harmony harmony = new($"{Author}.{Name}");
            Config = GetConfiguration();
            Config.Save(true);
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(SessionUserController))]
        private static class SessionUserControllerPatches
        {
            private static readonly ConditionalWeakTable<SessionUserController, StaticTexture2D> userVoiceModeImages = new();

            [HarmonyPostfix]
            [HarmonyPatch(nameof(SessionUserController.Create))]
            private static void CreatePostfix(SessionUserController __result, User user, UIBuilder ui)
            {
                var horizontalLayout = ui.Root.GetComponentInChildren<HorizontalLayout>().Slot;

                var builder = new UIBuilder(horizontalLayout);
                RadiantUI_Constants.SetupDefaultStyle(builder);
                ui.Style.Width = SessionUserController.HEIGHT;

                var image = builder.Image(VoiceHelper.GetIcon(user.VoiceMode));
                image.Slot.OrderOffset = -1;

                userVoiceModeImages.Add(__result, image.Slot.GetComponent<StaticTexture2D>());
            }

            private static Uri GetVoiceIcon(User user)
            {
                return VoiceHelper.GetIcon(user.isMuted ? VoiceMode.Mute : user.VoiceMode);
            }

            [HarmonyPostfix]
            [HarmonyPatch("OnCommonUpdate")]
            private static void OnCommonUpdatePostfix(SessionUserController __instance)
            {
                if (!userVoiceModeImages.TryGetValue(__instance, out var image) || image == null)
                    return;

                image.URL.Value = GetVoiceIcon(__instance.TargetUser);
            }
        }
    }
}