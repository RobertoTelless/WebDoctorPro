using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPacientePrescricaoItemRepository : IRepositoryBase<PACIENTE_PRESCRICAO_ITEM>
    {
        List<PACIENTE_PRESCRICAO_ITEM> GetAllItens(Int32 idUsu);
        PACIENTE_PRESCRICAO_ITEM GetItemById(Int32 id);
        List<PACIENTE_PRESCRICAO_ITEM> ExecuteFilter(Int32? forma, String nome, DateTime? inicio, DateTime? final, String remedio, String generico, Int32 idAss);
    }
}
