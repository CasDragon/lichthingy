using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using HarmonyLib;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.ElementsSystem;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.Utility;
using UnityModManagerNet;

namespace lichthingy
{
#if DEBUG
    [EnableReloading]
#endif
    public static class Main
    {
        internal static Harmony HarmonyInstance;
        internal static UnityModManager.ModEntry.ModLogger log;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            log = modEntry.Logger;
            modEntry.OnGUI = OnGUI;
            HarmonyInstance = new Harmony(modEntry.Info.Id);
            HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            return true;
        }

        public static void OnGUI(UnityModManager.ModEntry modEntry)
        {

        }

        [HarmonyPatch(typeof(BlueprintsCache))]
        public static class BlueprintsCaches_Patch
        {
            private static bool Initialized = false;

            [HarmonyPriority(Priority.First)]
            [HarmonyPatch(nameof(BlueprintsCache.Init)), HarmonyPostfix]
            public static void Init_Postfix()
            {
                try
                {
                    if (Initialized)
                    {
                        log.Log("Already initialized blueprints cache.");
                        return;
                    }
                    Initialized = true;

                    log.Log("Patching blueprints.");
                    AbilityAreaEffectConfigurator.For(AbilityAreaEffectRefs.LichAuraBolsterUndeadArea)
                        .EditComponent<AbilityAreaEffectRunAction>(c => dothing(c))
                        .Configure();
                }
                catch (Exception e)
                {
                    log.Log(string.Concat("Failed to initialize.", e));
                }
            }
        }

        public static void dothing(AbilityAreaEffectRunAction component)
        {
            Condition[] ogconditions = component.UnitEnter.Actions.OfType<Conditional>().First().ConditionsChecker.Conditions;
            Condition[] conditions = [ogconditions.OfType<ContextConditionIsAlly>().First(), ogconditions.OfType<ContextConditionHasFact>().First()];
            component.UnitEnter.Actions.OfType<Conditional>().First().ConditionsChecker.Conditions = conditions;
            Condition[] ogconditions1 = component.Round.Actions.OfType<Conditional>().First().ConditionsChecker.Conditions;
            Condition[] conditions1 = [ogconditions1.OfType<ContextConditionIsAlly>().First(), ogconditions1.OfType<ContextConditionHasFact>().First()];
            component.Round.Actions.OfType<Conditional>().First().ConditionsChecker.Conditions = conditions1;
        }
    }
}
