#pragma warning disable CS0108
#pragma warning disable CS0114
#pragma warning disable CS0169
#pragma warning disable CS0414
#pragma warning disable CS0649
#pragma warning disable IDE1006
#pragma warning disable IDE0002

using TMPro;
using MonoMod;
using ModelShark;
using UnityEngine;
using UnityEngine.UI;
using PavonisInteractive.TerraInvicta.Ship;
using PavonisInteractive.TerraInvicta.Audio;
using System.Text;
using System.Linq;
using System.Collections;

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
					shipSummaryTip.enabled = false;
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