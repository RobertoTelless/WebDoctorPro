using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class IndicacaoAnexoRepository : RepositoryBase<INDICACAO_ANEXO>, IIndicacaoAnexoRepository
    {
        public List<INDICACAO_ANEXO> GetAllItens()
        {
            return Db.INDICACAO_ANEXO.ToList();
        }

        public INDICACAO_ANEXO GetItemById(Int32 id)
        {
            IQueryable<INDICACAO_ANEXO> query = Db.INDICACAO_ANEXO.Where(p => p.INAN_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 