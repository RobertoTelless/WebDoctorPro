using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class IndicacaoAcaoRepository : RepositoryBase<INDICACAO_ACAO>, IIndicacaoAcaoRepository
    {
        public INDICACAO_ACAO GetItemById(Int32 id)
        {
            IQueryable<INDICACAO_ACAO> query = Db.INDICACAO_ACAO;
            query = query.Where(p => p.INAC_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<INDICACAO_ACAO> GetAllItens(Int32 idAss)
        {
            IQueryable<INDICACAO_ACAO> query = Db.INDICACAO_ACAO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
