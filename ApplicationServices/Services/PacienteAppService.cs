using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using ModelServices.EntitiesServices;
using EntitiesServices.Work_Classes;
using Newtonsoft.Json;

namespace ApplicationServices.Services
{
    public class PacienteAppService : AppServiceBase<PACIENTE>, IPacienteAppService
    {
        private readonly IPacienteService _baseService;
        private readonly IConfiguracaoService _confService;

        public PacienteAppService(IPacienteService baseService, IConfiguracaoService confService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
        }

        public PACIENTE CheckExist(PACIENTE tarefa, Int32 idAss)
        {
            PACIENTE item = _baseService.CheckExist(tarefa, idAss);
            return item;
        }

        public PACIENTE GetItemById(Int32 id)
        {
            PACIENTE item = _baseService.GetItemById(id);
            return item;
        }

        public PACIENTE GetItemByCPF(String cpf)
        {
            PACIENTE item = _baseService.GetItemByCPF(cpf);
            return item;
        }

        public List<PACIENTE_LOGIN> GetAllLogin(Int32 idAss)
        {
            return _baseService.GetAllLogin(idAss);
        }

        public List<PACIENTE> GetAllItens(Int32 idAss)
        {
            return _baseService.GetAllItens(idAss);
        }

        public List<PACIENTE> GetAllItensAdm(Int32 idAss)
        {
            return _baseService.GetAllItensAdm(idAss);
        }

        public List<CONTROLE_VERSAO> GetAllVersoes()
        {
            return _baseService.GetAllVersoes();
        }

        public CONTROLE_VERSAO GetVersaoById(Int32 id)
        {
            CONTROLE_VERSAO lista = _baseService.GetVersaoById(id);
            return lista;
        }

