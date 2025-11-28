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
using CrossCutting;

namespace DataServices.Repositories
{
    public class EspecialidadeRepository : RepositoryBase<ESPECIALIDADE>, IEspecialidadeRepository
    {
        public ESPECIALIDADE CheckExist(ESPECIALIDADE conta, Int32 idAss)
        {
            IQueryable<ESPECIALIDADE> query = Db.ESPECIALIDADE;
            query = query.Where(p => p.ESPE_NM_NOME == conta.ESPE_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.ESPE_IN_ATIVO == 1);
            return query.AsNoTracking().FirstOrDefault();
        }

        public ESPECIALIDADE GetItemById(Int32 id)
        {
            IQueryable<ESPECIALIDADE> query = Db.ESPECIALIDADE;
            query = query.Where(p => p.ESPE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<ESPECIALIDADE> GetAllItens(Int32 idAss)
        {
            IQueryable<ESPECIALIDADE> query = Db.ESPECIALIDADE.Where(p => p.ESPE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public List<ESPECIALIDADE> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<ESPECIALIDADE> query = Db.ESPECIALIDADE;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

    }
}
 