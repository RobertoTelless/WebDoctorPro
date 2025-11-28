using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPacienteAnamneseAnotacaoRepository : IRepositoryBase<PACIENTE_ANAMNESE_ANOTACAO>
    {
        List<PACIENTE_ANAMNESE_ANOTACAO> GetAllItens();
        PACIENTE_ANAMNESE_ANOTACAO GetItemById(Int32 id);
    }
}
