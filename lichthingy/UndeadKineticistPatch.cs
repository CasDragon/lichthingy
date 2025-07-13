using BlueprintCore.Utils;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.Kineticist;

namespace lichthingy
{
    [HarmonyPatch]
    internal class UndeadKineticistPatch
    {
        public static BlueprintUnitFactReference UndeadTypeRef = BlueprintTool.GetRef<BlueprintUnitFactReference>("734a29b693e9ec346ba2951b27987e33");


        /*[HarmonyPatch(typeof(UnitPartKineticist), nameof(UnitPartKineticist.Setup))]
        [HarmonyPostfix]
        public static void UndeadKineticist(UnitPartKineticist __instance)
        {
            if (__instance.m_Settings.MainStat != Kingmaker.EntitySystem.Stats.StatType.Constitution)
            {
                return;
            }
            var isUndead = __instance.Owner.HasFact(UndeadTypeRef);
            if (isUndead)
            {
                __instance.m_Settings.MainStat = Kingmaker.EntitySystem.Stats.StatType.Charisma;
                return;
            }
            if (__instance.Owner.GetFacts(UndeadTypeRef) != null)
            {
                __instance.m_Settings.MainStat = Kingmaker.EntitySystem.Stats.StatType.Charisma;
            }
        }*/

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UnitPartKineticist), nameof(UnitPartKineticist.Setup))]
        public static bool UndeadKine(UnitPartKineticist __instance, AddKineticistPart settings)
        {
            __instance.m_BurnHolderCache = __instance.Owner.Ensure<UnitPartKineticistBurnHolder>();
            __instance.m_Settings = settings;
            StatType stat = settings.MainStat;
            if (stat != StatType.Constitution)
                return false;
            if (__instance.Owner.HasFact(UndeadTypeRef))
            {
                __instance.m_Settings.MainStat = StatType.Charisma;
            }
            else
            {
                __instance.m_Settings.MainStat = (stat.IsAttribute() ? stat : StatType.Constitution);
            }
            return false;
        }
    }
}
