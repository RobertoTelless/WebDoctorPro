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

namespace DataServices.Repositories
{
    public class MovimentoAnotacaoRepository : RepositoryBase<MOVIMENTO_ANOTACAO>, IMovimentoAnotacaoRepository
    {
        public List<MOVIMENTO_ANOTACAO> GetAllItens()
        {
            return Db.MOVIMENTO_ANOTACAO.ToList();
        }

        public MOVIMENTO_ANOTACAO GetItemById(Int32 id)
        {
            IQueryable<MOVIMENTO_ANOTACAO> query = Db.MOVIMENTO_ANOTACAO.Where(p => p.MOAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 