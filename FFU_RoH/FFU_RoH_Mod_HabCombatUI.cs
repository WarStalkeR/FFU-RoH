#pragma warning disable CS0108
#pragma warning disable CS0114
#pragma warning disable CS0169
#pragma warning disable CS0414
#pragma warning disable CS0436
#pragma warning disable CS0649
#pragma warning disable IDE1006
#pragma warning disable IDE0002

/*
using TMPro;
using MonoMod;
using ModelShark;
using UnityEngine;
using UnityEngine.UI;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Ship;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using FullSerializer;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Entities;

[MonoModReplace] public class TIHabModuleTemplate : TIDataTemplate {
	private struct IncomeEntry {
		public string inlinePath;
		public string value;
		public IncomeEntry(string ip, string v) {
			inlinePath = ip;
			value = v;
		}
	}
	public bool coreModule;
	public HabType habType;
	public bool onePerHab;
	public bool automated;
	public bool alienModule;
	public bool noBuild;
	public string upgradesFromName;
	public int tier;
	public string requiredProjectName;
	public int crew;
	public int power;
	public float baseMass_tons;
	public float constructionTimeModifier = 1f;
	public float miningModifier;
	public bool allowsShipConstruction;
	public bool allowsResupply;
	public bool mine;
	public bool destroyed;
	public float buildTime_Days;
	public int spaceCombatValue;
	public float incomeMoney_month;
	public float incomeInfluence_month;
	public float incomeOps_month;
	public float incomeBoost_month;
	public int missionControl;
	public float incomeResearch_month;
	public int incomeProjects;
	public float incomeNobles_month;
	public float incomeFissiles_month;
	public float incomeAntimatter_month;
	public float incomeExotics_month;
	public TechBonus[] techBonuses;
	public string unlocksProjectName;
	public List<HabModuleSpecialRule> specialRules;
	public float specialRulesValue;
	public ResourceCostBuilder weightedBuildMaterials;
	public ResourceCostBuilder supportMaterials_month;
	public string baseIconResource;
	public string stationIconResource;
	public string stationModelResource;
	public string stationDestructionResource;
	public bool objectiveModule;
	public bool alertWorthy;
	public static readonly List<HabModuleSpecialRule> combatTroopsRules = new List<HabModuleSpecialRule>
	{
		HabModuleSpecialRule.DropTroops,
		HabModuleSpecialRule.Griffins,
		HabModuleSpecialRule.MarineCompany,
		HabModuleSpecialRule.MarinePlatoon,
		HabModuleSpecialRule.MarineBattalion,
		HabModuleSpecialRule.Salamanders,
		HabModuleSpecialRule.WarDogs
	};
	private List<HabModuleSpecialRule> _specialRules;
	private TIHabModuleTemplate _upgradesFrom;
	private TIHabModuleTemplate _upgradesTo;
	private bool _upgradeToChecked;
	public float upgradeDiscount => 2f / 3f;
	public bool constructionModule {
		get {
			if (!allowsShipConstruction) {
				return constructionTimeModifier < 1f;
			}
			return false;
		}
	}
	public bool EnablesLocalFounding {
		get {
			if (!SpecialRules.Contains(HabModuleSpecialRule.CanFoundTier1Habs) && !SpecialRules.Contains(HabModuleSpecialRule.CanFoundTier2Habs)) {
				return SpecialRules.Contains(HabModuleSpecialRule.CanFoundTier3Habs);
			}
			return true;
		}
	}
	public bool IsSolarPower {
		get {
			if (powerSource) {
				return SpecialRules.Contains(HabModuleSpecialRule.Solar_Power_Variable_Output);
			}
			return false;
		}
	}
	public bool IsNonSolarPower {
		get {
			if (powerSource) {
				return !IsSolarPower;
			}
			return false;
		}
	}
	public bool PowerFirst => specialRules.Contains(HabModuleSpecialRule.PowerFirst);
	public string description => Loc.T(new StringBuilder("TIHabModuleTemplate.description.").Append(base.dataName).ToString());
	public List<HabModuleSpecialRule> SpecialRules {
		get {
			if (_specialRules == null) {
				_specialRules = specialRules.Where((HabModuleSpecialRule x) => x != HabModuleSpecialRule.none).ToList();
			}
			return _specialRules;
		}
	}
	public string extendedDescription {
		get {
			StringBuilder stringBuilder = new StringBuilder(description);
			bool flag = false;
			foreach (HabModuleSpecialRule specialRule in SpecialRules) {
				if (!flag) {
					stringBuilder.AppendLine();
					flag = true;
				}
				string value = Loc.T(new StringBuilder("HabModuleSpecialRule.").Append(specialRule.ToString()).ToString(), specialRulesValue.ToString("N0"), specialRulesValue.ToPercent("P0"), specialRulesValue.ToString("N2"));
				if (!string.IsNullOrEmpty(value)) {
					stringBuilder.AppendLine(value);
				}
			}
			return stringBuilder.ToString().TrimEnd();
		}
	}
	public string constructionModelResource {
		get {
			switch (tier) {
				default:
					if (alienModule) {
						return TemplateManager.global.station_alien_underconstruction_t1_module;
					}
					return TemplateManager.global.station_human_underconstruction_t1_module;
				case 2:
					if (alienModule) {
						return TemplateManager.global.station_alien_underconstruction_t2_module;
					}
					return TemplateManager.global.station_human_underconstruction_t2_module;
				case 3:
					if (alienModule) {
						return TemplateManager.global.station_alien_underconstruction_t3_module;
					}
					return TemplateManager.global.station_human_underconstruction_t3_module;
			}
		}
	}
	public string constructionModelDestructionResource {
		get {
			switch (tier) {
				default:
					if (alienModule) {
						return TemplateManager.global.station_alien_underconstruction_t1_module_destruction;
					}
					return TemplateManager.global.station_human_underconstruction_t1_module_destruction;
				case 2:
					if (alienModule) {
						return TemplateManager.global.station_alien_underconstruction_t2_module_destruction;
					}
					return TemplateManager.global.station_human_underconstruction_t2_module_destruction;
				case 3:
					if (alienModule) {
						return TemplateManager.global.station_alien_underconstruction_t3_module_destruction;
					}
					return TemplateManager.global.station_human_underconstruction_t3_module_destruction;
			}
		}
	}
	public TIProjectTemplate RequiredProject {
		get {
			if (!string.IsNullOrEmpty(requiredProjectName)) {
				TIProjectTemplate tIProjectTemplate = TemplateManager.Find<TIProjectTemplate>(requiredProjectName);
				if (tIProjectTemplate == null) {
					Log.Error("Bad project templateName " + requiredProjectName + " in " + base.dataName);
				}
				return tIProjectTemplate;
			}
			return null;
		}
	}
	public TIHabModuleTemplate UpgradesFrom {
		get {
			if (_upgradesFrom == null && !string.IsNullOrEmpty(upgradesFromName)) {
				TIHabModuleTemplate tIHabModuleTemplate = TemplateManager.Find<TIHabModuleTemplate>(upgradesFromName);
				if (tIHabModuleTemplate == null) {
					Log.Error("Bad upgradesFromName " + upgradesFromName + " in " + base.dataName);
				}
				_upgradesFrom = tIHabModuleTemplate;
			}
			return _upgradesFrom;
		}
	}
	public TIHabModuleTemplate UpgradesTo {
		get {
			if (!_upgradeToChecked) {
				_upgradeToChecked = true;
				foreach (TIHabModuleTemplate item in TemplateManager.IterateByClass<TIHabModuleTemplate>()) {
					if (item.UpgradesFrom == this) {
						_upgradesTo = item;
						break;
					}
				}
			}
			return _upgradesTo;
		}
	}
	public float moduleConstructionSpeedModifier {
		get {
			if (allowsShipConstruction) {
				return 1f;
			}
			return constructionTimeModifier;
		}
	}
	public bool powerSource => power > 0;
	public int powerConsumed {
		get {
			if (power <= 0) {
				return -power;
			}
			return 0;
		}
	}
	public bool CanTurnOff => !coreModule;
	public string benefitsAndCostsDescription(TIFactionState faction, TIHabState hab) {
		TIGlobalConfig global = TemplateManager.global;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(Loc.T("UI.Habs.SimpleList", Loc.T("UI.Habs.Tier", tier.ToString("N0")), Loc.T("UI.Habs.Crew", crew.ToString("N0")), Loc.T("UI.Habs.Mass", Mass_tons(0f, GameStateManager.Luna(), GameControl.control.activePlayer).ToString("N0"))));
		List<IncomeEntry> list = new List<IncomeEntry>();
		List<IncomeEntry> list2 = new List<IncomeEntry>();
		int num = ((!(hab == null)) ? ProspectivePower(hab) : ProspectivePower(GameStateManager.Luna(), GameControl.control.activePlayer));
		if (num > 0) {
			list.Add(new IncomeEntry(global.habPowerInlineSpritePath, num.ToString("N0")));
		} else {
			list2.Add(new IncomeEntry(global.habPowerAlertInlineSpritePath, num.ToString("N0")));
		}
		Dictionary<FactionResource, float> dictionary = new Dictionary<FactionResource, float>();
		Dictionary<FactionResource, float> dictionary2 = new Dictionary<FactionResource, float>();
		FactionResource[] factionResources = Enums.FactionResources;
		foreach (FactionResource factionResource in factionResources) {
			dictionary[factionResource] = MonthlyResourceIncome(factionResource, hab, GameControl.control.activePlayer);
			if (dictionary[factionResource] != 0f) {
				string v = TIUtilities.FormatBigOrSmallNumber(dictionary[factionResource], 1, 3, 0, useSmallPrefixes: true);
				if (dictionary[factionResource] > 0f) {
					list.Add(new IncomeEntry(TIUtilities.InlineResourceStr(factionResource), v));
				} else {
					list2.Add(new IncomeEntry(TIUtilities.InlineResourceStr(factionResource), v));
				}
			}
			dictionary2[factionResource] = MonthlySupportCost(factionResource, includeCrewSupportCost: true, faction, hab);
			if (dictionary2[factionResource] != 0f) {
				string v2 = TIUtilities.FormatBigOrSmallNumber(dictionary2[factionResource], 1, 3, 0, useSmallPrefixes: true);
				list2.Add(new IncomeEntry(TIUtilities.InlineResourceStr(factionResource), v2));
			}
		}
		TechCategory[] techCategories = Enums.TechCategories;
		foreach (TechCategory category in techCategories) {
			float techBonusByCategory = GetTechBonusByCategory(category);
			string v3 = techBonusByCategory.ToPercent("P0");
			if (techBonusByCategory != 0f) {
				list.Add(new IncomeEntry(TIGenericTechTemplate.categoryInlineSprite(category), v3));
			}
		}
		if (allowsResupply) {
			list.Add(new IncomeEntry(global.habResupplyPresentInlineSpritePath, string.Empty));
		}
		float num2 = moduleConstructionSpeedModifier;
		if (num2 > 1f) {
			list.Add(new IncomeEntry(global.habModuleConstructionInlineSpritePath, (1f - num2).ToPercent("P0")));
		}
		if (allowsShipConstruction) {
			list.Add(new IncomeEntry(global.habShipyardPresentInlineSpritePath, string.Empty));
		}
		float num3 = BaseSpaceCombatValue(GameControl.control.activePlayer, hab, hab != null);
		if (num3 != 0f) {
			string v4 = num3.ToString("N0");
			list.Add(new IncomeEntry(global.habDefenseScoreInlineSpritePath, v4));
		}
		if (list.Count > 0) {
			stringBuilder.AppendLine(Loc.T("UI.Habs.IncomeAndBonuses"));
			foreach (IncomeEntry item in list) {
				stringBuilder.Append(item.inlinePath).Append(item.value).Append(" ");
			}
			stringBuilder.AppendLine();
		}
		if (list2.Count > 0) {
			stringBuilder.AppendLine(Loc.T("UI.Habs.Costs"));
			foreach (IncomeEntry item2 in list2) {
				stringBuilder.Append(item2.inlinePath).Append(item2.value).Append(" ");
			}
			stringBuilder.AppendLine();
		}
		return stringBuilder.ToString();
	}
	public string iconResource(HabType habType) {
		if (habType != 0) {
			return baseIconResource;
		}
		return stationIconResource;
	}
	public string constructionIconResource(HabType habType) {
		string result = string.Empty;
		if (habType == HabType.Station) {
			switch (tier) {
				case 1:
					result = (alienModule ? TemplateManager.global.station_alien_underconstruction_t1_icon : TemplateManager.global.station_human_underconstruction_t1_icon);
					break;
				case 2:
					result = (alienModule ? TemplateManager.global.station_alien_underconstruction_t2_icon : TemplateManager.global.station_human_underconstruction_t2_icon);
					break;
				case 3:
					result = (alienModule ? TemplateManager.global.station_alien_underconstruction_t3_icon : TemplateManager.global.station_human_underconstruction_t3_icon);
					break;
			}
		} else if (mine) {
			result = (alienModule ? "habmodules/base_T3_AlienUnderconstruction" : "habmodules/base_T3_underconstruction");
		} else {
			switch (tier) {
				case 1:
					result = (alienModule ? "habmodules/base_T1_AlienUnderconstruction" : "habmodules/base_T1_underconstruction");
					break;
				case 2:
					result = (alienModule ? "habmodules/base_T2_AlienUnderconstruction" : "habmodules/base_T2_underconstruction");
					break;
				case 3:
					result = (alienModule ? "habmodules/base_T3_AlienUnderconstruction" : "habmodules/base_T3_underconstruction");
					break;
			}
		}
		return result;
	}
	public static float HabModule_Mass_Modifier(TIHabModuleTemplate module, float irradiatedValue, TISpaceBodyState surfaceBody, TIFactionState faction) {
		return module.Mass_tons(irradiatedValue, surfaceBody, faction) / module.baseMass_tons;
	}
	public float Mass_tons(float irradiatedValue, TISpaceBodyState surfaceBody, TIFactionState faction) {
		float num = baseMass_tons;
		if (irradiatedValue > 1f) {
			num *= irradiatedValue;
		}
		if (SpecialRules.Contains(HabModuleSpecialRule.Cost_Scales_With_Gravity)) {
			num = num * 0.5f + num * 0.5f * (float)surfaceBody.relativeEnergyForMining(faction);
		}
		return num;
	}
	public float BaseSpaceCombatValue(TIFactionState faction, TIHabState hab, bool fullyCalculate) {
		if (fullyCalculate && hab != null && spaceCombatValue > 0) {
			TIShipWeaponTemplate tIShipWeaponTemplate = TemplateManager.Find<TIShipWeaponTemplate>(faction.GetBestHabWeapon(hab.IsBase, tier, preferLaser: true), allowChild: true);
			TIShipWeaponTemplate tIShipWeaponTemplate2 = TemplateManager.Find<TIShipWeaponTemplate>(faction.GetBestHabWeapon(hab.IsBase, tier, preferLaser: false), allowChild: true);
			return (float)tier * (tIShipWeaponTemplate.Score() + tIShipWeaponTemplate2.Score());
		}
		return spaceCombatValue;
	}
	public ResourceCostBuilder BuildMaterials(float irradiatedValue, TISpaceBodyState spaceBody, TIFactionState faction, float multiplier) {
		float num = Mass_tons(1f, spaceBody, faction);
		float num2 = Mass_tons(irradiatedValue, spaceBody, faction) - num;
		ResourceCostBuilder result = default(ResourceCostBuilder);
		result.water = weightedBuildMaterials.water * num * TemplateManager.global.spaceResourceToTons * multiplier;
		result.volatiles = weightedBuildMaterials.volatiles * num * TemplateManager.global.spaceResourceToTons * multiplier;
		result.metals = weightedBuildMaterials.metals * (num + num2) * TemplateManager.global.spaceResourceToTons * multiplier;
		result.nobleMetals = weightedBuildMaterials.nobleMetals * num * TemplateManager.global.spaceResourceToTons * multiplier;
		result.fissiles = weightedBuildMaterials.fissiles * num * TemplateManager.global.spaceResourceToTons * multiplier;
		result.antimatter = weightedBuildMaterials.antimatter * num * TemplateManager.global.spaceResourceToTons * multiplier;
		result.exotics = weightedBuildMaterials.exotics * num * TemplateManager.global.spaceResourceToTons * multiplier;
		return result;
	}
	public float MoneyCost(float irradiatedValue, TISpaceBodyState spaceBody, TIFactionState faction, float rateMultiplier, List<ResourceValue> preSuppliedResources = null) {
		float num = 0f;
		ResourceCostBuilder resourceCostBuilder = BuildMaterials(irradiatedValue, spaceBody, faction, rateMultiplier);
		if (preSuppliedResources != null) {
			foreach (ResourceValue preSuppliedResource in preSuppliedResources) {
				switch (preSuppliedResource.resource) {
					case FactionResource.Water:
						resourceCostBuilder.water -= preSuppliedResource.value;
						break;
					case FactionResource.Volatiles:
						resourceCostBuilder.volatiles -= preSuppliedResource.value;
						break;
					case FactionResource.Metals:
						resourceCostBuilder.metals -= preSuppliedResource.value;
						break;
					case FactionResource.NobleMetals:
						resourceCostBuilder.nobleMetals -= preSuppliedResource.value;
						break;
					case FactionResource.Fissiles:
						resourceCostBuilder.fissiles -= preSuppliedResource.value;
						break;
					case FactionResource.Antimatter:
						resourceCostBuilder.antimatter -= preSuppliedResource.value;
						break;
					case FactionResource.Exotics:
						resourceCostBuilder.exotics -= preSuppliedResource.value;
						break;
				}
			}
		}
		num += resourceCostBuilder.water * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(FactionResource.Water);
		num += resourceCostBuilder.volatiles * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(FactionResource.Volatiles);
		num += resourceCostBuilder.metals * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(FactionResource.Metals);
		num += resourceCostBuilder.nobleMetals * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(FactionResource.NobleMetals);
		num += resourceCostBuilder.fissiles * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(FactionResource.Fissiles);
		num += resourceCostBuilder.antimatter * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(FactionResource.Antimatter);
		return num + resourceCostBuilder.exotics * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(FactionResource.Exotics);
	}
	public float BoostCostFromEarth(float irradiatedValue, TISpaceBodyState spaceBody, TIFactionState faction, TIGameState destination, float rateMultiplier, List<ResourceValue> preSuppliedResources = null) {
		float num = 0f;
		ResourceCostBuilder resourceCostBuilder = BuildMaterials(irradiatedValue, spaceBody, faction, rateMultiplier);
		if (preSuppliedResources != null) {
			foreach (ResourceValue preSuppliedResource in preSuppliedResources) {
				switch (preSuppliedResource.resource) {
					case FactionResource.Water:
						resourceCostBuilder.water -= preSuppliedResource.value;
						break;
					case FactionResource.Volatiles:
						resourceCostBuilder.volatiles -= preSuppliedResource.value;
						break;
					case FactionResource.Metals:
						resourceCostBuilder.metals -= preSuppliedResource.value;
						break;
					case FactionResource.NobleMetals:
						resourceCostBuilder.nobleMetals -= preSuppliedResource.value;
						break;
					case FactionResource.Fissiles:
						resourceCostBuilder.fissiles -= preSuppliedResource.value;
						break;
					case FactionResource.Antimatter:
						resourceCostBuilder.antimatter -= preSuppliedResource.value;
						break;
					case FactionResource.Exotics:
						resourceCostBuilder.exotics -= preSuppliedResource.value;
						break;
				}
			}
		}
		num += resourceCostBuilder.water;
		num += resourceCostBuilder.volatiles;
		num += resourceCostBuilder.metals;
		num += resourceCostBuilder.nobleMetals;
		num += resourceCostBuilder.fissiles;
		num += resourceCostBuilder.antimatter;
		num += resourceCostBuilder.exotics;
		return (float)TISpaceObjectState.GenericTransferBoostFromEarthSurface(faction, destination, num / TemplateManager.global.spaceResourceToTons);
	}
	public TIResourcesCost CostFromEarth(TIFactionState faction, TIGameState destinationState, bool isUpgrade) {
		float irradiatedValue = TIUtilities.IrradiatedMultiplier(destinationState, strict: true);
		float num = (isUpgrade ? upgradeDiscount : 1f);
		TIResourcesCost tIResourcesCost = new TIResourcesCost();
		TISpaceBodyState spaceBody = destinationState.ref_spaceBody;
		float num2 = 1f;
		TIHabState tIHabState = destinationState as TIHabState;
		if (tIHabState != null) {
			num2 = tIHabState.GetModuleConstructionTimeModifier();
			if (tIHabState.IsBase) {
				spaceBody = tIHabState.habSite.parentBody;
			}
		} else {
			TIHabSiteState tIHabSiteState = destinationState as TIHabSiteState;
			if (tIHabSiteState != null) {
				spaceBody = tIHabSiteState.parentBody;
			}
		}
		tIResourcesCost.AddCost(FactionResource.Boost, BoostCostFromEarth(irradiatedValue, spaceBody, faction, destinationState, num));
		tIResourcesCost.AddCost(FactionResource.Money, MoneyCost(irradiatedValue, spaceBody, faction, num));
		float num3 = TISpaceObjectState.GenericTransferTimeFromEarthsSurface_d(faction, destinationState);
		float num4 = buildTime_Days * num * num2 + num3 + TIEffectsState.SumEffectsModifiers(Context.GenericModuleTransferTime, faction, num3);
		if (tIHabState != null && tIHabState.coreModule.underConstruction && tIHabState.tier <= tier) {
			num4 = Mathf.Max(num4, 0f - (float)TITimeState.Now().DifferenceInDays(new TIDateTime(tIHabState.coreModule.completionDate)));
		}
		tIResourcesCost.SetCompletionTime_Days(num4);
		return tIResourcesCost;
	}
	public TIResourcesCost CostFromSpace(TIFactionState faction, TIGameState destinationState, bool isUpgrade, bool substituteBoost, TIHabModuleState moduleBeingUpgraded, int maxDaysToSave = 0, bool dontRecalculateIncome = false) {
		float irradiatedValue = TIUtilities.IrradiatedMultiplier(destinationState, strict: true);
		float num = (isUpgrade ? upgradeDiscount : 1f);
		TISpaceBodyState ref_spaceBody = destinationState.ref_spaceBody;
		float num2 = 1f;
		TIHabState tIHabState = null;
		if (destinationState.isHabSiteState) {
			ref_spaceBody = destinationState.ref_habSite.ref_spaceBody;
		} else if (destinationState.isHabState) {
			tIHabState = destinationState.ref_hab;
			num2 = tIHabState.GetModuleConstructionTimeModifier();
			if (tIHabState.IsBase) {
				ref_spaceBody = tIHabState.habSite.ref_spaceBody;
			}
		}
		TIResourcesCost tIResourcesCost = BuildMaterials(irradiatedValue, ref_spaceBody, faction, num).ToResourcesCost();
		float num3 = 0f;
		if (substituteBoost && !tIResourcesCost.CanAfford(faction) && (faction.IsActiveHumanFaction || GameStateManager.AlienNation().extant)) {
			List<ResourceValue> list = new List<ResourceValue>();
			List<ResourceValue> list2 = new List<ResourceValue>();
			foreach (ResourceValue resourceCost in tIResourcesCost.resourceCosts) {
				float currentResourceAmount = faction.GetCurrentResourceAmount(resourceCost.resource);
				float dailyIncome = faction.GetDailyIncome(resourceCost.resource, dontRecalculateIncome);
				if (currentResourceAmount < resourceCost.value && currentResourceAmount + dailyIncome * (float)maxDaysToSave < resourceCost.value) {
					list2.Add(resourceCost);
				} else {
					list.Add(resourceCost);
				}
			}
			if (list.Count > 0) {
				float resourceAmount = MoneyCost(irradiatedValue, ref_spaceBody, faction, num, list);
				float resourceAmount2 = BoostCostFromEarth(irradiatedValue, ref_spaceBody, faction, destinationState, num, list);
				tIResourcesCost.AddCost(FactionResource.Money, resourceAmount);
				tIResourcesCost.AddCost(FactionResource.Boost, resourceAmount2);
				float num4 = TISpaceObjectState.GenericTransferTimeFromEarthsSurface_d(faction, destinationState);
				num4 += TIEffectsState.SumEffectsModifiers(Context.GenericModuleTransferTime, faction, num4);
				if (num4 > num3) {
					num3 = num4;
				}
				foreach (ResourceValue item in list2) {
					tIResourcesCost.RemoveCost(item.resource);
				}
			}
		}
		float num5 = buildTime_Days * num * num2 + num3;
		if (tIHabState != null && tIHabState.coreModule.underConstruction && tIHabState.tier <= tier) {
			num5 = Mathf.Max(num5, 0f - (float)TITimeState.Now().DifferenceInDays(new TIDateTime(tIHabState.coreModule.completionDate)));
		}
		tIResourcesCost.SetCompletionTime_Days(num5);
		return tIResourcesCost;
	}
	public TIResourcesCost MinimumBoostCost(TIFactionState faction, TIGameState location, bool isUpgrade = false, int maxDaysToSave = 180) {
		if (faction.IsAlienFaction) {
			return CostFromSpace(faction, location, isUpgrade, substituteBoost: false, null, maxDaysToSave, dontRecalculateIncome: true);
		}
		if (!coreModule || isUpgrade || faction.CanFoundHabFromHabAtLocation(location)) {
			TIResourcesCost tIResourcesCost = CostFromSpace(faction, location, isUpgrade, substituteBoost: true, null, maxDaysToSave, dontRecalculateIncome: true);
			if (maxDaysToSave == 0) {
				if (tIResourcesCost.CanAfford_AI(faction, this, location)) {
					return tIResourcesCost;
				}
			} else if (tIResourcesCost.CanPayInFuture(faction, maxDaysToSave)) {
				return tIResourcesCost;
			}
		}
		return CostFromEarth(faction, location, isUpgrade);
	}
	public TIResourcesCost MinimumBoostCostToday(TIFactionState faction, TIGameState location, bool isUpgrade = false) {
		return MinimumBoostCost(faction, location, isUpgrade, 0);
	}
	public bool AllowedForFaction(TIFactionState faction) {
		return alienModule == faction.IsAlienFaction;
	}
	public bool IsForHabType(HabType testHabType) {
		if (habType != testHabType) {
			return habType == HabType.Any;
		}
		return true;
	}
	public bool ModuleTierIsAllowed(TIHabState hab) {
		if (tier > hab.tier) {
			return coreModule;
		}
		return true;
	}
	public bool AllowedForHabAutomatedStatus(TIHabState hab) {
		if (automated) {
			if (!hab.sectors[0].habModules[0].moduleTemplate.automated) {
				return hab.sectors[0].habModules[0].moduleTemplate == null;
			}
			return true;
		}
		if (hab.sectors[0].habModules[0].moduleTemplate.automated) {
			return hab.sectors[0].habModules[0].moduleTemplate == null;
		}
		return true;
	}
	public bool AllowedLocation(TIGameState habLocation) {
		if (SpecialRules.Contains(HabModuleSpecialRule.EarthLEOOnly) && !habLocation.ref_orbit.isEarthLEO) {
			return false;
		}
		if (SpecialRules.Contains(HabModuleSpecialRule.Requires_Inhabited_Body)) {
			if (habLocation.ref_naturalSpaceObject.GetSunOrbitingRelatedObject.isEarth) {
				return true;
			}
			TISpaceBodyState ref_spaceBody = habLocation.ref_spaceBody;
			if ((object)ref_spaceBody != null && ref_spaceBody.population >= 50000) {
				return true;
			}
			return false;
		}
		SpecialRules.Contains(HabModuleSpecialRule.NotInIrradiated);
		if (SpecialRules.Contains(HabModuleSpecialRule.HarvestAntimatter) && habLocation.ref_orbit.amat_ugpy <= 0f) {
			return false;
		}
		return true;
	}
	public bool FactionCanBuild(TIFactionState faction) {
		if (noBuild || (alienModule && !faction.IsAlienFaction) || (!alienModule && faction.IsAlienFaction)) {
			return false;
		}
		if (RequiredProject == null) {
			return true;
		}
		return faction.completedProjects.Contains(RequiredProject);
	}
	public float MonthlySupportCost(FactionResource resource, bool includeCrewSupportCost = true, TIFactionState faction = null, TIHabState hab = null) {
		switch (resource) {
			case FactionResource.Money:
				return supportMaterials_month.money + (includeCrewSupportCost ? MonthlyCrewSupportCost(resource, faction, hab) : 0f);
			case FactionResource.Boost:
				return supportMaterials_month.boost;
			case FactionResource.Water:
				return supportMaterials_month.water + (includeCrewSupportCost ? MonthlyCrewSupportCost(resource, faction, hab) : 0f);
			case FactionResource.Volatiles:
				return supportMaterials_month.volatiles + (includeCrewSupportCost ? MonthlyCrewSupportCost(resource, faction, hab) : 0f);
			case FactionResource.Antimatter:
				return supportMaterials_month.antimatter;
			case FactionResource.Exotics:
				return supportMaterials_month.exotics;
			case FactionResource.Fissiles:
				return supportMaterials_month.fissiles;
			case FactionResource.Metals:
				return supportMaterials_month.metals;
			case FactionResource.NobleMetals:
				return supportMaterials_month.nobleMetals;
			default:
				return 0f;
		}
	}
	public float MonthlyCrewSupportCost(FactionResource resource, TIFactionState faction = null, TIHabState hab = null) {
		switch (resource) {
			case FactionResource.Money:
				if (!(faction == null) && faction.IsAlienFaction) {
					return 0f;
				}
				return (float)crew * TemplateManager.global.crewSalary_year / 12f;
			case FactionResource.Water:
				return (float)crew * TemplateManager.global.crewWaterConsumptionTons_year * TemplateManager.global.spaceResourceToTons / 12f;
			case FactionResource.Volatiles:
				return (float)crew * TemplateManager.global.crewVolatilesConsumptionTons_year * TemplateManager.global.spaceResourceToTons / 12f;
			default:
				return 0f;
		}
	}
	public float YearlySupportCost(FactionResource resource, bool includeCrewSupportCost = true, TIFactionState faction = null, TIHabState hab = null) {
		return MonthlySupportCost(resource, includeCrewSupportCost, faction, hab) * 12f;
	}
	public float DailySupportCost(FactionResource resource, bool includeCrewSupportCost = true, TIFactionState faction = null, TIHabState hab = null) {
		return YearlySupportCost(resource, includeCrewSupportCost, faction, hab) / 365.2422f;
	}
	public float YearlyResourceIncome(FactionResource resource, TIHabState hab = null, TIFactionState faction = null) {
		if (resource == FactionResource.Projects || resource == FactionResource.MissionControl) {
			return MonthlyResourceIncome(resource, hab, faction);
		}
		return MonthlyResourceIncome(resource, hab, faction) * 12f;
	}
	public float DailyResourceIncome(FactionResource resource, TIHabState hab = null, TIFactionState faction = null) {
		if (resource == FactionResource.Projects || resource == FactionResource.MissionControl) {
			return MonthlyResourceIncome(resource, hab, faction);
		}
		return YearlyResourceIncome(resource, hab, faction) / 365.2422f;
	}
	public float MonthlyResourceIncome(FactionResource resource, TIHabState hab = null, TIFactionState faction = null) {
		switch (resource) {
			case FactionResource.Antimatter: {
				float num = incomeAntimatter_month;
				if (hab != null && SpecialRules.Contains(HabModuleSpecialRule.HarvestAntimatter)) {
					num += specialRulesValue * (hab.orbitState.amat_ugpy / 12f * 1E-12f) * TemplateManager.global.spaceResourceToTons;
				}
				return num;
			}
			case FactionResource.Boost:
				return incomeBoost_month;
			case FactionResource.Money:
				if (SpecialRules.Contains(HabModuleSpecialRule.MoneyIfNotBuilding) && hab != null && hab.AllModules().Any((TIHabModuleState x) => x.underConstruction)) {
					return 0f;
				}
				if (supportMaterials_month.boost > 0f && faction != null && hab != null && faction.GetCurrentResourceAmount(FactionResource.Boost) <= 0f && faction.GetDailyIncome(FactionResource.Boost) <= 0f) {
					return 0f;
				}
				return incomeMoney_month;
			case FactionResource.Exotics:
				if (faction.IsAlienFaction) {
					return incomeExotics_month * (float)GameStateManager.GlobalValues().difficulty;
				}
				return incomeExotics_month;
			case FactionResource.Influence:
				return incomeInfluence_month;
			case FactionResource.Water:
			case FactionResource.Volatiles:
			case FactionResource.Metals:
				if (hab == null || faction == null) {
					return 0f;
				}
				return GetMiningIncome_Month(faction, hab, resource);
			case FactionResource.Fissiles:
				if (hab == null || faction == null) {
					return 0f;
				}
				return incomeFissiles_month + GetMiningIncome_Month(faction, hab, resource);
			case FactionResource.NobleMetals:
				if (hab == null || faction == null) {
					return 0f;
				}
				return incomeNobles_month + GetMiningIncome_Month(faction, hab, resource);
			case FactionResource.MissionControl:
				return missionControl;
			case FactionResource.Operations:
				return incomeOps_month;
			case FactionResource.Projects:
				return incomeProjects;
			case FactionResource.Research:
				return incomeResearch_month;
			default:
				return 0f;
		}
	}
	public TIHabModuleTemplate UpgradeModuleTemplate(TIFactionState faction, bool checkUnlocked) {
		if (checkUnlocked && UpgradesTo != null && UpgradesTo.FactionCanBuild(faction)) {
			return UpgradesTo;
		}
		return null;
	}
	public bool CanUpgrade(TIFactionState faction) {
		return UpgradeModuleTemplate(faction, checkUnlocked: true) != null;
	}
	public float GetTechBonusByCategory(TechCategory category) {
		float num = 0f;
		TechBonus[] array = techBonuses;
		for (int i = 0; i < array.Length; i++) {
			TechBonus techBonus = array[i];
			if (techBonus.category == category) {
				num += techBonus.bonus;
			}
		}
		return num;
	}
	public TIProjectTemplate GetProjectUnlocked() {
		if (!string.IsNullOrEmpty(unlocksProjectName)) {
			return TemplateManager.Find<TIProjectTemplate>(unlocksProjectName);
		}
		return null;
	}
	public float GetMiningIncome_Day(TIFactionState faction, TIHabState hab, FactionResource resource) {
		if (!mine) {
			return 0f;
		}
		float currentMiningMultiplierFromOrgsAndEffects = faction.GetCurrentMiningMultiplierFromOrgsAndEffects(resource);
		return hab.habSite.GetDailyProduction(resource) * miningModifier * currentMiningMultiplierFromOrgsAndEffects;
	}
	public float GetMiningIncome_Year(TIFactionState faction, TIHabState hab, FactionResource resource) {
		if (!mine) {
			return 0f;
		}
		return GetMiningIncome_Day(faction, hab, resource) * 365.2422f;
	}
	public float GetMiningIncome_Month(TIFactionState faction, TIHabState hab, FactionResource resource) {
		if (!mine) {
			return 0f;
		}
		return GetMiningIncome_Year(faction, hab, resource) / 12f;
	}
	public float shipyardConstructionSpeedModifier() {
		if (allowsShipConstruction) {
			return constructionTimeModifier;
		}
		return 1f;
	}
	public bool CombatTroops() {
		return SpecialRules.Intersect(combatTroopsRules).Any();
	}
	public int ProspectivePower(TISpaceBodyState spaceBody, TIFactionState faction) {
		for (int i = 0; i < SpecialRules.Count; i++) {
			if (SpecialRules[i] == HabModuleSpecialRule.Solar_Power_Variable_Output) {
				return TIHabModuleState.ProspectiveSolarPowerOutput(spaceBody, power);
			}
			if (SpecialRules[i] == HabModuleSpecialRule.Cost_Scales_With_Gravity) {
				return TIHabModuleState.EscapeVelocityBasedPowerRequirement(spaceBody, this, faction);
			}
		}
		return power;
	}
	public int ProspectivePower(TIHabSiteState site, TIFactionState faction) {
		for (int i = 0; i < SpecialRules.Count; i++) {
			if (SpecialRules[i] == HabModuleSpecialRule.Solar_Power_Variable_Output) {
				return TIHabModuleState.ProspectiveSolarPowerOutput(site, power);
			}
			if (SpecialRules[i] == HabModuleSpecialRule.Cost_Scales_With_Gravity) {
				return TIHabModuleState.EscapeVelocityBasedPowerRequirement(site, this, faction);
			}
		}
		return power;
	}
	public int ProspectivePower(TIOrbitState orbit) {
		for (int i = 0; i < SpecialRules.Count; i++) {
			if (SpecialRules[i] == HabModuleSpecialRule.Solar_Power_Variable_Output) {
				return TIHabModuleState.ProspectiveSolarPowerOutput(orbit, power);
			}
		}
		return power;
	}
	public int ProspectivePower(TIHabState hab) {
		for (int i = 0; i < SpecialRules.Count; i++) {
			if (SpecialRules[i] == HabModuleSpecialRule.Solar_Power_Variable_Output) {
				return TIHabModuleState.SolarPowerOutput(hab, power);
			}
			if (SpecialRules[i] == HabModuleSpecialRule.Cost_Scales_With_Gravity) {
				return TIHabModuleState.EscapeVelocityBasedPowerRequirement(hab, this, hab.faction);
			}
		}
		return power;
	}
}
namespace PavonisInteractive.TerraInvicta {
	public interface CombatWeaponCarrierState {
		TIGameState GetTargetableState();

		TIFactionState GetFaction();

		bool WeaponCanFire(ModuleDataEntry weaponData);

		void FireWeapon(ModuleDataEntry module, TISpaceCombatProjectileState targetedProjectile = null);

		void AddTargetedProjectile(TISpaceCombatProjectileState projectile);

		float FireControlFunction();

		TISpaceShipState ref_shipCarrier();

		TIHabModuleState ref_habModuleCarrier();

		bool isShip();

		bool isHabModule();
	}
	[MonoModReplace] public class TIHabModuleState : TIGameState, CombatWeaponCarrierState, CombatTargetableState {
		public bool C0;
		public bool N1;
		public bool N2;
		public bool E1;
		public bool E2;
		public bool W1;
		public bool W2;
		public bool S1;
		public bool S2;
		public TIResourcesCost buildCost;
		private TIHabModuleTemplate _priorModuleTemplate;
		private BeamWeapon weapon;
		[SerializeField]
		private TIDateTime lastTimeFired;
		public bool constructionCompleted { get; private set; }
		public DateTime completionDate { get; private set; }
		public bool decommissioning { get; private set; }
		public DateTime decommissionDate { get; private set; }
		public bool powered { get; private set; }
		public int slot { get; private set; }
		public TISectorState sector { get; private set; }
		public bool destroyed { get; private set; }
		public bool isCombatModule {
			get {
				TIHabModuleTemplate tIHabModuleTemplate = moduleTemplate;
				if (tIHabModuleTemplate == null) {
					return false;
				}
				return tIHabModuleTemplate.spaceCombatValue > 0;
			}
		}
		public string defenseWeaponTemplateName { get; private set; }
		public string defenseWeaponTemplateName_gun { get; private set; }
		public float _spaceCombatValue { get; private set; }
		public string priorModuleTemplateName { get; private set; }
		public bool priorModuleCompleted { get; private set; }
		public TIDateTime priorModuleCompletionDate { get; private set; }
		[fsIgnore]
		public TIHabModuleTemplate moduleTemplate { get; private set; }
		[fsIgnore]
		public TIShipWeaponTemplate defenseWeapon { get; private set; }
		[fsIgnore]
		public TIShipWeaponTemplate defenseWeapon_gun { get; private set; }
		public bool empty => moduleTemplate == null;
		public bool underConstruction {
			get {
				if (!empty) {
					return !constructionCompleted;
				}
				return false;
			}
		}
		public bool hasModule => !empty;
		public bool completed {
			get {
				if (!empty) {
					return constructionCompleted;
				}
				return false;
			}
		}
		public bool okay {
			get {
				if (!empty && !destroyed) {
					return !decommissioning;
				}
				return false;
			}
		}
		public bool functional {
			get {
				if (completed && !destroyed) {
					return !decommissioning;
				}
				return false;
			}
		}
		public bool active {
			get {
				if (functional) {
					return powered;
				}
				return false;
			}
		}
		public bool present {
			get {
				if (!empty) {
					return !destroyed;
				}
				return false;
			}
		}
		public bool mineLocation {
			get {
				if (hab.IsBase && sectorNum == 0) {
					return slot == 1;
				}
				return false;
			}
		}
		public int sectorNum => sector.sectorNum;
		public int tier => moduleTemplate.tier;
		public TIHabState hab => sector?.hab;
		public override bool isHabModuleState => true;
		public override TIFactionState ref_faction => sector.ref_faction;
		public override TIHabState ref_hab => sector.hab;
		public override TIHabSiteState ref_habSite => sector.ref_habSite;
		public override TIOrbitState ref_orbit => sector.ref_orbit;
		public override TINaturalSpaceObjectState ref_naturalSpaceObject => sector.ref_naturalSpaceObject;
		public override TISpaceBodyState ref_spaceBody => sector.ref_spaceBody;
		public override TISpaceObjectState ref_spaceObject => sector.ref_spaceObject;
		public override TISpaceAssetState ref_spaceAsset => sector.ref_spaceAsset;
		public override TIHabModuleState ref_habModule => this;
		public override bool hasMapObject => true;
		public override bool inSpace => true;
		public int crew {
			get {
				if (moduleTemplate == null || destroyed) {
					return 0;
				}
				if (underConstruction) {
					if (priorModuleTemplate == null) {
						return 0;
					}
					return priorModuleTemplate.crew;
				}
				return moduleTemplate.crew;
			}
		}
		public HabModuleController HabModuleController => hab.controller.GetComponentInChildren<HabModelController>(includeInactive: true).GetModuleControllers().FirstOrDefault((HabModuleController x) => x.module == this);
		public TIHabModuleTemplate priorModuleTemplate {
			get {
				if (_priorModuleTemplate == null && !string.IsNullOrEmpty(priorModuleTemplateName)) {
					_priorModuleTemplate = TemplateManager.Find<TIHabModuleTemplate>(priorModuleTemplateName);
				}
				return _priorModuleTemplate;
			}
		}
		public int baseHitPoints => (int)(0.35f * moduleTemplate.baseMass_tons / (float)moduleTemplate.tier / ref_faction.GetBestArmor(IsAlien()).mass_damagePoint_kg);
		public string baseSTOFireStr => new StringBuilder("STOFireMission").Append(base.ID.ToString()).ToString();
		public bool IsAlien() {
			return ref_faction.IsAlienFaction;
		}
		public bool isShip() {
			return false;
		}
		public bool isHabModule() {
			return true;
		}
		public TISpaceShipState ref_shipCarrier() {
			return null;
		}
		public TIHabModuleState ref_habModuleCarrier() {
			return this;
		}
		public bool CanUpgrade(TIFactionState faction) {
			if (constructionCompleted) {
				return moduleTemplate.CanUpgrade(faction);
			}
			return false;
		}
		public void InitializeEmpty(TISectorState sector, int slot) {
			this.sector = sector;
			this.slot = slot;
			constructionCompleted = false;
			decommissioning = false;
			powered = false;
			destroyed = false;
			priorModuleTemplateName = string.Empty;
			priorModuleCompleted = false;
			templateName = string.Empty;
			moduleTemplate = null;
			_priorModuleTemplate = null;
		}
		public override void PostGlobalGameStateCreateInit_2() {
			if (moduleTemplate == null && templateName != string.Empty) {
				moduleTemplate = TemplateManager.Find<TIHabModuleTemplate>(templateName);
			}
		}
		public override void PostInitializationInit_4() {
			if (sector == null || sector.deleted || hab == null || hab.deleted) {
				ArchiveState();
			} else if (isCombatModule) {
				SetSpaceCombatWeapon(ref_faction);
				if (hab.underBombardment) {
					InitializeForBombardment(justAddListener: true);
				}
				if (hab.IsBase) {
					GameControl.eventManager.AddListener<STOFireMission_Base>(OnFireMissionOrder, null, this, queueable: false);
					if (lastTimeFired == null) {
						lastTimeFired = TITimeState.Now();
					}
				}
			} else {
				_spaceCombatValue = 0f;
			}
		}
		private void SetModuleTemplate(string newModuleTemplateName) {
			if (!string.IsNullOrEmpty(templateName) && templateName != newModuleTemplateName) {
				priorModuleTemplateName = templateName;
				_priorModuleTemplate = moduleTemplate;
				priorModuleCompleted = completed;
				priorModuleCompletionDate = new TIDateTime(completionDate);
			}
			templateName = newModuleTemplateName;
			moduleTemplate = TemplateManager.Find<TIHabModuleTemplate>(newModuleTemplateName);
			if (moduleTemplate == null) {
				Log.Error("Missing template for moduleName " + templateName);
			}
			displayName = moduleTemplate.displayName;
		}
		public void SetCompletedModule(string moduleTemplateName, bool startup = false) {
			SetModuleTemplate(moduleTemplateName);
			completionDate = World.Active.GetExistingManager<GameTimeManager>().Now;
			CompleteConstruction(startup);
			destroyed = false;
		}
		public void InitiateConstructModule(string moduleTemplateName, TIResourcesCost cost, double selectedCompletionTime_Days) {
			destroyed = false;
			SetModuleTemplate(moduleTemplateName);
			constructionCompleted = false;
			decommissioning = false;
			SetPowerStatus(powerSetting: false);
			if (cost != null) {
				buildCost = new TIResourcesCost(cost);
			}
			completionDate = World.Active.GetExistingManager<GameTimeManager>().Now.AddDays((selectedCompletionTime_Days >= 0.0) ? selectedCompletionTime_Days : ((double)moduleTemplate.buildTime_Days));
		}
		public void CompleteConstruction(bool startup = false) {
			constructionCompleted = true;
			if (moduleTemplate?.coreModule ?? false) {
				hab.anyCoreCompleted = true;
				if (!hab.createdFromTemplate) {
					TIGlobalValuesState.GlobalValues.CheckGlobalMilestoneOnHabFounding(hab, createdFromTemplate: false);
				}
			}
			if (moduleTemplate?.allowsShipConstruction ?? false) {
				sector.faction.AddShipyardToFaction(this, startup);
				if (sector.faction.shipConstructionModules.Count == 1) {
					sector.faction.updateShipDesignsFlag = true;
				}
			}
			TIProjectTemplate tIProjectTemplate = moduleTemplate?.GetProjectUnlocked();
			if (tIProjectTemplate != null) {
				ref_faction.AddAvailableProject(tIProjectTemplate);
			}
			if (isCombatModule && !startup) {
				SetSpaceCombatWeapon(ref_faction);
			}
		}
		public bool PowerProvider() {
			return ModulePower() > 0;
		}
		public bool PowerConsumer() {
			return ModulePower() < 0;
		}
		public int PowerConsumed() {
			return ModulePower() * -1;
		}
		public void SetSpaceCombatWeapon(TIFactionState faction) {
			defenseWeaponTemplateName = faction.GetBestHabWeapon(hab.IsBase, tier, preferLaser: true);
			defenseWeapon = TemplateManager.Find<TIShipWeaponTemplate>(defenseWeaponTemplateName, allowChild: true);
			defenseWeaponTemplateName_gun = faction.GetBestHabWeapon(hab.IsBase, tier, preferLaser: false);
			defenseWeapon_gun = TemplateManager.Find<TIShipWeaponTemplate>(defenseWeaponTemplateName_gun, allowChild: true);
			if (moduleTemplate != null) {
				_spaceCombatValue = moduleTemplate.BaseSpaceCombatValue(faction, hab, fullyCalculate: true);
				_spaceCombatValue += 0.05f * (float)baseHitPoints;
				_spaceCombatValue *= 4.25f;
			} else {
				_spaceCombatValue = 0f;
			}
		}
		public float SpaceCombatValue() {
			return _spaceCombatValue;
		}
		public void SetPowerStatus(bool powerSetting, bool skipFullResourceUpdate = false) {
			bool num = powered;
			if (decommissioning) {
				powered = false;
			} else if (hab.anyCoreCompleted) {
				if (powerSetting && PowerConsumer() && PowerConsumed() > hab.NetPower(includeUnderConstruction: false, includeDeactivated: false)) {
					powered = false;
				} else {
					powered = powerSetting;
				}
			} else {
				powered = false;
			}
			if (num == powered) {
				return;
			}
			if (active && moduleTemplate.objectiveModule) {
				ref_faction.CheckForObjectivesCompleteViaHabModuleActivated(this);
			}
			if (moduleTemplate.incomeProjects > 0) {
				sector.faction.CheckforHabProjectUnlock();
			}
			if (completed && !powerSetting) {
				if (moduleTemplate.allowsResupply && !hab.AllowsResupply(ref_faction, allowHumanTheft: false)) {
					foreach (TISpaceFleetState dockedFleet in hab.dockedFleets) {
						if (dockedFleet.IsResupplying()) {
							dockedFleet.CancelOperation(dockedFleet.CurrentOperations().First((OperationData x) => x.operation is ResupplyOperation));
						}
					}
				}
				if (moduleTemplate.allowsShipConstruction && !hab.AllowsShipConstruction(ref_faction)) {
					foreach (TISpaceFleetState dockedFleet2 in hab.dockedFleets) {
						if (dockedFleet2.IsRepairing()) {
							dockedFleet2.CancelOperation(dockedFleet2.CurrentOperations().First((OperationData x) => x.operation is RepairFleetOperation));
						}
					}
				}
				if (moduleTemplate.spaceCombatValue > 0 && hab.underBombardment && hab.habSite.GetController() != null) {
					hab.habSite.GetController().CeaseBeamFire(this);
				}
			}
			if (!skipFullResourceUpdate) {
				sector.hab.UpdateCurrentAnnualNetResourceIncomes();
			}
		}
		public int ModulePower() {
			for (int i = 0; i < moduleTemplate.SpecialRules.Count; i++) {
				if (moduleTemplate.SpecialRules[i] == HabModuleSpecialRule.Solar_Power_Variable_Output) {
					return SolarPowerOutput(hab, moduleTemplate.power);
				}
				if (moduleTemplate.SpecialRules[i] == HabModuleSpecialRule.Cost_Scales_With_Gravity) {
					return EscapeVelocityBasedPowerRequirement(hab, moduleTemplate, ref_faction);
				}
			}
			return moduleTemplate.power;
		}
		public static int ProspectiveSolarPowerOutput(TIGameState location, float powerValue) {
			float num = 1f;
			double semiMajorAxis_AU;
			if (location.isOrbitState && location.ref_orbit.barycenter.isSun) {
				semiMajorAxis_AU = location.ref_orbit.semiMajorAxis_AU;
			} else {
				semiMajorAxis_AU = location.ref_naturalSpaceObject.GetSunOrbitingRelatedObject.semiMajorAxis_AU;
				if (location.isHabSiteState || location.isSpaceBodyState) {
					switch (location.ref_spaceBody.atmosphere) {
						case Atmosphere.Massive:
							num = 0f;
							break;
						case Atmosphere.Thick:
							num = 0.25f;
							break;
						case Atmosphere.Standard:
							num = 0.5f;
							break;
						case Atmosphere.Thin:
							num = 0.75f;
							break;
					}
				}
			}
			return (int)Math.Round((float)((double)(num * powerValue) / (semiMajorAxis_AU * semiMajorAxis_AU)));
		}
		public static int SolarPowerOutput(TIHabState hab, float powerValue) {
			float num = 1f;
			double semiMajorAxis_AU;
			if (hab.IsStation && hab.barycenter.isSun) {
				semiMajorAxis_AU = hab.orbitState.semiMajorAxis_AU;
			} else {
				semiMajorAxis_AU = hab.GetSunOrbitingRelatedObject.semiMajorAxis_AU;
				if (hab.IsBase) {
					switch (hab.habSite.parentBody.atmosphere) {
						case Atmosphere.Massive:
							num = 0f;
							break;
						case Atmosphere.Thick:
							num = 0.25f;
							break;
						case Atmosphere.Standard:
							num = 0.5f;
							break;
						case Atmosphere.Thin:
							num = 0.75f;
							break;
					}
				}
				if (hab.IsStation && hab.barycenter.isLagrangePointState && hab.barycenter.ref_lagrangePoint.lagrangeValue == LagrangeValue.L2 && hab.barycenter.ref_lagrangePoint.secondaryObject.barycenter.isSun) {
					num *= 0.05f;
				}
			}
			return (int)Math.Round((float)((double)(num * powerValue) / (semiMajorAxis_AU * semiMajorAxis_AU)));
		}
		public static int EscapeVelocityBasedPowerRequirement(TISpaceBodyState body, TIHabModuleTemplate moduleTemplate, TIFactionState faction) {
			return (int)((float)moduleTemplate.power / 2f + Mathf.Round((float)moduleTemplate.power / 2f * (float)body.relativeEnergyForMining(faction)));
		}
		public static int EscapeVelocityBasedPowerRequirement(TIHabSiteState site, TIHabModuleTemplate moduleTemplate, TIFactionState faction) {
			return EscapeVelocityBasedPowerRequirement(site.parentBody, moduleTemplate, faction);
		}
		public static int EscapeVelocityBasedPowerRequirement(TIHabState hab, TIHabModuleTemplate moduleTemplate, TIFactionState faction) {
			if (hab.IsBase) {
				return EscapeVelocityBasedPowerRequirement(hab.habSite, moduleTemplate, faction);
			}
			return moduleTemplate.power;
		}
		public void DestroyModule() {
			if (moduleTemplate.allowsShipConstruction) {
				sector.faction.RemoveShipyardFromFaction(this);
			}
			if (underConstruction) {
				completionDate = TITimeState.SystemNow();
			}
			SetPowerStatus(powerSetting: false);
			destroyed = true;
			constructionCompleted = true;
			if (moduleTemplate.coreModule) {
				hab.anyCoreCompleted = false;
			}
			int num = UnityEngine.Random.Range(1, 3);
			string text = new StringBuilder(moduleTemplate.alienModule ? "AlienDestroyedModule" : "DestroyedModule").Append(moduleTemplate.tier.ToString()).Append(num.ToString()).ToString();
			SetModuleTemplate(text);
			SetPowerStatus(powerSetting: true);
		}
		public bool CanDecomissionModule() {
			if (okay && !hab.IsAlien() && this != hab.coreModule) {
				if (priorModuleTemplate != null) {
					return priorModuleTemplate.dataName != templateName;
				}
				return true;
			}
			return false;
		}
		public TIResourcesCost DecommissionModuleCost() {
			float num = DecommissionDuration_days();
			TIResourcesCost tIResourcesCost = new TIResourcesCost();
			if (num > 0f) {
				tIResourcesCost.AddCost(FactionResource.Boost, (ref_orbit?.isEarthLEO ?? false) ? 0.1f : ((float)TISpaceObjectState.GenericTransferBoostFromEarthSurface(ref_faction, hab.IsBase ? hab.ref_habSite.ref_gameState : hab.ref_orbit.ref_gameState, (float)crew * TemplateManager.global.scuttlePerCrewMassCost)));
				tIResourcesCost.SetCompletionTime_Days(DecommissionDuration_days());
			}
			return tIResourcesCost;
		}
		public TIResourcesCost DecomissionModuleResourceRefund() {
			return moduleTemplate.BuildMaterials(hab.irradiatedMultiplier, ref_spaceBody, hab.faction, TemplateManager.global.decomissionModuleRefundRate).ToResourcesCost();
		}
		public void BeginDecomissionModule() {
			if (moduleTemplate.allowsShipConstruction) {
				sector.faction.RemoveShipyardFromFaction(this);
			}
			SetPowerStatus(powerSetting: false);
			if (DecommissionDuration_days() <= 0f) {
				hab.CompleteDecommissionModule(this);
				buildCost?.RefundCost(sector.faction);
				if (priorModuleTemplate != null) {
					if (!priorModuleCompleted) {
						TIResourcesCost tIResourcesCost = new TIResourcesCost();
						tIResourcesCost.SetCompletionTime_Days((float)priorModuleCompletionDate.DifferenceInDays(TITimeState.Now()));
						hab.InitiateModuleConstruction(sector, slot, priorModuleTemplate, tIResourcesCost);
						return;
					}
					SetCompletedModule(priorModuleTemplate.dataName);
					SetPowerStatus(powerSetting: false);
					hab.UpdateAllModuleConnectors();
				}
			} else {
				DecommissionModuleCost().PayCost(sector.faction);
				TIDateTime tIDateTime = TITimeState.Now();
				tIDateTime.AddDays(DecommissionDuration_days());
				decommissionDate = tIDateTime.ExportTime();
				decommissioning = true;
			}
			constructionCompleted = true;
		}
		public float DecommissionDuration_days() {
			TIHabModuleTemplate tIHabModuleTemplate = null;
			if (moduleTemplate.coreModule || constructionCompleted) {
				tIHabModuleTemplate = moduleTemplate;
			} else if (underConstruction) {
				if ((double)moduleTemplate.buildTime_Days <= new TIDateTime(completionDate).DifferenceInDays(TITimeState.Now())) {
					return 0f;
				}
				tIHabModuleTemplate = ((priorModuleTemplate != null) ? priorModuleTemplate : moduleTemplate);
			}
			float num = (float)tIHabModuleTemplate.tier * 60f;
			if (moduleTemplate.coreModule) {
				num += 5f;
			}
			return num;
		}
		public void CancelDecommissionModule() {
			decommissioning = false;
			if (moduleTemplate.allowsShipConstruction) {
				sector.faction.AddShipyardToFaction(this);
			}
		}
		public void CompleteDecommissionModule() {
			if (!moduleTemplate.coreModule) {
				decommissioning = false;
				TINotificationQueueState.LogDecommissionModuleComplete(this);
				templateName = string.Empty;
				moduleTemplate = null;
				priorModuleTemplateName = string.Empty;
			}
		}
		public void InitializeForBombardment(bool justAddListener = false) {
			if (defenseWeapon != null) {
				if (!justAddListener) {
					TIDateTime tIDateTime = TITimeState.Now();
					tIDateTime.AddSeconds();
					string eventName = baseSTOFireStr;
					TITimeEvent.CreateNewTimeEvent(tIDateTime, this, null, null, eventName);
					lastTimeFired = TITimeState.Now();
				}
				GameControl.eventManager.AddListener<TimeEventStart>(FireMissionToOrbit, baseSTOFireStr, null, queueable: false);
			}
		}
		public static TISpaceShipState SelectSTOTarget(TIGameState shooter, TIDateTime time) {
			List<TISpaceShipState> list = new List<TISpaceShipState>();
			foreach (TISpaceFleetState item in shooter.ref_spaceBody.fleetsInOrbit) {
				if (item.bombardmentTarget != null && item.bombardmentTarget.ref_hab == shooter.ref_hab && TISpaceShipState.BombardmentTargetInLineOfSight(item.ships[0], shooter, time)) {
					list.AddRange(item.ships.Where((TISpaceShipState x) => x.BombardmentValue(shooter.ref_spaceBody) > 0f));
				}
			}
			if (list.Any()) {
				return list.MaxBy((TISpaceShipState x) => x.BombardmentValue(shooter.ref_spaceBody));
			}
			return null;
		}
		public void FireMissionToOrbit(TimeEventStart e) {
			if (!destroyed && powered && hab.underBombardment) {
				TISpaceShipState tISpaceShipState = SelectSTOTarget(this, e.startTime);
				if (tISpaceShipState != null) {
					GameControl.eventManager.TriggerEvent(new STOFireMission_Base(this, tISpaceShipState, e.startTime), null, this);
				}
				TIDateTime tIDateTime = TITimeState.Now();
				tIDateTime.AddSeconds(defenseWeapon.cooldown_s);
				string eventName = baseSTOFireStr;
				TITimeEvent.CreateNewTimeEvent(tIDateTime, this, null, null, eventName);
			} else {
				GameControl.eventManager.RemoveListener<TimeEventStart>(FireMissionToOrbit, baseSTOFireStr);
			}
		}
		private void OnFireMissionOrder(STOFireMission_Base e) {
			if (active) {
				if ((ref_naturalSpaceObject?.controller?.modelLink?.activeInHierarchy).GetValueOrDefault()) {
					hab.habSite.GetController().DisplayBeam(this, e.target);
				}
				StrategyShipController component = e.target.visualizerLink.transform.parent.GetComponent<StrategyShipController>();
				if (weapon == null) {
					ModuleDataEntry weaponData = new ModuleDataEntry(defenseWeapon.ref_laserWeapon, 0);
					weapon = new BeamWeapon(this, weaponData);
					lastTimeFired.AddSeconds(0f - weapon.weaponTemplate.cooldown_s);
				}
				weapon.SetTarget_Strategy(component, component.ShipState.globalPosition);
				BeamWeapon.Beam damageSource = weapon.GetDamageSource(this, e.target.fleet.bombardmentAltitude_km);
				float secondsSinceLastFiring = (float)(TITimeState.Now() - lastTimeFired).TotalSeconds;
				int sTOShotCount = weapon.GetSTOShotCount(hab.habSite.longitude, e.target, secondsSinceLastFiring);
				for (int i = 0; i < sTOShotCount; i++) {
					component.ApplyDamage(damageSource);
				}
				lastTimeFired = TITimeState.Now();
				if (component.ShipState.ShipDestroyed()) {
					TINotificationQueueState.LogShipDestroyedInStrat(component.ShipState, hab.habSite.hab.ref_factions, component.ShipState.fleet.location);
					component.ShipState.DestroyShip(killCouncilors: true, damageSource.attacker?.GetFaction());
				}
			}
		}
		public TIGameState GetTargetableState() {
			return this;
		}
		public void AddTargetedProjectile(TISpaceCombatProjectileState projectile) {
			projectile.EnemyTargetsMe(this);
		}
		public TIFactionState GetFaction() {
			return ref_faction;
		}
		public float FireControlFunction() {
			return 1f;
		}
		public bool WeaponCanFire(ModuleDataEntry moduleData) {
			return isCombatModule;
		}
		public void FireWeapon(ModuleDataEntry module, TISpaceCombatProjectileState targetedProjectile) {
			if (targetedProjectile != null) {
				AddTargetedProjectile(targetedProjectile);
			}
		}
	}
}
namespace PavonisInteractive.TerraInvicta.SpaceCombat.UI {
    public class patch_CombatantListItemController : CombatantListItemController {
		[MonoModIgnore] public IDamageableType combatantType { get; private set; }
		[MonoModIgnore] private void SetRadiatorImageOn(CompleteExtendRadiatorsEvent e) { }
		[MonoModIgnore] private void SetRadiatorImageOff(CompleteRetractRadiatorsEvent e) { }
		[MonoModIgnore] private void RadiatorsDamageChange(ShipPartDamageChange e) { }
		[MonoModIgnore] private void StructureDamageChange(ShipSystemDamageChange e) { }
		public void Init(SpaceCombatCanvasController masterController, CombatantController combatantController, int position) {
			base.name = combatantController.name + " UI";
			this.masterController = masterController;
			this.combatantController = combatantController;
			spaceCombat = GameControl.spaceCombat;
			combatantType = ((IDamageable)combatantController).damageableType;
			this.position = position;
			hitReportObject.SetActive(value: false);
			switch (combatantType) {
				case IDamageableType.Ship:
					shipState = combatantController.ref_shipController.ShipState;
					noseImage.enabled = true;
					lateralImage.enabled = true;
					tailImage.enabled = true;
					radiatorImage.enabled = true;
					driveImage.enabled = true;
					GameControl.eventManager.AddListener<CompleteExtendRadiatorsEvent>(SetRadiatorImageOn, null, shipState);
					GameControl.eventManager.AddListener<CompleteRetractRadiatorsEvent>(SetRadiatorImageOff, null, shipState);
					GameControl.eventManager.AddListener<ShipPartDamageChange>(RadiatorsDamageChange, null, shipState);
					GameControl.eventManager.AddListener<ShipSystemDamageChange>(StructureDamageChange, null, shipState);
					GameControl.eventManager.AddListener<ShipArmorFacingStruck>(OnArmorHit, null, shipState);
					shipName.SetText(new StringBuilder(shipState.hull.displayName).Append(" ").Append(shipState.GetDisplayName(GameControl.control.activePlayer)));
					shipSummaryTip.SetDelegate("BodyText", () => shipState.template.quickSummary(shipState.isAlien && !GameControl.control.activePlayer.finishedProjectNames.Contains("Project_TheirWarships"), shipState));
				break;
				case IDamageableType.StationModule:
					habModuleState = combatantController.ref_habModuleController.habModule;
					noseImage.enabled = false;
					lateralImage.enabled = true;
					tailImage.enabled = false;
					radiatorImage.enabled = false;
					driveImage.enabled = false;
					councilorIconGrid.SetListSize<CombatShipCouncilorGridItemController>(0);
					councilorIconGrid.gameObject.SetActive(value: false);
					shipName.SetText(habModuleState.displayName);
					GameControl.eventManager.AddListener<HabModuleDamagedInCombat>(OnHabModuleHit, null, habModuleState);
					shipSummaryTip.SetDelegate("BodyText", () => shipState.template.quickSummary(shipState.isAlien && !GameControl.control.activePlayer.finishedProjectNames.Contains("Project_TheirWarships"), shipState));
				break;
			}
			UpdateListItem();
		}
	}
	public class patch_SpaceCombatCanvasController : SpaceCombatCanvasController {
		[MonoModIgnore] private bool isFleetCommandCardPanelOpen;
		private void NewFriendlyShipSelected(CombatantController combatantController, bool openPanel = true) {
			isFleetCommandCardPanelOpen = false;
			CombatShipController ref_shipController = combatantController.ref_shipController;
			if (selectedFriendlyShip == ref_shipController || (!openPanel && selectedFriendlyShip == null)) {
				return;
			}
			if (selectedFriendlyShip != null) {
				ClearFriendlyShipUI();
			}
			if (ref_shipController != null) {
				selectedFriendlyShip = ref_shipController;
				selectedFriendlyShipState = ref_shipController.ShipState;
				selectedFriendlyShip.ModelController.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.GreenSquare);
				selectedFriendlyShip.UIController().maintainAnimation = true;
				selectedFriendlyShip.ModelController.StartSelectionAnimation();
				if (selectedFriendlyShip.primaryTarget != null && selectedFriendlyShip.primaryTarget.GetCombatantType() == IDamageableType.Ship) {
					selectedFriendlyShip.primaryTarget.ref_shipController.ModelController.StartSelectionAnimation();
					selectedFriendlyShip.primaryTarget.UIController().maintainAnimation = true;
					(selectedFriendlyShip.primaryTarget.UIController().combatantListItemController as EnemyShipListItemController).OnPrimaryTargetSelected();
				}
				if (selectedFriendlyShip.primaryTarget != null && selectedFriendlyShip.primaryTarget.GetCombatantType() == IDamageableType.StationModule) {
					(selectedFriendlyShip.primaryTarget.UIController().combatantListItemController as EnemyShipListItemController).OnPrimaryTargetSelected();
				}
				SetSelectedShipPanel();
				if (!selectedShipPanel.enabled) {
					selectedShipPanel.enabled = true;
				}
			}
		}
	}
	[MonoModReplace] public class FriendlyShipListItemController : CombatantListItemController {
		public Image noseArmorImage;
		public Image portArmorImage;
		public Image starboardArmorImage;
		public Image tailArmorImage;
		public Image targetingImage;
		public Image heatImage;
		public Image AIControlIcon;
		public Image DVStaticImage;
		public Image warningIcon_None;
		public Image warningIcon_Warn;
		public Image warningIcon_Alert;
		public TooltipTrigger warningNoneTooltip;
		public TooltipTrigger warningWarnTooltip;
		public TooltipTrigger warningAlertTooltip;
		public TooltipTrigger targetingTooltip;
		public TMP_Text deltaVValue;
		public ListManagerBase maneuverList;
		public const float ySize = 80f;
		public Image batteryWarningIcon;
		private IWeapon[] noseWeaponComponents;
		private IWeapon[] hullWeaponComponents;
		private bool alertWeaponNoAmmo;
		private bool alertWeaponNoTarget;
		private bool alertShipNoWeapons;
		private bool alertShipNoThrust;
		private bool alertShipDisengaging;
		private Color32 batteryRed = new Color32(236, 36, 0, byte.MaxValue);
		private Color32 batteryYellow = new Color32(236, 236, 0, byte.MaxValue);
		private static WaitForSeconds delay1 = new WaitForSeconds(1f);
		public override void Init(SpaceCombatCanvasController masterController, CombatantController combatantController, int position) {
			base.Init(masterController, combatantController, position);
			noseArmorImage.color = Color.clear;
			portArmorImage.color = Color.clear;
			starboardArmorImage.color = Color.clear;
			tailArmorImage.color = Color.clear;
			switch (base.combatantType) {
				case IDamageableType.Ship: {
					GameControl.assetLoader.LoadAssetForImageAssignment(shipState.hull.combatUINoseArmorPath_OK(shipState.template.hullAppearanceIndex), noseArmorImage);
					GameControl.assetLoader.LoadAssetForImageAssignment(shipState.hull.combatUIPortArmorPath_OK(shipState.template.hullAppearanceIndex), portArmorImage);
					GameControl.assetLoader.LoadAssetForImageAssignment(shipState.hull.combatUIStarboardArmorPath_OK(shipState.template.hullAppearanceIndex), starboardArmorImage);
					GameControl.assetLoader.LoadAssetForImageAssignment(shipState.hull.combatUITailArmorPath_OK(shipState.template.hullAppearanceIndex), tailArmorImage);
					SpaceCombatCanvasController.SetHeatIcon(shipState, heatImage);
					SetAIControl(shipState.combatAIControl);
					DVStaticImage.enabled = true;
					deltaVValue.enabled = true;
					heatImage.enabled = true;
					SetDeltaVValue();
					SetBatteryStatus();
					noseWeaponComponents = (from x in combatantController.ref_shipController.hull.IterateByClass<IWeapon>()
											where (x as Weapon).weaponTemplate.noseWeapon
											select x).ToArray();
					hullWeaponComponents = (from x in combatantController.ref_shipController.hull.IterateByClass<IWeapon>()
											where (x as Weapon).weaponTemplate.hullWeapon
											select x).ToArray();
					IWeapon[] array = noseWeaponComponents;
					for (int i = 0; i < array.Length; i++) {
						if (array[i].currentFireMode is FocusFireMode && shipState.combatPrimaryTarget == null) {
							alertWeaponNoTarget = true;
						}
					}
					array = hullWeaponComponents;
					for (int i = 0; i < array.Length; i++) {
						if (array[i].currentFireMode is FocusFireMode && shipState.combatPrimaryTarget == null) {
							alertWeaponNoTarget = true;
						}
					}
					UpdateWeaponNoTargetAlert();
					UpdateWeaponOperationalStatus();
					maneuverList.gameObject.SetActive(value: true);
					SpaceCombatCanvasController.UpdateManeuverList(shipState, combatantController.ref_shipController, maneuverList);
					GameControl.eventManager.AddListener<ShipHeatChange>(OnHeatChange, null, shipState, queueable: false);
					GameControl.eventManager.AddListener<ShipAIControlChange>(OnAIControlChanged, null, shipState, queueable: false);
					GameControl.eventManager.AddListener<ShipCommandExecuted>(OnShipCommandExecuted, null, shipState, queueable: false);
					GameControl.eventManager.AddListener<CombatManeuverComplete>(OnCombatManenuverCompleted, null, shipState, queueable: false);
					GameControl.eventManager.AddListener<CombatCollisionAvoidanceStatusChange>(OnCollisionAvoidanceActivated, null, shipState, queueable: false);
					GameControl.eventManager.AddListener<ShipDeltaVChange>(OnShipDeltaVChange, null, shipState, queueable: false);
					GameControl.eventManager.AddListener<ShipBatteryChargeChange>(OnShipBatteryChange, null, shipState, queueable: false);
					GameControl.eventManager.AddListener<ShipWeaponFired>(OnShipWeaponFired, null, shipState, queueable: false);
					GameControl.eventManager.AddListener<ShipSystemDamageChange>(OnShipSystemDamaged, null, shipState, queueable: false);
					GameControl.eventManager.AddListener<ShipWeaponModeChanged>(OnShipWeaponModeChanged, null, shipState, queueable: false);
					GameControl.eventManager.AddListener<ShipDisengageChange>(OnShipDisengageChange, null, shipState, queueable: false);
					GameControl.eventManager.AddListener<ShipPrimaryTargetDestroyed>(OnShipPrimaryTargetDestroyed, null, shipState, queueable: false);
					break;
				}
				case IDamageableType.StationModule:
					AIControlIcon.enabled = false;
					batteryWarningIcon.enabled = false;
					DVStaticImage.enabled = false;
					deltaVValue.enabled = false;
					heatImage.enabled = false;
					warningIcon_Alert.enabled = false;
					warningIcon_None.enabled = false;
					warningIcon_Warn.enabled = false;
					maneuverList.gameObject.SetActive(value: false);
					targetingImage.enabled = false;
					break;
			}
		}
		private void UpdateWeaponNoTargetAlert() {
			alertWeaponNoTarget = false;
			bool flag = shipState.combatPrimaryTarget == null;
			bool flag2 = false;
			bool flag3 = false;
			if (!flag) {
				if (shipState.combatPrimaryTarget.GetTargetableState().ref_ship != null) {
					flag2 = shipState.combatPrimaryTarget.GetTargetableState().ref_ship.ShipDestroyed();
				} else if (shipState.combatPrimaryTarget.GetTargetableState().ref_habModule != null) {
					flag3 = shipState.combatPrimaryTarget.GetTargetableState().ref_habModule.destroyed;
				}
			}
			IWeapon[] array = noseWeaponComponents;
			for (int i = 0; i < array.Length; i++) {
				if (array[i].currentFireMode is FocusFireMode && (flag || flag2 || flag3)) {
					alertWeaponNoTarget = true;
					break;
				}
			}
			if (!alertWeaponNoTarget) {
				array = hullWeaponComponents;
				for (int i = 0; i < array.Length; i++) {
					if (array[i].currentFireMode is FocusFireMode && (flag || flag2 || flag3)) {
						alertWeaponNoTarget = true;
					}
				}
			}
			if (shipState.canSuicide) {
				alertWeaponNoTarget = alertWeaponNoTarget && shipState.canSuicide;
			}
			UpdateAlerts();
		}
		private void UpdateWeaponOperationalStatus() {
			bool flag = false;
			alertWeaponNoAmmo = false;
			foreach (ModuleDataEntry item in shipState.AllWeaponModuleData()) {
				if (shipState.WeaponCanFire(item)) {
					flag = true;
				}
				if (!shipState.WeaponHasAmmo(item)) {
					alertWeaponNoAmmo = true;
				}
			}
			if (!flag) {
				alertShipNoWeapons = true;
			} else {
				alertShipNoWeapons = false;
			}
			UpdateAlerts();
		}
		private void UpdateAlerts() {
			if (alertWeaponNoAmmo) {
				warningIcon_None.enabled = true;
				warningNoneTooltip.SetText("BodyText", Loc.T("UI.SpaceCombat.Warning.EmptyMagazine"));
			} else {
				warningIcon_None.enabled = false;
				warningNoneTooltip.SetText("BodyText", "");
			}
			if (alertWeaponNoTarget) {
				warningIcon_Warn.enabled = true;
				warningWarnTooltip.SetText("BodyText", Loc.T("UI.SpaceCombat.Warning.WeaponNoTarget"));
			} else {
				warningIcon_Warn.enabled = false;
				warningWarnTooltip.SetText("BodyText", "");
			}
			if (alertShipNoThrust || alertShipNoWeapons) {
				if (!warningIcon_Alert.enabled) {
					Mood.TriggerEvent(Mood.Event.SDKL_AlertEdgesRed);
				}
				warningIcon_Alert.enabled = true;
				string text = "";
				if (alertShipNoThrust) {
					text = text + Loc.T("UI.SpaceCombat.Warning.ShipNoThrust") + "\n";
				}
				if (alertShipNoWeapons) {
					text = text + Loc.T("UI.SpaceCombat.Warning.ShipNoWeapons") + "\n";
				}
				warningAlertTooltip.SetText("BodyText", text);
			} else {
				warningIcon_Alert.enabled = false;
				warningAlertTooltip.SetText("BodyText", "");
			}
			bool num = shipState.combatPrimaryTarget == null;
			bool flag = false;
			bool flag2 = false;
			if (!num) {
				if (shipState.combatPrimaryTarget.GetTargetableState().ref_ship != null) {
					flag = shipState.combatPrimaryTarget.GetTargetableState().ref_ship.ShipDestroyed();
				} else if (shipState.combatPrimaryTarget.GetTargetableState().ref_habModule != null) {
					flag2 = shipState.combatPrimaryTarget.GetTargetableState().ref_habModule.destroyed;
				}
			}
			if (num || flag || flag2) {
				targetingImage.enabled = false;
				targetingTooltip.SetText("BodyText", "");
				return;
			}
			TIGameState targetableState = shipState.combatPrimaryTarget.GetTargetableState();
			if (targetableState.isHabModuleState) {
				TIHabModuleState tIHabModuleState = targetableState as TIHabModuleState;
				targetingTooltip.SetText("BodyText", Loc.T("UI.SpaceCombat.Warning.ShipTarget", tIHabModuleState.displayName));
			} else {
				TISpaceShipState tISpaceShipState = targetableState as TISpaceShipState;
				targetingTooltip.SetText("BodyText", Loc.T("UI.SpaceCombat.Warning.ShipTarget", tISpaceShipState.displayName));
			}
			targetingImage.enabled = true;
		}
		public void OnShipSystemDamaged(ShipSystemDamageChange e) {
			if (!shipState.CanSetWaypoints()) {
				alertShipNoThrust = true;
			}
			UpdateWeaponOperationalStatus();
			UpdateAlerts();
		}
		public void OnShipWeaponModeChanged(ShipWeaponModeChanged e) {
			UpdateWeaponNoTargetAlert();
		}
		public void OnShipDisengageChange(ShipDisengageChange e) {
		}
		public void OnShipPrimaryTargetDestroyed(ShipPrimaryTargetDestroyed e) {
			UpdateWeaponNoTargetAlert();
		}
		public void OnShipWeaponFired(ShipWeaponFired e) {
			UpdateWeaponOperationalStatus();
			UpdateAlerts();
		}
		public void OnClickListItem() {
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CombatFriendlyShipSelect");
			GameControl.eventManager.TriggerEvent(new CombatTargetedableStateSelected(base.combatantController.GetCombatantState()), null);
		}
		public override void OnDoubleClick() {
			spaceCombat.combatCamera.LookAtCombatant(base.combatantController);
		}
		private void OnHeatChange(ShipHeatChange e) {
			SpaceCombatCanvasController.SetHeatIcon(shipState, heatImage);
		}
		public void OnAIControlChanged(ShipAIControlChange e) {
			SetAIControl(e.AIInControl);
		}
		public void SetAIControl(bool AIControlled) {
			AIControlIcon.enabled = AIControlled;
		}
		public void OnShipDeltaVChange(ShipDeltaVChange e) {
			SetDeltaVValue();
			if (!shipState.CanSetWaypoints() || shipState.currentDeltaV_kps <= 0f) {
				alertShipNoThrust = true;
			}
			UpdateAlerts();
		}
		public void SetDeltaVValue() {
			float num = shipState.AvailableDeltaVForCombat_kps();
			string text = TIUtilities.FormatBigOrSmallNumber(num, 1, 1);
			deltaVValue.SetText(Loc.T("UI.SpaceCombat.DeltaVCap", (num <= 0f) ? TIUtilities.RedLine(text) : text, TIUtilities.FormatBigOrSmallNumber(base.combatantController.combatMgr.combatState.maxDeltaVAvailableForCombat_kps[shipState])));
			if (shipState.AvailableDeltaVForCombat_kps() / base.combatantController.combatMgr.combatState.maxDeltaVAvailableForCombat_kps[shipState] <= 0.05f) {
				deltaVValue.SetText(TIUtilities.RedLine(deltaVValue.text));
			} else if (shipState.AvailableDeltaVForCombat_kps() / base.combatantController.combatMgr.combatState.maxDeltaVAvailableForCombat_kps[shipState] <= 0.25f) {
				deltaVValue.SetText(TIUtilities.YellowLine(deltaVValue.text));
			}
		}
		public void OnShipBatteryChange(ShipBatteryChargeChange e) {
			SetBatteryStatus();
		}
		public void SetBatteryStatus() {
			batteryWarningIcon.enabled = shipState.batteryChargeFraction <= 0.25f;
			if (shipState.batteryChargeFraction <= 0.05f) {
				batteryWarningIcon.color = batteryRed;
			} else if (shipState.batteryChargeFraction <= 0.25f) {
				batteryWarningIcon.color = batteryYellow;
			}
		}
		private void OnShipCommandExecuted(ShipCommandExecuted e) {
			if (e.command is TIShipManeuverCommandTemplate || e.command is RammingSpeedCommand) {
				SpaceCombatCanvasController.UpdateManeuverList(shipState, base.combatantController.ref_shipController, maneuverList);
			}
			if (e.command is SelectTargetCommand || e.command is ClearTargetCommand || e.command is RammingSpeedCommand) {
				UpdateWeaponNoTargetAlert();
			}
		}
		private void OnCombatManenuverCompleted(CombatManeuverComplete e) {
			SpaceCombatCanvasController.UpdateManeuverList(shipState, base.combatantController.ref_shipController, maneuverList);
		}
		private void OnCollisionAvoidanceActivated(CombatCollisionAvoidanceStatusChange e) {
			SpaceCombatCanvasController.UpdateManeuverList(shipState, base.combatantController.ref_shipController, maneuverList);
		}
		public override void OnArmorHit(ArmorFacing facing, float rawDamage, float absorbedDamage) {
			if (base.gameObject.activeInHierarchy) {
				base.OnArmorHit(facing, rawDamage, absorbedDamage);
				Color color = ((rawDamage > absorbedDamage) ? Color.red : Color.yellow);
				switch (facing) {
					case ArmorFacing.Left: {
						IEnumerator routine = FlashArmorFacing(portArmorImage, color);
						StartCoroutine(routine);
						break;
					}
					case ArmorFacing.Nose: {
						IEnumerator routine = FlashArmorFacing(noseArmorImage, color);
						StartCoroutine(routine);
						break;
					}
					case ArmorFacing.Tail: {
						IEnumerator routine = FlashArmorFacing(tailArmorImage, color);
						StartCoroutine(routine);
						break;
					}
					case ArmorFacing.Right: {
						IEnumerator routine = FlashArmorFacing(starboardArmorImage, color);
						StartCoroutine(routine);
						break;
					}
					case ArmorFacing.Dorsal:
					case ArmorFacing.Ventral:
						break;
				}
			}
		}
		private IEnumerator FlashArmorFacing(Image facing, Color color) {
			facing.color = color;
			yield return delay1;
			facing.color = Color.clear;
		}
		private new void OnDisable() {
			GameControl.eventManager.RemoveListener<ShipHeatChange>(OnHeatChange);
			GameControl.eventManager.RemoveListener<ShipAIControlChange>(OnAIControlChanged);
			GameControl.eventManager.RemoveListener<ShipArmorFacingStruck>(base.OnArmorHit);
			GameControl.eventManager.RemoveListener<ShipCommandExecuted>(OnShipCommandExecuted);
			GameControl.eventManager.RemoveListener<CombatManeuverComplete>(OnCombatManenuverCompleted);
			GameControl.eventManager.RemoveListener<CombatCollisionAvoidanceStatusChange>(OnCollisionAvoidanceActivated);
			GameControl.eventManager.RemoveListener<ShipDeltaVChange>(OnShipDeltaVChange);
			GameControl.eventManager.RemoveListener<ShipBatteryChargeChange>(OnShipBatteryChange);
			GameControl.eventManager.RemoveListener<ShipWeaponFired>(OnShipWeaponFired);
			GameControl.eventManager.RemoveListener<ShipSystemDamageChange>(OnShipSystemDamaged);
			GameControl.eventManager.RemoveListener<ShipWeaponModeChanged>(OnShipWeaponModeChanged);
			GameControl.eventManager.RemoveListener<ShipDisengageChange>(OnShipDisengageChange);
			GameControl.eventManager.RemoveListener<ShipPrimaryTargetDestroyed>(OnShipPrimaryTargetDestroyed);
			base.OnDisable();
		}
		private new void OnDestroy() {
			GameControl.eventManager.RemoveListener<ShipHeatChange>(OnHeatChange);
			GameControl.eventManager.RemoveListener<ShipAIControlChange>(OnAIControlChanged);
			GameControl.eventManager.RemoveListener<ShipArmorFacingStruck>(base.OnArmorHit);
			GameControl.eventManager.RemoveListener<ShipCommandExecuted>(OnShipCommandExecuted);
			GameControl.eventManager.RemoveListener<CombatManeuverComplete>(OnCombatManenuverCompleted);
			GameControl.eventManager.RemoveListener<CombatCollisionAvoidanceStatusChange>(OnCollisionAvoidanceActivated);
			GameControl.eventManager.RemoveListener<ShipDeltaVChange>(OnShipDeltaVChange);
			GameControl.eventManager.RemoveListener<ShipBatteryChargeChange>(OnShipBatteryChange);
			GameControl.eventManager.RemoveListener<ShipWeaponFired>(OnShipWeaponFired);
			GameControl.eventManager.RemoveListener<ShipSystemDamageChange>(OnShipSystemDamaged);
			GameControl.eventManager.RemoveListener<ShipWeaponModeChanged>(OnShipWeaponModeChanged);
			GameControl.eventManager.RemoveListener<ShipDisengageChange>(OnShipDisengageChange);
			GameControl.eventManager.RemoveListener<ShipPrimaryTargetDestroyed>(OnShipPrimaryTargetDestroyed);
			base.OnDestroy();
		}
	}
}
*/