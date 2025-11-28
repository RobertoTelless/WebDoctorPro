using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IRecebimentoAnexoRepository : IRepositoryBase<RECEBIMENTO_ANEXO>
    {
        List<RECEBIMENTO_ANEXO> GetAllItens();
        RECEBIMENTO_ANEXO GetItemById(Int32 id);
    }
}
