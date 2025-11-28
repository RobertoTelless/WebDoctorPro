using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPagamentoAnexoRepository : IRepositoryBase<PAGAMENTO_ANEXO>
    {
        List<PAGAMENTO_ANEXO> GetAllItens();
        PAGAMENTO_ANEXO GetItemById(Int32 id);
    }
}
