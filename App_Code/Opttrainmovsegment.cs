using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text;
/// <summary>
/// Criado por Eggo Pinheiro em 24/02/2015 23:12:17
/// <summary>

public class Opttrainmovsegment
{

	protected Train lvtrain_id;
	protected DateTime lvhorario;
	protected int lvcoordinate;
	protected Int16 lvlocation;
	protected Int16 lvtrack;
	protected DateTime lvhist;

	public Opttrainmovsegment()
	{
		this.lvtrain_id = new Train();

		Clear();
	}

	public Opttrainmovsegment(double train_id, DateTime horario, Int16 location)
	{
		this.lvtrain_id = new Train();

		Clear();

		this.lvtrain_id.Train_id = train_id;
		this.lvhorario = horario;
		this.lvlocation = location;
		Load();
	}

	public Opttrainmovsegment(double train_id, DateTime horario, int coordinate, Int16 location, Int16 track, DateTime hist)
	{
		this.lvtrain_id = new Train();

		this.lvtrain_id.Train_id = train_id;
		this.lvhorario = horario;
		this.lvcoordinate = coordinate;
		this.lvlocation = location;
		this.lvtrack = track;
		this.lvhist = hist;
	}

	public virtual bool Load()
	{
		bool lvResult = false;


		DataSet ds = OpttrainmovsegmentDataAccess.GetDataByKey(this.lvtrain_id.Train_id, this.lvhorario, this.lvlocation, false, "");

		foreach (DataRow row in ds.Tables[0].Rows)
		{
			this.lvtrain_id.Train_id = ((row["train_id"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["train_id"]);
			this.lvhorario = ((row["horario"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["horario"].ToString()));
			this.lvcoordinate = ((row["coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["coordinate"]);
			this.lvlocation = ((row["location"] == DBNull.Value) ? Int16.MinValue : (Int16)row["location"]);
			this.lvtrack = ((row["track"] == DBNull.Value) ? Int16.MinValue : (Int16)row["track"]);
			this.lvhist = ((row["hist"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["hist"].ToString()));

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

		ds = OpttrainmovsegmentDataAccess.GetData(this.lvtrain_id.Train_id, this.lvhorario, this.lvcoordinate, this.lvlocation, this.lvtrack, this.lvhist, true, "train_id");

		if (String.IsNullOrEmpty(pStrSymbol))
		{
			if(isDashed)
			{
				lvResult.Append("{\"color\": \"" + pStrColor + "\", \"label: \"" + pStrLabel + "\", \"ident: \"" + pStrIdent + "\", \"points\": {\"show\": true, \"radius\": 1, \"fill\": false}, \"lines\": {\"show\": false}, \"dashes\": {\"show\": true, \"lineWidth\": 3, \"dashLength\": 6}, \"hoverable\": true, \"clickable\": true, \"data\": [");
			}
			else
			{
				lvResult.Append("{\"color\": \"" + pStrColor + "\", \"label: \"" + pStrLabel + "\", \"ident: \"" + pStrIdent + "\", \"points\": {\"show\": false, \"radius\": 2}, \"lines\": {\"show\": true, \"lineWidth\": 3}, \"data\": [");
			}
		}
		else
		{
				lvResult.Append("{\"color\": \"" + pStrColor + "\", \"label: \"" + pStrLabel + "\", \"ident: \"" + pStrIdent + "\", \"points\": {\"show\": true, \"radius\": 2, \"symbol\": \"" + pStrSymbol + "\"}, \"data\": [");
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

            if (!lvHasElement)
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

		ds = OpttrainmovsegmentDataAccess.GetData(this.lvtrain_id.Train_id, this.lvhorario, this.lvcoordinate, this.lvlocation, this.lvtrack, this.lvhist, false, "");

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

			lvStrFlotClass += "Train_id: " + ((row["train_id"] == DBNull.Value) ? "\"\"" : row["train_id"].ToString().Replace(",", ".")) + ", ";
			lvStrFlotClass += "Horario: \"" + ((row["horario"] == DBNull.Value) ? "" : DateTime.Parse(row["horario"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
			lvStrFlotClass += "Coordinate: " + ((row["coordinate"] == DBNull.Value) ? "\"\"" : row["coordinate"].ToString()) + ", ";
			lvStrFlotClass += "Location: " + ((row["location"] == DBNull.Value) ? "\"\"" : row["location"].ToString()) + ", ";
			lvStrFlotClass += "Track: " + ((row["track"] == DBNull.Value) ? "\"\"" : row["track"].ToString()) + ", ";
			lvStrFlotClass += "Hist: \"" + ((row["hist"] == DBNull.Value) ? "" : DateTime.Parse(row["hist"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
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

	public List<Opttrainmovsegment> GetList()
	{
		List<Opttrainmovsegment> lvResult = new List<Opttrainmovsegment>();
		DataSet ds = null;
		Opttrainmovsegment lvElement = null;

		ds = OpttrainmovsegmentDataAccess.GetData(this.lvtrain_id.Train_id, this.lvhorario, this.lvcoordinate, this.lvlocation, this.lvtrack, this.lvhist, false, "");

		foreach (DataRow row in ds.Tables[0].Rows)
		{
			lvElement = new Opttrainmovsegment();

			lvElement.Train_id.Train_id = ((row["train_id"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["train_id"]);
			lvElement.Horario = ((row["horario"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["horario"].ToString()));
			lvElement.Coordinate = ((row["coordinate"] == DBNull.Value) ? Int32.MinValue : (int)row["coordinate"]);
			lvElement.Location = ((row["location"] == DBNull.Value) ? Int16.MinValue : (Int16)row["location"]);
			lvElement.Track = ((row["track"] == DBNull.Value) ? Int16.MinValue : (Int16)row["track"]);
			lvElement.Hist = ((row["hist"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["hist"].ToString()));

			lvResult.Add(lvElement);
			lvElement = null;
		}

		return lvResult;
	}

	public virtual void Clear()
	{

		this.lvtrain_id.Clear();
		this.lvhorario = DateTime.MinValue;
		this.lvcoordinate = Int32.MinValue;
		this.lvlocation = Int16.MinValue;
		this.lvtrack = Int16.MinValue;
		this.lvhist = DateTime.MinValue;
	}

	public virtual bool Insert()
	{
		bool lvResult = false;
		int lvRowsAffect = 0;


		try
		{
			lvRowsAffect = OpttrainmovsegmentDataAccess.Insert(this.lvtrain_id.Train_id, this.lvhorario, this.lvcoordinate, this.lvlocation, this.lvtrack, this.lvhist);

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
			lvRowsAffect = OpttrainmovsegmentDataAccess.Update(this.lvtrain_id.Train_id, this.lvhorario, this.lvcoordinate, this.lvlocation, this.lvtrack, this.lvhist);

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

	public bool UpdateKey(double train_id, DateTime horario, Int16 location)
	{
		bool lvResult = false;
		int lvRowsAffect = 0;

		try
		{
			lvRowsAffect = OpttrainmovsegmentDataAccess.UpdateKey(this.lvtrain_id.Train_id, this.lvhorario, this.lvlocation, train_id, horario, location);

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
			lvRowsAffect = OpttrainmovsegmentDataAccess.Delete(this.lvtrain_id.Train_id, this.lvhorario, this.lvlocation);

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

		ds = OpttrainmovsegmentDataAccess.GetData(this.lvtrain_id.Train_id, this.lvhorario, this.lvcoordinate, this.lvlocation, this.lvtrack, this.lvhist, true, "train_id");

		dt = ds.Tables[0];

		return dt;
	}

	public Train Train_id
	{
		get
		{
			return this.lvtrain_id;
		}
		set
		{
			this.lvtrain_id = value;
		}
	}

	public DateTime Horario
	{
		get
		{
			return this.lvhorario;
		}
		set
		{
			this.lvhorario = value;
		}
	}

	public int Coordinate
	{
		get
		{
			return this.lvcoordinate;
		}
		set
		{
			this.lvcoordinate = value;
		}
	}

	public Int16 Location
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

	public Int16 Track
	{
		get
		{
			return this.lvtrack;
		}
		set
		{
			this.lvtrack = value;
		}
	}

	public DateTime Hist
	{
		get
		{
			return this.lvhist;
		}
		set
		{
			this.lvhist = value;
		}
	}


}

