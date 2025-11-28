using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;

namespace ApplicationServices.Services
{
    public class TemplateEMailHTMLAppService : AppServiceBase<TEMPLATE_EMAIL_HTML>, ITemplateEMailHTMLAppService
    {
        private readonly ITemplateEMailHTMLService _baseService;

        public TemplateEMailHTMLAppService(ITemplateEMailHTMLService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<TEMPLATE_EMAIL_HTML> GetAllItens(Int32 idAss)
        {
            List<TEMPLATE_EMAIL_HTML> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public TEMPLATE_EMAIL_HTML CheckExist(TEMPLATE_EMAIL_HTML conta, Int32 idAss)
        {
            TEMPLATE_EMAIL_HTML item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<TEMPLATE_EMAIL_HTML> GetAllItensAdm(Int32 idAss)
        {
            List<TEMPLATE_EMAIL_HTML> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public TEMPLATE_EMAIL_HTML GetItemById(Int32 id)
        {
            TEMPLATE_EMAIL_HTML item = _baseService.GetItemById(id);
            return item;
        }

        public TEMPLATE_EMAIL_HTML GetItemByNome(String nome)
        {
            TEMPLATE_EMAIL_HTML item = _baseService.GetItemByNome(nome);
            return item;
        }

        public Int32 ValidateCreate(TEMPLATE_EMAIL_HTML item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.TEHT_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddTEHT",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TEMPLATE_EMAIL_HTML>(item),
                    LOG_IN_SISTEMA = 2
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(TEMPLATE_EMAIL_HTML item, TEMPLATE_EMAIL_HTML itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EdtTEHT",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TEMPLATE_EMAIL_HTML>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<TEMPLATE_EMAIL_HTML>(itemAntes),
                    LOG_IN_SISTEMA = 2
                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(TEMPLATE_EMAIL_HTML item, USUARIO usuario)
        {
            try
            {
                // Acerta campos
                item.TEHT_IN_ATIVO = 0;

                // Monta Log
                String frase = "Modelo: " + item.TEHT_NM_NOME;
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelTEHT",
                    LOG_TX_REGISTRO = frase,
                    LOG_IN_SISTEMA = 2

                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(TEMPLATE_EMAIL_HTML item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.TEHT_IN_ATIVO = 1;

                // Monta Log
                String frase = "Modelo: " + item.TEHT_NM_NOME;
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReaTEHT",
                    LOG_TX_REGISTRO = frase,
                    LOG_IN_SISTEMA = 2

                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
