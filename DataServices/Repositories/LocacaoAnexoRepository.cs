using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class LocacaoAnexoRepository : RepositoryBase<LOCACAO_ANEXO>, ILocacaoAnexoRepository
    {
        public List<LOCACAO_ANEXO> GetAllItens()
        {
            return Db.LOCACAO_ANEXO.ToList();
        }

        public LOCACAO_ANEXO GetItemById(Int32 id)
        {
            IQueryable<LOCACAO_ANEXO> query = Db.LOCACAO_ANEXO.Where(p => p.LOAX_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 