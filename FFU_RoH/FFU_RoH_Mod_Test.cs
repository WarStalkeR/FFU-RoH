#pragma warning disable CS0108
#pragma warning disable CS0626
#pragma warning disable IDE0079
#pragma warning disable IDE1006

using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;

public enum patch_InstantEffect {
	MonoModTest1 = 1337,
	MonoModTest2 = 1338,
	MonoModTest3 = 1339
}

namespace PavonisInteractive.TerraInvicta {
	public class patch_TIEffectsState : TIEffectsState {
		public static extern void orig_ProcessInstantEffect(TIFactionState sourceFaction, EffectTargetType effectTargetType, EffectSecondaryStateType secondaryStateType, InstantEffect instantEffect, float value, float randomizer, string strValue, TIGameState inputState, TIGameState secondaryinputState);
		public static void ProcessInstantEffect(TIFactionState sourceFaction, EffectTargetType effectTargetType, EffectSecondaryStateType secondaryStateType, InstantEffect instantEffect, float value, float randomizer, string strValue, TIGameState inputState, TIGameState secondaryinputState) { 
			switch ((patch_InstantEffect)instantEffect) {
				case patch_InstantEffect.MonoModTest1:
					Console.WriteLine("MonoModTest #1");
				break;
				case patch_InstantEffect.MonoModTest2:
					Console.WriteLine("MonoModTest #2");
				break;
				case patch_InstantEffect.MonoModTest3:
					Console.WriteLine("MonoModTest #3");
				break;
				default:
					orig_ProcessInstantEffect(sourceFaction, effectTargetType, secondaryStateType, instantEffect, value, randomizer, strValue, inputState, secondaryinputState);
				break;
			}
		}
	}
}