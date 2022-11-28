using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultarCEP
{
    class Conexao
    {
        private OdbcConnection conectarBanco(OdbcConnection connection)
        {
            //Colocar aqui a string de conexão ODBC
            connection.ConnectionString = "";
            connection.Open();
            return connection;
        }

        public void desconectarBanco(OdbcConnection connection)
        {
            connection.Close();
        }

        public OdbcDataReader consultarDados(string consultaSql, Conexao con, OdbcConnection connection)
        {
            con.conectarBanco(connection);
            OdbcCommand command = new OdbcCommand(consultaSql, connection);
            OdbcDataReader reader = command.ExecuteReader();
            return reader; 
        }

        public Boolean inserirDados(string insertSql, Conexao con, OdbcConnection connection)
        {
            try{
                OdbcCommand command = new OdbcCommand(insertSql);
                command.Connection = connection;
                command.ExecuteNonQuery();
                return true;
            } catch (Exception)
            {
                return false;
            }            
        }
    }
}
