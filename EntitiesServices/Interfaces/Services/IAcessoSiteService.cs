using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IAcessoSiteService : IServiceBase<ACESSO_SITE>
    {
        Int32 Create(ACESSO_SITE item);
        Int32 Edit(ACESSO_SITE item);

        List<ACESSO_SITE> GetAllItens();
        ACESSO_SITE GetItemById(Int32 id);
    }
}
