Option Explicit

Dim objWs
Dim objFSO
Dim objConn
Dim strConn
Dim strDestPath
Dim strDestFile
Dim strInitDate
Dim strEndDate
Dim vetRegioes()
Dim countRegioes

strDestPath = "C:\Users\eggo\Projetos\GAWebOptimization\"
'strDestPath = "\\172.20.8.14\sgf\"
'strDestFile = strDestPath & "rdo.mdb"
strDestFile = "bdsgf"

strInitDate = GetFormatedDateYMDOnly(CDate(Now() + 1)) & " 00:00:00"
strEndDate = GetFormatedDateYMDOnly(CDate(Now() + 1)) & " 23:59:59"

Class Regiao

	' declare private class variable
	Private coordInicio
	Private coordFim
	Private lvNome

	' declare the property
	Public Property Get Nome()
	  Nome = lvNome
	End Property

	Public Property Let Nome(strNome)
	  lvNome = strNome
	End Property

	Public Property Get Inicio()
	  Inicio = coordInicio
	End Property

	Public Property Let Inicio(strInicio)
	  coordInicio = strInicio
	End Property

	Public Property Get Fim()
	  Fim = coordFim
	End Property

	Public Property Let Fim(strFim)
	  coordFim = strFim
	End Property
	
	' retorn 0 se a coordinate estiver no intervalo da região
	' retorn 1 se a coordinate for maior que a região
	' retorn -1 se a coordinate for menor que a região
	Function Compara(coordinate)
		If (CLng(coordinate) >= CLng(coordInicio)) And (CLng(coordinate) < CLng(coordFim)) Then
			Compara = 0
		ElseIf (CLng(coordinate) > CLng(coordFim)) Then
			Compara = 1
		ElseIf (CLng(coordinate) < CLng(coordInicio)) Then
			Compara = -1
		End If 
	End Function
	
	Function ToString()
		ToString = "" & lvNome & ": ( " & coordInicio & " , " & coordFim & " )"
	End Function
	
End Class

Call Main()

Sub Main()

	Dim lvRes
	Dim countMin
	
	Set objWs = CreateObject("Wscript.Shell")
	Set objFSO = CreateObject("Scripting.FileSystemObject")
	
	'   lvRes = ObjWs.Run("%comspec% /c smartrain console TrainPlanList >> f:\sistemas\scripts\sgf\log_plan_" & GetFormatedDateOnlyYMDNum(Now()) & ".txt", 0, True)

'		If (countMin >= 1) Then
	PerformMain()
'			countMin = 0
			
	Set objWs = Nothing
	Set objFSO = Nothing
	
End Sub

Sub PerformMain()

	Dim lvStrConn
	Dim lvRsData
	Dim lvSql
	Dim pStrPlanId
	Dim lvStrTrainId

	Set objConn = CreateObject("ADODB.Connection")

	strConn = "Driver={MySQL ODBC 5.1 Driver};server=localhost;uid=root;pwd=root;database=bdsgf"

	'    strConn = "Provider=MySQLProv;Data Source=" & strDestFile
	'    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & strDestFile & ";Persist Security Info=False"

	'    MsgBox strConnExt

	On Error Resume Next
	objConn.Open strConn
	On Error Goto 0

	If Err.Number <> 0 Then
		Log Now() & " - Erro (" & strConn & ") " & Err.Number & ": " & Err.Description
		Err.Clear

		On Error Resume Next
		objConn.Close
		On Error Goto 0
	Else
		Err.Clear

		ImportData

		On Error Resume Next
		objConn.Close
		On Error Goto 0
	End If
	On Error GoTo 0

	Set objConn = Nothing
	'    Set objWs = Nothing

End Sub

