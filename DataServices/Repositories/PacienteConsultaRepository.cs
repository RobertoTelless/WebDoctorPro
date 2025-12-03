using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class PacienteConsultaRepository : RepositoryBase<PACIENTE_CONSULTA>, IPacienteConsultaRepository
    {
        public PACIENTE_CONSULTA CheckExist(PACIENTE_CONSULTA conta, Int32 idAss)
        {
            IQueryable<PACIENTE_CONSULTA> query = Db.PACIENTE_CONSULTA;
            query = query.Where(p => p.PACO_DT_CONSULTA == conta.PACO_DT_CONSULTA);
            query = query.Where(p => p.PACI_CD_ID == conta.PACI_CD_ID);
            return query.AsNoTracking().FirstOrDefault();
        }

        public PACIENTE_CONSULTA GetItemById(Int32 id)
        {
            IQueryable<PACIENTE_CONSULTA> query = Db.PACIENTE_CONSULTA;
            query = query.Where(p => p.PACO_CD_ID == id);
            query = query.Include(p => p.PACIENTE_ANAMNESE);
            query = query.Include(p => p.PACIENTE_ATESTADO);
            query = query.Include(p => p.PACIENTE_EXAMES);
            query = query.Include(p => p.PACIENTE_EXAME_FISICOS);
            query = query.Include(p => p.PACIENTE_PRESCRICAO);
            query = query.Include(p => p.PACIENTE_SOLICITACAO);
            query = query.Include(p => p.VALOR_CONSULTA);
            return query.FirstOrDefault();
        }

        public List<PACIENTE_CONSULTA> GetAllItens(Int32 idAss)
        {
            IQueryable<PACIENTE_CONSULTA> query = Db.PACIENTE_CONSULTA.Where(p => p.PACO_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public List<PACIENTE_CONSULTA> GetByCPF(String cpf)
        {
            IQueryable<PACIENTE_CONSULTA> query = Db.PACIENTE_CONSULTA.Where(p => p.PACO_IN_ATIVO == 1);
            query = query.Where(p => p.PACIENTE.PACI_NR_CPF == cpf);
            return query.ToList();
        }

        public List<PACIENTE_CONSULTA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<PACIENTE_CONSULTA> query = Db.PACIENTE_CONSULTA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().ToList();
        }

        public List<PACIENTE_CONSULTA> ExecuteFilter(Int32? tipo, String nome, DateTime? dataInicio, DateTime? dataFim, Int32? confirmada, Int32? usuario, Int32 idAss)
        {
            List<PACIENTE_CONSULTA> lista = new List<PACIENTE_CONSULTA>();
            IQueryable<PACIENTE_CONSULTA> query = Db.PACIENTE_CONSULTA;
            if (tipo != null & tipo > 0)
            {
                query = query.Where(p => p.VACO_CD_ID == tipo);
            }
            if (usuario != null & usuario > 0)
            {
                query = query.Where(p => p.USUA_CD_ID == usuario);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.PACIENTE.PACI_NM_NOME.Contains(nome));
            }
            if (confirmada != null)
            {
                query = query.Where(p => p.PACO_IN_CONFIRMADA == confirmada);
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim == DateTime.MinValue || dataFim == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PACO_DT_CONSULTA) >= DbFunctions.TruncateTime(dataInicio));
            }
            if ((dataInicio == DateTime.MinValue || dataInicio == null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PACO_DT_CONSULTA) <= DbFunctions.TruncateTime(dataFim));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PACO_DT_CONSULTA) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.PACO_DT_CONSULTA) <= DbFunctions.TruncateTime(dataFim));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.PACO_IN_ATIVO == 1);
                query = query.OrderBy(a => a.PACO_DT_CONSULTA);
                lista = query.AsNoTracking().ToList<PACIENTE_CONSULTA>();
            }
            return lista;
        }

        public List<PACIENTE_CONSULTA> ExecuteFilterConfirma(Int32? tipo, String nome, DateTime? dataInicio, DateTime? dataFim, Int32? situacao, Int32? usuario, Int32 idAss)
        {
            List<PACIENTE_CONSULTA> lista = new List<PACIENTE_CONSULTA>();
            IQueryable<PACIENTE_CONSULTA> query = Db.PACIENTE_CONSULTA;
            if (tipo != null & tipo > 0)
            {
                query = query.Where(p => p.VACO_CD_ID == tipo);
            }
            if (situacao != null)
            {
                query = query.Where(p => p.PACO_IN_CONFIRMADA == situacao);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.PACIENTE.PACI_NM_NOME.Contains(nome));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim == DateTime.MinValue || dataFim == null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PACO_DT_CONSULTA) >= DbFunctions.TruncateTime(dataInicio));
            }
            if ((dataInicio == DateTime.MinValue || dataInicio == null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PACO_DT_CONSULTA) <= DbFunctions.TruncateTime(dataFim));
            }
            if ((dataInicio != DateTime.MinValue & dataInicio != null) & (dataFim != DateTime.MinValue & dataFim != null))
            {
                query = query.Where(p => DbFunctions.TruncateTime(p.PACO_DT_CONSULTA) >= DbFunctions.TruncateTime(dataInicio) & DbFunctions.TruncateTime(p.PACO_DT_CONSULTA) <= DbFunctions.TruncateTime(dataFim));
            }
            if (usuario != null & usuario > 0)
            {
                query = query.Where(p => p.USUA_CD_ID == usuario);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.PACO_IN_ATIVO == 1);
                query = query.OrderBy(a => a.PACO_DT_CONSULTA);
                lista = query.AsNoTracking().ToList<PACIENTE_CONSULTA>();
            }
            return lista;
        }

    }
}
