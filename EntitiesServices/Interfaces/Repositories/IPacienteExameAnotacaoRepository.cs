using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPacienteExameAnotacaoRepository : IRepositoryBase<PACIENTE_EXAME_ANOTACAO>
    {
        List<PACIENTE_EXAME_ANOTACAO> GetAllItens();
        PACIENTE_EXAME_ANOTACAO GetItemById(Int32 id);
    }
}
