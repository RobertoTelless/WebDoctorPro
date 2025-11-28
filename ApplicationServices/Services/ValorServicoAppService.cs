using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;

namespace ApplicationServices.Services
{
    public class ValorServicoAppService : AppServiceBase<VALOR_SERVICO>, IValorServicoAppService
    {
        private readonly IValorServicoService _baseService;
        private readonly ITipoValorServicoService _servService;

        public ValorServicoAppService(IValorServicoService baseService, ITipoValorServicoService servService) : base(baseService)
        {
            _baseService = baseService;
            _servService = servService;
        }


        public List<VALOR_SERVICO> GetAllItens(Int32 idAss)
        {
            List<VALOR_SERVICO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<VALOR_SERVICO> GetAllItensAdm(Int32 idAss)
        {
            List<VALOR_SERVICO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public VALOR_SERVICO GetItemById(Int32 id)
        {
            VALOR_SERVICO item = _baseService.GetItemById(id);
            return item;
        }

        public VALOR_SERVICO CheckExist(VALOR_SERVICO conta, Int32 idAss)
        {
            VALOR_SERVICO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<TIPO_SERVICO_CONSULTA> GetAllServicos(Int32 idAss)
        {
            return _baseService.GetAllServicos(idAss);
        }

        public Int32 ValidateCreate(VALOR_SERVICO item, USUARIO usuario)
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
                    item.VASE_IN_ATIVO = 1;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "AddVASE",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<VALOR_SERVICO>(item),
                        LOG_IN_SISTEMA = 6
                    };

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                else
                {
                    // Completa objeto
                    item.VASE_IN_ATIVO = 1;

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


        public Int32 ValidateEdit(VALOR_SERVICO item, VALOR_SERVICO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EdtVASE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<VALOR_SERVICO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<VALOR_SERVICO>(itemAntes),
                    LOG_IN_SISTEMA = 6
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(VALOR_SERVICO item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.CONSULTA_RECEBIMENTO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.VASE_IN_ATIVO = 0;

                // Monta Log
                TIPO_SERVICO_CONSULTA serv = _servService.GetItemById(item.SERV_CD_ID.Value);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelVASE",
                    LOG_TX_REGISTRO = serv.SERV_NM_SERVICO,
                    LOG_IN_SISTEMA = 6

                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(VALOR_SERVICO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.VASE_IN_ATIVO = 1;

                // Monta Log
                TIPO_SERVICO_CONSULTA serv = _servService.GetItemById(item.SERV_CD_ID.Value);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReaVASE",
                    LOG_TX_REGISTRO = serv.SERV_NM_SERVICO,
                    LOG_IN_SISTEMA = 6

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
