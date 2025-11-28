using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITipoValorServicoAppService : IAppServiceBase<TIPO_SERVICO_CONSULTA>
    {
        Int32 ValidateCreate(TIPO_SERVICO_CONSULTA item, USUARIO usuario);
        Int32 ValidateEdit(TIPO_SERVICO_CONSULTA item, TIPO_SERVICO_CONSULTA itemAntes, USUARIO usuario);
        Int32 ValidateDelete(TIPO_SERVICO_CONSULTA item, USUARIO usuario);
        Int32 ValidateReativar(TIPO_SERVICO_CONSULTA item, USUARIO usuario);

        TIPO_SERVICO_CONSULTA CheckExist(TIPO_SERVICO_CONSULTA item, Int32 idAss);
        List<TIPO_SERVICO_CONSULTA> GetAllItens(Int32 idAss);
        TIPO_SERVICO_CONSULTA GetItemById(Int32 id);
        List<TIPO_SERVICO_CONSULTA> GetAllItensAdm(Int32 idAss);

    }
}
