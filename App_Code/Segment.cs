using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text;
/// <summary>
/// Criado por Eggo Pinheiro em 13/04/2015 18:56:55
/// <summary>

public class Segment : IEquatable<Segment>, IComparable<Segment>
{
    protected static List<Segment> mListSegment = null;
    protected static List<Segment> mListSwitch = null;
//    private ComparerSegmentDataType mSegmentComparer = null;
	protected int lvlocation;
	protected string lvsegment;
	protected int lvstart_coordinate;
	protected int lvend_coordinate;

	public Segment()
	{
		Clear();
//        mSegmentComparer = new ComparerSegmentDataType();
    }

	public Segment(int location, string segment) : this()
	{
		this.lvlocation = location;
		this.lvsegment = segment;
		Load();
    }

	public Segment(int location, string segment, int start_coordinate, int end_coordinate) : this()
	{
		this.lvlocation = location;
		this.lvsegment = segment;
		this.lvstart_coordinate = start_coordinate;
		this.lvend_coordinate = end_coordinate;
	}

    public int CompareTo(Segment pOther)
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

    public static bool operator ==(Segment obj1, Segment obj2)
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

    public static bool operator !=(Segment obj1, Segment obj2)
    {
        return !(obj1 == obj2);
    }

    public bool Equals(Segment other)
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

        if ((this.lvlocation == other.lvlocation) && (this.lvsegment.Equals(other.lvsegment)) && (this.lvstart_coordinate == other.lvstart_coordinate) && (this.lvend_coordinate == other.lvend_coordinate))
        {
            lvRes = true;
        }

