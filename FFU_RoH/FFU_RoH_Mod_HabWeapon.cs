#pragma warning disable CS0108
#pragma warning disable CS0414
#pragma warning disable CS0649
#pragma warning disable IDE1006

using FFU_Rise_of_Humanity;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta {
    public class patch_TIFactionState : TIFactionState {
		public string GetBestPointDefenseWeaponTemplateName() {
			string pdWeaponName = "30mmAutocannon";
			if (GameControl.control.skirmishMode) {
				pdWeaponName = IsAlienFaction ? FFU_RoH_Confs.skirmishAlienPD.Value : FFU_RoH_Confs.skirmishHumanPD.Value;
			} else {
				List<TIShipWeaponTemplate> validWeapons = allowedHullWeapons.ToList();
				if (validWeapons.Count > 0) {
					List<TIShipWeaponTemplate> validPDs = validWeapons.Where((TIShipWeaponTemplate x) => x.defenseMode && !x.attackMode && (x.isBeamWeapon)).ToList();
					pdWeaponName = (validPDs.Count <= 0) ? validWeapons.MinBy((TIShipWeaponTemplate x) => x.cooldown_s).dataName : validPDs.MinBy((TIShipWeaponTemplate x) => x.cooldown_s).dataName;
				}
			}
			return pdWeaponName;
		}
		public string GetBestHabWeapon(bool isBase, int tier, bool preferLaser) {
			if (isBase) return TILaserWeaponTemplate.GetBestHeavyDefenseLaser(this, tier).dataName;
			string habWeaponName = string.Empty;
			if (GameControl.control.skirmishMode) {
				if (preferLaser) {
					switch (tier) {
						case 1: habWeaponName = IsAlienFaction ? FFU_RoH_Confs.skirmishAlienT1Beam.Value : FFU_RoH_Confs.skirmishHumanT1Beam.Value; break;
						case 2: habWeaponName = IsAlienFaction ? FFU_RoH_Confs.skirmishAlienT2Beam.Value : FFU_RoH_Confs.skirmishHumanT2Beam.Value; break;
						case 3: habWeaponName = IsAlienFaction ? FFU_RoH_Confs.skirmishAlienT3Beam.Value : FFU_RoH_Confs.skirmishHumanT3Beam.Value; break;
					}
				} else {
					switch (tier) {
						case 1: habWeaponName = IsAlienFaction ? FFU_RoH_Confs.skirmishAlienT1Gun.Value : FFU_RoH_Confs.skirmishHumanT1Gun.Value; break;
						case 2: habWeaponName = IsAlienFaction ? FFU_RoH_Confs.skirmishAlienT2Gun.Value : FFU_RoH_Confs.skirmishHumanT2Gun.Value; break;
						case 3: habWeaponName = IsAlienFaction ? FFU_RoH_Confs.skirmishAlienT3Gun.Value : FFU_RoH_Confs.skirmishHumanT3Gun.Value; break;
					}
				}
			} else {
				habWeaponName = IsAlienFaction ? "AlienLightMagBattery" : "8-inchCannon";
				List<Mount> allowedMounts = new List<Mount>();
				allowedMounts.Add(Mount.OneHull);
				switch (tier) {
					case 2:
						allowedMounts.Add(Mount.TwoHullHoriz);
						allowedMounts.Add(Mount.TwoHullVert);
					break;
					case 3:
						allowedMounts.Add(Mount.TwoHullHoriz);
						allowedMounts.Add(Mount.TwoHullVert);
						allowedMounts.Add(Mount.ThreeHullHoriz);
						allowedMounts.Add(Mount.FourHull);
					break;
				}
				List<TIShipWeaponTemplate> validWeapons = allowedHullWeapons.Where((TIShipWeaponTemplate x) => x.weaponClass != WeaponClass.Missile && x.attackMode && allowedMounts.Contains(x.mount)).ToList();
				if (preferLaser) {
					List<TIShipWeaponTemplate> validBeamWeapons = validWeapons.Where((TIShipWeaponTemplate x) => x.isBeamWeapon).ToList();
					if (validBeamWeapons.Any()) validWeapons = validBeamWeapons;
				} else {
					List<TIShipWeaponTemplate> validGunWeapons = validWeapons.Where((TIShipWeaponTemplate x) => x.isGunWeapon).ToList();
					if (validGunWeapons.Any()) validWeapons = validGunWeapons;
				}
				if (validWeapons.Count > 0) {
					validWeapons = (from x in validWeapons
							orderby x.ScoreForRole(ShipRole.SL_DefenseBomber) descending, x.Score() descending, x.internalSize descending
							select x).ToList();
					habWeaponName = validWeapons[0].dataName;
				}
			}
			return habWeaponName;
		}
	}
}