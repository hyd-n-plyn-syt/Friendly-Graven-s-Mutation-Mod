using System;
using XRL.Language;
using XRL.Rules;

namespace XRL.World.Effects
{
	[Serializable]
	public class ProjectedMight : Effect, ITierInitialized
	{
		public ProjectedMight()
		{
			base.DisplayName = "Boosted " + Grammar.MakeTitleCase(XRL.World.Statistic.GetStatDisplayName(this.Statistic));
		}

		public ProjectedMight(int Duration, string Statistic, int Amount)
		{
			base.Duration = Duration;
			this.Statistic = Statistic;
			this.Bonus = Amount;
			base.DisplayName = "Boosted " + Grammar.MakeTitleCase(XRL.World.Statistic.GetStatDisplayName(Statistic));
		}

		public void Initialize(int Tier)
		{
			base.Duration = Stat.Random(200, 1000);
			this.Bonus = Stat.Random(1, 60) * 5;
		}

		public override int GetEffectType()
		{
			return 2;
		}

		public override bool UseStandardDurationCountdown()
		{
			return true;
		}

		public override bool SameAs(Effect e)
		{
			return false;
		}

		public override string GetDescription()
		{
			return "projected might";
		}

		public override string GetDetails()
		{
			return this.Bonus.Signed(false) + " " + "strength";
		}

		public override bool Apply(GameObject Object)
		{
			Effect effect = Object.GetEffect(delegate(Effect fx)
			{
				ProjectedMight ProjectedMight = fx as ProjectedMight;
				return ProjectedMight != null;
			});
			//ProjectedMight ProjectedMight = Object.GetEffect("ProjectedMight") as ProjectedMight;
			if (effect != null)
			{
				if (base.Duration > effect.Duration)
				{
					effect.Duration = base.Duration;
				}
				return false;
			}
			if (!Object.FireEvent(Event.New("ApplyBoostStatistic", "Event", this)))
			{
				return false;
			}
			if (Object.IsPlayer())
			{
				IComponent<GameObject>.AddPlayerMessage(string.Concat(new string[]
				{
					"Your ",
					XRL.World.Statistic.GetStatDisplayName(this.Statistic),
					" ",
					XRL.World.Statistic.IsStatPlural(this.Statistic) ? "increase" : "increase",
					"!"
				}), null, true);
			}
			base.StatShifter.SetStatShift(this.Statistic, this.Bonus, false);
			return true;
		}

		public override void Remove(GameObject Object)
		{
			if (Object.IsPlayer())
			{
				IComponent<GameObject>.AddPlayerMessage(string.Concat(new string[]
				{
					"Your ",
					XRL.World.Statistic.GetStatDisplayName(this.Statistic),
					" ",
					XRL.World.Statistic.IsStatPlural(this.Statistic) ? "return" : "returns",
					" to normal."
				}), null, true);
			}
			base.StatShifter.RemoveStatShifts();
		}

		public string Statistic = "Strength";

		public int Bonus;
	}
}
