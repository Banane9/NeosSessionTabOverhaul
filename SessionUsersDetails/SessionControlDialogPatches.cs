using BaseX;
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
        private static readonly float2 rectOffset = new(4, 4);

        [HarmonyPostfix]
        [HarmonyPatch(nameof(SessionControlDialog.GenerateUi))]
        private static void GenerateUiPostfix(SessionControlDialog __instance, SessionControlDialog.Tab tab)
        {
            if (tab != SessionControlDialog.Tab.Settings)
                return;

            foreach (var accessLevelRadio in __instance._accessLevelRadios)
            {
                var accessLevelRow = accessLevelRadio.Slot.Parent.Parent.Parent;

                var layoutElement = accessLevelRow.GetComponent<LayoutElement>();
                layoutElement.PreferredHeight.Value += 8;
                layoutElement.MinHeight.Value += 8;

                foreach (var child in accessLevelRow.Children)
                {
                    var rectTransform = child.GetComponent<RectTransform>();
                    rectTransform.OffsetMin.Value += rectOffset;
                    rectTransform.OffsetMax.Value -= rectOffset;
                }

                var image = accessLevelRow.AttachComponent<Image>();
                image.Tint.Value = (accessLevelRow.ChildIndex & 1) == 0 ? SessionUsersDetails.FirstRowColor : SessionUsersDetails.SecondRowColor;
            }
        }

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