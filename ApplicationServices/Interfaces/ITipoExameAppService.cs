using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITipoExameAppService : IAppServiceBase<TIPO_EXAME>
    {
        Int32 ValidateCreate(TIPO_EXAME item, USUARIO usuario);
        Int32 ValidateCreate(TIPO_EXAME item);
        Int32 ValidateEdit(TIPO_EXAME item, TIPO_EXAME itemAntes, USUARIO usuario);
        Int32 ValidateDelete(TIPO_EXAME item, USUARIO usuario);
        Int32 ValidateReativar(TIPO_EXAME item, USUARIO usuario);

        TIPO_EXAME CheckExist(TIPO_EXAME item, Int32 idAss);
        List<TIPO_EXAME> GetAllItens(Int32 idAss);
        TIPO_EXAME GetItemById(Int32 id);
        List<TIPO_EXAME> GetAllItensAdm(Int32 idAss);

    }
}
