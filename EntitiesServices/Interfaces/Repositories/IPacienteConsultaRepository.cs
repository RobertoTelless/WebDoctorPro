using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPacienteConsultaRepository : IRepositoryBase<PACIENTE_CONSULTA>
    {
        PACIENTE_CONSULTA CheckExist(PACIENTE_CONSULTA item, Int32 idAss);
        List<PACIENTE_CONSULTA> GetAllItens(Int32 idAss);
        List<PACIENTE_CONSULTA> GetByCPF(String cpf);
        PACIENTE_CONSULTA GetItemById(Int32 id);
        List<PACIENTE_CONSULTA> GetAllItensAdm(Int32 idAss);
        List<PACIENTE_CONSULTA> ExecuteFilter(Int32? tipo, String nome, DateTime? inicio, DateTime? final, Int32? encerrada, Int32? usuario, Int32 idAss);
        List<PACIENTE_CONSULTA> ExecuteFilterConfirma(Int32? tipo, String nome, DateTime? inicio, DateTime? final, Int32? situacao, Int32? usuario, Int32 idAss);
    }
}