Private Sub ImportData()

	Dim lvRsData
	Dim lvStrTrainId
	Dim lvStrPMTId
	Dim lvSql
	Dim lvInd
	Dim lvCount

	Log "Início de ImportData..."

	Set lvRsData = CreateObject("ADODB.Recordset")

	lvSql = "select train_id, pmt_id from tbtraintemp where (pmt_id is not null) And (departure_time = '0000-00-00 00:00:00' or departure_time > '2015/02/26 00:00:00' or creation_tm > '2015/02/26 00:00:00') order by departure_time asc"
	lvRsData.Open lvSql, objConn, 0, 1

	Do While Not lvRsData.EOF
		lvStrTrainId = "0"
		If Not IsNull(lvRsData("train_id")) Then
			lvStrTrainId = lvRsData("train_id")
		End If

		lvStrPMTId = ""
		If Not IsNull(lvRsData("pmt_id")) Then
			lvStrPMTId = lvRsData("pmt_id")
		End If

		lvSql = "update tbtrain set pmt_id = '" & lvStrPMTId & "' where train_id = " & lvStrTrainId
		Log "lvSql = " & lvSql
		objConn.Execute lvSql
		
		lvRsData.MoveNext
		Wscript.sleep 5
	Loop
	lvRsData.Close

	Log ""
	Log " ------------------------ Fim --------------------------- "
	
	Set lvRsData = Nothing

End Sub

Private Function GetFormatedDateYMD(pDate)

    Dim lvStrDia
    Dim lvStrMes
    Dim lvStrAno
    Dim lvStrHora
    Dim lvStrMin
    Dim lvStrSec
    Dim lvDia
    Dim lvMes
    Dim lvAno
    Dim lvHora
    Dim lvMin
    Dim lvSec

	If isNull(pDate) Then
	   GetFormatedDateYMD = "0"
	   Exit Function
	End If

	If Not isDate(pDate) Then
	   GetFormatedDateYMD = "0"
	   Exit Function
	End If
	
    If Len(Trim(pDate)) = 0 Then
       GetFormatedDateYMD = "0"
       Exit Function
    End If

    lvDia = DatePart("d", pDate)
    If lvDia < 10 Then
        lvStrDia = "0" & lvDia
    Else
        lvStrDia = lvDia
    End If

    lvMes = DatePart("m", pDate)
    If lvMes < 10 Then
        lvStrMes = "0" & lvMes
    Else
        lvStrMes = lvMes
    End If

    lvHora = DatePart("h", pDate)
    If lvHora < 10 Then
        lvStrHora = "0" & lvHora
    Else
        lvStrHora = lvHora
    End If

    lvMin = DatePart("n", pDate)
    If lvMin < 10 Then
        lvStrMin = "0" & lvMin
    Else
        lvStrMin = lvMin
    End If

    lvSec = DatePart("s", pDate)
    If lvSec < 10 Then
        lvStrSec = "0" & lvSec
    Else
        lvStrSec = lvSec
    End If

    GetFormatedDateYMD = DatePart("yyyy", pDate) & "/" & lvStrMes & "/" & lvStrDia & " " & lvStrHora & ":" & lvStrMin & ":" & lvStrSec

End Function

