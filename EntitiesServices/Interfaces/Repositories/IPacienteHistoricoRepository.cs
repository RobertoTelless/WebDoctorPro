using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPacienteHistoricoRepository : IRepositoryBase<PACIENTE_HISTORICO>
    {
        List<PACIENTE_HISTORICO> GetAllItens(Int32 idAss);
        PACIENTE_HISTORICO GetItemById(Int32 id);
        List<PACIENTE_HISTORICO> ExecuteFilter(Int32? tipo, String operacao, DateTime? inicio, DateTime? final, String descricao, Int32 idAss);
        List<PACIENTE_HISTORICO> ExecuteFilterGeral(Int32? tipo, String operacao, DateTime? inicio, DateTime? final, Int32? paciente, Int32 idAss);
    }
}
