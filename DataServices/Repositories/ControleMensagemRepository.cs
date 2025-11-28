using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ControleMensagemRepository : RepositoryBase<CONTROLE_MENSAGEM>, IControleMensagemRepository
    {
        public CONTROLE_MENSAGEM CheckExist(CONTROLE_MENSAGEM conta, Int32 idAss)
        {
            IQueryable<CONTROLE_MENSAGEM> query = Db.CONTROLE_MENSAGEM;
            query = query.Where(p => p.COME_DT_DATA == conta.COME_DT_DATA);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public CONTROLE_MENSAGEM GetItemById(Int32 id)
        {
            IQueryable<CONTROLE_MENSAGEM> query = Db.CONTROLE_MENSAGEM;
            query = query.Where(p => p.COME_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CONTROLE_MENSAGEM> GetAllItens(Int32 idAss)
        {
            IQueryable<CONTROLE_MENSAGEM> query = Db.CONTROLE_MENSAGEM;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public CONTROLE_MENSAGEM GetItemByDate(DateTime data, Int32 idAss)
        {
            IQueryable<CONTROLE_MENSAGEM> query = Db.CONTROLE_MENSAGEM;
            query = query.Where(p => DbFunctions.TruncateTime(p.COME_DT_DATA).Value >= DbFunctions.TruncateTime(data).Value);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

    }
}
 