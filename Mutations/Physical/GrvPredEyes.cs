using System;
using XRL.UI;
using XRL.Rules;
using XRL.World.Effects;

namespace XRL.World.Parts.Mutation
{
	[Serializable]
	public class GrvPredEyes : BaseMutation
	{

		public GrvPredEyes()
		{
			this.DisplayName = "Heightened Instincts";
		}

		public override bool CanLevel()
		{
			return true;
		}

		public override string GetDescription()
		{
			return "You are gifted with outstanding instincts that direct your blows towards enemy vitals.";
		}

		public override string GetLevelText(int Level)
		{
			int math1 = 3 + Level;
			return ("+{{cyan|" + math1 + "}} critical hit chance\n" + "you gain access to the precise hit point, armor, and dodge values of biological entities");
		}

		public override bool WantEvent(int ID, int cascade)
		{
			return base.WantEvent(ID, cascade) || ID == GetCriticalThresholdEvent.ID || ID == GetExtraPhysicalFeaturesEvent.ID;
		}
		

		public override bool HandleEvent(GetCriticalThresholdEvent E)
		{
			if (E.Attacker == this.ParentObject)
			{
				if (PercChanceVar.in10())
					E.Threshold--;
				E.Threshold -= StaticThresholdVar;
			}
			return base.HandleEvent(E);
		}
		
		public override bool HandleEvent(GetExtraPhysicalFeaturesEvent E)
		{
			E.Features.Add("predatory gaze");
			return base.HandleEvent(E);
		}
		
		public void PercChance(int Level)
		{
			PercChanceVar = (int)((3 + Level) % 5);
			PercChanceVar *= 2;
		}
		
		public void StaticThreshold(int Level)
		{
			StaticThresholdVar = (int)Math.Floor((Decimal)((Level + 3) / 5));
		}

		public override bool ChangeLevel(int NewLevel)
		{
			PercChance(NewLevel);
			StaticThreshold(NewLevel);
			return base.ChangeLevel(NewLevel);
		}

		public override bool Mutate(GameObject GO, int Level)
		{
			PercChance(Level);
			StaticThreshold(Level);
			this.ParentObject.SetIntProperty("BioScannerEquipped", 1);
			return base.Mutate(GO, Level);
		}

		public override bool Unmutate(GameObject GO)
		{
			this.ParentObject.RemoveProperty("BioScannerEquipped");
			return base.Unmutate(GO);
		}

		public int PercChanceVar = 6;
		public int StaticThresholdVar = 0;
	}
}