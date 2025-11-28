using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITemplateEMailHTMLService : IServiceBase<TEMPLATE_EMAIL_HTML>
    {
        Int32 Create(TEMPLATE_EMAIL_HTML perfil, LOG log);
        Int32 Create(TEMPLATE_EMAIL_HTML perfil);
        Int32 Edit(TEMPLATE_EMAIL_HTML perfil, LOG log);
        Int32 Edit(TEMPLATE_EMAIL_HTML perfil);
        Int32 Delete(TEMPLATE_EMAIL_HTML perfil, LOG log);

        TEMPLATE_EMAIL_HTML GetItemByNome(String nome);
        List<TEMPLATE_EMAIL_HTML> GetAllItens(Int32 idAss);
        TEMPLATE_EMAIL_HTML GetItemById(Int32 id);
        List<TEMPLATE_EMAIL_HTML> GetAllItensAdm(Int32 idAss);
        TEMPLATE_EMAIL_HTML CheckExist(TEMPLATE_EMAIL_HTML item, Int32 idAss);
    }
}
