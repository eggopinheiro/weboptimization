using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text;
/// <summary>
/// Criado por Eggo Pinheiro em 13/04/2015 18:56:58
/// <summary>

public class StopLocation : IEquatable<StopLocation>, IComparable<StopLocation>
{
    protected static List<StopLocation> mListStopLoc = null;
//    private ComparerTGSegmentDataType mTGSegmentComparer = null; 
    protected int lvlocation = -1;
	protected int lvstart_coordinate;
	protected int lvend_coordinate;
	protected Int16 lvcapacity;

    private static int FIRST_STOP_SEGMENT = Int32.MinValue;
    private static int LAST_STOP_SEGMENT = Int32.MinValue;

	public StopLocation()
	{
		Clear();
	}

	public StopLocation(int location)
	{
		Clear();

		this.lvlocation = location;
		Load();
    }

	public StopLocation(int location, int start_coordinate, int end_coordinate, Int16 capacity)
	{
		this.lvlocation = location;
		this.lvstart_coordinate = start_coordinate;
		this.lvend_coordinate = end_coordinate;
		this.lvcapacity = capacity;
    }

    public int CompareTo(StopLocation pOther)
    {
        int lvRes = 0;

        if (pOther == null) return 1;

        if (pOther.Start_coordinate >= this.Start_coordinate && pOther.End_coordinate <= this.End_coordinate)
        {
            lvRes = 0;
        }
        else if (this.Start_coordinate >= pOther.Start_coordinate)
        {
            lvRes = 1;
        }
        else if (this.Start_coordinate <= pOther.Start_coordinate)
        {
            lvRes = -1;
        }

        return lvRes;
    }

    public static bool operator ==(StopLocation obj1, StopLocation obj2)
    {
        bool lvRes = false;

        if (ReferenceEquals(obj1, null) && ReferenceEquals(obj2, null))
        {
            return true;
        }
        else if (ReferenceEquals(obj1, null))
        {
            return false;
        }
        else if (ReferenceEquals(obj2, null))
        {
            return false;
        }

        if ((obj1.lvstart_coordinate >= obj2.lvstart_coordinate) && (obj1.lvend_coordinate <= obj2.lvend_coordinate))
        {
            lvRes = true;
        }

        return lvRes;
    }

    public static bool operator !=(StopLocation obj1, StopLocation obj2)
    {
        return !(obj1 == obj2);
    }

    public bool Equals(StopLocation other)
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

        if ((this.lvlocation == other.lvlocation) && (this.lvstart_coordinate == other.lvstart_coordinate) && (this.lvend_coordinate == other.lvend_coordinate))
        {
            lvRes = true;
        }

