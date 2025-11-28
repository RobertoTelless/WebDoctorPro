using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPacienteAtestadoRepository : IRepositoryBase<PACIENTE_ATESTADO>
    {
        List<PACIENTE_ATESTADO> GetAllItens(Int32 idAss);
        List<PACIENTE_ATESTADO> GetAll();
        List<PACIENTE_ATESTADO> GetByCPF(String cpf);
        PACIENTE_ATESTADO GetItemById(Int32 id);
        List<PACIENTE_ATESTADO> ExecuteFilter(Int32? tipo, String nome, DateTime? inicio, DateTime? final, String titulo, String descricao, Int32 idAss);
    }
}
