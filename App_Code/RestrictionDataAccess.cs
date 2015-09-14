using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
/// <summary>
/// Criado por Eggo Pinheiro em 12/03/2015 15:53:23
/// <summary>

[DataObject(true)]
public class RestrictionDataAccess
{
    public RestrictionDataAccess()
	{
	//
	//TODO: Add constructor logic here
	//
	}

	[DataObjectMethod(DataObjectMethodType.Insert)]
	public static int Insert(double pti_id, int pstart_pos, int pend_pos, DateTime pstart_time, DateTime pend_time, Int16 pfield_interdicted, string pss_name, string pstatus, string preason, DateTime pplan_time, DateTime phist)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "insert into tbinterdicao(ti_id, start_pos, end_pos, start_time, end_time, field_interdicted, ss_name, status, reason, plan_time, hist) ";
		lvSql += "values(@ti_id, @start_pos, @end_pos, @start_time, @end_time, @field_interdicted, @ss_name, @status, @reason, @plan_time, @hist)";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(pti_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@ti_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@ti_id", MySqlDbType.Double).Value = pti_id;
		}

		if(pstart_pos == Int32.MinValue)
		{
			cmd.Parameters.Add("@start_pos", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@start_pos", MySqlDbType.Int32).Value = pstart_pos;
		}

		if(pend_pos == Int32.MinValue)
		{
			cmd.Parameters.Add("@end_pos", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@end_pos", MySqlDbType.Int32).Value = pend_pos;
		}

		if(pstart_time == DateTime.MinValue)
		{
			cmd.Parameters.Add("@start_time", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@start_time", MySqlDbType.DateTime).Value = pstart_time.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pend_time == DateTime.MinValue)
		{
			cmd.Parameters.Add("@end_time", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@end_time", MySqlDbType.DateTime).Value = pend_time.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pfield_interdicted == Int16.MinValue)
		{
			cmd.Parameters.Add("@field_interdicted", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@field_interdicted", MySqlDbType.Int16).Value = pfield_interdicted;
		}

		cmd.Parameters.Add("@ss_name", MySqlDbType.String).Value = pss_name;

		cmd.Parameters.Add("@status", MySqlDbType.String).Value = pstatus;

		cmd.Parameters.Add("@reason", MySqlDbType.String).Value = preason;

		if(pplan_time == DateTime.MinValue)
		{
			cmd.Parameters.Add("@plan_time", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@plan_time", MySqlDbType.DateTime).Value = pplan_time.ToString("yyyy/MM/dd HH:mm:ss");
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
			DebugLog.Logar("InterdicaoDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("InterdicaoDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Update)]
	public static int Update(double pti_id, int pstart_pos, int pend_pos, DateTime pstart_time, DateTime pend_time, Int16 pfield_interdicted, string pss_name, string pstatus, string preason, DateTime pplan_time, DateTime phist)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "update tbinterdicao set start_pos=@start_pos, end_pos=@end_pos, start_time=@start_time, end_time=@end_time, field_interdicted=@field_interdicted, ss_name=@ss_name, status=@status, reason=@reason, plan_time=@plan_time, hist=@hist Where ti_id=@ti_id";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(pti_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@ti_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@ti_id", MySqlDbType.Double).Value = pti_id;
		}

		if(pstart_pos == Int32.MinValue)
		{
			cmd.Parameters.Add("@start_pos", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@start_pos", MySqlDbType.Int32).Value = pstart_pos;
		}

		if(pend_pos == Int32.MinValue)
		{
			cmd.Parameters.Add("@end_pos", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@end_pos", MySqlDbType.Int32).Value = pend_pos;
		}

		if(pstart_time == DateTime.MinValue)
		{
			cmd.Parameters.Add("@start_time", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@start_time", MySqlDbType.DateTime).Value = pstart_time.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pend_time == DateTime.MinValue)
		{
			cmd.Parameters.Add("@end_time", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@end_time", MySqlDbType.DateTime).Value = pend_time.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pfield_interdicted == Int16.MinValue)
		{
			cmd.Parameters.Add("@field_interdicted", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@field_interdicted", MySqlDbType.Int16).Value = pfield_interdicted;
		}

		cmd.Parameters.Add("@ss_name", MySqlDbType.String).Value = pss_name;
		cmd.Parameters.Add("@status", MySqlDbType.String).Value = pstatus;
		cmd.Parameters.Add("@reason", MySqlDbType.String).Value = preason;
		if(pplan_time == DateTime.MinValue)
		{
			cmd.Parameters.Add("@plan_time", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@plan_time", MySqlDbType.DateTime).Value = pplan_time.ToString("yyyy/MM/dd HH:mm:ss");
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
			DebugLog.Logar("InterdicaoDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("InterdicaoDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Update)]
	public static int UpdateKey(double pOrigti_id, double pti_id)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "update tbinterdicao set ti_id=@ti_id Where ti_id=@origti_id";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(pti_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@ti_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@ti_id", MySqlDbType.Double).Value = pti_id;
		}

		if(pOrigti_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@origti_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@origti_id", MySqlDbType.Double).Value = pOrigti_id;
		}

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("InterdicaoDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("InterdicaoDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Delete)]
	public static int Delete(double pti_id)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "delete from tbinterdicao Where ti_id=@ti_id";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(pti_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@ti_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@ti_id", MySqlDbType.Double).Value = pti_id;
		}

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("InterdicaoDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("InterdicaoDataAccess => (" + lvSql + ") :: " + nullex.ToString());
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

		lvSql = "select * from tbinterdicao";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);
		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbinterdicao");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
    public static DataSet GetData(DateTime pDate, string pStrSortField = "")
	{
		string lvSql = "";
		DataSet ds = new DataSet();

        DateTime lvInitialDate = pDate.Date;
        DateTime lvFinalDate = pDate.AddDays(1).AddSeconds(-1);

        if (!string.IsNullOrEmpty(pStrSortField))
        {
            lvSql = "SELECT tbinterdicao.ti_id rest_id, tbinterdicao.start_time start_time, tbinterdicao.end_time end_time, (tbinterdicao.start_pos / 100000) startPos, (tbinterdicao.end_pos / 100000) endPos, tbinterdicao.reason reason, 'interdicao' rest_type FROM tbinterdicao WHERE (((end_time > @InitialDate AND end_time < @FinalDate) AND status = 'Cancelled') OR ((status = 'Active') AND (end_time >= DATE_SUB(now(), INTERVAL 10 MINUTE))) OR ((status = 'Planned') AND (start_time >= now() AND start_time < @FinalDate)) OR ((status = 'ToBeActive') AND (start_time >= now() AND start_time < @FinalDate))) AND (start_pos != end_pos) AND (field_interdicted = 1) AND (status <> 'Removed') UNION SELECT tbinterdicao.ti_id rest_id, tbinterdicao.start_time start_time, tbinterdicao.end_time end_time, (tbinterdicao.start_pos / 100000) startPos, (tbinterdicao.end_pos / 100000) endPos, tbinterdicao.reason reason, 'intervencao' rest_type FROM tbinterdicao Where (((end_time > @InitialDate And end_time < @FinalDate) And status = 'Cancelled') Or ((status = 'Active') And (end_time >= DATE_SUB(now(), INTERVAL 10 MINUTE))) Or ((status = 'Planned') And (start_time >= now() And start_time < @FinalDate)) Or ((status = 'ToBeActive') And (start_time >= now() And start_time < @FinalDate))) AND (start_pos != end_pos) AND (field_interdicted = 0) AND (status <> 'Removed') UNION SELECT tbspeedrestricted.sra_id rest_id, tbspeedrestricted.start_time start_time, tbspeedrestricted.end_time end_time, (tbspeedrestricted.start_pos / 100000) startPos, (tbspeedrestricted.end_pos / 100000) endPos, tbspeedrestricted.reason reason, 'restricao_velocidade' rest_type FROM tbspeedrestricted where (((((end_time >= @InitialDate And end_time <= @FinalDate) Or (start_time >= @InitialDate And start_time <= @FinalDate) Or (start_time < @InitialDate And end_time > @FinalDate)) And status = 'Cancelled')) Or (status = 'Active' Or status = 'ToExpire' Or status = 'Expired')) order by " + pStrSortField;
        }
        else
        {
            lvSql = "SELECT tbinterdicao.ti_id rest_id, tbinterdicao.start_time start_time, tbinterdicao.end_time end_time, (tbinterdicao.start_pos / 100000) startPos, (tbinterdicao.end_pos / 100000) endPos, tbinterdicao.reason reason, 'interdicao' rest_type FROM tbinterdicao WHERE (((end_time > @InitialDate AND end_time < @FinalDate) AND status = 'Cancelled') OR ((status = 'Active') AND (end_time >= DATE_SUB(now(), INTERVAL 10 MINUTE))) OR ((status = 'Planned') AND (start_time >= now() AND start_time < @FinalDate)) OR ((status = 'ToBeActive') AND (start_time >= now() AND start_time < @FinalDate))) AND (start_pos != end_pos) AND (field_interdicted = 1) AND (status <> 'Removed')  UNION  SELECT tbinterdicao.ti_id rest_id, tbinterdicao.start_time start_time, tbinterdicao.end_time end_time, (tbinterdicao.start_pos / 100000) startPos, (tbinterdicao.end_pos / 100000) endPos, tbinterdicao.reason reason, 'intervencao' rest_type FROM tbinterdicao Where (((end_time > @InitialDate And end_time < @FinalDate) And status = 'Cancelled') Or ((status = 'Active') And (end_time >= DATE_SUB(now(), INTERVAL 10 MINUTE))) Or ((status = 'Planned') And (start_time >= now() And start_time < @FinalDate)) Or ((status = 'ToBeActive') And (start_time >= now() And start_time < @FinalDate))) AND (start_pos != end_pos) AND (field_interdicted = 0) AND (status <> 'Removed') UNION SELECT tbspeedrestricted.sra_id rest_id, tbspeedrestricted.start_time start_time, tbspeedrestricted.end_time end_time, (tbspeedrestricted.start_pos / 100000) startPos, (tbspeedrestricted.end_pos / 100000) endPos, tbspeedrestricted.reason reason, 'restricao_velocidade' rest_type FROM tbspeedrestricted where (((((end_time >= @InitialDate And end_time <= @FinalDate) Or (start_time >= @InitialDate And start_time <= @FinalDate) Or (start_time < @InitialDate And end_time > @FinalDate)) And status = 'Cancelled')) Or (status = 'Active' Or status = 'ToExpire' Or status = 'Expired'))";
        }

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

        cmd.Parameters.Add("@InitialDate", MySql.Data.MySqlClient.MySqlDbType.DateTime).Value = lvInitialDate;
        cmd.Parameters.Add("@FinalDate", MySql.Data.MySqlClient.MySqlDbType.DateTime).Value = lvFinalDate;

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "restrictions");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetDataByRange(double pti_idInicio = -999999999999, double pti_idFim = -999999999999, int pstart_posInicio = -2147483648, int pstart_posFim = -2147483648, int pend_posInicio = -2147483648, int pend_posFim = -2147483648, DateTime pstart_timeInicio = default(DateTime), DateTime pstart_timeFim = default(DateTime), DateTime pend_timeInicio = default(DateTime), DateTime pend_timeFim = default(DateTime), Int16 pfield_interdictedInicio = -32768, Int16 pfield_interdictedFim = -32768, string pss_name = "", string pstatus = "", string preason = "", DateTime pplan_timeInicio = default(DateTime), DateTime pplan_timeFim = default(DateTime), DateTime phistInicio = default(DateTime), DateTime phistFim = default(DateTime), string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		lvSql = "select tbinterdicao.ti_id, tbinterdicao.start_pos, tbinterdicao.end_pos, tbinterdicao.start_time, tbinterdicao.end_time, tbinterdicao.field_interdicted, tbinterdicao.ss_name, tbinterdicao.status, tbinterdicao.reason, tbinterdicao.plan_time, tbinterdicao.hist From tbinterdicao";

		if(pti_idInicio > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbinterdicao.ti_id >= @ti_idInicio";
			}
			else
			{
				lvSqlWhere += " And tbinterdicao.ti_id >= @ti_idInicio";
			}
		}

		if(pti_idFim > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbinterdicao.ti_id <= @ti_idFim";
			}
			else
			{
				lvSqlWhere += " And tbinterdicao.ti_id <= @ti_idFim";
			}
		}

		if(pstart_posInicio > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbinterdicao.start_pos >= @start_posInicio";
			}
			else
			{
				lvSqlWhere += " And tbinterdicao.start_pos >= @start_posInicio";
			}
		}

		if(pstart_posFim > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbinterdicao.start_pos <= @start_posFim";
			}
			else
			{
				lvSqlWhere += " And tbinterdicao.start_pos <= @start_posFim";
			}
		}

		if(pend_posInicio > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbinterdicao.end_pos >= @end_posInicio";
			}
			else
			{
				lvSqlWhere += " And tbinterdicao.end_pos >= @end_posInicio";
			}
		}

		if(pend_posFim > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbinterdicao.end_pos <= @end_posFim";
			}
			else
			{
				lvSqlWhere += " And tbinterdicao.end_pos <= @end_posFim";
			}
		}

		if(pstart_timeInicio > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbinterdicao.start_time >= @start_timeInicio";
			}
			else
			{
				lvSqlWhere += " And tbinterdicao.start_time >= @start_timeInicio";
			}
		}

		if(pstart_timeFim > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbinterdicao.start_time <= @start_timeFim";
			}
			else
			{
				lvSqlWhere += " And tbinterdicao.start_time <= @start_timeFim";
			}
		}

		if(pend_timeInicio > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbinterdicao.end_time >= @end_timeInicio";
			}
			else
			{
				lvSqlWhere += " And tbinterdicao.end_time >= @end_timeInicio";
			}
		}

		if(pend_timeFim > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbinterdicao.end_time <= @end_timeFim";
			}
			else
			{
				lvSqlWhere += " And tbinterdicao.end_time <= @end_timeFim";
			}
		}

		if(pfield_interdictedInicio > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbinterdicao.field_interdicted >= @field_interdictedInicio";
			}
			else
			{
				lvSqlWhere += " And tbinterdicao.field_interdicted >= @field_interdictedInicio";
			}
		}

		if(pfield_interdictedFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbinterdicao.field_interdicted <= @field_interdictedFim";
			}
			else
			{
				lvSqlWhere += " And tbinterdicao.field_interdicted <= @field_interdictedFim";
			}
		}

		if(!string.IsNullOrEmpty(pss_name))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbinterdicao.ss_name like @ss_name";
			}
			else
			{
				lvSqlWhere += " And tbinterdicao.ss_name like @ss_name";
			}
		}

		if(!string.IsNullOrEmpty(pstatus))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbinterdicao.status like @status";
			}
			else
			{
				lvSqlWhere += " And tbinterdicao.status like @status";
			}
		}

