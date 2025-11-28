using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class AssinanteAnexoRepository : RepositoryBase<ASSINANTE_ANEXO>, IAssinanteAnexoRepository
    {
        public List<ASSINANTE_ANEXO> GetAllItens()
        {
            return Db.ASSINANTE_ANEXO.ToList();
        }

        public ASSINANTE_ANEXO GetItemById(Int32 id)
        {
            IQueryable<ASSINANTE_ANEXO> query = Db.ASSINANTE_ANEXO.Where(p => p.ASAN_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 