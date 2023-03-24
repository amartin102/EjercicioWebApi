using back_end_peliculas.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_peliculas.Repositorios
{
    public interface IRepositorio
    {
        void CrearGenero(Genero genero);
        List<Genero> ObtenerGeneros();
        Guid ObtenerGuid();
        Task<Genero> ObtenerId(int id);
    }
}
