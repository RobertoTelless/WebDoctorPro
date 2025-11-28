using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ILocacaoAnexoRepository : IRepositoryBase<LOCACAO_ANEXO>
    {
        List<LOCACAO_ANEXO> GetAllItens();
        LOCACAO_ANEXO GetItemById(Int32 id);
    }
}
