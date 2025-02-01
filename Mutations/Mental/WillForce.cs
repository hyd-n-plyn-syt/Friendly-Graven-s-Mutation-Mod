using System;
using XRL.Rules;
using XRL.UI;
using XRL.World.Effects;


namespace XRL.World.Parts.Mutation
{
	[Serializable]
	public class WillForce : BaseMutation
	{
		public WillForce()
		{
			this.DisplayName = "Ego Projection";
			base.Type = "Mental";
		}
		
		public override bool CanLevel()
        {
            return false;
        }

		public override bool WantEvent(int ID, int cascade)
		{
			return base.WantEvent(ID, cascade) || ID == AIGetOffensiveAbilityListEvent.ID || ID == GetItemElementsEvent.ID || ID == BeforeAbilityManagerOpenEvent.ID || ID == CommandEvent.ID;
		}

		public override bool HandleEvent(BeforeAbilityManagerOpenEvent E)
		{
			base.DescribeMyActivatedAbility(this.StrengthActivatedAbilityID, new Action<Templates.StatCollector>(this.CollectStats), null);
			base.DescribeMyActivatedAbility(this.AgilityActivatedAbilityID, new Action<Templates.StatCollector>(this.CollectStats), null);
			base.DescribeMyActivatedAbility(this.ToughnessActivatedAbilityID, new Action<Templates.StatCollector>(this.CollectStats), null);
			return base.HandleEvent(E);
		}

		public override bool HandleEvent(AIGetOffensiveAbilityListEvent E)
		{
			if (E.Distance <= 4)
			{
				if (base.IsMyActivatedAbilityAIUsable(this.StrengthActivatedAbilityID, null))
				{
					E.Add("CommandWillForceStrength", (E.Actor.BaseStat("Strength", 0) > E.Actor.BaseStat("Agility", 0) && E.Actor.BaseStat("Strength", 0) > E.Actor.BaseStat("Toughness", 0)) ? 3 : 1, null, false, false, null, null);
				}
				if (base.IsMyActivatedAbilityAIUsable(this.AgilityActivatedAbilityID, null))
				{
					E.Add("CommandWillForceAgility", (E.Actor.BaseStat("Agility", 0) > E.Actor.BaseStat("Strength", 0) && E.Actor.BaseStat("Agility", 0) > E.Actor.BaseStat("Toughness", 0)) ? 3 : 1, null, false, false, null, null);
				}
				if (base.IsMyActivatedAbilityAIUsable(this.ToughnessActivatedAbilityID, null))
				{
					E.Add("CommandWillForceToughness", 1, null, false, false, null, null);
				}
			}
			return base.HandleEvent(E);
		}

		public override bool HandleEvent(GetItemElementsEvent E)
		{
			if (E.IsRelevantCreature(this.ParentObject))
			{
				E.Add("might", 1);
			}
			return base.HandleEvent(E);
		}

		public override string GetDescription()
		{
			return "Through sheer force of will, you perform uncanny physical feats.";
		}

		public override string GetLevelText(int Level)
		{
			string text = "Augments one physical attribute by an amount equal to your Ego bonus\n";
			return text + "Bonus also applies to party members";
		}

		public override void CollectStats(Templates.StatCollector stats, int Level)
		{
			stats.Set("Bonus", Math.Max(this.ParentObject.StatMod("Ego", 0), 1), false, 0);
		}

		public override bool HandleEvent(CommandEvent E)
		{
			StatShifter.DefaultDisplayName = "Ego Projection";
			ActivatedAbilityEntry StrEntry = base.MyActivatedAbility(this.StrengthActivatedAbilityID, null);
			ActivatedAbilityEntry AgiEntry = base.MyActivatedAbility(this.AgilityActivatedAbilityID, null);
			ActivatedAbilityEntry TouEntry = base.MyActivatedAbility(this.ToughnessActivatedAbilityID, null);
			if (StrEntry != null & AgiEntry != null & TouEntry != null)
			{
				if (E.Command == WillForce.STR_COMMAND_NAME)
				{
					this.RecalculateBonus();
					StrEntry.ToggleState = !StrEntry.ToggleState;
					AgiEntry.ToggleState = false;
					TouEntry.ToggleState = false;
					StatShifter.RemoveStatShifts();
				}
				if (E.Command == WillForce.AGI_COMMAND_NAME)
				{
					this.RecalculateBonus();
					StrEntry.ToggleState = false;
					AgiEntry.ToggleState = !AgiEntry.ToggleState;
					TouEntry.ToggleState = false;
					StatShifter.RemoveStatShifts();
				}
				if (E.Command == WillForce.TOU_COMMAND_NAME)
				{
					this.RecalculateBonus();
					StrEntry.ToggleState = false;
					AgiEntry.ToggleState = false;
					TouEntry.ToggleState = !TouEntry.ToggleState;
					StatShifter.RemoveStatShifts();
				}
				if (StrEntry.ToggleState)
				{
					StatShifter.SetStatShift(target: ParentObject, statName: "Strength", amount: StatBonus, baseValue: false);
				}
				if (AgiEntry.ToggleState)
				{
					StatShifter.SetStatShift(target: ParentObject, statName: "Agility", amount: StatBonus, baseValue: false);
				}
				if (TouEntry.ToggleState)
				{
					StatShifter.SetStatShift(target: ParentObject, statName: "Toughness", amount: StatBonus, baseValue: false);
				}
			}
			return base.HandleEvent(E);
		}

