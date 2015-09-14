using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Configuration;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using System.Web.Script.Services;

/// <summary>
/// Summary description for GAOptimization
/// </summary>
[WebService(Namespace = "http://www.silnweb.com/gaoptimization")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class GAOptimization : System.Web.Services.WebService 
{
    private DateTime mInitialDate;
    private DateTime mFinalDate;

    public GAOptimization () 
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(CacheDuration = 60)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
    public string GetPlan(string pStrPriority)
    {
        string lvRes = "";
        string lvStrTrainAllowed = "";
        int lvPopulationSize = 0;
        int lvMaxGenerations = 0;
        int lvMutationRate = 0;
        IFitness<Gene> lvFitness = null;
        Population lvPopulation = null;
        TrainIndividual lvTrainIndividual = null;
        bool lvLogEnable = DebugLog.mEnable;

        if (!DateTime.TryParse(ConfigurationManager.AppSettings["INITIAL_DATE"], out mInitialDate))
        {
            mInitialDate = DateTime.Now.Date;
            mFinalDate = mInitialDate.Date == DateTime.Now.Date ? DateTime.Now : mInitialDate.Date.AddDays(1).AddSeconds(-1);
        }
        else
        {
            mFinalDate = mInitialDate.Date + DateTime.Now.TimeOfDay;
        }

        TrainIndividual.VMA = double.Parse(ConfigurationManager.AppSettings["VMA"]);
        TrainIndividual.TrainLen = int.Parse(ConfigurationManager.AppSettings["TRAIN_LEN"]);
        TrainIndividual.LimitDays = int.Parse(ConfigurationManager.AppSettings["LIMIT_DAYTIME"]);

        DebugLog.mEnable = true;

        DebugLog.Logar("TrainIndividual.VMA = " + TrainIndividual.VMA);
        DebugLog.Logar("TrainIndividual.TrainLen = " + TrainIndividual.TrainLen);

        lvPopulationSize = int.Parse(ConfigurationManager.AppSettings["POPULATION_SIZE"]);
        lvMaxGenerations = int.Parse(ConfigurationManager.AppSettings["MAX_GENERATIONS"]);
        lvMutationRate = int.Parse(ConfigurationManager.AppSettings["MUTATION_RATE"]);
        lvStrTrainAllowed = ConfigurationManager.AppSettings["TRAIN_TYPE_ALLOWED"];

        Population.TrainAllowed = lvStrTrainAllowed;

        DebugLog.Logar("lvPopulationSize = " + lvPopulationSize);
        DebugLog.Logar("lvMaxGenerations = " + lvMaxGenerations);
        DebugLog.Logar("lvMutationRate = " + lvMutationRate);
        DebugLog.Logar("lvStrTrainAllowed = " + lvStrTrainAllowed);

        DebugLog.Logar("lvInitialDate = " + mInitialDate);
        DebugLog.Logar("lvFinalDate = " + mFinalDate);

        StopLocation.LoadList();
        Segment.LoadList();
//        Interdicao.LoadList(mInitialDate, mFinalDate);
        Interdicao.LoadList(DateTime.Now, DateTime.Now);
        TrainIndividual.LoadPATs(mInitialDate, mFinalDate);
        TrainPerformanceControl.LoadDic();

        DebugLog.Logar(" ");

        DebugLog.mEnable = lvLogEnable;

        lvFitness = new RailRoadFitness();
        lvPopulation = new Population(lvFitness, lvPopulationSize, lvMutationRate, true, mInitialDate, mFinalDate, pStrPriority);

        DebugLog.mEnable = true;
        DebugLog.Logar(" ");
        DebugLog.Logar("Individuos = " + lvPopulation.Count);
        DebugLog.Logar(" ");
        DebugLog.mEnable = lvLogEnable;

        for (int i = 0; i < lvMaxGenerations; i++)
        {
            DebugLog.mEnable = true;
            DebugLog.Logar("Generation = " + i);
            DebugLog.mEnable = lvLogEnable;
            lvPopulation.NextGeneration();
            lvPopulation.dump();
        }

        lvTrainIndividual = (TrainIndividual)lvPopulation.GetIndividualAt(0);

        DebugLog.mEnable = true;
        DebugLog.Logar("Melhor = " + lvTrainIndividual.ToString());

        lvRes = lvTrainIndividual.GetFlotSeries();

        DebugLog.Logar("lvRes = " + lvRes);
        DebugLog.mEnable = lvLogEnable;

        return lvRes;
    }
    
}
