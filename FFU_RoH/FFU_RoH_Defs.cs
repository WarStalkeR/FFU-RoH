#pragma warning disable CS0108
#pragma warning disable CS0114
#pragma warning disable CS0162
#pragma warning disable CS0414
#pragma warning disable CS0618
#pragma warning disable CS0626
#pragma warning disable CS0649
#pragma warning disable IDE1006
#pragma warning disable IDE0019
#pragma warning disable IDE0002

using BepInEx;
using BepInEx.Configuration;
using FFU_Rise_of_Humanity;
using System.IO;
using UnityEngine;

namespace FFU_Rise_of_Humanity {
    public class FFU_RoH_Defs {
        public static ConfigFile conf_FFU_RoH;
        public static void Init() {
            conf_FFU_RoH = new ConfigFile(Path.Combine(Paths.ConfigPath, "mod_FFU_RoH.cfg"), true);
            FFU_RoH_Confs.ConfSkirmishHuman();
            FFU_RoH_Confs.ConfSkirmishAlien();
        }
        public static void Log() {
            FFU_RoH_Logs.LogSkirmishHuman();
            FFU_RoH_Logs.LogSkirmishAlien();
        }
    }
    public class FFU_RoH_Confs {
        public static ConfigEntry<string> skirmishHumanPD;
        public static ConfigEntry<string> skirmishHumanT1Beam;
        public static ConfigEntry<string> skirmishHumanT2Beam;
        public static ConfigEntry<string> skirmishHumanT3Beam;
        public static ConfigEntry<string> skirmishHumanT1Gun;
        public static ConfigEntry<string> skirmishHumanT2Gun;
        public static ConfigEntry<string> skirmishHumanT3Gun;
        public static void ConfSkirmishHuman() {
            skirmishHumanPD = FFU_RoH_Defs.conf_FFU_RoH.Bind("Config.Skirmish", "skirmishHumanPDName", "PointDefensePhaserTurret");
            skirmishHumanT1Beam = FFU_RoH_Defs.conf_FFU_RoH.Bind("Config.Skirmish", "skirmishHumanT1BeamName", "60cmUVPhaserBattery");
            skirmishHumanT2Beam = FFU_RoH_Defs.conf_FFU_RoH.Bind("Config.Skirmish", "skirmishHumanT2BeamName", "120cmUVPhaserBattery");
            skirmishHumanT3Beam = FFU_RoH_Defs.conf_FFU_RoH.Bind("Config.Skirmish", "skirmishHumanT3BeamName", "360cmUVPhaserBattery");
            skirmishHumanT1Gun = FFU_RoH_Defs.conf_FFU_RoH.Bind("Config.Skirmish", "skirmishHumanT1GunName", "LightRailgunBatteryMk3");
            skirmishHumanT2Gun = FFU_RoH_Defs.conf_FFU_RoH.Bind("Config.Skirmish", "skirmishHumanT2GunName", "RailgunBatteryMk3");
            skirmishHumanT3Gun = FFU_RoH_Defs.conf_FFU_RoH.Bind("Config.Skirmish", "skirmishHumanT3GunName", "HeavyRailgunBatteryMk3");
        }
        public static ConfigEntry<string> skirmishAlienPD;
        public static ConfigEntry<string> skirmishAlienT1Beam;
        public static ConfigEntry<string> skirmishAlienT2Beam;
        public static ConfigEntry<string> skirmishAlienT3Beam;
        public static ConfigEntry<string> skirmishAlienT1Gun;
        public static ConfigEntry<string> skirmishAlienT2Gun;
        public static ConfigEntry<string> skirmishAlienT3Gun;
        public static void ConfSkirmishAlien() {
            skirmishAlienPD = FFU_RoH_Defs.conf_FFU_RoH.Bind("Config.Skirmish", "skirmishAlienPDName", "AlienPointDefenseLaserTurret");
            skirmishAlienT1Beam = FFU_RoH_Defs.conf_FFU_RoH.Bind("Config.Skirmish", "skirmishAlienT1BeamName", "Alien64cmOrangeLaserBattery");
            skirmishAlienT2Beam = FFU_RoH_Defs.conf_FFU_RoH.Bind("Config.Skirmish", "skirmishAlienT2BeamName", "Alien128cmOrangeLaserBattery");
            skirmishAlienT3Beam = FFU_RoH_Defs.conf_FFU_RoH.Bind("Config.Skirmish", "skirmishAlienT3BeamName", "Alien384cmOrangeLaserBattery");
            skirmishAlienT1Gun = FFU_RoH_Defs.conf_FFU_RoH.Bind("Config.Skirmish", "skirmishAlienT1GunName", "AdvancedAlienLightMagBattery");
            skirmishAlienT2Gun = FFU_RoH_Defs.conf_FFU_RoH.Bind("Config.Skirmish", "skirmishAlienT2GunName", "AdvancedAlienMagBattery");
            skirmishAlienT3Gun = FFU_RoH_Defs.conf_FFU_RoH.Bind("Config.Skirmish", "skirmishAlienT3GunName", "AdvancedAlienHeavyMagBattery");
        }
    }
    public class FFU_RoH_Logs {
        public static void LogSkirmishHuman() {
            string confText = "Skirmish Human Weapon Configuration:";
            confText += $"\n > Human Point Defense: {FFU_RoH_Confs.skirmishHumanPD.Value}";
            confText += $"\n > Human T1 Beam Weapon: {FFU_RoH_Confs.skirmishHumanT1Beam.Value}";
            confText += $"\n > Human T2 Beam Weapon: {FFU_RoH_Confs.skirmishHumanT2Beam.Value}";
            confText += $"\n > Human T3 Beam Weapon: {FFU_RoH_Confs.skirmishHumanT3Beam.Value}";
            confText += $"\n > Human T1 Gun Weapon: {FFU_RoH_Confs.skirmishHumanT1Gun.Value}";
            confText += $"\n > Human T2 Gun Weapon: {FFU_RoH_Confs.skirmishHumanT2Gun.Value}";
            confText += $"\n > Human T3 Gun Weapon: {FFU_RoH_Confs.skirmishHumanT3Gun.Value}";
            Debug.LogWarning($"{confText}");
        }
        public static void LogSkirmishAlien() {
            string confText = "Skirmish Alien Weapon Configuration:";
            confText += $"\n > Alien Point Defense: {FFU_RoH_Confs.skirmishAlienPD.Value}";
            confText += $"\n > Alien T1 Beam Weapon: {FFU_RoH_Confs.skirmishAlienT1Beam.Value}";
            confText += $"\n > Alien T2 Beam Weapon: {FFU_RoH_Confs.skirmishAlienT2Beam.Value}";
            confText += $"\n > Alien T3 Beam Weapon: {FFU_RoH_Confs.skirmishAlienT3Beam.Value}";
            confText += $"\n > Alien T1 Gun Weapon: {FFU_RoH_Confs.skirmishAlienT1Gun.Value}";
            confText += $"\n > Alien T2 Gun Weapon: {FFU_RoH_Confs.skirmishAlienT2Gun.Value}";
            confText += $"\n > Alien T3 Gun Weapon: {FFU_RoH_Confs.skirmishAlienT3Gun.Value}";
            Debug.LogWarning($"{confText}");
        }
    }
}

namespace PavonisInteractive.TerraInvicta.Systems.Bootstrap {
    public class patch_GlobalInstaller : GlobalInstaller {
        public extern void orig_InstallBindings();
        public void InstallBindings() {
            FFU_RoH_Defs.Init();
            FFU_RoH_Defs.Log();
            orig_InstallBindings();
        }
    }
}