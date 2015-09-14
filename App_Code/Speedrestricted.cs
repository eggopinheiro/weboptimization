using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text;
/// <summary>
/// Criado por Eggo Pinheiro em 12/03/2015 15:53:24
/// <summary>

public class Speedrestricted
{

	protected double lvsra_id;
	protected int lvstart_pos;
	protected int lvend_pos;
	protected DateTime lvstart_time;
	protected DateTime lvend_time;
	protected Int16 lvflag;
	protected string lvdirection;
	protected int lvforward_speed_limit;
	protected int lvbackward_speed_limit;
	protected Int16 lvover_switch;
	protected string lvstatus;
	protected string lvreason;
	protected string lvstart_pos_desc;
	protected string lvend_pos_desc;
	protected DateTime lvhist_date;
	protected Int16 lvprogressive;
	protected string lvinfo;
	protected string lvstart_track;
	protected string lvend_track;

	public Speedrestricted()
	{
		Clear();
	}

	public Speedrestricted(double sra_id)
	{
		Clear();

		this.lvsra_id = sra_id;
		Load();
	}

	public Speedrestricted(double sra_id, int start_pos, int end_pos, DateTime start_time, DateTime end_time, Int16 flag, string direction, int forward_speed_limit, int backward_speed_limit, Int16 over_switch, string status, string reason, string start_pos_desc, string end_pos_desc, DateTime hist_date, Int16 progressive, string info, string start_track, string end_track, Int16 distrito)
	{
		this.lvsra_id = sra_id;
		this.lvstart_pos = start_pos;
		this.lvend_pos = end_pos;
		this.lvstart_time = start_time;
		this.lvend_time = end_time;
		this.lvflag = flag;
		this.lvdirection = direction;
		this.lvforward_speed_limit = forward_speed_limit;
		this.lvbackward_speed_limit = backward_speed_limit;
		this.lvover_switch = over_switch;
		this.lvstatus = status;
		this.lvreason = reason;
		this.lvstart_pos_desc = start_pos_desc;
		this.lvend_pos_desc = end_pos_desc;
		this.lvhist_date = hist_date;
		this.lvprogressive = progressive;
		this.lvinfo = info;
		this.lvstart_track = start_track;
		this.lvend_track = end_track;
	}

