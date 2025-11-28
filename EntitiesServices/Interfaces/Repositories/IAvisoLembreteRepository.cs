using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAvisoLembreteRepository : IRepositoryBase<AVISO_LEMBRETE>
    {
        List<AVISO_LEMBRETE> GetAllItens(Int32 idAss);
        AVISO_LEMBRETE GetItemById(Int32 id);
        List<AVISO_LEMBRETE> ExecuteFilter(String titulo, DateTime? inicio, DateTime? final, Int32? ciente, Int32 idAss);
    }
}
