$(function () {

    $('.calendar').each(function (c) {
        var picker = new Pikaday({
            field: this
        })
    });

    $('#peso, #talla, #temperatura').keypress(function (ev) {
        var num = (ev.charCode) ? ev.which : ev.keyCode;
        return (num == 46 || num == 8 || (num >= 48 && num <= 57));
        /*
            num 46 = point
            num 8 = delete button
            num 48 - 57 = numbers
        */
    })

    $('#sistolica, #diastolica, #fu, #fcf, #cc, #ca, #lf, #dbp, #pesoMadre, #pesoProducto, #ta, #fcm').keypress(function (ev) {
        var num = (ev.charCode) ? ev.which : ev.keyCode;
        return (num == 8 || (num >= 48 && num <= 57));
        /*
            num 8 = delete button
            num 48 - 57 = numbers
        */
    })

    //Calculo de masa corporal
    $('#peso').focusout(MasaCalculator);
    $('#talla').focusout(MasaCalculator);



    //calculo peso de la madre
    $('#pesoProducto').focusout(PesoMadre);
    $('#peso').focusout(PesoMadre);


    /*
        hide show specifications
    */

    $('#toxemias').change(function () {
        $('#EspToxemia').css('display', $(this).val() == 1 ? 'block' : 'none');
    })

    $('#partos').change(function () {
        $('#EspPartos').css('display', $(this).val() == 1 ? 'block' : 'none');
    })

    $('#distocia').change(function () {
        $('#EspDistocia').css('display', $(this).val() == 2 ? 'block' : 'none');
    })

    $('#motivoDistocia').change(function () {
        $('#EspMotivoDistocia').css('display', $(this).val() == 3 ? 'block' : 'none');
    })

    $('#ectopico').change(function () {
        $('#EspEctopico').css('display', $(this).val() == 1 ? 'block' : 'none');
    })

    $('#prevPregnancy').change(function () {
        $('#EspPrevPregnancy').css('display', $(this).val() == 1 ? 'block' : 'none');
    })

    $('#perinatales').change(function () {
        $('#EspPerinatales').css('display', $(this).val() == 1 ? 'block' : 'none');
    })

    $('#anembrionicos').change(function () {
        $('#EspAnembrionicos').css('display', $(this).val() == 1 ? 'block' : 'none');
    })



    //Calcular semanas de gestacion
    $('#ultMenstruacion').blur(function () {
        var now = new Date();
        var _MS_PER_DAY = 1000 * 60 * 60 * 24;

        var date = $('#ultMenstruacion').val();
        if (date != '')
        {
            var menstruation = new Date(date.split('/')[2], (parseInt(date.split('/')[1]) - 1), date.split('/')[0]);
            
            var utc1 = Date.UTC(now.getFullYear(), now.getMonth(), now.getDate());
            var utc2 = Date.UTC(menstruation.getFullYear(), menstruation.getMonth(), menstruation.getDate());

            var diffDays = Math.floor((utc1 - utc2) / _MS_PER_DAY);
            
            if (diffDays > 0)
            {
                var weeks = 0;
                var days = 0;

                var rank = 6;
                var pending = false;
                for (var i = 0; i < diffDays; i++) {
                    if (i == rank) {
                        weeks++;
                        rank = rank + 7;
                        pending = false;
                    } else pending = true;
                }

                if (pending) {
                    if (rank > 6) {
                        for (var i = (weeks * 7) ; i <= diffDays; i++) {
                            days++;
                        }
                    } else days = diffDays;
                }

                $('#gestacion').val(weeks + ' / ' + days);
            }
        }
    })


    $('#goGinecologia').click(function () {
        window.location.href = 'ConsultaGinecologica?idPac=' + window.idpac;
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

function PesoMadre() {
    var peso = $('#peso').val();
    var producto = $('#pesoProducto').val();
    var pesoM = $('#pesoMadre');

    var value = null;
    if (peso != '' && producto != '') {
        if (!isNaN(peso) && !isNaN(producto))
            value = peso - producto;
        else pesoM.val('');
    } else pesoM.val('');

    if (value != null)
        pesoM.val(parseFloat(value).toFixed(2));
}


function SHPreviousQuery() {

    var previous = $('#previousQuery');
    var query = $('#query');
    var btn = $('#btnPrevious');

    var showPrevious = previous.css('display') == 'none' ? false : true;
    if (showPrevious)
    {
        previous.hide();
        showPrevious = false;
        btn.text('Mostrar Consulta Anterior');
        $('.contenido-de-panel label').css({ 'white-space': 'pre', 'font-size': '12px', 'word-break': 'break-all' });
        $('* .n1').removeClass('col-lg-4').addClass('col-lg-2');
        $('.n-1').addClass('col-lg-6');
        $('* .n3').removeClass('col-lg-6').addClass('col-lg-3');
        $('.n4').removeClass('col-lg-6').addClass('col-lg-2');
        $('* .n5').removeClass('col-lg-6').addClass('col-lg-2');
        $('.n6').removeClass('col-lg-12').addClass('col-lg-4');
        $('.n7').removeClass('col-lg-12').addClass('col-lg-4');
        $('.n8').removeClass('col-lg-12').addClass('col-lg-9');
        $('.n9').addClass('col-lg-3');
        $('* .n10').removeClass('col-lg-6').addClass('col-lg-3');
        $('.n-10').addClass('col-lg-6');
        $('* .n11').removeClass('col-lg-6').addClass('col-lg-3');
        $('* .n12').removeClass('col-lg-4').addClass('col-lg-2');
        $('.n13').removeClass('col-lg-6').addClass('col-lg-4');
        $('.n14').removeClass('col-lg-6').addClass('col-lg-2');

    } else
    {
        previous.show();
        showPrevious = true;
        btn.text('Ocultar Consulta Anterior');
        $('.contenido-de-panel label').css({ 'white-space': 'pre-line', 'font-size': '8px', 'word-break': 'normal' });
        $('* .n1').removeClass('col-lg-2').addClass('col-lg-4');
        $('.n-1').removeClass('col-lg-6');
        $('* .n3').removeClass('col-lg-3').addClass('col-lg-6');
        $('.n4').removeClass('col-lg-2').addClass('col-lg-6');
        $('* .n5').removeClass('col-lg-2').addClass('col-lg-6');
        $('.n6').removeClass('col-lg-4').addClass('col-lg-12');
        $('.n7').removeClass('col-lg-4').addClass('col-lg-12');
        $('.n8').removeClass('col-lg-9').addClass('col-lg-12');
        $('.n9').removeClass('col-lg-3');
        $('* .n10').removeClass('col-lg-3').addClass('col-lg-6');
        $('.n-10').removeClass('col-lg-6');
        $('* .n11').removeClass('col-lg-3').addClass('col-lg-6');
        $('* .n12').removeClass('col-lg-2').addClass('col-lg-4');
        $('.n13').removeClass('col-lg-4').addClass('col-lg-6');
        $('.n14').removeClass('col-lg-2').addClass('col-lg-6');

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
    var req = new Object();
    req.idpac = window.idpac;
    req.noEmbarazo = $('#noEmbarazo').val();
    req.peso = $('#peso').val() == '' ? null : parseFloat($('#peso').val());
    req.talla = $('#talla').val() == '' ? null : parseFloat($('#talla').val());
    req.activaSexualmente = convertToBool($('#activa').val());
    req.temperatura = $('#temperatura').val() == '' ? null : parseFloat($('#temperatura').val());
    req.sistolica = $('#sistolica').val() == '' ? null : parseInt($('#sistolica').val());
    req.diastolica = $('#diastolica').val() == '' ? null : parseInt($('#diastolica').val());
    req.abortos = parseFloat($('#abortos').val());
    req.fechaUltimoParto = $('#ultParto').val();
    req.ultimaMenstruacion = $('#ultMenstruacion').val();
    req.toxemias = $('#toxemias').val();
    req.EspToxemias = $('#TxtToxemias').val();
    var parto = $('#partos').val();
    req.partos = parto;
    req.tipoDistocia = parto == 1 ? $('#distocia').val() : null;
    req.EspTipoDistocia = $('#EspTipoDistocia').val();
    req.motivoDistocia = parto == 1 ? $('#motivoDistocia').val() : null;
    req.EspMotivoDistocia = $('#TxtMotivoDistocia').val();
    req.cesareas = $('#cesareas').val();
    req.forceps = $('#forceps').val();
    req.mortinatos = $('#mortinatos').val();
    req.rnvivos = $('#rnvivos').val();
    req.EmbarazosEctopicos = $('#ectopico').val();
    req.EspEmbarazosEctopicos = $('#TxtEctopico').val();
    req.complicacionEmbarazo = $('#prevPregnancy').val();
    req.EspComplicacionEmbarazo = $('#TxtComplicacionesEmbarazo').val();
    req.perinatales = $('#perinatales').val();
    req.EspPerinatales = $('#TxtPerinatales').val();
    req.anembrionicos = $('#anembrionicos').val();
    req.EspAnembrionicos = $('#TxtAnembrionicos').val();
    req.observaciones = $('#observation').val();

    //Control de embarazo
    req.fu = $('#fu').val() == '' ? null : parseFloat($('#fu').val());
    req.fcf = $('#fcf').val() == '' ? null : parseFloat($('#fcf').val());
    req.cc = $('#cc').val() == '' ? null : parseFloat($('#cc').val());
    req.ca = $('#ca').val() == '' ? null : parseFloat($('#ca').val());
    req.lf = $('#lf').val() == '' ? null : parseFloat($('#lf').val());
    req.dbp = $('#dbp').val() == '' ? null : parseFloat($('#dbp').val());
    req.posicion = $('#posicion').val();
    req.presentacion = $('#presentacion').val();
    req.situacion = $('#situacion').val();
    req.actitud = $('#actitud').val();
    req.fetales = $('#fetales').val();
    req.pesoProducto = $('#pesoProducto').val() == '' ? null : parseFloat($('#pesoProducto').val());
    req.ta = $('#ta').val() == '' ? null : parseFloat($('#ta').val());
    req.fcm = $('#fcm').val() == '' ? null : parseFloat($('#fcm').val());
    req.edema = $('#edema').val();
    req.hizoUs = convertToBool($('#hizous').val());
    req.ultrasonido = $('#ultrazonido').val();
    req.motivoConsulta = $('#motivoConsulta').val();
    req.exploracion = $('#exploracion').val();

    $.ajax({
        url: 'ConsultaObstetrica'
            , data: req
            , method: 'POST'
            , success: function (res) {
                if (res.status == "true") {
                    alert('Consulta guardada correctamente.');
                    $('* #myForm')[0].reset();
                    window.location.href = 'Pacientes';
                } else
                {
                    alert('Hubo un error al intentar guardar la consulta, intente de nuevo');
                }
            }
        , error: function () {
            alert('Hubo un error al intentar guardar la consulta, intente de nuevo');
        }
    })
}

function convertToBool(string) {
    return string === '1' ? true : false;
}


function ChangePreviousQuery(id) {
    $.ajax({
        url: 'ConsultaObstetricaPorId'
            , data: { idPac: idpac, idquery: id }
            , method: 'POST'
            , success: function (res) {
                if (res.hasOwnProperty('consulta')) {
                    var peso = res.consulta[0].Peso == null ? 0 : parseFloat(res.consulta[0].Peso);
                    var altura = res.consulta[0].Altura == null ? 0 : parseFloat(res.consulta[0].Altura);
                    var masa = 0;
                    if (peso > 0 && altura > 0) {
                        masa = peso / (altura * altura);
                    }

                    $('#pPeso').html(peso > 0 ? peso : '&nbsp;');
                    $('#pTalla').html(altura > 0 ? altura : '&nbsp;');
                    $('#pMasa').html(masa > 0 ? masa.toFixed(2) : '&nbsp;');
                    $('#pEmbarazo').html(res.consulta[0].obst.noembarazo == null ? '&nbsp;' : '# ' + res.consulta[0].obst.noembarazo);
                    $('#pActiva').html(res.consulta[0].obst.activaSexualmente == null ? '&nbsp;' : (res.consulta[0].obst.activaSexualmente ? 'Si' : 'No'));
                    $('#pTempe').html(res.consulta[0].Temperatura == null ? '&nbsp;' : res.consulta[0].Temperatura);
                    $('#psistolica').html(res.consulta[0].TensionArterial == null ? '&nbsp;' : res.consulta[0].TensionArterial);
                    $('#pdistolica').html(res.consulta[0].TensionArterialB == null ? '&nbsp;' : res.consulta[0].TensionArterialB);
                    $('#pAbortos').html(res.consulta[0].obst.abortos == null ? '&nbsp;' : res.consulta[0].obst.abortos);
                    $('#pFechaParto').html(res.fUP);
                    $('#pFechaMenstruacion').html(res.fUM);
                    var toxemias = res.consulta[0].obst.ToxemiasPrevias;
                    $('#pToxemias').html(toxemias == 0 ? 'No' : 'Si');
                    if (toxemias == 1) $('#pEspToxemias').show();
                    else $('#pEspToxemias').hide();
                    $('#pPartos').html(res.consulta[0].obst.Partos == 0 ? 'Eut&oacute;cico' : 'Distocico');
                    $('#showpPartos').css('display', res.consulta[0].obst.Partos == 1 ? 'block' : 'none');

                    var tipoDistocia = '&nbsp;';
                    switch (res.consulta[0].obst.TipoDistocia) {
                        case 0:
                            tipoDistocia = 'Ces&aacute;rea';
                            break;
                        case 1:
                            tipoDistocia = 'F&oacute;rceps';
                            break;
                        case 2:
                            tipoDistocia = 'Otros &minus;&minus;&gt;';
                            break;
                    }

                    $('#pTipoDistocia').html(tipoDistocia);
                    $('#pShowTipoDistocia').css('display', res.consulta[0].obst.TipoDistocia == 2 ? 'block' : 'none');
                    $('#tipoooDistocia').html(res.consulta[0].obst.EspecifiqueTipoDistocia == null ? '&nbsp;' : res.consulta[0].obst.EspecifiqueTipoDistocia);
                    var motivoDistocia = '&nbsp;';
                    switch (res.consulta[0].obst.MotivoDistocia) {
                        case 0:
                            motivoDistocia = 'Sufrimiento fetal agudo';
                            break;
                        case 1:
                            motivoDistocia = 'Postura an&oacute;mala';
                            break;
                        case 2:
                            motivoDistocia = 'Prematurez';
                            break;
                        case 3:
                            motivoDistocia = 'Otros &minus;&minus;&gt;';
                            break;
                    }

                    $('#pMotivoDistocia').html(motivoDistocia);
                    $('#pShowMotivoDistocia').css('display', res.consulta[0].obst.MotivoDistocia == 3 ? 'block' : 'none');
                    $('#motivoooDistocia').html(res.consulta[0].obst.EspecifiqueMotivoDistocia == null ? '&nbsp;' : res.consulta[0].obst.EspecifiqueMotivoDistocia);

                    $('#pCesareas').html(res.consulta[0].obst.CesareasPrevia == null ? '&nbsp;' : res.consulta[0].obst.CesareasPrevia);
                    $('#pForceps').html(res.consulta[0].obst.UsoDeForceps == null ? '&nbsp;' : res.consulta[0].obst.UsoDeForceps);
                    $('#pMortinatos').html(res.consulta[0].obst.Motinatos == null ? '&nbsp;' : res.consulta[0].obst.Motinatos);
                    $('#pRNVivos').html(res.consulta[0].obst.RMVivos == null ? '&nbsp;' : res.consulta[0].obst.RMVivos);
                    $('#pEctopicos').html(res.consulta[0].obst.EmbarazoEtopicos == 0 ? 'No' : 'Si');
                    $('#pShowEctopico').css('display', res.consulta[0].obst.EmbarazoEtopicos == 0 ? 'none' : 'block');
                    $('#EspEctooopico').html(res.consulta[0].obst.EmbrazoEtopicoExplique == null ? '&nbsp;' : res.consulta[0].obst.EmbrazoEtopicoExplique);

                    $('#pEmbarazosPrevios').html(res.consulta[0].obst.EmbrazosComplicadosPrevios == 0 ? 'No' : 'Si');
                    $('#pShowEmbarazosPrevios').css('display', res.consulta[0].obst.EmbrazosComplicadosPrevios == 0 ? 'none' : 'block');
                    $('#EspEmbarazoPrevio').html(res.consulta[0].obst.EmbarazosComplicadosExplique == null ? '&nbsp;' : res.consulta[0].obst.EmbarazosComplicadosExplique);

                    $('#pPerinatales').html(res.consulta[0].obst.NoComplicacionesPertinales == 0 ? 'No' : 'Si');
                    $('#pShowPerinatales').css('display', res.consulta[0].obst.NoComplicacionesPertinales == 0 ? 'none' : 'block');
                    $('#periiinatales').html(res.consulta[0].obst.ComplicacionesPerinatalesExplique == null ? '&nbsp;' : res.consulta[0].obst.ComplicacionesPerinatalesExplique);

                    $('#pAnormales').html(res.consulta[0].obst.NoEmbrazosAnormales == 0 ? 'No' : 'Si');
                    $('#pShowAnormales').css('display', res.consulta[0].obst.NoEmbrazosAnormales == 0 ? 'none' : 'block');
                    $('#anoormales').html(res.consulta[0].obst.EmbarazosAnormalesExplique == null ? '&nbsp;' : res.consulta[0].obst.EmbarazosAnormalesExplique);

                    $('#pGestacion').html(res.gestacion);
                    $('#pfu').html(res.consulta[0].obst.FU == null ? '&nbsp;' : res.consulta[0].obst.FU);
                    $('#pfcf').html(res.consulta[0].obst.FCF == null ? '&nbsp;' : res.consulta[0].obst.FCF);
                    $('#pcc').html(res.consulta[0].obst.CC == null ? '&nbsp;' : res.consulta[0].obst.CC);
                    $('#pca').html(res.consulta[0].obst.CA == null ? '&nbsp;' : res.consulta[0].obst.CA);
                    $('#plf').html(res.consulta[0].obst.LF == null ? '&nbsp;' : res.consulta[0].obst.LF);
                    $('#pdbp').html(res.consulta[0].obst.DSP == null ? '&nbsp;' : res.consulta[0].obst.DSP);

                    $('#pposicion').html(res.consulta[0].obst.Posicion == null ? '&nbsp;' : res.consulta[0].obst.Posicion);
                    $('#ppresentacion').html(res.consulta[0].obst.Presentacion == null ? '&nbsp;' : res.consulta[0].obst.Presentacion);
                    $('#psituacion').html(res.consulta[0].obst.siuacuion == null ? '&nbsp;' : res.consulta[0].obst.siuacuion);
                    $('#pactitud').html(res.consulta[0].obst.Actitud == null ? '&nbsp;' : res.consulta[0].obst.Actitud);

                    $('#pfetales').html(res.consulta[0].obst.MovimientosFetales == null ? '&nbsp;' : res.consulta[0].obst.MovimientosFetales);
                    $('#ppesoProducto').html(res.consulta[0].obst.PesoAproxProducto == null ? '&nbsp;' : res.consulta[0].obst.PesoAproxProducto);
                    $('#ppesoMadre').html('');
                    $('#pta').html(res.consulta[0].obst.TA == null ? '&nbsp;' : res.consulta[0].obst.TA);

                    $('#pcfm').html(res.consulta[0].obst.FCM == null ? '&nbsp;' : res.consulta[0].obst.FCM);
                    $('#pedema').html(res.consulta[0].obst.Edema == null ? '&nbsp;' : res.consulta[0].obst.Edema);
                    $('#phizoUS').html(res.consulta[0].obst.TA == null ? '&nbsp;' : (res.consulta[0].obst.TA ? 'Si' : 'No'));

                    $('#uuultrazonido').html('');
                    $('#mootiivo').html(res.consulta[0].motivo == null ? '&nbsp;' : res.consulta[0].motivo);
                    $('#explooracion').html('');
                }
            }
    })
}