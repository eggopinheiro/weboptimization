using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for RailRoadFitness
/// </summary>
public class RailRoadFitness : IFitness<Gene>
{
	public RailRoadFitness()
	{
	}

    public double GetFitness(IIndividual<Gene> pIndividual)
    {
        Dictionary<double, FitnessElement> lvDicTrainTime = new Dictionary<double, FitnessElement>();
        Gene lvGene = null;
        FitnessElement lvFitnessElement = null;
        double lvRes = 0.0;
        double lvOpt = double.MaxValue;

        for (int i = 0; i < pIndividual.Count; i++)
        {
            lvGene = pIndividual.GetGene(i);

            if (!lvDicTrainTime.ContainsKey(lvGene.TrainId))
            {
                lvFitnessElement = new FitnessElement();

                lvFitnessElement.TrainId = lvGene.TrainId;
                lvFitnessElement.InitialTime = lvGene.Time;
                lvFitnessElement.ValueWeight = lvGene.ValueWeight;

                lvOpt = TrainIndividual.GetOptimum(lvGene);

                lvFitnessElement.Optimun = lvOpt;

                lvDicTrainTime.Add(lvGene.TrainId, lvFitnessElement);

                lvFitnessElement = null;
            }
            else
            {
                lvFitnessElement = lvDicTrainTime[lvGene.TrainId];
                lvFitnessElement.EndTime = lvGene.Time;
            }
        }

        foreach (FitnessElement lvFitnessElem in lvDicTrainTime.Values)
        {
            if (lvFitnessElem.Optimun > 0)
            {
                lvRes += lvFitnessElem.ValueWeight * ((lvFitnessElem.EndTime - lvFitnessElem.InitialTime).TotalHours - lvFitnessElem.Optimun) / lvFitnessElem.Optimun;
            }
        }

        return lvRes;
    }

    private class FitnessElement : IEquatable<FitnessElement>
    {
        private double mTrainId = 0.0;
        private DateTime mInitialTime = DateTime.MinValue;
        private DateTime mEndTime = DateTime.MinValue;
        private double mOptimun = 0.0;
        private double mValueWeight = 1.0;

        public double ValueWeight
        {
            get { return mValueWeight; }
            set { mValueWeight = value; }
        }

        public double TrainId
        {
            get { return mTrainId; }
            set { mTrainId = value; }
        }

        public DateTime InitialTime
        {
            get { return mInitialTime; }
            set { mInitialTime = value; }
        }

        public DateTime EndTime
        {
            get { return mEndTime; }
            set { mEndTime = value; }
        }

        public double Optimun
        {
            get { return mOptimun; }
            set { mOptimun = value; }
        }

        public static bool operator ==(FitnessElement obj1, FitnessElement obj2)
        {
            bool lvRes = false;

            if (ReferenceEquals(obj1, null))
            {
                return false;
            }

            if (ReferenceEquals(obj2, null))
            {
                return false;
            }

            if ((obj1.TrainId == obj2.TrainId))
            {
                lvRes = true;
            }

            return lvRes;
        }

        public static bool operator !=(FitnessElement obj1, FitnessElement obj2)
        {
            return !(obj1 == obj2);
        }

        public bool Equals(FitnessElement other)
        {
            bool lvRes = false;

            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if ((this.TrainId == other.TrainId) && (this.InitialTime == other.InitialTime) && (this.EndTime == other.EndTime))
            {
                lvRes = true;
            }

            return lvRes;
        }

        public override bool Equals(object obj)
        {
            bool lvRes = false;

            if (obj is FitnessElement)
            {
                lvRes = Equals(obj as FitnessElement);
            }

            return lvRes;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int lvHashCode = 0;

                if (TrainId > 0.0)
                {
                    lvHashCode = TrainId.GetHashCode();
                }

                return lvHashCode;
            }
        }
    }
}