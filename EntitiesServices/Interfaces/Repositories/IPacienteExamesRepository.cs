using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPacienteExamesRepository : IRepositoryBase<PACIENTE_EXAMES>
    {
        List<PACIENTE_EXAMES> GetAllItens(Int32 idAss);
        List<PACIENTE_EXAMES> GetByCPF(String cpf);
        PACIENTE_EXAMES GetItemById(Int32 id);
        List<PACIENTE_EXAMES> ExecuteFilter(Int32? tipo, Int32? lab, String nome, DateTime? inicio, DateTime? final, String titulo, String descricao, Int32 idAss);
    }
}
