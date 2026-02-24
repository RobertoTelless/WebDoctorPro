using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ILocacaoRepository : IRepositoryBase<LOCACAO>
    {
        LOCACAO CheckExist(LOCACAO item, Int32 idAss);
        List<LOCACAO> GetAllItens(Int32 idAss);
        List<LOCACAO> GetAllItensAdm(Int32 idAss);
        LOCACAO GetItemById(Int32 id);
        List<LOCACAO> GetByCPF(String cpf);
        List<LOCACAO> ExecuteFilter(String paciente, String prod, DateTime? inicio, DateTime? final, Int32? status, String numero, Int32 idAss);
    }
}
