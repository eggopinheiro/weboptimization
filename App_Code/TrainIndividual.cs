using System;
using System.Collections.Generic;
using System.Web;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;
using System.Text;

/// <summary>
/// Summary description for TrainIndividual
/// </summary>
public class TrainIndividual : IIndividual<Gene>, IComparable<IIndividual<Gene>>
{
    private IFitness<Gene> mFitness;
    private Dictionary<StopLocation, HashSet<Gene>> mStopLocationOcupation = null;
    private Dictionary<StopLocation, Gene[]> mStopLocationDeparture = null;
    private Dictionary<double, Gene> mDicTrain = null; 
    private static Dictionary<double, List<Trainpat>> mPATs = null;
    private int mUniqueId = -1;
    private List<Gene> mList;
    private DateTime mDateRef;
    private double mMinSpeedLimit = 0.0;
    private double mCurrentFitness = double.MaxValue;
    private static int mTrainLen = 0;
    private static int mLimitDays = 3;
    private static double mVMA = 0.0;

    private HashSet<Gene> mDeadLockResponsable = null;

    public TrainIndividual(IFitness<Gene> pFitness, DateTime pDateRef, Dictionary<StopLocation, HashSet<Gene>> pStopLocationOcupation, Dictionary<StopLocation, Gene[]> pStopLocationDeparture)
	{
        mFitness = pFitness;
        mList = new List<Gene>();
        mUniqueId = RuntimeHelpers.GetHashCode(this);
        mDateRef = pDateRef;

        if (!Double.TryParse(ConfigurationManager.AppSettings["MIN_MOV_SPEED_LIMIT"], out mMinSpeedLimit))
        {
            mMinSpeedLimit = 0.0;
        }

        if (pStopLocationOcupation == null)
        {
            mStopLocationOcupation = new Dictionary<StopLocation, HashSet<Gene>>();
            foreach (StopLocation lvStopLocation in StopLocation.GetList())
            {
                mStopLocationOcupation.Add(lvStopLocation, new HashSet<Gene>());
            }
        }
        else
        {
            mStopLocationOcupation = new Dictionary<StopLocation, HashSet<Gene>>(pStopLocationOcupation);
        }

        if (pStopLocationDeparture == null)
        {
            mStopLocationDeparture = new Dictionary<StopLocation, Gene[]>();
            foreach (StopLocation lvStopLocation in StopLocation.GetList())
            {
                mStopLocationDeparture.Add(lvStopLocation, new Gene[lvStopLocation.Capacity]);
            }
        }
        else
        {
            mStopLocationDeparture = new Dictionary<StopLocation, Gene[]>(pStopLocationDeparture);
        }
        mDicTrain = new Dictionary<double, Gene>();

        mDeadLockResponsable = new HashSet<Gene>();
	}

    public HashSet<Gene> GetDeadLockResponsible()
    {
        return mDeadLockResponsable;
    }

    public Dictionary<StopLocation, HashSet<Gene>> GetStopLocationOcupation()
    {
        return mStopLocationOcupation;
    }

    public Dictionary<StopLocation, Gene[]> GetStopLocationDeparture()
    {
        return mStopLocationDeparture;
    }

    private bool IsCloseToCross(Gene pGene)
    {
        bool lvRes = false;
        Segment lvNextSwitch = null;
        StopLocation lvStopLocation = null;

        if (pGene.StopLocation != null)
        {
            lvNextSwitch = Segment.GetNextSwitchSegment(pGene.StopLocation.Location, pGene.Direction);

            if (pGene.Direction > 0)
            {
                lvStopLocation = StopLocation.GetNextStopSegment(lvNextSwitch.End_coordinate + 100, pGene.Direction * (-1));
            }
            else
            {
                lvStopLocation = StopLocation.GetNextStopSegment(lvNextSwitch.Start_coordinate - 100, pGene.Direction * (-1));
            }

            if (pGene.StopLocation.Location == lvStopLocation.Location)
            {
                lvRes = true;
            }
        }

        return lvRes;
    }

    public List<Gene> GetGenes(int pStartIndex, int pEndIndex)
    {
        int lvIndex = pEndIndex;
        Gene lvGene = null;

        if (pStartIndex < 0 || pEndIndex >= mList.Count)
        {
            return new List<Gene>();
        }

        lvGene = mList[lvIndex];
        while (lvGene.HeadWayTime == DateTime.MinValue)
        {
            lvIndex++;
            lvGene = mList[lvIndex];

            if (pEndIndex >= mList.Count)
            {
                return new List<Gene>();
            }
        }

        return mList.GetRange(pStartIndex, (lvIndex - pStartIndex) + 1);
    }

    public void AddGenes(List<Gene> pGenes)
    {
        StopLocation lvEndStopLocation = null;
        StopLocation lvStopLocation = null;
        Gene[] lvGenesStopLocation = null;
        HashSet<Gene> lvListGeneStopLocation = null;
        Gene lvNewGene = null;
        int lvIndex = -1;

        if (pGenes != null)
        {
            foreach (Gene lvGene in pGenes)
            {
                lvNewGene = (Gene)lvGene.Clone();
                if (lvNewGene.StopLocation != null)
                {
                    lvStopLocation = lvNewGene.StopLocation;
                }
                else
                {
                    lvStopLocation = StopLocation.GetCurrentStopSegment(lvNewGene.Coordinate, lvNewGene.Direction, out lvIndex);
                }

                lvEndStopLocation = StopLocation.GetCurrentStopSegment(lvNewGene.End, lvNewGene.Direction, out lvIndex);

                if (lvEndStopLocation == null)
                {
                    lvEndStopLocation = StopLocation.GetNextStopSegment(lvNewGene.End, lvNewGene.Direction);
                }

                if ((lvStopLocation == lvEndStopLocation) && (lvStopLocation != null))
                {
                    RemoveFromStopLocation(lvNewGene);
                }
                else
                {
                    if (lvGene.HeadWayTime == DateTime.MinValue)
                    {
                        RemoveFromStopLocation(lvGene);

                        if (lvNewGene.StopLocation != null)
                        {
                            lvGenesStopLocation = mStopLocationDeparture[lvNewGene.StopLocation];
                            if (lvGene.Track <= lvGenesStopLocation.Length)
                            {
                                lvGenesStopLocation[lvGene.Track - 1] = lvNewGene;
                            }
                        }
                    }
                    else
                    {
                        if (lvNewGene.StopLocation != null)
                        {
                            lvListGeneStopLocation = mStopLocationOcupation[lvNewGene.StopLocation];
                            lvListGeneStopLocation.Add(lvNewGene);
                            //DebugLog.Logar("AddGenes => Adicionando Gene (" + lvNewGene + ") a Stop Location (" + lvNewGene.StopLocation + ")");
                        }
                    }
                }

                if (!mDicTrain.ContainsKey(lvNewGene.TrainId))
                {
                    mDicTrain.Add(lvNewGene.TrainId, lvNewGene);
                }
                else
                {
                    mDicTrain[lvNewGene.TrainId] = lvNewGene;
                }

                mList.Add(lvNewGene);
            }
        }
    }

    public int Count
    {
        get
        {
            return mList.Count;
        }
    }

    public double Fitness
    {
        get { return mCurrentFitness; }
    }

    public double GetFitness()
    {
        if (mFitness != null)
        {
            mCurrentFitness = mFitness.GetFitness(this);
        }

        return mCurrentFitness;
    }

