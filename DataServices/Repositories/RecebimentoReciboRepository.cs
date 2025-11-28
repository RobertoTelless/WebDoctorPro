using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class RecebimentoReciboRepository : RepositoryBase<RECEBIMENTO_RECIBO>, IRecebimentoReciboRepository
    {
        public List<RECEBIMENTO_RECIBO> GetAllItens()
        {
            return Db.RECEBIMENTO_RECIBO.ToList();
        }

        public RECEBIMENTO_RECIBO GetItemById(Int32 id)
        {
            IQueryable<RECEBIMENTO_RECIBO> query = Db.RECEBIMENTO_RECIBO.Where(p => p.RERC_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 