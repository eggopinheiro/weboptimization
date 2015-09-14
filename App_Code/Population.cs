using System;
using System.Collections.Generic;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;

/// <summary>
/// Summary description for Population
/// </summary>
public class Population
{
    private List<IIndividual<Gene>> mIndividuals = null;
    private bool USE_CROSSOVER_2_PONTOS = true;
    private Dictionary<StopLocation, HashSet<Gene>> mStopLocationOcupation = null;
    private Dictionary<StopLocation, Gene[]> mStopLocationDeparture = null;
    private Dictionary<string, int> mPriority = null;
    private IFitness<Gene> mFitness = null;
    private IIndividual<Gene> mFather = null;
    private IIndividual<Gene> mMother = null;
    private IIndividual<Gene> mSon = null;
    private IIndividual<Gene> mDaughter = null;
    private DateTime mInitialDate;
    private DateTime mFinalDate;
    private DateTime mDateRef;
    private List<Gene> mTrainList = null;
    private List<Gene> mPlanList = null;
    private int mInitialValidCoordinate = Int32.MinValue;
    private int mEndValidCoordinate = Int32.MaxValue;
    private bool mAllowNoDestinationTrain = true;
    private double mInvalidIndividualFitness = 10000.0;
    private int mMutationRate = 5;
    private int mSize = 0;

    private static double ELITE_PERC = 0.2;
    private static HashSet<string> mTrainAllowed = new HashSet<string>();

    public Population(IFitness<Gene> pFitness, int pSize, int pMutationRate, bool pUse2PointCrossOver, DateTime pInitialDate, DateTime pFinalDate, string pStrPriority)
	{
        IIndividual<Gene> lvIndividual = null;
        bool lvValidIndividual = false;

        mFitness = pFitness;
        mMutationRate = pMutationRate;
        USE_CROSSOVER_2_PONTOS = pUse2PointCrossOver;
        mInitialDate = pInitialDate;
        mFinalDate = pFinalDate;
        mPriority = new Dictionary<string, int>();

        if (DateTime.Now.Date == mInitialDate.Date)
        {
            mDateRef = DateTime.Now;
        }
        else
        {
            mDateRef = mFinalDate;
        }

        if (!bool.TryParse(ConfigurationManager.AppSettings["ALLOW_NO_DESTINATION_TRAIN"], out mAllowNoDestinationTrain))
        {
            mAllowNoDestinationTrain = true;
        }

        if (!double.TryParse(ConfigurationManager.AppSettings["INVALID_FITNESS_VALUE"], out mInvalidIndividualFitness))
        {
            mInvalidIndividualFitness = 10000.0;
        }

        if (!Int32.TryParse(ConfigurationManager.AppSettings["INITIAL_VALID_COORDINATE"], out mInitialValidCoordinate))
        {
            mInitialValidCoordinate = Int32.MinValue;
        }

        if (!Int32.TryParse(ConfigurationManager.AppSettings["END_VALID_COORDINATE"], out mInitialValidCoordinate))
        {
            mEndValidCoordinate = Int32.MaxValue;
        }

        mStopLocationOcupation = new Dictionary<StopLocation, HashSet<Gene>>();
        mStopLocationDeparture = new Dictionary<StopLocation, Gene[]>();

        foreach (StopLocation lvStopLocation in StopLocation.GetList())
        {
            mStopLocationOcupation.Add(lvStopLocation, new HashSet<Gene>());
            mStopLocationDeparture.Add(lvStopLocation, new Gene[lvStopLocation.Capacity]);
        }

        LoadTrainList(pStrPriority);

        DebugLog.Logar(" ");
        DebugLog.Logar("Gerando Individuos:");

        mSize = pSize;

        mIndividuals = new List<IIndividual<Gene>>();
        for (int i = 0; i < pSize; i++)
        {
            lvIndividual = new TrainIndividual(mFitness, mDateRef, mStopLocationOcupation, mStopLocationDeparture);
            DebugLog.Logar("Individuo " + lvIndividual.GetUniqueId() + ":");
            lvValidIndividual = lvIndividual.GenerateIndividual(mTrainList, mPlanList);
            DebugLog.Logar("Individuo Info = " + lvIndividual.ToString());
            DebugLog.Logar(" ");

            if (lvValidIndividual)
            {
                mIndividuals.Add(lvIndividual);
            }
        }
        DebugLog.Logar(" ");
    }

