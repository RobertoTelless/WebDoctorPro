using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ILeadAnotacaoRepository : IRepositoryBase<LEAD_ANOTACAO>
    {
        List<LEAD_ANOTACAO> GetAllItens();
        LEAD_ANOTACAO GetItemById(Int32 id);
    }
}
