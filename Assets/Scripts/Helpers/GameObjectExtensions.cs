using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

static class GameObjectExtensions
{
    public static bool IsSamePrefab(this GameObject obj, GameObject obj2)
    {
        return obj.name.StartsWith(obj2.name);
    }
}