    private void LoadTrainList(string pStrPriority)
    {
        HashSet<double> lvTrainSet = new HashSet<double>();
        DataTable lvDataTrains = null;
        DataTable lvDataPlans = null;
        Gene lvGene = null;
        Segment lvSegment = null;
        StopLocation lvCurrentStopSegment = null;
        StopLocation lvNextStopLocation = null;
        StopLocation lvEndStopLocation = null;
        double lvMeanSpeed = 0.0;
        int lvIndex;
        string lvKey;
        string lvStrTrainName = "";

        int lvCoordinate;
        int lvDirection;
        int lvLocation;
        string lvStrUD;
        DateTime lvOcupTime;
        DateTime lvCreationtime;
        bool lvLogEnable = DebugLog.mEnable;

        if (pStrPriority.Length > 0)
        {
            LoadPriority(pStrPriority);
        }

        mTrainList = new List<Gene>();

        DebugLog.mEnable = true;
        DebugLog.Logar(" ");
        DebugLog.Logar("Listando trens a serem considerados:");

        lvDataTrains = TrainmovsegmentDataAccess.GetCurrentTrainsData(mInitialDate, mFinalDate).Tables[0];

        foreach (DataRow row in lvDataTrains.Rows)
        {
            lvStrTrainName = ((row["name"] == DBNull.Value) ? "" : row["name"].ToString());

            if (mTrainAllowed.Contains(lvStrTrainName.Substring(0, 1)) || (mTrainAllowed.Count == 0))
            {
                lvGene = new Gene(mDateRef);

                lvGene.TrainName = lvStrTrainName;
                lvGene.TrainId = ((row["train_id"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["train_id"]);
                lvGene.Time = ((row["data_ocup"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["data_ocup"].ToString()));
                lvGene.Location = ((row["location"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["location"]));
                lvGene.UD = ((row["ud"] == DBNull.Value) ? "" : row["ud"].ToString());
                lvGene.Direction = ((row["direction"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["direction"]));
                lvGene.Track = ((row["track"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["track"]));
                lvGene.Coordinate = ((row["coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["coordinate"]);
                lvGene.Start = ((row["origem"] == DBNull.Value) ? Int32.MinValue : (int)row["origem"]);
                lvGene.End = ((row["destino"] == DBNull.Value) ? Int32.MinValue : (int)row["destino"]);
                lvGene.DepartureTime = ((row["departure_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["departure_time"].ToString()));
                lvCreationtime = ((row["creation_tm"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["creation_tm"].ToString()));

                if (lvGene.DepartureTime.AddYears(1) < lvCreationtime)
                {
                    lvGene.DepartureTime = lvCreationtime;
                }

                if (lvGene.DepartureTime == DateTime.MinValue)
                {
                    lvGene.DepartureTime = ((row["plan_departure_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["plan_departure_time"].ToString()));
                }

                if (lvGene.Start == -99999999)
                {
                    lvGene.Start = Int32.MinValue;
                }

                if (lvGene.End == -99999999)
                {
                    lvGene.End = Int32.MinValue;
                }

                if (!mAllowNoDestinationTrain)
                {
                    if (lvGene.End == Int32.MinValue)
                    {
                        continue;
                    }
                }

                lvCurrentStopSegment = StopLocation.GetCurrentStopSegment(lvGene.Coordinate, lvGene.Direction, out lvIndex);
                lvGene.StopLocation = lvCurrentStopSegment;

                lvNextStopLocation = StopLocation.GetNextStopSegment(lvGene.Coordinate, lvGene.Direction);
                lvEndStopLocation = StopLocation.GetCurrentStopSegment(lvGene.End, lvGene.Direction, out lvIndex);

                if (lvEndStopLocation == null)
                {
                    lvEndStopLocation = lvNextStopLocation;
                }

                if (lvNextStopLocation == null)
                {
                    continue;
                }
                else if ((lvCurrentStopSegment == lvEndStopLocation) && (lvCurrentStopSegment != null))
                {
                    continue;
                }
                else if (lvEndStopLocation != null)
                {
                    if (lvGene.Direction > 0)
                    {
                        if (lvGene.Coordinate >= lvEndStopLocation.Start_coordinate)
                        {
                            continue;
                        }
                        
                        if (lvGene.Coordinate < mInitialValidCoordinate)
                        {
                            continue;
                        }

                        if (lvGene.End > mEndValidCoordinate)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (lvGene.Coordinate <= lvEndStopLocation.End_coordinate)
                        {
                            continue;
                        }
                        
                        if (lvGene.Coordinate > mEndValidCoordinate)
                        {
                            continue;
                        }

                        if (lvGene.End < mInitialValidCoordinate)
                        {
                            continue;
                        }
                    }
                }

                if (mPriority.Keys.Count > 0)
                {
                    lvKey = lvGene.TrainName.Substring(0, 1) + lvGene.Direction;
                    if (mPriority.ContainsKey(lvKey))
                    {
                        lvGene.ValueWeight = mPriority[lvKey];
                        lvGene.ReloadValue(mDateRef);
                    }
                }

                lvMeanSpeed = TrainmovsegmentDataAccess.GetMeanSpeed(lvGene.TrainId, mFinalDate, out lvCoordinate, out lvDirection, out lvLocation, out lvStrUD, out lvOcupTime);
                lvGene.Speed = lvMeanSpeed;

                if (lvGene.StopLocation != null)
                {
                    if (mStopLocationOcupation.ContainsKey(lvCurrentStopSegment))
                    {
                        //DebugLog.Logar("GetTrainList => Adicionando Gene (" + lvGene + ") na Stop Location (" + lvCurrentStopSegment + ")");
                        mStopLocationOcupation[lvCurrentStopSegment].Add(lvGene);
                    }

                    if ((lvGene.UD.Equals("CV03B") && (lvGene.Direction == -1)) || (lvGene.UD.Equals("CV03C") && (lvGene.Direction == 1)) || lvGene.UD.StartsWith("SW") || lvGene.UD.Equals("WT"))
                    {
                        if (lvGene.Track <= lvCurrentStopSegment.Capacity)
                        {
                            mStopLocationDeparture[lvCurrentStopSegment][lvGene.Track - 1] = lvGene;
                        }
                        lvGene.StopLocation = null;

                        if (lvGene.Track != 0)
                        {
                            mTrainList.Insert(0, lvGene);
                            lvTrainSet.Add(lvGene.TrainId);
                            DebugLog.Logar("Trem " + lvGene.TrainId + " - " + lvGene.TrainName + " (Partida: " + lvGene.DepartureTime + "; Valor: " + lvGene.Value + "; End: " + lvGene.End + ". Location: " + lvGene.Location + "." + lvGene.UD + ")");
                        }
                    }
                    else
                    {
                        if (lvGene.Track != 0)
                        {
                            mTrainList.Add(lvGene);
                            lvTrainSet.Add(lvGene.TrainId);
                            DebugLog.Logar("Trem " + lvGene.TrainId + " - " + lvGene.TrainName + " (Partida: " + lvGene.DepartureTime + "; Valor: " + lvGene.Value + "; End: " + lvGene.End + ". Location: " + lvGene.Location + "." + lvGene.UD + ")");
                        }
                    }
                }
                else
                {
                    if (lvGene.Track != 0)
                    {
                        mTrainList.Insert(0, lvGene);
                        lvTrainSet.Add(lvGene.TrainId);
                        DebugLog.Logar("Trem " + lvGene.TrainId + " - " + lvGene.TrainName + " (Partida: " + lvGene.DepartureTime + "; Valor: " + lvGene.Value + "; End: " + lvGene.End + ". Location: " + lvGene.Location + "." + lvGene.UD + ")");
                    }
                }
            }
        }

        mPlanList = new List<Gene>();

        DebugLog.Logar(" ------------------------------------------------------------------------------------------------------ ");
        DebugLog.Logar(" ");

        DebugLog.Logar("Listando Planos a serem considerados:");

        if (DateTime.Now.Date == mInitialDate.Date)
        {
            lvDataPlans = PlanDataAccess.GetCurrentPlans(DateTime.Now, mFinalDate.AddDays(1)).Tables[0];
        }
        else
        {
            lvDataPlans = PlanDataAccess.GetCurrentPlans(mFinalDate, mFinalDate.AddDays(1)).Tables[0];
        }

        foreach (DataRow row in lvDataPlans.Rows)
        {
            lvGene = new Gene(mDateRef);

            lvGene.TrainId = ((row["plan_id"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["plan_id"]);

            if (!lvTrainSet.Contains(lvGene.TrainId))
            {
                lvGene.TrainName = ((row["train_name"] == DBNull.Value) ? "" : row["train_name"].ToString());

                if (mTrainAllowed.Contains(lvGene.TrainName.Substring(0, 1)) || (mTrainAllowed.Count == 0))
                {
                    if (lvGene.TrainName.Trim().Length == 0) continue;

                    lvGene.Start = ((row["origem"] == DBNull.Value) ? Int32.MinValue : (int)row["origem"]);
                    lvGene.End = ((row["destino"] == DBNull.Value) ? Int32.MinValue : (int)row["destino"]);
                    lvGene.DepartureTime = ((row["departure_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["departure_time"].ToString()));
                    lvGene.Time = DateTime.MinValue;
                    lvGene.Coordinate = lvGene.Start;
                    lvGene.Direction = Int16.Parse(lvGene.TrainName.Substring(1));
                    lvGene.Value = 1;

                    if ((lvGene.Direction % 2) == 0)
                    {
                        lvGene.Direction = -1;
                    }
                    else
                    {
                        lvGene.Direction = 1;
                    }

                    if (lvGene.Direction > 0)
                    {
                        if (lvGene.Start < mInitialValidCoordinate)
                        {
                            lvGene.Start = mInitialValidCoordinate;
                            lvGene.Coordinate = lvGene.Start;
                        }

                        if (lvGene.End > mEndValidCoordinate)
                        {
                            lvGene.End = mEndValidCoordinate;
                        }
                    }
                    else
                    {
                        if (lvGene.Start > mEndValidCoordinate)
                        {
                            lvGene.Start = mEndValidCoordinate;
                            lvGene.Coordinate = lvGene.Start;
                        }

                        if (lvGene.End < mInitialValidCoordinate)
                        {
                            lvGene.End = mInitialValidCoordinate;
                        }
                    }

                    lvSegment = Segment.GetCurrentSegment(lvGene.Coordinate, lvGene.Direction, 1, out lvIndex);

                    if (lvSegment != null)
                    {
                        lvGene.Location = (short)lvSegment.Location;
                        lvGene.UD = lvSegment.SegmentValue;
                        lvGene.Track = 1;

                        lvCurrentStopSegment = StopLocation.GetCurrentStopSegment(lvGene.Coordinate, lvGene.Direction, out lvIndex);
                        lvGene.StopLocation = lvCurrentStopSegment;
                    }
                    else
                    {
                        DebugLog.Logar("Não tem segment !");
                    }

                    if (mPriority.Keys.Count > 0)
                    {
                        lvKey = lvGene.TrainName.Substring(0, 1) + lvGene.Direction;
                        if (mPriority.ContainsKey(lvKey))
                        {
                            lvGene.ValueWeight = mPriority[lvKey];
                        }
                    }

                    if (mTrainAllowed.Count == 0)
                    {
                        mPlanList.Add(lvGene);
                        DebugLog.Logar("Plano " + lvGene.TrainId + " - " + lvGene.TrainName + " (Partida: " + lvGene.DepartureTime + ", Track: " + lvGene.Track + ")");
                    }
                    else if (mTrainAllowed.Contains(lvGene.TrainName.Substring(0, 1)))
                    {
                        mPlanList.Add(lvGene);
                        DebugLog.Logar("Plano " + lvGene.TrainId + " - " + lvGene.TrainName + " (Partida: " + lvGene.DepartureTime + ", Track: " + lvGene.Track + ")");
                    }
                }
            }
        }
        DebugLog.Logar(" --------------------------------------------------------------------------------------------- ");
        DebugLog.Logar(" ");

        DebugLog.mEnable = lvLogEnable;
    }

    private void LoadPriority(string pStrPriority)
    {
        string[] lvVarElement;
        string lvStrTrainType = "";
        int lvDirection = 0;
        int lvValue = 0;
        string lvKey;
        string[] lvVarPriority = pStrPriority.Split('|');

        foreach (string lvPriority in lvVarPriority)
        {
            lvVarElement = lvPriority.Split(':');

            if (lvVarPriority.Length >= 3)
            {
                lvStrTrainType = lvVarPriority[0];
                lvDirection = Int32.Parse(lvVarPriority[1]);
                lvValue = Int32.Parse(lvVarPriority[2]);

                lvKey = lvStrTrainType + lvVarPriority[1];

                if (!mPriority.ContainsKey(lvKey))
                {
                    mPriority.Add(lvKey, lvValue);
                }
            }
        }
    }

    public int Count
    {
        get
        {
            int lvRes = 0;

            if (mIndividuals != null)
            {
                lvRes = mIndividuals.Count;
            }

            return lvRes;
        }
    }

    public IIndividual<Gene> GetIndividualAt(int pIndex)
    {
        IIndividual<Gene> lvRes = null;

        if (mIndividuals != null)
        {
            if (pIndex >= 0 && pIndex < mIndividuals.Count)
            {
                lvRes = mIndividuals[pIndex];
            }
        }

        return lvRes;
    }

    public void NextGeneration()
    {
        bool lvValid = false;
        int lvIndElite = 0;
        int lvOrigQuant = mIndividuals.Count;
        int lvQuantToRemove = 0;
        int lvIndex = -1;
        IIndividual<Gene> lvMutated = null;
        List<IIndividual<Gene>> lvNewIndividuals = new List<IIndividual<Gene>>();
        Random lvRandom = new Random(DateTime.Now.Millisecond);

        if (mIndividuals.Count == 0)
        {
            return;
        }

        for (int i = 0; i < lvOrigQuant / 2; i++)
        {
            lvValid = RouletteWheelSelection();

            if (lvValid)
            {
                DoCrossOver();
            }

            if (mSon != null)
            {
                lvMutated = Mutate(mSon);

                if (lvMutated != null)
                {
                    lvMutated.GetFitness();
                    lvNewIndividuals.Add(lvMutated);
                }
                else
                {
                    mSon.GetFitness();
                    lvNewIndividuals.Add(mSon);
                }

                mSon = null;
                lvMutated = null;
            }

            if (mDaughter != null)
            {
                lvMutated = Mutate(mDaughter);

                if (lvMutated != null)
                {
                    lvMutated.GetFitness();
                    lvNewIndividuals.Add(lvMutated);
                }
                else
                {
                    mDaughter.GetFitness();
                    lvNewIndividuals.Add(mDaughter);
                }

                mDaughter = null;
                lvMutated = null;
            }
        }

        mIndividuals.AddRange(lvNewIndividuals);
        if (mIndividuals.Count >= mSize)
        {
            lvQuantToRemove = mIndividuals.Count - lvOrigQuant;
        }
        else
        {
            lvQuantToRemove = 0;
        }

        mIndividuals.Sort();

        lvIndElite = (int)(mIndividuals.Count * ELITE_PERC);
        lvIndex = lvRandom.Next(lvIndElite + 1, mIndividuals.Count) - lvQuantToRemove;

        dump();

        if (lvIndex <= lvIndElite)
        {
            lvIndex = lvIndElite + 1;
        }

        for (int i = 0; i < lvQuantToRemove; i++)
        {
            mIndividuals.RemoveAt(lvIndex);
        }
    }

    private bool RouletteWheelSelection()
    {
    	double lvTotalFitness = 0.0;
    	double lvRandomValue1;
    	double lvRandomValue2;
    	double lvTotal = 0.0;
    	Random lvRandom;
    	bool lvRes = true;
    	
    	mFather = null;
    	mMother = null;
    	
    	foreach(IIndividual<Gene> lvIndividual in mIndividuals)
    	{
            lvTotalFitness += (1 / (1 + lvIndividual.Fitness));
    	}
    	
        lvRandom = new Random(DateTime.Now.Millisecond);

    	lvRandomValue1 = lvTotalFitness * lvRandom.NextDouble();
    	lvRandomValue2 = lvTotalFitness * lvRandom.NextDouble();

        for (int i = 0; i < mIndividuals.Count; i++)
    	{
            lvTotal += (1 / (1 + mIndividuals[i].Fitness));
    		
    		if(lvRandomValue1 <= lvTotal)
    		{
    			if(mFather == null)
    			{
                    mFather = mIndividuals[i];
                    if (mMother == null)
                    {
                        if (lvRandomValue2 <= lvTotal)
                        {
                            if (i < mIndividuals.Count - 1)
                            {
                                mMother = mIndividuals[i + 1];
                            }
                            else
                            {
                                mMother = mIndividuals[0];
                            }
                        }
                    }
                }
    		}
    		
    		if(lvRandomValue2 <= lvTotal)
    		{
    			if(mMother == null)
    			{
                    mMother = mIndividuals[i];
    			}
    		}

            if ((mFather != null) && (mMother != null))
            {
                if (mFather.GetUniqueId() == mMother.GetUniqueId())
                {
                    lvRes = false;
                }
                break;
            }
    	}

        if(mFather == null || mMother == null)
        {
            lvRes = false;
        }

        return lvRes;
    }

    private void DumpHashSet(HashSet<Gene> pHashSet)
    {
        DebugLog.Logar(" ");
        DebugLog.Logar(" --------------------------------------  DumpHashSet ----------------------------------- ");
        foreach (Gene lvGene in pHashSet)
        {
            DebugLog.Logar(" ");
            DebugLog.Logar("lvGene.hashCode = " + lvGene.GetHashCode());
            DebugLog.Logar("lvGene = " + lvGene);
            if (lvGene.StopLocation != null)
            {
                DebugLog.Logar("lvGene.StopLocation = " + lvGene.StopLocation);
            }
            else
            {
                DebugLog.Logar("lvGene.StopLocation = null");
            }
        }
        DebugLog.Logar(" ------------------------------------------------------------------------------------------- ");
        DebugLog.Logar(" ");
    }

    private void DumpDuplicateStopLocation(TrainIndividual pIndividual)
    {
        Dictionary<double, StopLocation> lvGenesId = new Dictionary<double, StopLocation>();
        StopLocation lvStopLocationOrign = null;

        mStopLocationOcupation = pIndividual.GetStopLocationOcupation();

        DebugLog.Logar(" ");
        DebugLog.Logar(" --------------------------------------  DumpDuplicateStopLocation ----------------------------------- ");
        foreach (StopLocation lvStopLocation in mStopLocationOcupation.Keys)
        {
            foreach (Gene lvGene in mStopLocationOcupation[lvStopLocation])
            {
                if (lvGenesId.ContainsKey(lvGene.TrainId))
                {
                    DebugLog.Logar(" -----------------------");
                    DebugLog.Logar("StopLocation = " + lvStopLocation);

                    lvStopLocationOrign = lvGenesId[lvGene.TrainId];
                    DebugLog.Logar(" ");
                    DebugLog.Logar("lvStopLocationOrign = " + lvStopLocationOrign);

                    DebugLog.Logar(" ");
                    DebugLog.Logar("lvGene.TrainId = " + lvGene.TrainId);
                    DebugLog.Logar("lvGene.TrainName = " + lvGene.TrainName);
                    if (lvGene.StopLocation != null)
                    {
                        DebugLog.Logar("lvGene.StopLocation.Location = " + lvGene.StopLocation.Location);
                    }
                    DebugLog.Logar("lvGene.Track = " + lvGene.Track);
                    DebugLog.Logar("lvGene.Location = " + lvGene.Location);
                    DebugLog.Logar("lvGene.UD = " + lvGene.UD);
                    DebugLog.Logar("lvGene.Coordinate = " + lvGene.Coordinate);
                    DebugLog.Logar("lvGene.End = " + lvGene.End);
                    DebugLog.Logar("lvGene.Time = " + lvGene.Time);

                    DebugLog.Logar(" ");
                    DebugLog.Logar(" -----------------------");
                    DebugLog.Logar(" ");
                }
                else
                {
                    lvGenesId.Add(lvGene.TrainId, lvStopLocation);
                }
            }
        }
        DebugLog.Logar(" ------------------------------------------------------------------------------------------- ");
        DebugLog.Logar(" ");
    }

    private void DumpStopLocation(TrainIndividual pIndividual)
    {
        mStopLocationOcupation = pIndividual.GetStopLocationOcupation();

        DebugLog.Logar(" ");
        DebugLog.Logar(" -------------------------------------- DumpStopLocations ----------------------------------- ");
        foreach (StopLocation lvStopLocation in mStopLocationOcupation.Keys)
        {
            foreach (Gene lvGene in mStopLocationOcupation[lvStopLocation])
            {
                DebugLog.Logar(" -----------------------");
                DebugLog.Logar("StopLocation = " + lvStopLocation);
                DebugLog.Logar(" ");
                DebugLog.Logar("lvGene.TrainId = " + lvGene.TrainId);
                DebugLog.Logar("lvGene.TrainName = " + lvGene.TrainName);
                if (lvGene.StopLocation != null)
                {
                    DebugLog.Logar("lvGene.StopLocation.Location = " + lvGene.StopLocation.Location);
                }
                DebugLog.Logar("lvGene.Track = " + lvGene.Track);
                DebugLog.Logar("lvGene.Location = " + lvGene.Location);
                DebugLog.Logar("lvGene.UD = " + lvGene.UD);
                DebugLog.Logar("lvGene.Coordinate = " + lvGene.Coordinate);
                DebugLog.Logar("lvGene.End = " + lvGene.End);
                DebugLog.Logar("lvGene.Time = " + lvGene.Time);

                DebugLog.Logar(" ");
                DebugLog.Logar(" -----------------------");
                DebugLog.Logar(" ");
            }
        }
        DebugLog.Logar(" ------------------------------------------------------------------------------------------- ");
        DebugLog.Logar(" ");
    }

    private void DumpStopLocation(Gene pGene)
    {
        if (pGene != null)
        {
            DebugLog.Logar(" ");
            DebugLog.Logar(" -------------------------------------- DumpStopLocations ----------------------------------- ");
            foreach (StopLocation lvStopLocation in mStopLocationOcupation.Keys)
            {
                foreach (Gene lvGene in mStopLocationOcupation[lvStopLocation])
                {
                    if (pGene.TrainId == lvGene.TrainId)
                    {
                        DebugLog.Logar(" -----------------------");
                        DebugLog.Logar("StopLocation = " + lvStopLocation);
                        DebugLog.Logar(" ");
                        DebugLog.Logar("lvGene.TrainId = " + lvGene.TrainId);
                        DebugLog.Logar("lvGene.TrainName = " + lvGene.TrainName);
                        if (lvGene.StopLocation != null)
                        {
                            DebugLog.Logar("lvGene.StopLocation.Location = " + lvGene.StopLocation.Location);
                        }
                        DebugLog.Logar("lvGene.Track = " + lvGene.Track);
                        DebugLog.Logar("lvGene.Location = " + lvGene.Location);
                        DebugLog.Logar("lvGene.UD = " + lvGene.UD);
                        DebugLog.Logar("lvGene.Coordinate = " + lvGene.Coordinate);
                        DebugLog.Logar("lvGene.End = " + lvGene.End);
                        DebugLog.Logar("lvGene.Time = " + lvGene.Time);

                        DebugLog.Logar(" ");
                        DebugLog.Logar(" -----------------------");
                        DebugLog.Logar(" ");
                    }
                }
            }
            DebugLog.Logar(" ------------------------------------------------------------------------------------------- ");
            DebugLog.Logar(" ");
        }
    }

    private void DumpStopLocation(StopLocation pStopLocation)
    {
        if (pStopLocation != null)
        {
            DebugLog.Logar(" ");
            DebugLog.Logar(" -------------------------------------- DumpStopLocations ----------------------------------- ");
            foreach (StopLocation lvStopLocation in mStopLocationOcupation.Keys)
            {
                DebugLog.Logar(" -----------------------");
                DebugLog.Logar("StopLocation = " + lvStopLocation);
                DebugLog.Logar(" ");

                foreach (Gene lvGene in mStopLocationOcupation[lvStopLocation])
                {
                    DebugLog.Logar("lvGene.TrainId = " + lvGene.TrainId);
                    DebugLog.Logar("lvGene.TrainName = " + lvGene.TrainName);
                    if (lvGene.StopLocation != null)
                    {
                        DebugLog.Logar("lvGene.StopLocation.Location = " + lvGene.StopLocation.Location);
                    }
                    DebugLog.Logar("lvGene.Track = " + lvGene.Track);
                    DebugLog.Logar("lvGene.Location = " + lvGene.Location);
                    DebugLog.Logar("lvGene.UD = " + lvGene.UD);
                    DebugLog.Logar("lvGene.Coordinate = " + lvGene.Coordinate);
                    DebugLog.Logar("lvGene.End = " + lvGene.End);
                    DebugLog.Logar("lvGene.Time = " + lvGene.Time);

                    DebugLog.Logar(" ");
                }
                DebugLog.Logar(" -----------------------");
                DebugLog.Logar(" ");
            }
            DebugLog.Logar(" ------------------------------------------------------------------------------------------- ");
            DebugLog.Logar(" ");
        }
    }

    private void AddFromQueue(IIndividual<Gene> pIndividual, Queue<Gene> pQueue, HashSet<Gene> pHashRef)
    {
        List<Gene> lvGenes = null;
        Gene lvGene = null;

        for (int i = 0; i < pQueue.Count; i++)
        {
            lvGene = pQueue.Dequeue();
            lvGenes = pIndividual.GetNextPosition(lvGene, DateTime.MaxValue);
            if (lvGenes != null)
            {
                pIndividual.AddGenes(lvGenes);

                if (lvGenes.Count == 2)
                {
                    pHashRef.Add(lvGenes[0]);
                }
            }
            else
            {
                pQueue.Enqueue(lvGene);
            }
        }
    }

    private bool VerifyPendentGene(TrainIndividual pIndividual, TrainIndividual pParentIndividual, int pIndinit, int pIndEnd)
    {
        bool lvRes = false;
        int lvPedentCount = 0;
        int lvPedentVerify = 0;

        HashSet<Gene> lvPendent = pIndividual.GetDeadLockResponsible();
        lvPedentCount = lvPendent.Count;

        foreach (Gene lvGene in lvPendent)
        {
            for (int i = pIndinit; i < pIndEnd; i++)
            {
                if (lvGene.Equals(pParentIndividual.GetGene(i)))
                {
                    lvPedentVerify++;
                    DebugLog.Logar("VerifyPendentGene para retirar (" + lvGene + ") no indice = " + i);
                    break;
                }
            }
        }

        if ((lvPedentVerify >= lvPedentCount) && (lvPedentCount > 0))
        {
            lvRes = true;
        }
        else
        {
            lvRes = false;
        }

        return lvRes;
    }

    private void DoCrossOver()
    {
        int lvIndex1 = -1;
        int lvIndex2 = -1;
        int lvCount = 0;
        int lvIndividualSize = mFather.Count - 1;
        int lvHalfIndex = (int)lvIndividualSize / 2;
        Gene lvGeneFather = null;
        Gene lvGeneMother = null;
        Gene lvGene = null;
        Queue<Gene> lvQueueSon = new Queue<Gene>();
        Queue<Gene> lvQueueDaughter = new Queue<Gene>();
        List<Gene> lvInitialSon = null;
        List<Gene> lvInitialDaughter = null;
        List<Gene> lvGenes = null;
        HashSet<Gene> lvHashSetSon = new HashSet<Gene>();
        HashSet<Gene> lvHashSetDaughter = new HashSet<Gene>();

        Random lvRandom = new Random(DateTime.Now.Millisecond);

        if (USE_CROSSOVER_2_PONTOS)
        {
            lvIndex1 = lvRandom.Next(lvHalfIndex + 1);
            lvIndex2 = lvRandom.Next(lvHalfIndex + 1, lvIndividualSize + 1);
        }
        else
        {
            lvIndex1 = lvRandom.Next(lvIndividualSize+1);
            lvIndex2 = lvIndividualSize + 1;
        }

        mSon = new TrainIndividual(mFitness, mDateRef, null, null);
        lvInitialSon = mFather.GetGenes(0, lvIndex1);
        mSon.AddGenes(lvInitialSon);
        foreach (Gene lvGen in lvInitialSon)
        {
            if (lvGen.HeadWayTime == DateTime.MinValue)
            {
                lvHashSetSon.Add(lvGen);
            }
        }
        //DumpStopLocation((TrainIndividual)mSon);

        mDaughter = new TrainIndividual(mFitness, mDateRef, null, null);
        lvInitialDaughter = mMother.GetGenes(0, lvIndex1);
        mDaughter.AddGenes(lvInitialDaughter);
        foreach (Gene lvGen in lvInitialDaughter)
        {
            if (lvGen.HeadWayTime == DateTime.MinValue)
            {
                lvHashSetDaughter.Add(lvGen);
            }
        }

        /*
        DebugLog.Logar(" ");
        DebugLog.Logar("lvHashSetSon: ");
        DumpHashSet(lvHashSetSon);

        DebugLog.Logar(" ");
        DebugLog.Logar("lvHashSetDaughter: ");
        DumpHashSet(lvHashSetDaughter);

        DebugLog.Logar("Father: ");
        ((TrainIndividual)mFather).DumpGene(null);

        DebugLog.Logar("Mother: ");
        ((TrainIndividual)mMother).DumpGene(null);

        //DebugLog.Logar(" ");
        */

        lvCount = mMother.Count;
        if (mFather.Count > lvCount)
        {
            lvCount = mFather.Count;
        }

        for (int i = 0; i < lvCount; i++)
        {
            lvGeneFather = mFather.GetGene(i);
            lvGeneMother = mMother.GetGene(i);

            if ((mSon != null) && (lvGeneMother != null))
            {
                if ((mSon.Count - 1) < lvIndex2)
                {
                    if (!lvHashSetSon.Contains(lvGeneMother) && lvGeneMother.HeadWayTime == DateTime.MinValue)
                    {
                        /*
                        DebugLog.Logar("Tentando adicionar em crossover ao individuo mSon o Gene: " + lvGeneMother);
                        if (lvGeneMother.StopLocation != null)
                        {
                            DebugLog.Logar("lvGeneMother.StopLocation = " + lvGeneMother.StopLocation);
                        }
                        else
                        {
                            DebugLog.Logar("lvGeneMother.StopLocation is null !");
                        }
                        DebugLog.Logar("lvGeneMother.GetHashCode() = " + lvGeneMother.GetHashCode());
                        DebugLog.Logar("lvHashSetSon.Contains(lvGeneMother) = " + lvHashSetSon.Contains(lvGeneMother));

                        DebugLog.Logar("Listando movimentos anteriores a esse Gene em: " + mSon);
                        lvGene = ((TrainIndividual)mSon).DumpGene(lvGeneMother);
                        if (lvGene != null)
                        {
                            if (lvGene.StopLocation != null)
                            {
                                DebugLog.Logar("Ultimo Stop Location: " + lvGene.StopLocation);
                            }
                        }
                        */

                        AddFromQueue(mSon, lvQueueSon, lvHashSetSon);
                        lvGenes = mSon.GetNextPosition(lvGeneMother, DateTime.MaxValue);
                        if (lvGenes != null)
                        {
                            mSon.AddGenes(lvGenes);

                            if (lvGenes.Count == 2)
                            {
                                lvHashSetSon.Add(lvGenes[0]);
                                //mStopLocationOcupation = ((TrainIndividual)mSon).GetStopLocationOcupation();
                                //DumpStopLocation(lvGenes[lvGenes.Count - 1]);
                            }
                        }
                        else
                        {
                            lvQueueSon.Enqueue(lvGeneMother);
                            /*
                            DebugLog.Logar("Erro ao tentar unir Genes em crossover:");
                            ((TrainIndividual)mSon).DumpGene(null);
                            DebugLog.Logar("Erro ao tentar adicionar Gene (" + lvGeneMother + ")");
                            DebugLog.Logar("Genes de Mother:");
                            ((TrainIndividual)mMother).DumpGene(null);
                            DebugLog.Logar("Existe movimentacao pendente para sair de Dead Lock = " + VerifyPendentGene((TrainIndividual)mSon, (TrainIndividual)mMother, i, lvIndex2));
                            //mSon = null;
                             */ 
                        }
                    }
                }
            }

            if ((mDaughter != null) && (lvGeneFather != null))
            {
                if ((mDaughter.Count - 1) < lvIndex2)
                {
                    if (!lvHashSetDaughter.Contains(lvGeneFather) && lvGeneFather.HeadWayTime == DateTime.MinValue)
                    {
                        /*
                        DebugLog.Logar("Tentando adicionar em crossover ao individuo mDaughter o Gene: " + lvGeneFather);
                        if (lvGeneFather.StopLocation != null)
                        {
                            DebugLog.Logar("lvGeneFather.StopLocation = " + lvGeneFather.StopLocation);
                        }
                        else
                        {
                            DebugLog.Logar("lvGeneFather.StopLocation is null !");
                        }
                        DebugLog.Logar("lvGeneFather.GetHashCode() = " + lvGeneFather.GetHashCode());
                        DebugLog.Logar("lvHashSetDaughter.Contains(lvGeneFather) = " + lvHashSetDaughter.Contains(lvGeneFather));

                        DebugLog.Logar("Listando movimentos anteriores a esse Gene em: " + mDaughter);

                        lvGene = ((TrainIndividual)mDaughter).DumpGene(lvGeneFather);
                        if (lvGene != null)
                        {
                            if(lvGene.StopLocation != null)
                            {
                                DebugLog.Logar("Ultimo Stop Location: " + lvGene.StopLocation);
                            }
                        }
                        */

                        AddFromQueue(mDaughter, lvQueueDaughter, lvHashSetDaughter);
                        lvGenes = mDaughter.GetNextPosition(lvGeneFather, DateTime.MaxValue);
                        if (lvGenes != null)
                        {
                            mDaughter.AddGenes(lvGenes);

                            if (lvGenes.Count == 2)
                            {
                                lvHashSetDaughter.Add(lvGenes[0]);
                                //mStopLocationOcupation = ((TrainIndividual)mDaughter).GetStopLocationOcupation();
                                //DumpStopLocation(lvGenes[lvGenes.Count - 1]);
                            }
                        }
                        else
                        {
                            lvQueueDaughter.Enqueue(lvGeneFather);
                            /*
                            DebugLog.Logar("Erro ao tentar unir Genes em crossover:");
                            ((TrainIndividual)mDaughter).DumpGene(null);
                            DebugLog.Logar("Erro ao tentar adicionar Gene (" + lvGeneFather + ")");
                            DebugLog.Logar("Genes de Father:");
                            ((TrainIndividual)mFather).DumpGene(null);
                            DebugLog.Logar("Existe movimentacao pendente para sair de Dead Lock = " + VerifyPendentGene((TrainIndividual)mSon, (TrainIndividual)mMother, i, lvIndex2));
                            */
                            //mDaughter = null;
                        }
                    }
                }
            }

            if (((mSon.Count - 1) >= lvIndex2) && ((mDaughter.Count - 1) >= lvIndex2))
            {
                AddFromQueue(mSon, lvQueueSon, lvHashSetSon);
                AddFromQueue(mDaughter, lvQueueDaughter, lvHashSetDaughter);
                break;
            }
        }

        for(int i = lvIndex1 + 1; i < lvCount; i++)
        {
            lvGeneFather = mFather.GetGene(i);
            lvGeneMother = mMother.GetGene(i);

            if ((mSon != null) && (lvGeneFather != null))
            {
                if (!lvHashSetSon.Contains(lvGeneFather) && lvGeneFather.HeadWayTime == DateTime.MinValue)
                {
                    /*
                    DebugLog.Logar("Tentando adicionar em crossover ao individuo mSon o Gene: " + lvGeneFather);
                    if (lvGeneFather.StopLocation != null)
                    {
                        DebugLog.Logar("lvGeneFather.StopLocation = " + lvGeneFather.StopLocation);
                    }
                    else
                    {
                        DebugLog.Logar("lvGeneFather.StopLocation is null !");
                    }
                    DebugLog.Logar("lvGeneFather.GetHashCode() = " + lvGeneFather.GetHashCode());
                    DebugLog.Logar("lvHashSetSon.Contains(lvGeneFather) = " + lvHashSetSon.Contains(lvGeneFather));

                    ((TrainIndividual)mSon).DumpGene(null);
                    DebugLog.Logar("Listando movimentos anteriores a esse Gene em: " + mSon);
                    lvGene = ((TrainIndividual)mSon).DumpGene(lvGeneFather);
                    if (lvGene != null)
                    {
                        if (lvGene.StopLocation != null)
                        {
                            DebugLog.Logar("Ultimo Stop Location: " + lvGene.StopLocation);
                        }
                    }
                     */ 
                    AddFromQueue(mSon, lvQueueSon, lvHashSetSon);
                    lvGenes = mSon.GetNextPosition(lvGeneFather, DateTime.MaxValue);
                    if (lvGenes != null)
                    {
                        mSon.AddGenes(lvGenes);

                        if (lvGenes.Count == 2)
                        {
                            lvHashSetSon.Add(lvGenes[0]);
                            //mStopLocationOcupation = ((TrainIndividual)mSon).GetStopLocationOcupation();
                            //DumpStopLocation(lvGenes[lvGenes.Count - 1]);
                        }
                    }
                    else
                    {
                        lvQueueSon.Enqueue(lvGeneFather);
                        /*
                        DebugLog.Logar("Erro ao tentar unir Genes em crossover:");
                        ((TrainIndividual)mSon).DumpGene(null);
                        DebugLog.Logar("Erro ao tentar adicionar Gene (" + lvGeneFather + ")");
                        DebugLog.Logar("Genes de Father:");
                        ((TrainIndividual)mFather).DumpGene(null);
                        DebugLog.Logar("Existe movimentacao pendente para sair de Dead Lock = " + VerifyPendentGene((TrainIndividual)mSon, (TrainIndividual)mFather, i, lvCount));
                        */
                        //mSon = null;
                    }
                }
            }

            if ((mDaughter != null) && (lvGeneMother != null))
            {
                if (!lvHashSetDaughter.Contains(lvGeneMother) && lvGeneMother.HeadWayTime == DateTime.MinValue)
                {
                    /*
                    DebugLog.Logar("Tentando adicionar em crossover ao individuo mDaughter o Gene: " + lvGeneMother);
                    if (lvGeneMother.StopLocation != null)
                    {
                        DebugLog.Logar("lvGeneMother.StopLocation = " + lvGeneMother.StopLocation);
                    }
                    else
                    {
                        DebugLog.Logar("lvGeneMother.StopLocation is null !");
                    }
                    DebugLog.Logar("lvGeneMother.GetHashCode() = " + lvGeneMother.GetHashCode());
                    DebugLog.Logar("lvHashSetSon.Contains(lvGeneMother) = " + lvHashSetSon.Contains(lvGeneMother));

                    ((TrainIndividual)mDaughter).DumpGene(null);
                    DebugLog.Logar("Listando movimentos anteriores a esse Gene em: " + mDaughter);
                    lvGene = ((TrainIndividual)mDaughter).DumpGene(lvGeneMother);
                    if (lvGene != null)
                    {
                        if (lvGene.StopLocation != null)
                        {
                            DebugLog.Logar("Ultimo Stop Location: " + lvGene.StopLocation);
                        }
                    }
                     */ 
                    AddFromQueue(mDaughter, lvQueueDaughter, lvHashSetDaughter);
                    lvGenes = mDaughter.GetNextPosition(lvGeneMother, DateTime.MaxValue);
                    if (lvGenes != null)
                    {
                        mDaughter.AddGenes(lvGenes);

                        if (lvGenes.Count == 2)
                        {
                            lvHashSetDaughter.Add(lvGenes[0]);
                            //mStopLocationOcupation = ((TrainIndividual)mSon).GetStopLocationOcupation();
                            //DumpStopLocation(lvGenes[lvGenes.Count - 1]);
                        }
                    }
                    else
                    {
                        lvQueueDaughter.Enqueue(lvGeneMother);
                        /*
                        DebugLog.Logar("Erro ao tentar unir Genes em crossover:");
                        ((TrainIndividual)mDaughter).DumpGene(null);
                        DebugLog.Logar("Erro ao tentar adicionar Gene (" + lvGeneMother + ")");
                        DebugLog.Logar("Genes de Mother:");
                        ((TrainIndividual)mMother).DumpGene(null);
                        DebugLog.Logar("Existe movimentacao pendente para sair de Dead Lock = " + VerifyPendentGene((TrainIndividual)mDaughter, (TrainIndividual)mMother, i, lvCount));
                        */
                        //mDaughter = null;
                    }
                }
            }
        }

        AddFromQueue(mSon, lvQueueSon, lvHashSetSon);
        AddFromQueue(mDaughter, lvQueueDaughter, lvHashSetDaughter);

        if (lvQueueSon.Count > 0)
        {
            mSon = null;
        }

        if (lvQueueDaughter.Count > 0)
        {
            mDaughter = null;
        }

        /*
        if (mSon != null)
        {
            mSon.IsValid();
        }

        if (mDaughter != null)
        {
            mDaughter.IsValid();
        }
         */ 
    }

    private IIndividual<Gene> Mutate(IIndividual<Gene> pIndividual)
    {
        Random lvRandom = new Random(DateTime.Now.Millisecond);
        int lvRandomValue = lvRandom.Next(1, 101);
        int lvNewPosition = -1;
        IIndividual<Gene> lvMutatedIndividual = null;
        List<Gene> lvNextMov = null;
        Queue<Gene> lvQueue = new Queue<Gene>();
        Gene lvRefGene = null;
        Gene lvGene = null;
        HashSet<Gene> lvHashSet = new HashSet<Gene>();
        int lvPrevGene = -1;
        int lvNextGene = -1;
        int lvEndIndex = -1;

        if (lvRandomValue <= mMutationRate)
        {
            lvRandomValue = lvRandom.Next(0, pIndividual.Count - 1);
            lvRefGene = pIndividual.GetGene(lvRandomValue);
            while (lvRefGene.HeadWayTime != DateTime.MinValue)
            {
                lvRandomValue++;
                lvRefGene = pIndividual.GetGene(lvRandomValue);

                if (lvRefGene == null)
                {
                    lvRandomValue = pIndividual.Count - 1;
                }
            }

            lvPrevGene = 0;
            for (int i = lvRandomValue - 1; i >= 0; i--)
            {
                lvGene = pIndividual.GetGene(i);

                if ((lvGene.TrainId == lvRefGene.TrainId) && (lvRefGene.HeadWayTime == DateTime.MinValue))
                {
                    lvPrevGene = i + 1;
                    break;
                }
            }

            lvNextGene = pIndividual.Count - 1;
            for (int i = lvRandomValue + 2; i < pIndividual.Count; i++)
            {
                lvGene = pIndividual.GetGene(i);

                if ((lvGene.TrainId == lvRefGene.TrainId) && (lvRefGene.HeadWayTime == DateTime.MinValue))
                {
                    lvNextGene = i - 1;
                    break;
                }
            }

            lvNewPosition = lvRandom.Next(lvPrevGene, lvNextGene + 1);

            if (lvNewPosition != lvRandomValue)
            {
                lvMutatedIndividual = new TrainIndividual(mFitness, mDateRef, null, null);
                if (lvNewPosition < lvRandomValue)
                {
                    lvEndIndex = lvNewPosition - 1;
                }
                else
                {
                    lvEndIndex = lvRandomValue - 1;
                }
                lvMutatedIndividual.AddGenes(pIndividual.GetGenes(0, lvEndIndex));

                for (int i = lvRandomValue + 1; i < lvNewPosition; i++)
                {
                    lvGene = pIndividual.GetGene(i);
                    if (lvGene.HeadWayTime == DateTime.MinValue)
                    {
                        AddFromQueue(lvMutatedIndividual, lvQueue, lvHashSet);
                        lvNextMov = lvMutatedIndividual.GetNextPosition(lvGene, DateTime.MaxValue);
                        if (lvNextMov != null)
                        {
                            lvMutatedIndividual.AddGenes(lvNextMov);
                        }
                        else
                        {
                            lvQueue.Enqueue(lvGene);
                        }
                    }
                }

                /* Insere o Gene na nova posição */
                AddFromQueue(lvMutatedIndividual, lvQueue, lvHashSet);
                lvNextMov = lvMutatedIndividual.GetNextPosition(lvRefGene);
                if (lvNextMov != null)
                {
                    lvMutatedIndividual.AddGenes(lvNextMov);
                }
                else
                {
                    lvQueue.Enqueue(lvGene);
                }

                if (lvNewPosition < lvRandomValue)
                {
                    for (int i = lvNewPosition + 1; i < pIndividual.Count; i++)
                    {
                        lvGene = pIndividual.GetGene(i);
                        if ((lvGene.HeadWayTime == DateTime.MinValue) && (lvGene != lvRefGene))
                        {
                            AddFromQueue(lvMutatedIndividual, lvQueue, lvHashSet);
                            lvNextMov = lvMutatedIndividual.GetNextPosition(lvGene);
                            if (lvNextMov != null)
                            {
                                lvMutatedIndividual.AddGenes(lvNextMov);
                            }
                            else
                            {
                                lvQueue.Enqueue(lvGene);
                            }
                        }
                    }
                }
                else
                {
                    for (int i = lvNewPosition; i < pIndividual.Count; i++)
                    {
                        lvGene = pIndividual.GetGene(i);
                        if (lvGene.HeadWayTime == DateTime.MinValue)
                        {
                            AddFromQueue(lvMutatedIndividual, lvQueue, lvHashSet);
                            lvNextMov = lvMutatedIndividual.GetNextPosition(lvGene);
                            if (lvNextMov != null)
                            {
                                lvMutatedIndividual.AddGenes(lvNextMov);
                            }
                            else
                            {
                                lvQueue.Enqueue(lvGene);
                            }
                            lvMutatedIndividual.AddGenes(lvNextMov);
                        }
                    }
                }
            }

            if (lvQueue.Count > 0)
            {
                AddFromQueue(lvMutatedIndividual, lvQueue, lvHashSet);
            }

            if (lvQueue.Count > 0)
            {
                lvMutatedIndividual = null;
            }
        }

        return lvMutatedIndividual;
    }

    public static string TrainAllowed
    {
        set
        {
            if (value.Trim().Length > 0)
            {
                string[] lvStrTrens = value.Trim().Split('|');
                foreach (string lvTrem in lvStrTrens)
                {
                    mTrainAllowed.Add(lvTrem);
                }
            }
        }
    }

    public void dump()
    {
        bool lvRes = DebugLog.mEnable;

        DebugLog.mEnable = true;
        DebugLog.Logar(" ");
        DebugLog.Logar(" -------------------------- dump Individuals --------------------------- ");
        DebugLog.Logar(" ");
        foreach (IIndividual<Gene> lvIndividual in mIndividuals)
        {
            DebugLog.Logar("Individual " + lvIndividual.GetUniqueId() + " = " + lvIndividual.Fitness + " (Tamanho: " + lvIndividual.Count + ")");
        }
        DebugLog.Logar(" ");
        DebugLog.Logar(" ----------------------------------------------------------------------  ");

        DebugLog.mEnable = lvRes;
    }
}