        return lvRes;
    }

    public override bool Equals(object obj)
    {
        bool lvRes = false;

        if (obj is Segment)
        {
            lvRes = Equals(obj as Segment);
        }

        return lvRes;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int lvHashCode = 0;

            if (lvsegment != null)
            {
                lvHashCode = lvsegment.GetHashCode();
                lvHashCode = (lvHashCode * 397) ^ lvlocation.GetHashCode();
            }

            return lvHashCode;
        }
    }

	public virtual bool Load()
	{
		bool lvResult = false;


		DataSet ds = SegmentDataAccess.GetDataByKey(this.lvlocation, this.lvsegment, "");

		foreach (DataRow row in ds.Tables[0].Rows)
		{
			this.lvlocation = ((row["location"] == DBNull.Value) ? Int32.MinValue : (int)row["location"]);
			this.lvsegment = ((row["segment"] == DBNull.Value) ? "" : row["segment"].ToString());
			this.lvstart_coordinate = ((row["start_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["start_coordinate"]);
			this.lvend_coordinate = ((row["end_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["end_coordinate"]);

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

		ds = SegmentDataAccess.GetData(this.lvlocation, this.lvsegment, this.lvstart_coordinate, this.lvend_coordinate, "");

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

		ds = SegmentDataAccess.GetData(this.lvlocation, this.lvsegment, this.lvstart_coordinate, this.lvend_coordinate, "");

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
			lvStrFlotClass += "Segment: \"" + ((row["segment"] == DBNull.Value) ? "" : row["segment"].ToString()) + "\", ";
			lvStrFlotClass += "Start_coordinate: " + ((row["start_coordinate"] == DBNull.Value) ? "\"\"" : row["start_coordinate"].ToString()) + ", ";
			lvStrFlotClass += "End_coordinate: " + ((row["end_coordinate"] == DBNull.Value) ? "\"\"" : row["end_coordinate"].ToString()) + ", ";
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

    public static Segment GetNextSegment(int pCoordinate, int pDirection)
    {
        int lvSegIndex = -1;
        Segment lvCurrentSegment = null;
        Segment lvResSegment = null;

        if (mListSegment == null)
        {
            return null;
        }

        lvCurrentSegment = Segment.GetCurrentSegment(pCoordinate, pDirection, 0, out lvSegIndex);

        if (lvCurrentSegment != null)
        {
            if (pDirection > 0)
            {
                if (lvSegIndex == (mListSegment.Count - 1))
                {
                    lvResSegment = mListSegment[lvSegIndex];
                }
                else
                {
                    lvResSegment = mListSegment[lvSegIndex + 1];
                }
            }
            else if (pDirection < 0)
            {
                if (lvSegIndex <= 1)
                {
                    lvResSegment = mListSegment[0];
                }
                else
                {
                    lvResSegment = mListSegment[lvSegIndex - 1];
                }
            }
        }
        else
        {
            if (pDirection > 0)
            {
                if (~lvSegIndex == mListSegment.Count)
                {
                    lvResSegment = mListSegment[mListSegment.Count - 1];
                }
                else if (~lvSegIndex == 0)
                {
                    lvResSegment = mListSegment[0];
                }
                else
                {
                    lvResSegment = mListSegment[~lvSegIndex];
                }
            }
            else if (pDirection < 0)
            {
                if (~lvSegIndex == mListSegment.Count)
                {
                    lvResSegment = mListSegment[mListSegment.Count - 1];
                }
                else if (~lvSegIndex <= 1)
                {
                    lvResSegment = mListSegment[0];
                }
                else
                {
                    lvResSegment = mListSegment[~lvSegIndex - 1];
                }
            }
        }

        return lvResSegment;
    }

    public static Segment GetCurrentSegment(int pCoordinate, int pDirection, int pLine, out int pSegIndex)
    {
        Segment lvSubSegment = new Segment();
        Segment lvResSegment = null;
        Segment lvOrigSegment = null;

        if (mListSegment == null)
        {
            pSegIndex = -1;
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

        pSegIndex = mListSegment.BinarySearch(lvSubSegment);

        if (pSegIndex >= 0)
        {
            lvResSegment = mListSegment[pSegIndex];
            if (pLine != 0)
            {
                lvOrigSegment = lvResSegment;

                if (lvResSegment.SegmentValue.StartsWith("CDV_1") && pLine != 1)
                {
                    lvResSegment = mListSegment[++pSegIndex];
                }

                if (lvResSegment.SegmentValue.StartsWith("CDV_2") && pLine != 2)
                {
                    lvResSegment = mListSegment[--pSegIndex];
                }

                if (lvResSegment.SegmentValue.StartsWith("PARKI") && pLine != 1)
                {
                    lvResSegment = mListSegment[--pSegIndex];
                }

                if (lvResSegment.SegmentValue.StartsWith("PARKII") && pLine != 2)
                {
                    lvResSegment = mListSegment[--pSegIndex];
                }

                if (pCoordinate < lvResSegment.Start_coordinate || pCoordinate > lvResSegment.End_coordinate)
                {
                    lvResSegment = lvOrigSegment;
                }
            }
        }
        else
        {
            lvResSegment = null;
        }

        return lvResSegment;
    }

    public static Segment GetNextSwitchSegment(int pCoordinate, int pDirection)
    {
        int lvSegIndex = -1;
        Segment lvCurrentSegment = null;
        Segment lvResSegment = null;

        if (mListSwitch == null)
        {
            return null;
        }

        lvCurrentSegment = Segment.GetCurrentSwitchSegment(pCoordinate, pDirection, out lvSegIndex);

        if (lvCurrentSegment != null)
        {
            if (pDirection > 0)
            {
                if (lvSegIndex == (mListSwitch.Count - 1))
                {
                    lvResSegment = mListSwitch[lvSegIndex];
                }
                else
                {
                    lvResSegment = mListSwitch[lvSegIndex + 1];
                }
            }
            else if (pDirection < 0)
            {
                if (lvSegIndex <= 1)
                {
                    lvResSegment = mListSwitch[0];
                }
                else
                {
                    lvResSegment = mListSwitch[lvSegIndex - 1];
                }
            }
        }
        else
        {
            if (pDirection > 0)
            {
                if (~lvSegIndex == mListSwitch.Count)
                {
                    lvResSegment = mListSwitch[mListSwitch.Count - 1];
                }
                else if (~lvSegIndex == 0)
                {
                    lvResSegment = mListSwitch[0];
                }
                else
                {
                    lvResSegment = mListSwitch[~lvSegIndex];
                }
            }
            else if (pDirection < 0)
            {
                if (~lvSegIndex == mListSwitch.Count)
                {
                    lvResSegment = mListSwitch[mListSwitch.Count - 1];
                }
                else if (~lvSegIndex <= 1)
                {
                    lvResSegment = mListSwitch[0];
                }
                else
                {
                    lvResSegment = mListSwitch[~lvSegIndex - 1];
                }
            }
        }

        return lvResSegment;
    }

    public static Segment GetCurrentSwitchSegment(int pCoordinate, int pDirection, out int pSegIndex)
    {
        Segment lvSubSegment = new Segment();
        Segment lvResSegment = null;

        if (mListSegment == null)
        {
            pSegIndex = -1;
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

        pSegIndex = mListSwitch.BinarySearch(lvSubSegment);

        if (pSegIndex >= 0)
        {
            lvResSegment = mListSwitch[pSegIndex];
        }
        else
        {
            lvResSegment = null;
        }

        return lvResSegment;
    }

    public static void LoadList()
    {
        mListSegment = new List<Segment>();
        mListSwitch = new List<Segment>();

        DataSet ds = null;
        Segment lvElement = null;

        ds = SegmentDataAccess.GetAll("location, start_coordinate, end_coordinate");

        foreach (DataRow row in ds.Tables[0].Rows)
        {
            lvElement = new Segment();

            lvElement.Location = ((row["location"] == DBNull.Value) ? Int32.MinValue : (int)row["location"]);
            lvElement.SegmentValue = ((row["segment"] == DBNull.Value) ? "" : row["segment"].ToString());
            lvElement.Start_coordinate = ((row["start_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["start_coordinate"]);
            lvElement.End_coordinate = ((row["end_coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["end_coordinate"]);

            mListSegment.Add(lvElement);

            if (lvElement.SegmentValue.StartsWith("CV03") || lvElement.SegmentValue.StartsWith("SW") || lvElement.SegmentValue.Equals("WT"))
            {
                mListSwitch.Add(lvElement);
            }
            lvElement = null;
        }

        DebugLog.Logar("LoadList.mListSegment.Count = " + mListSegment.Count);
        DebugLog.Logar("LoadList.mListSwitch.Count = " + mListSwitch.Count);
    }

	public static List<Segment> GetList()
	{
        return mListSegment;
	}

    public static List<Segment> GetListSwitch()
    {
        return mListSwitch;
    }

	public virtual void Clear()
	{

		this.lvlocation = Int32.MinValue;
		this.lvsegment = "";
		this.lvstart_coordinate = Int32.MinValue;
		this.lvend_coordinate = Int32.MinValue;
	}

	public virtual bool Insert()
	{
		bool lvResult = false;
		int lvRowsAffect = 0;


		try
		{
			lvRowsAffect = SegmentDataAccess.Insert(this.lvlocation, this.lvsegment, this.lvstart_coordinate, this.lvend_coordinate);

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
			lvRowsAffect = SegmentDataAccess.Update(this.lvlocation, this.lvsegment, this.lvstart_coordinate, this.lvend_coordinate);

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

	public bool UpdateKey(int location, string segment)
	{
		bool lvResult = false;
		int lvRowsAffect = 0;

		try
		{
			lvRowsAffect = SegmentDataAccess.UpdateKey(this.lvlocation, this.lvsegment, location, segment);

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
			lvRowsAffect = SegmentDataAccess.Delete(this.lvlocation, this.lvsegment);

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

		ds = SegmentDataAccess.GetData(this.lvlocation, this.lvsegment, this.lvstart_coordinate, this.lvend_coordinate, "");

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

	public string SegmentValue
	{
		get
		{
			return this.lvsegment;
		}
		set
		{
			this.lvsegment = value;
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


}

