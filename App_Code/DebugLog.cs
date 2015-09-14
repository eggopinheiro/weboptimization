using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.IO;
/// <summary>
/// Criado por Eggo Pinheiro em 09/09/2014 15:42:21
/// <summary>

public class DebugLog
{
    public static bool mEnable = true;

	 public DebugLog()
	{
	//
	//TODO: Add constructor logic here
	//
	}

	public static bool Logar(string strInfo)
	{
		bool result = false;
		string fileName = "";
		StreamWriter tw = null;
        FileStream lvFileStrem;

        if (mEnable)
        {
            try
            {
                fileName = ConfigurationManager.AppSettings["LOG_FILE_NAME"] + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "_" + DateTime.Now.Hour;
                lvFileStrem = new FileStream(HttpContext.Current.Server.MapPath("") + "/Logs/" + fileName + ".txt", FileMode.Append, FileAccess.Write, FileShare.Write);
                tw = new StreamWriter(lvFileStrem);

                strInfo = DateTime.Now + " => " + strInfo;
                tw.WriteLine(strInfo);
                result = true;
                tw.Close();
            }
            catch (Exception ex)
            { }
        }
		return result;
	}	
    
    public static bool Logar(string strInfo, string strFileName)
	{
		bool result = false;
		string fileName = "";
		StreamWriter tw = null;

		fileName = strFileName + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour;
		tw = new StreamWriter(HttpContext.Current.Server.MapPath("") + "/Logs/" + fileName + ".txt", true, System.Text.Encoding.Default);

		strInfo = DateTime.Now + " => " + strInfo;
		tw.WriteLine(strInfo);
		result = true;
		tw.Close();

		return result;
	}
	public static bool Logar(string strInfo, string strFileName, string strExt)
	{
		bool result = false;
		string fileName = "";
		StreamWriter tw = null;

		fileName = strFileName + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour;
		tw = new StreamWriter(HttpContext.Current.Server.MapPath("") + "/Logs/" + fileName + "." + strExt, true, System.Text.Encoding.Default);

		strInfo = DateTime.Now + " => " + strInfo;
		tw.WriteLine(strInfo);
		result = true;
		tw.Close();

		return result;
	}
}

