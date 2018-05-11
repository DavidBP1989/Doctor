$(function () {

    $('.calendar').each(function (c) {
        var picker = new Pikaday({
            field: this
        })
    });

    $('.drelationship').hide();
    $('* .radioy').click(function () {
        $('.drelationship').css('display', $(this).val() == 1 ? 'block' : 'none');
    })

    $('* .radiox').click(function () {
        $('.pvrelationship').css('display', $(this).val() == 1 ? 'block' : 'none');
    })

    $('#edad, #gestas, #paragestas, #cesarea, #aborto, #recienNacido, #mortinato, #edadMenarca, #sistolica, #diastolica').keypress(function (ev) {
        var num = (ev.charCode) ? ev.which : ev.keyCode;
        return (num == 8 || (num >= 48 && num <= 57));
        /*
            num 8 = delete button
            num 48 - 57 = numbers
        */
    })

    $('#peso, #talla, #temperatura').keypress(function (ev) {
        var num = (ev.charCode) ? ev.which : ev.keyCode;
        return (num == 46 || num == 8 || (num >= 48 && num <= 57));
        /*
            num 46 = point
            num 8 = delete button
            num 48 - 57 = numbers
        */
    })


    $('#otros').click(function () {
        var bool = $('#otros').prop('checked');
        if (bool)
            $('#explique').removeAttr('disabled');
        else $('#explique').attr('disabled', 'disabled');
    })

    //Calculo de masa corporal
    $('#peso').focusout(MasaCalculator);
    $('#talla').focusout(MasaCalculator);


    $('#goObstetrica').click(function () {
        window.location.href = 'ConsultaObstetrica?idPac=' + window.idpac;
    })
})




function MasaCalculator() {
    var peso = $('#peso').val();
    var talla = $('#talla').val();
    var masa = $('#masa');

    var value = null;
    if (peso != '' && talla != '') {
        if (!isNaN(peso) && !isNaN(talla))
            value = peso / (talla * talla);
        else masa.val('');
    } else masa.val('');

    if (value != null)
        masa.val(parseFloat(value).toFixed(2));
}


