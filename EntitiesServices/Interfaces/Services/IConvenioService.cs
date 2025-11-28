using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IConvenioService : IServiceBase<CONVENIO>
    {
        Int32 Create(CONVENIO perfil, LOG log);
        Int32 Create(CONVENIO perfil);
        Int32 Edit(CONVENIO perfil, LOG log);
        Int32 Edit(CONVENIO perfil);
        Int32 Delete(CONVENIO perfil, LOG log);

        CONVENIO CheckExist(CONVENIO item, Int32 idAss);
        List<CONVENIO> GetAllItens(Int32 idAss);
        CONVENIO GetItemById(Int32 id);
        List<CONVENIO> GetAllItensAdm(Int32 idAss);
    }
}
