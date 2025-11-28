using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IAvisoLembreteService : IServiceBase<AVISO_LEMBRETE>
    {
        Int32 Create(AVISO_LEMBRETE perfil, LOG log);
        Int32 Create(AVISO_LEMBRETE perfil);
        Int32 Edit(AVISO_LEMBRETE perfil, LOG log);
        Int32 Edit(AVISO_LEMBRETE perfil);
        Int32 Delete(AVISO_LEMBRETE perfil, LOG log);

        List<AVISO_LEMBRETE> GetAllItens(Int32 idAss);
        AVISO_LEMBRETE GetItemById(Int32 id);
        List<AVISO_LEMBRETE> ExecuteFilter(String titulo, DateTime? inicio, DateTime? final, Int32? ciente, Int32 idAss);
    }
}
