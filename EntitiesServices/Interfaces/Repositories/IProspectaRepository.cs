using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IProspectaRepository : IRepositoryBase<PROSPECTA_MAIL>
    {
        List<PROSPECTA_MAIL> GetAllItens();
        PROSPECTA_MAIL GetItemById(Int32 id);
        List<PROSPECTA_MAIL> ExecuteFilter(DateTime? entrada, String cidade, String uf, Int32? enviado);
    }
}
