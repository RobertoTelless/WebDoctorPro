using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMedicamentoRepository : IRepositoryBase<MEDICAMENTO>
    {
        List<MEDICAMENTO> GetAllItens(Int32 idAss);
        MEDICAMENTO GetItemById(Int32 id);
        List<MEDICAMENTO> GetAllItensAdm(Int32 idAss);
        List<MEDICAMENTO> ExecuteFilter(String generico, String nome, String laboratorio, Int32 idAss);
        MEDICAMENTO CheckExist(MEDICAMENTO item, Int32 idAss);
        MEDICAMENTO CheckExistDesc(String nome, String generico, String lab, Int32 idAss);
    }
}
