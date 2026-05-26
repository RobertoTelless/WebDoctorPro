using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class LeadRepository : RepositoryBase<LEAD>, ILeadRepository
    {
        public LEAD CheckExist(LEAD item, Int32 idAss)
        {
            IQueryable<LEAD> query = Db.LEAD;
            query = query.Where(p => p.LEAD_NM_NOME.ToUpper() == item.LEAD_NM_NOME.ToUpper());
            query = query.Where(p => p.LEAD_DT_ENTRADA == item.LEAD_DT_ENTRADA);
            query = query.Where(p => p.LEAD_IN_SISTEMA == 6);
            query = query.Where(p => p.LEAD_IN_ATIVO == 1);
            return query.AsNoTracking().FirstOrDefault();
        }

        public LEAD GetItemById(Int32 id)
        {
            IQueryable<LEAD> query = Db.LEAD;
            query = query.Where(p => p.LEAD_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<LEAD> GetAllItens(Int32 idAss)
        {
            IQueryable<LEAD> query = Db.LEAD;
            query = query.Where(p => p.LEAD_IN_ATIVO == 1);
            query = query.Where(p => p.LEAD_IN_SISTEMA == 6);
            return query.AsNoTracking().ToList();
        }

        public List<LEAD> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<LEAD> query = Db.LEAD;
            query = query.Where(p => p.LEAD_IN_SISTEMA == 6);
            return query.AsNoTracking().ToList();
        }

        public List<LEAD> ExecuteFilter(DateTime? dataInicio, DateTime? dataFim, String nome, String email, Int32? status, String cpf, String cnpj, String cidade, Int32? uf, Int32 idAss)
        {
            List<LEAD> lista = new List<LEAD>();
            IQueryable<LEAD> query = Db.LEAD;


            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim == DateTime.MinValue || dataFim == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.LEAD_DT_ENTRADA) >= DbFunctions.TruncateTime(dataInicio));
            }
            if ((dataInicio == DateTime.MinValue || dataInicio == null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.LEAD_DT_ENTRADA) <= DbFunctions.TruncateTime(dataFim));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.LEAD_DT_ENTRADA) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.LEAD_DT_ENTRADA) <= DbFunctions.TruncateTime(dataFim));
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.LEAD_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(email))
            {
                query = query.Where(p => p.LEAD_EM_EMAIL.Contains(email));
            }
            if (!String.IsNullOrEmpty(cpf))
            {
                query = query.Where(p => p.LEAD_NR_CPF == cpf);
            }
            if (!String.IsNullOrEmpty(cnpj))
            {
                query = query.Where(p => p.LEAD_NR_CNPJ == cnpj);
            }
            if (!String.IsNullOrEmpty(cidade))
            {
                query = query.Where(p => p.LEAD_NM_CIDADE.Contains(cidade));
            }
            if (!String.IsNullOrEmpty(cpf))
            {
                query = query.Where(p => p.LEAD_NR_CPF == cpf);
            }
            if (status != null & status > 0)
            {
                query = query.Where(p => p.LEAD_IN_STATUS == status);
            }
            if (uf != null & uf > 0)
            {
                query = query.Where(p => p.UF_CD_ID == uf);
            }
            if (query != null)
            {
                query = query.Where(p => p.LEAD_IN_ATIVO == 1);
                query = query.Where(p => p.LEAD_IN_SISTEMA == 6);
                query = query.OrderBy(a => a.LEAD_DT_ENTRADA);
                lista = query.AsNoTracking().ToList<LEAD>();
            }
            return lista;
        }

    }
}
