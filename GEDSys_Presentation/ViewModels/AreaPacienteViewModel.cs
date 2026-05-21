    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EntitiesServices.Attributes;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class AreaPacienteViewModel
    {
        [Key]
        public int AREA_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<System.DateTime> AREA_DT_ENTRADA { get; set; }
        public Nullable<int> PACI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> AREA_IN_TIPO { get; set; }
        public Nullable<int> AREA_IN_ATIVO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DA CONSULTA Deve ser uma data válida")]
        public Nullable<System.DateTime> AREA_DT_CONSULTA { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> AREA_HR_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> AREA_HR_FINAL { get; set; }
        [Required(ErrorMessage = "Campo TITULO obrigatorio")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "O TÍTULO DO ITEM deve conter no minimo 1 caracteres e no máximo 500 caracteres.")]
        public string AREA_NM_TITULO { get; set; }
        public string AREA_TX_CONTEUDO { get; set; }
        public string AREA_GU_IDENTIFICADOR { get; set; }
        [StringLength(250, ErrorMessage = "O NOME DO EXAME deve conter no máximo 250 caracteres.")]
        public string AREA_NM_EXAME { get; set; }
        public Nullable<int> TIEX_CD_ID { get; set; }
        public Nullable<int> AREA_IN_TIPO_EXAME { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DA CONSULTA Deve ser uma data válida")]
        public Nullable<System.DateTime> AREA_DT_DATA_EXAME { get; set; }
        public Nullable<int> LABS_CD_ID { get; set; }
        [StringLength(5000, ErrorMessage = "Os COMENTÁRIOS devem conter no máximo 5000 caracteres.")]
        public string AREA_TX_COMENTARIOS_DOCUMENTO { get; set; }
        public Nullable<int> LOCA_CD_ID { get; set; }
        public Nullable<int> AREA_IN_TIPO_CONSULTA { get; set; }
        public Nullable<int> AREA_IN_VISTA { get; set; }
        public Nullable<int> AREA_IN_PROCESSADA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE PROCESSAMENTO Deve ser uma data válida")]
        public Nullable<System.DateTime> AREA_DT_PROCESSO { get; set; }
        public string AREA_NM_PACIENTE_DUMMY { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE PROCESSAMENTO Deve ser uma data válida")]
        public Nullable<System.DateTime> AREA_DT_DUMMY { get; set; }

        public string NOME_PACIENTE { get; set; }
        public string NOME_PROFISSIONAL { get; set; }
        public string HORARIO { get; set; }
        public string EMAIL_PROFISSIONAL { get; set; }

        public string Vista
        {
            get
            {
                if (AREA_IN_VISTA == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public string Processada
        {
            get
            {
                if (AREA_IN_PROCESSADA == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public string TipoConsulta
        {
            get
            {
                if (AREA_IN_TIPO_CONSULTA == 1)
                {
                    return "Presencial";
                }
                return "Remota";
            }
        }

        public string DocumentoAzureUrl
        {
            get
            {
                // 1. Instancia o primeiro anexo da lista se houver
                var primeiroAnexo = this.AREA_PACIENTE_ANEXO?.FirstOrDefault();
                if (primeiroAnexo == null) return null;

                // 2. Monta o caminho físico exato das pastas do Storage (Usando o ID do Paciente e da Consulta)
                // Nota: Certifique-se de que as propriedades de ID correspondentes existam na sua ViewModel
                string blobOrigemPath = $"Imagens/{this.ASSI_CD_ID}/AreaPaciente/{this.AREA_CD_ID}/Anexos/{primeiroAnexo.APAN_NM_TITULO}";

                // 3. Junta tudo com o endereço do seu container oficial do Azure
                string urlCompleta = $"https://rtistoragemain.blob.core.windows.net/rti-datacontainer/{blobOrigemPath}";

                // 4. Retorna a URL codificada para ser aceita como parâmetro de QueryString no viewer.html
                return System.Web.HttpUtility.UrlEncode(urlCompleta);
            }
        }

        public string DocumentoAzureUrlNova
        {
            get
            {
                // Usando o campo dinâmico que guarda o caminho ou nome do arquivo no banco
                // (Substitua APAN_AQ_ARQUIVO pelo nome real da propriedade string do seu Model/ViewModel)
                Int32 id = AREA_CD_ID;
                var primeiroAnexo = this.AREA_PACIENTE_ANEXO.FirstOrDefault();
                if (primeiroAnexo == null) return null;
                if (string.IsNullOrEmpty(primeiroAnexo.APAN_AQ_ARQUIVO)) return null;

                // Limpa os caracteres de caminhos locais do servidor antigo
                string path = primeiroAnexo.APAN_AQ_ARQUIVO.Replace("~/", "").Replace("~", "");

                // Retorna a URL direta do Storage (sem UrlEncode aqui dentro)
                return $"https://rtistoragemain.blob.core.windows.net/rti-datacontainer/{path}";
            }
        }

        public string DocumentoAzureUrlNovissima
        {
            get
            {
                // Captura o ID da consulta da instância atual
                Int32 id = this.AREA_CD_ID;
                if (id == 0) return null;

                // 1. Tenta usar a lista da instância se ela já estiver carregada
                var primeiroAnexo = this.AREA_PACIENTE_ANEXO?.FirstOrDefault();

                // 2. Se a lista veio nula (falta de Lazy Loading), fazemos a busca explícita no banco
                if (primeiroAnexo == null)
                {
                    // Substitua 'SeuDbContext' pelo nome oficial da classe de contexto do seu projeto (ex: ErpContext)
                    using (var db = new EntitiesServices.Model.CRMSysDBEntities())
                    {
                        primeiroAnexo = db.AREA_PACIENTE_ANEXO
                                          .FirstOrDefault(x => x.AREA_CD_ID == id);
                    }
                }

                // Se mesmo buscando no banco não houver anexo, encerra
                if (primeiroAnexo == null || string.IsNullOrEmpty(primeiroAnexo.APAN_AQ_ARQUIVO))
                    return null;

                // Limpa os caracteres de caminhos locais do servidor antigo
                string path = primeiroAnexo.APAN_AQ_ARQUIVO.Replace("~/", "").Replace("~", "");

                // Retorna a URL direta do Storage limpa
                return $"https://rtistoragemain.blob.core.windows.net/rti-datacontainer/{path}";
            }
        }

        public string ObterDocumentoAzureUrl(Int32 id)
        {
            if (id == 0) return null;

            // 1. Tenta usar a lista da instância se ela já estiver carregada na memória
            var primeiroAnexo = this.AREA_PACIENTE_ANEXO?.FirstOrDefault();

            // 2. Se a lista veio nula (falta de Lazy Loading), faz a busca explícita usando o id recebido
            if (primeiroAnexo == null)
            {
                using (var db = new EntitiesServices.Model.CRMSysDBEntities())
                {
                    primeiroAnexo = db.AREA_PACIENTE_ANEXO
                                      .FirstOrDefault(x => x.AREA_CD_ID == id);
                }
            }

            // Se mesmo buscando no banco não houver anexo, encerra
            if (primeiroAnexo == null || string.IsNullOrEmpty(primeiroAnexo.APAN_AQ_ARQUIVO))
                return null;

            // Limpa os caracteres de caminhos locais do servidor antigo
            string path = primeiroAnexo.APAN_AQ_ARQUIVO.Replace("~/", "").Replace("~", "");

            // Retorna a URL direta do Storage limpa
            return $"https://rtistoragemain.blob.core.windows.net/rti-datacontainer/{path}";
        }

        public virtual PACIENTE PACIENTE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AREA_PACIENTE_ANEXO> AREA_PACIENTE_ANEXO { get; set; }
        public virtual LABORATORIO LABORATORIO { get; set; }
        public virtual TIPO_EXAME TIPO_EXAME { get; set; }
        public virtual LOCACAO LOCACAO { get; set; }
    }
}