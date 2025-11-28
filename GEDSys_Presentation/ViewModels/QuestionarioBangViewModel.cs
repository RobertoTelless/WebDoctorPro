using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class QuestionarioBangViewModel
    {
        [Key]
        public int QUSB_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int PACI_CD_ID { get; set; }
        [DisplayName("Você ronca alto (alto o suficiente para que possa ser ouvido através de portas fechadas ou seu companheiro cutuca você à noite para parar de roncar)?")]
        public Nullable<int> QUSB_IN_RONCO { get; set; }
        [DisplayName("Você frequentemente se sente cansado, exausto ou sonolento durante o dia (como, por exemplo, adormecer enquanto dirige)?")]
        public Nullable<int> QUSB_IN_CANSADO { get; set; }
        [DisplayName("Alguém observou que você para de respirar ou engasga/fica ofegante durante o seu sono?")]
        public Nullable<int> QUSB_IN_OBSERVA { get; set; }
        [DisplayName("Você tem ou está sendo tratado para pressão sanguínea alta?")]
        public Nullable<int> QUSB_IN_PRESSAO { get; set; }
        [DisplayName("O colar é de 43 cm ou mais (homens) / 41 cm ou mais (mulheres)?")]
        public Nullable<int> QUSB_IN_PESCOCO { get; set; }
        [DisplayName("IMC maior que 35 kg/m²?")]
        public Nullable<int> QUSB_IN_IMC { get; set; }
        [DisplayName("Idade acima de 50 anos?")]
        public Nullable<int> QUSB_IN_IDADE { get; set; }
        [DisplayName("Sexo masculino?")]
        public Nullable<int> QUSB_IN_MASCULINO { get; set; }
        public string QUSB_DS_PONTUACAO { get; set; }
        public Nullable<int> QUSB_IN_ATIVO { get; set; }
        public Nullable<int> QUSB_IN_PONTUACAO { get; set; }

        public static SelectList SimNaoOptions { get; } = new SelectList(new List<object>
    {
        new { Value = true, Text = "Sim" },
        new { Value = false, Text = "Não" }
    }, "Value", "Text", false); // 'false' é o valor default (Não)

        // Opcional: Adiciona uma opção para o critério do Pescoço Grosso, que tem descrição diferente.
        public string TextoPescocoGrosso => "Para homens, o colarinho da sua camisa é de 43 cm ou mais? Para mulheres, o colarinho da sua camisa é de 41 cm ou mais?";

        public virtual PACIENTE PACIENTE { get; set; }
    }
}