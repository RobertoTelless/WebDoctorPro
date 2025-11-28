using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class EmpresaAnexoRepository : RepositoryBase<EMPRESA_ANEXO>, IEmpresaAnexoRepository
    {
        public List<EMPRESA_ANEXO> GetAllItens()
        {
            return Db.EMPRESA_ANEXO.ToList();
        }

        public EMPRESA_ANEXO GetItemById(Int32 id)
        {
            IQueryable<EMPRESA_ANEXO> query = Db.EMPRESA_ANEXO.Where(p => p.EMAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 