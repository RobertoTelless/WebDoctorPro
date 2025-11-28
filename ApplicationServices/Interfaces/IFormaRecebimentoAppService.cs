using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IFormaRecebimentoAppService : IAppServiceBase<FORMA_RECEBIMENTO>
    {
        Int32 ValidateCreate(FORMA_RECEBIMENTO item, USUARIO usuario);
        Int32 ValidateEdit(FORMA_RECEBIMENTO item, FORMA_RECEBIMENTO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(FORMA_RECEBIMENTO item, USUARIO usuario);
        Int32 ValidateReativar(FORMA_RECEBIMENTO item, USUARIO usuario);

        FORMA_RECEBIMENTO CheckExist(FORMA_RECEBIMENTO item, Int32 idAss);
        List<FORMA_RECEBIMENTO> GetAllItens(Int32 idAss);
        FORMA_RECEBIMENTO GetItemById(Int32 id);
        List<FORMA_RECEBIMENTO> GetAllItensAdm(Int32 idAss);

    }
}
