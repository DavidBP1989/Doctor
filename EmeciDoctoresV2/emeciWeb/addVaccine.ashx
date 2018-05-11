<%@ WebHandler Language="VB" Class="addVaccine" %>

Imports System
Imports System.Web
Imports EmeciFacade
Imports EmeciCommon

Public Class addVaccine : Implements IHttpHandler
    
    Private Property idReg() As Integer
        Get
            Return HttpContext.Current.Session("idregisto")
        End Get
        Set(value As Integer)
            HttpContext.Current.Session("idregisto") = value
        End Set
    End Property
    
    Private Property idPac() As Integer
        Get
            Return HttpContext.Current.Session("idpaciente")
        End Get
        Set(value As Integer)
            HttpContext.Current.Session("idpaciente") = value
        End Set
    End Property
    
    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        
        Dim vaccines As String = context.Request.Form("vaccine")
        Dim idPac As Integer = 0
        If context.Request.Form("idPac") IsNot Nothing Then
            idPac = CType(context.Request.Form("idPac"), Integer)
        End If
        
        Dim dsVac As dsVacunas
        Dim drVac As dsVacunas.VacunasRow
        
        With New clsFAVacunas
            .DeleteVacunas(idPac)
            Try
                
                If vaccines <> String.Empty Then
                    Dim _vaccines() As String = vaccines.Split("|")
                    Dim i As Integer = 1
                    For Each v As String In _vaccines
                        
                        If (v <> "") Then
                            Dim doses() As String = v.Split("-")
                            For Each d As String In doses
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.codigo = i
                                drVac.idpaciente = idPac
                                drVac.vacunaenfprev = preventsDisease(i)
                                
                                Dim dose() As String = d.Split(",")
                                If dose(0) <> "" Then
                                    drVac.vacunadosis = dose(0)
                                End If
                                If dose(1) <> "" Then
                                    drVac.vacunaEdad = dose(1)    
                                End If
                                If dose(2) <> "" Then
                                    drVac.Fecha = dose(2)
                                End If
                                
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                                
                                register(dose(1), dose(2), i)
                            Next
                        End If
                        i += 1
                    Next
                End If
                
            Catch ex As Exception

            End Try
            
        End With
        
        context.Response.ContentType = "application/json"
        context.Response.Write("{""status"":200}")
    End Sub
    
    Function preventsDisease(ByVal vaccine As Integer) As String
        Dim b As String = ""
        Select Case vaccine
            Case 1
                b = "Tuberculosis"
            Case 2
                b = "Hepatitis B"
            Case 3
                b = "Difteria, Tosferina, Tétanos, Poliovirus Inactivado, Haemophilus tipo b"
            Case 4
                b = "Rotavirus"
            Case 5
                b = "Neumococo"
            Case 6
                b = "Hepatitis A"
            Case 7
                b = "Varicela"
            Case 8
                b = "Sarampión, Parotiditis y Rubéola"
            Case 9
                b = "Virus del Papiloma Humano"
            Case 10
                b = "Influenza"
        End Select
        Return b
    End Function
    
    Private Sub register(ByVal age As String, ByVal _date As String, ByVal code As Integer)
        Try
            Dim ds As New DSRegistro
            Dim dr As DSRegistro.RegistroRow
            Dim drP As DSRegistro.PacienteRow
                
            dr = ds.Registro.NewRegistroRow
            ds.Registro.AddRegistroRow(dr)
            dr.AcceptChanges()
                
            dr.idRegistro = Me.idReg
            drP = ds.Paciente.NewPacienteRow
            ds.Paciente.AddPacienteRow(drP)
                
            drP.idPaciente = Me.idPac
            drP.IdRegistro = Me.idReg
            drP.otraVacuna = ""
            drP.vacunaedad = age
            drP.vacunaenfprev = preventsDisease(code)
            drP.VacunaFecha = _date
            
            drP.AcceptChanges()
            
            Dim rp As Boolean
            With New clsFARegistro
                rp = .GuardarRegistro(ds, True)
            End With
        Catch ex As Exception

        End Try
    End Sub
 
    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class