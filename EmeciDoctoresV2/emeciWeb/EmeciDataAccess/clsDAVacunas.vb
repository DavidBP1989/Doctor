Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationSettings
Imports EmeciCommon
Public Class clsDAVacunas
    Implements IDisposable

    Private dsCommand As SqlDataAdapter
    Private loadCommand As SqlCommand

    Private Const spVacunas_PROCEDURE As String = "spVacunasSelect"
    Private Const IdPaciente_PARM As String = "@idpaciente"

    Private Enum QueryProcedure
        GetVacunas
    End Enum

    Public Sub New()
        MyBase.New()
        dsCommand = New SqlDataAdapter
        dsCommand.TableMappings.Add("Table", "Vacunas")
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(True)    ' as a service to those who might inherit from us
    End Sub

    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If (Not disposing) Then
            Exit Sub     ' we're being collected, so let the GC take care of this object
        End If

        If Not dsCommand Is Nothing Then
            If Not dsCommand.SelectCommand Is Nothing Then
                If Not dsCommand.SelectCommand.Connection Is Nothing Then
                    dsCommand.SelectCommand.Connection.Dispose()
                End If
                dsCommand.SelectCommand.Dispose()
            End If
            dsCommand.Dispose()
            dsCommand = Nothing
        End If
    End Sub

    Private Function GetLoadCommand(ByVal p As QueryProcedure) As SqlCommand
        Dim procedure As String
        loadCommand = New SqlCommand("", New SqlConnection(AppSettings("Connection")))
        Select Case p
            Case QueryProcedure.GetVacunas
                procedure = spVacunas_PROCEDURE
                loadCommand.CommandText = procedure
                loadCommand.CommandType = CommandType.StoredProcedure
                loadCommand.Parameters.Add(New SqlParameter(IdPaciente_PARM, SqlDbType.Int))
        End Select
        GetLoadCommand = loadCommand
    End Function

    Public Function GetVacunas(ByVal idpaciente As Integer, ByRef Caderror As String) As DataSet
        Dim errorpago As Boolean = False
        Dim ckin, ckout As Date
        Dim idReg As Long
        Dim ds As New DataSet
        Try
            Dim sqlcmd As SqlCommand = GetLoadCommand(QueryProcedure.GetVacunas)
            sqlcmd.Parameters(IdPaciente_PARM).Value = idpaciente
            dsCommand.SelectCommand = sqlcmd
            dsCommand.Fill(ds)
        Catch e As Exception
            Caderror = e.Message
        Finally
        End Try
        Return ds
    End Function

    Public Function VacunaInsert(ByRef dsVac As dsVacunas, ByRef Caderror As String) As Boolean
        Dim errorpago As Boolean = False
        Dim ckin, ckout As Date
        Dim sqlconn As New SqlConnection(AppSettings("connection"))

        Dim strProc As String
        Try
            sqlconn.Open()
            ''' borrar las vacunas y meter las nuevas ''''''''''''''


            strProc = "SPVacunasInsert"
            Dim sqlcmd As New SqlCommand(strProc, sqlconn)
            sqlcmd.CommandType = CommandType.StoredProcedure

            sqlcmd.Parameters.Add("@idpaciente", SqlDbType.Int).Value = dsVac.Vacunas(0).idpaciente
            If dsVac.Vacunas(0).Fecha IsNot Nothing Or dsVac.Vacunas(0).Fecha = "" Then
                sqlcmd.Parameters.Add("@Fecha", SqlDbType.SmallDateTime).Value = dsVac.Vacunas(0).Fecha
            End If
            sqlcmd.Parameters.Add("@Codigo", SqlDbType.NVarChar, 2).Value = dsVac.Vacunas(0).codigo

            If Not dsVac.Vacunas(0).IsvacunaenfprevNull Then sqlcmd.Parameters.Add("@vacunaenfprev", SqlDbType.NVarChar, 50).Value = dsVac.Vacunas(0).vacunaenfprev
            If Not dsVac.Vacunas(0).IsvacunadosisNull Then sqlcmd.Parameters.Add("@vacunadosis", SqlDbType.NVarChar, 30).Value = dsVac.Vacunas(0).vacunadosis
            If Not dsVac.Vacunas(0).IsvacunaEdadNull Then sqlcmd.Parameters.Add("@vacunaEdad", SqlDbType.NVarChar, 30).Value = dsVac.Vacunas(0).vacunaEdad

            sqlcmd.ExecuteNonQuery()

        Catch e As Exception
            If sqlconn.State = ConnectionState.Open Then sqlconn.Close()
            Caderror = e.Message
            Return False
        Finally
            If sqlconn.State = ConnectionState.Open Then sqlconn.Close()
        End Try
        Return True
    End Function

    Public Function VacunaDelete(ByRef idpaciente As Integer, ByRef Caderror As String) As Boolean
        Dim errorpago As Boolean = False

        Dim sqlconn As New SqlConnection(AppSettings("connection"))

        Dim strProc As String
        Try
            sqlconn.Open()
            ''' borrar las vacunas y meter las nuevas ''''''''''''''
            Dim scmdDel As New SqlCommand("spVacunasDelete", sqlconn)
            scmdDel.CommandType = CommandType.StoredProcedure
            scmdDel.Parameters.Add("@idpaciente", SqlDbType.Int).Value = idpaciente
            scmdDel.ExecuteNonQuery()

        Catch e As Exception
            If sqlconn.State = ConnectionState.Open Then sqlconn.Close()
            Caderror = e.Message
            Return False
        Finally
            If sqlconn.State = ConnectionState.Open Then sqlconn.Close()
        End Try
        Return True
    End Function

End Class
