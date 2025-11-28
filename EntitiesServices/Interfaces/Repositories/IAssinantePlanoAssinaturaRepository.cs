using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAssinantePlanoAssinaturaRepository : IRepositoryBase<ASSINANTE_PLANO_ASSINATURA>
    {
        List<ASSINANTE_PLANO_ASSINATURA> GetAllItens(Int32 idAss);
        ASSINANTE_PLANO_ASSINATURA GetItemById(Int32 id);
    }
}
