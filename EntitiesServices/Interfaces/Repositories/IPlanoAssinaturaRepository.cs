using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPlanoAssinaturaRepository : IRepositoryBase<PLANO_ASSINATURA>
    {
        List<PLANO_ASSINATURA> GetAllItens();
        PLANO_ASSINATURA GetItemById(Int32 id);
    }
}
