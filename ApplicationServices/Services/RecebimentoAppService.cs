using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Linq;

namespace ApplicationServices.Services
{
    public class RecebimentoAppService : AppServiceBase<CONSULTA_RECEBIMENTO>, IRecebimentoAppService
    {
        private readonly IRecebimentoService _baseService;
        private readonly IConfiguracaoService _confService;

        public RecebimentoAppService(IRecebimentoService baseService, IConfiguracaoService confService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
        }

        public CONSULTA_RECEBIMENTO GetItemById(Int32 id)
        {
            CONSULTA_RECEBIMENTO item = _baseService.GetItemById(id);
            return item;
        }

        public List<CONSULTA_RECEBIMENTO> GetAllItens(Int32 idAss)
        {
            return _baseService.GetAllItens(idAss);
        }

        public Tuple<Int32, List<CONSULTA_RECEBIMENTO>, Boolean> ExecuteFilterTuple(Int32? tipo, Int32? paciente, Int32? consulta, Int32? forma, String nome, DateTime? inicio, DateTime? final, Int32? conferido, Int32 idAss)
        {
            try
            {
                List<CONSULTA_RECEBIMENTO> objeto = new List<CONSULTA_RECEBIMENTO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(tipo, paciente, consulta, forma, nome, inicio, final, conferido, idAss);
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

        public RECEBIMENTO_ANEXO GetAnexoById(Int32 id)
        {
            RECEBIMENTO_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public Int32 ValidateEditAnexo(RECEBIMENTO_ANEXO item)
        {
            try
            {
                // Persiste
                return _baseService.EditAnexo(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public RECEBIMENTO_RECIBO GetReciboById(Int32 id)
        {
            RECEBIMENTO_RECIBO lista = _baseService.GetReciboById(id);
            return lista;
        }

        public Int32 ValidateEditRecibo(RECEBIMENTO_RECIBO item)
        {
            try
            {
                // Persiste
                return _baseService.EditRecibo(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public RECEBIMENTO_ANOTACAO GetAnotacaoById(Int32 id)
        {
            RECEBIMENTO_ANOTACAO lista = _baseService.GetAnotacaoById(id);
            return lista;
        }

        public Int32 ValidateEditAnotacao(RECEBIMENTO_ANOTACAO item)
        {
            try
            {
                // Persiste
                return _baseService.EditAnotacao(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<VALOR_CONSULTA> GetAllValorConsulta(Int32 idAss)
        {
            return _baseService.GetAllValorConsulta(idAss);
        }
        public List<FORMA_RECEBIMENTO> GetAllForma(Int32 idAss)
        {
            return _baseService.GetAllForma(idAss);
        }

        public FORMA_RECEBIMENTO GetFormaById(Int32 id)
        {
            return _baseService.GetFormaById(id);
        }

        public List<VALOR_CONVENIO> GetAllValorConvenio(Int32 idAss)
        {
            return _baseService.GetAllValorConvenio(idAss);
        }

        public Int32 ValidateCreate(CONSULTA_RECEBIMENTO item, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.CORE_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.Create(item);
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CONSULTA_RECEBIMENTO item, CONSULTA_RECEBIMENTO itemAntes, USUARIO usuario)
        {
            try
            {

                // Persiste
                Int32 volta = _baseService.Edit(item);
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CONSULTA_RECEBIMENTO item, CONSULTA_RECEBIMENTO itemAntes)
        {
            try
            {
                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CONSULTA_RECEBIMENTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CORE_IN_ATIVO = 0;

                // Persiste
                Int32 volta = _baseService.Edit(item);
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
