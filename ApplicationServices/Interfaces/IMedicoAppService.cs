using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IMedicoAppService : IAppServiceBase<MEDICOS>
    {
        Int32 ValidateCreate(MEDICOS item, USUARIO usuario);
        Int32 ValidateEdit(MEDICOS item, MEDICOS itemAntes, USUARIO usuario);
        Int32 ValidateDelete(MEDICOS item, USUARIO usuario);
        Int32 ValidateReativar(MEDICOS item, USUARIO usuario);

        MEDICOS CheckExist(MEDICOS item, Int32 idAss);
        List<MEDICOS> GetAllItens(Int32 idAss);
        MEDICOS GetItemById(Int32 id);
        List<MEDICOS> GetAllItensAdm(Int32 idAss);
        Tuple<Int32, List<MEDICOS>, Boolean> ExecuteFilter(Int32? espec, String nome, String crm, String email, Int32 idAss);
        List<ESPECIALIDADE> GetAllEspec(Int32 idAss);
        List<TIPO_ENVIO> GetAllTipos(Int32 idAss);

        MEDICOS_ENVIO_ANEXO GetMedicoAnexoById(Int32 id);
        Int32 ValidateEditMedicoAnexo(MEDICOS_ENVIO_ANEXO item);

        MEDICOS_ENVIO GetEnvioById(Int32 id);
        Int32 ValidateEditEnvio(MEDICOS_ENVIO item);
        Int32 ValidateCreateEnvio(MEDICOS_ENVIO item);
        List<MEDICOS_ENVIO> GetAllEnvio(Int32 idAss);

        Int32 ValidateEditAnotacao(MEDICOS_ENVIO_ANOTACAO item);
        MEDICOS_ENVIO_ANOTACAO GetAnotacaoById(Int32 id);

        MEDICOS_MENSAGEM CheckExistTextoMensagem(MEDICOS_MENSAGEM item, Int32 idAss);
        List<MEDICOS_MENSAGEM> GetAllTextoMensagem(Int32 idAss);
        MEDICOS_MENSAGEM GetTextoMensagemById(Int32 id);
        Int32 ValidateEditTextoMensagem(MEDICOS_MENSAGEM item);
        Int32 ValidateCreateTextoMensagem(MEDICOS_MENSAGEM item);

    }
}
