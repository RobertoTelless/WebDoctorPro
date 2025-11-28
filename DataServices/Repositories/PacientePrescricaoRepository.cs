using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class PacientePrescricaoRepository : RepositoryBase<PACIENTE_PRESCRICAO>, IPacientePrescricaoRepository
    {
        public PACIENTE_PRESCRICAO GetItemById(Int32 id)
        {
            IQueryable<PACIENTE_PRESCRICAO> query = Db.PACIENTE_PRESCRICAO;
            query = query.Where(p => p.PAPR_CD_ID == id);
            query = query.Include(p => p.PACIENTE_PRESCRICAO_ITEM);
            return query.FirstOrDefault();
        }

        public List<PACIENTE_PRESCRICAO> GetAllItens(Int32 idAss)
        {
            IQueryable<PACIENTE_PRESCRICAO> query = Db.PACIENTE_PRESCRICAO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.PACIENTE_PRESCRICAO_ITEM);
            return query.ToList();
        }

        public List<PACIENTE_PRESCRICAO> GetAll()
        {
            IQueryable<PACIENTE_PRESCRICAO> query = Db.PACIENTE_PRESCRICAO;
            query = query.Include(p => p.PACIENTE_PRESCRICAO_ITEM);
            return query.ToList();
        }

        public List<PACIENTE_PRESCRICAO> GetByCPF(String cpf)
        {
            IQueryable<PACIENTE_PRESCRICAO> query = Db.PACIENTE_PRESCRICAO.Where(p => p.PAPR_IN_ATIVO == 1);
            query = query.Where(p => p.PACIENTE.PACI_NR_CPF == cpf);
            return query.ToList();
        }

        public List<PACIENTE_PRESCRICAO> ExecuteFilter(Int32? tipo, String nome, DateTime? dataInicio, DateTime? dataFim, String remedio, String generico, Int32 idAss)
        {
            List<PACIENTE_PRESCRICAO> lista = new List<PACIENTE_PRESCRICAO>();
            IQueryable<PACIENTE_PRESCRICAO> query = Db.PACIENTE_PRESCRICAO;
            if (tipo != null & tipo > 0)
            {
                query = query.Where(p => p.TICO_CD_ID == tipo);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.PACIENTE.PACI_NM_NOME.Contains(nome));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim == DateTime.MinValue || dataFim == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PAPR_DT_EMISSAO) >= DbFunctions.TruncateTime(dataInicio));
            }
            if ((dataInicio == DateTime.MinValue || dataInicio == null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PAPR_DT_EMISSAO) <= DbFunctions.TruncateTime(dataFim));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PAPR_DT_EMISSAO) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.PAPR_DT_EMISSAO) <= DbFunctions.TruncateTime(dataFim));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.PAPR_IN_ATIVO == 1);
                query = query.OrderBy(a => a.PAPR_DT_EMISSAO);
                lista = query.ToList<PACIENTE_PRESCRICAO>();
            }
            return lista;
        }

    }
}
