using FrooxEngine;
using FrooxEngine.UIX;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionUsersDetails
{
    [HarmonyPatch(typeof(SessionControlDialog))]
    internal static class SessionControlDialogPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(SessionControlDialog.OnAttach))]
        private static void OnAttachPostfix(SessionControlDialog __instance)
        {
            var rectTransform = __instance.Slot.GetComponent<RectTransform>();
            rectTransform.OffsetMin.Value = new(16, 16);
            rectTransform.OffsetMax.Value = new(-16, -16);
        }
    }
}