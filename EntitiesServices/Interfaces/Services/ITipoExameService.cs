using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITipoExameService : IServiceBase<TIPO_EXAME>
    {
        Int32 Create(TIPO_EXAME perfil, LOG log);
        Int32 Create(TIPO_EXAME perfil);
        Int32 Edit(TIPO_EXAME perfil, LOG log);
        Int32 Edit(TIPO_EXAME perfil);
        Int32 Delete(TIPO_EXAME perfil, LOG log);

        TIPO_EXAME CheckExist(TIPO_EXAME item, Int32 idAss);
        List<TIPO_EXAME> GetAllItens(Int32 idAss);
        TIPO_EXAME GetItemById(Int32 id);
        List<TIPO_EXAME> GetAllItensAdm(Int32 idAss);
    }
}
