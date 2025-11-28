using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class PacienteRepository : RepositoryBase<PACIENTE>, IPacienteRepository
    {
        public PACIENTE CheckExist(PACIENTE cliente, Int32 idAss)
        {
            IQueryable<PACIENTE> query = Db.PACIENTE;
            query = query.Where(p => p.PACI_NM_NOME.ToUpper() == cliente.PACI_NM_NOME.ToUpper());
            query = query.Where(p => p.PACI_DT_NASCIMENTO == cliente.PACI_DT_NASCIMENTO);
            if (cliente.PACI_NR_CPF != null & cliente.PACI_IN_MENOR == 0)
            {
                query = query.Where(p => p.PACI_NR_CPF == cliente.PACI_NR_CPF);
            }
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.AsNoTracking().FirstOrDefault();
        }

        public PACIENTE GetItemById(Int32 id)
        {
            IQueryable<PACIENTE> query = Db.PACIENTE;
            query = query.Where(p => p.PACI__CD_ID == id);
            query = query.Include(p => p.PACIENTE_ANAMNESE);
            query = query.Include(p => p.PACIENTE_ANEXO);
            query = query.Include(p => p.PACIENTE_ANOTACAO);
            query = query.Include(p => p.PACIENTE_CONSULTA);
            query = query.Include(p => p.PACIENTE_EXAMES);
            query = query.Include(p => p.PACIENTE_EXAME_FISICOS);
            query = query.Include(p => p.PACIENTE_PRESCRICAO);
            query = query.Include(p => p.PACIENTE_HISTORICO);
            query = query.Include(p => p.PACIENTE_DADOS_EXAME_FISICO);
            return query.FirstOrDefault();
        }

        public PACIENTE GetItemByCPF(String cpf)
        {
            IQueryable<PACIENTE> query = Db.PACIENTE;
            query = query.Where(p => p.PACI_NR_CPF == cpf);
            query = query.Include(p => p.PACIENTE_ANAMNESE);
            query = query.Include(p => p.PACIENTE_ANEXO);
            query = query.Include(p => p.PACIENTE_ANOTACAO);
            query = query.Include(p => p.PACIENTE_CONSULTA);
            query = query.Include(p => p.PACIENTE_EXAMES);
            query = query.Include(p => p.PACIENTE_EXAME_FISICOS);
            query = query.Include(p => p.PACIENTE_PRESCRICAO);
            query = query.Include(p => p.PACIENTE_HISTORICO);
            query = query.Include(p => p.PACIENTE_DADOS_EXAME_FISICO);
            return query.FirstOrDefault();
        }

        public List<PACIENTE> GetAllItens(Int32 idAss)
        {
            IQueryable<PACIENTE> query = Db.PACIENTE.Where(p => p.PACI_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderBy(a => a.PACI_NM_NOME);
            return query.AsNoTracking().ToList();
        }

        public List<PACIENTE> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<PACIENTE> query = Db.PACIENTE;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderBy(a => a.PACI_NM_NOME);
            return query.AsNoTracking().ToList();
        }

        public List<PACIENTE> ExecuteFilter(Int32? id, Int32? catId, Int32? sexo, String nome, String cpf, Int32? conv, Int32? menor, String celular, String email, String cidade, Int32? uf, Int32 idAss)
        {
            List<PACIENTE> lista = new List<PACIENTE>();
            IQueryable<PACIENTE> query = Db.PACIENTE;
            if (id > 0)
            {
                query = query.Where(p => p.PACI__CD_ID == id);
            }
            if (catId != null & catId > 0)
            {
                query = query.Where(p => p.SEXO_CD_ID == catId);
            }
            if (sexo != null & sexo > 0)
            {
                query = query.Where(p => p.SEXO_CD_ID == sexo);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.PACI_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(cpf))
            {
                query = query.Where(p => p.PACI_NR_CPF.Contains(cpf));
            }
            if (conv != null & conv > 0)
            {
                query = query.Where(p => p.CONV_CD_ID == conv);
            }
            if (!String.IsNullOrEmpty(celular))
            {
                query = query.Where(p => p.PACI_NR_CELULAR.Contains(celular));
            }
            if (menor != null & menor > 0)
            {
                query = query.Where(p => p.PACI_IN_MENOR == menor);
            }
            if (!String.IsNullOrEmpty(email))
            {
                query = query.Where(p => p.PACI_NM_EMAIL.Contains(email));
            }
            if (!String.IsNullOrEmpty(cidade))
            {
                query = query.Where(p => p.PACI_NM_CIDADE.Contains(cidade));
            }
            if (uf != null & uf > 0)
            {
                query = query.Where(p => p.UF_CD_ID == uf);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.PACI_IN_ATIVO == 1);
                query = query.OrderBy(a => a.PACI_NM_NOME);
                lista = query.AsNoTracking().ToList<PACIENTE>();
            }
            return lista;
        }

        public List<PACIENTE> FiltrarContatos(GRUPO_PAC grupo, Int32 idAss)
        {
            List<PACIENTE> lista = new List<PACIENTE>();
            IQueryable<PACIENTE> query = Db.PACIENTE;
            if (grupo.SEXO_CD_ID != null)
            {
                query = query.Where(p => p.SEXO_CD_ID == grupo.SEXO_CD_ID);
            }
            if (grupo.TIPA_CD_ID != null)
            {
                query = query.Where(p => p.TIPA_CD_ID == grupo.TIPA_CD_ID);
            }
            if (!String.IsNullOrEmpty(grupo.GRUP_NM_CIDADE))
            {
                query = query.Where(p => p.PACI_NM_CIDADE.Contains(grupo.GRUP_NM_CIDADE));
            }
            if (grupo.UF_CD_ID != null)
            {
                query = query.Where(p => p.UF_CD_ID == grupo.UF_CD_ID);
            }
            if (grupo.GRUP_NM_CIDADE != null)
            {
                query = query.Where(p => p.PACI_NM_CIDADE == grupo.GRUP_NM_CIDADE);
            }
            if (grupo.GRUP_DT_NASCIMENTO != null)
            {
                query = query.Where(p => p.PACI_DT_NASCIMENTO == grupo.GRUP_DT_NASCIMENTO);
            }
            else
            {
                if (grupo.GRUP_NR_DIA != null)
                {
                    Int32 dia = Convert.ToInt32(grupo.GRUP_NR_DIA);
                    query = query.Where(p => DbFunctions.TruncateTime(p.PACI_DT_NASCIMENTO).Value.Day == dia);
                }
                if (grupo.GRUP_NR_MES != null)
                {
                    Int32 mes = Convert.ToInt32(grupo.GRUP_NR_MES);
                    query = query.Where(p => DbFunctions.TruncateTime(p.PACI_DT_NASCIMENTO).Value.Month == mes);
                }
                if (grupo.GRUP_NR_ANO != null)
                {
                    Int32 ano = Convert.ToInt32(grupo.GRUP_NR_ANO);
                    query = query.Where(p => DbFunctions.TruncateTime(p.PACI_DT_NASCIMENTO).Value.Year == ano);
                }
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.PACI_IN_ATIVO == 1);
                query = query.OrderBy(a => a.PACI_NM_NOME);
                lista = query.AsNoTracking().ToList<PACIENTE>();
            }
            return lista;
        }

    }
}
