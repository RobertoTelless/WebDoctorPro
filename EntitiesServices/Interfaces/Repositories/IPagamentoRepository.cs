using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPagamentoRepository : IRepositoryBase<CONSULTA_PAGAMENTO>
    {
        List<CONSULTA_PAGAMENTO> GetAllItens(Int32 idAss);
        CONSULTA_PAGAMENTO GetItemById(Int32 id);
        List<CONSULTA_PAGAMENTO> ExecuteFilter(Int32? tipo, String nome, String favorecido, DateTime? inicio, DateTime? final, Int32? conferido, Int32 idAss);
    }
}
