using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class MedicoRepository : RepositoryBase<MEDICOS>, IMedicoRepository
    {
        public MEDICOS CheckExist(MEDICOS item, Int32 idAss)
        {
            IQueryable<MEDICOS> query = Db.MEDICOS;
            query = query.Where(p => p.MEDC_NM_MEDICO.ToUpper() == item.MEDC_NM_MEDICO.ToUpper());
            query = query.Where(p => p.MEDC_NR_CRM.ToUpper() == item.MEDC_NR_CRM.ToUpper());
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.MEDC_IN_ATIVO == 1);
            return query.AsNoTracking().FirstOrDefault();
        }

        public MEDICOS GetItemById(Int32 id)
        {
            IQueryable<MEDICOS> query = Db.MEDICOS;
            query = query.Where(p => p.MEDC_CD_ID == id);
            query = query.Include(p => p.MEDICOS_ENVIO);
            return query.FirstOrDefault();
        }

        public List<MEDICOS> GetAllItens(Int32 idAss)
        {
            IQueryable<MEDICOS> query = Db.MEDICOS;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.MEDC_IN_ATIVO == 1);
            return query.AsNoTracking().ToList();
        }

        public List<MEDICOS> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<MEDICOS> query = Db.MEDICOS;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public List<MEDICOS> ExecuteFilter(Int32? espec, String nome, String crm, String email, Int32 idAss)
        {
            List<MEDICOS> lista = new List<MEDICOS>();
            IQueryable<MEDICOS> query = Db.MEDICOS;
            if (espec != null & espec > 0)
            {
                query = query.Where(p => p.ESPE_CD_ID == espec);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.MEDC_NM_MEDICO.Contains(nome));
            }
            if (!String.IsNullOrEmpty(email))
            {
                query = query.Where(p => p.MEDC_EM_EMAIL.Contains(email));
            }
            if (!String.IsNullOrEmpty(crm))
            {
                query = query.Where(p => p.MEDC_NR_CRM == crm);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.MEDC_IN_ATIVO == 1);
                query = query.OrderBy(a => a.MEDC_NM_MEDICO);
                lista = query.AsNoTracking().ToList<MEDICOS>();
            }
            return lista;
        }

    }
}
