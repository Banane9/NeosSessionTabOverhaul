using FrooxEngine;
using FrooxEngine.UIX;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionTabOverhaul
{
    [HarmonyPatch(typeof(SessionPermissionController))]
    internal static class SessionPermissionControllerPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(SessionPermissionController.Create))]
        private static void CreatePostfix(SessionPermissionController __result)
        {
            var bgImage = __result.Slot.AttachComponent<Image>();
            bgImage.Tint.Value = (__result.Slot.ChildIndex & 1) == 0 ?
                SessionTabOverhaul.FirstRowColor : SessionTabOverhaul.SecondRowColor;

            __result.Slot.GetComponent<LayoutElement>().MinHeight.Value += 8;

            var horizontal = __result.Slot.GetComponentInChildren<HorizontalLayout>();
            horizontal.PaddingBottom.Value = 4;
            horizontal.PaddingRight.Value = 4;
            horizontal.PaddingLeft.Value = 4;
            horizontal.PaddingTop.Value = 4;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(SessionPermissionController.OnCommonUpdate))]
        private static void OnCommonUpdatePostfix(SessionPermissionController __instance)
        {
            __instance.Slot.GetComponent<Image>().Tint.Value = (__instance.Slot.ChildIndex & 1) == 0 ?
                SessionTabOverhaul.FirstRowColor : SessionTabOverhaul.SecondRowColor;
        }
    }
}