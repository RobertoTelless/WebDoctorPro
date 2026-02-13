using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAcessoSiteRepository : IRepositoryBase<ACESSO_SITE>
    {
        List<ACESSO_SITE> GetAllItens();
        ACESSO_SITE GetItemById(Int32 id);
    }
}