		if(!string.IsNullOrEmpty(preason))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbinterdicao.reason like @reason";
			}
			else
			{
				lvSqlWhere += " And tbinterdicao.reason like @reason";
			}
		}

		if(pplan_timeInicio > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbinterdicao.plan_time >= @plan_timeInicio";
			}
			else
			{
				lvSqlWhere += " And tbinterdicao.plan_time >= @plan_timeInicio";
			}
		}

		if(pplan_timeFim > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbinterdicao.plan_time <= @plan_timeFim";
			}
			else
			{
				lvSqlWhere += " And tbinterdicao.plan_time <= @plan_timeFim";
			}
		}

		if(phistInicio > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbinterdicao.hist >= @histInicio";
			}
			else
			{
				lvSqlWhere += " And tbinterdicao.hist >= @histInicio";
			}
		}

		if(phistFim > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbinterdicao.hist <= @histFim";
			}
			else
			{
				lvSqlWhere += " And tbinterdicao.hist <= @histFim";
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

		if(pti_idInicio == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@ti_idInicio", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@ti_idInicio", MySqlDbType.Double).Value = pti_idInicio;
		}

		if(pti_idFim == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@ti_idFim", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@ti_idFim", MySqlDbType.Double).Value = pti_idFim;
		}

		if(pstart_posInicio == Int32.MinValue)
		{
			cmd.Parameters.Add("@start_posInicio", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@start_posInicio", MySqlDbType.Int32).Value = pstart_posInicio;
		}

		if(pstart_posFim == Int32.MinValue)
		{
			cmd.Parameters.Add("@start_posFim", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@start_posFim", MySqlDbType.Int32).Value = pstart_posFim;
		}

		if(pend_posInicio == Int32.MinValue)
		{
			cmd.Parameters.Add("@end_posInicio", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@end_posInicio", MySqlDbType.Int32).Value = pend_posInicio;
		}

		if(pend_posFim == Int32.MinValue)
		{
			cmd.Parameters.Add("@end_posFim", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@end_posFim", MySqlDbType.Int32).Value = pend_posFim;
		}

		if(pstart_timeInicio == DateTime.MinValue)
		{
			cmd.Parameters.Add("@start_timeInicio", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@start_timeInicio", MySqlDbType.DateTime).Value = pstart_timeInicio.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pstart_timeFim == DateTime.MinValue)
		{
			cmd.Parameters.Add("@start_timeFim", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@start_timeFim", MySqlDbType.DateTime).Value = pstart_timeFim.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pend_timeInicio == DateTime.MinValue)
		{
			cmd.Parameters.Add("@end_timeInicio", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@end_timeInicio", MySqlDbType.DateTime).Value = pend_timeInicio.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pend_timeFim == DateTime.MinValue)
		{
			cmd.Parameters.Add("@end_timeFim", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@end_timeFim", MySqlDbType.DateTime).Value = pend_timeFim.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pfield_interdictedInicio == Int16.MinValue)
		{
			cmd.Parameters.Add("@field_interdictedInicio", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@field_interdictedInicio", MySqlDbType.Int16).Value = pfield_interdictedInicio;
		}

		if(pfield_interdictedFim == Int16.MinValue)
		{
			cmd.Parameters.Add("@field_interdictedFim", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@field_interdictedFim", MySqlDbType.Int16).Value = pfield_interdictedFim;
		}

		cmd.Parameters.Add("@ss_name", MySqlDbType.String).Value = "%" + pss_name + "%";

		cmd.Parameters.Add("@status", MySqlDbType.String).Value = "%" + pstatus + "%";

		cmd.Parameters.Add("@reason", MySqlDbType.String).Value = "%" + preason + "%";

		if(pplan_timeInicio == DateTime.MinValue)
		{
			cmd.Parameters.Add("@plan_timeInicio", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@plan_timeInicio", MySqlDbType.DateTime).Value = pplan_timeInicio.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pplan_timeFim == DateTime.MinValue)
		{
			cmd.Parameters.Add("@plan_timeFim", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@plan_timeFim", MySqlDbType.DateTime).Value = pplan_timeFim.ToString("yyyy/MM/dd HH:mm:ss");
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
		adapter.Fill(ds, "tbinterdicao");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetDataByKey(double pti_id = -999999999999, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		lvSql = "select tbinterdicao.ti_id, tbinterdicao.start_pos, tbinterdicao.end_pos, tbinterdicao.start_time, tbinterdicao.end_time, tbinterdicao.field_interdicted, tbinterdicao.ss_name, tbinterdicao.status, tbinterdicao.reason, tbinterdicao.plan_time, tbinterdicao.hist From tbinterdicao";
		if(pti_id > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbinterdicao.ti_id=@ti_id";
			}
			else
			{
				lvSqlWhere += " And tbinterdicao.ti_id=@ti_id";
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

		if(pti_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@ti_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@ti_id", MySqlDbType.Double).Value = pti_id;
		}

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbinterdicao");
		return ds;
	}

}

