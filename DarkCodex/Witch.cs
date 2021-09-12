﻿using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Enums.Damage;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;

namespace DarkCodex
{
    public class Witch
    {
        public static void createExtraHex()
        {

        }

        public static void createHexStrike()
        {

        }

        public static void createSplitHex()
        {

        }

        public static void createAmplifyHex()
        {

        }

        public static void createIceTomb()
        {
            var IcyPrison = ResourcesLibrary.TryGetBlueprint<BlueprintAbility>("65e8d23aef5e7784dbeb27b1fca40931");
            var WitchMajorHex = Helper.ToRef<BlueprintFeatureReference>("8ac781b33e380c84aa578f1b006dd6c5");
            var Staggered = Helper.ToRef<BlueprintBuffReference>("df3950af5a783bd4d91ab73eb8fa0fd3");
            var IcyPrisonParalyzedBuff = Helper.ToRef<BlueprintBuffReference>("6f0e450771cc7d446aea798e1fef1c7a");
            var WitchHexSelection = ResourcesLibrary.TryGetBlueprint<BlueprintFeatureSelection>("9846043cf51251a4897728ed6e24e76f");


            var icetomb_cooldown = Helper.CreateBlueprintBuff("WitchHexIceTombCooldownBuff", "", "").Flags(hidden: true);

            var icetomb_debuff = Helper.CreateBlueprintBuff("WitchHexIceTombBuff", "Frozen", "A storm of ice and freezing wind enveloped the creature.", null, null, null
                ).SetComponents(
                Helper.CreateAddCondition(UnitCondition.Paralyzed),
                Helper.CreateAddCondition(UnitCondition.Unconscious)
                );

            var runaction = Helper.CreateAbilityEffectRunAction(
                SavingThrowType.Fortitude,
                Helper.CreateContextActionApplyBuff(icetomb_cooldown, 1, DurationRate.Days),
                Helper.CreateContextActionDealDamage(DamageEnergyType.Cold, Helper.CreateContextDiceValue(dice: DiceType.D6, diceCount: 3, bonus: 0), halfIfSaved: true),
                Helper.CreateContextActionConditionalSaved(
                    failed: Helper.CreateContextActionApplyBuff(icetomb_debuff, Helper.CreateContextDurationValue(diceRank: AbilityRankType.Default, rate: DurationRate.Minutes), false, false),
                    succeed: Helper.CreateContextActionApplyBuff(Staggered, Helper.CreateContextDurationValue(diceCount: 1, dice: DiceType.D4), false, false)
                    )
                );

            var icetomb_ab = Helper.CreateBlueprintAbility(
                "WitchHexIceTombAbility",
                "Ice Tomb",
                "A storm of ice and freezing wind envelops the creature, which takes 3d8 points of cold damage (Fortitude half). If the target fails its save, it is paralyzed and unconscious for 1 minute/level. A successful save destroys the ice freeing the creature, which is staggered for 1d4 rounds. Whether or not the target’s saving throw is successful, it cannot be the target of this hex again for 1 day.",
                null,
                IcyPrison.Icon,
                AbilityType.Supernatural,
                CommandType.Standard,
                AbilityRange.Medium,
                Resource.Strings.MinutesPerLevel,
                Resource.Strings.FortitudePartial
                ).SetComponents(
                runaction,
                Helper.CreateAbilityTargetHasFact(true, icetomb_cooldown.ToRef2()),
                Helper.CreateSpellDescriptorComponent(SpellDescriptor.Hex | SpellDescriptor.Cold)
                );

            var icetomb = Helper.CreateBlueprintFeature(
                "WitchHexIceTombFeature",
                icetomb_ab.m_DisplayName,
                icetomb_ab.m_Description,
                null,
                IcyPrison.Icon,
                FeatureGroup.WitchHex
                ).SetComponents(
                Helper.CreateAddFacts(icetomb_ab.ToRef2()),
                Helper.CreatePrerequisiteFeature(WitchMajorHex)
                );

            Helper.AppendAndReplace(ref WitchHexSelection.m_AllFeatures, icetomb.ToRef());
        }
    }
}
