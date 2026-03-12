# Processar Distrato de Locação

Este método é responsável por validar, processar e arquivar o distrato assinado enviado pelo paciente através da Área do Paciente.

## ⚙️ Funcionamento Técnico

A Action realiza a validação de nomenclatura do arquivo e, em caso de sucesso, move o documento da pasta temporária da Área do Paciente para a pasta definitiva de Locação.

### Assinatura do Método
- **Controller:** `AreaPacienteController`
- **Tipo:** `POST` / `GET` (Action Task)
- **Retorno:** `RedirectToAction` (MontarTelaAreaPacienteVer)

---

## 📋 Regras de Validação de Arquivo

O sistema aplica duas validações rigorosas baseadas no nome do arquivo anexado:

### 1. Prefixo Obrigatório
O nome do arquivo deve começar obrigatoriamente com os primeiros 16 caracteres sendo:
`<span class="text-danger font-bold">DISTRATO_LOCACAO</span>`

### 2. Formato Exato da Nomenclatura
O título do anexo deve seguir exatamente o padrão abaixo:

> **Padrão:** `Distrato_Locacao{Nome_Paciente}_{GUID_Locacao}_Assinado.pdf`

---

## 🛠 Fluxo de Processamento

Se o arquivo passar nas validações acima, o sistema executa:

1. **Cópia Física:** O arquivo é copiado da pasta de origem para o destino final:
   - **Origem:** `/Imagens/{Assinatura}/AreaPaciente/{ID}/Anexos/`
   - **Destino:** `/Imagens/{Assinatura}/Locacao/{ID_Locacao}/Assinado/`
2. **Atualização de Status:**
   - `AREA_IN_VISTA = 1`
   - `AREA_IN_PROCESSADA = 1`
   - `AREA_DT_PROCESSO = DateTime.Now`

---

## ❌ Tratamento de Erros

Caso a nomenclatura esteja incorreta, o sistema interrompe o processo e:

| Falha | Mensagem de Erro | Ação Realizada |
| :--- | :--- | :--- |
| **Prefixo Inválido** | "O documento não é um distrato de locação válido." | Envia e-mail ao paciente informando o erro. |
| **Nome Incorreto** | "O documento não é válido para este paciente/locação." | Envia e-mail ao paciente informando o erro. |

<Warning>
Se a sessão do usuário expirante (`Session["Ativa"] == null`), o sistema redirecionará automaticamente para a tela de Logout.
</Warning>

---

## 💻 Exemplo de Código (Trecho de Validação)

```csharp
// Validação de Prefixo
String nome_obriga = item.APAN_NM_TITULO.Substring(0, 16);
if (nome_obriga.ToUpper() != "DISTRATO_LOCACAO") {
    falha = 1;
}