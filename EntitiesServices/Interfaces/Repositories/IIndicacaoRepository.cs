using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IIndicacaoRepository : IRepositoryBase<INDICACAO>
    {
        List<INDICACAO> GetAllItens(Int32 idAss);
        List<INDICACAO> GetAll();
        INDICACAO GetItemById(Int32 id);
        List<INDICACAO> ExecuteFilter(Int32? autor, String nome, DateTime? inicio, DateTime? final, String email, Int32? status, Int32 idAss);
    }
}
