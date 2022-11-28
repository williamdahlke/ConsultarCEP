using ControlzEx.Standard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsultarCEP
{
    public partial class frmConsulta : Form
    {
        public frmConsulta()
        {
            InitializeComponent();
        }

        private void btConsultaCep_Click(object sender, EventArgs e)
        {

            int numCaracteres = txtCEP.Text.Length;
            if (numCaracteres < 8)
            {
                MessageBox.Show("Não é possível salvar um CEP com menos que 8 digítos. Favor verificar!");
                return;
            }
            try
            {
                var WS = new WSCorreios.AtendeClienteClient();
                var Resposta = WS.consultaCEP(txtCEP.Text);

                String cidade;
                cidade = Resposta.cidade;
                
                txtEstado.Text = Resposta.uf;
                txtCidade.Text = cidade.ToUpper();

                WS.Close();

                int nPosInicial = txtCidade.Text.IndexOf("(");
                int nPosFinal = txtCidade.Text.IndexOf(")");

                if ((nPosInicial > 0) && (nPosFinal > 0))
                {
                    nPosInicial = nPosInicial + 1;
                    int nPos = nPosFinal - nPosInicial;
                    string vCidade = txtCidade.Text.Substring(nPosInicial, nPos);
                    txtCidade.Text = vCidade;


                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void bt_salvar_Click(object sender, EventArgs e)
        {

            if ((txtCEP.Text != "") && (txtCidade.Text != "") && (txtEstado.Text != ""))
            {

                try
                {
                    //Remover os acentos e caracteres especiais do campo cidade
                    txtCidade.Text = UI.ObterStringSemAcentosECaracteresEspeciais(txtCidade.Text);

                    Conexao con = new Conexao();
                    OdbcConnection connectBanco = new OdbcConnection();

                    //Verificar se existe a cidade cadastrada no vestis
                    string sqlQuery = "select 1 from cidade where cidade = '" + txtCidade.Text + "' and uf = '" + txtEstado.Text + "'";
                    OdbcDataReader readerCidade = con.consultarDados(sqlQuery, con, connectBanco);
                    if (readerCidade.Read() == true)
                    {
                        if (readerCidade.GetString(0) != "1")
                        {
                            MessageBox.Show("Não foi possível encontrar o registro da cidade: " + txtCidade + " do estado: " + txtEstado + ". Favor verificar com o responsável!");
                            readerCidade.Close();
                            con.desconectarBanco(connectBanco);
                            return;
                        }
                    }
                    readerCidade.Close();
                    con.desconectarBanco(connectBanco);

                    //Verificar se já não existe esse cep cadastrado
                    sqlQuery = "select 1 from cep_cidade where cidade = '" + txtCidade.Text + "' and uf = '" + txtEstado.Text + "' and cep_inicio = '" + txtCEP.Text + "' and cep_fim = '" + txtCEP.Text + "'";
                    OdbcDataReader readerCep = con.consultarDados(sqlQuery, con, connectBanco);
                    if (readerCep.Read() == true)
                    {
                        if (readerCep.GetString(0) == "1")
                        {
                            MessageBox.Show("Este CEP já foi cadastrado!");
                            readerCep.Close();
                            con.desconectarBanco(connectBanco);
                            return;
                        }
                    }
                    readerCep.Close();
                    con.desconectarBanco(connectBanco);

                    
                    int seq = 1;

                    //Buscar o último sequencial cadastrado, caso não for encontrado ficará com o valor default da variável seq (1)
                    sqlQuery = "";
                    sqlQuery = "select sequencial from cep_cidade where cidade = '" + txtCidade.Text + "' and uf = '" + txtEstado.Text + "' order by sequencial desc";
                    OdbcDataReader reader = con.consultarDados(sqlQuery, con, connectBanco);
                    if (reader.Read() == true)
                    {
                        seq = int.Parse(reader.GetString(0));
                        seq = seq + 1;
                    }
                    
                    CepCidade cepCid = new CepCidade();
                    cepCid.setCep(txtCEP.Text);
                    cepCid.setCidade(txtCidade.Text);
                    cepCid.setEstado(txtEstado.Text);
                    cepCid.setSequencial(seq);                    

                    string sqlInsert = "insert into cep_cidade values ('" + cepCid.getCidade() + "','" + cepCid.getEstado() + "'," + cepCid.getSequencial() + ",'" + cepCid.getCep() + "','" + cepCid.getCep() + "', '')";
                    Boolean vSalvou = con.inserirDados(sqlInsert, con, connectBanco);

                    if (vSalvou == true)
                    {
                        MessageBox.Show("Os dados de CEP foram salvos com sucesso!");
                    }
                    else
                    {
                        MessageBox.Show("Os dados de CEP não foram salvos. Favor informar ao TI!");
                    }

                    con.desconectarBanco(connectBanco);
                    reader.Close();

                } catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            else
            {
                MessageBox.Show("É necessário consultar os dados antes de iniciar a gravação do CEP. Favor Verificar!");
                return;
            }

            //limpar todos os campos da tela
            // UI ui = new UI();
            //ui.LimpaCampos(this.panel1.Controls);

        }

        private void txtCEP_TextChanged(object sender, EventArgs e)
        {
            txtCidade.Text = "";
            txtEstado.Text = "";
        }

        private void bt_datagrid_Click_1(object sender, EventArgs e)
        {
            DataGridView dataGridViewCEP = new DataGridView();
            dataGridViewCEP.Location = new Point(50, 111);
            dataGridViewCEP.Visible = true;

            dataGridViewCEP.Columns.Add("cep_inicial", "CEP Inicial");
            dataGridViewCEP.Columns.Add("cep_final", "CEP Final");

            String[] row1 = { "89063001", "89063003" };
            dataGridViewCEP.Rows.Add(row1);          

            String[] row2 = { "89063003", "89063003" };
            dataGridViewCEP.Rows.Add(row2);

            dataGridViewCEP.AutoResizeColumn(0);
            dataGridViewCEP.AutoResizeColumn(1);          
            
        }
    }
}
