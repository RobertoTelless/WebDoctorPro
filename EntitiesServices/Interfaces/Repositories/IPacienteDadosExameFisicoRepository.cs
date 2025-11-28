using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPacienteDadosExameFisicoRepository : IRepositoryBase<PACIENTE_DADOS_EXAME_FISICO>
    {
        List<PACIENTE_DADOS_EXAME_FISICO> GetAllItens(Int32 idAss);
        PACIENTE_DADOS_EXAME_FISICO GetItemById(Int32 id);
    }
}