function SHPreviousQuery()
{
    var previous = $('#previousQuery');
    var query = $('#query');
    var btn = $('#btnPrevious');
    var sinprev = $('* .sinprev');
    var conprev = $('* .conprev');
    var sin_prev = $('* .sin_prev');
    var con_prev = $('* .con_prev');

    var showPrevious = previous.css('display') == 'none' ? false : true;
    if (showPrevious)
    {
        previous.hide();
        showPrevious = false;
        btn.text('Mostrar Consulta Anterior');
        $('.contenido-de-panel label').css({ 'white-space': 'pre', 'font-size': '13px', 'word-break': 'break-all' });
        $('* .n1').removeClass('col-lg-4').addClass('col-lg-2');
        $('.n-1').addClass('col-lg-6');
        $('* .n3').removeClass('col-lg-6').addClass('col-lg-3');
        $('.n4').removeClass('col-lg-6').addClass('col-lg-3');
        $('.n-4').removeClass('col-lg-6');
        $('.n5').removeClass('col-lg-12').addClass('col-lg-9');
        $('* .n6').removeClass('col-lg-4').addClass('col-lg-2');
        $('.n7').removeClass('col-lg-6').addClass('col-lg-4');
        $('.n8').removeClass('col-lg-6').addClass('col-lg-3');
        $('.n-8').addClass('col-lg-5');
        $('* .n9').removeClass('col-lg-6').addClass('col-lg-3');
        $('.n-9').addClass('col-lg-6');
        $('.n10').removeClass('col-lg-5').addClass('col-lg-2');
        $('.n11').removeClass('col-lg-7').addClass('col-lg-10');
        $('.n12').removeClass('col-lg-12').addClass('col-lg-6');
        $('* .n13').removeClass('col-lg-4').addClass('col-lg-2');
        $('* .n14').removeClass('col-lg-4').addClass('col-lg-3');
        $('.n15').removeClass('col-lg-12').addClass('col-lg-3');

    } else
    {
        previous.show();
        showPrevious = true;
        btn.text('Ocultar Consulta Anterior');
        $('.contenido-de-panel label').css({ 'white-space': 'pre-line', 'font-size': '8px', 'word-break': 'normal' });
        $('* .n1').removeClass('col-lg-2').addClass('col-lg-4');
        $('.n-1').removeClass('col-lg-6');
        $('* .n3').removeClass('col-lg-3').addClass('col-lg-6');
        $('.n4').removeClass('col-lg-3').addClass('col-lg-6');
        $('.n-4').addClass('col-lg-6');
        $('.n5').removeClass('col-lg-9').addClass('col-lg-12');
        $('* .n6').removeClass('col-lg-2').addClass('col-lg-4');
        $('.n7').removeClass('col-lg-4').addClass('col-lg-6');
        $('.n8').removeClass('col-lg-3').addClass('col-lg-6');
        $('.n-8').removeClass('col-lg-5');
        $('* .n9').removeClass('col-lg-3').addClass('col-lg-6');
        $('.n-9').removeClass('col-lg-6');
        $('.n10').removeClass('col-lg-2').addClass('col-lg-5');
        $('.n11').removeClass('col-lg-10').addClass('col-lg-7');
        $('.n12').removeClass('col-lg-6').addClass('col-lg-12');
        $('* .n13').removeClass('col-lg-2').addClass('col-lg-4');
        $('* .n14').removeClass('col-lg-3').addClass('col-lg-4');
        $('.n15').removeClass('col-lg-3').addClass('col-lg-12');
    }

   
    if (showPrevious)
    {
        query.removeClass('col-lg-12');
        query.addClass('col-lg-6');
    } else
    {
        query.removeClass('col-lg-6');
        query.addClass('col-lg-12');
    }
}

function SaveQuery()
{
    if (ItHasCouple())
    {
        var req = new Object();
        req.idpac = window.idpac;
        req.peso = $('#peso').val() == '' ? null : parseFloat($('#peso').val());
        req.talla = $('#talla').val() == '' ? null : parseFloat($('#talla').val());
        req.edadMenarca = $('#edadMenarca').val() == '' ? null : parseInt($('#edadMenarca').val());
        req.temperatura = $('#temperatura').val() == '' ? null : parseFloat($('#temperatura').val());
        req.sistolica = $('#sistolica').val() == '' ? null : parseInt($('#sistolica').val());
        req.diastolica = $('#diastolica').val() == '' ? null : parseInt($('#diastolica').val());
        req.fechaMenstruacion = $('#fechaMenstruacion').val();
        req.gestas = parseInt($('#gestas').val());
        req.paragestas = parseInt($('#paragestas').val());
        req.cesarea = parseInt($('#cesarea').val());
        req.aborto = parseInt($('#aborto').val());
        req.recienNacido = parseInt($('#recienNacido').val());
        req.mortinato = parseInt($('#mortinato').val());
        req.edadSexual = parseInt($('#edadSexual').val());
        req.menacma = $('#menacma').val();

        req.oligomenorrea = $('#oligomenorrea').prop('checked');
        req.proimenorrea = $('#proimenorrea').prop('checked');
        req.hipermenorrea = $('#hipermenorrea').prop('checked');
        req.dismenorrea = $('#dismenorrea').prop('checked');
        req.dispareunia = $('#dispareunia').prop('checked');
        req.leucorrea = $('#leucorrea').prop('checked');
        req.lactorrea = $('#lactorrea').prop('checked');
        req.amenorrea = $('#amenorrea').prop('checked');
        req.metrorragia = $('#metrorragia').prop('checked');
        req.otros = $('#otros').prop('checked');
        req.explique = $('#explique').val();

        req.pareja = $('* .radioy')[0].checked;
        req.nombrePareja = $('#nombrePareja').val();
        req.sexoPareja = $('#sexoPareja').val();
        req.estadoCivilPareja = $('#estadoCivilPareja').val();
        req.grupoRHPareja = $('#grupoRHPareja').val();
        req.fechaPareja = $('#fechaPareja').val();
        req.edadPareja = $('#edadPareja').val();
        req.ocupacionPareja = $('#ocupacionPareja').val();
        req.telefonoPareja = $('#telefonoPareja').val();
        req.motivoConsulta = $('#motivoConsulta').val();
        
        $.ajax({
            url: 'ConsultaGinecologica'
            , data: req
            , method: 'POST'
            , success: function (res) {
                if (res.status == "true")
                {
                    alert('Consulta guardada correctamente.');
                    $('* #myForm')[1].reset();
                    window.location.href = 'Pacientes';
                } else
                {
                    alert('Hubo un error al intentar guardar la consulta, intente de nuevo');
                }
            }
            , error: function (res) {
                alert('Hubo un error al intentar guardar la consulta, intente de nuevo');
            }
        })
    }
}

