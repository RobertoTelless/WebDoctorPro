using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ILogAppService : IAppServiceBase<LOG>
    {
        LOG GetById(Int32 id);
        Int32 ValidateCreate(LOG log);

        List<LOG> GetAllItens(Int32 idAss);
        List<LOG> GetAllItens();
        Tuple<Int32, List<LOG>, Boolean> ExecuteFilterTuple(Int32? usuId, DateTime? data, DateTime? dataFim, String operacao, Int32 idAss);
        Tuple<Int32, List<LOG>, Boolean> ExecuteFilterTuple(Int32? usuId, DateTime? data, DateTime? dataFim, String operacao);
        List<LOG> GetAllItensDataCorrente(Int32 idAss);
        List<LOG> GetAllItensMesCorrente(Int32 idAss);
        List<LOG> GetAllItensMesAnterior(Int32 idAss);
        List<LOG> GetAllItensMesCorrente();
        List<LOG> GetAllItensMesAnterior();
        List<LOG> GetAllItensUsuario(Int32 id, Int32 idAss);
        List<LOG> GetAllItensDataCorrente();
        List<LOG> GetLogByFaixa(DateTime inicio, DateTime final, Int32 idAss);
    }
}
