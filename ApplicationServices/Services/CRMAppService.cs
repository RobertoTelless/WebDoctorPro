using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ApplicationServices.Services
{
    public class CRMAppService : AppServiceBase<CRM>, ICRMAppService
    {
        private readonly ICRMService _baseService;
        private readonly ILeadService _cliService;

        public CRMAppService(ICRMService baseService, ILeadService cliService) : base(baseService)
        {
            _baseService = baseService;
            _cliService = cliService;
        }

        public List<CRM> GetAllItens(Int32 idAss)
        {
            List<CRM> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<CRM> GetAllItensAdm(Int32 idAss)
        {
            List<CRM> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public List<CRM> GetTarefaStatus(Int32 tipo, Int32 idAss)
        {
            List<CRM> lista = _baseService.GetTarefaStatus(tipo, idAss);
            return lista;
        }

        public List<CRM> GetByDate(DateTime data, Int32 idAss)
        {
            List<CRM> lista = _baseService.GetByDate(data, idAss);
            return lista;
        }

        public List<CRM> GetByUser(Int32 user)
        {
            List<CRM> lista = _baseService.GetByUser(user);
            return lista;
        }

        public CRM GetItemById(Int32 id)
        {
            CRM item = _baseService.GetItemById(id);
            return item;
        }

        public MOTIVO_CANCELAMENTO GetMotivoCancelamentoById(Int32 id)
        {
            return _baseService.GetMotivoCancelamentoById(id);
        }

        public MOTIVO_ENCERRAMENTO GetMotivoEncerramentoById(Int32 id)
        {
            return _baseService.GetMotivoEncerramentoById(id);
        }

        public USUARIO GetUserById(Int32 id)
        {
            USUARIO item = _baseService.GetUserById(id);
            return item;
        }

        public CRM_CONTATO GetContatoById(Int32 id)
        {
            CRM_CONTATO lista = _baseService.GetContatoById(id);
            return lista;
        }

        public CRM_ACAO GetAcaoById(Int32 id)
        {
            CRM_ACAO lista = _baseService.GetAcaoById(id);
            return lista;
        }

        public CRM CheckExist(CRM tarefa, Int32 idUsu, Int32 idAss)
        {
            CRM item = _baseService.CheckExist(tarefa, idUsu, idAss);
            return item;
        }

        public List<CRM_ACAO> GetAllAcoes(Int32 idAss)
        {
            List<CRM_ACAO> lista = _baseService.GetAllAcoes(idAss);
            return lista;
        }

        public List<CRM_ACAO> GetAllAcoes()
        {
            List<CRM_ACAO> lista = _baseService.GetAllAcoes();
            return lista;
        }

        public List<CRM_FOLLOW> GetAllFollow(Int32 idAss)
        {
            List<CRM_FOLLOW> lista = _baseService.GetAllFollow(idAss);
            return lista;
        }

        public List<CRM_COMENTARIO> GetAllAnotacao(Int32 idAss)
        {
            List<CRM_COMENTARIO> lista = _baseService.GetAllAnotacao(idAss);
            return lista;
        }

        public List<TIPO_ACAO> GetAllTipoAcao(Int32 idAss)
        {
            List<TIPO_ACAO> lista = _baseService.GetAllTipoAcao(idAss);
            return lista;
        }

        public List<TIPO_FOLLOW> GetAllTipoFollow(Int32 idAss)
        {
            List<TIPO_FOLLOW> lista = _baseService.GetAllTipoFollow(idAss);
            return lista;
        }

        public List<MOTIVO_CANCELAMENTO> GetAllMotivoCancelamento(Int32 idAss)
        {
            List<MOTIVO_CANCELAMENTO> lista = _baseService.GetAllMotivoCancelamento(idAss);
            return lista;
        }

        public List<MOTIVO_ENCERRAMENTO> GetAllMotivoEncerramento(Int32 idAss)
        {
            List<MOTIVO_ENCERRAMENTO> lista = _baseService.GetAllMotivoEncerramento(idAss);
            return lista;
        }

        public List<CRM_ORIGEM> GetAllOrigens(Int32 idAss)
        {
            List<CRM_ORIGEM> lista = _baseService.GetAllOrigens(idAss);
            return lista;
        }

        public CRM_ANEXO GetAnexoById(Int32 id)
        {
            CRM_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public CRM_COMENTARIO GetComentarioById(Int32 id)
        {
            CRM_COMENTARIO lista = _baseService.GetComentarioById(id);
            return lista;
        }

        public CRM_FOLLOW GetFollowById(Int32 id)
        {
            CRM_FOLLOW lista = _baseService.GetFollowById(id);
            return lista;
        }

        public Tuple<Int32, List<CRM>, Boolean> ExecuteFilter(Int32? status, DateTime? inicio, DateTime? final, Int32? origem, Int32? adic, String nome, String busca,  Int32? estrela, Int32? temperatura, Int32? funil, String campanha, Int32? filial, Int32 idAss)
        {
            try
            {
                List<CRM> objeto = new List<CRM>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(status, inicio, final, origem, adic, nome, busca, estrela, temperatura, funil, campanha, filial, idAss);
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

        public Int32 ValidateCreate(CRM item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.USUA_CD_ID, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                //Verifica Campos
                if (item.TIPO_CRM != null)
                {
                    item.TIPO_CRM = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }

                // Verifica existencia prévia

                // Completa objeto
                item.CRM1_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Processo CRM - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CRM>(item),
                    LOG_IN_SISTEMA = 6
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);

                // Gera diario
                LEAD cli = _cliService.GetItemById(item.LEAD_CD_ID.Value);
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                dia.USUA_CD_ID = usuario.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = item.CRM1_CD_ID;
                dia.DIPR_NM_OPERACAO = "Criação de Processo";
                dia.DIPR_DS_DESCRICAO = "Criação do Processo " + item.CRM1_NM_NOME.ToUpper() + " para o Lead " + cli.LEAD_NM_NOME.ToUpper();
                dia.EMPR_CD_ID = usuario.EMPR_CD_ID;
                dia.DIPR_IN_SISTEMA = 6;
                Int32 volta1 = _baseService.CreateDiario(dia);
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public Int32 ValidateEdit(CRM item, CRM itemAntes, USUARIO usuario)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Verificação
                if (item.CRM1_DT_ENCERRAMENTO != null)
                {
                    if (item.CRM1_DT_ENCERRAMENTO < item.CRM1_DT_CRIACAO)
                    {
                        return 1;
                    }
                    if (item.CRM1_DT_ENCERRAMENTO > DateTime.Today.Date)
                    {
                        return 2;
                    }
                }
                if (item.CRM1_DT_CANCELAMENTO != null)
                {
                    if (item.CRM1_DT_CANCELAMENTO < item.CRM1_DT_CRIACAO)
                    {
                        return 3;
                    }
                    if (item.CRM1_DT_CANCELAMENTO > DateTime.Today.Date)
                    {
                        return 4;
                    }
                    if (item.CRM1_DS_MOTIVO_CANCELAMENTO == null)
                    {
                        return 5;
                    }
                }

                // Serializa registro
                LEAD cli = _cliService.GetItemById(item.LEAD_CD_ID.Value);
                DTO_CRM dto = MontarCRMDTOObj(item);
                DTO_CRM dtoAntes = MontarCRMDTOObj(itemAntes);
                String json = JsonConvert.SerializeObject(dto, settings);
                String jsonAntes = JsonConvert.SerializeObject(dtoAntes, settings);

                // Monta Log
                LOG log = new LOG();
                if (item.CRM1_DT_CANCELAMENTO != null)
                {
                    log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        LOG_NM_OPERACAO = "Processo CRM - Cancelamento",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_TX_REGISTRO_ANTES = jsonAntes,
                        LOG_IN_SISTEMA = 6
                    };

                    // Gera diario
                    DIARIO_PROCESSO dia1 = new DIARIO_PROCESSO();
                    dia1.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    dia1.USUA_CD_ID = usuario.USUA_CD_ID;
                    dia1.DIPR_DT_DATA = DateTime.Today.Date;
                    dia1.CRM1_CD_ID = item.CRM1_CD_ID;
                    dia1.DIPR_NM_OPERACAO = "Cancelamento de Processo";
                    dia1.DIPR_DS_DESCRICAO = "Cancelamento do Processo " + item.CRM1_NM_NOME.ToUpper() + ". Lead: " + cli.LEAD_NM_NOME.ToUpper();
                    Int32 volta4 = _baseService.CreateDiario(dia1);

                }
                else if (item.CRM1_DT_ENCERRAMENTO != null)
                {
                    log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        LOG_NM_OPERACAO = "Processo CRM - Encerramento",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_TX_REGISTRO_ANTES = jsonAntes,
                        LOG_IN_SISTEMA = 6
                    };

                    // Gera diario
                    DIARIO_PROCESSO dia1 = new DIARIO_PROCESSO();
                    dia1.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    dia1.USUA_CD_ID = usuario.USUA_CD_ID;
                    dia1.DIPR_DT_DATA = DateTime.Today.Date;
                    dia1.CRM1_CD_ID = item.CRM1_CD_ID;
                    dia1.DIPR_NM_OPERACAO = "Encerramento de Processo";
                    dia1.DIPR_DS_DESCRICAO = "Encerramento do Processo " + item.CRM1_NM_NOME.ToUpper() + ". Lead: " + cli.LEAD_NM_NOME.ToUpper();
                    Int32 volta4 = _baseService.CreateDiario(dia1);

                }
                else
                {
                    log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        LOG_NM_OPERACAO = "Processo CRM - Alteração",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = json,
                        LOG_TX_REGISTRO_ANTES = jsonAntes,
                        LOG_IN_SISTEMA = 6
                    };

                    // Gera diario
                    DIARIO_PROCESSO dia1 = new DIARIO_PROCESSO();
                    dia1.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    dia1.USUA_CD_ID = usuario.USUA_CD_ID;
                    dia1.DIPR_DT_DATA = DateTime.Today.Date;
                    dia1.CRM1_CD_ID = item.CRM1_CD_ID;
                    dia1.DIPR_NM_OPERACAO = "Alteração de Processo";
                    dia1.DIPR_DS_DESCRICAO = "Alteração do Processo " + item.CRM1_NM_NOME.ToUpper() + ". Lead: " + cli.LEAD_NM_NOME.ToUpper();
                    Int32 volta4 = _baseService.CreateDiario(dia1);

                }

                // Persiste
                //item.LEAD = null;
                item.CRM_ORIGEM = null;
                Int32 volta = _baseService.Edit(item, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditSimples(CRM item, CRM itemAntes, USUARIO usuario)
        {
            try
            {
                // Verificação
                if (item.CRM1_DT_ENCERRAMENTO != null)
                {
                    if (item.CRM1_DT_ENCERRAMENTO < item.CRM1_DT_CRIACAO)
                    {
                        return 1;
                    }
                    if (item.CRM1_DT_ENCERRAMENTO > DateTime.Today.Date)
                    {
                        return 2;
                    }
                }
                if (item.CRM1_DT_CANCELAMENTO != null)
                {
                    if (item.CRM1_DT_CANCELAMENTO < item.CRM1_DT_CRIACAO)
                    {
                        return 3;
                    }
                    if (item.CRM1_DT_CANCELAMENTO > DateTime.Today.Date)
                    {
                        return 4;
                    }
                }

                // Persiste
                item.LEAD = null;
                item.CRM_ORIGEM = null;
                Int32 volta = _baseService.EditTudo(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CRM item, CRM itemAntes)
        {
            try
            {
                if (item.CRM1_DT_ENCERRAMENTO != null)
                {
                    if (item.CRM1_DT_ENCERRAMENTO < item.CRM1_DT_CRIACAO)
                    {
                        return 1;
                    }
                    if (item.CRM1_DT_ENCERRAMENTO > DateTime.Today.Date)
                    {
                        return 2;
                    }
                }
                if (item.CRM1_DT_CANCELAMENTO != null)
                {
                    if (item.CRM1_DT_CANCELAMENTO < item.CRM1_DT_CRIACAO)
                    {
                        return 3;
                    }
                    if (item.CRM1_DT_CANCELAMENTO > DateTime.Today.Date)
                    {
                        return 4;
                    }
                }

                // Persiste
                //item.LEAD = null;
                item.CRM_ORIGEM = null;
                Int32 volta = _baseService.Edit(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CRM item, USUARIO usuario)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Verifica integridade
                List<CRM_ACAO> acao = item.CRM_ACAO.Where(p => p.CRAC_IN_STATUS == 1).ToList();
                if (acao.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.CRM1_IN_ATIVO = 2;
                item.CRM1_DT_EXCLUSAO = DateTime.Today.Date;

                // Serializa registro
                LEAD cli = _cliService.GetItemById(item.LEAD_CD_ID.Value);
                DTO_CRM dto = MontarCRMDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "Processo CRM - Exclusão",
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };

                // Persiste
                Int32 volta =  _baseService.Edit(item, log);

                // Gera diario
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                dia.USUA_CD_ID = usuario.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = item.CRM1_CD_ID;
                dia.DIPR_NM_OPERACAO = "Exclusão de Processo";
                dia.DIPR_DS_DESCRICAO = "Exclusão do Processo " + item.CRM1_NM_NOME.ToUpper() + ". Lead: " + cli.LEAD_NM_NOME.ToUpper();
                Int32 volta3 = _baseService.CreateDiario(dia);

                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CRM item, USUARIO usuario)
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
                item.CRM1_IN_ATIVO = 1;

                // Serializa registro
                LEAD cli = _cliService.GetItemById(item.CLIE_CD_ID);
                DTO_CRM dto = MontarCRMDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "Proceso CRM - Reativação",
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 2
                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);

                // Gera diario
                DIARIO_PROCESSO dia = new DIARIO_PROCESSO();
                dia.ASSI_CD_ID = usuario.ASSI_CD_ID;
                dia.USUA_CD_ID = usuario.USUA_CD_ID;
                dia.DIPR_DT_DATA = DateTime.Today.Date;
                dia.CRM1_CD_ID = item.CRM1_CD_ID;
                dia.DIPR_NM_OPERACAO = "Reativação de Processo";
                dia.DIPR_DS_DESCRICAO = "Reativação do Processo " + item.CRM1_NM_NOME + ". Lead: " + cli.LEAD_NM_NOME;
                Int32 volta3 = _baseService.CreateDiario(dia);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditContato(CRM_CONTATO item)
        {
            try
            {
                // Persiste
                return _baseService.EditContato(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateContato(CRM_CONTATO item)
        {
            try
            {
                item.CRCO_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateContato(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditAnotacao(CRM_COMENTARIO item)
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

        public Int32 ValidateEditFollow(CRM_FOLLOW item)
        {
            try
            {
                // Persiste
                return _baseService.EditFollow(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditAcao(CRM_ACAO item)
        {
            try
            {
                // Persiste
                return _baseService.EditAcao(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateAcao(CRM_ACAO item, USUARIO usuario)
        {
            try
            {
                item.CRAC_IN_ATIVO = 1;

                // Recupera CRM
                CRM crm = _baseService.GetItemById(item.CRM1_CD_ID);
                LEAD cli = _cliService.GetItemById(crm.LEAD_CD_ID.Value);

                // Persiste
                Int32 volta = _baseService.CreateAcao(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditAnexo(CRM_ANEXO item)
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

        public DTO_CRM MontarCRMDTOObj(CRM antes)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_CRM()
                {
                    ASSI_CD_ID = antes.ASSI_CD_ID,
                    CRM1_AQ_IMAGEM = antes.CRM1_AQ_IMAGEM,
                    CRM1_DS_DESCRICAO = antes.CRM1_DS_DESCRICAO,
                    CRM1_DS_INFORMACOES_ENCERRAMENTO = antes.CRM1_DS_INFORMACOES_ENCERRAMENTO,
                    CRM1_DS_MOTIVO_CANCELAMENTO = antes.CRM1_DS_MOTIVO_CANCELAMENTO,
                    CRM1_DT_CANCELAMENTO = antes.CRM1_DT_CANCELAMENTO,
                    CRM1_DT_CRIACAO = antes.CRM1_DT_CRIACAO,
                    CRM1_DT_ENCERRAMENTO = antes.CRM1_DT_ENCERRAMENTO,
                    CRM1_GU_GUID = antes.CRM1_GU_GUID,
                    CRM1_ID_IDENTIFICADOR = antes.CRM1_ID_IDENTIFICADOR,
                    CRM1_IN_ATIVO = antes.CRM1_IN_ATIVO,
                    CRM1_IN_DUMMY = antes.CRM1_IN_DUMMY,
                    CRM1_IN_ENCERRADO = antes.CRM1_IN_ENCERRADO,
                    CRM1_IN_ESTRELA = antes.CRM1_IN_ESTRELA,
                    CRM1_IN_SISTEMA = antes.CRM1_IN_SISTEMA,
                    CRM1_IN_STATUS = antes.CRM1_IN_STATUS,
                    CRM1_NM_CAMPANHA = antes.CRM1_NM_CAMPANHA,
                    CRM1_NM_NOME = antes.CRM1_NM_NOME,
                    CRM1_NR_ATRASO = antes.CRM1_NR_ATRASO,
                    CRM1_NR_TEMPERATURA = antes.CRM1_NR_TEMPERATURA,
                    CRM1_TX_INFORMACOES_GERAIS = antes.CRM1_TX_INFORMACOES_GERAIS,
                    CRM1_VL_VALOR_FINAL = antes.CRM1_VL_VALOR_FINAL,
                    CRM1_VL_VALOR_INICIAL = antes.CRM1_VL_VALOR_INICIAL,
                    EMFI_CD_ID = antes.EMFI_CD_ID,
                    EMPR_CD_ID = antes.EMPR_CD_ID,
                    FUNI_CD_ID = antes.FUNI_CD_ID,
                    LEAD_CD_ID = antes.LEAD_CD_ID,
                    MENS_CD_ID = antes.MENS_CD_ID,
                    MOCA_CD_ID = antes.MOCA_CD_ID,
                    MOEN_CD_ID = antes.MOEN_CD_ID,
                    ORIG_CD_ID = antes.ORIG_CD_ID,
                    PACI_CD_ID = antes.PACI_CD_ID,
                    TICR_CD_ID = antes.TICR_CD_ID,
                    USUA_CD_ID = antes.USUA_CD_ID,
                };
                return mediDTO;
            }
        }

    }
}
