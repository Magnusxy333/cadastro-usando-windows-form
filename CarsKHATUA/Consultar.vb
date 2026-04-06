Public Class Consultar

    Private Sub CadastroDeProdutoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CadastroDeProdutoToolStripMenuItem.Click
        Try
            ' IMPORTANTE: Reseta o ID para -1 ANTES de abrir o formulário
            id_editar = -1

            ' Cria uma NOVA instância do Form1 para garantir que está limpo
            Dim frmCadastro As New Form1()
            frmCadastro.ShowDialog()

            ' Atualiza a grid após fechar
            Carregar_dados()
        Catch ex As Exception
            MsgBox("Erro ao abrir cadastro: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub dgv_dados_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgv_dados.CellContentClick
        Try
            If e.RowIndex < 0 Then Exit Sub

            ' Botão EDITAR
            If dgv_dados.Columns(e.ColumnIndex).Name = "btn_editar" Then
                ' Define o ID para edição
                id_editar = Convert.ToInt32(dgv_dados.Rows(e.RowIndex).Cells(0).Value)

                ' Cria uma NOVA instância do Form1 para edição
                Dim frmEdicao As New Form1()
                frmEdicao.ShowDialog()

                ' RESETA O ID NOVAMENTE APÓS FECHAR A EDIÇÃO
                id_editar = -1

                ' Atualiza a grid
                Carregar_dados()
            End If

            ' Botão EXCLUIR
            If dgv_dados.Columns(e.ColumnIndex).Name = "btn_excluir" Then
                Dim id As Integer = Convert.ToInt32(dgv_dados.Rows(e.RowIndex).Cells(0).Value)
                Dim nome As String = dgv_dados.Rows(e.RowIndex).Cells(1).Value.ToString

                If MsgBox($"Deseja realmente excluir o produto: {nome}?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "CONFIRMAÇÃO") = MsgBoxResult.Yes Then
                    sql = $"DELETE FROM tb_cadastro_prod WHERE id_produto = {id}"
                    db.Execute(sql)
                    MsgBox("Produto excluído com sucesso!", MsgBoxStyle.Information)
                    Carregar_dados()
                End If
            End If

        Catch ex As Exception
            MsgBox("Erro: " & ex.Message)
        End Try
    End Sub
End Class