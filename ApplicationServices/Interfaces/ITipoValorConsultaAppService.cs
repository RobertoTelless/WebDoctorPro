using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITipoValorConsultaAppService : IAppServiceBase<TIPO_VALOR_CONSULTA>
    {
        Int32 ValidateCreate(TIPO_VALOR_CONSULTA item, USUARIO usuario);
        Int32 ValidateCreate(TIPO_VALOR_CONSULTA item);
        Int32 ValidateEdit(TIPO_VALOR_CONSULTA item, TIPO_VALOR_CONSULTA itemAntes, USUARIO usuario);
        Int32 ValidateDelete(TIPO_VALOR_CONSULTA item, USUARIO usuario);
        Int32 ValidateReativar(TIPO_VALOR_CONSULTA item, USUARIO usuario);

        TIPO_VALOR_CONSULTA CheckExist(TIPO_VALOR_CONSULTA item, Int32 idAss);
        List<TIPO_VALOR_CONSULTA> GetAllItens(Int32 idAss);
        TIPO_VALOR_CONSULTA GetItemById(Int32 id);
        List<TIPO_VALOR_CONSULTA> GetAllItensAdm(Int32 idAss);

    }
}
