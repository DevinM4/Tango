﻿using ColossalFramework;
using CSM.Commands;
using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CSM.Injections
{
    public class TreeHandler
    {
        public static List<uint> IgnoreTrees { get; } = new List<uint>();
    }


    [HarmonyPatch(typeof(TreeManager))]
    [HarmonyPatch("CreateTree")]
    public class CreateTree
    {
        public static void Postfix(bool __result, ref uint tree, Vector3 position, bool single)
        {
            if (__result)
            {
                TreeInstance treeInstance = Singleton<TreeManager>.instance.m_trees.m_buffer[tree];

                Command.SendToAll(new TreeCreateCommand
                {
                    Position = position,
                    TreeID = tree,
                    Single = single,
                    InfoIndex = treeInstance.m_infoIndex
                });
            }
        }
    }

    [HarmonyPatch(typeof(TreeManager))]
    [HarmonyPatch("MoveTree")]
    public class MoveTree
    {
        public static void Postfix(uint tree, Vector3 position)
        {
            if (!TreeHandler.IgnoreTrees.Contains(tree))
            {
                Command.SendToAll(new TreeMoveCommand
                {
                    TreeID = tree,
                    Position = position
                });
            }
        }
    }

    [HarmonyPatch(typeof(TreeManager))]
    [HarmonyPatch("ReleaseTree")]
    public class ReleaseTree
    {
        public static void Prefix(uint tree)
        {
            if (!TreeHandler.IgnoreTrees.Contains(tree))
            {
                Command.SendToAll(new TreeReleaseCommand
                {
                    TreeID = tree
                });

            }
    }
    }

}