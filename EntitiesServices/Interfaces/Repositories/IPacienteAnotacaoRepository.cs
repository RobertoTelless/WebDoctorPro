using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPacienteAnotacaoRepository : IRepositoryBase<PACIENTE_ANOTACAO>
    {
        List<PACIENTE_ANOTACAO> GetAllItens();
        PACIENTE_ANOTACAO GetItemById(Int32 id);
    }
}
