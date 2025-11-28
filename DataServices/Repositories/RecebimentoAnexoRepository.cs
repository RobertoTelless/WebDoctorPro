using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class RecebimentoAnexoRepository : RepositoryBase<RECEBIMENTO_ANEXO>, IRecebimentoAnexoRepository
    {
        public List<RECEBIMENTO_ANEXO> GetAllItens()
        {
            return Db.RECEBIMENTO_ANEXO.ToList();
        }

        public RECEBIMENTO_ANEXO GetItemById(Int32 id)
        {
            IQueryable<RECEBIMENTO_ANEXO> query = Db.RECEBIMENTO_ANEXO.Where(p => p.REAN_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 