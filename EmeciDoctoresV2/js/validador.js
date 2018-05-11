var validateForm = (function () {

    function validate(input) {
        if (input.value != '') {
            privateSuccess(input); return true;
        } else {
            privateError(input, ''); return false;
        }
    }

    function samePassword(pass, pass2){
        if (pass.value == pass2.value){
            privateSuccess(pass2); return true;
        } else {
            privateError(pass2, 'samePassword'); return false;
        }
    }

    function validateText(input) {
        var validator = /^([a-zA-Z\s])*$/.test(input.value);
        if (validator) {
            privateSuccess(input); return true;
        } else {
            privateError(input, 'validateText'); return false;
        }
    }

    function validatEmail(input) {
        var manyEmails = input.value.split(',')
        var pass = true;
        for (n = 0; n < manyEmails.length; n++) {
            var validator = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/igm;
            var b = validator.test(manyEmails[n].toString().replace(" ", ""));
            pass = (pass && b);
        }

        if (pass) {
            privateSuccess(input); return true;
        } else {
            privateError(input, 'validatEmail'); return false;
        }
    }

    function AlfaNumeric(input) {
        var validator = /^[a-zA-Z0-9/-]+$/.test(input.value);
        if (validator) {
            privateSuccess(input); return true;
        } else {
            if (!/\S+/.test(input.value)) {
                privateSuccess(input); return true;
            }
            else {
                privateError(input, 'AlfaNumeric'); return false;
            }
        }
    }

    function onlyNumber(input) {
        var validator = /^[0-9-]+$/.test(input.value);
        if (validator) {
            privateSuccess(input); return true;
        } else {
            if (!/\S+/.test(input.value)) {
                privateSuccess(input); return true;
            }
            else {
                privateError(input, 'onlyNumber'); return false;
            }
        }
    }

    function privateSuccess(input) {
        if ($('.popupAlert').length > 0){
            $('* .popupAlert').remove();
        }

        input.style.backgroundColor = '';
        input.style.color = '';
        input.style.border = '';
    }

    function privateError(input, typeError) {
        //var error = '';
        /*switch (typeError){
            case 'validateText': error = 'Solo se permiten letras'; break;
            case 'onlyNumber' : error = 'Solo se permiten numeros'; break;
            case 'AlfaNumeric' : error = 'Carecteres validos (a-z, 0-9, /, -)'; break;
            case 'validatEmail' : error = 'Ejemplo: email@host.com'; break;
            case 'samePassword' : error = 'La contraseña no coinciden'; break;
            default : 'Error';
        }

        if ($('.popupAlert').length == 0){
            $(input).parent().append('<div class="popupAlert">' + error + '</div>').show('slow');
        }*/

        input.style.backgroundColor = '#F2DEDE';
        input.style.color = '#B94A48';
        input.style.border = '1px solid #B94A48';
    }

    return {
        Txt: validateText,
        Mail: validatEmail,
        Alfa: AlfaNumeric,
        Num: onlyNumber,
        All : validate,
        pass : samePassword
    };

})();

var findElement = function (elm) {
    var e = document.getElementById(elm);
    if (e) return e;
    else return '';
}