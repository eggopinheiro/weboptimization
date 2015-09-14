using System;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using MySql.Data.MySqlClient;
/// <summary>
/// Criado por Eggo Pinheiro em 12/03/2015 15:53:24
/// <summary>

[DataObject(true)]
public class SpeedrestrictedDataAccess
{
	public SpeedrestrictedDataAccess()
	{
	//
	//TODO: Add constructor logic here
	//
	}

	[DataObjectMethod(DataObjectMethodType.Insert)]
	public static int Insert(double psra_id, int pstart_pos, int pend_pos, DateTime pstart_time, DateTime pend_time, Int16 pflag, string pdirection, int pforward_speed_limit, int pbackward_speed_limit, Int16 pover_switch, string pstatus, string preason, string pstart_pos_desc, string pend_pos_desc, DateTime phist_date, Int16 pprogressive, string pinfo, string pstart_track, string pend_track)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "insert into tbspeedrestricted(sra_id, start_pos, end_pos, start_time, end_time, flag, direction, forward_speed_limit, backward_speed_limit, over_switch, status, reason, start_pos_desc, end_pos_desc, hist_date, progressive, info, start_track, end_track) ";
		lvSql += "values(@sra_id, @start_pos, @end_pos, @start_time, @end_time, @flag, @direction, @forward_speed_limit, @backward_speed_limit, @over_switch, @status, @reason, @start_pos_desc, @end_pos_desc, @hist_date, @progressive, @info, @start_track, @end_track)";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(psra_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@sra_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@sra_id", MySqlDbType.Double).Value = psra_id;
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

		if(pflag == Int16.MinValue)
		{
			cmd.Parameters.Add("@flag", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@flag", MySqlDbType.Int16).Value = pflag;
		}

		cmd.Parameters.Add("@direction", MySqlDbType.String).Value = pdirection;

		if(pforward_speed_limit == Int32.MinValue)
		{
			cmd.Parameters.Add("@forward_speed_limit", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@forward_speed_limit", MySqlDbType.Int32).Value = pforward_speed_limit;
		}

		if(pbackward_speed_limit == Int32.MinValue)
		{
			cmd.Parameters.Add("@backward_speed_limit", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@backward_speed_limit", MySqlDbType.Int32).Value = pbackward_speed_limit;
		}

		if(pover_switch == Int16.MinValue)
		{
			cmd.Parameters.Add("@over_switch", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@over_switch", MySqlDbType.Int16).Value = pover_switch;
		}

		cmd.Parameters.Add("@status", MySqlDbType.String).Value = pstatus;

		cmd.Parameters.Add("@reason", MySqlDbType.String).Value = preason;

		cmd.Parameters.Add("@start_pos_desc", MySqlDbType.String).Value = pstart_pos_desc;

		cmd.Parameters.Add("@end_pos_desc", MySqlDbType.String).Value = pend_pos_desc;

		if(phist_date == DateTime.MinValue)
		{
			cmd.Parameters.Add("@hist_date", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@hist_date", MySqlDbType.DateTime).Value = phist_date.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pprogressive == Int16.MinValue)
		{
			cmd.Parameters.Add("@progressive", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@progressive", MySqlDbType.Int16).Value = pprogressive;
		}

		cmd.Parameters.Add("@info", MySqlDbType.String).Value = pinfo;

		cmd.Parameters.Add("@start_track", MySqlDbType.String).Value = pstart_track;

		cmd.Parameters.Add("@end_track", MySqlDbType.String).Value = pend_track;

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("SpeedrestrictedDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("SpeedrestrictedDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Update)]
	public static int Update(double psra_id, int pstart_pos, int pend_pos, DateTime pstart_time, DateTime pend_time, Int16 pflag, string pdirection, int pforward_speed_limit, int pbackward_speed_limit, Int16 pover_switch, string pstatus, string preason, string pstart_pos_desc, string pend_pos_desc, DateTime phist_date, Int16 pprogressive, string pinfo, string pstart_track, string pend_track)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "update tbspeedrestricted set start_pos=@start_pos, end_pos=@end_pos, start_time=@start_time, end_time=@end_time, flag=@flag, direction=@direction, forward_speed_limit=@forward_speed_limit, backward_speed_limit=@backward_speed_limit, over_switch=@over_switch, status=@status, reason=@reason, start_pos_desc=@start_pos_desc, end_pos_desc=@end_pos_desc, hist_date=@hist_date, progressive=@progressive, info=@info, start_track=@start_track, end_track=@end_track Where sra_id=@sra_id";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(psra_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@sra_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@sra_id", MySqlDbType.Double).Value = psra_id;
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

		if(pflag == Int16.MinValue)
		{
			cmd.Parameters.Add("@flag", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@flag", MySqlDbType.Int16).Value = pflag;
		}

		cmd.Parameters.Add("@direction", MySqlDbType.String).Value = pdirection;
		if(pforward_speed_limit == Int32.MinValue)
		{
			cmd.Parameters.Add("@forward_speed_limit", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@forward_speed_limit", MySqlDbType.Int32).Value = pforward_speed_limit;
		}

		if(pbackward_speed_limit == Int32.MinValue)
		{
			cmd.Parameters.Add("@backward_speed_limit", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@backward_speed_limit", MySqlDbType.Int32).Value = pbackward_speed_limit;
		}

		if(pover_switch == Int16.MinValue)
		{
			cmd.Parameters.Add("@over_switch", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@over_switch", MySqlDbType.Int16).Value = pover_switch;
		}

		cmd.Parameters.Add("@status", MySqlDbType.String).Value = pstatus;
		cmd.Parameters.Add("@reason", MySqlDbType.String).Value = preason;
		cmd.Parameters.Add("@start_pos_desc", MySqlDbType.String).Value = pstart_pos_desc;
		cmd.Parameters.Add("@end_pos_desc", MySqlDbType.String).Value = pend_pos_desc;
		if(phist_date == DateTime.MinValue)
		{
			cmd.Parameters.Add("@hist_date", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@hist_date", MySqlDbType.DateTime).Value = phist_date.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pprogressive == Int16.MinValue)
		{
			cmd.Parameters.Add("@progressive", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@progressive", MySqlDbType.Int16).Value = pprogressive;
		}

		cmd.Parameters.Add("@info", MySqlDbType.String).Value = pinfo;
		cmd.Parameters.Add("@start_track", MySqlDbType.String).Value = pstart_track;
		cmd.Parameters.Add("@end_track", MySqlDbType.String).Value = pend_track;

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("SpeedrestrictedDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("SpeedrestrictedDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Update)]
	public static int UpdateKey(double pOrigsra_id, double psra_id)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "update tbspeedrestricted set sra_id=@sra_id Where sra_id=@origsra_id";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(psra_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@sra_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@sra_id", MySqlDbType.Double).Value = psra_id;
		}

		if(pOrigsra_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@origsra_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@origsra_id", MySqlDbType.Double).Value = pOrigsra_id;
		}

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("SpeedrestrictedDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("SpeedrestrictedDataAccess => (" + lvSql + ") :: " + nullex.ToString());
			throw nullex;
		}

		conn.Close();

		return lvRowAffects;
	}

	[DataObjectMethod(DataObjectMethodType.Delete)]
	public static int Delete(double psra_id)
	{
		string lvSql = "";
		int lvRowAffects = 0;

		lvSql = "delete from tbspeedrestricted Where sra_id=@sra_id";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);

		if(psra_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@sra_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@sra_id", MySqlDbType.Double).Value = psra_id;
		}

		cmd.CommandType = CommandType.Text;

		try
		{
			lvRowAffects = cmd.ExecuteNonQuery();
		}
		catch (MySqlException myex)
		{
			DebugLog.Logar("SpeedrestrictedDataAccess => (" + lvSql + ") :: " + myex.ToString());
			throw myex;
		}
		catch (NullReferenceException nullex)
		{
			DebugLog.Logar("SpeedrestrictedDataAccess => (" + lvSql + ") :: " + nullex.ToString());
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

		lvSql = "select * from tbspeedrestricted";

		MySqlConnection conn = ConnectionManager.GetObjConnection();
		MySqlCommand cmd = new MySqlCommand(lvSql, conn);
		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbspeedrestricted");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetData(double psra_id = -999999999999, int pstart_pos = -2147483648, int pend_pos = -2147483648, DateTime pstart_time = default(DateTime), DateTime pend_time = default(DateTime), Int16 pflag = -32768, string pdirection = "", int pforward_speed_limit = -2147483648, int pbackward_speed_limit = -2147483648, Int16 pover_switch = -32768, string pstatus = "", string preason = "", string pstart_pos_desc = "", string pend_pos_desc = "", DateTime phist_date = default(DateTime), Int16 pprogressive = -32768, string pinfo = "", string pstart_track = "", string pend_track = "", string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		lvSql = "select tbspeedrestricted.sra_id, tbspeedrestricted.start_pos, tbspeedrestricted.end_pos, tbspeedrestricted.start_time, tbspeedrestricted.end_time, tbspeedrestricted.flag, tbspeedrestricted.direction, tbspeedrestricted.forward_speed_limit, tbspeedrestricted.backward_speed_limit, tbspeedrestricted.over_switch, tbspeedrestricted.status, tbspeedrestricted.reason, tbspeedrestricted.start_pos_desc, tbspeedrestricted.end_pos_desc, tbspeedrestricted.hist_date, tbspeedrestricted.progressive, tbspeedrestricted.info, tbspeedrestricted.start_track, tbspeedrestricted.end_track, tbspeedrestricted.distrito From tbspeedrestricted";

		if(psra_id > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.sra_id=@sra_id";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.sra_id=@sra_id";
			}
		}

		if(pstart_pos > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.start_pos=@start_pos";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.start_pos=@start_pos";
			}
		}

		if(pend_pos > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.end_pos=@end_pos";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.end_pos=@end_pos";
			}
		}

		if(pstart_time > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.start_time=@start_time";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.start_time=@start_time";
			}
		}

		if(pend_time > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.end_time=@end_time";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.end_time=@end_time";
			}
		}

		if(pflag > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.flag=@flag";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.flag=@flag";
			}
		}

		if(!string.IsNullOrEmpty(pdirection))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.direction like @direction";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.direction like @direction";
			}
		}

		if(pforward_speed_limit > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.forward_speed_limit=@forward_speed_limit";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.forward_speed_limit=@forward_speed_limit";
			}
		}

		if(pbackward_speed_limit > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.backward_speed_limit=@backward_speed_limit";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.backward_speed_limit=@backward_speed_limit";
			}
		}

		if(pover_switch > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.over_switch=@over_switch";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.over_switch=@over_switch";
			}
		}

		if(!string.IsNullOrEmpty(pstatus))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.status like @status";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.status like @status";
			}
		}

		if(!string.IsNullOrEmpty(preason))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.reason like @reason";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.reason like @reason";
			}
		}

		if(!string.IsNullOrEmpty(pstart_pos_desc))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.start_pos_desc like @start_pos_desc";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.start_pos_desc like @start_pos_desc";
			}
		}

		if(!string.IsNullOrEmpty(pend_pos_desc))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.end_pos_desc like @end_pos_desc";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.end_pos_desc like @end_pos_desc";
			}
		}

		if(phist_date > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.hist_date=@hist_date";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.hist_date=@hist_date";
			}
		}

		if(pprogressive > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.progressive=@progressive";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.progressive=@progressive";
			}
		}

		if(!string.IsNullOrEmpty(pinfo))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.info like @info";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.info like @info";
			}
		}

		if(!string.IsNullOrEmpty(pstart_track))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.start_track like @start_track";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.start_track like @start_track";
			}
		}

		if(!string.IsNullOrEmpty(pend_track))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.end_track like @end_track";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.end_track like @end_track";
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

		if(psra_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@sra_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@sra_id", MySqlDbType.Double).Value = psra_id;
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

		if(pflag == Int16.MinValue)
		{
			cmd.Parameters.Add("@flag", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@flag", MySqlDbType.Int16).Value = pflag;
		}

		cmd.Parameters.Add("@direction", MySqlDbType.String).Value = "%" + pdirection + "%";

		if(pforward_speed_limit == Int32.MinValue)
		{
			cmd.Parameters.Add("@forward_speed_limit", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@forward_speed_limit", MySqlDbType.Int32).Value = pforward_speed_limit;
		}

		if(pbackward_speed_limit == Int32.MinValue)
		{
			cmd.Parameters.Add("@backward_speed_limit", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@backward_speed_limit", MySqlDbType.Int32).Value = pbackward_speed_limit;
		}

		if(pover_switch == Int16.MinValue)
		{
			cmd.Parameters.Add("@over_switch", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@over_switch", MySqlDbType.Int16).Value = pover_switch;
		}

		cmd.Parameters.Add("@status", MySqlDbType.String).Value = "%" + pstatus + "%";

		cmd.Parameters.Add("@reason", MySqlDbType.String).Value = "%" + preason + "%";

		cmd.Parameters.Add("@start_pos_desc", MySqlDbType.String).Value = "%" + pstart_pos_desc + "%";

		cmd.Parameters.Add("@end_pos_desc", MySqlDbType.String).Value = "%" + pend_pos_desc + "%";

		if(phist_date == DateTime.MinValue)
		{
			cmd.Parameters.Add("@hist_date", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@hist_date", MySqlDbType.DateTime).Value = phist_date.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pprogressive == Int16.MinValue)
		{
			cmd.Parameters.Add("@progressive", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@progressive", MySqlDbType.Int16).Value = pprogressive;
		}

		cmd.Parameters.Add("@info", MySqlDbType.String).Value = "%" + pinfo + "%";

		cmd.Parameters.Add("@start_track", MySqlDbType.String).Value = "%" + pstart_track + "%";

		cmd.Parameters.Add("@end_track", MySqlDbType.String).Value = "%" + pend_track + "%";

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbspeedrestricted");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetDataByRange(double psra_idInicio = -999999999999, double psra_idFim = -999999999999, int pstart_posInicio = -2147483648, int pstart_posFim = -2147483648, int pend_posInicio = -2147483648, int pend_posFim = -2147483648, DateTime pstart_timeInicio = default(DateTime), DateTime pstart_timeFim = default(DateTime), DateTime pend_timeInicio = default(DateTime), DateTime pend_timeFim = default(DateTime), Int16 pflagInicio = -32768, Int16 pflagFim = -32768, string pdirection = "", int pforward_speed_limitInicio = -2147483648, int pforward_speed_limitFim = -2147483648, int pbackward_speed_limitInicio = -2147483648, int pbackward_speed_limitFim = -2147483648, Int16 pover_switchInicio = -32768, Int16 pover_switchFim = -32768, string pstatus = "", string preason = "", string pstart_pos_desc = "", string pend_pos_desc = "", DateTime phist_dateInicio = default(DateTime), DateTime phist_dateFim = default(DateTime), Int16 pprogressiveInicio = -32768, Int16 pprogressiveFim = -32768, string pinfo = "", string pstart_track = "", string pend_track = "", Int16 pdistritoInicio = -32768, Int16 pdistritoFim = -32768, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		lvSql = "select tbspeedrestricted.sra_id, tbspeedrestricted.start_pos, tbspeedrestricted.end_pos, tbspeedrestricted.start_time, tbspeedrestricted.end_time, tbspeedrestricted.flag, tbspeedrestricted.direction, tbspeedrestricted.forward_speed_limit, tbspeedrestricted.backward_speed_limit, tbspeedrestricted.over_switch, tbspeedrestricted.status, tbspeedrestricted.reason, tbspeedrestricted.start_pos_desc, tbspeedrestricted.end_pos_desc, tbspeedrestricted.hist_date, tbspeedrestricted.progressive, tbspeedrestricted.info, tbspeedrestricted.start_track, tbspeedrestricted.end_track, tbspeedrestricted.distrito From tbspeedrestricted";

		if(psra_idInicio > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.sra_id >= @sra_idInicio";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.sra_id >= @sra_idInicio";
			}
		}

		if(psra_idFim > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.sra_id <= @sra_idFim";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.sra_id <= @sra_idFim";
			}
		}

		if(pstart_posInicio > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.start_pos >= @start_posInicio";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.start_pos >= @start_posInicio";
			}
		}

		if(pstart_posFim > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.start_pos <= @start_posFim";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.start_pos <= @start_posFim";
			}
		}

		if(pend_posInicio > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.end_pos >= @end_posInicio";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.end_pos >= @end_posInicio";
			}
		}

		if(pend_posFim > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.end_pos <= @end_posFim";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.end_pos <= @end_posFim";
			}
		}

		if(pstart_timeInicio > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.start_time >= @start_timeInicio";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.start_time >= @start_timeInicio";
			}
		}

		if(pstart_timeFim > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.start_time <= @start_timeFim";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.start_time <= @start_timeFim";
			}
		}

		if(pend_timeInicio > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.end_time >= @end_timeInicio";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.end_time >= @end_timeInicio";
			}
		}

		if(pend_timeFim > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.end_time <= @end_timeFim";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.end_time <= @end_timeFim";
			}
		}

		if(pflagInicio > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.flag >= @flagInicio";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.flag >= @flagInicio";
			}
		}

		if(pflagFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.flag <= @flagFim";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.flag <= @flagFim";
			}
		}

		if(!string.IsNullOrEmpty(pdirection))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.direction like @direction";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.direction like @direction";
			}
		}

		if(pforward_speed_limitInicio > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.forward_speed_limit >= @forward_speed_limitInicio";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.forward_speed_limit >= @forward_speed_limitInicio";
			}
		}

		if(pforward_speed_limitFim > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.forward_speed_limit <= @forward_speed_limitFim";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.forward_speed_limit <= @forward_speed_limitFim";
			}
		}

		if(pbackward_speed_limitInicio > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.backward_speed_limit >= @backward_speed_limitInicio";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.backward_speed_limit >= @backward_speed_limitInicio";
			}
		}

		if(pbackward_speed_limitFim > Int32.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.backward_speed_limit <= @backward_speed_limitFim";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.backward_speed_limit <= @backward_speed_limitFim";
			}
		}

		if(pover_switchInicio > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.over_switch >= @over_switchInicio";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.over_switch >= @over_switchInicio";
			}
		}

		if(pover_switchFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.over_switch <= @over_switchFim";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.over_switch <= @over_switchFim";
			}
		}

		if(!string.IsNullOrEmpty(pstatus))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.status like @status";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.status like @status";
			}
		}

		if(!string.IsNullOrEmpty(preason))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.reason like @reason";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.reason like @reason";
			}
		}

		if(!string.IsNullOrEmpty(pstart_pos_desc))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.start_pos_desc like @start_pos_desc";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.start_pos_desc like @start_pos_desc";
			}
		}

		if(!string.IsNullOrEmpty(pend_pos_desc))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.end_pos_desc like @end_pos_desc";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.end_pos_desc like @end_pos_desc";
			}
		}

		if(phist_dateInicio > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.hist_date >= @hist_dateInicio";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.hist_date >= @hist_dateInicio";
			}
		}

		if(phist_dateFim > DateTime.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.hist_date <= @hist_dateFim";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.hist_date <= @hist_dateFim";
			}
		}

		if(pprogressiveInicio > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.progressive >= @progressiveInicio";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.progressive >= @progressiveInicio";
			}
		}

		if(pprogressiveFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.progressive <= @progressiveFim";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.progressive <= @progressiveFim";
			}
		}

		if(!string.IsNullOrEmpty(pinfo))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.info like @info";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.info like @info";
			}
		}

		if(!string.IsNullOrEmpty(pstart_track))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.start_track like @start_track";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.start_track like @start_track";
			}
		}

		if(!string.IsNullOrEmpty(pend_track))
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.end_track like @end_track";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.end_track like @end_track";
			}
		}

		if(pdistritoInicio > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.distrito >= @distritoInicio";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.distrito >= @distritoInicio";
			}
		}

		if(pdistritoFim > Int16.MinValue)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.distrito <= @distritoFim";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.distrito <= @distritoFim";
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

		if(psra_idInicio == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@sra_idInicio", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@sra_idInicio", MySqlDbType.Double).Value = psra_idInicio;
		}

		if(psra_idFim == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@sra_idFim", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@sra_idFim", MySqlDbType.Double).Value = psra_idFim;
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

		if(pflagInicio == Int16.MinValue)
		{
			cmd.Parameters.Add("@flagInicio", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@flagInicio", MySqlDbType.Int16).Value = pflagInicio;
		}

		if(pflagFim == Int16.MinValue)
		{
			cmd.Parameters.Add("@flagFim", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@flagFim", MySqlDbType.Int16).Value = pflagFim;
		}

		cmd.Parameters.Add("@direction", MySqlDbType.String).Value = "%" + pdirection + "%";

		if(pforward_speed_limitInicio == Int32.MinValue)
		{
			cmd.Parameters.Add("@forward_speed_limitInicio", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@forward_speed_limitInicio", MySqlDbType.Int32).Value = pforward_speed_limitInicio;
		}

		if(pforward_speed_limitFim == Int32.MinValue)
		{
			cmd.Parameters.Add("@forward_speed_limitFim", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@forward_speed_limitFim", MySqlDbType.Int32).Value = pforward_speed_limitFim;
		}

		if(pbackward_speed_limitInicio == Int32.MinValue)
		{
			cmd.Parameters.Add("@backward_speed_limitInicio", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@backward_speed_limitInicio", MySqlDbType.Int32).Value = pbackward_speed_limitInicio;
		}

		if(pbackward_speed_limitFim == Int32.MinValue)
		{
			cmd.Parameters.Add("@backward_speed_limitFim", MySqlDbType.Int32).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@backward_speed_limitFim", MySqlDbType.Int32).Value = pbackward_speed_limitFim;
		}

		if(pover_switchInicio == Int16.MinValue)
		{
			cmd.Parameters.Add("@over_switchInicio", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@over_switchInicio", MySqlDbType.Int16).Value = pover_switchInicio;
		}

		if(pover_switchFim == Int16.MinValue)
		{
			cmd.Parameters.Add("@over_switchFim", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@over_switchFim", MySqlDbType.Int16).Value = pover_switchFim;
		}

		cmd.Parameters.Add("@status", MySqlDbType.String).Value = "%" + pstatus + "%";

		cmd.Parameters.Add("@reason", MySqlDbType.String).Value = "%" + preason + "%";

		cmd.Parameters.Add("@start_pos_desc", MySqlDbType.String).Value = "%" + pstart_pos_desc + "%";

		cmd.Parameters.Add("@end_pos_desc", MySqlDbType.String).Value = "%" + pend_pos_desc + "%";

		if(phist_dateInicio == DateTime.MinValue)
		{
			cmd.Parameters.Add("@hist_dateInicio", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@hist_dateInicio", MySqlDbType.DateTime).Value = phist_dateInicio.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(phist_dateFim == DateTime.MinValue)
		{
			cmd.Parameters.Add("@hist_dateFim", MySqlDbType.DateTime).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@hist_dateFim", MySqlDbType.DateTime).Value = phist_dateFim.ToString("yyyy/MM/dd HH:mm:ss");
		}

		if(pprogressiveInicio == Int16.MinValue)
		{
			cmd.Parameters.Add("@progressiveInicio", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@progressiveInicio", MySqlDbType.Int16).Value = pprogressiveInicio;
		}

		if(pprogressiveFim == Int16.MinValue)
		{
			cmd.Parameters.Add("@progressiveFim", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@progressiveFim", MySqlDbType.Int16).Value = pprogressiveFim;
		}

		cmd.Parameters.Add("@info", MySqlDbType.String).Value = "%" + pinfo + "%";

		cmd.Parameters.Add("@start_track", MySqlDbType.String).Value = "%" + pstart_track + "%";

		cmd.Parameters.Add("@end_track", MySqlDbType.String).Value = "%" + pend_track + "%";

		if(pdistritoInicio == Int16.MinValue)
		{
			cmd.Parameters.Add("@distritoInicio", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@distritoInicio", MySqlDbType.Int16).Value = pdistritoInicio;
		}

		if(pdistritoFim == Int16.MinValue)
		{
			cmd.Parameters.Add("@distritoFim", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@distritoFim", MySqlDbType.Int16).Value = pdistritoFim;
		}

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbspeedrestricted");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetDataByKey(double psra_id = -999999999999, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		lvSql = "select tbspeedrestricted.sra_id, tbspeedrestricted.start_pos, tbspeedrestricted.end_pos, tbspeedrestricted.start_time, tbspeedrestricted.end_time, tbspeedrestricted.flag, tbspeedrestricted.direction, tbspeedrestricted.forward_speed_limit, tbspeedrestricted.backward_speed_limit, tbspeedrestricted.over_switch, tbspeedrestricted.status, tbspeedrestricted.reason, tbspeedrestricted.start_pos_desc, tbspeedrestricted.end_pos_desc, tbspeedrestricted.hist_date, tbspeedrestricted.progressive, tbspeedrestricted.info, tbspeedrestricted.start_track, tbspeedrestricted.end_track, tbspeedrestricted.distrito From tbspeedrestricted";
		if(psra_id > ConnectionManager.DOUBLE_REF_VALUE)
		{
			if(lvSqlWhere.Length == 0)
			{
				lvSqlWhere += "tbspeedrestricted.sra_id=@sra_id";
			}
			else
			{
				lvSqlWhere += " And tbspeedrestricted.sra_id=@sra_id";
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

		if(psra_id == ConnectionManager.DOUBLE_REF_VALUE)
		{
			cmd.Parameters.Add("@sra_id", MySqlDbType.Double).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@sra_id", MySqlDbType.Double).Value = psra_id;
		}

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbspeedrestricted");
		return ds;
	}

	[DataObjectMethod(DataObjectMethodType.Select)]
	public static DataSet GetDataBySrdistrict(Int16 pdistrito = -32768, string pStrSortField = "")
	{
		string lvSql = "";
		string lvSqlWhere = "";
		DataSet ds = new DataSet();

		lvSql = "select tbspeedrestricted.sra_id, tbspeedrestricted.start_pos, tbspeedrestricted.end_pos, tbspeedrestricted.start_time, tbspeedrestricted.end_time, tbspeedrestricted.flag, tbspeedrestricted.direction, tbspeedrestricted.forward_speed_limit, tbspeedrestricted.backward_speed_limit, tbspeedrestricted.over_switch, tbspeedrestricted.status, tbspeedrestricted.reason, tbspeedrestricted.start_pos_desc, tbspeedrestricted.end_pos_desc, tbspeedrestricted.hist_date, tbspeedrestricted.progressive, tbspeedrestricted.info, tbspeedrestricted.start_track, tbspeedrestricted.end_track, tbspeedrestricted.distrito From tbspeedrestricted";
		lvSqlWhere = "tbspeedrestricted.distrito=@distrito";

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

		if(pdistrito == Int16.MinValue)
		{
			cmd.Parameters.Add("@distrito", MySqlDbType.Int16).Value = DBNull.Value;
		}
		else
		{
			cmd.Parameters.Add("@distrito", MySqlDbType.Int16).Value = pdistrito;
		}

		cmd.CommandType = CommandType.Text;

		conn.Close();

		MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
		adapter.Fill(ds, "tbspeedrestricted");
		return ds;
	}

}

