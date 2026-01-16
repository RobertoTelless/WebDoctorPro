using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;

namespace ApplicationServices.Services
{
    public class AcessoMetodoAppService : AppServiceBase<ACESSO_METODO>, IAcessoMetodoAppService
    {
        private readonly IAcessoMetodoService _baseService;

        public AcessoMetodoAppService(IAcessoMetodoService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<ACESSO_METODO> GetAllItens(Int32 idAss)
        {
            List<ACESSO_METODO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public ACESSO_METODO GetItemById(Int32 id)
        {
            ACESSO_METODO item = _baseService.GetItemById(id);
            return item;
        }

        public Tuple<Int32, List<ACESSO_METODO>, Boolean> ExecuteFilter(Int32? assi, Int32? usuario, DateTime? inicio, DateTime? final, String sigla, String entidade, String metodo, Int32 idAss)
        {
            try
            {
                List<ACESSO_METODO> objeto = new List<ACESSO_METODO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(assi, usuario, inicio, final, sigla, entidade, metodo, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }

                // Monta tupla
                var tupla = Tuple.Create(volta, objeto, true);
                return tupla;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(ACESSO_METODO item)
        {
            try
            {
                // Completa objeto
                item.ACES_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(ACESSO_METODO item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.Edit(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
