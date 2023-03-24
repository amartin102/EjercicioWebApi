using back_end_peliculas.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_peliculas.Repositorios
{
    public class RepositorioEnMemoria: IRepositorio
    {
        private List<Genero> _generos;

        public RepositorioEnMemoria() {
            _generos = new List<Genero>()
            {
                new Genero() {Id = 1, Nombre = "Acción" },
                new Genero() {Id = 2, Nombre = "Comedia" }
            };

            _guid = Guid.NewGuid(); // string extenso en forma de key
        }

        public Guid _guid;

        public List<Genero> ObtenerGeneros() {
            return _generos;
        }

        public async Task<Genero> ObtenerId(int id) {
            await Task.Delay(1);
            return _generos.FirstOrDefault(x => x.Id == id);
        }

        public Guid ObtenerGuid() {
            return _guid;
        }

        public void CrearGenero(Genero genero) {

            genero.Id = _generos.Count() + 1;

            _generos.Add(genero);
        }
    }
}
