using System;
using UnityEngine;

[CreateAssetMenu(menuName = "My FMOD Callback Handler")]
public class FMODCallbackHandler : FMODUnity.PlatformCallbackHandler
{
    public override void PreInitialize(FMOD.Studio.System studioSystem, Action<FMOD.RESULT, string> reportResult)
    {
        FMOD.RESULT result;

        FMOD.System coreSystem;
        result = studioSystem.getCoreSystem(out coreSystem);
        reportResult(result, "studioSystem.getCoreSystem");

        // Set up studioSystem and coreSystem as desired
    }
}
