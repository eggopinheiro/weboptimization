using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text;
/// <summary>
/// Criado por Eggo Pinheiro em 24/02/2015 21:00:05
/// <summary>

public class Trainpat
{
	protected string lvpmt_id;
	protected DateTime lvprev_part;
	protected Int16 lvKMOrigem;
	protected Int16 lvKMDestino;
	protected Int16 lvKMParada;
	protected string lvActivity;
	protected int lvEspera;
	protected DateTime lvdate_hist;

	public Trainpat()
	{

		Clear();
	}

	public Trainpat(string pmt_id, Int16 KMParada)
	{
		Clear();

		this.lvpmt_id = pmt_id;
		this.lvKMParada = KMParada;
		Load();
	}

	public Trainpat(string pmt_id, DateTime prev_part, Int16 KMOrigem, Int16 KMDestino, Int16 KMParada, string Activity, int Espera, DateTime date_hist)
	{

		this.lvpmt_id = pmt_id;
		this.lvprev_part = prev_part;
		this.lvKMOrigem = KMOrigem;
		this.lvKMDestino = KMDestino;
		this.lvKMParada = KMParada;
		this.lvActivity = Activity;
		this.lvEspera = Espera;
		this.lvdate_hist = date_hist;
	}

	public virtual bool Load()
	{
		bool lvResult = false;


		DataSet ds = TrainpatDataAccess.GetDataByKey(this.lvpmt_id, this.lvKMParada, "");

		foreach (DataRow row in ds.Tables[0].Rows)
		{
			this.lvpmt_id = ((row["pmt_id"] == DBNull.Value) ? "" : row["pmt_id"].ToString());
			this.lvprev_part = ((row["prev_part"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["prev_part"].ToString()));
			this.lvKMOrigem = ((row["KMOrigem"] == DBNull.Value) ? Int16.MinValue : (Int16)row["KMOrigem"]);
			this.lvKMDestino = ((row["KMDestino"] == DBNull.Value) ? Int16.MinValue : (Int16)row["KMDestino"]);
			this.lvKMParada = ((row["KMParada"] == DBNull.Value) ? Int16.MinValue : (Int16)row["KMParada"]);
			this.lvActivity = ((row["Activity"] == DBNull.Value) ? "" : row["Activity"].ToString());
			this.lvEspera = ((row["Espera"] == DBNull.Value) ? Int32.MinValue : (int)row["Espera"]);
			this.lvdate_hist = ((row["date_hist"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["date_hist"].ToString()));

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

		ds = TrainpatDataAccess.GetData(this.lvpmt_id, this.lvprev_part, this.lvKMOrigem, this.lvKMDestino, this.lvKMParada, this.lvActivity, this.lvEspera, this.lvdate_hist, "");

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

		lvResult.Append("]}; \n\n");


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

		ds = TrainpatDataAccess.GetData(this.lvpmt_id, this.lvprev_part, this.lvKMOrigem, this.lvKMDestino, this.lvKMParada, this.lvActivity, this.lvEspera, this.lvdate_hist, "");

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

			lvStrFlotClass += "Pmt_id: \"" + ((row["pmt_id"] == DBNull.Value) ? "" : row["pmt_id"].ToString()) + "\", ";
			lvStrFlotClass += "Prev_part: \"" + ((row["prev_part"] == DBNull.Value) ? "" : DateTime.Parse(row["prev_part"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
			lvStrFlotClass += "KMOrigem: " + ((row["KMOrigem"] == DBNull.Value) ? "\"\"" : row["KMOrigem"].ToString()) + ", ";
			lvStrFlotClass += "KMDestino: " + ((row["KMDestino"] == DBNull.Value) ? "\"\"" : row["KMDestino"].ToString()) + ", ";
			lvStrFlotClass += "KMParada: " + ((row["KMParada"] == DBNull.Value) ? "\"\"" : row["KMParada"].ToString()) + ", ";
			lvStrFlotClass += "Activity: \"" + ((row["Activity"] == DBNull.Value) ? "" : row["Activity"].ToString()) + "\", ";
			lvStrFlotClass += "Espera: " + ((row["Espera"] == DBNull.Value) ? "\"\"" : row["Espera"].ToString()) + ", ";
			lvStrFlotClass += "Date_hist: \"" + ((row["date_hist"] == DBNull.Value) ? "" : DateTime.Parse(row["date_hist"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")) + "\", ";
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

	public List<Trainpat> GetList()
	{
		List<Trainpat> lvResult = new List<Trainpat>();
		DataSet ds = null;
		Trainpat lvElement = null;

		ds = TrainpatDataAccess.GetData(this.lvpmt_id, this.lvprev_part, this.lvKMOrigem, this.lvKMDestino, this.lvKMParada, this.lvActivity, this.lvEspera, this.lvdate_hist, "");

		foreach (DataRow row in ds.Tables[0].Rows)
		{
			lvElement = new Trainpat();

			lvElement.Pmt_id = ((row["pmt_id"] == DBNull.Value) ? "" : row["pmt_id"].ToString());
			lvElement.Prev_part = ((row["prev_part"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["prev_part"].ToString()));
			lvElement.KMOrigem = ((row["KMOrigem"] == DBNull.Value) ? Int16.MinValue : (Int16)row["KMOrigem"]);
			lvElement.KMDestino = ((row["KMDestino"] == DBNull.Value) ? Int16.MinValue : (Int16)row["KMDestino"]);
			lvElement.KMParada = ((row["KMParada"] == DBNull.Value) ? Int16.MinValue : (Int16)row["KMParada"]);
			lvElement.Activity = ((row["Activity"] == DBNull.Value) ? "" : row["Activity"].ToString());
			lvElement.Espera = ((row["Espera"] == DBNull.Value) ? Int32.MinValue : (int)row["Espera"]);
			lvElement.Date_hist = ((row["date_hist"] == DBNull.Value) ? DateTime.MinValue : DateTime.Parse(row["date_hist"].ToString()));

			lvResult.Add(lvElement);
			lvElement = null;
		}

		return lvResult;
	}

	public virtual void Clear()
	{

		this.lvpmt_id = "";
		this.lvprev_part = DateTime.MinValue;
		this.lvKMOrigem = Int16.MinValue;
		this.lvKMDestino = Int16.MinValue;
		this.lvKMParada = Int16.MinValue;
		this.lvActivity = "";
		this.lvEspera = Int32.MinValue;
		this.lvdate_hist = DateTime.MinValue;
	}

	public virtual bool Insert()
	{
		bool lvResult = false;
		int lvRowsAffect = 0;


		try
		{
			lvRowsAffect = TrainpatDataAccess.Insert(this.lvpmt_id, this.lvprev_part, this.lvKMOrigem, this.lvKMDestino, this.lvKMParada, this.lvActivity, this.lvEspera, this.lvdate_hist);

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
			lvRowsAffect = TrainpatDataAccess.Update(this.lvpmt_id, this.lvprev_part, this.lvKMOrigem, this.lvKMDestino, this.lvKMParada, this.lvActivity, this.lvEspera, this.lvdate_hist);

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

	public bool UpdateKey(string pmt_id, Int16 KMParada)
	{
		bool lvResult = false;
		int lvRowsAffect = 0;

		try
		{
			lvRowsAffect = TrainpatDataAccess.UpdateKey(this.lvpmt_id, this.lvKMParada, pmt_id, KMParada);

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
			lvRowsAffect = TrainpatDataAccess.Delete(this.lvpmt_id, this.lvKMParada);

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

		ds = TrainpatDataAccess.GetData(this.lvpmt_id, this.lvprev_part, this.lvKMOrigem, this.lvKMDestino, this.lvKMParada, this.lvActivity, this.lvEspera, this.lvdate_hist, "");

		dt = ds.Tables[0];

		return dt;
	}

	public string Pmt_id
	{
		get
		{
			return this.lvpmt_id;
		}
		set
		{
			this.lvpmt_id = value;
		}
	}

	public DateTime Prev_part
	{
		get
		{
			return this.lvprev_part;
		}
		set
		{
			this.lvprev_part = value;
		}
	}

	public Int16 KMOrigem
	{
		get
		{
			return this.lvKMOrigem;
		}
		set
		{
			this.lvKMOrigem = value;
		}
	}

	public Int16 KMDestino
	{
		get
		{
			return this.lvKMDestino;
		}
		set
		{
			this.lvKMDestino = value;
		}
	}

	public Int16 KMParada
	{
		get
		{
			return this.lvKMParada;
		}
		set
		{
			this.lvKMParada = value;
		}
	}

	public string Activity
	{
		get
		{
			return this.lvActivity;
		}
		set
		{
			this.lvActivity = value;
		}
	}

	public int Espera
	{
		get
		{
			return this.lvEspera;
		}
		set
		{
			this.lvEspera = value;
		}
	}

	public DateTime Date_hist
	{
		get
		{
			return this.lvdate_hist;
		}
		set
		{
			this.lvdate_hist = value;
		}
	}


}

