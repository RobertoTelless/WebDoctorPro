using System;
using System.Collections.Generic;
using System.Web;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IMensagemAppService : IAppServiceBase<MENSAGENS>
    {
        Int32 ValidateCreate(MENSAGENS item, USUARIO usuario);
        Int32 ValidateEdit(MENSAGENS item, MENSAGENS perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(MENSAGENS item, MENSAGENS itemAntes);
        Int32 ValidateDelete(MENSAGENS item, USUARIO usuario);
        Int32 ValidateReativar(MENSAGENS item, USUARIO usuario);

        List<MENSAGENS> GetAllItens(Int32 idAss);
        List<MENSAGENS> GetAllItensAdm(Int32 idAss);
        MENSAGENS GetItemById(Int32 id);
        MENSAGENS CheckExist(MENSAGENS conta, Int32 idAss);
        MENSAGEM_ANEXO GetAnexoById(Int32 id);
        Tuple<Int32, List<MENSAGENS>, Boolean> ExecuteFilterSMS(DateTime? envio, DateTime? faixa, Int32 cliente, String texto, Int32 idAss);
        Tuple<Int32, List<MENSAGENS>, Boolean> ExecuteFilterEMail(DateTime? envio, DateTime? faixa, Int32 cliente, String texto, Int32 idAss);
        Tuple<Int32, List<RESULTADO_ROBOT>, Boolean> ExecuteFilterRobot(Int32? tipo, DateTime? inicio, DateTime? final, String cliente, String email, String celular, Int32? status, Int32 idAss);

        List<UF> GetAllUF();
        List<TEMPLATE_EMAIL> GetAllTemplate(Int32 idAss);
        UF GetUFbySigla(String sigla);
        List<RESULTADO_ROBOT> GetAllEnviosRobot(Int32 idAss);

        List<MENSAGENS_DESTINOS> GetAllDestinos(Int32 idAss);
        MENSAGENS_DESTINOS GetDestinoById(Int32 id);
        Int32 ValidateEditDestino(MENSAGENS_DESTINOS item);
        Int32 ValidateCreateDestino(MENSAGENS_DESTINOS item);

        void SendEmailScheduleAsync(Int32? mensId, HttpServerUtilityBase server);
    }
}
