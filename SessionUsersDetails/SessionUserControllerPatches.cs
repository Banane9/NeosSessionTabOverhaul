using FrooxEngine.UIX;
using FrooxEngine;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BaseX;

#if true
// Disable CodeMaid Cleanup
#endif

namespace SessionUsersDetails
{
    [HarmonyPatch(typeof(SessionUserController))]
    internal static class SessionUserControllerPatches
    {
        private const string headless = "headless";
        private const string headlessSprite = $"<sprite name=\"{headless}\">";

        private const string screen = "screen";
        private const string screenSprite = $"<sprite name=\"{screen}\">";

        private const string vr = "vr";
        private const string vrSprite = $"<sprite name=\"{vr}\">";

        private const string muteSprite = $"<sprite name=\"{nameof(VoiceMode.Mute)}\">";
        private const string whisperSprite = $"<sprite name=\"{nameof(VoiceMode.Whisper)}\">";
        private const string normalSprite = $"<sprite name=\"{nameof(VoiceMode.Normal)}\">";
        private const string shoutSprite = $"<sprite name=\"{nameof(VoiceMode.Shout)}\">";
        private const string broadcastSprite = $"<sprite name=\"{nameof(VoiceMode.Broadcast)}\">";

        private static readonly ConditionalWeakTable<SessionUserController, SessionUserControllerExtraData> controllerExtraData = new();

        [HarmonyPrefix]
        [HarmonyPatch(nameof(SessionUserController.Create))]
        private static bool CreatePrefix(out SessionUserController __result, User user, UIBuilder ui)
        {
            ui.Style.MinHeight = SessionUserController.HEIGHT;
            //var rectTransform = ui.Panel();
            var horizontal = ui.HorizontalLayout(4, 0, Alignment.MiddleCenter);
            horizontal.ForceExpandHeight.Value = false;
            horizontal.ForceExpandWidth.Value = false;

            var controller = horizontal.Slot.AttachComponent<SessionUserController>();
            controller._cachedUserName = user.UserName;
            controller.TargetUser = user;

            var extraData = controllerExtraData.GetOrCreateValue(controller);
            var badgeFont = controller.GetBadgeFontCollection();

            ui.Style.MinWidth = 0.8f * SessionUserController.HEIGHT;

            ui.Text(user.IsHost ? "<sprite name=\"host\">" : "", alignment: Alignment.MiddleCenter).Font.Target = badgeFont;

            extraData.DeviceLabel = ui.Text(GetUserDeviceLabel(user), alignment: Alignment.MiddleCenter);
            extraData.DeviceLabel.Font.Target = badgeFont;

            ui.Style.FlexibleWidth = 1;
            controller._name.Target = ui.Text(controller._cachedUserName, alignment: Alignment.MiddleLeft);
            ui.Style.FlexibleWidth = -1;

            ui.Style.MinWidth = 256;
            controller._slider.Target = ui.Slider(SessionUserController.HEIGHT, 0f, 0f, 2f);
            controller._slider.Target.BaseColor.Value = GetUserVoiceModeColor(user);

            var colorDrive = controller._slider.Target.ColorDrivers[0];
            colorDrive.TintColorMode.Value = InteractionElement.ColorMode.Explicit;
            colorDrive.NormalColor.Value = color.LightGray;
            colorDrive.HighlightColor.Value = color.White;
            colorDrive.PressColor.Value = color.Gray;
            colorDrive.DisabledColor.Value = color.DarkGray;

            ui.Style.MinWidth = 32;

            extraData.VoiceModeLabel = ui.Text(GetUserVoiceModeLabel(user), alignment: Alignment.MiddleLeft);
            extraData.VoiceModeLabel.Font.Target = badgeFont;

            ui.Style.MinWidth = 96;

            controller._mute.Target = ui.Button("User.Actions.Mute".AsLocaleKey(), controller.OnMute);

            controller._jump.Target = ui.Button("User.Actions.Jump".AsLocaleKey(), controller.OnJump);

            controller._respawn.Target = ui.Button("User.Actions.Respawn".AsLocaleKey(), controller.OnRespawn);

            controller._silence.Target = ui.Button("User.Actions.Silence".AsLocaleKey(), controller.OnSilence);

            controller._kick.Target = ui.Button("User.Actions.Kick".AsLocaleKey(), controller.OnKick);

            controller._ban.Target = ui.Button("User.Actions.Ban".AsLocaleKey(), controller.OnBan);

            ui.NestOut();

            if (user.Platform.IsMobilePlatform())
                controller.AddBadge("mobile");

            if (user.Platform == Platform.Linux)
                controller.AddBadge("linux");

            if (user.UserID != null)
            {
                controller.StartTask(async delegate
                {
                    var cloudResult = await controller.Cloud.GetUserCached(user.UserID);
                    if (cloudResult.IsOK)
                    {
                        controller.SetCloudData(cloudResult.Entity);
                    }
                });
            }

            __result = controller;
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(SessionUserController.GetBadgeFontCollection))]
        private static void GetBadgeFontCollectionPostfix(FontCollection __result)
        {
            if (SessionUsersDetails.SpritesInjected)
                return;

            var spriteFont = (DynamicSpriteFont)__result.FontSets[1];

            if (!spriteFont.HasSprite(screen))
                spriteFont.AddSprite(screen, new Uri("neosdb:///1c88a45653f60a9b29eefc5e3adc4659f3021a85debd5b4f3425ff29d4564794"));

            if (!spriteFont.HasSprite(vr))
                spriteFont.AddSprite(vr, new Uri("neosdb:///1d2dc53aa1b44d8a21aaaa3ce41b695ae724eb0553e7ee08d50fc0c7922ae149"));

            foreach (VoiceMode voiceMode in Enum.GetValues(typeof(VoiceMode)))
            {
                var name = voiceMode.ToString();

                if (!spriteFont.HasSprite(name))
                    spriteFont.AddSprite(name, VoiceHelper.GetIcon(voiceMode));
            }

            SessionUsersDetails.SpritesInjected = true;
        }

        private static string GetUserDeviceLabel(User user)
        {
            if (user.HeadDevice == HeadOutputDevice.Headless)
                return headlessSprite;

            if (user.VR_Active)
                return vrSprite;

            return screenSprite;
        }

        private static VoiceMode GetUserVoiceMode(User user)
            => user.isMuted ? VoiceMode.Mute : user.VoiceMode;

        private static string GetUserVoiceModeLabel(User user)
            => GetUserVoiceMode(user) switch
            {
                VoiceMode.Mute => muteSprite,
                VoiceMode.Whisper => whisperSprite,
                VoiceMode.Normal => normalSprite,
                VoiceMode.Shout => shoutSprite,
                VoiceMode.Broadcast => broadcastSprite,
                _ => ""
            };

        private static color GetUserVoiceModeColor(User user)
            => VoiceHelper.GetColor(GetUserVoiceMode(user)).SetSaturation(.5f);

        [HarmonyPostfix]
        [HarmonyPatch("OnCommonUpdate")]
        private static void OnCommonUpdatePostfix(SessionUserController __instance)
        {
            if (!controllerExtraData.TryGetValue(__instance, out var extraData) || extraData == null)
                return;

            var user = __instance.TargetUser;

            extraData.DeviceLabel.Content.Value = GetUserDeviceLabel(user);

            extraData.VoiceModeLabel.Content.Value = GetUserVoiceModeLabel(user);
            __instance._slider.Target.BaseColor.Value = GetUserVoiceModeColor(user);
        }
    }
}