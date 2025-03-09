using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using RealismMod;
using SPT.Reflection.Patching;

namespace RealismModSync.HazardZones.Patches;

public class InteractableGroupComponentNamePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(InteractableGroupComponent).GetMethod("Start",
            BindingFlags.Instance | BindingFlags.NonPublic);
    }

    [PatchPrefix]
    public static void Patch(InteractableGroupComponent __instance)
    {
        
        // interaction name gen
        var names = new List<string>();
        
        var zones = __instance.GetComponentsInChildren<InteractionZone>();
        foreach (var zone in zones)
        {
            names.Add(
                $"{zone.Name}||{zone.InteractableData.InteractionType}||{zone.InteractableData.TargeObject}||{string.Join(",","zone.InteractableData.ZoneNames")}");
        }
        
        var md5 = new MD5CryptoServiceProvider();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(string.Join("@@", names)));
        var hashString = BitConverter.ToString(hash).Replace("-", String.Empty).ToLowerInvariant();
        var newName = $"Interactable{hashString}";
    
        Plugin.REAL_Logger.LogInfo($"Changing {__instance.name} to {newName}");

        __instance.name = newName;
    }
}