Private Function GetFormatedDateT(pDate)

    Dim lvStrDia
    Dim lvStrMes
    Dim lvStrAno
    Dim lvStrHora
    Dim lvStrMin
    Dim lvStrSec
    Dim lvDia
    Dim lvMes
    Dim lvAno
    Dim lvHora
    Dim lvMin
    Dim lvSec

	If isNull(pDate) Then
	   GetFormatedDateT = "0"
	   Exit Function
	End If

	If Not isDate(pDate) Then
	   GetFormatedDateT = "0"
	   Exit Function
	End If
	
    If Len(Trim(pDate)) = 0 Then
       GetFormatedDateT = "0"
       Exit Function
    End If

    lvDia = DatePart("d", pDate)
    If lvDia < 10 Then
        lvStrDia = "0" & lvDia
    Else
        lvStrDia = lvDia
    End If

    lvMes = DatePart("m", pDate)
    If lvMes < 10 Then
        lvStrMes = "0" & lvMes
    Else
        lvStrMes = lvMes
    End If

    lvMes = DatePart("yyyy", pDate)
    If lvAno < 10 Then
        lvStrAno = "0" & lvAno
    Else
        lvStrAno = lvAno
    End If

    lvHora = DatePart("h", pDate)
    If lvHora < 10 Then
        lvStrHora = "0" & lvHora
    Else
        lvStrHora = lvHora
    End If

    lvMin = DatePart("n", pDate)
    If lvMin < 10 Then
        lvStrMin = "0" & lvMin
    Else
        lvStrMin = lvMin
    End If

    lvSec = DatePart("s", pDate)
    If lvSec < 10 Then
        lvStrSec = "0" & lvSec
    Else
        lvStrSec = lvSec
    End If

    GetFormatedDateT = DatePart("yyyy", pDate) & "-" & lvStrMes & "-" & lvStrDia & "T" & lvStrHora & ":" & lvStrMin & ":" & lvStrSec

End Function

Private Function GetFormatedDateYMDOnly(pDate)

    Dim lvStrDia
    Dim lvStrMes
    Dim lvStrAno
    Dim lvStrHora
    Dim lvStrMin
    Dim lvStrSec
    Dim lvDia
    Dim lvMes
    Dim lvAno
    Dim lvHora
    Dim lvMin
    Dim lvSec

	If isNull(pDate) Then
	   GetFormatedDateYMD = "0"
	   Exit Function
	End If

	If Not isDate(pDate) Then
	   GetFormatedDateYMD = "0"
	   Exit Function
	End If
	
    If Len(Trim(pDate)) = 0 Then
       GetFormatedDateYMD = "0"
       Exit Function
    End If

    lvDia = DatePart("d", pDate)
    If lvDia < 10 Then
        lvStrDia = "0" & lvDia
    Else
        lvStrDia = lvDia
    End If

    lvMes = DatePart("m", pDate)
    If lvMes < 10 Then
        lvStrMes = "0" & lvMes
    Else
        lvStrMes = lvMes
    End If

    lvMes = DatePart("yyyy", pDate)
    If lvAno < 10 Then
        lvStrAno = "0" & lvAno
    Else
        lvStrAno = lvAno
    End If

    lvHora = DatePart("h", pDate)
    If lvHora < 10 Then
        lvStrHora = "0" & lvHora
    Else
        lvStrHora = lvHora
    End If

    lvMin = DatePart("n", pDate)
    If lvMin < 10 Then
        lvStrMin = "0" & lvMin
    Else
        lvStrMin = lvMin
    End If

    lvSec = DatePart("s", pDate)
    If lvSec < 10 Then
        lvStrSec = "0" & lvSec
    Else
        lvStrSec = lvSec
    End If

    GetFormatedDateYMDOnly = DatePart("yyyy", pDate) & "/" & lvStrMes & "/" & lvStrDia

End Function

Private Function GetFormatedDate(pDate)

    Dim lvStrDia
    Dim lvStrMes
    Dim lvStrAno
    Dim lvStrHora
    Dim lvStrMin
    Dim lvStrSec
    Dim lvDia
    Dim lvMes
    Dim lvAno
    Dim lvHora
    Dim lvMin
    Dim lvSec

    If Len(Trim(pDate)) = 0 Then
       GetFormatedDate = "0"
       Exit Function
    End If

    lvDia = DatePart("d", pDate)
    If lvDia < 10 Then
        lvStrDia = "0" & lvDia
    Else
        lvStrDia = lvDia
    End If

    lvMes = DatePart("m", pDate)
    If lvMes < 10 Then
        lvStrMes = "0" & lvMes
    Else
        lvStrMes = lvMes
    End If

    lvMes = DatePart("yyyy", pDate)
    If lvAno < 10 Then
        lvStrAno = "0" & lvAno
    Else
        lvStrAno = lvAno
    End If

    lvHora = DatePart("h", pDate)
    If lvHora < 10 Then
        lvStrHora = "0" & lvHora
    Else
        lvStrHora = lvHora
    End If

    lvMin = DatePart("n", pDate)
    If lvMin < 10 Then
        lvStrMin = "0" & lvMin
    Else
        lvStrMin = lvMin
    End If

    lvSec = DatePart("s", pDate)
    If lvSec < 10 Then
        lvStrSec = "0" & lvSec
    Else
        lvStrSec = lvSec
    End If

    GetFormatedDate = lvStrDia & "/" & lvStrMes & "/" & DatePart("yyyy", pDate) & " " & lvStrHora & ":" & lvStrMin & ":" & lvStrSec

