using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPacientePrescricaoRepository : IRepositoryBase<PACIENTE_PRESCRICAO>
    {
        List<PACIENTE_PRESCRICAO> GetAllItens(Int32 idAss);
        List<PACIENTE_PRESCRICAO> GetAll();
        List<PACIENTE_PRESCRICAO> GetByCPF(String cpf);
        PACIENTE_PRESCRICAO GetItemById(Int32 id);
        List<PACIENTE_PRESCRICAO> ExecuteFilter(Int32? tipo, String nome, DateTime? inicio, DateTime? final, String remedio, String generico, Int32 idAss);
    }
}
