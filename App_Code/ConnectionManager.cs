using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using MySql.Data.MySqlClient;
/// <summary>
/// Criado por Eggo Pinheiro em 13/02/2015 11:42:34
/// <summary>

public class ConnectionManager
{
	 public ConnectionManager()
	{
	//
	//TODO: Add constructor logic here
	//
	}

	public static double DOUBLE_REF_VALUE = -999999999999;
	public static double FLOAT_REF_VALUE = -999999999999f;
    private static System.Text.RegularExpressions.Regex mReg = new System.Text.RegularExpressions.Regex(@"\s");

	public static MySqlConnection GetObjConnection()
	{
		string connectionString = ConfigurationManager.ConnectionStrings["MYCONN"].ConnectionString;

		MySqlConnection conn = new MySqlConnection(connectionString);
		conn.Open();

		return conn;
	}

	public static MySqlConnection GetObjConnectionBdUser()
	{
		string connectionString = ConfigurationManager.ConnectionStrings["USERMYCONN"].ConnectionString;

		MySqlConnection conn = new MySqlConnection(connectionString);
		conn.Open();

		return conn;
	}

	public static long GetUTCDateTime(DateTime pValue)
	{
        System.TimeSpan span = new System.TimeSpan(System.DateTime.Parse("1/1/1970").Ticks);
        System.DateTime time = pValue.ToUniversalTime().Subtract(span);

        return (long)(time.Ticks / 10000);
	}

	public static string ToJsonString(string text)
	{
		char[] charArray = text.ToCharArray();
		List<string> output = new List<string>();
		foreach (char c in charArray)
		{
			if (((int)c) == 8)              //Backspace
			{
				output.Add("\\b");
			}
			else if (((int)c) == 9)         //Horizontal tab
			{
				output.Add("\\t");
			}
			else if (((int)c) == 10)        //Newline
			{
				output.Add("\\n");
			}
			else if (((int)c) == 12)        //Formfeed
			{
				output.Add("\\f");
			}
			else if (((int)c) == 13)        //Carriage return
			{
				output.Add("\\n");
			}
			else if (((int)c) == 34)        //Double-quotes
			{
				output.Add("\\" + c.ToString());
			}
			else if (((int)c) == 47)        //Solidus   (/)
			{
				output.Add("\\" + c.ToString());
			}
			else if (((int)c) == 92)        //Reverse solidus   (\)
			{
				output.Add("\\" + c.ToString());
			}
			else if (((int)c) > 31)
			{
                output.Add(c.ToString());
            }
		}
		return string.Join("", output.ToArray());
	}

	public static string GetFlotSerieBlock(string pStrColor, string pStrLabel, DateTime pStartTime, DateTime? pEndTime, double pStartPos, double pEndPos)
	{
	    string lvResult = "";
	    long lvStartTime = 0L;
	    long lvEndTime = 0L;
	    double lvDisplacement = Math.Abs((pEndPos - pStartPos));
	    long lvTimeDisplacement = 0L;

	    lvStartTime = ConnectionManager.GetUTCDateTime(pStartTime);
	    lvEndTime = pEndTime.HasValue ? ConnectionManager.GetUTCDateTime(pEndTime.Value) : ConnectionManager.GetUTCDateTime(DateTime.Now.Date.AddDays(1).AddSeconds(-1));

        pStrLabel = mReg.Replace(pStrLabel.Trim(), " ");
        pStrLabel = pStrLabel.Replace("\"", "\\\"");
        pStrLabel = pStrLabel.Replace("'", "\'");
	    pStrLabel = ConnectionManager.ToJsonString(pStrLabel);

	    lvTimeDisplacement = lvEndTime - lvStartTime;

        lvResult = @"{ ""data"": [[null, null], [@StartTime, @StartPos]], ""color"": ""rgba(0, 0, 0, 0)"", ""lines"": { ""show"": false }, ""points"": { ""show"": false }, ""bars"": { ""show"": false, ""align"": ""left"", ""horizontal"": true }, ""stack"": true }, { ""data"": [[null, null], [@Time, @StartPos]], ""color"": ""@Color"", ""label"": ""@Reason"", ""lines"": { ""show"": false }, ""points"": { ""show"": false }, ""bars"": { ""show"": true, ""align"": ""left"", ""horizontal"": true, ""barWidth"": @Displacement, ""fill"": true, ""fillColor"": ""@Color"" }, ""stack"": true, ""clickable"": true }";

	    lvResult = lvResult.Replace("@StartTime", lvStartTime.ToString());
	    lvResult = lvResult.Replace("@EndTime", lvEndTime.ToString());
	    lvResult = lvResult.Replace("@StartPos", pStartPos.ToString());
	    lvResult = lvResult.Replace("@Time", lvTimeDisplacement.ToString());
	    lvResult = lvResult.Replace("@Reason", pStrLabel);
	    lvResult = lvResult.Replace("@Displacement", lvDisplacement.ToString());
	    lvResult = lvResult.Replace("@Color", pStrColor);

	    return lvResult;
	}

    public static void DebugMySqlQuery(MySqlCommand pCmd, string pStrInfo)
    {
        string lvSql = pCmd.CommandText.ToString();
        string lvStr;

        foreach (MySqlParameter lvParam in pCmd.Parameters)
        {
            lvStr = lvParam.ParameterName;

            if (lvParam.DbType == DbType.DateTime)
            {
                lvSql = lvSql.Replace(lvStr, "'" + DateTime.Parse(lvParam.Value.ToString()).ToString("yyyy/MM/dd HH:mm:ss") + "'");
            }
            else if(lvParam.DbType == DbType.String)
            {
                lvSql = lvSql.Replace(lvStr, "'" + lvParam.Value.ToString() + "'");
            }
            else
            {
                lvSql = lvSql.Replace(lvStr, lvParam.Value.ToString());
            }
        }
        DebugLog.Logar(pStrInfo + " = " + lvSql);

    }
}

