

/**
 * Mostrar/Ocultar Contenedor de consultas anteriores.
 *
 **/
var isHidden;
isHidden = true;
$('#btn-mostrar-consulta-anterior').click(function (e) {

    e.preventDefault();

    if (isHidden) {
        // Actualizamos el mensaje del boton.
        $('#btn-mostrar-consulta-anterior').text('Ocultar Consulta Anterior');

        // Redimencionamos el contenedor de la nueva consulta a una medida de 6 columnas.
        $('#nueva-consulta').removeClass('col-lg-12');
        $('#nueva-consulta').addClass('col-lg-6');


        $('#consulta-anterior').removeClass('hidden');

        // $('#consulta-anterior').addClass('fadeInRight');

        isHidden = false;
    } else {

        // Actualizamos el mensaje del boton.
        $('#btn-mostrar-consulta-anterior').text('Mostrar Consulta Anterior');

        // Regresamos el contenedor al ancho de 12 columnas.
        $('#nueva-consulta').removeClass('col-lg-6');



        $('#nueva-consulta').addClass('animated col-lg-12');

       
        $('#consulta-anterior').addClass('hidden');

        isHidden = true;

    }

});