        public Tuple<Int32, List<PACIENTE>, Boolean> ExecuteFilterTuple(Int32? id, Int32? catId, Int32? sexo, String nome, String cpf, Int32? conv, Int32? menor, String celular, String email, String cidade, Int32? uf, Int32 idAss)
        {
            try
            {
                List<PACIENTE> objeto = new List<PACIENTE>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(id, catId, sexo, nome, cpf, conv, menor, celular, email, cidade, uf, idAss);
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

        public Tuple<Int32, List<PACIENTE_SOLICITACAO>, Boolean> ExecuteFilterTupleSolicitacao(Int32? tipo, String nome, DateTime? inicio, DateTime? final, String titulo, String descricao, Int32 idAss)
        {
            try
            {
                List<PACIENTE_SOLICITACAO> objeto = new List<PACIENTE_SOLICITACAO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterSolicitacao(tipo, nome, inicio, final, titulo, descricao, idAss);
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

        public Tuple<Int32, List<PACIENTE_ATESTADO>, Boolean> ExecuteFilterTupleAtestado(Int32? tipo, String nome, DateTime? inicio, DateTime? final, String titulo, String descricao, Int32 idAss)
        {
            try
            {
                List<PACIENTE_ATESTADO> objeto = new List<PACIENTE_ATESTADO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterAtestado(tipo, nome, inicio, final, titulo, descricao, idAss);
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

        public Tuple<Int32, List<PACIENTE_CONSULTA>, Boolean> ExecuteFilterTupleConsulta(Int32? tipo, String nome, DateTime? inicio, DateTime? final, Int32? encerrada, Int32? usuario, Int32 idAss)
        {
            try
            {
                List<PACIENTE_CONSULTA> objeto = new List<PACIENTE_CONSULTA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterConsulta(tipo, nome, inicio, final, encerrada, usuario, idAss);
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

        public Tuple<Int32, List<PACIENTE_CONSULTA>, Boolean> ExecuteFilterTupleConfirmaConsulta(Int32? tipo, String nome, DateTime? inicio, DateTime? final, Int32? situacao, Int32? usuario, Int32 idAss)
        {
            try
            {
                List<PACIENTE_CONSULTA> objeto = new List<PACIENTE_CONSULTA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterConfirmaConsulta(tipo, nome, inicio, final, situacao, usuario, idAss);
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

        public Tuple<Int32, List<PACIENTE_EXAMES>, Boolean> ExecuteFilterTupleExame(Int32? tipo, Int32? lab, String nome, DateTime? inicio, DateTime? final, String titulo, String descricao, Int32 idAss)
        {
            try
            {
                List<PACIENTE_EXAMES> objeto = new List<PACIENTE_EXAMES>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterExame(tipo, lab, nome, inicio, final, titulo, descricao, idAss);
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

        public Tuple<Int32, List<PACIENTE_PRESCRICAO>, Boolean> ExecuteFilterTuplePrescricao(Int32? tipo, String nome, DateTime? inicio, DateTime? final, String remedio, String generico, USUARIO usuario, Int32 idAss)
        {
            try
            {
                List<PACIENTE_PRESCRICAO> objeto = new List<PACIENTE_PRESCRICAO>();
                Int32 volta = 0;

                // Processa filtro
                if (tipo > 0 || nome != null || inicio != null || final != null)
                {
                    objeto = _baseService.ExecuteFilterPrescricao(tipo, nome, inicio, final, remedio, generico, idAss);
                    if (objeto.Count == 0)
                    {
                        volta = 1;
                    }
                }               

                // Processa filtro complementar
                List<PACIENTE_PRESCRICAO> prescricoes = _baseService.GetAllPrescricao(usuario.ASSI_CD_ID).Where(p => p.USUA_CD_ID == usuario.USUA_CD_ID).ToList();
                List<PACIENTE_PRESCRICAO_ITEM> itens = new List<PACIENTE_PRESCRICAO_ITEM>();
                if (remedio != null)
                {
                    itens = prescricoes.Where(p => p.PACIENTE_PRESCRICAO_ITEM != null).SelectMany(p => p.PACIENTE_PRESCRICAO_ITEM).ToList();
                    itens = itens.Where(p => p.PAPI_NM_REMEDIO.ToUpper().Contains(remedio.ToUpper())).ToList();
                    volta = itens.Count > 0 ? 0 : 1;
                    foreach (PACIENTE_PRESCRICAO_ITEM item in itens)
                    {
                        objeto.Add(item.PACIENTE_PRESCRICAO);
                    }
                }
                if (generico != null)
                {
                    itens = prescricoes.Where(p => p.PACIENTE_PRESCRICAO_ITEM != null).SelectMany(p => p.PACIENTE_PRESCRICAO_ITEM).Where(p => p.PAPI_NM_GENERICO.ToUpper().Contains(generico.ToUpper())).ToList();
                    volta = itens.Count > 0 ? 0 : 1;
                    foreach (PACIENTE_PRESCRICAO_ITEM item in itens)
                    {
                        objeto.Add(item.PACIENTE_PRESCRICAO);
                    }
                }

                // Elimina duplicidades
                if (volta == 0)
                {
                    objeto = objeto.Distinct().ToList();
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

        public Tuple<Int32, List<PACIENTE_PRESCRICAO_ITEM>, Boolean> ExecuteFilterTuplePrescricaoItem(Int32? forma, String nome, DateTime? inicio, DateTime? final, String remedio, String generico, Int32 idAss)
        {
            try
            {
                List<PACIENTE_PRESCRICAO_ITEM> objeto = new List<PACIENTE_PRESCRICAO_ITEM>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterPrescricaoItem(forma, nome, inicio, final, remedio, generico, idAss);
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

        public PACIENTE_ANEXO GetAnexoById(Int32 id)
        {
            PACIENTE_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public Int32 ValidateEditAnexo(PACIENTE_ANEXO item)
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

        public PACIENTE_FICHA GetFichaById(Int32 id)
        {
            PACIENTE_FICHA lista = _baseService.GetFichaById(id);
            return lista;
        }

        public Int32 ValidateEditFicha(PACIENTE_FICHA item)
        {
            try
            {
                // Persiste
                return _baseService.EditFicha(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public PACIENTE_ANOTACAO GetAnotacaoById(Int32 id)
        {
            PACIENTE_ANOTACAO lista = _baseService.GetAnotacaoById(id);
            return lista;
        }

        public Int32 ValidateEditAnotacao(PACIENTE_ANOTACAO item)
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

        public PACIENTE_ANAMNESE_ANOTACAO GetAnamneseAnotacaoById(Int32 id)
        {
            PACIENTE_ANAMNESE_ANOTACAO lista = _baseService.GetAnamneseAnotacaoById(id);
            return lista;
        }

        public Int32 ValidateCreateAnamneseAnotacao(PACIENTE_ANAMNESE_ANOTACAO item)
        {
            try
            {
                // Persiste
                return _baseService.CreateAnamneseAnotacao(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditAnamneseAnotacao(PACIENTE_ANAMNESE_ANOTACAO item)
        {
            try
            {
                // Persiste
                return _baseService.EditAnamneseAnotacao(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<TIPO_PACIENTE> GetAllTipos(Int32 idAss)
        {
            List<TIPO_PACIENTE> lista = _baseService.GetAllTipos(idAss);
            return lista;
        }

        public List<TIPO_PESSOA> GetAllTiposPessoa()
        {
            List<TIPO_PESSOA> lista = _baseService.GetAllTiposPessoa();
            return lista;
        }

        public List<SEXO> GetAllSexo()
        {
            List<SEXO> lista = _baseService.GetAllSexo();
            return lista;
        }

        public List<RACA> GetAllRaca()
        {
            List<RACA> lista = _baseService.GetAllRaca();
            return lista;
        }

        public List<GRAU_PARENTESCO> GetAllGrauParentesco()
        {
            List<GRAU_PARENTESCO> lista = _baseService.GetAllGrauParentesco();
            return lista;
        }

        public List<COR> GetAllCor()
        {
            List<COR> lista = _baseService.GetAllCor();
            return lista;
        }

        public List<TIPO_CONTROLE> GetAllTipoControle()
        {
            List<TIPO_CONTROLE> lista = _baseService.GetAllTipoControle();
            return lista;
        }

        public List<GRAU_INSTRUCAO> GetAllGrau()
        {
            List<GRAU_INSTRUCAO> lista = _baseService.GetAllGrau();
            return lista;
        }

        public List<TIPO_FORMA> GetAllFormas()
        {
            return _baseService.GetAllFormas();
        }

        public List<ESTADO_CIVIL> GetAllEstadoCivil()
        {
            List<ESTADO_CIVIL> lista = _baseService.GetAllEstadoCivil();
            return lista;
        }

        public List<CONVENIO> GetAllConvenio(Int32 idAss)
        {
            List<CONVENIO> lista = _baseService.GetAllConvenio(idAss);
            return lista;
        }

        public List<TIPO_EXAME> GetAllTipoExame(Int32 idAss)
        {
            List<TIPO_EXAME> lista = _baseService.GetAllTipoExame(idAss);
            return lista;
        }

        public List<TIPO_ATESTADO> GetAllTipoAtestado(Int32 idAss)
        {
            List<TIPO_ATESTADO> lista = _baseService.GetAllTipoAtestado(idAss);
            return lista;
        }

        public List<LABORATORIO> GetAllLaboratorios(Int32 idAss)
        {
            List<LABORATORIO> lista = _baseService.GetAllLaboratorios(idAss);
            return lista;
        }

        public List<USUARIO> GetAllUsuario(Int32 idAss)
        {
            List<USUARIO> lista = _baseService.GetAllUsuario(idAss);
            return lista;
        }

        public List<UF> GetAllUF()
        {
            List<UF> lista = _baseService.GetAllUF();
            return lista;
        }

        public UF GetUFbySigla(String sigla)
        {
            return _baseService.GetUFbySigla(sigla);
        }

        public List<LINGUA> GetAllLinguas()
        {
            List<LINGUA> lista = _baseService.GetAllLinguas();
            return lista;
        }

        public List<NACIONALIDADE> GetAllNacionalidades()
        {
            List<NACIONALIDADE> lista = _baseService.GetAllNacionalidades();
            return lista;
        }

        public List<MUNICIPIO> GetAllMunicipios()
        {
            List<MUNICIPIO> lista = _baseService.GetAllMunicipios();
            return lista;
        }

        public List<MUNICIPIO> GetMunicipioByUF(Int32 uf)
        {
            List<MUNICIPIO> lista = _baseService.GetMunicipioByUF(uf);
            return lista;
        }

        public PACIENTE_CONSULTA GetConsultaById(Int32 id)
        {
            PACIENTE_CONSULTA lista = _baseService.GetConsultaById(id);
            return lista;
        }

        public Int32 ValidateEditConsulta(PACIENTE_CONSULTA item)
        {
            try
            {
                // Persiste
                return _baseService.EditConsulta(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditConsultaConfirma(PACIENTE_CONSULTA item)
        {
            try
            {
                // Persiste
                return _baseService.EditConsultaConfirma(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateConsultaCompleta(PACIENTE_CONSULTA item, PACIENTE_ANAMNESE anamnese, PACIENTE_EXAME_FISICOS fisico)
        {
            try
            {
                item.PACO_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateConsultaCompleta(item, anamnese, fisico);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateConsulta(PACIENTE_CONSULTA item)
        {
            try
            {
                item.PACO_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateConsulta(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PACIENTE_CONSULTA> GetAllConsultas(Int32 idAss)
        {
            return _baseService.GetAllConsultas(idAss);
        }

        public PACIENTE_PRESCRICAO GetPrescricaoById(Int32 id)
        {
            PACIENTE_PRESCRICAO lista = _baseService.GetPrescricaoById(id);
            return lista;
        }

        public Int32 ValidateEditPrescricao(PACIENTE_PRESCRICAO item)
        {
            try
            {
                // Persiste
                return _baseService.EditPrescricao(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreatePrescricao(PACIENTE_PRESCRICAO item)
        {
            try
            {
                item.PAPR_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreatePrescricao(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PACIENTE_PRESCRICAO> GetAllPrescricao(Int32 idAss)
        {
            return _baseService.GetAllPrescricao(idAss);
        }

        public List<PACIENTE_PRESCRICAO> GetAllPrescricaoGeral()
        {
            return _baseService.GetAllPrescricaoGeral();
        }

        public PACIENTE_ANAMNESE GetAnamneseById(Int32 id)
        {
            PACIENTE_ANAMNESE lista = _baseService.GetAnamneseById(id);
            return lista;
        }

        public Int32 ValidateEditAnamnese(PACIENTE_ANAMNESE item)
        {
            try
            {
                // Persiste
                return _baseService.EditAnamnese(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditAnamnesePrevia(PACIENTE_ANAMNESE item)
        {
            try
            {
                // Persiste
                return _baseService.EditAnamnese(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditAnamneseConfirma(PACIENTE_ANAMNESE item)
        {
            try
            {
                // Persiste
                return _baseService.EditAnamneseConfirma(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateAnamnese(PACIENTE_ANAMNESE item)
        {
            try
            {
                item.PAAM_IN_ATIVO = 1;
                item.PACIENTE_CONSULTA = null;

                // Persiste
                Int32 volta = _baseService.CreateAnamnese(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PACIENTE_ANAMNESE> GetAllAnamnese(Int32 idAss)
        {
            return _baseService.GetAllAnamnese(idAss);
        }

        public PACIENTE_EXAMES GetExameById(Int32 id)
        {
            PACIENTE_EXAMES lista = _baseService.GetExameById(id);
            return lista;
        }

        public Int32 ValidateEditExame(PACIENTE_EXAMES item)
        {
            try
            {
                // Persiste
                return _baseService.EditExame(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Int32> ValidateEditExameAsync(PACIENTE_EXAMES item)
        {
            try
            {
                // Persiste
                return await _baseService.EditExameAsync(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateExame(PACIENTE_EXAMES item)
        {
            try
            {
                item.PAEX_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateExame(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PACIENTE_EXAMES> GetAllExame(Int32 idAss)
        {
            return _baseService.GetAllExame(idAss);
        }
        public List<PACIENTE_PRESCRICAO_ITEM> GetAllPrescricaoItem(Int32 idAss)
        {
            return _baseService.GetAllPrescricaoItem(idAss);
        }

        public PACIENTE_EXAME_FISICOS GetExameFisicoById(Int32 id)
        {
            PACIENTE_EXAME_FISICOS lista = _baseService.GetExameFisicoById(id);
            return lista;
        }

        public Int32 ValidateEditExameFisico(PACIENTE_EXAME_FISICOS item)
        {
            try
            {
                // Persiste
                return _baseService.EditExameFisico(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateExameFisico(PACIENTE_EXAME_FISICOS item)
        {
            try
            {
                item.PAEF_IN_ATIVO = 1;
                item.PACIENTE_CONSULTA = null;

                // Persiste
                Int32 volta = _baseService.CreateExameFisico(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PACIENTE_EXAME_FISICOS> GetAllExameFisico(Int32 idAss)
        {
            return _baseService.GetAllExameFisico(idAss);
        }

        public Int32 ValidateCreate(PACIENTE item, USUARIO usuario)
        {
            try
            {
                // Verifica Existencia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.PACI_IN_ATIVO = 1;

                // Acerta letras
                if (item.PACI_NM_NOME != null)
                {
                    item.PACI_NM_NOME = CommonHelpers.ToPascalCase(item.PACI_NM_NOME);
                }
                if (item.PACI_NM_SOCIAL != null)
                {
                    item.PACI_NM_SOCIAL = CommonHelpers.ToPascalCase(item.PACI_NM_SOCIAL);
                }
                if (item.PACI_NM_ENDERECO != null)
                {
                    item.PACI_NM_ENDERECO = CommonHelpers.ToPascalCase(item.PACI_NM_ENDERECO);
                }
                if (item.PACI_NM_BAIRRO != null)
                {
                    item.PACI_NM_BAIRRO = CommonHelpers.ToPascalCase(item.PACI_NM_BAIRRO);
                }
                if (item.PACI_NM_CIDADE != null)
                {
                    item.PACI_NM_CIDADE = CommonHelpers.ToPascalCase(item.PACI_NM_CIDADE);
                }
                if (item.PACI_NM_PAI != null)
                {
                    item.PACI_NM_PAI = CommonHelpers.ToPascalCase(item.PACI_NM_PAI);
                }
                if (item.PACI_NM_MAE != null)
                {
                    item.PACI_NM_MAE = CommonHelpers.ToPascalCase(item.PACI_NM_MAE);
                }

                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };


                // Monta Log
                DTO_Paciente dto = MontarPacienteDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Paciente - Inclusão",
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

        public Int32 ValidateCreate(PACIENTE item, USUARIO usuario, String linha)
        {
            try
            {
                // Verifica Existencia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.PACI_IN_ATIVO = 1;

                // Acerta letras
                if (item.PACI_NM_NOME != null)
                {
                    item.PACI_NM_NOME = CommonHelpers.ToPascalCase(item.PACI_NM_NOME);
                }
                if (item.PACI_NM_SOCIAL != null)
                {
                    item.PACI_NM_SOCIAL = CommonHelpers.ToPascalCase(item.PACI_NM_SOCIAL);
                }
                if (item.PACI_NM_ENDERECO != null)
                {
                    item.PACI_NM_ENDERECO = CommonHelpers.ToPascalCase(item.PACI_NM_ENDERECO);
                }
                if (item.PACI_NM_BAIRRO != null)
                {
                    item.PACI_NM_BAIRRO = CommonHelpers.ToPascalCase(item.PACI_NM_BAIRRO);
                }
                if (item.PACI_NM_CIDADE != null)
                {
                    item.PACI_NM_CIDADE = CommonHelpers.ToPascalCase(item.PACI_NM_CIDADE);
                }
                if (item.PACI_NM_PAI != null)
                {
                    item.PACI_NM_PAI = CommonHelpers.ToPascalCase(item.PACI_NM_PAI);
                }
                if (item.PACI_NM_MAE != null)
                {
                    item.PACI_NM_MAE = CommonHelpers.ToPascalCase(item.PACI_NM_MAE);
                }

                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };


                // Monta Log
                DTO_Paciente dto = MontarPacienteDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Paciente - Inclusão",
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

        public Int32 ValidateEdit(PACIENTE item, PACIENTE itemAntes, USUARIO usuario)
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
                DTO_Paciente dto = MontarPacienteDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                DTO_Paciente dtoAntes = MontarPacienteDTOObj(itemAntes);
                String jsonAntes = JsonConvert.SerializeObject(dtoAntes, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Paciente - Edição",
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

        public Int32 ValidateEditArea(PACIENTE item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.EditArea(item);
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(PACIENTE item, PACIENTE itemAntes)
        {
            try
            {
                item.UF = null;
                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(PACIENTE item, USUARIO usuario, String linha)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EdtPACI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = linha,
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

        public async Task<Int32> ValidateEditAsync(PACIENTE item, PACIENTE itemAntes)
        {
            try
            {
                item.UF = null;
                // Persiste
                return await _baseService.EditAsync(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(PACIENTE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.PACIENTE_CONSULTA != null)
                {
                    List<PACIENTE_CONSULTA> consultas = item.PACIENTE_CONSULTA.Where(p => p.PACO_DT_CONSULTA.Date >= DateTime.Today.Date).ToList();
                    if (consultas.Count > 0)
                    {
                        return 1;
                    }            
                }

                // Acerta campos
                item.PACI_IN_ATIVO = 0;

                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Monta Log
                DTO_Paciente dto = MontarPacienteDTO(item.PACI__CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "Paciente - Exclusão",
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

        public Int32 ValidateDelete(PACIENTE item, USUARIO usuario, String linha)
        {
            try
            {
                // Verifica integridade referencial
                if (item.PACIENTE_CONSULTA != null)
                {
                    List<PACIENTE_CONSULTA> consultas = item.PACIENTE_CONSULTA.Where(p => p.PACO_DT_CONSULTA.Date >= DateTime.Today.Date).ToList();
                    if (consultas.Count > 0)
                    {
                        return 1;
                    }
                }

                // Acerta campos
                item.PACI_IN_ATIVO = 0;

                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Monta Log
                DTO_Paciente dto = MontarPacienteDTO(item.PACI__CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "Paciente - Exclusão",
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

        public Int32 ValidateReativar(PACIENTE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.PACI_IN_ATIVO = 1;

                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Monta Log
                DTO_Paciente dto = MontarPacienteDTO(item.PACI__CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "Paciente - Reativação",
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

        public PACIENTE_SOLICITACAO GetSolicitacaoById(Int32 id)
        {
            PACIENTE_SOLICITACAO lista = _baseService.GetSolicitacaoById(id);
            return lista;
        }

        public Int32 ValidateEditSolicitacao(PACIENTE_SOLICITACAO item)
        {
            try
            {
                // Persiste
                return _baseService.EditSolicitacao(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateSolicitacao(PACIENTE_SOLICITACAO item)
        {
            try
            {
                item.PASO_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateSolicitacao(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PACIENTE_SOLICITACAO> GetAllSolicitacao(Int32 idAss)
        {
            return _baseService.GetAllSolicitacao(idAss);
        }

        public List<PACIENTE_SOLICITACAO> GetAllSolicitacaoGeral()
        {
            return _baseService.GetAllSolicitacaoGeral();
        }

        public PACIENTE_ATESTADO GetAtestadoById(Int32 id)
        {
            PACIENTE_ATESTADO lista = _baseService.GetAtestadoById(id);
            return lista;
        }

        public Int32 ValidateEditAtestado(PACIENTE_ATESTADO item)
        {
            try
            {
                // Persiste
                return _baseService.EditAtestado(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateAtestado(PACIENTE_ATESTADO item)
        {
            try
            {
                item.PAAT_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateAtestado(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PACIENTE_CONTATO> GetAllContato(Int32 idAss)
        {
            return _baseService.GetAllContato(idAss);
        }

        public PACIENTE_CONTATO GetContatoById(Int32 id)
        {
            PACIENTE_CONTATO lista = _baseService.GetContatoById(id);
            return lista;
        }

        public Int32 ValidateEditContato(PACIENTE_CONTATO item)
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

        public Int32 ValidateCreateContato(PACIENTE_CONTATO item)
        {
            try
            {
                item.PACO_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateContato(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PACIENTE_ATESTADO> GetAllAtestado(Int32 idAss)
        {
            return _baseService.GetAllAtestado(idAss);
        }

        public List<PACIENTE_ATESTADO> GetAllAtestadoGeral()
        {
            return _baseService.GetAllAtestadoGeral();
        }

        public GRUPO_PACIENTE GetGrupoById(Int32 id)
        {
            GRUPO_PACIENTE lista = _baseService.GetGrupoById(id);
            return lista;
        }

        public Int32 ValidateCreateGrupo(GRUPO_PACIENTE item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateGrupo(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditGrupo(GRUPO_PACIENTE item)
        {
            try
            {
                // Persiste
                return _baseService.EditGrupo(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public PACIENTE_EXAME_ANEXO GetExameAnexoById(Int32 id)
        {
            PACIENTE_EXAME_ANEXO lista = _baseService.GetExameAnexoById(id);
            return lista;
        }

        public Int32 EditExameAnexo(PACIENTE_EXAME_ANEXO item)
        {
            try
            {
                // Persiste
                return _baseService.EditExameAnexo(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public PACIENTE_EXAME_ANOTACAO GetExameAnotacaoById(Int32 id)
        {
            PACIENTE_EXAME_ANOTACAO lista = _baseService.GetExameAnotacaoById(id);
            return lista;
        }

        public Int32 EditExameAnotacao(PACIENTE_EXAME_ANOTACAO item)
        {
            try
            {
                // Persiste
                return _baseService.EditExameAnotacao(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public PACIENTE_PRESCRICAO_ITEM GetPrescricaoItemById(Int32 id)
        {
            PACIENTE_PRESCRICAO_ITEM lista = _baseService.GetPrescricaoItemById(id);
            return lista;
        }

        public Int32 ValidateEditPrescricaoItem(PACIENTE_PRESCRICAO_ITEM item)
        {
            try
            {
                // Persiste
                return _baseService.EditPrescricaoItem(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreatePrescricaoItem(PACIENTE_PRESCRICAO_ITEM item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreatePrescricaoItem(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PACIENTE_HISTORICO> GetAllHistorico(Int32 idAss)
        {
            return _baseService.GetAllHistorico(idAss);
        }

        public PACIENTE_HISTORICO GetHistoricoById(Int32 id)
        {
            PACIENTE_HISTORICO lista = _baseService.GetHistoricoById(id);
            return lista;
        }

        public Int32 ValidateCreateHistorico(PACIENTE_HISTORICO item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateHistorico(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Tuple<Int32, List<PACIENTE_HISTORICO>, Boolean> ExecuteFilterTupleHistorico(Int32? tipo, String operacao, DateTime? inicio, DateTime? final, String descricao, Int32 idAss)
        {
            try
            {
                List<PACIENTE_HISTORICO> objeto = new List<PACIENTE_HISTORICO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterHistorico(tipo, operacao, inicio, final, descricao, idAss);
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

        public Tuple<Int32, List<PACIENTE_HISTORICO>, Boolean> ExecuteFilterTupleHistoricoGeral(Int32? tipo, String operacao, DateTime? inicio, DateTime? final, Int32? paciente, Int32 idAss)
        {
            try
            {
                List<PACIENTE_HISTORICO> objeto = new List<PACIENTE_HISTORICO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterHistoricoGeral(tipo, operacao, inicio, final, paciente, idAss);
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

        public Int32 ValidateCreateLogin(PACIENTE_LOGIN log)
        {
            // Persiste
            Int32 volta = _baseService.CreateLogin(log);
            return volta;
        }

        public Int32 ValidateLoginArea(String cpf, String senha, out PACIENTE usuario)
        {
            try
            {
                usuario = new PACIENTE();
                Int32 idade = 0;

                // Checa preenchimento do login
                if (String.IsNullOrEmpty(cpf))
                {
                    return 10;
                }

                // Checa senha
                if (String.IsNullOrEmpty(senha))
                {
                    return 9;
                }

                // Checa login
                usuario = _baseService.GetItemByCPF(cpf);
                if (usuario == null)
                {
                    usuario = new PACIENTE();
                    return 2;
                }

                // Verifica se está ativo
                if (usuario.PACI_IN_ATIVO != 1)
                {
                    return 3;
                }

                // Verifica credenciais
                Boolean retorno = _baseService.VerificarCredenciais(senha, usuario);
                if (!retorno)
                {
                    return 7;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Int32> GerarNovaSenha(PACIENTE usu)
        {
            // Critica preenchimento
            if (usu.PACI_NR_CPF == null)
            {
                return 5;
            }

            // Verifica validade de CPF
            if (!CrossCutting.ValidarNumerosDocumentos.IsCFPValid(usu.PACI_NR_CPF))
            {
                return 8;
            }

            // Recupera paciente
            PACIENTE usuario = _baseService.GetItemByCPF(usu.PACI_NR_CPF);
            if (usuario == null)
            {
                return 2;
            }

            // Verifica se paciente está ativo
            if (usuario.PACI_IN_ATIVO == 0)
            {
                return 3;
            }

            // Gera nova senha
            String senha = Cryptography.GenerateRandomPassword(6);
            usuario.PACI_NM_SENHA = senha;

            // Atauliza objeto
            usuario.PACI_DT_ALTERACAO = DateTime.Now;

            // Atualiza usuario
            Int32 volta = _baseService.Edit(usuario);

            // Recupera template e-mail
            String header = _baseService.GetTemplate("NEWPWDAP").TEMP_TX_CABECALHO;
            String body = _baseService.GetTemplate("NEWPWDAP").TEMP_TX_CORPO;
            String data = _baseService.GetTemplate("NEWPWDAP").TEMP_TX_DADOS;

            // Prepara dados do e-mail  
            header = header.Replace("{nome}", usuario.PACI_NM_NOME);
            data = data.Replace("{Data}", DateTime.Now.ToString());
            data = data.Replace("{Senha}", senha);

            // Concatena
            String emailBody = header + body + data;

            // Prepara e-mail e enviar
            CONFIGURACAO conf = _baseService.CarregaConfiguracao(usuario.ASSI_CD_ID);
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
            EmailAzure mensagem = new EmailAzure();
            mensagem.ASSUNTO = "Geração de Nova Senha";
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_TO_DESTINO = usuario.PACI_NM_EMAIL;
            mensagem.NOME_EMISSOR_AZURE = conf.CONF_NM_EMISSOR_AZURE;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = "WebDoctor";
            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
            mensagem.IS_HTML = true;
            mensagem.NETWORK_CREDENTIAL = net;
            mensagem.ConnectionString = conf.CONF_CS_CONNECTION_STRING_AZURE;
            String status = "Succeeded";
            String iD = "xyz";

            // Envia mensagem
            try
            {
                await CrossCutting.CommunicationAzurePackage.SendMailAsync(mensagem, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 0;
        }

        public async Task<Int32> ValidateChangePassword(PACIENTE usu)
        {
            try
            {
                // Sanitização
                usu.PACI_NM_NOVA_SENHA = CrossCutting.UtilitariosGeral.CleanStringSenha(usu.PACI_NM_NOVA_SENHA);
                usu.PACI_NM_SENHA_CONFIRMA = CrossCutting.UtilitariosGeral.CleanStringSenha(usu.PACI_NM_SENHA_CONFIRMA);
                usu.PACI_NR_CPF = CrossCutting.UtilitariosGeral.CleanStringDocto(usu.PACI_NR_CPF);

                // Verifica validade de CPF e CNPJ e recupera assinante
                if (!CrossCutting.ValidarNumerosDocumentos.IsCFPValid(usu.PACI_NR_CPF))
                {
                    return 13;
                }

                // Recupera paciente
                PACIENTE usuario = _baseService.GetItemByCPF(usu.PACI_NR_CPF);
                if (usuario == null)
                {
                    return 2;
                }

                // Verifica se paciente está ativo
                if (usuario.PACI_IN_ATIVO == 0)
                {
                    return 3;
                }

                // Checa preenchimento
                CONFIGURACAO conf = _baseService.CarregaConfiguracao(usuario.ASSI_CD_ID);
                if (String.IsNullOrEmpty(usuario.PACI_NR_CPF))
                {
                    return 5;
                }
                if (String.IsNullOrEmpty(usu.PACI_NM_NOVA_SENHA))
                {
                    return 6;
                }
                if (String.IsNullOrEmpty(usu.PACI_NM_SENHA_CONFIRMA))
                {
                    return 7;
                }

                // Verifica se senha igual a anterior
                if (usu.PACI_NM_NOVA_SENHA == usuario.PACI_NM_SENHA)
                {
                    return 8;
                }

                // Verifica se senha foi confirmada
                if (usu.PACI_NM_NOVA_SENHA != usu.PACI_NM_SENHA_CONFIRMA)
                {
                    return 12;
                }

                // Verifica força da senha
                String senha = usu.PACI_NM_NOVA_SENHA;
                if (senha.Length < conf.CONF_NR_TAMANHO_SENHA)
                {
                    return 9;
                }
                if (!senha.Any(char.IsUpper) || !senha.Any(char.IsLower) && !senha.Any(char.IsDigit))
                {
                    return 10;
                }
                if (!senha.Any(char.IsLetterOrDigit))
                {
                    return 11;
                }

                // Grava senha
                usuario.PACI_NM_SENHA = senha;
                usuario.PACI_DT_ALTERACAO = DateTime.Now;
                Int32 volta = _baseService.Edit(usuario);

                // Recupera template e-mail
                String header = _baseService.GetTemplate("ALTSENHAAP").TEMP_TX_CABECALHO;
                String body = _baseService.GetTemplate("ALTSENHAAP").TEMP_TX_CORPO;
                String data = _baseService.GetTemplate("ALTSENHAAP").TEMP_TX_DADOS;

                // Prepara dados do e-mail  
                header = header.Replace("{nome}", usuario.PACI_NM_NOME);
                data = data.Replace("{Data}", DateTime.Now.ToString());
                data = data.Replace("{Senha}", senha);

                // Concatena
                String emailBody = header + body + data;

                // Prepara e-mail e enviar
                NetworkCredential net = new NetworkCredential(conf.CONF_NM_SENDGRID_LOGIN, conf.CONF_NM_SENDGRID_PWD);
                EmailAzure mensagem = new EmailAzure();
                mensagem.ASSUNTO = "Alteração de Senha";
                mensagem.CORPO = emailBody;
                mensagem.DEFAULT_CREDENTIALS = false;
                mensagem.EMAIL_TO_DESTINO = usuario.PACI_NM_EMAIL;
                mensagem.NOME_EMISSOR_AZURE = conf.CONF_NM_EMISSOR_AZURE;
                mensagem.ENABLE_SSL = true;
                mensagem.NOME_EMISSOR = "WebDoctor";
                mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                mensagem.SENHA_EMISSOR = conf.CONF_NM_SENDGRID_PWD;
                mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                mensagem.IS_HTML = true;
                mensagem.NETWORK_CREDENTIAL = net;
                mensagem.ConnectionString = conf.CONF_CS_CONNECTION_STRING_AZURE;
                String status = "Succeeded";
                String iD = "xyz";

                // Envia mensagem
                try
                {
                    await CrossCutting.CommunicationAzurePackage.SendMailAsync(mensagem, null);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PACIENTE_CONSULTA> GetConsultasByCPF(String cpf)
        {
            return _baseService.GetConsultasByCPF(cpf);
        }

        public List<PACIENTE_ATESTADO> GetAtestadosByCPF(String cpf)
        {
            return _baseService.GetAtestadosByCPF(cpf);
        }

        public List<PACIENTE_EXAMES> GetExamesByCPF(String cpf)
        {
            return _baseService.GetExamesByCPF(cpf);
        }

        public List<PACIENTE_SOLICITACAO> GetSolicitacaoByCPF(String cpf)
        {
            return _baseService.GetSolicitacaoByCPF(cpf);
        }

        public List<PACIENTE_PRESCRICAO> GetPrescricaoByCPF(String cpf)
        {
            return _baseService.GetPrescricaoByCPF(cpf);
        }

        public PACIENTE_DADOS_EXAME_FISICO GetDadosExameFisicoById(Int32 id)
        {
            PACIENTE_DADOS_EXAME_FISICO lista = _baseService.GetDadosExameFisicoById(id);
            return lista;
        }

        public Int32 ValidateEditDadosExameFisico(PACIENTE_DADOS_EXAME_FISICO item)
        {
            try
            {
                // Persiste
                return _baseService.EditDadosExameFisico(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateDadosExameFisico(PACIENTE_DADOS_EXAME_FISICO item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateDadosExameFisico(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PACIENTE_DADOS_EXAME_FISICO> GetAllDadosExameFisico(Int32 idAss)
        {
            return _baseService.GetAllDadosExameFisico(idAss);
        }

        public Int32 ValidateEditBerlim(QUESTIONARIO_BERLIM item)
        {
            try
            {
                // Persiste
                return _baseService.EditQuestionarioBerlim(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditEpworth(QUESTIONARIO_EPWORTH item)
        {
            try
            {
                // Persiste
                return _baseService.EditQuestionarioEpworth(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditBang(QUESTIONARIO_STOPBANG item)
        {
            try
            {
                // Persiste
                return _baseService.EditQuestionarioBang(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DTO_Paciente MontarPacienteDTO(Int32 mediId)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = context.PACIENTE
                    .Where(l => l.PACI__CD_ID == mediId)
                    .Select(l => new DTO_Paciente
                    {
                        ASSI_CD_ID = l.ASSI_CD_ID,
                        GRAU_CD_ID = l.GRAU_CD_ID,
                        NACI_CD_ID = l.NACI_CD_ID,
                        CONV_CD_ID = l.CONV_CD_ID,
                        MUNI_CD_ID = l.MUNI_CD_ID,
                        MUNI_SG_UF = l.MUNI_SG_UF,
                        PACI_DT_CONSULTA = l.PACI_DT_CONSULTA,
                        PACI_DT_NASCIMENTO = l.PACI_DT_NASCIMENTO,
                        PACI_DT_PREVISAO_RETORNO = l.PACI_DT_PREVISAO_RETORNO,
                        PACI_DT_ULTIMO_ENVIO = l.PACI_DT_ULTIMO_ENVIO,
                        PACI_IN_ATIVO = l.PACI_IN_ATIVO,
                        PACI_IN_COMPLETADO = l.PACI_IN_COMPLETADO,
                        PACI_IN_FICHAS = l.PACI_IN_FICHAS,
                        PACI_IN_FIM_ENVIO = l.PACI_IN_FIM_ENVIO,
                        PACI_IN_HUMANO = l.PACI_IN_HUMANO,
                        PACI_IN_MENOR = l.PACI_IN_MENOR,
                        PACI_IN_MENSAGEM_ATRASO = l.PACI_IN_MENSAGEM_ATRASO,
                        PACI_IN_NUMERO_ENVIO = l.PACI_IN_NUMERO_ENVIO,
                        PACI_IN_PADRAO_ANAMNESE = l.PACI_IN_PADRAO_ANAMNESE,
                        PACI_IN_PADRAO_CONTINUA = l.PACI_IN_PADRAO_CONTINUA,
                        PACI_NM_BAIRRO = l.PACI_NM_BAIRRO,
                        PACI_NM_CIDADE = l.PACI_NM_CIDADE,
                        PACI_NM_EMAIL = l.PACI_NM_EMAIL,
                        PACI_NM_ENDERECO = l.PACI_NM_ENDERECO,
                        PACI_NM_INDICACAO = l.PACI_NM_INDICACAO,
                        PACI_NM_LOGIN = l.PACI_NM_LOGIN,
                        PACI_NM_MAE = l.PACI_NM_MAE,
                        PACI_NM_NACIONALIDADE = l.PACI_NM_NACIONALIDADE,
                        PACI_NM_NATURALIDADE = l.PACI_NM_NATURALIDADE,
                        PACI_NM_NOME = l.PACI_NM_NOME,
                        PACI_NM_NOVA_SENHA = l.PACI_NM_NOVA_SENHA,
                        PACI_NM_PAI = l.PACI_NM_PAI,
                        PACI_NM_PROFISSAO = l.PACI_NM_PROFISSAO,
                        PACI_NM_RESPONSAVEL = l.PACI_NM_RESPONSAVEL,
                        PACI_NM_SENHA = l.PACI_NM_SENHA,
                        PACI_NM_SENHA_CONFIRMA = l.PACI_NM_SENHA_CONFIRMA,
                        PACI_NM_SOCIAL = l.PACI_NM_SOCIAL,
                        PACI_NR_CELULAR = l.PACI_NR_CELULAR,
                        PACI_NR_CEP = l.PACI_NR_CEP,
                        PACI_NR_COMPLEMENTO = l.PACI_NR_COMPLEMENTO,
                        PACI_NR_CPF = l.PACI_NR_CPF,
                        PACI_NR_IDADE = l.PACI_NR_IDADE,
                        PACI_NR_MATRICULA = l.PACI_NR_MATRICULA,
                        PACI_NR_NUMERO = l.PACI_NR_NUMERO,
                        PACI_NR_RG = l.PACI_NR_RG,
                        PACI_NR_TELEFONE = l.PACI_NR_TELEFONE,
                        PACI_SG_NATURALIDADE_UF = l.PACI_SG_NATURALIDADE_UF,
                        COR1_CD_ID = l.COR1_CD_ID,
                        ESCI_CD_ID = l.ESCI_CD_ID,
                        PACI_AQ_FOTO = l.PACI_AQ_FOTO,
                        PACI_DT_ACESSO = l.PACI_DT_ACESSO,
                        PACI_DT_ALTERACAO = l.PACI_DT_ALTERACAO,
                        PACI_DT_CADASTRO = l.PACI_DT_CADASTRO,
                        PACI_DT_PRECO = l.PACI_DT_PRECO,
                        PACI_DT_ULTIMO_ACESSO = l.PACI_DT_ULTIMO_ACESSO,
                        PACI_GU_GUID = l.PACI_GU_GUID,
                        PACI_TX_OBSERVACOES = l.PACI_TX_OBSERVACOES,
                        PACI__CD_ID = l.PACI__CD_ID,
                        SEXO_CD_ID = l.SEXO_CD_ID,
                        TIPA_CD_ID = l.TIPA_CD_ID,
                        UF_CD_ID = l.UF_CD_ID,
                        USUA_CD_ID = l.USUA_CD_ID,
                        VACO_CD_ID = l.VACO_CD_ID,
                    })
                    .FirstOrDefault();
                return mediDTO;
            }
        }

        public DTO_Paciente MontarPacienteDTOObj(PACIENTE l)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_Paciente()
                {
                    ASSI_CD_ID = l.ASSI_CD_ID,
                    GRAU_CD_ID = l.GRAU_CD_ID,
                    NACI_CD_ID = l.NACI_CD_ID,
                    CONV_CD_ID = l.CONV_CD_ID,
                    MUNI_CD_ID = l.MUNI_CD_ID,
                    MUNI_SG_UF = l.MUNI_SG_UF,
                    PACI_DT_CONSULTA = l.PACI_DT_CONSULTA,
                    PACI_DT_NASCIMENTO = l.PACI_DT_NASCIMENTO,
                    PACI_DT_PREVISAO_RETORNO = l.PACI_DT_PREVISAO_RETORNO,
                    PACI_DT_ULTIMO_ENVIO = l.PACI_DT_ULTIMO_ENVIO,
                    PACI_IN_ATIVO = l.PACI_IN_ATIVO,
                    PACI_IN_COMPLETADO = l.PACI_IN_COMPLETADO,
                    PACI_IN_FICHAS = l.PACI_IN_FICHAS,
                    PACI_IN_FIM_ENVIO = l.PACI_IN_FIM_ENVIO,
                    PACI_IN_HUMANO = l.PACI_IN_HUMANO,
                    PACI_IN_MENOR = l.PACI_IN_MENOR,
                    PACI_IN_MENSAGEM_ATRASO = l.PACI_IN_MENSAGEM_ATRASO,
                    PACI_IN_NUMERO_ENVIO = l.PACI_IN_NUMERO_ENVIO,
                    PACI_IN_PADRAO_ANAMNESE = l.PACI_IN_PADRAO_ANAMNESE,
                    PACI_IN_PADRAO_CONTINUA = l.PACI_IN_PADRAO_CONTINUA,
                    PACI_NM_BAIRRO = l.PACI_NM_BAIRRO,
                    PACI_NM_CIDADE = l.PACI_NM_CIDADE,
                    PACI_NM_EMAIL = l.PACI_NM_EMAIL,
                    PACI_NM_ENDERECO = l.PACI_NM_ENDERECO,
                    PACI_NM_INDICACAO = l.PACI_NM_INDICACAO,
                    PACI_NM_LOGIN = l.PACI_NM_LOGIN,
                    PACI_NM_MAE = l.PACI_NM_MAE,
                    PACI_NM_NACIONALIDADE = l.PACI_NM_NACIONALIDADE,
                    PACI_NM_NATURALIDADE = l.PACI_NM_NATURALIDADE,
                    PACI_NM_NOME = l.PACI_NM_NOME,
                    PACI_NM_NOVA_SENHA = l.PACI_NM_NOVA_SENHA,
                    PACI_NM_PAI = l.PACI_NM_PAI,
                    PACI_NM_PROFISSAO = l.PACI_NM_PROFISSAO,
                    PACI_NM_RESPONSAVEL = l.PACI_NM_RESPONSAVEL,
                    PACI_NM_SENHA = l.PACI_NM_SENHA,
                    PACI_NM_SENHA_CONFIRMA = l.PACI_NM_SENHA_CONFIRMA,
                    PACI_NM_SOCIAL = l.PACI_NM_SOCIAL,
                    PACI_NR_CELULAR = l.PACI_NR_CELULAR,
                    PACI_NR_CEP = l.PACI_NR_CEP,
                    PACI_NR_COMPLEMENTO = l.PACI_NR_COMPLEMENTO,
                    PACI_NR_CPF = l.PACI_NR_CPF,
                    PACI_NR_IDADE = l.PACI_NR_IDADE,
                    PACI_NR_MATRICULA = l.PACI_NR_MATRICULA,
                    PACI_NR_NUMERO = l.PACI_NR_NUMERO,
                    PACI_NR_RG = l.PACI_NR_RG,
                    PACI_NR_TELEFONE = l.PACI_NR_TELEFONE,
                    PACI_SG_NATURALIDADE_UF = l.PACI_SG_NATURALIDADE_UF,
                    COR1_CD_ID = l.COR1_CD_ID,
                    ESCI_CD_ID = l.ESCI_CD_ID,
                    PACI_AQ_FOTO = l.PACI_AQ_FOTO,
                    PACI_DT_ACESSO = l.PACI_DT_ACESSO,
                    PACI_DT_ALTERACAO = l.PACI_DT_ALTERACAO,
                    PACI_DT_CADASTRO = l.PACI_DT_CADASTRO,
                    PACI_DT_PRECO = l.PACI_DT_PRECO,
                    PACI_DT_ULTIMO_ACESSO = l.PACI_DT_ULTIMO_ACESSO,
                    PACI_GU_GUID = l.PACI_GU_GUID,
                    PACI_TX_OBSERVACOES = l.PACI_TX_OBSERVACOES,
                    PACI__CD_ID = l.PACI__CD_ID,
                    SEXO_CD_ID = l.SEXO_CD_ID,
                    TIPA_CD_ID = l.TIPA_CD_ID,
                    UF_CD_ID = l.UF_CD_ID,
                    USUA_CD_ID = l.USUA_CD_ID,
                    VACO_CD_ID = l.VACO_CD_ID,
                };
                return mediDTO;
            }
        }

        public RESPOSTA_CONSULTA GetRespostaById(Int32 id)
        {
            RESPOSTA_CONSULTA lista = _baseService.GetRespostaById(id);
            return lista;
        }

        public Int32 ValidateEditResposta(RESPOSTA_CONSULTA item)
        {
            try
            {
                // Persiste
                return _baseService.EditResposta(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<RESPOSTA_CONSULTA> GetAllResposta(Int32 idAss)
        {
            return _baseService.GetAllResposta(idAss);
        }

        public List<PACIENTE_CONSULTA_MATERIAL> GetAllConsultaMaterial(Int32 idAss)
        {
            return _baseService.GetAllConsultaMaterial(idAss);
        }

        public PACIENTE_CONSULTA_MATERIAL GetConsultaMaterialById(Int32 id)
        {
            PACIENTE_CONSULTA_MATERIAL lista = _baseService.GetConsultaMaterialById(id);
            return lista;
        }

        public Int32 ValidadeEditConsultaMaterial(PACIENTE_CONSULTA_MATERIAL item)
        {
            try
            {
                // Persiste
                return _baseService.EditConsultaMaterial(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateConsultaMaterial(PACIENTE_CONSULTA_MATERIAL item)
        {
            try
            {
                item.PCMA_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateConsultaMaterial(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PACIENTE_VACINA> GetAllVacina(Int32 idAss)
        {
            return _baseService.GetAllVacina(idAss);
        }

        public PACIENTE_VACINA GetVacinaById(Int32 id)
        {
            PACIENTE_VACINA lista = _baseService.GetVacinaById(id);
            return lista;
        }

        public Int32 ValidateEditVacina(PACIENTE_VACINA item)
        {
            try
            {
                // Persiste
                return _baseService.EditVacina(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateVacina(PACIENTE_VACINA item)
        {
            try
            {
                item.PAVI_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateVacina(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<VACINA> GetAllVacinas(Int32 idAss)
        {
            return _baseService.GetAllVacinas(idAss);
        }

        public VACINA GetVacinasById(Int32 id)
        {
            VACINA lista = _baseService.GetVacinasById(id);
            return lista;
        }

        public Int32 ValidateCreateAnexoImagem(PACIENTE_EXAME_ANEXO_IMAGEM item)
        {
            try
            {
                item.PAIM_IN_ATIVO = 1;
                item.PAIM_DT_CRIACAO = DateTime.Today.Date;

                // Persiste
                Int32 volta = _baseService.CreateAnexoImagem(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PACIENTE_EXAME_ANEXO_IMAGEM> GetPontosById(Int32 id)
        {
            return _baseService.GetPontosById(id);
        }

    }
}
