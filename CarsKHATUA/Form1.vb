Imports System.IO

Public Class Form1
    ' Variável para controlar se a foto mudou ou não
    Private foto_selecionada As String = ""
    Private lbl_margem As Label ' Corrigido: deve ser Label, não Object

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Connecta_banco()
        Carregar_tipo() ' Carrega os itens das ComboBoxes (Marcas/Categorias)

        ' VERIFICAÇÃO DE EDIÇÃO: Se id_produto_global > 0, carrega os dados do banco
        If id_produto_global > 0 Then
            Try
                Dim sql As String = "SELECT * FROM tb_cadastro_prod WHERE id_produto = " & id_produto_global
                Dim rs As Object = db.Execute(sql)

                If Not rs.EOF Then
                    ' Preenche os campos com o que está no banco
                    desc_produto_text.Text = rs.Fields("desc_produto").Value & ""
                    qnt_dis_prod_text.Text = rs.Fields("qnt_dis_prod").Value & ""
                    data_lote_date.Value = rs.Fields("data_lote").Value
                    preco_cust_text.Text = rs.Fields("preco_custo").Value
                    preco_venda_text.Text = rs.Fields("preco_venda").Value
                    marca_produto_cmd.Text = rs.Fields("marca_produto").Value & ""
                    categoria_produto_cmd.Text = rs.Fields("categoria_prod").Value & ""
                    foto_selecionada = rs.Fields("foto_prod").Value & ""

                    ' Carrega a foto se o caminho existir
                    If foto_selecionada <> "" AndAlso File.Exists(foto_selecionada) Then
                        foto_prod_pic.Load(foto_selecionada)
                    Else
                        foto_prod_pic.Image = Nothing
                    End If

                    bnt_gravar.Text = "ATUALIZAR" ' Muda o texto do botão
                    Calcular_Margem()
                End If
            Catch ex As Exception
                MsgBox("Erro ao carregar dados: " & ex.Message)
            End Try
        Else
            ' Se for 0, limpa para novo cadastro
            Limpar_campos_novo()
            bnt_gravar.Text = "GRAVAR"
        End If
    End Sub

    ' --- FUNÇÃO DE CÁLCULO DA MARGEM (CORRIGIDA) ---
    Sub Calcular_Margem()
        If lbl_margem Is Nothing Then Exit Sub
        Try
            Dim custo As Double = Val(preco_cust_text.Text.Replace(",", "."))
            Dim venda As Double = Val(preco_venda_text.Text.Replace(",", "."))

            If venda > 0 Then
                Dim margem As Double = ((venda - custo) / venda) * 100

                ' lbl_margem deve ser o nome da sua Label no Design
                lbl_margem.Text = FormatNumber(margem, 2) & "%"
                lbl_margem.ForeColor = If(margem < 0, Color.Red, Color.Green)
            Else
                lbl_margem.Text = "0,00%"
                lbl_margem.ForeColor = Color.Black
            End If
        Catch ex As Exception
            If lbl_margem IsNot Nothing Then
                lbl_margem.Text = "0,00%"
                lbl_margem.ForeColor = Color.Black
            End If
        End Try
    End Sub

    Private Sub preco_cust_text_TextChanged(sender As Object, e As EventArgs) Handles preco_cust_text.TextChanged, preco_venda_text.TextChanged
        Calcular_Margem()
    End Sub

    ' --- BOTÃO GRAVAR / ATUALIZAR ---
    Private Sub bnt_gravar_Click(sender As Object, e As EventArgs) Handles bnt_gravar.Click
        Try
            ' Validação básica: Descrição não pode ser vazia
            If String.IsNullOrWhiteSpace(desc_produto_text.Text) Then
                MsgBox("Digite a descrição!", MsgBoxStyle.Exclamation)
                Exit Sub
            End If

            ' Prepara os valores para o MySQL (Ponto em vez de vírgula)
            Dim v_custo As String = preco_cust_text.Text.Replace(",", ".")
            Dim v_venda As String = preco_venda_text.Text.Replace(",", ".")
            Dim v_data As String = data_lote_date.Value.ToString("yyyy-MM-dd")
            Dim v_foto As String = foto_selecionada.Replace("\", "\\")
            Dim sql As String

            If id_produto_global > 0 Then
                ' --- MODO EDIÇÃO (UPDATE) ---
                sql = "UPDATE tb_cadastro_prod SET " &
                      "desc_produto = '" & desc_produto_text.Text & "', " &
                      "qnt_dis_prod = " & Val(qnt_dis_prod_text.Text) & ", " &
                      "data_lote = '" & v_data & "', " &
                      "preco_custo = " & v_custo & ", " &
                      "preco_venda = " & v_venda & ", " &
                      "marca_produto = '" & marca_produto_cmd.Text & "', " &
                      "categoria_prod = '" & categoria_produto_cmd.Text & "', " &
                      "foto_prod = '" & v_foto & "' " &
                      "WHERE id_produto = " & id_produto_global

                db.Execute(sql)
                MsgBox("Produto atualizado com sucesso!", MsgBoxStyle.Information)
                Me.Close() ' Fecha o form e volta para a consulta
            Else
                ' --- MODO NOVO (INSERT) ---
                sql = "INSERT INTO tb_cadastro_prod (desc_produto, qnt_dis_prod, data_lote, preco_custo, preco_venda, marca_produto, categoria_prod, foto_prod) VALUES (" &
                      "'" & desc_produto_text.Text & "', " &
                      Val(qnt_dis_prod_text.Text) & ", '" & v_data & "', " &
                      v_custo & ", " & v_venda & ", " &
                      "'" & marca_produto_cmd.Text & "', '" & categoria_produto_cmd.Text & "', '" & v_foto & "')"

                db.Execute(sql)
                MsgBox("Produto gravado com sucesso!", MsgBoxStyle.Information)
                Limpar_campos_novo()
            End If

        Catch ex As Exception
            MsgBox("Erro na operação: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    ' --- SELECIONAR FOTO ---
    Private Sub foto_prod_pic_Click(sender As Object, e As EventArgs) Handles foto_prod_pic.Click
        Try
            Using OpenFileDialog1 As New OpenFileDialog()
                OpenFileDialog1.Title = "Selecione uma Foto"
                OpenFileDialog1.Filter = "Imagens|*.jpg;*.png;*.jpeg"
                If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
                    foto_selecionada = OpenFileDialog1.FileName
                    foto_prod_pic.Load(foto_selecionada)
                    foto_prod_pic.SizeMode = PictureBoxSizeMode.StretchImage
                End If
            End Using
        Catch ex As Exception
            MsgBox("Erro ao carregar imagem: " & ex.Message)
        End Try
    End Sub

    Sub Limpar_campos_novo()
        desc_produto_text.Clear()
        qnt_dis_prod_text.Clear()
        preco_cust_text.Clear()
        preco_venda_text.Clear()
        foto_prod_pic.Image = Nothing
        foto_selecionada = ""
        If lbl_margem IsNot Nothing Then lbl_margem.Text = "0,00%"
        desc_produto_text.Focus()
    End Sub

End Class