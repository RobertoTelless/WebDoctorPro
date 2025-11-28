using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITemplateEMailHTMLRepository : IRepositoryBase<TEMPLATE_EMAIL_HTML>
    {
        List<TEMPLATE_EMAIL_HTML> GetAllItens(Int32 idAss);
        TEMPLATE_EMAIL_HTML GetItemById(Int32 id);
        TEMPLATE_EMAIL_HTML GetItemByNome(String nome);
        List<TEMPLATE_EMAIL_HTML> GetAllItensAdm(Int32 idAss);
        TEMPLATE_EMAIL_HTML CheckExist(TEMPLATE_EMAIL_HTML item, Int32 idAss);
    }
}
