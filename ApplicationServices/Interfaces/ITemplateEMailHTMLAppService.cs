using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITemplateEMailHTMLAppService : IAppServiceBase<TEMPLATE_EMAIL_HTML>
    {
        Int32 ValidateCreate(TEMPLATE_EMAIL_HTML item, USUARIO usuario);
        Int32 ValidateEdit(TEMPLATE_EMAIL_HTML item, TEMPLATE_EMAIL_HTML itemAntes, USUARIO usuario);
        Int32 ValidateDelete(TEMPLATE_EMAIL_HTML item, USUARIO usuario);
        Int32 ValidateReativar(TEMPLATE_EMAIL_HTML item, USUARIO usuario);

        TEMPLATE_EMAIL_HTML GetItemByNome(String nome);
        TEMPLATE_EMAIL_HTML CheckExist(TEMPLATE_EMAIL_HTML item, Int32 idAss);
        List<TEMPLATE_EMAIL_HTML> GetAllItens(Int32 idAss);
        TEMPLATE_EMAIL_HTML GetItemById(Int32 id);
        List<TEMPLATE_EMAIL_HTML> GetAllItensAdm(Int32 idAss);
    }
}
