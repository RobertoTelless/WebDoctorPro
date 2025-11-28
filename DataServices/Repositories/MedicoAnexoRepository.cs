using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class MedicoAnexoRepository : RepositoryBase<MEDICOS_ENVIO_ANEXO>, IMedicoAnexoRepository
    {
        public List<MEDICOS_ENVIO_ANEXO> GetAllItens()
        {
            return Db.MEDICOS_ENVIO_ANEXO.ToList();
        }

        public MEDICOS_ENVIO_ANEXO GetItemById(Int32 id)
        {
            IQueryable<MEDICOS_ENVIO_ANEXO> query = Db.MEDICOS_ENVIO_ANEXO.Where(p => p.MVAN_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 