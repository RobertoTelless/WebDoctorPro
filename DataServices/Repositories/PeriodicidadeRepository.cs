using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class PeriodicidadeRepository : RepositoryBase<PERIODICIDADE_TAREFA>, IPeriodicidadeRepository
    {
        public PERIODICIDADE_TAREFA CheckExist(PERIODICIDADE_TAREFA conta)
        {
            IQueryable<PERIODICIDADE_TAREFA> query = Db.PERIODICIDADE_TAREFA;
            query = query.Where(p => p.PETA_NM_NOME == conta.PETA_NM_NOME);
            return query.FirstOrDefault();
        }

        public PERIODICIDADE_TAREFA GetItemById(Int32 id)
        {
            IQueryable<PERIODICIDADE_TAREFA> query = Db.PERIODICIDADE_TAREFA;
            query = query.Where(p => p.PETA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PERIODICIDADE_TAREFA> GetAllItensAdm()
        {
            IQueryable<PERIODICIDADE_TAREFA> query = Db.PERIODICIDADE_TAREFA;
            return query.ToList();
        }

        public List<PERIODICIDADE_TAREFA> GetAllItens()
        {
            IQueryable<PERIODICIDADE_TAREFA> query = Db.PERIODICIDADE_TAREFA.Where(p => p.PETA_IN_ATIVO == 1);
            return query.ToList();
        }
    }
}
 