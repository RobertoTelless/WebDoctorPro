using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class LocacaoOcorrenciaRepository : RepositoryBase<LOCACAO_OCORRENCIA>, ILocacaoOcorrenciaRepository
    {
        public List<LOCACAO_OCORRENCIA> GetAllItens(Int32 idAss)
        {
            IQueryable<LOCACAO_OCORRENCIA> query = Db.LOCACAO_OCORRENCIA.Where(p => p.LOOC_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public LOCACAO_OCORRENCIA GetItemById(Int32 id)
        {
            IQueryable<LOCACAO_OCORRENCIA> query = Db.LOCACAO_OCORRENCIA.Where(p => p.LOOC_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 