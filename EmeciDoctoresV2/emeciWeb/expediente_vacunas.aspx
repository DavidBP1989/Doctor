<%@ Page Language="VB" AutoEventWireup="false" %>
<%@ Import Namespace="EmeciFacade" %>
<%@ Import Namespace="EmeciCommon" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Register TagPrefix="uc" TagName="header" Src="Modulos/ucltrMenu.ascx" %>
<!DOCTYPE html>

<html>
<head>
<meta charset="utf-8"/>
    <title></title>
    <meta content="Bluefish 1.0.6" name="generator">
	<meta content="2007-09-19T01:28:10-0700" name="date">
	<meta content="" name="copyright">
	<meta content="" name="keywords">
	<meta content="" name="description">
	<meta content="NOINDEX, NOFOLLOW" name="ROBOTS">
	<meta http-equiv="content-type" content="text/html; charset=windows-1252">
    <link href="expedienteemeci.css" type="text/css" rel="stylesheet">
    <link href="Scripts/pikaday.css" rel="stylesheet" />
    <style>
        .tableVaccine{ margin:0 auto !important; width:90%; height:0;}
        .linkDose { cursor:pointer; text-decoration:underline;}
        #error { display:none; padding-left:10px; padding-top:10px;}
        .uiBgIconEdit { cursor:pointer;}
    </style>
    <script src="Scripts/Jquery_1_10.js"></script>
    <script src="http://code.jquery.com/jquery-1.11.3.min.js"></script>
    <script src="Scripts/pikaday.js"></script>
    <script src="Scripts/angular.min.js"></script>
    <script src="Scripts/appVaccine.js"></script>
    <script>
        function valid() {
            var done = true;
            $('input[type=text]').map(function () {
                if ($(this).val() === '') {
                    if ($(this).attr('class') !== 'null ng-scope') done = false;
                }
            });

            if (!done) $('#error').css('display', 'block');
            else saveValues();
        }

        function saveValues() {
            var resp = '';
            var totalVaccines = $('#TotalVaccines').val();
            for (var i = 1; i <= totalVaccines; i++) {
                var rows = $('* #row_' + i.toString());
                if (rows.length) {
                    var doses = '';
                    rows.map(function (row) {
                        var rowL = $(rows[row]).find('input').length; 
                        if (rowL) {
                            if (rowL === 1) {
                                doses += rows.find('td').eq(2)[0].innerHTML + ','
                                doses += rows.find('td').eq(3)[0].innerHTML + ','
                                doses += $(rows[row]).find('input').val() + ',';
                            } else {
                                $(rows[row]).find('input').each(function (index, tag) {
                                    doses += tag.value + ','
                                });
                            }

                            doses = doses.substring(0, doses.length - 1) + '-';
                        }
                    });

                    if (doses !== '') doses = doses.substring(0, doses.length - 1);
                    resp += doses;
                }

                resp += '|';
            }

            $.ajax({
                url: 'addVaccine.ashx'
                    , data: { vaccine: resp, idPac : $('#_idPac').val() }
                    , method: 'POST'
            }).success(function (status) {
                window.location.reload()
            });
        }
    </script>
</head>
<body>
    <uc:header runat="server" />
    <form runat="server">
        <div ng-app="app" ng-controller="vController" class="uiContainer">
            <div class="uiPanel">
                <div class="uiPanelHead">
                    <h2 class="uiPanelHeadTitle">Tabla de Inmunizaciones</h2>
                    <div class="uiPanelHeadDescription">
                        Es muy importante aplicar las vacunas en las fechas
                    que se indica ya que as&iacute; se puede prevenir
                    correctamente la enfermedad.
                    </div>
                </div>
                <div class="uiPanelToolBar">
                    <div class="uiPanelMenuBar">
                        <a onclick="valid()" class="button uiBgIconEdit">Guardar cambios</a>
                        <asp:LinkButton runat="server" ID="_cancelVaccine" CssClass="button uiBgIconCancel">Cancelar</asp:LinkButton>
                    </div>
                </div>
                <div class="uiPanelContent">
                    <span id="error">* Los campos dosis, edad y fecha no pueden ser vacios</span>
                    <br style="clear:both" />

                    <table class="uiTableDataList tableVaccine" border="1" align="center" cellpadding="2" cellspacing="0">
                        <tr>
                            <th style="width:110px"><strong>Vacuna</strong></th>
                            <th>
                                <div align="center">
                                    <strong>Enfermedad que previene</strong>
                                </div>
                            </th>
                            <th style="width:95px;"><strong>Dosis</strong></th>
                            <th style="width:95px;"><strong>Edad</strong></th>
                            <th style="width:135px;"><strong>Fecha de vacunaci&oacute;n</strong></th>
                        </tr>
                        <tr ng-repeat="j in json track by $index" id="row_{{j.code}}" style="background-color:{{j.css}}">
                            <td ng-if="j.name !== 'no'" rowspan="{{j.rowspan}}">{{j.name}}</td>
                            <td ng-if="j.prev_disease !== 'no'" rowspan="{{j.rowspan}}">{{j.prev_disease}}</td>
                            <td>{{j.dose}}</td>
                            <td>{{j.age}}</td>
                            <td html="{{j.date}}"></td>
                        </tr>
                    </table>
                </div>
                <div class="uiPanelToolBar">
                    <div class="uiPanelMenuBar">
                        <a onclick="valid()" class="button uiBgIconEdit">Guardar cambios</a>
                        <asp:LinkButton runat="server" ID="_cancelVaccine2" CssClass="button uiBgIconCancel">Cancelar</asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>
        <input runat="server" type="hidden" id="TotalVaccines" />
    </form>

    <input type="hidden" runat="server" id="ResponseVaccine" />
    <input type="hidden" runat="server" id="Jsonvaccine" />
    <input type="hidden" runat="server" id="_idPac" />
