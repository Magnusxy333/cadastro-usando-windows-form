Public Class Consultar
    Private id_editar As String

    Private Sub Consultar_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Connecta_banco()
        Carregar_tipo() ' Importante carregar o tipo ANTES dos dados para a busca funcionar
        Carregar_dados()
    End Sub

    Private Sub CadastroDeProdutoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CadastroDeProdutoToolStripMenuItem.Click
        Try
            Form1.ShowDialog()
            Carregar_dados() ' Atualiza a grade ao fechar o cadastro
        Catch ex As Exception
            MsgBox("Erro ao abrir cadastro", MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub txt_buscar_TextChanged(sender As Object, e As EventArgs) Handles txt_buscar.TextChanged
        Try
            Dim campo_banco As String = ""
            ' Certifique-se que o nome do ComboBox é cmb_campo
            Select Case cmd_campo.Text
                Case "DESCRIÇÃO" : campo_banco = "desc_produto"
                Case "CATEGORIA" : campo_banco = "categoria_prod"
                Case "MARCA" : campo_banco = "marca_produto"
                Case Else : campo_banco = "desc_produto"
            End Select

            sql = $"SELECT * FROM tb_cadastro_prod WHERE {campo_banco} LIKE '{txt_buscar.Text}%' ORDER BY desc_produto ASC"
            rs = db.Execute(sql)

            With dgv_dados
                .Rows.Clear()
                Do While Not rs.EOF
                    Dim v_custo As Double = Val(rs.Fields("preco_custo").Value & "")
                    Dim v_venda As Double = Val(rs.Fields("preco_venda").Value & "")
                    Dim v_margem As String = "0,00%"
                    If v_venda > 0 Then
                        v_margem = FormatNumber(((v_venda - v_custo) / v_venda) * 100, 2) & "%"
                    End If

                    .Rows.Add(rs.Fields("id_produto").Value,
                      rs.Fields("desc_produto").Value & "",
                      rs.Fields("categoria_prod").Value & "",
                      rs.Fields("marca_produto").Value & "",
                      Val(rs.Fields("qnt_dis_prod").Value & ""),
                      FormatCurrency(v_venda),
                      v_margem,
                      Nothing, ' <-- Mude de "" para Nothing na coluna EDITAR
                      Nothing  ' <-- Mude de "" para Nothing na coluna EXCLUIR
                      )
                    rs.MoveNext()
                Loop
            End With
        Catch ex As Exception
            Debug.Print("Erro na busca: " & ex.Message)
        End Try
    End Sub

    Private Sub dgv_dados_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgv_dados.CellContentClick
        Try
            ' Verifica se o clique foi em uma linha válida (não no cabeçalho)
            If e.RowIndex < 0 Then Exit Sub

            ' --- AÇÃO DO BOTÃO EXCLUIR ---
            If dgv_dados.Columns(e.ColumnIndex).Name = "btn_excluir" Then
                Dim id As String = dgv_dados.Rows(e.RowIndex).Cells(0).Value.ToString ' Pega o ID (Coluna 0)
                Dim nome As String = dgv_dados.Rows(e.RowIndex).Cells(1).Value.ToString ' Pega o Nome

                If MsgBox($"Deseja realmente excluir o produto: {nome}?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "CONFIRMAÇÃO") = MsgBoxResult.Yes Then
                    sql = $"DELETE FROM tb_cadastro_prod WHERE id_produto = {id}"
                    db.Execute(sql)
                    MsgBox("Produto excluído com sucesso!", MsgBoxStyle.Information)
                    Carregar_dados() ' Atualiza a lista
                End If
            End If

            ' --- AÇÃO DO BOTÃO EDITAR ---
            If dgv_dados.Columns(e.ColumnIndex).Name = "btn_editar" Then
                ' Aqui enviamos o ID para o Form1 carregar os dados
                ' Criaremos uma variável pública no Module1 chamada "id_editar"
                id_editar = dgv_dados.Rows(e.RowIndex).Cells(0).Value.ToString

                Form1.ShowDialog()
                Carregar_dados() ' Atualiza a lista após fechar a edição
            End If

        Catch ex As Exception
            MsgBox("Erro na operação: " & ex.Message)
        End Try
    End Sub
End Class