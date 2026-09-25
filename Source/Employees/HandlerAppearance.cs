using Il2CppScheduleOne.AvatarFramework;
using UnityEngine;

namespace VehicleHandlers.Employees
{
    internal static class HandlerAppearance
    {
        private static readonly Color UniformOrange = new Color(1.0f, 0.36f, 0.04f, 1.0f);

        public static bool TryApply(Avatar avatar)
        {
            if (avatar == null || avatar.CurrentSettings == null)
            {
                return false;
            }

            AvatarSettings settings = avatar.CurrentSettings;
            for (int layerIndex = 7; layerIndex >= 1; layerIndex--)
            {
                string layerPath = GetBodyLayerPath(settings, layerIndex);
                if (string.IsNullOrEmpty(layerPath))
                {
                    continue;
                }

                avatar.SetBodyLayer(layerIndex, layerPath, UniformOrange);
                return true;
            }

            return false;
        }

        private static string GetBodyLayerPath(AvatarSettings settings, int layerIndex)
        {
            switch (layerIndex)
            {
                case 1:
                    return settings.BodyLayer2Path;
                case 2:
                    return settings.BodyLayer3Path;
                case 3:
                    return settings.BodyLayer4Path;
                case 4:
                    return settings.BodyLayer5Path;
                case 5:
                    return settings.BodyLayer6Path;
                case 6:
                    return settings.BodyLayer7Path;
                case 7:
                    return settings.BodyLayer8Path;
                default:
                    return string.Empty;
            }
        }
    }
}
