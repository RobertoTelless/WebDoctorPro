using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class MensagemAnexoRepository : RepositoryBase<MENSAGEM_ANEXO>, IMensagemAnexoRepository
    {
        public List<MENSAGEM_ANEXO> GetAllItens()
        {
            return Db.MENSAGEM_ANEXO.ToList();
        }

        public MENSAGEM_ANEXO GetItemById(Int32 id)
        {
            IQueryable<MENSAGEM_ANEXO> query = Db.MENSAGEM_ANEXO.Where(p => p.MEAN_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 