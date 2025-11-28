using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPagamentoNotaRepository : IRepositoryBase<PAGAMENTO_NOTA_FISCAL>
    {
        List<PAGAMENTO_NOTA_FISCAL> GetAllItens();
        PAGAMENTO_NOTA_FISCAL GetItemById(Int32 id);
    }
}
