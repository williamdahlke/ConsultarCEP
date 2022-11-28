using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultarCEP
{
    class CepCidade
    {
        private string cidade;
        private string estado;
        private string cep;
        private int sequencial;

        public CepCidade()
        {
            this.cidade = "";
            this.estado = "";
            this.cep = "";
            this.sequencial = 1;
        }

        public string getCidade()
        {
            return this.cidade;
        }

        public void setCidade(string cidade)
        {
            this.cidade = cidade;
        }

        public string getEstado()
        {
            return this.estado;
        }

        public void setEstado(string estado)
        {
            this.estado = estado;
        }

        public string getCep()
        {
            return this.cep;
        }

        public void setCep(string cep)
        {
            this.cep = cep;
        }

        public int getSequencial()
        {
            return this.sequencial; 
        }

        public void setSequencial(int sequencial)
        {
            this.sequencial = sequencial;
        }

    }
}
