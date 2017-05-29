﻿using System;
using System.Reflection;
using ColonistBarKF.NoCCL;
using Verse;


namespace ColonistBarKF
{
    public class CB_SpecialInjector : SpecialInjector
    {

        private static Assembly Assembly { get { return Assembly.GetAssembly(typeof(CB_SpecialInjector)); } }

        private static readonly BindingFlags[] bindingFlagCombos = {
            BindingFlags.Instance | BindingFlags.Public, BindingFlags.Static | BindingFlags.Public,
            BindingFlags.Instance | BindingFlags.NonPublic, BindingFlags.Static | BindingFlags.NonPublic
        };

        public override bool Inject()
        {

            #region Automatic hookup
            // Loop through all detour attributes and try to hook them up
            foreach (Type targetType in Assembly.GetTypes())
            {
                foreach (BindingFlags bindingFlags in bindingFlagCombos)
                {
                    foreach (MethodInfo targetMethod in targetType.GetMethods(bindingFlags))
                    {
                        foreach (DetourAttribute detour in targetMethod.GetCustomAttributes(typeof(DetourAttribute), true))
                        {
                            BindingFlags flags = detour.bindingFlags != default(BindingFlags) ? detour.bindingFlags : bindingFlags;
                            MethodInfo sourceMethod = detour.source.GetMethod(targetMethod.Name, flags);
                            if (sourceMethod == null)
                            {
                                Log.Error(string.Format("ColonistBar_KF :: Detours :: Can't find source method '{0} with bindingflags {1}", targetMethod.Name, flags));
                                return false;
                            }
                            if (!Detours.TryDetourFromTo(sourceMethod, targetMethod))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            #endregion

            return true;
        
        }
    }
}
