using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class EmpresaFilialRepository : RepositoryBase<EMPRESA_FILIAL>, IEmpresaFilialRepository
    {
        public EMPRESA_FILIAL CheckExistFilial(EMPRESA_FILIAL cliente, Int32 idAss)
        {
            IQueryable<EMPRESA_FILIAL> query = Db.EMPRESA_FILIAL;
            if (cliente.EMFI_NR_CNPJ != null)
            {
                query = query.Where(p => p.EMFI_NR_CNPJ == cliente.EMFI_NR_CNPJ);
            }
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public List<EMPRESA_FILIAL> GetAllItens()
        {
            return Db.EMPRESA_FILIAL.ToList();
        }

        public EMPRESA_FILIAL GetItemById(Int32 id)
        {
            IQueryable<EMPRESA_FILIAL> query = Db.EMPRESA_FILIAL.Where(p => p.EMFI_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 