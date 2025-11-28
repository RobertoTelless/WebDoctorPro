using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class TipoHistoricoRepository : RepositoryBase<TIPO_HISTORICO>, ITipoHistoricoRepository
    {
        public TIPO_HISTORICO CheckExist(TIPO_HISTORICO conta, Int32 idAss)
        {
            IQueryable<TIPO_HISTORICO> query = Db.TIPO_HISTORICO;
            query = query.Where(p => p.TIHI_NM_NOME == conta.TIHI_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public TIPO_HISTORICO GetItemById(Int32 id)
        {
            IQueryable<TIPO_HISTORICO> query = Db.TIPO_HISTORICO;
            query = query.Where(p => p.TIHI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_HISTORICO> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_HISTORICO> query = Db.TIPO_HISTORICO.Where(p => p.TIHI_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public List<TIPO_HISTORICO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TIPO_HISTORICO> query = Db.TIPO_HISTORICO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }
    }
}