End Function

Private Function GetFormatedHour(pDate)

    Dim lvStrDia
    Dim lvStrMes
    Dim lvStrAno
    Dim lvStrHora
    Dim lvStrMin
    Dim lvStrSec
    Dim lvDia
    Dim lvMes
    Dim lvAno
    Dim lvHora
    Dim lvMin
    Dim lvSec

    If Len(Trim(pDate)) = 0 Then
       GetFormatedDate = "0"
       Exit Function
    End If

    lvDia = DatePart("d", pDate)
    If lvDia < 10 Then
        lvStrDia = "0" & lvDia
    Else
        lvStrDia = lvDia
    End If

    lvMes = DatePart("m", pDate)
    If lvMes < 10 Then
        lvStrMes = "0" & lvMes
    Else
        lvStrMes = lvMes
    End If

    lvMes = DatePart("yyyy", pDate)
    If lvAno < 10 Then
        lvStrAno = "0" & lvAno
    Else
        lvStrAno = lvAno
    End If

    lvHora = DatePart("h", pDate)
    If lvHora < 10 Then
        lvStrHora = "0" & lvHora
    Else
        lvStrHora = lvHora
    End If

    lvMin = DatePart("n", pDate)
    If lvMin < 10 Then
        lvStrMin = "0" & lvMin
    Else
        lvStrMin = lvMin
    End If

    lvSec = DatePart("s", pDate)
    If lvSec < 10 Then
        lvStrSec = "0" & lvSec
    Else
        lvStrSec = lvSec
    End If

    GetFormatedHour = lvStrHora & ":" & lvStrMin & ":" & lvStrSec

End Function

Private Function GetFormatedDateTrunc(pDate)

    Dim lvStrDia
    Dim lvStrMes
    Dim lvStrAno
    Dim lvStrHora
    Dim lvStrMin
    Dim lvStrSec
    Dim lvDia
    Dim lvMes
    Dim lvAno
    Dim lvHora
    Dim lvMin
    Dim lvSec

    If Len(Trim(pDate)) = 0 Then
       GetFormatedDateTrunc = "0"
       Exit Function
    End If

    lvDia = DatePart("d", pDate)
    If lvDia < 10 Then
        lvStrDia = "0" & lvDia
    Else
        lvStrDia = lvDia
    End If

    lvMes = DatePart("m", pDate)
    If lvMes < 10 Then
        lvStrMes = "0" & lvMes
    Else
        lvStrMes = lvMes
    End If

    lvMes = DatePart("yyyy", pDate)
    If lvAno < 10 Then
        lvStrAno = "0" & lvAno
    Else
        lvStrAno = lvAno
    End If

    lvHora = DatePart("h", pDate)
    If lvHora < 10 Then
        lvStrHora = "0" & lvHora
    Else
        lvStrHora = lvHora
    End If

    lvMin = DatePart("n", pDate)
    If lvMin < 10 Then
        lvStrMin = "0" & lvMin
    Else
        lvStrMin = lvMin
    End If

    lvSec = DatePart("s", pDate)
    If lvSec < 10 Then
        lvStrSec = "0" & lvSec
    Else
        lvStrSec = lvSec
    End If

