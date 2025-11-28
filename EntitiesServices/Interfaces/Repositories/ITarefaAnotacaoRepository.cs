using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITarefaAnotacaoRepository : IRepositoryBase<TAREFA_ACOMPANHAMENTO>
    {
        List<TAREFA_ACOMPANHAMENTO> GetAllItens();
        TAREFA_ACOMPANHAMENTO GetItemById(Int32 id);
    }
}
