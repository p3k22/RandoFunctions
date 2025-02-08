namespace P3ks_Afterlife_Secondary_Toolbelt_Fix.Mods
{
   // TODO -- The original mod has a bug where if you take off your secondary toolbet with items equipped to it, you lose those items for good. I also want to be able to hold shift and move backwards and still be able to select items from primary toolbelt"
   using AL.UI;

   using HarmonyLib;

   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Reflection;

   [HarmonyPatch(typeof(PlayerMoveController))]
   public static class PlayerMoveControllerPatch
   {
      [HarmonyPatch("Update")]
      [HarmonyTranspiler]
      private static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> ins)
      {
         var matcher = new CodeMatcher(ins);

         matcher.Start().SearchForward(i => i.operand is MethodInfo {Name: "get_SHIFT_KEY_SLOT_OFFSET"}).Advance(1)
            .SearchForward(i => i.operand is MethodInfo {Name: "get_SHIFT_KEY_SLOT_OFFSET"}).Advance(2).Insert(
            Transpilers.EmitDelegate<Func<int, int>>(
            idx =>
               {
                  var isRunning = GameManager.Instance.World?.GetPrimaryPlayer()?.MovementRunning ?? false;
                  if (isRunning)
                  {
                     var adjusted = idx >= 10 ? idx - 10 : idx;
                     return adjusted;
                  }

                  Toolbelt.ActivateSecondaryToolbelt();
                  return idx;
               }));

         return matcher.InstructionEnumeration();
      }
   }
}