'    GetFormatedDateTrunc = lvStrDia & "/" & lvStrMes & "/" & DatePart("yyyy", pDate) & lvStrHora & ":" & lvStrMin & ":" & lvStrSec
    GetFormatedDateTrunc = lvStrDia & "/" & lvStrMes & "/" & DatePart("yyyy", pDate) & lvStrHora & ":" & lvStrMin & ":00"

End Function

Private Function GetFormatedDateOnly(pDate)

    Dim lvStrDia
    Dim lvStrMes
    Dim lvStrAno
    Dim lvDia
    Dim lvMes
    Dim lvAno

    If Len(Trim(pDate)) = 0 Then
       GetFormatedDateOnly = "0"
       Exit Function
    End If

    lvDia = DatePart("d", pDate)
    If lvDia < 10 Then
        lvStrDia = "0" & lvDia
    Else
        lvStrDia = lvDia
    End If

    lvMes = DatePart("m", pDate)
    If lvMes < 10 Then
        lvStrMes = "0" & lvMes
    Else
        lvStrMes = lvMes
    End If

    lvMes = DatePart("yyyy", pDate)
    If lvAno < 10 Then
        lvStrAno = "0" & lvAno
    Else
        lvStrAno = lvAno
    End If

    GetFormatedDateOnly = lvStrDia & "/" & lvStrMes & "/" & DatePart("yyyy", pDate)

End Function

Public Sub Log(strInfo)

    Dim fs
    Dim lvTextFile

'    On error resume next

    Set fs = CreateObject("Scripting.FileSystemObject")

    Set lvTextFile = fs.OpenTextFile(strDestPath & "log_import_" & GetFormatedDateOnlyYMDHourNum(Now()) & ".txt", 8, True)

    lvTextFile.WriteLine Now() & " => " & strInfo

    lvTextFile.Close
    Set lvTextFile = Nothing
    Set fs = Nothing
    
End Sub

Private Function GetTimeStamp()

    Dim lvStrDia
    Dim lvStrMes
    Dim lvStrAno
    Dim lvDia
    Dim lvMes
    Dim lvAno
	 Dim lvSec

    lvDia = DatePart("y", Now())
    If lvDia < 10 Then
        lvStrDia = "0" & lvDia
    Else
        lvStrDia = lvDia
    End If

    lvSec = CLng(Timer * 1000)

    GetTimeStamp = DatePart("yyyy", Now()) & lvStrDia & lvSec

End Function

Private Function GetFormatedDateYMDNum(pDate)

    Dim lvStrDia
    Dim lvStrMes
    Dim lvStrAno
    Dim lvStrHora
    Dim lvStrMin
    Dim lvStrSec
    Dim lvDia
    Dim lvMes
    Dim lvAno
    Dim lvHora
    Dim lvMin
    Dim lvSec

    If Len(Trim(pDate)) = 0 Then
       GetFormatedDateYMDNum = "0"
       Exit Function
    End If

    lvDia = DatePart("d", pDate)
    If lvDia < 10 Then
        lvStrDia = "0" & lvDia
    Else
        lvStrDia = lvDia
    End If

    lvMes = DatePart("m", pDate)
    If lvMes < 10 Then
        lvStrMes = "0" & lvMes
    Else
        lvStrMes = lvMes
    End If

    lvMes = DatePart("yyyy", pDate)
    If lvAno < 10 Then
        lvStrAno = "0" & lvAno
    Else
        lvStrAno = lvAno
    End If

    lvHora = DatePart("h", pDate)
    If lvHora < 10 Then
        lvStrHora = "0" & lvHora
    Else
        lvStrHora = lvHora
    End If

    lvMin = DatePart("n", pDate)
    If lvMin < 10 Then
        lvStrMin = "0" & lvMin
    Else
        lvStrMin = lvMin
    End If

    lvSec = DatePart("s", pDate)
    If lvSec < 10 Then
        lvStrSec = "0" & lvSec
    Else
        lvStrSec = lvSec
    End If

    GetFormatedDateYMDNum = DatePart("yyyy", pDate) & lvStrMes & lvStrDia & lvStrHora & lvStrMin & lvStrSec

