using System;
using System.Collections.Generic;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class MedicoMensagemViewModel
    {
        public int METX_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public Nullable<System.DateTime> METX_DT_CRIACAO { get; set; }
        public string METX_NM_NOME { get; set; }
        public string METX_TX_TEXTO { get; set; }
        public Nullable<int> METX_IN_ATIVO { get; set; }
        public Nullable<int> METX_IN_FIXO { get; set; }

        public string Fixo
        {
            get
            {
                if (METX_IN_FIXO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MEDICOS_ENVIO> MEDICOS_ENVIO { get; set; }
        public virtual USUARIO USUARIO { get; set; }

    }
}