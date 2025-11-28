using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IValorServicoAppService : IAppServiceBase<VALOR_SERVICO>
    {
        Int32 ValidateCreate(VALOR_SERVICO item, USUARIO usuario);
        Int32 ValidateEdit(VALOR_SERVICO item, VALOR_SERVICO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(VALOR_SERVICO item, USUARIO usuario);
        Int32 ValidateReativar(VALOR_SERVICO item, USUARIO usuario);

        VALOR_SERVICO CheckExist(VALOR_SERVICO item, Int32 idAss);
        List<VALOR_SERVICO> GetAllItens(Int32 idAss);
        VALOR_SERVICO GetItemById(Int32 id);
        List<VALOR_SERVICO> GetAllItensAdm(Int32 idAss);

        List<TIPO_SERVICO_CONSULTA> GetAllServicos(Int32 idAss);
    }
}
