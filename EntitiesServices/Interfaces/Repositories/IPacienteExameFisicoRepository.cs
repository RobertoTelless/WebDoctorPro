using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPacienteExameFisicoRepository : IRepositoryBase<PACIENTE_EXAME_FISICOS>
    {
        List<PACIENTE_EXAME_FISICOS> GetAllItens(Int32 idAss);
        PACIENTE_EXAME_FISICOS GetItemById(Int32 id);
    }
}
