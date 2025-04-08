using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace formulariosimples
{
    public partial class frmForumularioSimples : Form
    {
        MySqlConnection Conexao;
        string data_source = "datasource=localhost;username=root;password=;database=formulariosimples";

        public frmForumularioSimples()
        {
            InitializeComponent();
        }

        private void btnCadastrar_Click(object sender, EventArgs e)
        {
            try
            {
                string nomeUsuario;
                int numeroCadastro;
                DateTime dataNascimento;
                string cidade;
                bool generoF;
                bool generoM;
                bool generoNB;

                //validação de campos obrigatórios
                if (string.IsNullOrWhiteSpace(txtNumeroCadastro.Text))
                {
                    MessageBox.Show("Por favor, preencha o número de cadastro.");
                    return; // Interrompe a execução do código caso o campo esteja 
                }

                if (string.IsNullOrWhiteSpace(txtNomeCompleto.Text))
                {
                    MessageBox.Show("Por favor, preencha o nome completo.");
                    return;
                }

                //validação de data de nascimento usando DataTIMEPicker
                dataNascimento = dateTimePicker1.Value.Date;

                //verifica se a data é posterior ou igual a data atual
                if (dataNascimento >= DateTime.Now.Date) //Compara com a data atual sem a hora
                {
                    MessageBox.Show("Verifique novamente a sua data de nascimento.");
                    return;
                }

                if (comboBoxCidade.SelectedItem == null)
                {
                    MessageBox.Show("Por favor, selecione a cidade.");
                    return;
                }

                if (!rbFeminino.Checked && !rbMasculino.Checked && !rbNaoBinario.Checked)
                {
                    MessageBox.Show("Por favor, selecione o gênero.");
                    return;
                }


                //Agora, caso todos os campos estejam preenchidos, a validação prossegue
                numeroCadastro = Convert.ToInt32(txtNumeroCadastro.Text);
                nomeUsuario = txtNomeCompleto.Text;
                cidade = comboBoxCidade.Text;
                generoF = rbFeminino.Checked;
                generoM = rbMasculino.Checked;
                generoNB = rbNaoBinario.Checked;

                //formatar a data para formato brasileiro
                string dataFormatada = dataNascimento.ToString("dd/MM/yyyy");

                //Determinar o gênero selecionado
                string generoSelecionado = "Não informado"; //Caso nenhum gênero seja seleciondado
                if (generoF)
                    generoSelecionado = "Feminino";
                else if (generoM)
                    generoSelecionado = "Masculino";
                else if (generoNB)
                    generoSelecionado = "Não Binário";

                //Exibir as informações em messageBox
                MessageBox.Show("Número cadastro: " + numeroCadastro);
                MessageBox.Show("Nome: " + nomeUsuario);
                MessageBox.Show("Data de Nascimento: " + dataNascimento);
                MessageBox.Show("Cidade: " + cidade);
                MessageBox.Show("Gênero: " + generoSelecionado);

                //Cria a conexão com o banco de dados
                Conexao = new MySqlConnection(data_source);
                Conexao.Open();

                MessageBox.Show("Conexão feita com sucesso",
                               "Sucesso",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Information);

                //Comando SQL para inserir um novo cliente no banco de dados
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = Conexao
                };

                cmd.Prepare();

                cmd.CommandText = "INSERT INTO cadastro(numero_cadastro, nome_completo, data_nascimento, cidade, genero) " +
                "VALUES (@numero_cadastro, @nome_completo, @data_nascimento, @cidade, @genero)";

                //Adiciona os parâmetros com os dados do formulário
                cmd.Parameters.AddWithValue("@nome_completo", txtNomeCompleto.Text.Trim());
                cmd.Parameters.AddWithValue("@numero_cadastro", txtNumeroCadastro.Text.Trim());
                cmd.Parameters.AddWithValue("@data_nascimento", dataNascimento);
                cmd.Parameters.AddWithValue("@cidade", cidade);
                cmd.Parameters.AddWithValue("@genero", generoSelecionado);

                //Executa o comando de inserção no banco
                cmd.ExecuteNonQuery();

                //Mensagem de sucesso
                MessageBox.Show("Contato inserido com Sucesso: ",
                    "Sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (MySqlException ex)
            {
                //Trata erros relacionados ao MySQL
                MessageBox.Show("Erro " + ex.Number + " ocorreu: " + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            catch (Exception ex)
            {

                //Trata outros tipos de erro
                MessageBox.Show("Ocorreu: " + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                //Garante que a conexão com o banco será fechada, mesmo se ocorrer erro
                if (Conexao != null && Conexao.State == ConnectionState.Open)
                {
                    Conexao.Close();

                    //Teste de fechamento de banco
                    //MessageBox.Show("Conexão fechada com sucesso");
                }
            }

        }

        private void txtNumeroCadastro_Click(object sender, EventArgs e)
        {
            //Limpa o conteúdo do TextBox quando o usuário clicar nele
            if (txtNumeroCadastro.Text == "Número Cadastro")
            {
                txtNumeroCadastro.Text = "";
            }
            
        }

        private void txtNomeCompleto_Click(object sender, EventArgs e)
        {
            //Limpa o conteúdo do TextBox quando o usuário clicar nele
            if (txtNomeCompleto.Text == "Insira o seu nome completo")
            {
                txtNumeroCadastro.Text = "";
            }

        }
    }
}