End Function

Private Function GetFormatedDateYMDNumParc(pDate)

    Dim lvStrDia
    Dim lvStrMes
    Dim lvStrAno
    Dim lvStrHora
    Dim lvStrMin
    Dim lvStrSec
    Dim lvDia
    Dim lvMes
    Dim lvAno
    Dim lvHora
    Dim lvMin
    Dim lvSec

    If Len(Trim(pDate)) = 0 Then
       GetFormatedDateYMDNumParc = "0"
       Exit Function
    End If

    lvDia = DatePart("d", pDate)
    If lvDia < 10 Then
        lvStrDia = "0" & lvDia
    Else
        lvStrDia = lvDia
    End If

    lvMes = DatePart("m", pDate)
    If lvMes < 10 Then
        lvStrMes = "0" & lvMes
    Else
        lvStrMes = lvMes
    End If

    lvMes = DatePart("yyyy", pDate)
    If lvAno < 10 Then
        lvStrAno = "0" & lvAno
    Else
        lvStrAno = lvAno
    End If

    lvHora = DatePart("h", pDate)
    If lvHora < 10 Then
        lvStrHora = "0" & lvHora
    Else
        lvStrHora = lvHora
    End If

    lvMin = DatePart("n", pDate)
    If lvMin < 10 Then
        lvStrMin = "0" & lvMin
    Else
        lvStrMin = lvMin
    End If

    lvSec = DatePart("s", pDate)
    If lvSec < 10 Then
        lvStrSec = "0" & lvSec
    Else
        lvStrSec = lvSec
    End If

    GetFormatedDateYMDNumParc = DatePart("yyyy", pDate) & lvStrMes & lvStrDia & lvStrHora & ":" & lvStrMin & ":" & lvStrSec

End Function

Private Function GetFormatedDateOnlyYMDHourNum(pDate)

    Dim lvStrDia
    Dim lvStrMes
    Dim lvStrAno
    Dim lvStrHora
    Dim lvStrMin
    Dim lvStrSec
    Dim lvDia
    Dim lvMes
    Dim lvAno
    Dim lvHora
    Dim lvMin
    Dim lvSec

    If (Len(Trim(pDate)) > 0) And (Not isDate(pDate)) Then
       GetFormatedDateOnlyYMDHourNum = ""
       Exit Function
    End If

    lvDia = DatePart("d", pDate)
    If lvDia < 10 Then
        lvStrDia = "0" & lvDia
    Else
        lvStrDia = lvDia
    End If

    lvMes = DatePart("m", pDate)
    If lvMes < 10 Then
        lvStrMes = "0" & lvMes
    Else
        lvStrMes = lvMes
    End If

    lvMes = DatePart("yyyy", pDate)
    If lvAno < 10 Then
        lvStrAno = "0" & lvAno
    Else
        lvStrAno = lvAno
    End If
	 
	 lvHora = DatePart("h", pDate)
    If lvHora < 10 Then
        lvStrHora = "0" & lvHora
    Else
        lvStrHora = lvHora
    End If

    GetFormatedDateOnlyYMDHourNum = DatePart("yyyy", pDate) & lvStrMes & lvStrDia & lvStrHora

End Function

