using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class MedicoMensagemRepository : RepositoryBase<MEDICOS_MENSAGEM>, IMedicoMensagemRepository
    {
        public MEDICOS_MENSAGEM CheckExist(MEDICOS_MENSAGEM conta, Int32 idAss)
        {
            IQueryable<MEDICOS_MENSAGEM> query = Db.MEDICOS_MENSAGEM;
            query = query.Where(p => p.METX_NM_NOME == conta.METX_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().FirstOrDefault();
        }

        public MEDICOS_MENSAGEM GetItemById(Int32 id)
        {
            IQueryable<MEDICOS_MENSAGEM> query = Db.MEDICOS_MENSAGEM;
            query = query.Where(p => p.METX_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<MEDICOS_MENSAGEM> GetAllItens(Int32 idAss)
        {
            IQueryable<MEDICOS_MENSAGEM> query = Db.MEDICOS_MENSAGEM.Where(p => p.METX_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public List<MEDICOS_MENSAGEM> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<MEDICOS_MENSAGEM> query = Db.MEDICOS_MENSAGEM;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }
    }
}
