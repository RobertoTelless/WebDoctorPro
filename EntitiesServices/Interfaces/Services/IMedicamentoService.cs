using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IMedicamentoService : IServiceBase<MEDICAMENTO>
    {
        Int32 Create(MEDICAMENTO perfil, LOG log);
        Int32 Create(MEDICAMENTO perfil);
        Int32 Edit(MEDICAMENTO perfil, LOG log);
        Int32 Edit(MEDICAMENTO perfil);
        Int32 Delete(MEDICAMENTO perfil, LOG log);

        List<MEDICAMENTO> GetAllItens(Int32 idAss);
        MEDICAMENTO GetItemById(Int32 id);
        List<MEDICAMENTO> GetAllItensAdm(Int32 idAss);
        List<MEDICAMENTO> ExecuteFilter(String generico, String nome, String laboratorio, Int32 idAss);
        MEDICAMENTO CheckExist(MEDICAMENTO item, Int32 idAss);
        List<TIPO_FORMA> GetAllFormas();
        MEDICAMENTO CheckExistDesc(String nome, String generico, String lab, Int32 idAss);
    }
}
