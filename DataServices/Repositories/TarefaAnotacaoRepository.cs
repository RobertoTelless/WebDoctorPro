using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class TarefaAnotacaoRepository : RepositoryBase<TAREFA_ACOMPANHAMENTO>, ITarefaAnotacaoRepository
    {
        public List<TAREFA_ACOMPANHAMENTO> GetAllItens()
        {
            return Db.TAREFA_ACOMPANHAMENTO.ToList();
        }

        public TAREFA_ACOMPANHAMENTO GetItemById(Int32 id)
        {
            IQueryable<TAREFA_ACOMPANHAMENTO> query = Db.TAREFA_ACOMPANHAMENTO.Where(p => p.TAAC_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 