Private Function GetFormatedDateOnlyYMDNum(pDate)

    Dim lvStrDia
    Dim lvStrMes
    Dim lvStrAno
    Dim lvStrHora
    Dim lvStrMin
    Dim lvStrSec
    Dim lvDia
    Dim lvMes
    Dim lvAno
    Dim lvHora
    Dim lvMin
    Dim lvSec

    If (Len(Trim(pDate)) > 0) And (Not isDate(pDate)) Then
       GetFormatedDateOnlyYMDNum = ""
       Exit Function
    End If

    lvDia = DatePart("d", pDate)
    If lvDia < 10 Then
        lvStrDia = "0" & lvDia
    Else
        lvStrDia = lvDia
    End If

    lvMes = DatePart("m", pDate)
    If lvMes < 10 Then
        lvStrMes = "0" & lvMes
    Else
        lvStrMes = lvMes
    End If

    lvMes = DatePart("yyyy", pDate)
    If lvAno < 10 Then
        lvStrAno = "0" & lvAno
    Else
        lvStrAno = lvAno
    End If

    GetFormatedDateOnlyYMDNum = DatePart("yyyy", pDate) & lvStrMes & lvStrDia

End Function

Private Function GetTrainLen(pStrTrainId, pOutWeight)

	Dim lvSql
	Dim lvRsData
	Dim lvStrSerie
	Dim lvLenght
	Dim lvWeight
	
	lvLenght = 0
	lvWeight = 0

	Set lvRsData = CreateObject("ADODB.Recordset")
	
	lvSql = "select pmt_id, tipo, serie, peso_ind, date_hist from tbtraincompo where (train_id = " & pStrTrainId & ") Order by date_hist"
	Log "lvSql GetTrainLen = " & lvSql
	
	On Error Resume Next
	lvRsData.Open lvSql, objConn, 0, 1
	
	If Err.Number <> 0 Then
	  Log Now() & " - (" & lvSql & ") Erro " & Err.Number & ": " & Err.Description
	  GetTrainLen = "0"
	  pOutWeight = lvWeight
	  Exit Function
	End If
	On Error Goto 0

	Do While Not lvRsData.EOF
		If Not isNull(lvRsData("serie")) Then
			lvStrSerie = CStr(lvRsData("serie"))
			If objDicCar.Exists(lvStrSerie) Then
				lvLenght = lvLenght + CDbl(objDicCar(lvStrSerie))
			End If
		End If
		
		If lvRsData("tipo") = "COMPOVAGAO" Then
			lvWeight = lvWeight + CLng(lvRsData("peso_ind") * 1000000)
		ElseIf lvRsData("tipo") = "COMPOLOCOS" Then
			lvWeight = lvWeight + (LOCO_WEIGHT * 1000)
		End If
		lvRsData.MoveNext
	Loop
	lvRsData.Close
	Wscript.sleep 10
	
	Set lvRsData = Nothing

	pOutWeight = lvWeight
	
	GetTrainLen = lvLenght
	
End Function

Private Function GetTrainIdCFlexUsed(pStrTrainId)

	Dim lvSql
	Dim lvRsData
	Dim lvRes

	Set lvRsData = CreateObject("ADODB.Recordset")
	
	lvSql = "select * from tbtraincflexused where train_id = " & pStrTrainId
	
	On Error Resume Next
	lvRsData.Open lvSql, objConn, 0, 1

	If Err.Number <> 0 Then
		Log Now() & " - GetTrainIdCFlexUsed.Erro (" & lvSql & ") " & Err.Number & ": " & Err.Description
		GetTrainIdCFlexUsed = ""
		Exit Function
	End If
	
	lvRes = ""
	If Not (lvRsData.BOF And lvRsData.EOF) Then
		lvRes = pStrTrainId 
	End If
	lvRsData.Close

	If Err.Number <> 0 Then
		Log Now() & " - GetTrainIdCFlexUsed.Erro " & Err.Number & ": " & Err.Description
		GetTrainIdCFlexUsed = ""
		Exit Function
	End If
	
	On Error Goto 0
	
	GetTrainIdCFlexUsed = lvRes
	
	Set lvRsData = Nothing
	
End Function

