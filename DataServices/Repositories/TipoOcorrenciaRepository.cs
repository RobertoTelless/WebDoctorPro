using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;

namespace DataServices.Repositories
{
    public class TipoOcorrenciaRepository : RepositoryBase<TIPO_OCORRENCIA>, ITipoOcorrenciaRepository
    {
        public TIPO_OCORRENCIA GetItemById(Int32 id)
        {
            IQueryable<TIPO_OCORRENCIA> query = Db.TIPO_OCORRENCIA;
            query = query.Where(p => p.TIOC_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_OCORRENCIA> GetAllItens(Int32 id)
        {
            IQueryable<TIPO_OCORRENCIA> query = Db.TIPO_OCORRENCIA;
            query = query.Where(p => p.ASSI_CD_ID == id);
            return query.ToList();
        }

    }
}
