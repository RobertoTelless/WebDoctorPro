$(function () {
    $("#cpf").mask("999.999.999-99");
    $("#cnpj").mask("99.999.999/9999-99");
    $('.date-picker').mask('99/99/9999');
    $("#cep").mask("99999-999");
    $("#hora").mask("99:99");
    $("#hora1").mask("99:99");
    $('#data').mask('99/99/9999');

});

$(function () {
    $('.date-picker').datepicker(
        {
            dateFormat: 'dd/mm/yy',
            dayNames: ['Domingo', 'Segunda', 'Terça', 'Quarta', 'Quinta', 'Sexta', 'Sábado'],
            dayNamesMin: ['D', 'S', 'T', 'Q', 'Q', 'S', 'S', 'D'],
            dayNamesShort: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb', 'Dom'],
            monthNames: ['Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho', 'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'],
            monthNamesShort: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'],
            nextText: 'Proximo',
            prevText: 'Anterior',
            showOn: "focus"
        }
    )
    .css("display", "normal")
    .next("button").button({
        icons: { primary: "ui-icon-calendar" },
        label: "Selecione uma data",
        text: false
    });
});

$(document).ready(function () {
    $.fn.dataTable.moment('DD/MM/YYYY');

    $('.dataTables-example').DataTable({
        pageLength: 25,
        dom: '<"html5buttons"B>lTfgitp',
        buttons: [
            { extend: 'excel', title: 'Planilha' },
            { extend: 'pdf', title: 'PDF' },
            {
                extend: 'print',
                customize: function (win) {
                    $(win.document.body).addClass('white-bg');
                    $(win.document.body).css('font-size', '10px');

                    $(win.document.body).find('table')
                        .addClass('compact')
                        .css('font-size', 'inherit');
                }
            }
        ]
    });
});