Public Sub GetAllRegions()
	
	Dim lvSql
	Dim lvObjRegiao
	Dim lvNomePatioAnt
	Dim lvPatio
	Dim lvCoord
	Dim lvRsData
	
	countRegioes = 0
	
	Set lvRsData = CreateObject("ADODB.Recordset")

	lvSql = "SELECT patio, ud, coordinate FROM tbud WHERE (direction = 1 and ud LIKE 'CV03C') OR (direction = -1 and ud LIKE 'CV03B') OR (ud = 'SW03' OR ud = 'SW04') ORDER BY coordinate"
	
'	On Error Resume Next
   lvRsData.Open lvSql, objConn, 0, 1
	
'	If Err.Number <> 0 Then
'		Log Now() & " - Erro(" & lvSql & ") => " & Err.Number & ": " & Err.Description
'	End If
'	On Error GoTo 0
	
	lvNomePatioAnt = ""
	lvPatio = ""
	lvCoord = ""

	Set lvObjRegiao = Nothing
	
	Do While Not lvRsData.EOF
		Redim Preserve vetRegioes(countRegioes)
		'obtem as informações do registro atual
		lvPatio = CStr(lvRsData("patio"))
		lvCoord = CStr(lvRsData("coordinate"))
		
		' se não possuir nenhum registro antes
		If lvNomePatioAnt = "" Then
			Set lvObjRegiao = Nothing
			Set lvObjRegiao = New Regiao
			lvObjRegiao.Nome = lvPatio 
			lvObjRegiao.Inicio = lvCoord 
			
		' se o patio do registro anterior for igual ao do registro atual	
		ElseIf lvNomePatioAnt = lvPatio Then
			lvObjRegiao.Fim = lvCoord
			' adiciona a região ao vetor
			Set vetRegioes(countRegioes) = lvObjRegiao
			countRegioes = countRegioes + 1
			'Log lvObjRegiao.ToString
			
			' cria região para próxima iteração - região entre patios (atual_proximo)
			Set lvObjRegiao = Nothing
			Set lvObjRegiao = New Regiao
			lvObjRegiao.Nome = lvPatio & "_"
			lvObjRegiao.Inicio = lvCoord 
			
		Else
			lvObjRegiao.Nome = lvObjRegiao.Nome & lvPatio
			lvObjRegiao.Fim = lvCoord
			' adiciona a região entre patios no vetor de regiões
			Set vetRegioes(countRegioes) = lvObjRegiao
			countRegioes = countRegioes + 1
			'Log lvObjRegiao.ToString
			
			'instancia a região para a próxima iteração
			Set lvObjRegiao = Nothing
			Set lvObjRegiao = New Regiao
			lvObjRegiao.Nome = lvPatio
			lvObjRegiao.Inicio = lvCoord
			
		End If
		
		'obtem o nome do patio que foi tratado nessa iteração para identificar na próxima
		lvNomePatioAnt = lvPatio
		
		lvRsData.MoveNext
		WScript.sleep 5
    Loop
	 lvRsData.Close
	 Set lvRsData = Nothing
	
End Sub

Public Function BuscaBinaria(coord)
   Dim e
   Dim m
   Dim d
   Dim resul
	Dim lvObjRegiao
   
   resul = -1
   e = 0
   d = countRegioes - 1
   
   Do While (e <= d)
		
		m = (e + d)\2 
'		Log "m = " & m
		
		'		Log "(vet(m) is Nothing) = " & (vet(m) is Nothing)
'		Log "(isNull(vet(m))) = " & (isNull(vet(m)))
'		Log "(isEmpty(vet(m))) = " & (isEmpty(vet(m)))
		Set lvObjRegiao = vetRegioes(m)
		If (lvObjRegiao.Compara(coord) = 0) Then 
			resul = m
			Exit Do
		ElseIf (lvObjRegiao.Compara(coord) = -1) Then
			d = m - 1
'			Log "d = " & d
		Else 
			e = m + 1
'			Log "e = " & e
		End If

   Loop
   BuscaBinaria = resul
End Function
