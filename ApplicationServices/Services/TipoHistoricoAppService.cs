using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using Newtonsoft.Json;

namespace ApplicationServices.Services
{
    public class TipoHistoricoAppService : AppServiceBase<TIPO_HISTORICO>, ITipoHistoricoAppService
    {
        private readonly ITipoHistoricoService _baseService;

        public TipoHistoricoAppService(ITipoHistoricoService baseService) : base(baseService)
        {
            _baseService = baseService;
        }


        public List<TIPO_HISTORICO> GetAllItens(Int32 idAss)
        {
            List<TIPO_HISTORICO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<TIPO_HISTORICO> GetAllItensAdm(Int32 idAss)
        {
            List<TIPO_HISTORICO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public TIPO_HISTORICO GetItemById(Int32 id)
        {
            TIPO_HISTORICO item = _baseService.GetItemById(id);
            return item;
        }

        public TIPO_HISTORICO CheckExist(TIPO_HISTORICO conta, Int32 idAss)
        {
            TIPO_HISTORICO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public Int32 ValidateCreate(TIPO_HISTORICO item, USUARIO usuario)
        {
            try
            {
                if (usuario != null)
                {
                    // Verifica existencia prévia
                    if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                    {
                        return 1;
                    }

                    // Completa objeto
                    item.TIHI_IN_ATIVO = 1;

                    // Monta Log
                    String json = JsonConvert.SerializeObject(item);

                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "AddTIHI",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_IN_SISTEMA = 1
                    };

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                else
                {
                    // Completa objeto
                    item.TIHI_IN_ATIVO = 1;

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(TIPO_HISTORICO item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.Create(item);
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(TIPO_HISTORICO item, TIPO_HISTORICO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                String json = JsonConvert.SerializeObject(item);
                String jsonAntes = JsonConvert.SerializeObject(itemAntes);

                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EdtTIHI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_TX_REGISTRO_ANTES = jsonAntes,
                    LOG_IN_SISTEMA = 1
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(TIPO_HISTORICO item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.LOCACAO_HISTORICO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.TIHI_IN_ATIVO = 0;

                // Monta Log
                String json = JsonConvert.SerializeObject(item);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelTIHI",
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 1

                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(TIPO_HISTORICO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.TIHI_IN_ATIVO = 1;

                // Monta Log
                String json = JsonConvert.SerializeObject(item);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReaTIHI",
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 1

                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
