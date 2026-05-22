using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ILeadAnexoRepository : IRepositoryBase<LEAD_ANEXO>
    {
        List<LEAD_ANEXO> GetAllItens();
        LEAD_ANEXO GetItemById(Int32 id);
    }
}
