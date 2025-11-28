using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPacienteRepository : IRepositoryBase<PACIENTE>
    {
        PACIENTE CheckExist(PACIENTE item, Int32 idAss);
        PACIENTE GetItemById(Int32 id);
        PACIENTE GetItemByCPF(String cpf);
        List<PACIENTE> GetAllItens(Int32 idAss);
        List<PACIENTE> GetAllItensAdm(Int32 idAss);
        List<PACIENTE> ExecuteFilter(Int32? id, Int32? catId, Int32? sexo, String nome, String cpf, Int32? conv, Int32? menor, String celular, String email, String cidade, Int32? uf, Int32 idAss);
        List<PACIENTE> FiltrarContatos(GRUPO_PAC grupo, Int32 idAss);
    }
}