	public virtual bool Load()
	{
		bool lvResult = false;

		DataSet ds = SpeedrestrictedDataAccess.GetDataByKey(this.lvsra_id, "");

		foreach (DataRow row in ds.Tables[0].Rows)
		{
			this.lvsra_id = ((row["sra_id"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["sra_id"]);
			this.lvstart_pos = ((row["start_pos"] == DBNull.Value) ? Int32.MinValue : (int)row["start_pos"]);
			this.lvend_pos = ((row["end_pos"] == DBNull.Value) ? Int32.MinValue : (int)row["end_pos"]);
			this.lvstart_time = ((row["start_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["start_time"].ToString()));
			this.lvend_time = ((row["end_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["end_time"].ToString()));
			this.lvflag = ((row["flag"] == DBNull.Value) ? Int16.MinValue : (Int16)row["flag"]);
			this.lvdirection = ((row["direction"] == DBNull.Value) ? "" : row["direction"].ToString());
			this.lvforward_speed_limit = ((row["forward_speed_limit"] == DBNull.Value) ? Int32.MinValue : (int)row["forward_speed_limit"]);
			this.lvbackward_speed_limit = ((row["backward_speed_limit"] == DBNull.Value) ? Int32.MinValue : (int)row["backward_speed_limit"]);
			this.lvover_switch = ((row["over_switch"] == DBNull.Value) ? Int16.MinValue : (Int16)row["over_switch"]);
			this.lvstatus = ((row["status"] == DBNull.Value) ? "" : row["status"].ToString());
			this.lvreason = ((row["reason"] == DBNull.Value) ? "" : row["reason"].ToString());
			this.lvstart_pos_desc = ((row["start_pos_desc"] == DBNull.Value) ? "" : row["start_pos_desc"].ToString());
			this.lvend_pos_desc = ((row["end_pos_desc"] == DBNull.Value) ? "" : row["end_pos_desc"].ToString());
			this.lvhist_date = ((row["hist_date"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["hist_date"].ToString()));
			this.lvprogressive = ((row["progressive"] == DBNull.Value) ? Int16.MinValue : (Int16)row["progressive"]);
			this.lvinfo = ((row["info"] == DBNull.Value) ? "" : row["info"].ToString());
			this.lvstart_track = ((row["start_track"] == DBNull.Value) ? "" : row["start_track"].ToString());
			this.lvend_track = ((row["end_track"] == DBNull.Value) ? "" : row["end_track"].ToString());

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

		ds = SpeedrestrictedDataAccess.GetData(this.lvsra_id, this.lvstart_pos, this.lvend_pos, this.lvstart_time, this.lvend_time, this.lvflag, this.lvdirection, this.lvforward_speed_limit, this.lvbackward_speed_limit, this.lvover_switch, this.lvstatus, this.lvreason, this.lvstart_pos_desc, this.lvend_pos_desc, this.lvhist_date, this.lvprogressive, this.lvinfo, this.lvstart_track, this.lvend_track, "");

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

			if(lvHasElement)
			{
				lvResult.Append(", [" + lvXValues + ", " + lvYValues + "]");
			}
			else
			{
				lvResult.Append("[" + lvXValues + ", " + lvYValues + "]");
				lvHasElement = true;
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

		ds = SpeedrestrictedDataAccess.GetData(this.lvsra_id, this.lvstart_pos, this.lvend_pos, this.lvstart_time, this.lvend_time, this.lvflag, this.lvdirection, this.lvforward_speed_limit, this.lvbackward_speed_limit, this.lvover_switch, this.lvstatus, this.lvreason, this.lvstart_pos_desc, this.lvend_pos_desc, this.lvhist_date, this.lvprogressive, this.lvinfo, this.lvstart_track, this.lvend_track, "");

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

			lvStrFlotClass += "Sra_id: " + ((row["sra_id"] == DBNull.Value) ? "\"\"" : row["sra_id"].ToString().Replace(",", ".")) + ", ";
			lvStrFlotClass += "Start_pos: " + ((row["start_pos"] == DBNull.Value) ? "\"\"" : row["start_pos"].ToString()) + ", ";
			lvStrFlotClass += "End_pos: " + ((row["end_pos"] == DBNull.Value) ? "\"\"" : row["end_pos"].ToString()) + ", ";
			lvStrFlotClass += "Start_time: \"" + ((row["start_time"] == DBNull.Value) ? "" : DateTime.Parse(row["start_time"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
			lvStrFlotClass += "End_time: \"" + ((row["end_time"] == DBNull.Value) ? "" : DateTime.Parse(row["end_time"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
			lvStrFlotClass += "Flag: " + ((row["flag"] == DBNull.Value) ? "\"\"" : row["flag"].ToString()) + ", ";
			lvStrFlotClass += "Direction: \"" + ((row["direction"] == DBNull.Value) ? "" : row["direction"].ToString()) + "\", ";
			lvStrFlotClass += "Forward_speed_limit: " + ((row["forward_speed_limit"] == DBNull.Value) ? "\"\"" : row["forward_speed_limit"].ToString()) + ", ";
			lvStrFlotClass += "Backward_speed_limit: " + ((row["backward_speed_limit"] == DBNull.Value) ? "\"\"" : row["backward_speed_limit"].ToString()) + ", ";
			lvStrFlotClass += "Over_switch: " + ((row["over_switch"] == DBNull.Value) ? "\"\"" : row["over_switch"].ToString()) + ", ";
			lvStrFlotClass += "Status: \"" + ((row["status"] == DBNull.Value) ? "" : row["status"].ToString()) + "\", ";
			lvStrFlotClass += "Reason: \"" + ((row["reason"] == DBNull.Value) ? "" : row["reason"].ToString()) + "\", ";
			lvStrFlotClass += "Start_pos_desc: \"" + ((row["start_pos_desc"] == DBNull.Value) ? "" : row["start_pos_desc"].ToString()) + "\", ";
			lvStrFlotClass += "End_pos_desc: \"" + ((row["end_pos_desc"] == DBNull.Value) ? "" : row["end_pos_desc"].ToString()) + "\", ";
			lvStrFlotClass += "Hist_date: \"" + ((row["hist_date"] == DBNull.Value) ? "" : DateTime.Parse(row["hist_date"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
			lvStrFlotClass += "Progressive: " + ((row["progressive"] == DBNull.Value) ? "\"\"" : row["progressive"].ToString()) + ", ";
			lvStrFlotClass += "Info: \"" + ((row["info"] == DBNull.Value) ? "" : row["info"].ToString()) + "\", ";
			lvStrFlotClass += "Start_track: \"" + ((row["start_track"] == DBNull.Value) ? "" : row["start_track"].ToString()) + "\", ";
			lvStrFlotClass += "End_track: \"" + ((row["end_track"] == DBNull.Value) ? "" : row["end_track"].ToString()) + "\", ";
			lvStrFlotClass += "Distrito: " + ((row["distrito"] == DBNull.Value) ? "\"\"" : row["distrito"].ToString()) + ", ";
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

	public List<Speedrestricted> GetList()
	{
		List<Speedrestricted> lvResult = new List<Speedrestricted>();
		DataSet ds = null;
		Speedrestricted lvElement = null;

		ds = SpeedrestrictedDataAccess.GetData(this.lvsra_id, this.lvstart_pos, this.lvend_pos, this.lvstart_time, this.lvend_time, this.lvflag, this.lvdirection, this.lvforward_speed_limit, this.lvbackward_speed_limit, this.lvover_switch, this.lvstatus, this.lvreason, this.lvstart_pos_desc, this.lvend_pos_desc, this.lvhist_date, this.lvprogressive, this.lvinfo, this.lvstart_track, this.lvend_track, "");

		foreach (DataRow row in ds.Tables[0].Rows)
		{
			lvElement = new Speedrestricted();

			lvElement.Sra_id = ((row["sra_id"] == DBNull.Value) ? ConnectionManager.DOUBLE_REF_VALUE : (double)row["sra_id"]);
			lvElement.Start_pos = ((row["start_pos"] == DBNull.Value) ? Int32.MinValue : (int)row["start_pos"]);
			lvElement.End_pos = ((row["end_pos"] == DBNull.Value) ? Int32.MinValue : (int)row["end_pos"]);
			lvElement.Start_time = ((row["start_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["start_time"].ToString()));
			lvElement.End_time = ((row["end_time"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["end_time"].ToString()));
			lvElement.Flag = ((row["flag"] == DBNull.Value) ? Int16.MinValue : (Int16)row["flag"]);
			lvElement.Direction = ((row["direction"] == DBNull.Value) ? "" : row["direction"].ToString());
			lvElement.Forward_speed_limit = ((row["forward_speed_limit"] == DBNull.Value) ? Int32.MinValue : (int)row["forward_speed_limit"]);
			lvElement.Backward_speed_limit = ((row["backward_speed_limit"] == DBNull.Value) ? Int32.MinValue : (int)row["backward_speed_limit"]);
			lvElement.Over_switch = ((row["over_switch"] == DBNull.Value) ? Int16.MinValue : (Int16)row["over_switch"]);
			lvElement.Status = ((row["status"] == DBNull.Value) ? "" : row["status"].ToString());
			lvElement.Reason = ((row["reason"] == DBNull.Value) ? "" : row["reason"].ToString());
			lvElement.Start_pos_desc = ((row["start_pos_desc"] == DBNull.Value) ? "" : row["start_pos_desc"].ToString());
			lvElement.End_pos_desc = ((row["end_pos_desc"] == DBNull.Value) ? "" : row["end_pos_desc"].ToString());
			lvElement.Hist_date = ((row["hist_date"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["hist_date"].ToString()));
			lvElement.Progressive = ((row["progressive"] == DBNull.Value) ? Int16.MinValue : (Int16)row["progressive"]);
			lvElement.Info = ((row["info"] == DBNull.Value) ? "" : row["info"].ToString());
			lvElement.Start_track = ((row["start_track"] == DBNull.Value) ? "" : row["start_track"].ToString());
			lvElement.End_track = ((row["end_track"] == DBNull.Value) ? "" : row["end_track"].ToString());

			lvResult.Add(lvElement);
			lvElement = null;
		}

		return lvResult;
	}

	public virtual void Clear()
	{

		this.lvsra_id = ConnectionManager.DOUBLE_REF_VALUE;
		this.lvstart_pos = Int32.MinValue;
		this.lvend_pos = Int32.MinValue;
		this.lvstart_time = DateTime.MinValue;
		this.lvend_time = DateTime.MinValue;
		this.lvflag = Int16.MinValue;
		this.lvdirection = "";
		this.lvforward_speed_limit = Int32.MinValue;
		this.lvbackward_speed_limit = Int32.MinValue;
		this.lvover_switch = Int16.MinValue;
		this.lvstatus = "";
		this.lvreason = "";
		this.lvstart_pos_desc = "";
		this.lvend_pos_desc = "";
		this.lvhist_date = DateTime.MinValue;
		this.lvprogressive = Int16.MinValue;
		this.lvinfo = "";
		this.lvstart_track = "";
		this.lvend_track = "";
	}

	public virtual bool Insert()
	{
		bool lvResult = false;
		int lvRowsAffect = 0;


		try
		{
			lvRowsAffect = SpeedrestrictedDataAccess.Insert(this.lvsra_id, this.lvstart_pos, this.lvend_pos, this.lvstart_time, this.lvend_time, this.lvflag, this.lvdirection, this.lvforward_speed_limit, this.lvbackward_speed_limit, this.lvover_switch, this.lvstatus, this.lvreason, this.lvstart_pos_desc, this.lvend_pos_desc, this.lvhist_date, this.lvprogressive, this.lvinfo, this.lvstart_track, this.lvend_track);

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
			lvRowsAffect = SpeedrestrictedDataAccess.Update(this.lvsra_id, this.lvstart_pos, this.lvend_pos, this.lvstart_time, this.lvend_time, this.lvflag, this.lvdirection, this.lvforward_speed_limit, this.lvbackward_speed_limit, this.lvover_switch, this.lvstatus, this.lvreason, this.lvstart_pos_desc, this.lvend_pos_desc, this.lvhist_date, this.lvprogressive, this.lvinfo, this.lvstart_track, this.lvend_track);

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

	public bool UpdateKey(double sra_id)
	{
		bool lvResult = false;
		int lvRowsAffect = 0;

		try
		{
			lvRowsAffect = SpeedrestrictedDataAccess.UpdateKey(this.lvsra_id, sra_id);

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
			lvRowsAffect = SpeedrestrictedDataAccess.Delete(this.lvsra_id);

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

		ds = SpeedrestrictedDataAccess.GetData(this.lvsra_id, this.lvstart_pos, this.lvend_pos, this.lvstart_time, this.lvend_time, this.lvflag, this.lvdirection, this.lvforward_speed_limit, this.lvbackward_speed_limit, this.lvover_switch, this.lvstatus, this.lvreason, this.lvstart_pos_desc, this.lvend_pos_desc, this.lvhist_date, this.lvprogressive, this.lvinfo, this.lvstart_track, this.lvend_track, "");

		dt = ds.Tables[0];

		return dt;
	}

	public double Sra_id
	{
		get
		{
			return this.lvsra_id;
		}
		set
		{
			this.lvsra_id = value;
		}
	}

	public int Start_pos
	{
		get
		{
			return this.lvstart_pos;
		}
		set
		{
			this.lvstart_pos = value;
		}
	}

	public int End_pos
	{
		get
		{
			return this.lvend_pos;
		}
		set
		{
			this.lvend_pos = value;
		}
	}

	public DateTime Start_time
	{
		get
		{
			return this.lvstart_time;
		}
		set
		{
			this.lvstart_time = value;
		}
	}

	public DateTime End_time
	{
		get
		{
			return this.lvend_time;
		}
		set
		{
			this.lvend_time = value;
		}
	}

	public Int16 Flag
	{
		get
		{
			return this.lvflag;
		}
		set
		{
			this.lvflag = value;
		}
	}

	public string Direction
	{
		get
		{
			return this.lvdirection;
		}
		set
		{
			this.lvdirection = value;
		}
	}

	public int Forward_speed_limit
	{
		get
		{
			return this.lvforward_speed_limit;
		}
		set
		{
			this.lvforward_speed_limit = value;
		}
	}

	public int Backward_speed_limit
	{
		get
		{
			return this.lvbackward_speed_limit;
		}
		set
		{
			this.lvbackward_speed_limit = value;
		}
	}

	public Int16 Over_switch
	{
		get
		{
			return this.lvover_switch;
		}
		set
		{
			this.lvover_switch = value;
		}
	}

	public string Status
	{
		get
		{
			return this.lvstatus;
		}
		set
		{
			this.lvstatus = value;
		}
	}

	public string Reason
	{
		get
		{
			return this.lvreason;
		}
		set
		{
			this.lvreason = value;
		}
	}

	public string Start_pos_desc
	{
		get
		{
			return this.lvstart_pos_desc;
		}
		set
		{
			this.lvstart_pos_desc = value;
		}
	}

	public string End_pos_desc
	{
		get
		{
			return this.lvend_pos_desc;
		}
		set
		{
			this.lvend_pos_desc = value;
		}
	}

	public DateTime Hist_date
	{
		get
		{
			return this.lvhist_date;
		}
		set
		{
			this.lvhist_date = value;
		}
	}

	public Int16 Progressive
	{
		get
		{
			return this.lvprogressive;
		}
		set
		{
			this.lvprogressive = value;
		}
	}

	public string Info
	{
		get
		{
			return this.lvinfo;
		}
		set
		{
			this.lvinfo = value;
		}
	}

	public string Start_track
	{
		get
		{
			return this.lvstart_track;
		}
		set
		{
			this.lvstart_track = value;
		}
	}

	public string End_track
	{
		get
		{
			return this.lvend_track;
		}
		set
		{
			this.lvend_track = value;
		}
	}
}

