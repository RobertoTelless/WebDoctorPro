using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IControleMensagemAppService : IAppServiceBase<CONTROLE_MENSAGEM>
    {
        Int32 ValidateCreate(CONTROLE_MENSAGEM perfil, USUARIO usuario);
        Int32 ValidateEdit(CONTROLE_MENSAGEM perfil, CONTROLE_MENSAGEM perfilAntes, USUARIO usuario);

        List<CONTROLE_MENSAGEM> GetAllItens(Int32 idAss);
        CONTROLE_MENSAGEM GetItemById(Int32 id);
        CONTROLE_MENSAGEM CheckExist(CONTROLE_MENSAGEM conta, Int32 idAss);
        CONTROLE_MENSAGEM GetItemByDate(DateTime data, Int32 idAss);
    }
}
