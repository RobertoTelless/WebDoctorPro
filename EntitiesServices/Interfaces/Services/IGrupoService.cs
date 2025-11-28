using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IGrupoService : IServiceBase<GRUPO_PAC>
    {
        Int32 Create(GRUPO_PAC item, LOG log);
        Int32 Create(GRUPO_PAC item);
        Int32 Edit(GRUPO_PAC item, LOG log);
        Int32 Edit(GRUPO_PAC item);
        Int32 Delete(GRUPO_PAC item, LOG log);

        GRUPO_PAC CheckExist(GRUPO_PAC item, Int32 idAss);
        GRUPO_PAC GetItemById(Int32 id);
        List<GRUPO_PAC> GetAllItens(Int32 idAss);
        List<GRUPO_PAC> GetAllItensAdm(Int32 idAss);

        GRUPO_PACIENTE GetContatoById(Int32 id);
        Int32 CreateContato(GRUPO_PACIENTE item);
        Int32 EditContato(GRUPO_PACIENTE item);
        GRUPO_PACIENTE CheckExistContato(GRUPO_PACIENTE item);
        List<PACIENTE> FiltrarContatos(GRUPO_PAC grupo, Int32 idAss);
    }
}
