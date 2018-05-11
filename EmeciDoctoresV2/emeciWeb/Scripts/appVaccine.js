angular.module('app', []);

(function (_module) {

    vaccineController.$inject = ['$scope'];
    function vaccineController(s) {

        s.json = [];

        var vaccine = document.getElementById('Jsonvaccine').value;
        if (vaccine !== null) {
            s.json = JSON.parse(vaccine);
            applyCss();
            validJson();
        }


        function applyCss() {
            if (s.json.length) {
                var colors =
                ['#e5fff7', '#ffeecc', '#edffe5', '#ebe5ff', '#c2ffa8', '#e5ffff', '#fff2e5', '#ffe5e5', '#d9ffb3'];

                var i = 0;
                s.json.map(function (b) {
                    b.css = colors[i];
                    if (i === colors.length) i = 0;
                    i++;
                });
            }
        }

        function validJson() {
            if (s.json.length) {
                var temp = [];

                var obj = new Object();
                s.json.map(function (n) {
                    obj = JSON.parse(JSON.stringify(n));
                    if (n.doses.length) {
                        delete obj.doses;
                        var i = 0;
                        n.doses.map(function (d) {

                            temp.push({
                                date: '<input class="null" type="text" value="' + d.date.toString().replace('-', '/').replace('-', '/') + '" calendar />'
                                , age: d.age
                                , dose: d.dose
                                , name: (i > 0 ? 'no' : obj.name)
                                , prev_disease: (i > 0 ? 'no' : obj.prev_disease)
                                , rowspan: (i > 0 ? 1 : obj.rowspan)
                                , css: obj.css
                                , code: obj.code
                            });

                            i++;
                        });

                        temp.push({
                            date: '<a dose class="linkDose">Agregar dosis</a>'
                            , age: ''
                            , dose: ''
                            , prev_disease: 'no'
                            , name: 'no'
                            , code: obj.code
                            , css: obj.css
                            , rowspan : 1
                        });
                    } else {
                        obj.date = '<a dose class="linkDose">Agregar dosis</a>';
                        obj.age = '';
                        obj.dose = '';
                        obj.rowspan = 1;
                        delete obj.doses;
                        temp.push(obj);
                    }
                });

                s.json = temp;
            }
        }
    }


    //DIRECTIVAS
    html.$inject = ['$compile'];
    function html(c) {
        var _return = { restrict: 'A', link: undefined };
        _return.link = function (s, element, attr) {
            var compile = c(attr.html)(s);
            $(element).html(compile);
        }

        return _return;
    }

    dose.$inject = ['$compile'];
    function dose(c) {
        var _return = { restrict: 'A', link: undefined };
        _return.link = function (s, element, attr) {
            element.bind('click', function () {

                var tag = $(element).parent().parent();
                var allRows = $('* #' + tag[0].id);

                var rowspan = allRows.length;

                var allColumns = allRows.eq(rowspan - 1).children();
                rowspan++;
                allRows.eq(0).children().eq(0).attr('rowspan', rowspan)
                allRows.eq(0).children().eq(1).attr('rowspan', rowspan);

                var b = 0;
                for (var i = (rowspan === 2 ? 2 : 0) ; i <= (rowspan === 2 ? 4 : 2) ; i++) {
                    if (b === 2) {
                        var compile = c('<input class="null" type="text" calendar placeholder="dd/mm/yyyy" />')(s);
                        allColumns.eq(i).html(compile);

                    } else allColumns.eq(i).html('<input type="text" />');

                    b++;
                }


                var newRow = '<tr style="' + tag.attr('style') + '" id="' + tag.attr('id') + '">'
                newRow += '<td></td>'
                newRow += '<td></td>';
                newRow += '<td><a class="linkDose" dose>Agregar dosis</a></td>';
                newRow += '</tr>';
                var compile = c(newRow)(s);

                tag.after(compile);
            });
        }

        return _return;
    }

    Calendar.$inject = ['$parse'];
    function Calendar(p) {
        var _return = { restrict: 'A', link: undefined };
        _return.link = function (s, element, attr) {
            var ngModel = p(attr.ngModel);
            var opt =
            {
                field: element[0]
                , numberOfMonths: 1
                , bound: true
                , defaultDate: new Date()
            }

            var picker = new Pikaday(opt);
        }

        return _return;
    }

    _module.controller('vController', vaccineController);
    _module.directive('html', html);
    _module.directive('dose', dose);
    _module.directive('calendar', Calendar);
})(angular.module('app'));