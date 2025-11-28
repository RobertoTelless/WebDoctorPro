using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IRecebimentoReciboRepository : IRepositoryBase<RECEBIMENTO_RECIBO>
    {
        List<RECEBIMENTO_RECIBO> GetAllItens();
        RECEBIMENTO_RECIBO GetItemById(Int32 id);
    }
}
