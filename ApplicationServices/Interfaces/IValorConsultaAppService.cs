using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IValorConsultaAppService : IAppServiceBase<VALOR_CONSULTA>
    {
        Int32 ValidateCreate(VALOR_CONSULTA item, USUARIO usuario);
        Int32 ValidateEdit(VALOR_CONSULTA item, VALOR_CONSULTA itemAntes, USUARIO usuario);
        Int32 ValidateDelete(VALOR_CONSULTA item, USUARIO usuario);
        Int32 ValidateReativar(VALOR_CONSULTA item, USUARIO usuario);

        VALOR_CONSULTA CheckExist(VALOR_CONSULTA item, Int32 idAss);
        List<VALOR_CONSULTA> GetAllItens(Int32 idAss);
        VALOR_CONSULTA GetItemById(Int32 id);
        List<VALOR_CONSULTA> GetAllItensAdm(Int32 idAss);

        List<TIPO_VALOR_CONSULTA> GetAllTipos(Int32 idAss);
    }
}