        return lvRes;
    }

    public override bool Equals(object obj)
    {
        bool lvRes = false;

        if (obj is StopLocation)
        {
            lvRes = Equals(obj as StopLocation);
        }

        return lvRes;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int lvHashCode = 0;

            if (lvlocation != -1)
            {
                lvHashCode = lvlocation;
            }

            return lvHashCode;
        }
    }

	public virtual bool Load()
	{
		bool lvResult = false;


		DataSet ds = StopLocationDataAccess.GetDataByKey(this.lvlocation, "");

		foreach (DataRow row in ds.Tables[0].Rows)
		{
			this.lvlocation = ((row["location"] == DBNull.Value) ? Int32.MinValue : (int)row["location"]);
			this.lvstart_coordinate = ((row["start_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["start_coordinate"]);
			this.lvend_coordinate = ((row["end_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["end_coordinate"]);
			this.lvcapacity = ((row["capacity"] == DBNull.Value) ? Int16.MinValue : (Int16)row["capacity"]);

			lvResult = true;
		}

		return lvResult;
	}

	public string GetFlotSerie(string pStrColor, string pStrLabel, string pStrXAxisName, string pStrYAxisName, string pStrIdent, Boolean isDashed, string pStrSymbol)
	{
		StringBuilder lvResult = new StringBuilder();
		DataSet ds = null;
		string lvXValues = "";
		string lvYValues = "";
		Boolean lvHasElement = false;

		ds = StopLocationDataAccess.GetData(this.lvlocation, this.lvstart_coordinate, this.lvend_coordinate, this.lvcapacity, "");

		if (String.IsNullOrEmpty(pStrSymbol))
		{
			if(isDashed)
			{
				lvResult.Append("{\"color\": \"" + pStrColor + "\", \"label\": \"" + pStrLabel + "\", \"ident\": \"" + pStrIdent + "\", \"points\": {\"show\": true, \"radius\": 1, \"fill\": false}, \"lines\": {\"show\": false}, \"dashes\": {\"show\": true, \"lineWidth\": 3, \"dashLength\": 6}, \"hoverable\": true, \"clickable\": true, \"data\": [");
			}
			else
			{
				lvResult.Append("{\"color\": \"" + pStrColor + "\", \"label\": \"" + pStrLabel + "\", \"ident\": \"" + pStrIdent + "\", \"points\": {\"show\": false, \"radius\": 2}, \"lines\": {\"show\": true, \"lineWidth\": 3}, \"data\": [");
			}
		}
		else
		{
				lvResult.Append("{\"color\": \"" + pStrColor + "\", \"label\": \"" + pStrLabel + "\", \"ident\": \"" + pStrIdent + "\", \"points\": {\"show\": true, \"radius\": 2, \"symbol\": \"" + pStrSymbol + "\"}, \"data\": [");
		}

		foreach (DataRow row in ds.Tables[0].Rows)
		{

			if(row[pStrXAxisName.Trim()].GetType().Name.Equals("DateTime"))
			{
				lvXValues = ConnectionManager.GetUTCDateTime((row[pStrXAxisName.Trim()] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row[pStrXAxisName.Trim()].ToString())).ToString();
			}
			else
			{
				lvXValues = ((row[pStrXAxisName.Trim()] == DBNull.Value) ? "0" : row[pStrXAxisName.Trim()].ToString());
			}

			if(row[pStrYAxisName.Trim()].GetType().Name.Equals("DateTime"))
			{
				lvYValues = ConnectionManager.GetUTCDateTime((row[pStrYAxisName.Trim()] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row[pStrYAxisName.Trim()].ToString())).ToString();
			}
			else
			{
				lvYValues = ((row[pStrYAxisName.Trim()] == DBNull.Value) ? "0" : row[pStrYAxisName.Trim()].ToString());
			}

			lvXValues = lvXValues.Replace(",", ".");
			lvYValues = lvYValues.Replace(",", ".");

			if(!lvHasElement)
			{
				lvResult.Append("[" + lvXValues + ", " + lvYValues + "]");
				lvHasElement = true;
			}
			else
			{
				lvResult.Append(", [" + lvXValues + ", " + lvYValues + "]");
			}
		}

		lvResult.Append("]}");


		return lvResult.ToString();
	}

	public string GetFlotClass(string pStrLabel)
	{
		string lvResult = "";
		DataSet ds = null;
		string lvStrFlotClass = "";
		string lvStrVector = "";
		int lvLabelLen = -1;
		Boolean lvHasElement = false;

		lvLabelLen = pStrLabel.IndexOf(".");
		if(lvLabelLen > -1)
		{
			lvStrVector = pStrLabel.Substring(0, lvLabelLen).Trim();
		}
		else
		{
			lvStrVector = pStrLabel.Trim();
		}

		if(lvStrVector.Length == 0)
		{
			return lvResult;
		}

		ds = StopLocationDataAccess.GetData(this.lvlocation, this.lvstart_coordinate, this.lvend_coordinate, this.lvcapacity, "");

		lvResult = "var " + lvStrVector + " = [";

		foreach (DataRow row in ds.Tables[0].Rows)
		{

			if(lvHasElement)
			{
				lvStrFlotClass = ", {";
			}
			else
			{
				lvStrFlotClass = "{";
				lvHasElement = true;
			}

			lvStrFlotClass += "Location: " + ((row["location"] == DBNull.Value) ? "\"\"" : row["location"].ToString()) + ", ";
			lvStrFlotClass += "Start_coordinate: " + ((row["start_coordinate"] == DBNull.Value) ? "\"\"" : row["start_coordinate"].ToString()) + ", ";
			lvStrFlotClass += "End_coordinate: " + ((row["end_coordinate"] == DBNull.Value) ? "\"\"" : row["end_coordinate"].ToString()) + ", ";
			lvStrFlotClass += "Capacity: " + ((row["capacity"] == DBNull.Value) ? "\"\"" : row["capacity"].ToString()) + ", ";
			if(lvStrFlotClass.LastIndexOf(",") == lvStrFlotClass.Length - 2)
			{
				lvStrFlotClass = lvStrFlotClass.Substring(0, lvStrFlotClass.Length - 2);
			}

			lvStrFlotClass += "}";

			lvResult += lvStrFlotClass + " \n ";
		}

		lvResult += "]; \n\n";

		return lvResult;
	}

    public static StopLocation GetNextStopSegment(int pCoordinate, int pDirection)
    {
        int lvStopSegIndex = -1;
        StopLocation lvCurrentSegment = null;
        StopLocation lvResSegment = null;

        if (mListStopLoc == null)
        {
            return null;
        }

        if (mListStopLoc.Count == 0)
        {
            return null;
        }

        lvCurrentSegment = StopLocation.GetCurrentStopSegment(pCoordinate, pDirection, out lvStopSegIndex);

        if (lvCurrentSegment != null)
        {
            if (pDirection > 0)
            {
                if (lvStopSegIndex < (mListStopLoc.Count - 1))
                {
                    lvResSegment = mListStopLoc[lvStopSegIndex + 1];
                }
            }
            else if (pDirection < 0)
            {
                if (lvStopSegIndex > 0)
                {
                    lvResSegment = mListStopLoc[lvStopSegIndex - 1];
                }
            }
        }
        else
        {
            if (pDirection > 0)
            {
                if (~lvStopSegIndex >= mListStopLoc.Count)
                {
                    lvResSegment = mListStopLoc[mListStopLoc.Count - 1];
                }
                else if (~lvStopSegIndex <= 0)
                {
                    lvResSegment = mListStopLoc[0];
                }
                else
                {
                    lvResSegment = mListStopLoc[~lvStopSegIndex];
                }
            }
            else if (pDirection < 0)
            {
                if (~lvStopSegIndex >= mListStopLoc.Count)
                {
                    lvResSegment = mListStopLoc[mListStopLoc.Count - 1];
                }
                else if (~lvStopSegIndex <= 1)
                {
                    lvResSegment = mListStopLoc[0];
                }
                else
                {
                    lvResSegment = mListStopLoc[~lvStopSegIndex - 1];
                }
            }
        }

        return lvResSegment;
    }

    public static StopLocation GetCurrentStopSegment(int pCoordinate, int pDirection, out int pIdxSegment)
    {
        int lvStopSegIndex = -1;
        StopLocation lvSubSegment = new StopLocation();
        StopLocation lvResSegment = null;

        if (mListStopLoc == null)
        {
            pIdxSegment = -1;
            return null;
        }

        if (mListStopLoc.Count == 0)
        {
            pIdxSegment = -1;
            return null;
        }

        if (pDirection > 0)
        {
            lvSubSegment.Start_coordinate = pCoordinate;
            lvSubSegment.End_coordinate = pCoordinate + 100;
        }
        else if (pDirection < 0)
        {
            lvSubSegment.Start_coordinate = pCoordinate - 100;
            lvSubSegment.End_coordinate = pCoordinate;
        }

        lvStopSegIndex = mListStopLoc.BinarySearch(lvSubSegment);

        pIdxSegment = lvStopSegIndex;
        if (lvStopSegIndex >= 0)
        {
            lvResSegment = mListStopLoc[lvStopSegIndex];
        }
        else
        {
            lvResSegment = null;
        }

        return lvResSegment;
    }

    public static void LoadList()
    {
        mListStopLoc = new List<StopLocation>();
        DataSet ds = null;
        int lvLastLocation = Int32.MinValue;
        StopLocation lvElement = null;

        ds = StopLocationDataAccess.GetAll("location asc");

        foreach (DataRow row in ds.Tables[0].Rows)
        {
            lvElement = new StopLocation();

            lvElement.Location = ((row["location"] == DBNull.Value) ? Int32.MinValue : (int)row["location"]);
            lvElement.Start_coordinate = ((row["start_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["start_coordinate"]);
            lvElement.End_coordinate = ((row["end_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["end_coordinate"]);
            lvElement.Capacity = ((row["capacity"] == DBNull.Value) ? Int16.MinValue : Convert.ToInt16(row["capacity"]));

            if (FIRST_STOP_SEGMENT == Int32.MinValue)
            {
                FIRST_STOP_SEGMENT = lvElement.Location;
            }

            lvLastLocation = lvElement.Location;

            mListStopLoc.Add(lvElement);
            lvElement = null;
        }

        DebugLog.Logar("LoadList.mListStopLoc.Count = " + mListStopLoc.Count);

        LAST_STOP_SEGMENT = lvLastLocation;
    }

    public static List<StopLocation> GetList()
    {
        return mListStopLoc;
    }

    public static string GetFlotTicks()
    {
        StringBuilder lvStrRes = new StringBuilder();
        int lvValue;
        int lvCount = 0; ;
        string lvStrValue = "";
        DataSet ds = null;

        ds = StopLocationDataAccess.GetAll();

        lvStrRes.Append("[");
        foreach (DataRow row in ds.Tables[0].Rows)
        {
            lvValue = ((row["location"] == DBNull.Value) ? Int32.MinValue : (int)row["location"]) / 100000;
            lvStrValue = lvValue.ToString();
            if (lvStrValue.Length == 1)
            {
                lvStrValue = "00" + lvStrValue;
            }
            else if (lvStrValue.Length == 2)
            {
                lvStrValue = "0" + lvStrValue;
            }

            lvStrValue = "[" + lvValue + ", \"KM " + lvStrValue + "\"]";

            if (lvCount > 0)
            {
                lvStrRes.Append(", ");
            }
            lvStrRes.Append(lvStrValue);
            lvCount++;
        }
        lvStrRes.Append("]");

//        DebugLog.Logar("Ticks = " + lvStrRes.ToString());

        return lvStrRes.ToString();
    }

	public virtual void Clear()
	{

		this.lvlocation = Int32.MinValue;
		this.lvstart_coordinate = Int32.MinValue;
		this.lvend_coordinate = Int32.MinValue;
		this.lvcapacity = Int16.MinValue;
	}

	public virtual bool Insert()
	{
		bool lvResult = false;
		int lvRowsAffect = 0;


		try
		{
			lvRowsAffect = StopLocationDataAccess.Insert(this.lvlocation, this.lvstart_coordinate, this.lvend_coordinate, this.lvcapacity);

			if (lvRowsAffect > 0)
			{
				lvResult = true;
			}
		}
		catch (MySqlException myex)
		{
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			throw nullex;
		}

		return lvResult;
	}

	public virtual bool Update()
	{
		bool lvResult = false;
		int lvRowsAffect = 0;


		try
		{
			lvRowsAffect = StopLocationDataAccess.Update(this.lvlocation, this.lvstart_coordinate, this.lvend_coordinate, this.lvcapacity);

			if (lvRowsAffect > 0)
			{
				lvResult = true;
			}
		}
		catch (MySqlException myex)
		{
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			throw nullex;
		}

		return lvResult;
	}

	public bool UpdateKey(int location)
	{
		bool lvResult = false;
		int lvRowsAffect = 0;

		try
		{
			lvRowsAffect = StopLocationDataAccess.UpdateKey(this.lvlocation, location);

			if (lvRowsAffect > 0)
			{
				lvResult = true;
			}
		}
		catch (MySqlException myex)
		{
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			throw nullex;
		}

		return lvResult;
	}

	public virtual bool Delete()
	{
		bool lvResult = false;
		int lvRowsAffect = 0;


		try
		{
			lvRowsAffect = StopLocationDataAccess.Delete(this.lvlocation);

			if (lvRowsAffect > 0)
			{
				lvResult = true;
			}
		}
		catch (MySqlException myex)
		{
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			throw nullex;
		}

		return lvResult;
	}

	public DataTable GetData()
	{
		DataTable dt = null;
		DataSet ds = null;

		ds = StopLocationDataAccess.GetData(this.lvlocation, this.lvstart_coordinate, this.lvend_coordinate, this.lvcapacity, "");

		dt = ds.Tables[0];

		return dt;
	}

	public int Location
	{
		get
		{
			return this.lvlocation;
		}
		set
		{
			this.lvlocation = value;
		}
	}

	public int Start_coordinate
	{
		get
		{
			return this.lvstart_coordinate;
		}
		set
		{
			this.lvstart_coordinate = value;
		}
	}

	public int End_coordinate
	{
		get
		{
			return this.lvend_coordinate;
		}
		set
		{
			this.lvend_coordinate = value;
		}
	}

	public Int16 Capacity
	{
		get
		{
			return this.lvcapacity;
		}
		set
		{
			this.lvcapacity = value;
		}
	}

    public static int FirstLocation
    {
        get
        {
            return (FIRST_STOP_SEGMENT / 100000);
        }
    }

    public static int LastLocation
    {
        get
        {
            return (LAST_STOP_SEGMENT / 100000);
        }
    }

    public override string ToString()
    {
        return "Stop Segment Location: " + lvlocation + ", Start: " + lvstart_coordinate + ", End: " + lvend_coordinate + ", Capacity: " + lvcapacity;
    }
}

