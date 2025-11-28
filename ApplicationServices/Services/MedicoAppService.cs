using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using EntitiesServices.Work_Classes;
using Newtonsoft.Json;

namespace ApplicationServices.Services
{
    public class MedicoAppService : AppServiceBase<MEDICOS>, IMedicoAppService
    {
        private readonly IMedicoService _baseService;

        public MedicoAppService(IMedicoService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<ESPECIALIDADE> GetAllEspec(Int32 idAss)
        {
            return _baseService.GetAllEspec(idAss);
        }

        public List<TIPO_ENVIO> GetAllTipos(Int32 idAss)
        {
            return _baseService.GetAllTipos(idAss);
        }

        public List<MEDICOS> GetAllItens(Int32 idAss)
        {
            List<MEDICOS> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public MEDICOS CheckExist(MEDICOS conta, Int32 idAss)
        {
            MEDICOS item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<MEDICOS> GetAllItensAdm(Int32 idAss)
        {
            List<MEDICOS> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public MEDICOS GetItemById(Int32 id)
        {
            MEDICOS item = _baseService.GetItemById(id);
            return item;
        }

        public Tuple<Int32, List<MEDICOS>, Boolean> ExecuteFilter(Int32? espec, String nome, String crm, String email, Int32 idAss)
        {
            try
            {
                List<MEDICOS> objeto = new List<MEDICOS>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(espec, nome, crm, email, idAss);
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

        public Int32 ValidateCreate(MEDICOS item, USUARIO usuario)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.MEDC_IN_ATIVO = 1;

                // Monta Log
                DTO_Medico dto = MontarMedicoDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Médico - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
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

        public Int32 ValidateEdit(MEDICOS item, MEDICOS itemAntes, USUARIO usuario)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Monta Log
                DTO_Medico dto = MontarMedicoDTO(item.MEDC_CD_ID);
                DTO_Medico dtoAntes = MontarMedicoDTOObj(itemAntes);
                String json = JsonConvert.SerializeObject(dto, settings);
                String jsonAntes = JsonConvert.SerializeObject(dtoAntes, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Médico - Alteração",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_TX_REGISTRO_ANTES = jsonAntes,
                    LOG_IN_SISTEMA = 6
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

        public Int32 ValidateDelete(MEDICOS item, USUARIO usuario)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Checa integridade
                if (item.MEDICOS_ENVIO.Count() > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.MEDC_IN_ATIVO = 0;

                // Monta Log
                DTO_Medico dto = MontarMedicoDTO(item.MEDC_CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "Médico - Exclusão",
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6

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

        public Int32 ValidateReativar(MEDICOS item, USUARIO usuario)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Verifica integridade referencial

                // Acerta campos
                item.MEDC_IN_ATIVO = 1;

                // Monta Log
                DTO_Medico dto = MontarMedicoDTO(item.MEDC_CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "Médico - Reativação",
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6

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

        public List<MEDICOS_ENVIO> GetAllEnvio(Int32 idAss)
        {
            return _baseService.GetAllEnvio(idAss);
        }

        public MEDICOS_ENVIO GetEnvioById(Int32 id)
        {
            MEDICOS_ENVIO lista = _baseService.GetEnvioById(id);
            return lista;
        }

        public Int32 ValidateEditEnvio(MEDICOS_ENVIO item)
        {
            try
            {
                // Persiste
                return _baseService.EditEnvio(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateEnvio(MEDICOS_ENVIO item)
        {
            try
            {
                item.MEEV_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateEnvio(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public MEDICOS_ENVIO_ANEXO GetMedicoAnexoById(Int32 id)
        {
            MEDICOS_ENVIO_ANEXO lista = _baseService.GetMedicoAnexoById(id);
            return lista;
        }

        public Int32 ValidateEditMedicoAnexo(MEDICOS_ENVIO_ANEXO item)
        {
            try
            {
                // Persiste
                return _baseService.EditMedicoAnexo(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public MEDICOS_ENVIO_ANOTACAO GetAnotacaoById(Int32 id)
        {
            MEDICOS_ENVIO_ANOTACAO lista = _baseService.GetAnotacaoById(id);
            return lista;
        }

        public Int32 ValidateEditAnotacao(MEDICOS_ENVIO_ANOTACAO item)
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

        public DTO_Medico_Envio MontarMedicoEnvioDTO(Int32 mediId)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = context.MEDICOS_ENVIO
                    .Where(l => l.MEEV_CD_ID == mediId)
                    .Select(l => new DTO_Medico_Envio
                    {
                        ASSI_CD_ID = l.ASSI_CD_ID,
                        USUA_CD_ID = l.USUA_CD_ID,
                        MEDC_CD_ID = l.MEDC_CD_ID,
                        MEEV_CD_ID = l.MEEV_CD_ID,
                        MEEV_DS_SINTOMAS = l.MEEV_DS_SINTOMAS,
                        MEEV_DT_ENVIO = l.MEEV_DT_ENVIO,
                        MEEV_DT_NOITE_FINAL = l.MEEV_DT_NOITE_FINAL,
                        MEEV_DT_NOITE_INICIO = l.MEEV_DT_NOITE_INICIO,
                        MEEV_DT_REMESSA = l.MEEV_DT_REMESSA,
                        MEEV_GU_IDENTIFICADOR = l.MEEV_GU_IDENTIFICADOR,
                        MEEV_IN_ANAMNESE = l.MEEV_IN_ANAMNESE,
                        MEEV_IN_ATIVO = l.MEEV_IN_ATIVO,
                        MEEV_IN_ENVIADO = l.MEEV_IN_ENVIADO,
                        MEEV_IN_ENVIOS = l.MEEV_IN_ENVIOS,
                        MEEV_IN_IAH = l.MEEV_IN_IAH,
                        MEEV_IN_IAH_RESIDUAL = l.MEEV_IN_IAH_RESIDUAL,
                        MEEV_IN_NUM_NOITES = l.MEEV_IN_NUM_NOITES,
                        MEEV_IN_PERCENTUAL = l.MEEV_IN_PERCENTUAL,
                        MEEV_NM_EQUIPAMENTO = l.MEEV_NM_EQUIPAMENTO,
                        MEEV_NM_MASCARA = l.MEEV_NM_MASCARA,
                        MEEV_NM_TITULO = l.MEEV_NM_TITULO,
                        MEEV_NR_IAH_RESIDUAL = l.MEEV_NR_IAH_RESIDUAL,
                        MEEV_NR_MEDIA_USO = l.MEEV_NR_MEDIA_USO,
                        MEEV_NR_PARAM_PRESSAO = l.MEEV_NR_PARAM_PRESSAO,
                        MEEV_NR_PRESSAO_POSITIVA = l.MEEV_NR_PRESSAO_POSITIVA,
                        MEEV_TX_MENSAGEM = l.MEEV_TX_MENSAGEM,
                        PACI_CD_ID = l.PACI_CD_ID,
                        TIEN_CD_ID = l.TIEN_CD_ID,
                    })
                    .FirstOrDefault();
                return mediDTO;
            }
        }

        public DTO_Medico_Envio MontarMedicoEnvioDTOObj(MEDICOS_ENVIO antes)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_Medico_Envio()
                {
                    ASSI_CD_ID = antes.ASSI_CD_ID,
                    USUA_CD_ID = antes.USUA_CD_ID,
                    MEDC_CD_ID = antes.MEDC_CD_ID,
                    MEEV_CD_ID = antes.MEEV_CD_ID,
                    MEEV_DS_SINTOMAS = antes.MEEV_DS_SINTOMAS,
                    MEEV_DT_ENVIO = antes.MEEV_DT_ENVIO,
                    MEEV_DT_NOITE_FINAL = antes.MEEV_DT_NOITE_FINAL,
                    MEEV_DT_NOITE_INICIO = antes.MEEV_DT_NOITE_INICIO,
                    MEEV_DT_REMESSA = antes.MEEV_DT_REMESSA,
                    MEEV_GU_IDENTIFICADOR = antes.MEEV_GU_IDENTIFICADOR,
                    MEEV_IN_ANAMNESE = antes.MEEV_IN_ANAMNESE,
                    MEEV_IN_ATIVO = antes.MEEV_IN_ATIVO,
                    MEEV_IN_ENVIADO = antes.MEEV_IN_ENVIADO,
                    MEEV_IN_ENVIOS = antes.MEEV_IN_ENVIOS,
                    MEEV_IN_IAH = antes.MEEV_IN_IAH,
                    MEEV_IN_IAH_RESIDUAL = antes.MEEV_IN_IAH_RESIDUAL,
                    MEEV_IN_NUM_NOITES = antes.MEEV_IN_NUM_NOITES,
                    MEEV_IN_PERCENTUAL = antes.MEEV_IN_PERCENTUAL,
                    MEEV_NM_EQUIPAMENTO = antes.MEEV_NM_EQUIPAMENTO,
                    MEEV_NM_MASCARA = antes.MEEV_NM_MASCARA,
                    MEEV_NM_TITULO = antes.MEEV_NM_TITULO,
                    MEEV_NR_IAH_RESIDUAL = antes.MEEV_NR_IAH_RESIDUAL,
                    MEEV_NR_MEDIA_USO = antes.MEEV_NR_MEDIA_USO,
                    MEEV_NR_PARAM_PRESSAO = antes.MEEV_NR_PARAM_PRESSAO,
                    MEEV_NR_PRESSAO_POSITIVA = antes.MEEV_NR_PRESSAO_POSITIVA,
                    MEEV_TX_MENSAGEM = antes.MEEV_TX_MENSAGEM,
                    PACI_CD_ID = antes.PACI_CD_ID,
                    TIEN_CD_ID = antes.TIEN_CD_ID,
                };
                return mediDTO;
            }
        }

        public DTO_Medico MontarMedicoDTO(Int32 mediId)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = context.MEDICOS
                    .Where(l => l.MEDC_CD_ID == mediId)
                    .Select(l => new DTO_Medico
                    {
                        ASSI_CD_ID = l.ASSI_CD_ID,
                        MEDC_CD_ID = l.MEDC_CD_ID,
                        MEDC_EM_EMAIL = l.MEDC_EM_EMAIL,
                        MEDC_GU_IDENTIFICADOR = l.MEDC_GU_IDENTIFICADOR,
                        MEDC_IN_ATIVO = l.MEDC_IN_ATIVO,
                        MEDC_NM_BAIRRO = l.MEDC_NM_BAIRRO,
                        MEDC_NM_CIDADE = l.MEDC_NM_CIDADE,
                        MEDC_NM_COMPLEMENTO = l.MEDC_NM_COMPLEMENTO,
                        MEDC_NM_ENDERECO = l.MEDC_NM_ENDERECO,
                        MEDC_NM_MEDICO = l.MEDC_NM_MEDICO,
                        MEDC_NR_CELULAR = l.MEDC_NR_CELULAR,
                        MEDC_NR_CEP = l.MEDC_NR_CEP,
                        MEDC_NR_CRM = l.MEDC_NR_CRM,
                        MEDC_NR_NUMERO = l.MEDC_NR_NUMERO,
                        MEDC_NR_TELEFONE = l.MEDC_NR_TELEFONE,
                        ESPE_CD_ID = l.ESPE_CD_ID,
                        UF_CD_ID = l.UF_CD_ID,
                    })
                    .FirstOrDefault();
                return mediDTO;
            }
        }

        public DTO_Medico MontarMedicoDTOObj(MEDICOS antes)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_Medico()
                {
                    ASSI_CD_ID = antes.ASSI_CD_ID,
                    MEDC_CD_ID =  antes.MEDC_CD_ID,
                    MEDC_EM_EMAIL = antes.MEDC_EM_EMAIL,
                    MEDC_GU_IDENTIFICADOR = antes.MEDC_GU_IDENTIFICADOR,
                    MEDC_IN_ATIVO = antes.MEDC_IN_ATIVO,
                    MEDC_NM_BAIRRO = antes.MEDC_NM_BAIRRO,
                    MEDC_NM_CIDADE = antes.MEDC_NM_CIDADE,
                    MEDC_NM_COMPLEMENTO = antes.MEDC_NM_COMPLEMENTO,
                    MEDC_NM_ENDERECO = antes.MEDC_NM_ENDERECO,
                    MEDC_NM_MEDICO = antes.MEDC_NM_MEDICO,
                    MEDC_NR_CELULAR = antes.MEDC_NR_CELULAR,
                    MEDC_NR_CEP = antes.MEDC_NR_CEP,
                    MEDC_NR_CRM = antes.MEDC_NR_CRM,
                    MEDC_NR_NUMERO = antes.MEDC_NR_NUMERO,
                    MEDC_NR_TELEFONE = antes.MEDC_NR_TELEFONE,
                    ESPE_CD_ID = antes.ESPE_CD_ID,
                    UF_CD_ID = antes.UF_CD_ID,
                };
                return mediDTO;
            }
        }

        public List<MEDICOS_MENSAGEM> GetAllTextoMensagem(Int32 idAss)
        {
            List<MEDICOS_MENSAGEM> lista = _baseService.GetAllTextoMensagem(idAss);
            return lista;
        }

        public MEDICOS_MENSAGEM CheckExistTextoMensagem(MEDICOS_MENSAGEM conta, Int32 idAss)
        {
            MEDICOS_MENSAGEM item = _baseService.CheckExistTextoMensagem(conta, idAss);
            return item;
        }

        public MEDICOS_MENSAGEM GetTextoMensagemById(Int32 id)
        {
            MEDICOS_MENSAGEM item = _baseService.GetTextoMensagemById(id);
            return item;
        }

        public Int32 ValidateEditTextoMensagem(MEDICOS_MENSAGEM item)
        {
            try
            {
                // Persiste
                return _baseService.EditTextoMensagem(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateTextoMensagem(MEDICOS_MENSAGEM item)
        {
            try
            {
                item.METX_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateTextoMensagem(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
