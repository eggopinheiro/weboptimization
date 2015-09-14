using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for IIndivudual
/// </summary>
public interface IIndividual<T> : IComparable<IIndividual<T>>
{
    int Count { get; }
    double Fitness { get; }
    bool GenerateIndividual(List<T> pTrainList, List<T> pPlanList);
    double GetFitness();
    bool IsValid();
    int GetUniqueId();
    List<T> GetNextPosition(T pGene, DateTime pCurrentTime = default(DateTime));
    List<T> GetGenes(int pStartIndex, int pEndIndex);
    void AddGenes(List<T> pGenes);
    T GetGene(int pIndex);
    void SetGene(T pValue, int pIndex);
}