    public int CompareTo(IIndividual<Gene> pOther)
    {
        int lvRes = 0;

        if (pOther == null) return 1;

        if (pOther.Fitness == this.Fitness)
        {
            lvRes = 0;
        }
        else if (this.Fitness >= pOther.Fitness)
        {
            lvRes = 1;
        }
        else if (this.Fitness <= pOther.Fitness)
        {
            lvRes = -1;
        }

        return lvRes;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return mUniqueId;
        }
    }

    private void DumpNewMov(List<Gene> pGenes)
    {
        StringBuilder lvRes = new StringBuilder();

        foreach (Gene lvGene in pGenes)
        {
            if (lvRes.Length > 0)
            {
                lvRes.Append(" => " + lvGene);
            }
            else
            {
                lvRes.Append(lvGene);
            }
        }

        DebugLog.Logar("NewGenes = " + lvRes.ToString());
    }

    public override string ToString()
    {
        StringBuilder lvRes = new StringBuilder();

        foreach (Gene lvGene in mList)
        {
            if (lvRes.Length > 0)
            {
                lvRes.Append(" => ");
            }
            lvRes.Append(lvGene.TrainId);
            lvRes.Append(", Data: ");
            lvRes.Append(lvGene.Time);
            lvRes.Append(", Local: ");
            if (lvGene.StopLocation != null)
            {
                lvRes.Append(lvGene.StopLocation.Location);
            }
            else
            {
                lvRes.Append(lvGene.Location + "." + lvGene.UD);
            }
            lvRes.Append(", Status: ");
            if (lvGene.HeadWayTime == DateTime.MinValue)
            {
                lvRes.Append(" Saida");
            }
            else
            {
                lvRes.Append(" Chegada");
            }
            lvRes.Append(", Track: ");
            lvRes.Append(lvGene.Track);
            lvRes.Append(", Destino: ");
            lvRes.Append(lvGene.End);
        }

        return lvRes.ToString();
    }

    public string ToString(double pTrainId)
    {
        StringBuilder lvRes = new StringBuilder();

        foreach (Gene lvGene in mList)
        {
            if (pTrainId == lvGene.TrainId)
            {
                if (lvRes.Length > 0)
                {
                    lvRes.Append(" => ");
                }
                lvRes.Append(lvGene.TrainId);
                lvRes.Append(", Data: ");
                lvRes.Append(lvGene.Time);
                lvRes.Append(", Local: ");
                if (lvGene.StopLocation != null)
                {
                    lvRes.Append(lvGene.StopLocation.Location);
                }
                else
                {
                    lvRes.Append(lvGene.Location + "." + lvGene.UD);
                }
                lvRes.Append(", Status: ");
                if (lvGene.HeadWayTime == DateTime.MinValue)
                {
                    lvRes.Append(" Saida");
                }
                else
                {
                    lvRes.Append(" Chegada");
                }
                lvRes.Append(", Track: ");
                lvRes.Append(lvGene.Track);
                lvRes.Append(", Destino: ");
                lvRes.Append(lvGene.End);
            }
        }

        return lvRes.ToString();
    }

    public string GetFlotSeries()
    {
        Dictionary<double, StringBuilder> lvDicSeries = new Dictionary<double, StringBuilder>();
        StringBuilder lvStrSerie = null;
        StringBuilder lvRes = new StringBuilder();
        DateTime lvCurrentDate = DateTime.MinValue;
        string lvStrTrainLabel = "";
        string lvXValues = "";
        string lvYValues = "";
        string lvStrColor = "";

        lvCurrentDate = mDateRef;

        foreach (Gene lvGene in mList)
        {
            if (lvDicSeries.ContainsKey(lvGene.TrainId))
            {
                lvStrSerie = lvDicSeries[lvGene.TrainId];

                lvXValues = ConnectionManager.GetUTCDateTime(lvGene.Time).ToString();
                if (lvGene.StopLocation != null)
                {
                    lvYValues = ((double)lvGene.StopLocation.Location / 100000.0).ToString();
                }
                else
                {
                    lvYValues = ((double)lvGene.Coordinate / 100000.0).ToString();
                }
                lvXValues = lvXValues.Replace(",", ".");
                lvYValues = lvYValues.Replace(",", ".");

                lvStrSerie.Append(", [" + lvXValues + ", " + lvYValues + "]");
                lvStrSerie = null;
            }
            else
            {
                lvStrSerie = new StringBuilder();
                lvStrColor = Train.GetColorByTrainType(lvGene.TrainName.Substring(0, 1).ToUpper());
                lvStrTrainLabel = string.Format("{0} - {1}", lvGene.TrainName, lvGene.TrainId);
                lvStrSerie.Append("{\"color\": \"" + lvStrColor + "\", \"label\": \"" + lvStrTrainLabel + "\", \"ident\": \"" + lvGene.TrainId + "\", \"points\": {\"show\": true, \"radius\": 1, \"fill\": false}, \"lines\": {\"show\": false}, \"dashes\": {\"show\": true, \"lineWidth\": 3, \"dashLength\": 6}, \"hoverable\": true, \"clickable\": true, \"data\": [");

                lvXValues = ConnectionManager.GetUTCDateTime(lvGene.Time).ToString();
                if(lvGene.StopLocation != null)
                {
                    lvYValues = ((double)lvGene.StopLocation.Location / 100000.0).ToString();
                }
                else
                {
                    lvYValues = ((double)lvGene.Coordinate / 100000.0).ToString();
                }
                lvXValues = lvXValues.Replace(",", ".");
                lvYValues = lvYValues.Replace(",", ".");

                lvStrSerie.Append("[" + ConnectionManager.GetUTCDateTime(lvCurrentDate) + ", " + lvYValues + "]");
                lvStrSerie.Append(", [" + lvXValues + ", " + lvYValues + "]");

                lvDicSeries.Add(lvGene.TrainId, lvStrSerie);
                lvStrSerie = null;
            }
        }

        foreach (StringBuilder lvStrTrem in lvDicSeries.Values)
        {
            lvStrTrem.Append("]}");

            if (lvRes.Length > 0)
            {
                lvRes.Append(",");
            }
            lvRes.Append(lvStrTrem.ToString());
        }

        return lvRes.ToString();
    }

    public static void LoadPATs(DateTime pInitDate, DateTime pEndDate)
    {
        double lvTrainId = 0.0;
        DataSet ds = TrainpatDataAccess.GetPATTrain(pInitDate, pEndDate, "pmt_id, KMParada");
        List<Trainpat> lvTrainPat = null;

        mPATs = new Dictionary<double, List<Trainpat>>();

        foreach (DataRow row in ds.Tables[0].Rows)
        {
            Trainpat lvPat = new Trainpat();

            lvTrainId = ((row["train_id"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["train_id"]);
            lvPat.Pmt_id = ((row["pmt_id"] == DBNull.Value) ? "" : row["pmt_id"].ToString());
            lvPat.Prev_part = ((row["prev_part"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["prev_part"].ToString()));
            lvPat.KMOrigem = ((row["KMOrigem"] == DBNull.Value) ? Int16.MinValue : (Int16)row["KMOrigem"]);
            lvPat.KMDestino = ((row["KMDestino"] == DBNull.Value) ? Int16.MinValue : (Int16)row["KMDestino"]);
            lvPat.KMParada = ((row["KMParada"] == DBNull.Value) ? Int16.MinValue : (Int16)row["KMParada"]);
            lvPat.Activity = ((row["Activity"] == DBNull.Value) ? "" : row["Activity"].ToString());
            lvPat.Espera = ((row["Espera"] == DBNull.Value) ? Int32.MinValue : (int)row["Espera"]);
            lvPat.Date_hist = ((row["date_hist"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["date_hist"].ToString()));

            if (!mPATs.ContainsKey(lvTrainId))
            {
                lvTrainPat = new List<Trainpat>();
                lvTrainPat.Add(lvPat);
                mPATs.Add(lvTrainId, lvTrainPat);
            }
            else
            {
                mPATs[lvTrainId].Add(lvPat);
            }
        }

        //DebugLog.Logar("LoadPATs.mPATs.Count = " + mPATs.Count);
    }

    private void RemoveGeneFromAllLocation(Gene pGene)
    {
        foreach (HashSet<Gene> lvGenes in mStopLocationOcupation.Values)
        {
            lvGenes.Remove(pGene);
        }
    }

    private void RemoveFromStopLocation(Gene pGene)
    {
        HashSet<Gene> lvListGeneStopLocation = null;

        /* Atualizando a lista de Stop Location Ocupation */
        if (pGene.StopLocation != null)
        {
            lvListGeneStopLocation = mStopLocationOcupation[pGene.StopLocation];
            if (lvListGeneStopLocation.Contains(pGene))
            {
                lvListGeneStopLocation.Remove(pGene);
                DebugLog.Logar("Removendo Gene (" + pGene + ") da Stop Location (" + pGene.StopLocation + ")");
            }
            else
            {
                DebugLog.Logar("Nao foi possivel remover Gene (" + pGene + ") de (" + pGene.StopLocation + ") - não encontrado !");
            }
        }
        else
        {
            DebugLog.Logar("Nao foi possivel remover Gene (" + pGene + ") StopLocation == null ! Procurando em All Stop Locations");
            RemoveGeneFromAllLocation(pGene);
        }
    }

    private void DumpDuplicatePosition()
    {
        Dictionary<double, int> lvDicCountDuplicate = new Dictionary<double,int>();

        DebugLog.Logar(" ");
        DebugLog.Logar(" ----------------------------- DumpDuplicatePosition() ------------------------------- ");

        foreach(KeyValuePair<StopLocation, HashSet<Gene>> lvEntry in mStopLocationOcupation)
        {
            foreach (Gene lvGene in lvEntry.Value)
            {
                if(!lvDicCountDuplicate.ContainsKey(lvGene.TrainId))
                {
                    lvDicCountDuplicate.Add(lvGene.TrainId, 1);
                    DebugLog.Logar("Trem " + lvGene.TrainId + " inserido na lista de verificação de duplicação !");
                }
                else
                {
                    lvDicCountDuplicate[lvGene.TrainId]++;
                    DebugLog.Logar("Trem " + lvGene.TrainId + " duplicado em " + lvEntry.Key.Location);
                }
            }
        }

        DebugLog.Logar(" ------------------------------------------------------------------------------------- ");
        DebugLog.Logar(" ");
    }

    public bool IsValid()
    {
        bool lvRes = true;
        Dictionary<double, StopLocation> lvGenesStopLocation = new Dictionary<double, StopLocation>();
        StopLocation lvStopLocation = null;
        StopLocation lvPrevStopLocation = null;

        foreach (Gene lvGene in mList)
        {
            lvStopLocation = lvGene.StopLocation;

            if(lvStopLocation != null)
            {
                lvPrevStopLocation = StopLocation.GetNextStopSegment(lvStopLocation.Location, lvGene.Direction * (-1));
            }
            else
            {
                lvPrevStopLocation = StopLocation.GetNextStopSegment(lvGene.Coordinate, lvGene.Direction * (-1));
            }

            if(!lvGenesStopLocation.ContainsKey(lvGene.TrainId))
            {
                lvGenesStopLocation.Add(lvGene.TrainId, lvStopLocation);
            }
            else
            {
                if ((lvPrevStopLocation != null) && (lvStopLocation != null))
                {
                    if (lvPrevStopLocation != lvGenesStopLocation[lvGene.TrainId])
                    {
                        lvRes = false;
                        //DebugLog.Logar("Sequencia de Genes invalida (" + lvGene + "):");
                        //DebugLog.Logar(this.ToString());
                        break;
                    }
                }

                lvGenesStopLocation[lvGene.TrainId] = lvStopLocation;
            }
        }

        return lvRes;
    }

    public Gene DumpGene(Gene pGene)
    {
        Gene lvResGene = null;
        double lvTrainId = -1;
        StringBuilder lvRes = new StringBuilder();

        if (pGene != null)
        {
            lvTrainId = pGene.TrainId;
        }

        if (lvTrainId == -1)
        {
            for (int i = 0; i < mList.Count; i++)
            {
                if (lvRes.Length > 0)
                {
                    lvRes.Append(" <= ");
                }

                lvRes.Append("(" + mList[i].TrainId + " - " + mList[i].TrainName + ") ");
                if (mList[i].HeadWayTime == DateTime.MinValue)
                {
                    lvRes.Append(mList[i].Location + "." + mList[i].UD + " (Saida: " + mList[i].Time + ")");
                }
                else
                {
                    lvRes.Append(mList[i].Location + "." + mList[i].UD + " (Chegada: " + mList[i].Time + ")");
                }
            }
        }
        else
        {
            for (int i = mList.Count - 1; i >= 0; i--)
            {
                if (mList[i].TrainId == lvTrainId)
                {
                    if (lvRes.Length > 0)
                    {
                        lvRes.Append(" <= ");
                    }

                    if (mList[i].HeadWayTime == DateTime.MinValue)
                    {
                        lvRes.Append(mList[i].Location + "." + mList[i].UD + " (Saida: " + mList[i].Time + ")");
                    }
                    else
                    {
                        lvRes.Append(mList[i].Location + "." + mList[i].UD + " (Chegada: " + mList[i].Time + ")");
                    }

                    lvResGene = mList[i];
                }
            }
        }

        if (lvRes.Length > 0)
        {
            if (pGene != null)
            {
                lvRes.Insert(0, pGene.TrainId + " - " + pGene.TrainName + ": ");
            }
        }

        if (lvRes.Length > 0)
        {
            DebugLog.Logar("Gene " + lvRes.ToString());
        }
        else
        {
            DebugLog.Logar("O Gene especificado nao esta nesse individuo !");
        }

        return lvResGene;
    }

    public void DumpStopLocationByGene(Gene pGene)
    {
        List<StopLocation> lvListStopLocation = null;
        StopLocation lvNextStopLocation = StopLocation.GetNextStopSegment(pGene.Coordinate, pGene.Direction);

        if (lvNextStopLocation != null)
        {
            HashSet<Gene> lvGenes = mStopLocationOcupation[lvNextStopLocation];

            DebugLog.Logar(" ----------------------------  DumpStopLocationByGene(" + pGene.TrainId + " - " + pGene.TrainName + ") ------------------------------------ ");
            DebugLog.Logar(" ");

            lvListStopLocation = StopLocation.GetList();

            foreach (StopLocation lvStopLocation in lvListStopLocation)
            {
                lvGenes = mStopLocationOcupation[lvStopLocation];
                foreach (Gene lvGene in lvGenes)
                {
                    if (lvGene.TrainId == pGene.TrainId)
                    {
                        DebugLog.Logar(lvStopLocation.ToString());
                    }
                }
            }

            DebugLog.Logar(" -------------------------------------------------------------------------------------- ");
        }
    }

    private void DumpStopDepLocation(StopLocation pStopLocation)
    {
        Gene[] lvGenes = null;

        if (pStopLocation != null)
        {
            DebugLog.Logar(" ----------------------------  DumpStopDepLocation (" + pStopLocation + ") ------------------------------------ ");
            if (mStopLocationDeparture.ContainsKey(pStopLocation))
            {
                lvGenes = mStopLocationDeparture[pStopLocation];
                foreach (Gene lvGene in lvGenes)
                {
                    if (lvGene != null)
                    {
                        DebugLog.Logar(" ");
                        DebugLog.Logar("pGene.TrainId = " + lvGene.TrainId);
                        DebugLog.Logar("pGene.TrainName = " + lvGene.TrainName);
                        DebugLog.Logar("pGene.Location = " + lvGene.Location);
                        DebugLog.Logar("pGene.UD = " + lvGene.UD);
                        DebugLog.Logar("pGene.Coordinate = " + lvGene.Coordinate);
                        DebugLog.Logar("pGene.Track = " + lvGene.Track);
                        DebugLog.Logar("pGene.Direction = " + lvGene.Direction);
                        DebugLog.Logar("pGene.End = " + lvGene.End);
                        DebugLog.Logar("pGene.Time = " + lvGene.Time);
                    }
                }
            }

            DebugLog.Logar(" -------------------------------------------------------------------------------------------------------------- ");
        }
    }

    private void DumpNextStopLocation(Gene pGene)
    {
        int lvCoordinate = -1;
        int lvDirectionRef = 0;
        int lvDirection = 0;
        int lvIndex = 0;
        Interdicao lvInterdicao = null;
        StopLocation lvNextStopLocation = StopLocation.GetNextStopSegment(pGene.Coordinate, pGene.Direction);

        if (lvNextStopLocation == null)
        {
            return;
        }

        HashSet<Gene> lvNextGenes = mStopLocationOcupation[lvNextStopLocation];

        DebugLog.Logar(" ----------------------------  DumpNextStopLocation (Quant Proximos: " + lvNextGenes.Count + ") ------------------------------------ ");
        DebugLog.Logar(" ");
        DebugLog.Logar("pGene.TrainId = " + pGene.TrainId);
        DebugLog.Logar("pGene.TrainName = " + pGene.TrainName);
        if (pGene.StopLocation != null)
        {
            DebugLog.Logar("pGene.StopLocation.Location = " + pGene.StopLocation.Location);
        }
        DebugLog.Logar("pGene.Location = " + pGene.Location);
        DebugLog.Logar("pGene.UD = " + pGene.UD);
        DebugLog.Logar("pGene.Coordinate = " + pGene.Coordinate);
        DebugLog.Logar("pGene.Track = " + pGene.Track);
        DebugLog.Logar("pGene.Direction = " + pGene.Direction);
        DebugLog.Logar("pGene.End = " + pGene.End);
        DebugLog.Logar("pGene.Time = " + pGene.Time);
        lvDirectionRef = pGene.Direction;

        foreach (Gene lvGene in lvNextGenes)
        {
            DebugLog.Logar(" ");
            DebugLog.Logar("lvGene.TrainId = " + lvGene.TrainId);
            DebugLog.Logar("lvGene.TrainName = " + lvGene.TrainName);
            if (lvGene.StopLocation != null)
            {
                DebugLog.Logar("lvGene.StopLocation.Location = " + lvGene.StopLocation.Location);
            }
            DebugLog.Logar("lvGene.Location = " + lvGene.Location);
            DebugLog.Logar("lvGene.UD = " + lvGene.UD);
            DebugLog.Logar("lvGene.Coordinate = " + lvGene.Coordinate);
            DebugLog.Logar("lvGene.Track = " + lvGene.Track);
            DebugLog.Logar("lvGene.Direction = " + lvGene.Direction);
            DebugLog.Logar("lvGene.End = " + lvGene.End);
            DebugLog.Logar("lvGene.Time = " + lvGene.Time);
            lvDirection += lvGene.Direction;
        }

        lvCoordinate = lvNextStopLocation.Start_coordinate + (lvNextStopLocation.End_coordinate - lvNextStopLocation.Start_coordinate) / 2;
        lvInterdicao = Interdicao.GetCurrentInterdiction(lvCoordinate, out lvIndex);

        if (lvInterdicao != null)
        {
            DebugLog.Logar(" ");
            DebugLog.Logar("lvInterdicao.Ti_id = " + lvInterdicao.Ti_id);
            DebugLog.Logar("lvInterdicao.Ss_name = " + lvInterdicao.Ss_name);
            DebugLog.Logar("lvInterdicao.Start_pos = " + lvInterdicao.Start_pos);
            DebugLog.Logar("lvInterdicao.End_pos = " + lvInterdicao.End_pos);
            DebugLog.Logar("lvInterdicao.Start_time = " + lvInterdicao.Start_time);
            DebugLog.Logar("lvInterdicao.End_time = " + lvInterdicao.End_time);
        }

        if (lvDirectionRef > 0)
        {
            if (lvDirection < 0)
            {
                DebugLog.Logar("Dump em StopLocation (" + pGene.StopLocation + ")");
                if (pGene.StopLocation != null)
                {
                    DumpStopLocation(pGene.StopLocation);
                }

                DebugLog.Logar(" ");
                DebugLog.Logar("DeadLock Found:");
                DebugLog.Logar(" ");

                for (int i = mList.Count - 1; i >= 0; i--)
                {
                    if (mList[i].TrainId == pGene.TrainId)
                    {
                        if (mList[i].HeadWayTime == DateTime.MinValue)
                        {
                            DebugLog.Logar("Ultimo movimento do Gene que tentou se mover = " + mList[i]);
                            break;
                        }
                    }
                }

                foreach (Gene lvGene in lvNextGenes)
                {
                    for (int i = mList.Count - 1; i >= 0; i--)
                    {
                        if (mList[i].TrainId == lvGene.TrainId)
                        {
                            if (mList[i].HeadWayTime == DateTime.MinValue)
                            {
                                DebugLog.Logar("Ultimo movimento do Gene que estava no local = " + mList[i]);
                                break;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (lvDirection > 0)
            {
                DebugLog.Logar("Dump em StopLocation (" + pGene.StopLocation + ")");
                if (pGene.StopLocation != null)
                {
                    DumpStopLocation(pGene.StopLocation);
                }

                DebugLog.Logar(" ");
                DebugLog.Logar("DeadLock Found:");
                DebugLog.Logar(" ");

                for (int i = mList.Count - 1; i >= 0; i--)
                {
                    if (mList[i].TrainId == pGene.TrainId)
                    {
                        if (mList[i].HeadWayTime == DateTime.MinValue)
                        {
                            DebugLog.Logar("Ultimo movimento do Gene que tentou se mover = " + mList[i]);
                            break;
                        }
                    }
                }

                foreach (Gene lvGene in lvNextGenes)
                {
                    for (int i = mList.Count - 1; i >= 0; i--)
                    {
                        if (mList[i].TrainId == lvGene.TrainId)
                        {
                            if (mList[i].HeadWayTime == DateTime.MinValue)
                            {
                                DebugLog.Logar("Ultimo movimento do Gene que estava no local = " + mList[i]);
                                break;
                            }
                        }
                    }
                }
            }
        }

        DebugLog.Logar(" -------------------------------------------------------------------------------------- ");
    }

    private List<Gene> ProcessGene(Gene pGene, DateTime pCurrentTime)
    {
        List<Gene> lvNewGenes = null;
        Gene lvGene = null;
        StopLocation lvCurrentStopLocation = null;
        StopLocation lvEndStopLocation = null;
        double lvMeanCoordinate = 0.0;
        int lvIndex;

        if ((pGene.End != Int32.MinValue) && (pGene.Time < mDateRef.AddDays(mLimitDays)))
        {
            lvNewGenes = GetNextPosition(pGene, pCurrentTime);

            if (lvNewGenes != null)
            {
                if (lvNewGenes.Count > 1)
                {
                    //DebugLog.Logar(" ");

                    lvGene = lvNewGenes[lvNewGenes.Count - 1];

                    lvEndStopLocation = StopLocation.GetCurrentStopSegment(lvGene.End, lvGene.Direction, out lvIndex);
                    if(lvEndStopLocation == null)
                    {
                        lvEndStopLocation = StopLocation.GetNextStopSegment(lvGene.End, lvGene.Direction);
                    }

                    if (lvGene.StopLocation != null)
                    {
                        lvCurrentStopLocation = lvGene.StopLocation;
                    }
                    else
                    {
                        lvCurrentStopLocation = StopLocation.GetCurrentStopSegment(lvGene.Coordinate, lvGene.Direction, out lvIndex);
                    }
                    lvMeanCoordinate = lvCurrentStopLocation.Start_coordinate + (lvCurrentStopLocation.End_coordinate - lvCurrentStopLocation.Start_coordinate) / 2;

                    mList.AddRange(lvNewGenes);

                    if (mDicTrain.ContainsKey(lvGene.TrainId))
                    {
                        if (lvCurrentStopLocation != null)
                        {
                            if (((lvGene.Direction > 0) && (lvCurrentStopLocation.Location >= lvEndStopLocation.Location)) || ((lvGene.Direction < 0) && (lvCurrentStopLocation.Location <= lvEndStopLocation.Location)))
                            {
                                DebugLog.Logar(lvGene.TrainId + " - " + pGene.TrainName + " chegou ao seu destino na Location " + lvCurrentStopLocation.Location + " (Limite: " + lvGene.End + ")");
                                mDicTrain.Remove(lvGene.TrainId);
                                RemoveFromStopLocation(lvGene);
                            }
                        }
                        else
                        {
                            if (((lvGene.Direction > 0) && (lvMeanCoordinate >= lvGene.End)) || ((lvGene.Direction < 0) && (lvMeanCoordinate <= lvGene.End)))
                            {
                                DebugLog.Logar(lvGene.TrainId + " - " + pGene.TrainName + " chegou ao seu destino na Location " + lvCurrentStopLocation.Location + " (Limite: " + lvGene.End + ")");
                                mDicTrain.Remove(lvGene.TrainId);
                                RemoveFromStopLocation(lvGene);
                            }
                            else
                            {
                                mDicTrain[lvGene.TrainId] = lvGene;
                            }
                        }
                    }
                }
                else if (lvNewGenes.Count == 0)
                {
                    if (pGene.StopLocation != null)
                    {
                        DebugLog.Logar(pGene.TrainId + " - " + pGene.TrainName + " chegou ao seu destino na Location " + pGene.StopLocation.Location + " (Limite: " + pGene.End + ")");
                    }
                    else
                    {
                        DebugLog.Logar(pGene.TrainId + " - " + pGene.TrainName + " chegou ao seu destino na Location " + pGene.Location + "." + pGene.UD + " (Limite: " + pGene.End + ")");
                    }
                    mDicTrain.Remove(pGene.TrainId);
                    RemoveFromStopLocation(pGene);
                }
            }
            else
            {
                if (pGene.DepartureTime > pCurrentTime)
                {
                    DebugLog.Logar("Gene " + pGene.TrainId + " - " + pGene.TrainName + " (Partida: " + pGene.DepartureTime + ") recebeu null no movimento porque não chegou a hora de partida !");
                }
                else
                {
                    DebugLog.Logar("Gene " + pGene.TrainId + " - " + pGene.TrainName + " (" + pGene.TrainName + ") recebeu null no movimento !");
                    DumpNextStopLocation(pGene);
                    DumpStopLocationByGene(pGene);
                }
            }
        }
        else
        {
            if (mDicTrain.ContainsKey(pGene.TrainId))
            {
                DebugLog.Logar(pGene.TrainId + " - " + pGene.TrainName + " não possui destino ou tempo limite excedido (Location: " + pGene.Location + " Limite: " + pGene.End + "; Time: " + pGene.Time + ")");
                mDicTrain.Remove(pGene.TrainId);
                lvNewGenes = new List<Gene>();
            }
        }

        return lvNewGenes;
    }

    public bool GenerateIndividual(List<Gene> pTrainList, List<Gene> pPlanList)
    {
        bool lvRes = false;
        List<Gene> lvMovTurn = null;
        List<Gene> lvPlans = null;
        List<Gene> lvNewGenes = null;
        DateTime lvCurrentTime = DateTime.MaxValue;
        Random lvRandom;
        int lvTotalValue = 0;
        int lvTotal = 0;
        int lvRandomValue;
        int lvGeneCount = -1;
        int lvDicTrainCount = -1;
        int lvInd = -1;

        // Gerando individuo considerando a hora atual
        lvRandom = new Random(DateTime.Now.Millisecond);

        lvMovTurn = new List<Gene>(pTrainList);
        lvPlans = new List<Gene>(pPlanList);

        lvCurrentTime = mDateRef;

        foreach (Gene lvGen in pTrainList)
        {
            mDicTrain.Add(lvGen.TrainId, lvGen);
        }

        for (int i = 0; i < pTrainList.Count; i++)
        {
            /* Se não estiver me região de parada tem prioridade */
            if (pTrainList[i].StopLocation == null)
            {
                lvNewGenes = ProcessGene(pTrainList[i], lvCurrentTime);

                if (lvNewGenes != null)
                {
                    if (lvNewGenes.Count == 0)
                    {
                        mDicTrain.Remove(pTrainList[i].TrainId);
                    }

                    lvMovTurn.RemoveAt(0);
                }
            }
        }

        while (mDicTrain.Count > 0)
        {
            foreach (Gene lvGen in mDicTrain.Values)
            {
                if (lvGen.Time > lvCurrentTime)
                {
                    lvCurrentTime = lvGen.Time;
                }
            }

            while (lvMovTurn.Count > 0)
            {
                /* Obtem o total para rodar a roleta */
                lvTotalValue = 0;
                foreach (Gene lvGen in lvMovTurn)
                {
                    lvTotalValue += lvGen.Value;
                }
                lvRandomValue = lvRandom.Next(lvTotalValue+1);

                lvTotal = 0;
                for(int i=0; i<lvMovTurn.Count; i++)
                {
                    lvTotal += lvMovTurn[i].Value;

                    if (lvTotal >= lvRandomValue)
                    {
                        ProcessGene(lvMovTurn[i], lvCurrentTime);
                        lvMovTurn.RemoveAt(i);

                        break;
                    }
                }
            }

            lvInd = 0;
            for (int i = 0; i < lvPlans.Count; i++)
            {
                lvNewGenes = ProcessGene(lvPlans[i], lvCurrentTime);

                if (lvNewGenes != null)
                {
                    lvPlans.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }

            /*
            if (mDeadLockResponsable.Count > 0)
            {
                DebugLog.Logar(" ");
                DebugLog.Logar(" ------------------- Possivel DeadLock ! ---------------------- ");
            }
            foreach (Gene lvGeneDump in mDeadLockResponsable)
            {
                DumpNextStopLocation(lvGeneDump);
            }
            if (mDeadLockResponsable.Count > 0)
            {
                DebugLog.Logar(" --------------------------------------------------------------- ");
            }
            */

            lvMovTurn = new List<Gene>(mDicTrain.Values);
            /*
            DebugLog.Logar("Tamanho do Individuo " + mUniqueId + " = " + mList.Count);
            DebugLog.Logar("Tamanho do dic de trens do individuo " + mUniqueId + " = " + mDicTrain.Count);
            DebugLog.Logar("lvCurrentTime = " + lvCurrentTime);
             */ 
            //DebugLog.Logar("Montando Individuo " + mUniqueId + " => " + this.ToString(lvRefTrainDebug));

            if ((lvDicTrainCount == mDicTrain.Count) && (lvGeneCount == mList.Count))
            {
                DumpDicTrain(mDicTrain);
                DumpDeadLockResponsable();
                lvRes = false;
                break;
            }
            else
            {
                DumpDeadLockResponsable();
                DumpCountStopLocations(null);
                lvGeneCount = mList.Count;
                lvDicTrainCount = mDicTrain.Count;
            }
        }

        if(mDicTrain.Count == 0)
        {
            lvRes = true;
        }

        mCurrentFitness = GetFitness();

        return lvRes;
    }

    private void DumpDicTrain(Dictionary<double, Gene> pDicTrain)
    {
        bool lvRes = DebugLog.mEnable;

        DebugLog.mEnable = true;
        DebugLog.Logar(" ");
        DebugLog.Logar(" ---------------------------- pDicTrain" + "(count: " + pDicTrain.Count + ") ------------------------------- ");

        foreach(Gene lvGene in pDicTrain.Values)
        {
            if (lvGene.HeadWayTime == DateTime.MinValue)
            {
                DebugLog.Logar(lvGene.TrainId + " - " + lvGene.TrainName + " = " + lvGene.Time + " (Local: " + lvGene.Location + "." + lvGene.UD + ", Linha: " + lvGene.Track + ", Satus: Saída)");
            }
            else
            {
                DebugLog.Logar(lvGene.TrainId + " - " + lvGene.TrainName + " = " + lvGene.Time + " (Local: " + lvGene.Location + "." + lvGene.UD + ", Linha: " + lvGene.Track + ", Satus: Chegada)");
            }
        }

        DebugLog.Logar(" ------------------------------------------------------------------------------ ");
        DebugLog.Logar(" ");
        DebugLog.mEnable = lvRes;
    }

    private static int GetLineByUD(string pStrUD)
    {
        int lvRes = 0;

        if (pStrUD.StartsWith("CDV_1"))
        {
            lvRes = 1;
        }
        else if (pStrUD.StartsWith("CDV_2"))
        {
            lvRes = 2;
        }
        else if (pStrUD.StartsWith("CDV_3"))
        {
            lvRes = 3;
        }
        else if (pStrUD.Equals("SBA") || pStrUD.Equals("CAR02") || pStrUD.Equals("SLZ02") || pStrUD.Equals("PARKI") || pStrUD.Equals("1"))
        {
            lvRes = 1;
        }
        else if (pStrUD.Equals("PARKII") || pStrUD.Equals("2"))
        {
            lvRes = 2;
        }
        else if (pStrUD.Equals("PARKIII") || pStrUD.Equals("3"))
        {
            lvRes = 3;
        }

        return lvRes;
    }

    private DateTime GetEndInterdiction(int pInitCoordinate, int pEndCoordinate, DateTime pTimeRef, int pLine)
    {
        DateTime lvRes = DateTime.MinValue;
        int lvIndex = -1;
        int lvCoordinate = 0;
        List<Interdicao> lvInterdictions = null;
        Interdicao lvInterdicao = null;
        int lvCurrentLine = 0;

        lvCoordinate = pInitCoordinate + (pEndCoordinate - pInitCoordinate) / 2;

        lvInterdictions = Interdicao.GetList();
        lvInterdicao = Interdicao.GetCurrentInterdiction(lvCoordinate, out lvIndex);

        if (lvIndex >= 0)
        {
            for (int i = lvIndex; i < lvInterdictions.Count; i++)
            {
                lvInterdicao = lvInterdictions[i];
                if ((lvInterdicao.Start_time <= pTimeRef) && (lvInterdicao.End_time > pTimeRef) && (lvCoordinate >= lvInterdicao.Start_pos) && (lvCoordinate <= lvInterdicao.End_pos))
                {
                    lvCurrentLine = TrainIndividual.GetLineByUD(lvInterdicao.Ss_name);
                    if (lvCurrentLine == pLine || lvCurrentLine == 0)
                    {
                        lvRes = lvInterdicao.End_time;
                        break;
                    }
                }
            }
        }

        return lvRes;
    }

    private DateTime GetCurrentFirstOutputTime(Gene pGene, StopLocation pStopLocation, int pDirection)
    {
        DateTime lvRes = DateTime.MinValue;

        if (mStopLocationDeparture.ContainsKey(pStopLocation) && (pGene.Time > DateTime.MinValue))
        {
            foreach (Gene lvGene in mStopLocationDeparture[pStopLocation])
            {
                if (lvGene == null)
                {
                    lvRes = DateTime.MinValue;
                    break;
                }
                else
                {
                    if (pGene.TrainId != lvGene.TrainId)
                    {
                        if (lvRes == DateTime.MinValue)
                        {
                            lvRes = lvGene.Time;
                            //DebugLog.Logar("Atraso do trem " + pGene.TrainId + " Em " + pGene.Location + "." + pGene.UD + " com tempo anteior de " + pGene.Time + " foi para " + lvRes + " por lvLastDepTime (" + lvGene + ")");
                            //DumpStopDepLocation(pStopLocation);
                        }
                        else if (lvGene.Time < lvRes)
                        {
                            lvRes = lvGene.Time;
                            //DebugLog.Logar("Atraso do trem " + pGene.TrainId + " Em " + pGene.Location + "." + pGene.UD + " com tempo anteior de " + pGene.Time + " foi para " + lvRes + " por lvLastDepTime (" + lvGene + ")");
                            //DumpStopDepLocation(pStopLocation);
                        }
                    }
                }
            }
        }

        return lvRes;
    }

    private DateTime GetHeadWay(Gene pGene, StopLocation pStopLocation, int pDirection)
    {
        DateTime lvRes = DateTime.MinValue;
        DateTime lvMyHeadWay = DateTime.MinValue;

        if (pStopLocation != null)
        {
            if (mStopLocationOcupation.ContainsKey(pStopLocation))
            {
                foreach (Gene lvGene in mStopLocationOcupation[pStopLocation])
                {
                    if (pGene.TrainId == lvGene.TrainId)
                    {
                        lvMyHeadWay = lvGene.HeadWayTime;
                    }
                    else
                    {
                        if ((lvGene.HeadWayTime > lvRes) && (pDirection != lvGene.Direction))
                        {
                            lvRes = lvGene.HeadWayTime;
                            DebugLog.Logar("GetHeadWay => lvCurrentTime após HeadWayTime de (" + lvGene.TrainId + " - " + lvGene.TrainName + " em " + lvGene.Location + "." + lvGene.UD + ", HeadWayTime = " + lvGene.HeadWayTime + ") para Gene (" + pGene.TrainId + " - " + pGene.TrainName + " em " + lvGene.Location + "." + lvGene.UD + ", considerando Direction: " + pDirection + ") = " + lvRes);
                            DumpGene(lvGene);
                            DumpStopLocation(pStopLocation);
                        }
                    }
                }

                if ((lvRes == DateTime.MinValue) && (lvMyHeadWay != DateTime.MinValue))
                {
                    lvRes = lvMyHeadWay;
                    DebugLog.Logar("GetHeadWay => lvCurrentTime após MyHeadWay para Gene (" + pGene.TrainId + " - " + pGene.TrainName + " em (" + pStopLocation + "), considerando Direction: " + pDirection + ") = " + lvRes);
                    DumpGene(pGene);
                    DumpStopLocation(pStopLocation);
                }
            }
        }

        return lvRes;
    }

    private int GetPAT(StopLocation pStopLocation, double pTrainId)
    {
        int lvRes = 0;
        List<Trainpat> lvPATs = null;
        int lvKMParada = -1;

        if (pStopLocation != null)
        {
            if (mPATs.ContainsKey(pTrainId))
            {
                lvPATs = mPATs[pTrainId];

                foreach (Trainpat lvPAT in lvPATs)
                {
                    lvKMParada = lvPAT.Espera * 100000;

                    if (pStopLocation.Start_coordinate >= lvKMParada && pStopLocation.End_coordinate <= lvKMParada)
                    {
                        lvRes = lvPAT.Espera;
                        break;
                    }
                    else if (lvKMParada > pStopLocation.End_coordinate)
                    {
                        break;
                    }
                }
            }
        }

        return lvRes;
    }

    public static double GetOptimum(Gene pGene)
    {
        double lvRes = 0.0;
        double lvMinTime = double.MaxValue;
        double lvTime = double.MaxValue;
        List<Trainpat> lvPATs = null;
        StopLocation lvStopLocation = null;
        StopLocation lvNextStopLocation = null;
        StopLocation lvDestLocation = null;
        Segment lvCurrentSegment = null;
        Segment lvNextSegment = null;
        TrainPerformanceControl lvTrainPerformance = null;
        int lvMeanCoordinate = 0;
        int lvIndex;

        lvStopLocation = StopLocation.GetCurrentStopSegment(pGene.Coordinate, pGene.Direction, out lvIndex);
        lvCurrentSegment = Segment.GetCurrentSegment(pGene.Coordinate, pGene.Direction, pGene.Track, out lvIndex);

        lvDestLocation = StopLocation.GetCurrentStopSegment(pGene.End, pGene.Direction, out lvIndex);
        if (lvDestLocation == null)
        {
            lvDestLocation = StopLocation.GetNextStopSegment(pGene.End, pGene.Direction);
        }

        while ((lvStopLocation != lvDestLocation) && (lvCurrentSegment != null))
        {
            if (lvStopLocation != null)
            {
                lvNextStopLocation = StopLocation.GetNextStopSegment(lvStopLocation.Location, pGene.Direction);

                if (lvNextStopLocation == null)
                {
                    break;
                }

                lvMinTime = double.MaxValue;
                for (int i = (lvNextStopLocation.Capacity - 1); i >= 0; i--)
                {
                    lvTime = double.MaxValue;

                    if (pGene.Direction > 0)
                    {
                        lvNextSegment = Segment.GetCurrentSegment(lvNextStopLocation.Start_coordinate, pGene.Direction, (i + 1), out lvIndex);
                    }
                    else
                    {
                        lvNextSegment = Segment.GetCurrentSegment(lvNextStopLocation.End_coordinate, pGene.Direction, (i + 1), out lvIndex);
                    }

                    if (lvNextSegment != null)
                    {
                        lvTrainPerformance = TrainPerformanceControl.GetElementByKey(pGene.TrainName.Substring(0, 1), pGene.Direction, lvCurrentSegment.Location, lvCurrentSegment.SegmentValue, (i + 1));

                        if (lvTrainPerformance != null)
                        {
                            if (lvTrainPerformance.TimeMov <= 0.0)
                            {
                                if (pGene.Direction > 0)
                                {
                                    lvMeanCoordinate = lvNextSegment.Start_coordinate - lvCurrentSegment.Start_coordinate;
                                }
                                else
                                {
                                    lvMeanCoordinate = lvNextSegment.End_coordinate - lvCurrentSegment.End_coordinate;
                                }

                                lvTime = (Math.Abs(lvMeanCoordinate) / 100000) / VMA;
                            }
                            else 
                            {
                                if ((lvTrainPerformance.TimeMov / 60) < lvMinTime)
                                {
                                    lvTime = lvTrainPerformance.TimeMov / 60;
                                }
                            }

                        }
                        else
                        {
                            if (pGene.Direction > 0)
                            {
                                lvMeanCoordinate = lvNextSegment.Start_coordinate - lvCurrentSegment.Start_coordinate;
                            }
                            else
                            {
                                lvMeanCoordinate = lvNextSegment.End_coordinate - lvCurrentSegment.End_coordinate;
                            }

                            lvTime = (Math.Abs(lvMeanCoordinate) / 100000) / VMA;
                        }
                    }

                    if (lvTime < lvMinTime)
                    {
                        lvMinTime = lvTime;
                    }
                }
            }
            else
            {
                if (pGene.Direction > 0)
                {
                    lvNextStopLocation = StopLocation.GetNextStopSegment(lvCurrentSegment.Start_coordinate, pGene.Direction);
                    lvNextSegment = Segment.GetCurrentSegment(lvNextStopLocation.Start_coordinate, pGene.Direction, 1, out lvIndex);
                }
                else
                {
                    lvNextStopLocation = StopLocation.GetNextStopSegment(lvCurrentSegment.End_coordinate, pGene.Direction);
                    lvNextSegment = Segment.GetCurrentSegment(lvNextStopLocation.End_coordinate, pGene.Direction, 1, out lvIndex);
                }

                if (lvNextSegment != null)
                {
                    if (pGene.Direction > 0)
                    {
                        lvMeanCoordinate = lvNextSegment.Start_coordinate - lvCurrentSegment.Start_coordinate;
                    }
                    else
                    {
                        lvMeanCoordinate = lvNextSegment.End_coordinate - lvCurrentSegment.End_coordinate;
                    }

                    lvMinTime = (Math.Abs(lvMeanCoordinate) / 100000) / VMA;
                }
                else
                {
                    lvMinTime = 0.0;
                }
            }

            lvRes += lvMinTime;

            lvStopLocation = lvNextStopLocation;
            lvCurrentSegment = lvNextSegment;
        }

        if (mPATs.ContainsKey(pGene.TrainId))
        {
            lvPATs = mPATs[pGene.TrainId];

            foreach (Trainpat lvPAT in lvPATs)
            {
                lvRes += (double)lvPAT.Espera / 60.0;
            }
        }

        return lvRes;
    }

    private void DumpDeadLockVerify(Gene pRefGene, Gene pGene, Segment pNextSwitch, int pLine)
    {
        int lvLimitPosition = -1;

        if (pNextSwitch == null)
        {
            return;
        }

        if (pGene.Direction > 0)
        {
            lvLimitPosition = pNextSwitch.Start_coordinate;
        }
        else
        {
            lvLimitPosition = pNextSwitch.End_coordinate;
        }

        DebugLog.Logar(" ");
        DebugLog.Logar(" --------------------------  DumpDeadLockVerify -------------------------------------- ");

        DebugLog.Logar("lvLimitPosition = " + lvLimitPosition);
        DebugLog.Logar("pLine = " + pLine);
        DebugLog.Logar(" ");

        DebugLog.Logar("pRefGene.TrainId = " + pRefGene.TrainId);
        DebugLog.Logar("pRefGene.TrainName = " + pRefGene.TrainName);
        DebugLog.Logar("pRefGene.Location = " + pRefGene.Location);
        DebugLog.Logar("pRefGene.UD = " + pRefGene.UD);
        DebugLog.Logar("pRefGene.Time = " + pRefGene.Time);
        if (pRefGene.StopLocation != null)
        {
            DebugLog.Logar("pRefGene.StopLocation.Location = " + pRefGene.StopLocation.Location);
        }
        DebugLog.Logar("pRefGene.Coordinate = " + pRefGene.Coordinate);
        DebugLog.Logar("pRefGene.Track = " + pRefGene.Track);
        DebugLog.Logar("pRefGene.Direction = " + pRefGene.Direction);
        DebugLog.Logar(" ");

        DebugLog.Logar("pGene.TrainId = " + pGene.TrainId);
        DebugLog.Logar("pGene.TrainName = " + pGene.TrainName);
        DebugLog.Logar("pGene.Location = " + pGene.Location);
        DebugLog.Logar("pGene.UD = " + pGene.UD);
        DebugLog.Logar("pGene.Time = " + pGene.Time);
        if (pGene.StopLocation != null)
        {
            DebugLog.Logar("pGene.StopLocation.Location = " + pGene.StopLocation.Location);
        }
        DebugLog.Logar("pGene.Coordinate = " + pGene.Coordinate);
        DebugLog.Logar("pGene.Track = " + pGene.Track);
        DebugLog.Logar("pGene.Direction = " + pGene.Direction);

        DebugLog.Logar(" -------------------------- ---------------------------------------------------------- ");
        DebugLog.Logar(" ");
    }

    private void DumpBoolArray(bool[] pArray)
    {
        DebugLog.Logar(" ");
        DebugLog.Logar(" ------------------------------------------------------ ");
        for (int i = 0; i < pArray.Length; i++)
        {
            DebugLog.Logar("pArray[" + i + "] = " + pArray[i]);
        }
        DebugLog.Logar(" ------------------------------------------------------ ");
        DebugLog.Logar(" ");
    }

    private bool[] VerifyNextSegment(Gene pGene, int pNextStopLocationCoordinate, Segment pNextSwitch, int pCapacity, out bool pHasSameDirection, out int pSumDir)
    {
        bool[] lvRes = new bool[pCapacity];
        bool[] lvOccup = new bool[pCapacity];
        List<StopLocation> lvStopLocations = null;
        StopLocation lvStopLocation = null;
        DateTime lvEndInterditionTime = DateTime.MinValue;
        int lvLimitPosition = Int32.MinValue;
        int lvCurrentPosition = Int32.MinValue;
        int lvOccupCount = 0;
        int lvIndex = -1;

        lvStopLocations = StopLocation.GetList();

        pHasSameDirection = false;
        pSumDir = 0;
        for (int i = 0; i < lvRes.Length; i++)
        {
            lvRes[i] = true;
        }

        if (pGene.Direction > 0)
        {
            lvLimitPosition = pNextSwitch.Start_coordinate;
        }
        else
        {
            lvLimitPosition = pNextSwitch.End_coordinate;
        }
        lvCurrentPosition = pNextStopLocationCoordinate;
        lvStopLocation = StopLocation.GetCurrentStopSegment(lvCurrentPosition, pGene.Track, out lvIndex);
        lvCurrentPosition = lvStopLocation.Location;
        DebugLog.Logar("lvStopLocation (lvIndex: " + lvIndex + ") = " + lvStopLocation);

        DebugLog.Logar("VerifyNextSegment para " + pGene);
        DebugLog.Logar("pCapacity = " + pCapacity);

        DumpNextStopLocation(pGene);

        while (((lvCurrentPosition < lvLimitPosition) && (pGene.Direction > 0)) || ((lvCurrentPosition > lvLimitPosition) && (pGene.Direction < 0)))
        {
            //DebugLog.Logar("lvCurrentPosition = " + lvCurrentPosition);
            for (int i = 0; i < pCapacity; i++)
            {
                lvEndInterditionTime = GetEndInterdiction(lvStopLocation.Start_coordinate, lvStopLocation.End_coordinate, pGene.Time, (i + 1));

                if (lvEndInterditionTime != DateTime.MinValue)
                {
                    DebugLog.Logar("DeadLock por Interdiction em " + lvStopLocation);
                    DumpDeadLockVerify(pGene, pGene, pNextSwitch, (i + 1));
                    if (!lvOccup[i])
                    {
                        lvOccup[i] = true;
                        lvOccupCount++;
                        pSumDir += pGene.Direction * (-1);
                    }
                    lvRes[i] = false;
                }
            }

            foreach (Gene lvGene in mStopLocationOcupation[lvStopLocation])
            {
                DebugLog.Logar("Verificando (" + lvGene + ")");
                if (lvGene.Track <= pCapacity)
                {
                    DebugLog.Logar("pGene.Direction = " + pGene.Direction);
                    DebugLog.Logar("lvGene.Direction = " + lvGene.Direction);
                    if (pGene.Direction != lvGene.Direction)
                    {
                        lvRes[lvGene.Track - 1] = false;
                    }
                    else if (pGene.Direction == lvGene.Direction)
                    {
                        pHasSameDirection = true;
                    }

                    if (!lvOccup[lvGene.Track - 1])
                    {
                        lvOccup[lvGene.Track - 1] = true;
                        pSumDir += lvGene.Direction;
                        lvOccupCount++;
                    }
                }
            }

            DebugLog.Logar("lvStopLocation Antes de Atualizar (lvIndex: " + lvIndex + ") = " + lvStopLocation);

            if (pGene.Direction > 0)
            {
                if (lvIndex >= (lvStopLocations.Count - 1))
                {
                    break;
                }

                lvStopLocation = lvStopLocations[++lvIndex];
                lvCurrentPosition = lvStopLocation.Location;
            }
            else
            {
                if (lvIndex <= 0)
                {
                    break;
                }

                lvStopLocation = lvStopLocations[--lvIndex];
                lvCurrentPosition = lvStopLocation.Location;
            }
            DebugLog.Logar("lvStopLocation Depois de atualizar (lvIndex: " + lvIndex + ") = " + lvStopLocation);
            DebugLog.Logar("Verificando Stop Location:");

            DumpStopLocation(lvStopLocation);
            DumpBoolArray(lvRes);
        }

        if (lvOccupCount == pCapacity)
        {
            pHasSameDirection = false;
        }

        DumpBoolArray(lvRes);

        return lvRes;
    }

    private bool[] VerifySegmentDeadLock(Gene pGene, int pNextStopLocationCoordinate, Segment pNextSwitch, int pCapacity)
    {
        bool[] lvRes = null;
        bool[] lvResSameDir = new bool[pCapacity];
        bool lvHasSameDirection = false;
        List<StopLocation> lvStopLocations = null;
        StopLocation lvStopLocation = null;
        Segment lvNextSwitch = null;
        DateTime lvEndInterditionTime = DateTime.MinValue;
        int lvSumSameDir = 0;
        int lvLimitPosition = Int32.MinValue;
        int lvCurrentPosition = Int32.MinValue;
        int lvCapacity = pCapacity;

        DebugLog.Logar("VerifySegmentDeadLock para (" + pGene + ")");

        if (pGene.Track == 0)
        {
            return new bool[pCapacity];
        }

        if (pNextSwitch == null)
        {
            lvRes = new bool[pCapacity];
            for (int i = 0; i < lvRes.Length; i++)
            {
                lvRes[i] = true;
            }
            return lvRes;
        }

        lvStopLocations = StopLocation.GetList();

        DebugLog.Logar("Verificando ate " + pNextSwitch.Location);
        lvRes = VerifyNextSegment(pGene, pNextStopLocationCoordinate, pNextSwitch, pCapacity, out lvHasSameDirection, out lvSumSameDir);

        if(lvHasSameDirection)
        {
            DebugLog.Logar("lvHasSameDirection (" + pGene + "), verificando se ha trem em no mesmo sentido...");
            DumpNextStopLocation(pGene);

            /* Verificar o próximo do próximo vizinho (próximo do atual) */
            if (pGene.Direction > 0)
            {
                lvStopLocation = StopLocation.GetNextStopSegment(pNextSwitch.Start_coordinate, pGene.Direction);
                if (lvStopLocation != null)
                {
                    lvNextSwitch = Segment.GetNextSwitchSegment(lvStopLocation.Location, pGene.Direction);
                    lvLimitPosition = lvNextSwitch.Start_coordinate;
                }
                else
                {
                    DebugLog.Logar("lvHasSameDirection lvStopLocation == null, abortando verificacao para Gene (" + pGene + ")");
                    return lvRes;
                }
            }
            else
            {
                lvStopLocation = StopLocation.GetNextStopSegment(pNextSwitch.End_coordinate, pGene.Direction);
                if (lvStopLocation != null)
                {
                    lvNextSwitch = Segment.GetNextSwitchSegment(lvStopLocation.Location, pGene.Direction);
                    lvLimitPosition = lvNextSwitch.End_coordinate;
                }
                else
                {
                    DebugLog.Logar("lvHasSameDirection lvStopLocation == null, abortando verificacao para Gene (" + pGene + ")");
                    return lvRes;
                }
            }
            lvCurrentPosition = lvStopLocation.Location;
            DumpStopLocation(lvStopLocation);

            DebugLog.Logar("Verificando ate " + lvNextSwitch.Location);
            lvResSameDir = VerifyNextSegment(pGene, lvCurrentPosition, lvNextSwitch, pCapacity, out lvHasSameDirection, out lvSumSameDir);
            DebugLog.Logar("lvSumSameDir = " + lvSumSameDir);

            if (lvResSameDir.Length == 2)
            {
                if ((Math.Abs(lvSumSameDir) == pCapacity) || (!lvResSameDir[0] || !lvResSameDir[1]))
                {
                    lvRes = new bool[pCapacity];
                    DebugLog.Logar("Evitando DeadLock de " + pGene.TrainId + " (" + pGene.TrainName + ") em " + pGene.Location + "." + pGene.UD + " indo para " + lvStopLocation.Location);
                }
//                else if ((lvSumSameDir == pGene.Direction) || ((lvSumSameDir == 0) && (lvResSameDir[0] != lvResSameDir[1])))
                else if (lvSumSameDir == pGene.Direction)
                {
                    /* Verificar o próximo do próximo do próximo vizinho */
                    if (pGene.Direction > 0)
                    {
                        lvStopLocation = StopLocation.GetNextStopSegment(lvNextSwitch.Start_coordinate, pGene.Direction);
                        if (lvStopLocation != null)
                        {
                            lvNextSwitch = Segment.GetNextSwitchSegment(lvStopLocation.Location, pGene.Direction);
                            lvLimitPosition = lvNextSwitch.Start_coordinate;
                        }
                        else
                        {
                            DebugLog.Logar("lvHasSameDirection lvStopLocation == null, abortando verificacao para Gene (" + pGene + ")");
                            return lvRes;
                        }
                    }
                    else
                    {
                        lvStopLocation = StopLocation.GetNextStopSegment(lvNextSwitch.End_coordinate, pGene.Direction);
                        if (lvStopLocation != null)
                        {
                            lvNextSwitch = Segment.GetNextSwitchSegment(lvStopLocation.Location, pGene.Direction);
                            lvLimitPosition = lvNextSwitch.End_coordinate;
                        }
                        else
                        {
                            DebugLog.Logar("lvHasSameDirection lvStopLocation == null, abortando verificacao para Gene (" + pGene + ")");
                            return lvRes;
                        }
                    }
                    lvCurrentPosition = lvStopLocation.Location;
                    DumpStopLocation(lvStopLocation);

                    DebugLog.Logar("Verificando ate " + lvNextSwitch.Location);
                    lvResSameDir = VerifyNextSegment(pGene, lvCurrentPosition, lvNextSwitch, pCapacity, out lvHasSameDirection, out lvSumSameDir);
                    DebugLog.Logar("lvSumSameDir = " + lvSumSameDir);

                    if (lvSumSameDir == (pGene.Direction * pCapacity * (-1)))
                    {
                        lvRes = new bool[pCapacity];
                        DebugLog.Logar("Evitando DeadLock de " + pGene.TrainId + " (" + pGene.TrainName + ") em " + pGene.Location + "." + pGene.UD + " indo para " + lvStopLocation.Location);
                    }
                }
            }
        }

        return lvRes;
    }

    public List<Gene> GetNextPosition(Gene pGene, DateTime pInitialTime = default(DateTime))
    {
        List<Gene> lvRes = new List<Gene>();
        bool[] lvNextAvailable = null;
        HashSet<Gene> lvListGeneStopLocation = null;
        Gene[] lvGenesStopLocation = null;
        Gene lvNewGene = null;
        Gene lvGene = null;
        TrainPerformanceControl lvTrainPerformance = null;
        TrainPerformanceControl lvPrevTrainPerformance = null;
        StopLocation lvStopLocation = null;
        StopLocation lvNextStopLocation = null;
        StopLocation lvEndStopLocation = null;
        Segment lvCurrentSegment = null;
        Segment lvNextSegment = null;
        Segment lvPrevNextSegment = null;
        Segment lvLocDepartureSegment = null;
        Segment lvNextSwitch = null;
        DateTime lvEndInterditionTime = DateTime.MinValue;
        DateTime lvEndInterditionTime2 = DateTime.MinValue;
        DateTime lvCurrentTime = DateTime.MinValue;
        DateTime lvNextHeadWay = DateTime.MinValue;
        DateTime lvLastDepTime = DateTime.MinValue;
        double lvMeanSpeed = 0.0;
        double lvHeadWayTime = 0.0;
        double lvSpentTime = 0.0;
        double lvStayTime = 0.0;
        int lvInitCoordinate = Int32.MinValue;
        int lvEndCoordinate = Int32.MinValue;
        int lvMeanCoordinate = 0;
        int lvNextCapacity = 0;
        int lvPATTime = 0;
        int lvIndex;

        if (pGene.DepartureTime > pInitialTime)
        {
            return null;
        }

        if (mDicTrain.ContainsKey(pGene.TrainId))
        {
            lvGene = mDicTrain[pGene.TrainId];
        }
        else
        {
            lvGene = pGene;
        }

        if (lvGene.StopLocation != null)
        {
            lvStopLocation = lvGene.StopLocation;
        }
        else
        {
            lvStopLocation = StopLocation.GetCurrentStopSegment(lvGene.Coordinate, lvGene.Direction, out lvIndex);
            lvGene.StopLocation = lvStopLocation;
        }

        if (lvStopLocation != null)
        {
            lvMeanCoordinate = lvStopLocation.Start_coordinate + (lvStopLocation.End_coordinate - lvStopLocation.Start_coordinate) / 2;
            lvNextStopLocation = StopLocation.GetNextStopSegment(lvMeanCoordinate, lvGene.Direction);
        }
        else
        {
            lvNextStopLocation = StopLocation.GetNextStopSegment(lvGene.Coordinate, lvGene.Direction);
        }

        lvEndStopLocation = StopLocation.GetCurrentStopSegment(lvGene.End, lvGene.Direction, out lvIndex);
        lvCurrentSegment = Segment.GetCurrentSegment(lvGene.Coordinate, lvGene.Direction, lvGene.Track, out lvIndex);

        if(lvEndStopLocation == null)
        {
            lvEndStopLocation = StopLocation.GetNextStopSegment(lvGene.End, lvGene.Direction);
        }

        if (lvNextStopLocation == null)
        {
            mDicTrain.Remove(lvGene.TrainId);
            RemoveFromStopLocation(lvGene);
            return lvRes;
        }

        if ((lvStopLocation == lvEndStopLocation) && (lvStopLocation != null))
        {
            mDicTrain.Remove(lvGene.TrainId);
            RemoveFromStopLocation(lvGene);
            return lvRes;
        }

        if (lvGene.Direction > 0)
        {
            if (lvStopLocation != null)
            {
                if (lvStopLocation.Location >= lvEndStopLocation.Location)
                {
                    mDicTrain.Remove(lvGene.TrainId);
                    RemoveFromStopLocation(lvGene);
                    return lvRes;
                }
            }
            else if (lvGene.Coordinate >= lvEndStopLocation.Start_coordinate)
            {
                mDicTrain.Remove(lvGene.TrainId);
                RemoveFromStopLocation(lvGene);
                return lvRes;
            }
        }
        else
        {
            if (lvStopLocation != null)
            {
                if (lvStopLocation.Location <= lvEndStopLocation.Location)
                {
                    mDicTrain.Remove(lvGene.TrainId);
                    RemoveFromStopLocation(lvGene);
                    return lvRes;
                }
            }
            if (lvGene.Coordinate <= lvEndStopLocation.End_coordinate)
            {
                mDicTrain.Remove(lvGene.TrainId);
                RemoveFromStopLocation(lvGene);
                return lvRes;
            }
        }

        if (lvCurrentSegment == null)
        {
            lvCurrentSegment = Segment.GetNextSegment(lvGene.Coordinate, lvGene.Direction);
        }

        lvNextSwitch = Segment.GetNextSwitchSegment(lvNextStopLocation.Location, lvGene.Direction);
        lvNextAvailable = VerifySegmentDeadLock(lvGene, lvNextStopLocation.Location, lvNextSwitch, lvNextStopLocation.Capacity);

        lvNextCapacity = 0;
        for (int i = 0; i < lvNextAvailable.Length; i++)
        {
            if (lvNextAvailable[i])
            {
                lvNextCapacity++;
            }
        }

        DebugLog.Logar("lvNextCapacity = " + lvNextCapacity);

        /* Obtem o segmento de partida do Gene */
        if (lvStopLocation != null)
        {
            if (lvGene.Direction > 0)
            {
                lvLocDepartureSegment = Segment.GetCurrentSegment(lvStopLocation.End_coordinate, lvGene.Direction * (-1), lvGene.Track, out lvIndex);
            }
            else
            {
                lvLocDepartureSegment = Segment.GetCurrentSegment(lvStopLocation.Start_coordinate, lvGene.Direction * (-1), lvGene.Track, out lvIndex);
            }
        }
        else
        {
            lvLocDepartureSegment = lvCurrentSegment;
        }
        /* ------------------------------------  */

        DebugLog.Logar(" ");
        DebugLog.Logar(" --------- GetNextPosition ---------- ");
        DebugLog.Logar("pInitialTime para Gene (" + lvGene.TrainId + " - " + lvGene.TrainName + ") = " + pInitialTime);
        if (lvStopLocation != null)
        {
            DebugLog.Logar("lvStopLocation = " + lvStopLocation);
        }
        DebugLog.Logar("lvNextStopLocation = " + lvNextStopLocation);

        lvCurrentTime = GetHeadWay(lvGene, lvStopLocation, lvGene.Direction);
        lvNextHeadWay = GetHeadWay(lvGene, lvNextStopLocation, lvGene.Direction * (-1));

        DebugLog.Logar("lvCurrentTime para Gene (" + lvGene.TrainId + " - " + lvGene.TrainName + ") = " + lvCurrentTime);

        if (lvNextHeadWay > lvCurrentTime)
        {
            lvCurrentTime = lvNextHeadWay;
            DebugLog.Logar("lvCurrentTime atualizado por lvNextHeadWay para Gene (" + lvGene.TrainId + " - " + lvGene.TrainName + ") = " + lvCurrentTime);
        }

        if (lvGene.Time > lvCurrentTime)
        {
            lvCurrentTime = lvGene.Time;
            DebugLog.Logar("lvCurrentTime atualizado por lvGene.Time para Gene (" + lvGene.TrainId + " - " + lvGene.TrainName + ") = " + lvCurrentTime);
        }
        else if (lvGene.DepartureTime > lvCurrentTime)
        {
            lvCurrentTime = lvGene.DepartureTime;
            DebugLog.Logar("lvCurrentTime atualizado por lvGene.DepartureTime para Gene (" + lvGene.TrainId + " - " + lvGene.TrainName + ") = " + lvCurrentTime);
        }

        DebugLog.Logar("lvCurrentTime = " + lvCurrentTime);
        lvLastDepTime = GetCurrentFirstOutputTime(lvGene, lvNextStopLocation, lvGene.Direction);

        if (lvLastDepTime > lvCurrentTime)
        {
            lvCurrentTime = lvLastDepTime;
            DebugLog.Logar("lvCurrentTime atualizado por lvLastDepTime para Gene (" + lvGene.TrainId + " - " + lvGene.TrainName + ") = " + lvCurrentTime);
        }

        lvNextSwitch = Segment.GetNextSwitchSegment(lvGene.Coordinate, lvGene.Direction);

        if ((lvGene.Track <= lvNextCapacity) && (((lvGene.Direction > 0) && (lvNextStopLocation.Start_coordinate < lvNextSwitch.Start_coordinate)) || ((lvGene.Direction < 0) && (lvNextStopLocation.End_coordinate > lvNextSwitch.End_coordinate))))
        {
            for (int i = 0; i < lvNextCapacity; i++)
            {
                if ((lvGene.Track - 1) == i)
                {
                    lvNextAvailable[i] = true;
                }
                else
                {
                    lvNextAvailable[i] = false;
                }
            }
            lvNextCapacity = 1;
            DebugLog.Logar("NextCapacity reduzida para 1 devido ao local atual do Gene ser " + lvGene.Location + "." + lvGene.UD);

            if (mStopLocationOcupation.ContainsKey(lvNextStopLocation))
            {
                foreach (Gene lvGen in mStopLocationOcupation[lvNextStopLocation])
                {
                    if ((lvGene.Track == lvGen.Track) && lvGene.TrainId != lvGen.TrainId)
                    {
                        lvNextCapacity = 0;
                        if (lvGene.Track <= lvNextAvailable.Length)
                        {
                            lvNextAvailable[lvGene.Track - 1] = false;
                            DebugLog.Logar("NextCapacity reduzida em 1 devido ao local de destino (" + lvNextStopLocation + ") estar ocupada pelo Gene " + lvGene.TrainId + " - " + lvGene.TrainName + " em " + lvGene.Location + "." + lvGene.UD + ", track " + lvGene.Track);
                        }
                    }
                }
            }

            if (lvNextCapacity == 1)
            {
                lvEndInterditionTime = GetEndInterdiction(lvNextStopLocation.Start_coordinate, lvNextStopLocation.End_coordinate, lvGene.Time, lvGene.Track);

                if (lvEndInterditionTime <= lvCurrentTime)
                {
                    lvEndInterditionTime = DateTime.MinValue;
                }
            }
        }
        else
        {
            if (mStopLocationOcupation.ContainsKey(lvNextStopLocation))
            {
                foreach (Gene lvGen in mStopLocationOcupation[lvNextStopLocation])
                {
                    if (lvGen.Track <= lvNextAvailable.Length)
                    {
                        if ((lvNextAvailable[lvGen.Track - 1] == true) && (lvGene.TrainId != lvGen.TrainId))
                        {
                            lvNextAvailable[lvGen.Track - 1] = false;
                            lvNextCapacity--;
                            DebugLog.Logar("NextCapacity reduzida em 1 devido ao local de destino (" + lvNextStopLocation + ") estar ocupada pelo Gene " + lvGen.TrainId + " - " + lvGen.TrainName + " em " + lvGen.Location + "." + lvGen.UD + ", track " + lvGen.Track);
                        }
                    }
                }
            }

            if (lvNextCapacity > 0)
            {
                lvEndInterditionTime = GetEndInterdiction(lvNextStopLocation.Start_coordinate, lvNextStopLocation.End_coordinate, lvGene.Time, 1);
                lvEndInterditionTime2 = GetEndInterdiction(lvNextStopLocation.Start_coordinate, lvNextStopLocation.End_coordinate, lvGene.Time, 2);

                if (lvEndInterditionTime == DateTime.MinValue && lvEndInterditionTime2 == DateTime.MinValue)
                {
                    if (lvEndInterditionTime > lvEndInterditionTime2)
                    {
                        if (lvEndInterditionTime2 > lvCurrentTime)
                        {
                            lvEndInterditionTime = lvEndInterditionTime2;
                            lvNextCapacity--;
                            lvNextAvailable[0] = false;
                            DebugLog.Logar("NextCapacity reduzida em 1 devido a interdicao entre " + lvGene.Location + "." + lvGene.UD + " e (" + lvNextStopLocation + ")");
                        }
                        else
                        {
                            lvEndInterditionTime = DateTime.MinValue;
                        }
                    }
                    else
                    {
                        if (lvEndInterditionTime > lvCurrentTime)
                        {
                            lvNextCapacity--;
                            lvNextAvailable[1] = false;
                            DebugLog.Logar("NextCapacity reduzida em 1 devido a interdicao entre " + lvGene.Location + "." + lvGene.UD + " e (" + lvNextStopLocation + ")");
                        }
                        else
                        {
                            lvEndInterditionTime = DateTime.MinValue;
                        }
                    }
                }
            }
        }

        if (lvEndInterditionTime > lvCurrentTime)
        {
            lvCurrentTime = lvEndInterditionTime;
            DebugLog.Logar("lvCurrentTime atualizado por lvEndInterditionTime para Gene (" + lvGene.TrainId + " - " + lvGene.TrainName + ") = " + lvCurrentTime);
        }

        if (lvGene.Direction > 0)
        {
            if (lvStopLocation != null)
            {
                lvEndInterditionTime = GetEndInterdiction(lvStopLocation.End_coordinate, lvNextStopLocation.Start_coordinate, lvGene.Time, 1);
            }
            else
            {
                lvEndInterditionTime = GetEndInterdiction(lvGene.Coordinate, lvNextStopLocation.Start_coordinate, lvGene.Time, 1);
            }
        }
        else
        {
            if (lvStopLocation != null)
            {
                lvEndInterditionTime = GetEndInterdiction(lvNextStopLocation.End_coordinate, lvStopLocation.Start_coordinate, lvGene.Time, 1);
            }
            else
            {
                lvEndInterditionTime = GetEndInterdiction(lvNextStopLocation.End_coordinate, lvGene.Coordinate, lvGene.Time, 1);
            }
        }

        if (lvEndInterditionTime > lvCurrentTime)
        {
            lvCurrentTime = lvEndInterditionTime;
            DebugLog.Logar("lvCurrentTime atualizado por lvEndInterditionTime para Gene (" + lvGene.TrainId + " - " + lvGene.TrainName + ") = " + lvCurrentTime);
        }

        lvPATTime = GetPAT(lvStopLocation, lvGene.TrainId);

        if (lvPATTime > 0)
        {
            if (lvGene.HeadWayTime.AddMinutes(lvPATTime) > lvCurrentTime)
            {
                lvCurrentTime = lvGene.Time.AddMinutes(lvPATTime);
                DebugLog.Logar("lvCurrentTime atualizado por lvPATTime para Gene (" + lvGene.TrainId + " - " + lvGene.TrainName + ") = " + lvCurrentTime);
            }
        }

        if(lvCurrentTime > mDateRef.AddDays(mLimitDays))
        {
            return lvRes;
        }

        if (lvGene.Speed > mMinSpeedLimit)
        {
            lvSpentTime = (Math.Abs(lvCurrentSegment.Start_coordinate - lvCurrentSegment.End_coordinate) / 100000.0) / lvGene.Speed;
        }
        else
        {
            lvSpentTime = (Math.Abs(lvCurrentSegment.Start_coordinate - lvCurrentSegment.End_coordinate) / 100000.0) / mVMA;
        }
        lvStayTime = (lvCurrentTime - lvGene.Time).TotalHours;

        if(lvSpentTime < lvStayTime)
        {
            lvSpentTime = lvStayTime;
        }

        if (lvGene.Direction > 0)
        {
            lvInitCoordinate = lvLocDepartureSegment.Start_coordinate;
        }
        else
        {
            lvInitCoordinate = lvLocDepartureSegment.End_coordinate;
        }

        lvMeanCoordinate = lvNextStopLocation.Start_coordinate + (lvNextStopLocation.End_coordinate - lvNextStopLocation.Start_coordinate) / 2;
        for (int i = 0; i < lvNextAvailable.Length; i++)
        {
            if (lvNextAvailable[i])
            {
                lvNextSegment = Segment.GetCurrentSegment(lvMeanCoordinate, lvGene.Direction, (i + 1), out lvIndex);

                if(lvNextSegment == null)
                {
                    if(lvGene.Direction > 0)
                    {
                        lvNextSegment = Segment.GetCurrentSegment(lvNextStopLocation.Start_coordinate + 1000, lvGene.Direction, (i + 1), out lvIndex);
                    }
                    else
                    {
                        lvNextSegment = Segment.GetCurrentSegment(lvNextStopLocation.End_coordinate - 1000, lvGene.Direction, (i + 1), out lvIndex);
                    }
                }

                if (lvNextSegment != null)
                {
                    lvTrainPerformance = TrainPerformanceControl.GetElementByKey(lvGene.TrainName.Substring(0, 1), lvGene.Direction, lvGene.Location, lvGene.UD, (i + 1));

                    if (lvPrevTrainPerformance != null && lvTrainPerformance != null)
                    {
                        if (lvGene.Speed > mMinSpeedLimit)
                        {
                            if (lvTrainPerformance.TimeHeadWayMov < lvPrevTrainPerformance.TimeHeadWayMov)
                            {
                                lvTrainPerformance = lvPrevTrainPerformance;
                                lvNextSegment = lvPrevNextSegment;
                            }
                        }
                        else
                        {
                            if (lvTrainPerformance.TimeHeadWayStop < lvPrevTrainPerformance.TimeHeadWayStop)
                            {
                                lvTrainPerformance = lvPrevTrainPerformance;
                                lvNextSegment = lvPrevNextSegment;
                            }
                        }
                    }

                    if (lvTrainPerformance != null)
                    {
                        lvPrevTrainPerformance = lvTrainPerformance;
                        lvPrevNextSegment = lvNextSegment;
                    }
                }
            }
        }

        if (lvNextCapacity > 0)
        {
            if (lvGene.Direction > 0)
            {
                lvEndCoordinate = lvCurrentSegment.End_coordinate;
            }
            else
            {
                lvEndCoordinate = lvCurrentSegment.Start_coordinate;
            }
            lvMeanSpeed = (Math.Abs(lvEndCoordinate - lvGene.Coordinate) / 100000.0) / lvSpentTime;

            if (lvMeanSpeed > mVMA)
            {
                lvMeanSpeed = mVMA;
            }

            /* Finalmente monta o Gene de saída !!!! */
            lvNewGene = (Gene)lvGene.Clone();

            /*Gene de saída */
            lvNewGene.Time = lvGene.Time.AddHours(lvSpentTime);
            lvLastDepTime = lvNewGene.Time;
            lvNewGene.HeadWayTime = DateTime.MinValue;
            lvNewGene.Location = (short)lvLocDepartureSegment.Location;
            lvNewGene.UD = lvLocDepartureSegment.SegmentValue;
            lvNewGene.Coordinate = lvInitCoordinate;
            lvNewGene.Speed = lvMeanSpeed;
            lvRes.Add(lvNewGene);

            /* Atualizando a lista de Stop Location Departure */
            if (lvStopLocation != null)
            {
                lvGenesStopLocation = mStopLocationDeparture[lvStopLocation];
                if (lvGene.Track <= lvNextAvailable.Length)
                {
                    lvGenesStopLocation[lvNewGene.Track - 1] = lvNewGene;

                    //DumpStopDepLocation(lvStopLocation);
                }
            }

            /*Gene de chegada do próximo Stop Location */
            lvNewGene = (Gene)lvNewGene.Clone();
            lvNewGene.Track = 0;

            DebugLog.Logar("lvLastDepTime = " + lvLastDepTime);

            if (lvTrainPerformance == null)
            {
                for (int i = 0; i < lvNextAvailable.Length; i++)
                {
                    if (lvNextAvailable[i])
                    {
                        lvNewGene.Track = (short)(i + 1);
                        break;
                    }
                }

                if (lvNewGene.Track == 0)
                {
                    lvRes = null;
                }

                if (lvGene.Direction > 0)
                {
                    lvNextSegment = Segment.GetCurrentSegment(lvNextStopLocation.Start_coordinate, lvGene.Direction, lvNewGene.Track, out lvIndex);
                    lvEndCoordinate = lvNextSegment.Start_coordinate;
                }
                else
                {
                    lvNextSegment = Segment.GetCurrentSegment(lvNextStopLocation.End_coordinate, lvGene.Direction, lvNewGene.Track, out lvIndex);
                    lvEndCoordinate = lvNextSegment.End_coordinate;
                }
                lvNewGene.Coordinate = lvEndCoordinate;

                if (lvMeanSpeed <= mMinSpeedLimit)
                {
                    lvSpentTime = (Math.Abs(lvEndCoordinate - lvInitCoordinate) / 100000.0) / mVMA;
                }
                else
                {
                    lvSpentTime = (Math.Abs(lvEndCoordinate - lvInitCoordinate) / 100000.0) / lvMeanSpeed;
                }

                lvNewGene.Time = lvLastDepTime.AddHours(lvSpentTime);
                lvNewGene.Speed = lvMeanSpeed;
                if (lvMeanSpeed > mMinSpeedLimit)
                {
                    lvHeadWayTime = lvSpentTime + ((double)mTrainLen / 1000.0) / lvMeanSpeed;
                }
                else
                {
                    lvHeadWayTime = lvSpentTime;
                }

                DebugLog.Logar("lvHeadWayTime = " + lvHeadWayTime);
                lvNewGene.HeadWayTime = lvLastDepTime.AddHours(lvHeadWayTime);
            }
            else
            {
                lvNewGene.Track = lvTrainPerformance.DestinationTrack;

                if (lvGene.Direction > 0)
                {
                    lvNextSegment = Segment.GetCurrentSegment(lvNextStopLocation.Start_coordinate, lvGene.Direction, lvNewGene.Track, out lvIndex);
                    lvEndCoordinate = lvNextSegment.Start_coordinate;
                }
                else
                {
                    lvNextSegment = Segment.GetCurrentSegment(lvNextStopLocation.End_coordinate, lvGene.Direction, lvNewGene.Track, out lvIndex);
                    lvEndCoordinate = lvNextSegment.End_coordinate;
                }
                lvNewGene.Coordinate = lvEndCoordinate;

                if (lvMeanSpeed > mMinSpeedLimit)
                {
                    if (lvTrainPerformance.TimeMov <= 0.0)
                    {
                        lvMeanSpeed = (lvMeanSpeed + mVMA) / 2.0;
                    }
                    else
                    {
                        lvMeanSpeed = (lvMeanSpeed + (Math.Abs(lvEndCoordinate - lvInitCoordinate) / 100000.0) / (lvTrainPerformance.TimeMov / 60.0)) / 2.0;
                    }

                    lvSpentTime = (Math.Abs(lvEndCoordinate - lvInitCoordinate) / 100000.0) / lvMeanSpeed;
                    if (lvTrainPerformance.TimeHeadWayMov > 0)
                    {
                        lvHeadWayTime = lvTrainPerformance.TimeHeadWayMov / 60.0;
                    }
                    else
                    {
                        lvHeadWayTime = lvSpentTime + ((double)mTrainLen / 1000.0) / lvMeanSpeed;
                    }

                    DebugLog.Logar("lvHeadWayTime obtido de TrainPerformance lvMeanSpeed(" + lvMeanSpeed + ") > mMinSpeedLimit = " + lvHeadWayTime);
                }
                else
                {
                    if (lvTrainPerformance.TimeStop <= 0.0)
                    {
                        lvMeanSpeed = (lvNewGene.Speed + mVMA) / 2.0;
                        lvSpentTime = (Math.Abs(lvEndCoordinate - lvInitCoordinate) / 100000.0) / lvMeanSpeed;
                    }
                    else
                    {
                        lvMeanSpeed = (Math.Abs(lvEndCoordinate - lvInitCoordinate) / 100000.0) / (lvTrainPerformance.TimeStop / 60.0);
//                        lvMeanSpeed = (mVMA + lvMeanSpeed) / 2.0;
//                        lvMeanSpeed = (lvNewGene.Speed + lvMeanSpeed) / 2.0;
//                        lvSpentTime = (Math.Abs(lvEndCoordinate - lvInitCoordinate) / 100000.0) / lvMeanSpeed;

                        lvSpentTime = (lvTrainPerformance.TimeStop / 60.0);
                    }

                    if (lvTrainPerformance.TimeHeadWayStop > 0)
                    {
                        lvHeadWayTime = lvTrainPerformance.TimeHeadWayStop / 60.0;
                    }
                    else
                    {
                        lvHeadWayTime = lvSpentTime + ((double)mTrainLen / 1000.0) / lvMeanSpeed;
                    }

                    DebugLog.Logar("lvHeadWayTime obtido de TrainPerformance lvMeanSpeed(" + lvMeanSpeed + ") <= mMinSpeedLimit = " + lvHeadWayTime);
                }

                lvNewGene.Time = lvLastDepTime.AddHours(lvSpentTime);
                lvNewGene.Speed = lvMeanSpeed;
                lvNewGene.HeadWayTime = lvLastDepTime.AddHours(lvHeadWayTime);
            }
            lvNewGene.Location = (short)lvNextSegment.Location;
            lvNewGene.UD = lvNextSegment.SegmentValue;
            lvNewGene.StopLocation = lvNextStopLocation;

            if (lvNewGene.HeadWayTime == DateTime.MinValue)
            {
                DebugLog.Logar("Gene (" + lvNewGene + ") nao possui Headway na chegada !");
            }

            if (lvSpentTime >= 2.0)
            {
                DebugLog.Logar(lvNewGene + " demorou muito para chegar no proximo patio, verificar trainPerformance !");
            }

            lvRes.Add(lvNewGene);

            if(!mDicTrain.ContainsKey(lvNewGene.TrainId))
            {
                mDicTrain.Add(lvNewGene.TrainId, lvNewGene);
            }
            else
            {
                mDicTrain[lvNewGene.TrainId] = lvNewGene;
            }

            RemoveFromStopLocation(lvGene);
            DumpCountStopLocations(lvGene);

            if (lvStopLocation != null)
            {
                if (((lvNewGene.Direction > 0) && (lvNextStopLocation.Location < lvEndStopLocation.Location)) || ((lvNewGene.Direction < 0) && (lvNextStopLocation.Location > lvEndStopLocation.Location)))
                {
                    lvListGeneStopLocation = mStopLocationOcupation[lvNextStopLocation];
                    lvListGeneStopLocation.Add(lvNewGene);

                    DebugLog.Logar("Colocando (" + lvNewGene + ") na stop location (" + lvNextStopLocation + ")");

                    DumpStopLocation(lvStopLocation);
                    DumpStopLocation(lvNextStopLocation);
                }
                else
                {
                    mDicTrain.Remove(lvNewGene.TrainId);
                }
            }
            else
            {
                if (((lvNewGene.Direction > 0) && (lvMeanCoordinate < lvNewGene.End)) || ((lvNewGene.Direction < 0) && (lvMeanCoordinate > lvNewGene.End)))
                {
                    lvListGeneStopLocation = mStopLocationOcupation[lvNextStopLocation];
                    lvListGeneStopLocation.Add(lvNewGene);

                    DebugLog.Logar("Colocando (" + lvNewGene + ") na stop location (" + lvNextStopLocation + ")");

                    DumpStopLocation(lvNextStopLocation);
                }
                else
                {
                    mDicTrain.Remove(lvNewGene.TrainId);
                }
            }

            DebugLog.Logar("Gene Adicionado " + lvRes[0].TrainId + " (" + lvRes[0].TrainName + ") em " + lvRes[0].Location + "." + lvRes[0].UD + " => " + lvRes[1].TrainId + " (" + lvRes[1].TrainName + ") em " + lvRes[1].Location + "." + lvRes[1].UD + " com HeadWayTime de " + lvRes[1].HeadWayTime);
            mDeadLockResponsable.Remove(lvGene);
        }
        else
        {
            DebugLog.Logar("Gene " + lvGene.TrainId + " (" + lvGene.TrainName + ") nao pode sair de " + lvGene.Location + "." + lvGene.UD + " devido aos destinos estarem ocupados ! lvNextCapacity = " + lvNextCapacity);
            lvRes = null;

            if (lvStopLocation != null)
            {
                DumpStopLocation(lvStopLocation);
            }
            DumpNextStopLocation(lvGene);

            lvListGeneStopLocation = mStopLocationOcupation[lvNextStopLocation];
            foreach(Gene lvGen in lvListGeneStopLocation)
            {
                mDeadLockResponsable.Add(lvGen);
            }

            /*
            if (pInitialTime == DateTime.MaxValue)
            {
                DebugLog.Logar("Recebendo null no crossover...");
            }
             */ 
        }

        return lvRes;
    }

    private void DumpDeadLockResponsable()
    {
        StopLocation lvNextStopLocation = null;

        if (mDeadLockResponsable.Count == 0)
        {
            return;
        }

        DebugLog.Logar(" ");
        DebugLog.Logar(" --------------------------------------  DumpDeadLockResponsable ----------------------------------- ");
        foreach (Gene lvGene in mDeadLockResponsable)
        {
            DebugLog.Logar("Responsable = " + lvGene);
            if (lvGene.StopLocation != null)
            {
                foreach (Gene lvGen in mStopLocationOcupation[lvGene.StopLocation])
                {
                    DebugLog.Logar(" ");
                    DebugLog.Logar("lvGen.TrainId = " + lvGen.TrainId);
                    DebugLog.Logar("lvGen.TrainName = " + lvGen.TrainName);
                    if (lvGen.StopLocation != null)
                    {
                        DebugLog.Logar("lvGen.StopLocation.Location = " + lvGen.StopLocation.Location);
                    }
                    DebugLog.Logar("lvGen.Track = " + lvGen.Track);
                    DebugLog.Logar("lvGen.Location = " + lvGen.Location);
                    DebugLog.Logar("lvGen.UD = " + lvGen.UD);
                    DebugLog.Logar("lvGen.Coordinate = " + lvGen.Coordinate);
                    DebugLog.Logar("lvGen.End = " + lvGen.End);
                    DebugLog.Logar("lvGen.Time = " + lvGen.Time);
                }

                lvNextStopLocation = StopLocation.GetNextStopSegment(lvGene.StopLocation.Location, lvGene.Direction);
            }
            else
            {
                lvNextStopLocation = StopLocation.GetNextStopSegment(lvGene.Coordinate, lvGene.Direction);
            }

            DebugLog.Logar(" ");
            DebugLog.Logar("                   -------------------------------------------------                    ");
            foreach (Gene lvGen in mStopLocationOcupation[lvNextStopLocation])
            {
                DebugLog.Logar(" ");
                DebugLog.Logar("lvGen.TrainId = " + lvGen.TrainId);
                DebugLog.Logar("lvGen.TrainName = " + lvGen.TrainName);
                if (lvGen.StopLocation != null)
                {
                    DebugLog.Logar("lvGen.StopLocation.Location = " + lvGen.StopLocation.Location);
                }
                DebugLog.Logar("lvGen.Track = " + lvGen.Track);
                DebugLog.Logar("lvGen.Location = " + lvGen.Location);
                DebugLog.Logar("lvGen.UD = " + lvGen.UD);
                DebugLog.Logar("lvGen.Coordinate = " + lvGen.Coordinate);
                DebugLog.Logar("lvGen.End = " + lvGen.End);
                DebugLog.Logar("lvGen.Time = " + lvGen.Time);
            }

        }
        DebugLog.Logar(" ------------------------------------------------------------------------------------------- ");
        DebugLog.Logar(" ");
    }

    private void DumpCountStopLocations(Gene pGene)
    {
        int lvCount = 0;

        if (pGene == null)
        {
            foreach (Gene lvGene in mDicTrain.Values)
            {
                lvCount = 0;
                foreach (StopLocation lvStopLocation in mStopLocationOcupation.Keys)
                {
                    foreach (Gene lvGen in mStopLocationOcupation[lvStopLocation])
                    {
                        if (lvGen.TrainId == lvGene.TrainId)
                        {
                            lvCount++;
                        }
                    }
                }

                if (lvCount > 1)
                {
                    if (lvGene.StopLocation == null)
                    {
                        DebugLog.Logar("Stop Locations Count para (" + lvGene + ") = " + lvCount);
                    }
                    else
                    {
                        DebugLog.Logar("Stop Locations Count para (" + lvGene + ", Current: " + lvGene.StopLocation + ") = " + lvCount);
                    }
                }
            }
        }
        else
        {
            lvCount = 0;
            foreach (StopLocation lvStopLocation in mStopLocationOcupation.Keys)
            {
                foreach (Gene lvGen in mStopLocationOcupation[lvStopLocation])
                {
                    if (lvGen.TrainId == pGene.TrainId)
                    {
                        lvCount++;
                        DebugLog.Logar("Stop Locations para Gene (" + pGene + ") = " + lvStopLocation);
                    }
                }
            }

            if (lvCount > 0)
            {
                if (pGene.StopLocation == null)
                {
                    DebugLog.Logar("Stop Locations Count para (" + pGene + ") = " + lvCount);
                }
                else
                {
                    DebugLog.Logar("Stop Locations Count para (" + pGene + ", Current: " + pGene.StopLocation + ") = " + lvCount);
                }
                RemoveFromStopLocation(pGene);
            }
        }
    }

    private void DumpStopLocation(Gene pGene)
    {
        if (pGene != null)
        {
            DebugLog.Logar(" ");
            DebugLog.Logar(" -------------------------------------- DumpStopLocationsByGene ----------------------------------- ");
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
            DebugLog.Logar(" -------------------------------------- DumpStopLocation ----------------------------------- ");
            foreach(Gene lvGene in mStopLocationOcupation[pStopLocation])
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
            DebugLog.Logar(" ------------------------------------------------------------------------------------------- ");
            DebugLog.Logar(" ");
        }
    }

    public int GetUniqueId()
    {
        return mUniqueId;
    }

    public Gene GetGene(int pIndex)
    {
        Gene lvGene = null;

        if((pIndex < mList.Count) && (pIndex >= 0))
        {
            lvGene = mList[pIndex];
        }

        return lvGene;
    }

    public void SetGene(Gene pValue, int pIndex)
    {
        if ((pIndex < mList.Count) && (pIndex >= 0))
        {
            mList[pIndex] = pValue;
        }
    }

    public static double VMA
    {
        get { return TrainIndividual.mVMA; }
        set { TrainIndividual.mVMA = value; }
    }

    public static int TrainLen
    {
        get { return TrainIndividual.mTrainLen; }
        set { TrainIndividual.mTrainLen = value; }
    }

    public static int LimitDays
    {
        get { return TrainIndividual.mLimitDays; }
        set { TrainIndividual.mLimitDays = value; }
    }
}