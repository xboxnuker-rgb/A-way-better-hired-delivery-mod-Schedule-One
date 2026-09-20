using System;
using Il2CppScheduleOne.Employees;
using MelonLoader;

namespace VehicleHandlers.Employees
{
    public sealed class HandlerEmployee : Employee
    {
        public HandlerEmployee(IntPtr nativePointer)
            : base(nativePointer)
        {
        }

        public override void Awake()
        {
            Type = EEmployeeType.Handler;
            base.Awake();
            Type = EEmployeeType.Handler;
        }

        public override void InitializeAppearance(bool isMale, int appearanceIndex)
        {
            base.InitializeAppearance(isMale, appearanceIndex);

            try
            {
                if (!HandlerAppearance.TryApply(Avatar))
                {
                    MelonLogger.Warning("Handler appearance could not find a non-base body layer; preserving the generated employee appearance.");
                }
            }
            catch (Exception exception)
            {
                MelonLogger.Warning($"Handler appearance override failed safely: {exception.Message}");
            }
        }
    }
}
