using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class FormaRecebimentoRepository : RepositoryBase<FORMA_RECEBIMENTO>, IFormaRecebimentoRepository
    {
        public FORMA_RECEBIMENTO CheckExist(FORMA_RECEBIMENTO conta, Int32 idAss)
        {
            IQueryable<FORMA_RECEBIMENTO> query = Db.FORMA_RECEBIMENTO;
            query = query.Where(p => p.FORE_NM_FORMA == conta.FORE_NM_FORMA);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public FORMA_RECEBIMENTO GetItemById(Int32 id)
        {
            IQueryable<FORMA_RECEBIMENTO> query = Db.FORMA_RECEBIMENTO;
            query = query.Where(p => p.FORE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<FORMA_RECEBIMENTO> GetAllItens(Int32 idAss)
        {
            IQueryable<FORMA_RECEBIMENTO> query = Db.FORMA_RECEBIMENTO.Where(p => p.FORE_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<FORMA_RECEBIMENTO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<FORMA_RECEBIMENTO> query = Db.FORMA_RECEBIMENTO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