		public override void Register(GameObject Object, IEventRegistrar Registrar)
		{
			Registrar.Register("MinionTakingAction");
			base.Register(Object, Registrar);
		}

		public override bool FireEvent(Event E)
		{
			if (E.ID == "MinionTakingAction")
			{
				this.RecalculateBonus();
				ActivatedAbilityEntry StrEntry = base.MyActivatedAbility(this.StrengthActivatedAbilityID, null);
				ActivatedAbilityEntry AgiEntry = base.MyActivatedAbility(this.AgilityActivatedAbilityID, null);
				ActivatedAbilityEntry TouEntry = base.MyActivatedAbility(this.ToughnessActivatedAbilityID, null);
				GameObject gameObjectParameter = E.GetGameObjectParameter("Object");
				RecalculateBonus();
				if (StrEntry != null & AgiEntry != null & TouEntry != null)
				{
					if (StrEntry.ToggleState)
					{
						gameObjectParameter.RemoveEffect<ProjectedVigor>();
						gameObjectParameter.RemoveEffect<ProjectedSkill>();
						gameObjectParameter.ApplyEffect(new ProjectedMight(5, "Strength", StatBonus), null);
					}
					if (AgiEntry.ToggleState)
					{
						gameObjectParameter.RemoveEffect<ProjectedVigor>();
						gameObjectParameter.RemoveEffect<ProjectedMight>();
						gameObjectParameter.ApplyEffect(new ProjectedSkill(5, "Agility", StatBonus), null);
					}
					if (TouEntry.ToggleState)
					{
						gameObjectParameter.RemoveEffect<ProjectedMight>();
						gameObjectParameter.RemoveEffect<ProjectedSkill>();
						gameObjectParameter.ApplyEffect(new ProjectedVigor(5, "Toughness", StatBonus), null);
					}
				}
			}
			return base.FireEvent(E);
		}
		
		

		public override bool ChangeLevel(int NewLevel)
		{
			this.RecalculateBonus();
			return base.ChangeLevel(NewLevel);
		}
		
		public void RecalculateBonus()
		{
			StatBonus = Math.Max(this.ParentObject.StatMod("Ego", 0), 1);
		}

		public override bool Mutate(GameObject GO, int Level)
		{
			this.RecalculateBonus();
			this.StrengthActivatedAbilityID = base.AddMyActivatedAbility("Boost Strength", STR_COMMAND_NAME, "Mental Mutation", null, "¾", null, true, false, true, false, false, false, false, true, true, false, false, -1, null, null, null, null, null, null);
			this.AgilityActivatedAbilityID = base.AddMyActivatedAbility("Boost Agility", AGI_COMMAND_NAME, "Mental Mutation", null, "¯", null, true, false, true, false, false, false, false, true, true, false, false, -1, null, null, null, null, null, null);
			this.ToughnessActivatedAbilityID = base.AddMyActivatedAbility("Boost Toughness", TOU_COMMAND_NAME, "Mental Mutation", null, "\u0003", null, true, false, true, false, false, false, false, true, true, false, false, -1, null, null, null, null, null, null);
			return base.Mutate(GO, Level);
		}

		public override bool Unmutate(GameObject GO)
		{
			base.RemoveMyActivatedAbility(ref this.StrengthActivatedAbilityID, null);
			base.RemoveMyActivatedAbility(ref this.AgilityActivatedAbilityID, null);
			base.RemoveMyActivatedAbility(ref this.ToughnessActivatedAbilityID, null);
			return base.Unmutate(GO);
		}

		public Guid StrengthActivatedAbilityID = Guid.Empty;
		public Guid AgilityActivatedAbilityID = Guid.Empty;
		public Guid ToughnessActivatedAbilityID = Guid.Empty;
		
		public static string STR_COMMAND_NAME = "CommandToggleWillForceStrength";
		public static string AGI_COMMAND_NAME = "CommandToggleWillForceAgility";
		public static string TOU_COMMAND_NAME = "CommandToggleWillForceToughness";
		
		public int StatBonus = 1;
	}
}