</body>
</html>

<script runat="server">
    
    Private Property idPac() As Integer
        Get
            Return Session("idpaciente")
        End Get
        Set(value As Integer)
            Session("idpaciente") = value
        End Set
    End Property
    
    Private vaccine As DataSet
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Dim q As String = Request.QueryString("emeci")
        
        Dim dsPac As DataSet
        With New clsFARegistro
            If q IsNot Nothing Then
                Dim emeci As String = Mid(q, 70, 5) & "-" & Mid(q, 66, 4) & "-" & Mid(q, 75, 4)
                dsPac = .GetpacienteByEmeci(emeci)
            Else : dsPac = .GetpacienteById(Me.idPac)
            End If
        End With
        
        If dsPac IsNot Nothing AndAlso dsPac.Tables(0).Rows.Count > 0 Then
            Me.idPac = dsPac.Tables(0).Rows(0)("idpaciente")
        End If
        Me._idPac.Value = Me.idPac
        
        Vaccine_List()
        With New clsFAVacunas
            vaccine = .GetListVacunas(Me.idPac)
            If vaccine IsNot Nothing AndAlso Not vaccine.Tables(0).Rows.Count > 0 Then
                DefaultVaccine()
                LoadVaccines()
            Else
                Dim exist As Integer = 0
                For Each d As DataRow In vaccine.Tables(0).Rows
                    If Not IsDBNull(d("vacunadosis")) Then
                        exist += 1
                    End If
                Next
                If exist > 1 Then
                    LoadVaccines()
                Else
                    DefaultVaccine()
                    LoadVaccines()
                End If
            End If
        End With
    End Sub
    
    
    'LISTA DE VACUNAS
    Private vaccineList As New Dictionary(Of Integer, String)
    Protected Sub Vaccine_List()
        vaccineList.Add(1, "BCG")
        vaccineList.Add(2, "HB")
        vaccineList.Add(3, "PENTAVALENTE DPT + IPV + Hib")
        vaccineList.Add(4, "RV")
        vaccineList.Add(5, "Neumococo")
        vaccineList.Add(6, "HA")
        vaccineList.Add(7, "Varicela")
        vaccineList.Add(8, "Tripe Viral")
        vaccineList.Add(9, "VPH")
        vaccineList.Add(10, "Gripe")
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
    
    Private dsVac As dsVacunas
    Protected Sub DefaultVaccine()
        
        Dim drVac As dsVacunas.VacunasRow
        With New clsFAVacunas
            For Each vacc As KeyValuePair(Of Integer, String) In vaccineList
                
                Select Case vacc.Key
                    Case 1
                        dsVac = New dsVacunas
                        drVac = dsVac.Vacunas.NewRow
                        drVac.Fecha = New DateTime(2010, 2, 10)
                        newRow(drVac, vacc.Key, "única", "Al nacer")
                        dsVac.Vacunas.AddVacunasRow(drVac)
                        .GuardarVacunas(dsVac)
                    Case 2
                        For i As Integer = 1 To 3
                            If i = 1 Then
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.Fecha = New DateTime(2010, 2, 10)
                                newRow(drVac, vacc.Key, "Primera", "Al nacer")
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                            ElseIf i = 2 Then
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.Fecha = CType(Nothing, DateTime)
                                newRow(drVac, vacc.Key, "Segunda", "2 Meses")
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                            Else
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.Fecha = New DateTime(2011, 7, 13)
                                newRow(drVac, vacc.Key, "Tercera", "6 Meses")
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                            End If
                        Next
                    Case 3, 4
                        For i As Integer = 1 To 3
                            If i = 1 Then
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.Fecha = New DateTime(2010, 2, 10)
                                newRow(drVac, vacc.Key, "Primera", "Al nacer")
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                            ElseIf i = 2 Then
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.Fecha = Nothing
                                newRow(drVac, vacc.Key, "Segunda", "2 Meses")
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                            Else
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.Fecha = New DateTime(2011, 7, 13)
                                newRow(drVac, vacc.Key, "Tercera", "6 Meses")
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                            End If
                        Next
                    Case 5
                        For i As Integer = 1 To 4
                            If i = 1 Then
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.Fecha = New DateTime(2011, 4, 29)
                                newRow(drVac, vacc.Key, "Primera", "Al nacer")
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                            ElseIf i = 2 Then
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.Fecha = Nothing
                                newRow(drVac, vacc.Key, "Segunda", "2 Meses")
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                            ElseIf i = 3 Then
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.Fecha = Nothing
                                newRow(drVac, vacc.Key, "Tercera", "6 Meses")
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                            Else
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.Fecha = New DateTime(2011, 9, 7)
                                newRow(drVac, vacc.Key, "Refuerzo", "15 Meses")
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                            End If
                        Next
                    Case 6
                        For i As Integer = 1 To 2
                            If i = 1 Then
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.Fecha = New DateTime(2011, 6, 7)
                                newRow(drVac, vacc.Key, "Primera", "1 Año")
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                            ElseIf i = 2 Then
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.Fecha = Nothing
                                newRow(drVac, vacc.Key, "Segunda", "18 Meses")
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                            End If
                        Next
                    Case 7
                        For i As Integer = 1 To 2
                            If i = 1 Then
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.Fecha = Nothing
                                newRow(drVac, vacc.Key, "Primera", "1 Año")
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                            ElseIf i = 2 Then
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.Fecha = Nothing
                                newRow(drVac, vacc.Key, "Segunda", "13 Meses")
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                            End If
                        Next
                    Case 8
                        For i As Integer = 1 To 2
                            If i = 1 Then
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.Fecha = New DateTime(2011, 2, 26)
                                newRow(drVac, vacc.Key, "Primera", "1 Año")
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                            ElseIf i = 2 Then
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.Fecha = Nothing
                                newRow(drVac, vacc.Key, "Refuerzo", "6 Años")
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                            End If
                        Next
                    Case 9
                        For i As Integer = 1 To 3
                            If i = 1 Then
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.Fecha = Nothing
                                newRow(drVac, vacc.Key, "Primera", "9 Años")
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                            ElseIf i = 2 Then
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.Fecha = Nothing
                                newRow(drVac, vacc.Key, "Segunda", "2 Meses Despues")
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                            Else
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.Fecha = Nothing
                                newRow(drVac, vacc.Key, "Tercera", "6 Meses Despues")
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                            End If
                        Next
                    Case 10
                        For i As Integer = 1 To 3
                            If i = 1 Then
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.Fecha = New DateTime(2010, 12, 12)
                                newRow(drVac, vacc.Key, "Primera", "6 Años")
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                            ElseIf i = 2 Then
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.Fecha = Nothing
                                newRow(drVac, vacc.Key, "Segunda", "7 Meses")
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                            Else
                                dsVac = New dsVacunas
                                drVac = dsVac.Vacunas.NewRow
                                drVac.Fecha = Nothing
                                newRow(drVac, vacc.Key, "Refuerzo", "Anual")
                                dsVac.Vacunas.AddVacunasRow(drVac)
                                .GuardarVacunas(dsVac)
                            End If
                        Next
                End Select
            Next
        End With
    End Sub
    
    Private Sub newRow(ByRef drvac As dsVacunas.VacunasRow, ByVal key As Integer, _
                       ByVal dosis As String, ByVal edad As String)
        drvac.idpaciente = Me.idPac
        drvac.codigo = key
        drvac.vacunaenfprev = preventsDisease(key)
        drvac.vacunadosis = dosis
        drvac.vacunaEdad = edad
    End Sub
    
    
    Protected Sub LoadVaccines()
        Dim _vaccine As String = ""
        Dim dose As String = ""
        Dim json As String = ""
        
        With New clsFAVacunas
            vaccine = .GetListVacunas(Me.idPac)
        End With
        
        For Each vacc As KeyValuePair(Of Integer, String) In vaccineList
            _vaccine &= """name"":""" & vacc.Value & ""","
            _vaccine &= """code"":""" & vacc.Key & ""","
            _vaccine &= """prev_disease"":""" & preventsDisease(vacc.Key) & ""","
            
            Dim rowspan As Integer = 1
            If vaccine IsNot Nothing AndAlso vaccine.Tables(0).Rows.Count > 0 Then
                For Each row As DataRow In vaccine.Tables(0).Rows
                    If row("codigo") = vacc.Key Then
                        If Not IsDBNull(row("vacunadosis")) Then
                            dose &= "{""dose"":""" & row("vacunadosis") & ""","
                            dose &= """age"":""" & row("vacunaEdad") & ""","
                            dose &= """date"":""" & row("Fecha") & """},"
                            rowspan += 1
                        End If
                    End If
                Next
                
                _vaccine &= """rowspan"":""" & rowspan.ToString & ""","
                If dose <> "" Then
                    dose = dose.Substring(0, dose.Length - 1)
                End If
                
                json &= "{" & _vaccine & """doses"":[" & dose & "]},"
                dose = ""
            Else
                json &= "{" & _vaccine & """doses"":[]},"
            End If
        Next
        
        json = json.Substring(0, json.Length - 1)
        json = "[" & json & "]"
        Jsonvaccine.Value = json
        TotalVaccines.Value = vaccineList.Count
    End Sub
</script>
