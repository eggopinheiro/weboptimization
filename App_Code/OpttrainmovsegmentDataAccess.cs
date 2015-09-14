using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
/// <summary>
/// Criado por Eggo Pinheiro em 24/02/2015 23:12:16
/// <summary>

[DataObject(true)]
public class OpttrainmovsegmentDataAccess
{
	public OpttrainmovsegmentDataAccess()
	{
	//
	//TODO: Add constructor logic here
	//
	}

	[DataObjectMethod(DataObjectMethodType.Insert)]
	public static int Insert(double ptrain_id, DateTime phorario, int pcoordinate, Int16 plocation, Int16 ptrack, DateTime phist)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "insert into tbopttrainmovsegment(train_id, horario, coordinate, location, track, hist) ";
		lvSql += "values(@train_id, @horario, @coordinate, @location, @track, @hist)";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
		}

		if(phorario == DateTime.MinValue)
		{
			cmd.Parameters.Add("@horario", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@horario", MySqlDbType.DateTime).Value = phorario.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pcoordinate == Int32.MinValue)
		{
			cmd.Parameters.Add("@coordinate", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@coordinate", MySqlDbType.Int32).Value = pcoordinate;
		}

		if(plocation == Int16.MinValue)
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int16).Value = plocation;
		}

		if(ptrack == Int16.MinValue)
		{
			cmd.Parameters.Add("@track", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@track", MySqlDbType.Int16).Value = ptrack;
		}

		if(phist == DateTime.MinValue)
		{
			cmd.Parameters.Add("@hist", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@hist", MySqlDbType.DateTime).Value = phist.ToString("yyyy/MM/dd HH:mm:ss");
		}

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("OpttrainmovsegmentDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("OpttrainmovsegmentDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Update)]
	public static int Update(double ptrain_id, DateTime phorario, int pcoordinate, Int16 plocation, Int16 ptrack, DateTime phist)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "update tbopttrainmovsegment set coordinate=@coordinate, track=@track, hist=@hist Where train_id=@train_id And horario=@horario And location=@location";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
		}

		if(phorario == DateTime.MinValue)
		{
			cmd.Parameters.Add("@horario", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@horario", MySqlDbType.DateTime).Value = phorario.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pcoordinate == Int32.MinValue)
		{
			cmd.Parameters.Add("@coordinate", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@coordinate", MySqlDbType.Int32).Value = pcoordinate;
		}

		if(plocation == Int16.MinValue)
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int16).Value = plocation;
		}

		if(ptrack == Int16.MinValue)
		{
			cmd.Parameters.Add("@track", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@track", MySqlDbType.Int16).Value = ptrack;
		}

		if(phist == DateTime.MinValue)
		{
			cmd.Parameters.Add("@hist", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@hist", MySqlDbType.DateTime).Value = phist.ToString("yyyy/MM/dd HH:mm:ss");
		}

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("OpttrainmovsegmentDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("OpttrainmovsegmentDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Update)]
	public static int UpdateKey(double pOrigtrain_id, DateTime pOrighorario, Int16 pOriglocation, double ptrain_id, DateTime phorario, Int16 plocation)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "update tbopttrainmovsegment set train_id=@train_id, horario=@horario, location=@location Where train_id=@origtrain_id And horario=@orighorario And location=@origlocation";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
		}

		if(pOrigtrain_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@origtrain_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@origtrain_id", MySqlDbType.Double).Value = pOrigtrain_id;
		}

		if(phorario == DateTime.MinValue)
		{
			cmd.Parameters.Add("@horario", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@horario", MySqlDbType.DateTime).Value = phorario.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pOrighorario == DateTime.MinValue)
		{
			cmd.Parameters.Add("@orighorario", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@orighorario", MySqlDbType.DateTime).Value = pOrighorario.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(plocation == Int16.MinValue)
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int16).Value = plocation;
		}

		if(pOriglocation == Int16.MinValue)
		{
			cmd.Parameters.Add("@origlocation", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@origlocation", MySqlDbType.Int16).Value = pOriglocation;
		}

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("OpttrainmovsegmentDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("OpttrainmovsegmentDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Delete)]
	public static int Delete(double ptrain_id, DateTime phorario, Int16 plocation)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "delete from tbopttrainmovsegment Where train_id=@train_id And horario=@horario And location=@location";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
		}

		if(phorario == DateTime.MinValue)
		{
			cmd.Parameters.Add("@horario", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@horario", MySqlDbType.DateTime).Value = phorario.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(plocation == Int16.MinValue)
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int16).Value = plocation;
		}

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("OpttrainmovsegmentDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("OpttrainmovsegmentDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetAll()
	{
		string lvSql = "";
		DataSet ds = new DataSet();

		lvSql = "select * from tbopttrainmovsegment";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);
		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbopttrainmovsegment");
		return ds;
	}

    public static DataSet GetGroupData(double ptrain_id = -999999999999, string pStrSortField = "")
    {
        string lvSql = "";
        string lvSqlWhere = "";
        DataSet ds = new DataSet();

        lvSql = "select distinct tbopttrainmovsegment.train_id, tbtrain.name tbtrain_name From tbopttrainmovsegment Left Join tbtrain on tbopttrainmovsegment.train_id=tbtrain.train_id";

        if (ptrain_id > ConnectionManager.DOUBLE_REF_VALUE)
        {
            if (lvSqlWhere.Length == 0)
            {
                lvSqlWhere += "tbopttrainmovsegment.train_id=@train_id";
            }
            else
            {
                lvSqlWhere += " And tbopttrainmovsegment.train_id=@train_id";
            }
        }

        if (!string.IsNullOrEmpty(lvSqlWhere))
        {
            lvSql += " where " + lvSqlWhere;
        }

        if (!string.IsNullOrEmpty(pStrSortField))
        {
            lvSql += " order by " + pStrSortField;
        }

        MySqlConnection conn = ConnectionManager.GetObjConnection();
        MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        if (ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
        {
            cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
        }
        else
        {
            cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
        }

        cmd.CommandType = CommandType.Text;

        conn.Close();

        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        adapter.Fill(ds, "tbopttrainmovsegment");
        return ds;
    }

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetData(double ptrain_id = -999999999999, DateTime phorario = default(DateTime), int pcoordinate = -2147483648, Int16 plocation = -32768, Int16 ptrack = -32768, DateTime phist = default(DateTime), Boolean pUseForeignKey = false, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		if(pUseForeignKey)
		{
//			lvSql = "select tbopttrainmovsegment.train_id, tbopttrainmovsegment.horario, tbopttrainmovsegment.coordinate, tbopttrainmovsegment.location, tbopttrainmovsegment.track, tbopttrainmovsegment.hist, tbtrain.train_id tbtrain_train_id, tbtrain.name tbtrain_name, tbtrain.type tbtrain_type, tbtrain.creation_tm tbtrain_creation_tm, tbtrain.departure_time tbtrain_departure_time, tbtrain.arrival_time tbtrain_arrival_time, tbtrain.direction tbtrain_direction, tbtrain.priority tbtrain_priority, tbtrain.status tbtrain_status, tbtrain.departure_coordinate tbtrain_departure_coordinate, tbtrain.arrival_coordinate tbtrain_arrival_coordinate, tbtrain.lotes tbtrain_lotes, tbtrain.isvalid tbtrain_isvalid, tbtrain.last_coordinate tbtrain_last_coordinate, tbtrain.last_info_updated tbtrain_last_info_updated, tbtrain.pmt_id tbtrain_pmt_id, tbtrain.OS tbtrain_OS, tbtrain.plan_id tbtrain_plan_id, tbtrain.OSSGF tbtrain_OSSGF, tbtrain.last_track tbtrain_last_track, tbtrain.hist tbtrain_hist, tbtrain.cmd_loco_id tbtrain_cmd_loco_id, tbtrain.usr_cmd_loco_id tbtrain_usr_cmd_loco_id, tbtrain.plan_id_lock tbtrain_plan_id_lock, tbtrain.oid tbtrain_oid, tbtrain.unilogcurrcoord tbtrain_unilogcurrcoord, tbtrain.unilogcurinfodate tbtrain_unilogcurinfodate, tbtrain.unilogcurseg tbtrain_unilogcurseg, tbtrain.loco_code tbtrain_loco_code From tbopttrainmovsegment Left Join tbtrain on tbopttrainmovsegment.train_id=tbtrain.train_id";
            lvSql = "select tbopttrainmovsegment.train_id, tbopttrainmovsegment.horario, tbopttrainmovsegment.coordinate, tbopttrainmovsegment.location, tbopttrainmovsegment.track, tbopttrainmovsegment.hist, tbtrain.name tbtrain_name, tbtrain.pmt_id tbtrain_pmt_id, tbtrain.OS tbtrain_OS From tbopttrainmovsegment Left Join tbtrain on tbopttrainmovsegment.train_id=tbtrain.train_id";
        }
		else
		{
			lvSql = "select tbopttrainmovsegment.train_id, tbopttrainmovsegment.horario, tbopttrainmovsegment.coordinate, tbopttrainmovsegment.location, tbopttrainmovsegment.track, tbopttrainmovsegment.hist From tbopttrainmovsegment";
		}

		if(ptrain_id > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbopttrainmovsegment.train_id=@train_id";
			}
			else
			{
				lvSqlWhere += " And tbopttrainmovsegment.train_id=@train_id";
			}
		}

		if(phorario > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbopttrainmovsegment.horario=@horario";
			}
			else
			{
				lvSqlWhere += " And tbopttrainmovsegment.horario=@horario";
			}
		}

		if(pcoordinate > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbopttrainmovsegment.coordinate=@coordinate";
			}
			else
			{
				lvSqlWhere += " And tbopttrainmovsegment.coordinate=@coordinate";
			}
		}

		if(plocation > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbopttrainmovsegment.location=@location";
			}
			else
			{
				lvSqlWhere += " And tbopttrainmovsegment.location=@location";
			}
		}

		if(ptrack > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbopttrainmovsegment.track=@track";
			}
			else
			{
				lvSqlWhere += " And tbopttrainmovsegment.track=@track";
			}
		}

		if(phist > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbopttrainmovsegment.hist=@hist";
			}
			else
			{
				lvSqlWhere += " And tbopttrainmovsegment.hist=@hist";
			}
		}

		if(!string.IsNullOrEmpty(lvSqlWhere))
		{
			lvSql += " where " + lvSqlWhere;
		}

		if(!string.IsNullOrEmpty(pStrSortField))
		{
			lvSql += " order by " + pStrSortField;
		}

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
		}

		if(phorario == DateTime.MinValue)
		{
			cmd.Parameters.Add("@horario", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@horario", MySqlDbType.DateTime).Value = phorario.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pcoordinate == Int32.MinValue)
		{
			cmd.Parameters.Add("@coordinate", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@coordinate", MySqlDbType.Int32).Value = pcoordinate;
		}

		if(plocation == Int16.MinValue)
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int16).Value = plocation;
		}

		if(ptrack == Int16.MinValue)
		{
			cmd.Parameters.Add("@track", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@track", MySqlDbType.Int16).Value = ptrack;
		}

		if(phist == DateTime.MinValue)
		{
			cmd.Parameters.Add("@hist", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@hist", MySqlDbType.DateTime).Value = phist.ToString("yyyy/MM/dd HH:mm:ss");
		}

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbopttrainmovsegment");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetDataByRange(double ptrain_idInicio = -999999999999, double ptrain_idFim = -999999999999, DateTime phorarioInicio = default(DateTime), DateTime phorarioFim = default(DateTime), int pcoordinateInicio = -2147483648, int pcoordinateFim = -2147483648, Int16 plocationInicio = -32768, Int16 plocationFim = -32768, Int16 ptrackInicio = -32768, Int16 ptrackFim = -32768, DateTime phistInicio = default(DateTime), DateTime phistFim = default(DateTime), Boolean pUseForeignKey = false, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		if(pUseForeignKey)
		{
			lvSql = "select tbopttrainmovsegment.train_id, tbopttrainmovsegment.horario, tbopttrainmovsegment.coordinate, tbopttrainmovsegment.location, tbopttrainmovsegment.track, tbopttrainmovsegment.hist, tbtrain.train_id tbtrain_train_id, tbtrain.name tbtrain_name, tbtrain.type tbtrain_type, tbtrain.creation_tm tbtrain_creation_tm, tbtrain.departure_time tbtrain_departure_time, tbtrain.arrival_time tbtrain_arrival_time, tbtrain.direction tbtrain_direction, tbtrain.priority tbtrain_priority, tbtrain.status tbtrain_status, tbtrain.departure_coordinate tbtrain_departure_coordinate, tbtrain.arrival_coordinate tbtrain_arrival_coordinate, tbtrain.lotes tbtrain_lotes, tbtrain.isvalid tbtrain_isvalid, tbtrain.last_coordinate tbtrain_last_coordinate, tbtrain.last_info_updated tbtrain_last_info_updated, tbtrain.pmt_id tbtrain_pmt_id, tbtrain.OS tbtrain_OS, tbtrain.plan_id tbtrain_plan_id, tbtrain.OSSGF tbtrain_OSSGF, tbtrain.last_track tbtrain_last_track, tbtrain.hist tbtrain_hist, tbtrain.cmd_loco_id tbtrain_cmd_loco_id, tbtrain.usr_cmd_loco_id tbtrain_usr_cmd_loco_id, tbtrain.plan_id_lock tbtrain_plan_id_lock, tbtrain.oid tbtrain_oid, tbtrain.unilogcurrcoord tbtrain_unilogcurrcoord, tbtrain.unilogcurinfodate tbtrain_unilogcurinfodate, tbtrain.unilogcurseg tbtrain_unilogcurseg, tbtrain.loco_code tbtrain_loco_code From tbopttrainmovsegment Left Join tbtrain on tbopttrainmovsegment.train_id=tbtrain.train_id";
		}
		else
		{
			lvSql = "select tbopttrainmovsegment.train_id, tbopttrainmovsegment.horario, tbopttrainmovsegment.coordinate, tbopttrainmovsegment.location, tbopttrainmovsegment.track, tbopttrainmovsegment.hist From tbopttrainmovsegment";
		}

		if(ptrain_idInicio > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbopttrainmovsegment.train_id >= @train_idInicio";
			}
			else
			{
				lvSqlWhere += " And tbopttrainmovsegment.train_id >= @train_idInicio";
			}
		}

		if(ptrain_idFim > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbopttrainmovsegment.train_id <= @train_idFim";
			}
			else
			{
				lvSqlWhere += " And tbopttrainmovsegment.train_id <= @train_idFim";
			}
		}

		if(phorarioInicio > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbopttrainmovsegment.horario >= @horarioInicio";
			}
			else
			{
				lvSqlWhere += " And tbopttrainmovsegment.horario >= @horarioInicio";
			}
		}

		if(phorarioFim > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbopttrainmovsegment.horario <= @horarioFim";
			}
			else
			{
				lvSqlWhere += " And tbopttrainmovsegment.horario <= @horarioFim";
			}
		}

		if(pcoordinateInicio > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbopttrainmovsegment.coordinate >= @coordinateInicio";
			}
			else
			{
				lvSqlWhere += " And tbopttrainmovsegment.coordinate >= @coordinateInicio";
			}
		}

		if(pcoordinateFim > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbopttrainmovsegment.coordinate <= @coordinateFim";
			}
			else
			{
				lvSqlWhere += " And tbopttrainmovsegment.coordinate <= @coordinateFim";
			}
		}

		if(plocationInicio > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbopttrainmovsegment.location >= @locationInicio";
			}
			else
			{
				lvSqlWhere += " And tbopttrainmovsegment.location >= @locationInicio";
			}
		}

		if(plocationFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbopttrainmovsegment.location <= @locationFim";
			}
			else
			{
				lvSqlWhere += " And tbopttrainmovsegment.location <= @locationFim";
			}
		}

		if(ptrackInicio > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbopttrainmovsegment.track >= @trackInicio";
			}
			else
			{
				lvSqlWhere += " And tbopttrainmovsegment.track >= @trackInicio";
			}
		}

		if(ptrackFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbopttrainmovsegment.track <= @trackFim";
			}
			else
			{
				lvSqlWhere += " And tbopttrainmovsegment.track <= @trackFim";
			}
		}

		if(phistInicio > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbopttrainmovsegment.hist >= @histInicio";
			}
			else
			{
				lvSqlWhere += " And tbopttrainmovsegment.hist >= @histInicio";
			}
		}

		if(phistFim > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbopttrainmovsegment.hist <= @histFim";
			}
			else
			{
				lvSqlWhere += " And tbopttrainmovsegment.hist <= @histFim";
			}
		}

		if(!string.IsNullOrEmpty(lvSqlWhere))
		{
			lvSql += " where " + lvSqlWhere;
		}

		if(!string.IsNullOrEmpty(pStrSortField))
		{
			lvSql += " order by " + pStrSortField;
		}

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(ptrain_idInicio == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@train_idInicio", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@train_idInicio", MySqlDbType.Double).Value = ptrain_idInicio;
		}

		if(ptrain_idFim == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@train_idFim", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@train_idFim", MySqlDbType.Double).Value = ptrain_idFim;
		}

		if(phorarioInicio == DateTime.MinValue)
		{
			cmd.Parameters.Add("@horarioInicio", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@horarioInicio", MySqlDbType.DateTime).Value = phorarioInicio.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(phorarioFim == DateTime.MinValue)
		{
			cmd.Parameters.Add("@horarioFim", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@horarioFim", MySqlDbType.DateTime).Value = phorarioFim.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pcoordinateInicio == Int32.MinValue)
		{
			cmd.Parameters.Add("@coordinateInicio", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@coordinateInicio", MySqlDbType.Int32).Value = pcoordinateInicio;
		}

		if(pcoordinateFim == Int32.MinValue)
		{
			cmd.Parameters.Add("@coordinateFim", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@coordinateFim", MySqlDbType.Int32).Value = pcoordinateFim;
		}

		if(plocationInicio == Int16.MinValue)
		{
			cmd.Parameters.Add("@locationInicio", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@locationInicio", MySqlDbType.Int16).Value = plocationInicio;
		}

		if(plocationFim == Int16.MinValue)
		{
			cmd.Parameters.Add("@locationFim", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@locationFim", MySqlDbType.Int16).Value = plocationFim;
		}

		if(ptrackInicio == Int16.MinValue)
		{
			cmd.Parameters.Add("@trackInicio", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@trackInicio", MySqlDbType.Int16).Value = ptrackInicio;
		}

		if(ptrackFim == Int16.MinValue)
		{
			cmd.Parameters.Add("@trackFim", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@trackFim", MySqlDbType.Int16).Value = ptrackFim;
		}

		if(phistInicio == DateTime.MinValue)
		{
			cmd.Parameters.Add("@histInicio", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@histInicio", MySqlDbType.DateTime).Value = phistInicio.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(phistFim == DateTime.MinValue)
		{
			cmd.Parameters.Add("@histFim", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@histFim", MySqlDbType.DateTime).Value = phistFim.ToString("yyyy/MM/dd HH:mm:ss");
		}

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbopttrainmovsegment");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetDataByKey(double ptrain_id = -999999999999, DateTime phorario = default(DateTime), Int16 plocation = -32768, Boolean pUseForeignKey = false, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		if(pUseForeignKey)
		{
			lvSql = "select tbopttrainmovsegment.train_id, tbopttrainmovsegment.horario, tbopttrainmovsegment.coordinate, tbopttrainmovsegment.location, tbopttrainmovsegment.track, tbopttrainmovsegment.hist, tbtrain.train_id tbtrain_train_id, tbtrain.name tbtrain_name, tbtrain.type tbtrain_type, tbtrain.creation_tm tbtrain_creation_tm, tbtrain.departure_time tbtrain_departure_time, tbtrain.arrival_time tbtrain_arrival_time, tbtrain.direction tbtrain_direction, tbtrain.priority tbtrain_priority, tbtrain.status tbtrain_status, tbtrain.departure_coordinate tbtrain_departure_coordinate, tbtrain.arrival_coordinate tbtrain_arrival_coordinate, tbtrain.lotes tbtrain_lotes, tbtrain.isvalid tbtrain_isvalid, tbtrain.last_coordinate tbtrain_last_coordinate, tbtrain.last_info_updated tbtrain_last_info_updated, tbtrain.pmt_id tbtrain_pmt_id, tbtrain.OS tbtrain_OS, tbtrain.plan_id tbtrain_plan_id, tbtrain.OSSGF tbtrain_OSSGF, tbtrain.last_track tbtrain_last_track, tbtrain.hist tbtrain_hist, tbtrain.cmd_loco_id tbtrain_cmd_loco_id, tbtrain.usr_cmd_loco_id tbtrain_usr_cmd_loco_id, tbtrain.plan_id_lock tbtrain_plan_id_lock, tbtrain.oid tbtrain_oid, tbtrain.unilogcurrcoord tbtrain_unilogcurrcoord, tbtrain.unilogcurinfodate tbtrain_unilogcurinfodate, tbtrain.unilogcurseg tbtrain_unilogcurseg, tbtrain.loco_code tbtrain_loco_code From tbopttrainmovsegment Left Join tbtrain on tbopttrainmovsegment.train_id=tbtrain.train_id";
		}
		else
		{
			lvSql = "select tbopttrainmovsegment.train_id, tbopttrainmovsegment.horario, tbopttrainmovsegment.coordinate, tbopttrainmovsegment.location, tbopttrainmovsegment.track, tbopttrainmovsegment.hist From tbopttrainmovsegment";
		}

		if(ptrain_id > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbopttrainmovsegment.train_id=@train_id";
			}
			else
			{
				lvSqlWhere += " And tbopttrainmovsegment.train_id=@train_id";
			}
		}

		if(phorario > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbopttrainmovsegment.horario=@horario";
			}
			else
			{
				lvSqlWhere += " And tbopttrainmovsegment.horario=@horario";
			}
		}

		if(plocation > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbopttrainmovsegment.location=@location";
			}
			else
			{
				lvSqlWhere += " And tbopttrainmovsegment.location=@location";
			}
		}

		if(!string.IsNullOrEmpty(lvSqlWhere))
		{
			lvSql += " where " + lvSqlWhere;
		}

		if(!string.IsNullOrEmpty(pStrSortField))
		{
			lvSql += " order by " + pStrSortField;
		}

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
		}

		if(phorario == DateTime.MinValue)
		{
			cmd.Parameters.Add("@horario", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@horario", MySqlDbType.DateTime).Value = phorario.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(plocation == Int16.MinValue)
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@location", MySqlDbType.Int16).Value = plocation;
		}

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbopttrainmovsegment");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetDataByTrain(double ptrain_id = -999999999999, Boolean pUseForeignKey = false, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		if(pUseForeignKey)
		{
			lvSql = "select tbopttrainmovsegment.train_id, tbopttrainmovsegment.horario, tbopttrainmovsegment.coordinate, tbopttrainmovsegment.location, tbopttrainmovsegment.track, tbopttrainmovsegment.hist, tbtrain.train_id tbtrain_train_id, tbtrain.name tbtrain_name, tbtrain.type tbtrain_type, tbtrain.creation_tm tbtrain_creation_tm, tbtrain.departure_time tbtrain_departure_time, tbtrain.arrival_time tbtrain_arrival_time, tbtrain.direction tbtrain_direction, tbtrain.priority tbtrain_priority, tbtrain.status tbtrain_status, tbtrain.departure_coordinate tbtrain_departure_coordinate, tbtrain.arrival_coordinate tbtrain_arrival_coordinate, tbtrain.lotes tbtrain_lotes, tbtrain.isvalid tbtrain_isvalid, tbtrain.last_coordinate tbtrain_last_coordinate, tbtrain.last_info_updated tbtrain_last_info_updated, tbtrain.pmt_id tbtrain_pmt_id, tbtrain.OS tbtrain_OS, tbtrain.plan_id tbtrain_plan_id, tbtrain.OSSGF tbtrain_OSSGF, tbtrain.last_track tbtrain_last_track, tbtrain.hist tbtrain_hist, tbtrain.cmd_loco_id tbtrain_cmd_loco_id, tbtrain.usr_cmd_loco_id tbtrain_usr_cmd_loco_id, tbtrain.plan_id_lock tbtrain_plan_id_lock, tbtrain.oid tbtrain_oid, tbtrain.unilogcurrcoord tbtrain_unilogcurrcoord, tbtrain.unilogcurinfodate tbtrain_unilogcurinfodate, tbtrain.unilogcurseg tbtrain_unilogcurseg, tbtrain.loco_code tbtrain_loco_code From tbopttrainmovsegment Left Join tbtrain on tbopttrainmovsegment.train_id=tbtrain.train_id";
		}
		else
		{
			lvSql = "select tbopttrainmovsegment.train_id, tbopttrainmovsegment.horario, tbopttrainmovsegment.coordinate, tbopttrainmovsegment.location, tbopttrainmovsegment.track, tbopttrainmovsegment.hist From tbopttrainmovsegment";
		}

		lvSqlWhere = "tbopttrainmovsegment.train_id=@train_id";

		if(!string.IsNullOrEmpty(lvSqlWhere))
		{
			lvSql += " where " + lvSqlWhere;
		}

		if(!string.IsNullOrEmpty(pStrSortField))
		{
			lvSql += " order by " + pStrSortField;
		}

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
		}

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbopttrainmovsegment");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetCrossData(double ptrain_id, string pStrSortField)
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		lvSql = "select tbopttrainmovsegment.train_id, tbopttrainmovsegment.horario, tbopttrainmovsegment.coordinate, tbopttrainmovsegment.location, tbopttrainmovsegment.track, tbopttrainmovsegment.hist, tbtrain.train_id tbtrain_train_id, tbtrain.name tbtrain_name, tbtrain.type tbtrain_type, tbtrain.creation_tm tbtrain_creation_tm, tbtrain.departure_time tbtrain_departure_time, tbtrain.arrival_time tbtrain_arrival_time, tbtrain.direction tbtrain_direction, tbtrain.priority tbtrain_priority, tbtrain.status tbtrain_status, tbtrain.departure_coordinate tbtrain_departure_coordinate, tbtrain.arrival_coordinate tbtrain_arrival_coordinate, tbtrain.lotes tbtrain_lotes, tbtrain.isvalid tbtrain_isvalid, tbtrain.last_coordinate tbtrain_last_coordinate, tbtrain.last_info_updated tbtrain_last_info_updated, tbtrain.pmt_id tbtrain_pmt_id, tbtrain.OS tbtrain_OS, tbtrain.plan_id tbtrain_plan_id, tbtrain.OSSGF tbtrain_OSSGF, tbtrain.last_track tbtrain_last_track, tbtrain.hist tbtrain_hist, tbtrain.cmd_loco_id tbtrain_cmd_loco_id, tbtrain.usr_cmd_loco_id tbtrain_usr_cmd_loco_id, tbtrain.plan_id_lock tbtrain_plan_id_lock, tbtrain.oid tbtrain_oid, tbtrain.unilogcurrcoord tbtrain_unilogcurrcoord, tbtrain.unilogcurinfodate tbtrain_unilogcurinfodate, tbtrain.unilogcurseg tbtrain_unilogcurseg, tbtrain.loco_code tbtrain_loco_code From tbopttrainmovsegment Inner Join tbtrain on tbopttrainmovsegment.train_id=tbtrain.train_id";

		if(ptrain_id > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbopttrainmovsegment.train_id=@train_id";
			}
			else
			{
				lvSqlWhere += " And tbopttrainmovsegment.train_id=@train_id";
			}
		}


		if(!string.IsNullOrEmpty(lvSqlWhere))
		{
			lvSql += " where " + lvSqlWhere;
		}

		if(!string.IsNullOrEmpty(pStrSortField))
		{
			lvSql += " order by " + pStrSortField;
		}

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(ptrain_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@train_id", MySqlDbType.Double).Value = ptrain_id;
		}

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbopttrainmovsegment");
		return ds;
	}

}

