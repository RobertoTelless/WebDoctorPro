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

function ExibirMensagem(tipo, mensagem, titulo) {
    titulo = titulo || 'WebDoctor';
    
    // Configurações detalhadas para bater com o comportamento desejado
    toastr.options = {
        "closeButton": true,
        "progressBar": true,
        "preventDuplicates": false,
        "positionClass": "toast-top-right",
        "onclick": null,
        "showDuration": "400",
        "hideDuration": "1000",
        "timeOut": "15000",       // 15 segundos na tela
        "extendedTimeOut": "5000", // +5 segundos se o mouse passar por cima
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };

    // Mapeamento de tipos (aceita string ou número vindo do banco/C#)
    if (tipo === 'sucesso' || tipo === 1) {
        toastr.success(mensagem, titulo);
    } else if (tipo === 'erro' || tipo === 2) {
        // Para erros, você pode opcionalmente travar na tela (timeOut 0)
        // mas aqui mantivemos os 15s conforme solicitado
        toastr.error(mensagem, titulo);
    } else if (tipo === 'aviso' || tipo === 3) {
        toastr.warning(mensagem, titulo);
    } else {
        toastr.info(mensagem, titulo);
    }
}

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



