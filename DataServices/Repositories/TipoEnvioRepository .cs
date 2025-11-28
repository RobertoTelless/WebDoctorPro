using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class TipoEnvioRepository : RepositoryBase<TIPO_ENVIO>, ITipoEnvioRepository
    {
        public TIPO_ENVIO CheckExist(TIPO_ENVIO conta, Int32 idAss)
        {
            IQueryable<TIPO_ENVIO> query = Db.TIPO_ENVIO;
            query = query.Where(p => p.TIEN_NM_NOME == conta.TIEN_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public TIPO_ENVIO GetItemById(Int32 id)
        {
            IQueryable<TIPO_ENVIO> query = Db.TIPO_ENVIO;
            query = query.Where(p => p.TIEN_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_ENVIO> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_ENVIO> query = Db.TIPO_ENVIO.Where(p => p.TIEN_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<TIPO_ENVIO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TIPO_ENVIO> query = Db.TIPO_ENVIO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
