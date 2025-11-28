using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IRecebimentoRepository : IRepositoryBase<CONSULTA_RECEBIMENTO>
    {
        List<CONSULTA_RECEBIMENTO> GetAllItens(Int32 idAss);
        CONSULTA_RECEBIMENTO GetItemById(Int32 id);
        List<CONSULTA_RECEBIMENTO> ExecuteFilter(Int32? tipo, Int32? paciente, Int32? consulta, Int32? forma, String nome, DateTime? inicio, DateTime? final, Int32? conferido, Int32 idAss);
    }
}
