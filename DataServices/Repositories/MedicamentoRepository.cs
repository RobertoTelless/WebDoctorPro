using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class MedicamentoRepository : RepositoryBase<MEDICAMENTO>, IMedicamentoRepository
    {
        public MEDICAMENTO GetItemById(Int32 id)
        {
            IQueryable<MEDICAMENTO> query = Db.MEDICAMENTO;
            query = query.Where(p => p.MEDI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<MEDICAMENTO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<MEDICAMENTO> query = Db.MEDICAMENTO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<MEDICAMENTO> GetAllItens(Int32 idAss)
        {
            IQueryable<MEDICAMENTO> query = Db.MEDICAMENTO.Where(p => p.MEDI_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public MEDICAMENTO CheckExist(MEDICAMENTO item, Int32 idAss)
        {
            IQueryable<MEDICAMENTO> query = Db.MEDICAMENTO;
            query = query.Where(p => p.MEDI_NM_MEDICAMENTO.ToUpper() == item.MEDI_NM_MEDICAMENTO.ToUpper());
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.MEDI_IN_ATIVO == 1);
            if (item.MEDI_NM_LABORATORIO != null)
            {
                query = query.Where(p => p.MEDI_NM_LABORATORIO.ToUpper() == item.MEDI_NM_LABORATORIO.ToUpper());
            }
            if (item.MEDI_NM_APRESENTACAO != null)
            {
                query = query.Where(p => p.MEDI_NM_APRESENTACAO.ToUpper() == item.MEDI_NM_APRESENTACAO.ToUpper());
            }
            return query.FirstOrDefault();
        }

        public MEDICAMENTO CheckExistDesc(String nome, String generico, String lab, Int32 idAss)
        {
            IQueryable<MEDICAMENTO> query = Db.MEDICAMENTO.Where(p => p.MEDI_IN_ATIVO == 1);
            query = query.Where(p => p.MEDI_NM_MEDICAMENTO.ToUpper() == nome.ToUpper());
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.MEDI_NM_GENERICO.ToUpper() == generico.ToUpper());
            query = query.Where(p => p.MEDI_NM_LABORATORIO.ToUpper() == lab.ToUpper());
            return query.FirstOrDefault();
        }

        public List<MEDICAMENTO> ExecuteFilter(String generico, String nome, String laboratorio, Int32 idAss)
        {
            List<MEDICAMENTO> lista = new List<MEDICAMENTO>();
            IQueryable<MEDICAMENTO> query = Db.MEDICAMENTO;
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.MEDI_NM_MEDICAMENTO.ToUpper().Contains(nome.ToUpper()));
            }
            if (!String.IsNullOrEmpty(generico))
            {
                query = query.Where(p => p.MEDI_NM_GENERICO.ToUpper().Contains(generico.ToUpper()));
            }
            if (!String.IsNullOrEmpty(laboratorio))
            {
                query = query.Where(p => p.MEDI_NM_LABORATORIO.ToUpper().Contains(laboratorio.ToUpper()));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.MEDI_IN_ATIVO == 1);
                lista = query.ToList<MEDICAMENTO>();
            }
            return lista;
        }
    }
}
 