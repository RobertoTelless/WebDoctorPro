using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class ControleVersaoRepository : RepositoryBase<CONTROLE_VERSAO>, IControleVersaoRepository
    {
        public CONTROLE_VERSAO GetItemById(Int32 id)
        {
            IQueryable<CONTROLE_VERSAO> query = Db.CONTROLE_VERSAO;
            query = query.Where(p => p.COVE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CONTROLE_VERSAO> GetAllItens()
        {
            IQueryable<CONTROLE_VERSAO> query = Db.CONTROLE_VERSAO;
            query = query.Where(p => p.COVE_IN_ATIVO == 1);
            query = query.Where(p => p.COVE_IN_SISTEMA == 6);
            return query.ToList();
        }

    }
}
