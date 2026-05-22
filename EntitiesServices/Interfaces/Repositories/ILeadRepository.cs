using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ILeadRepository : IRepositoryBase<LEAD>
    {
        LEAD CheckExist(LEAD item, Int32 idAss);
        List<LEAD> GetAllItens(Int32 idAss);
        List<LEAD> GetAllItensAdm(Int32 idAss);
        LEAD GetItemById(Int32 id);
        List<LEAD> ExecuteFilter(DateTime? inicio, DateTime? final, String nome, String email, Int32? status, String cpf, String cnpj, String cidade, Int32? uf, Int32 idAss);
    }
}
