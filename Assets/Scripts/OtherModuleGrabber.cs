using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class OtherModuleGrabber
{
    private static readonly HashSet<Transform> OtherModules = new HashSet<Transform>();

    private static readonly Type BombType = null;
    private static readonly Type BombComponentType = null;
    private static readonly FieldInfo IsSolvableField = null;

    static OtherModuleGrabber()
    {
        BombType = ReflectionHelper.FindType("Bomb");
        BombComponentType = ReflectionHelper.FindType("BombComponent");
        IsSolvableField = BombComponentType.GetField("IsSolvable", BindingFlags.Instance | BindingFlags.Public);
    }

    public static Transform GetOtherModule(AppreciateArtModule sourceModule)
    {
        Component bomb = sourceModule.GetComponentInParent(BombType);
        if (bomb == null)
        {
            return null;
        }

        Component[] bombComponents = bomb.GetComponentsInChildren(BombComponentType).Where(x => (bool)IsSolvableField.GetValue(x)).ToArray();
        if (bombComponents == null)
        {
            return null;
        }

        Transform[] possiblePicks = bombComponents.Where(x => !x.GetComponent<AppreciateArtModule>()).Select(x => x.transform).Except(OtherModules).ToArray();
        if (possiblePicks == null || possiblePicks.Length == 0)
        {
            return null;
        }

        Transform pick = possiblePicks.RandomPick();
        if (pick != null)
        {
            OtherModules.Add(pick);
            return pick;
        }

        return null;
    }

    public static void GiveBackOtherModule(Transform module)
    {
        OtherModules.Remove(module);
    }
}
