using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IAcessoSiteAppService : IAppServiceBase<ACESSO_SITE>
    {
        Int32 ValidateCreate(ACESSO_SITE item);
        Int32 ValidateEdit(ACESSO_SITE item);

        List<ACESSO_SITE> GetAllItens();
        ACESSO_SITE GetItemById(Int32 id);
    }
}