function convertToBool(string)
{
    return string === '1' ? true : false;
}

function ItHasCouple()
{
    /* validar los datos de la pareja */
    var _continue = true;
    var val = $('* .radioy')[0].checked;
    if (val)
    {
        _continue = (_continue && validateForm.All($('#nombrePareja')[0]));
        _continue = (_continue && validateForm.All($('#edadPareja')[0]));
        _continue = (_continue && validateForm.All($('#ocupacionPareja')[0]));
    }
    
    if (!_continue)
        $("html, body").animate({ scrollTop: $(document).height() }, 1000);
    return _continue;
}





function ChangePreviousQuery(id)
{
    $.ajax({
        url: 'ConsultaGinecologicaPorId'
            , data: { idPac: idpac, idquery : id}
            , method: 'POST'
            , success: function (res) {
                if (res.hasOwnProperty('consulta') && res.hasOwnProperty('paciente')) {
                    var peso = res.consulta[0].Peso == null ? 0 : parseFloat(res.consulta[0].Peso);
                    var altura = res.consulta[0].Altura == null ? 0 : parseFloat(res.consulta[0].Altura);
                    var masa = 0;
                    if (peso > 0 && altura > 0) {
                        masa = peso / (altura * altura);
                    }

                    $('#pPeso').html(peso > 0 ? peso : '&nbsp;');
                    $('#pTalla').html(altura > 0 ? altura : '&nbsp;');
                    $('#pMasa').html(masa > 0 ? masa.toFixed(2) : '&nbsp;');
                    $('#pEdadMenarca').html(res.paciente.EdadMenarca == null ? '&nbsp;' : res.paciente.EdadMenarca);
                    $('#pTempe').html(res.consulta[0].Temperatura == null ? '&nbsp;' : res.consulta[0].Temperatura);
                    $('#psistolica').html(res.consulta[0].TensionArterial == null ? '&nbsp;' : res.consulta[0].TensionArterial);
                    $('#pdistolica').html(res.consulta[0].TensionArterialB == null ? '&nbsp;' : res.consulta[0].TensionArterialB);
                    $('#pFechaM').html(res.fUM);
                    $('#pGesta').html(res.consulta[0].gine.Gestas == null ? '&nbsp;' : res.consulta[0].gine.Gestas);
                    $('#pParegesta').html(res.consulta[0].gine.ParaGestas == null ? '&nbsp;' : res.consulta[0].gine.ParaGestas);
                    $('#pCesarea').html(res.consulta[0].gine.Cesareas == null ? '&nbsp;' : res.consulta[0].gine.Cesareas);
                    $('#pAborto').html(res.consulta[0].gine.abortos == null ? '&nbsp;' : res.consulta[0].gine.abortos);
                    $('#pNacido').html(res.consulta[0].gine.RecienNacidosVivos == null ? '&nbsp;' : res.consulta[0].gine.RecienNacidosVivos);
                    $('#pMortinato').html(res.consulta[0].gine.mortinatos == null ? '&nbsp;' : res.consulta[0].gine.mortinatos);
                    $('#pEdadSexual').html(res.consulta[0].gine.EdadInicioVidaSexual == null ? '&nbsp;' : res.consulta[0].gine.EdadInicioVidaSexual);
                    $('#pMenacma').html(res.consulta[0].gine.menacma == null ? '&nbsp;' : res.consulta[0].gine.menacma);
                    $('#prh').html(res.paciente.grupoRH == null ? '&nbsp;' : res.paciente.grupoRH);
                    $('#cboligomenorrea').prop('checked', res.consulta[0].gine.oligonorrea);
                    $('#cbproiomenorrea').prop('checked', res.consulta[0].gine.Proiomenorrea);
                    $('#cbhipermenorrea').prop('checked', res.consulta[0].gine.Hipermenorrea);
                    $('#cbdismenorrea').prop('checked', res.consulta[0].gine.Dismenorrea);
                    $('#cbdispareunia').prop('checked', res.consulta[0].gine.Dispareunia);
                    $('#cbleucorrea').prop('checked', res.consulta[0].gine.Leucorrea);
                    $('#cblactorrea').prop('checked', res.consulta[0].gine.Lactorrea);
                    $('#cbamenorrea').prop('checked', res.consulta[0].gine.Amenorrea);
                    $('#cbmetrorragia').prop('checked', res.consulta[0].gine.Metrorragia);
                    $('#cbotros').prop('checked', res.consulta[0].gine.Otros);
                    $('#reaason').html(res.consulta[0].gine.OtrosEspecifique == null ? '&nbsp;' : res.consulta[0].gine.OtrosEspecifique);
                    if (res.consulta[0].gine.TienePareja) {
                        $('* .radiox')[0].checked = true;
                        $('.pvrelationship').show();

                        $('#pNombrePareja').html(res.consulta[0].gine.nombrePareja == null ? '&nbsp;' : res.consulta[0].gine.nombrePareja);
                        $('#psexoPareja').html(res.consulta[0].gine.SexoPareja == null ? '&nbsp;' : (res.consulta[0].gine.SexoPareja == 'F' ? 'Femenino' : 'Masculino'));
                        var estadoCivil = '';
                        if (res.consulta[0].gine.EstadoCivilPareja != null) {
                            switch (res.consulta[0].gine.EstadoCivilPareja) {
                                case '0':
                                    estadoCivil = 'Casado';
                                    break;
                                case '1':
                                    estadoCivil = 'Separado';
                                    break;
                                case '2':
                                    estadoCivil = 'Soltero';
                                    break;
                                case '3':
                                    estadoCivil = 'Union libre';
                                    break;
                                case '4':
                                    estadoCivil = 'Viudo';
                                    break;
                                case '5':
                                    estadoCivil = 'Divorciado';
                                    break;
                            }
                        }

                        $('#pEstadoCivilPareja').html(estadoCivil);
                        $('#pGRHPareja').html(res.consulta[0].gine.GrupoRHPareja == null ? '&nbsp;' : res.consulta[0].gine.GrupoRHPareja);
                        $('#pFechaPareja').html(res.fNP);
                        $('#pEdadPareja').html(res.consulta[0].gine.edadPareja == null ? '&nbsp;' : res.consulta[0].gine.edadPareja);
                        $('#pOcupation').html(res.consulta[0].gine.OcupacionPareja == null ? '&nbsp;' : res.consulta[0].gine.OcupacionPareja);
                        $('#pTelefonos').html(res.consulta[0].gine.TelefonoPareja == null ? '&nbsp;' : res.consulta[0].gine.TelefonoPareja);
                        $('#motivoooo').html(res.consulta[0].motivo == null ? '&nbsp;' : res.consulta[0].motivo);
                    }
                    else {
                        $('* .radiox')[1].checked = true;
                        $('.pvrelationship').hide();
                    }
                }
            }
    })
}