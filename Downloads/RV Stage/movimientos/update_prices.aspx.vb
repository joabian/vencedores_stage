﻿Imports System.Data
Imports System.Text
Imports System.Drawing
Imports System.IO
Imports Excel

Partial Class movimientos_update_prices
    Inherits System.Web.UI.Page
    Public query As String
    Public queryLog As String
    Public Dataconnect As New DataConn_login
    Public ds As DataSet

    Public Sub readExcelNew()
        Dim items As String = ""
        Dim filepath As String = "e:\HostingSpaces\vencedo2\radiadoresvencedores.com\wwwroot\docs"
        Dim uploadedFiles As HttpFileCollection = Request.Files
        Dim i As Integer = 0
        Do Until i = uploadedFiles.Count
            Dim userPostedFile As HttpPostedFile = uploadedFiles(i)
            Try
                If (userPostedFile.ContentLength > 0) Then
                    Dim filename As String = Replace(System.IO.Path.GetFileName(userPostedFile.FileName), "'", "")
                    Dim fullpath As String = filepath & "\" & filename
                    Try
                        userPostedFile.SaveAs(fullpath)

                        Dim stream As FileStream = File.Open(fullpath, FileMode.Open, FileAccess.Read)
                        Dim excelReader As IExcelDataReader = ExcelReaderFactory.CreateOpenXmlReader(stream)
                        excelReader.IsFirstRowAsColumnNames = False
                        Dim result As DataSet = excelReader.AsDataSet()

                        File.Delete(filepath & "\" + filename)

                        add_itemsNew(result)

                    Catch ex As Exception
                        lbl_error_file.Text = ex.Message
                    End Try
                End If
            Catch ex As Exception
                lbl_error_file.Text = ex.Message
            End Try
            i += 1
        Loop

    End Sub

    Protected Sub leadexcel_Click(sender As Object, e As System.EventArgs) Handles leadexcel.Click
        readExcelNew()
    End Sub

    Public Sub add_itemsNew(ByVal list_items As DataSet)

        Dim motivo, sucursal, sucursal_id, error_msg, item, rack, price, id_record, logevent, part_code As String
        error_msg = ""
        Dim username As String = Membership.GetUser().UserName

        For i = 0 To list_items.Tables(0).Rows.Count - 1

            part_code = Replace(list_items.Tables(0).Rows(i)(0).ToString(), "'", "").ToUpper()
            price = list_items.Tables(0).Rows(i)(1).ToString()

            'verificamos que sean cantidades numericas
            If Not IsNumeric(price) Or price = "0" Then
                error_msg += "Verifique que todos los datos esten correctos en linea: " + (i + 1).ToString() + "<br />"
            Else

                query = "update products set " + ddlFieldToUpdate.SelectedValue + " = " + price.ToString() + " where code = '" + part_code.ToString() + "'"
                Dataconnect.runquery(query)

                logevent = "Actualizacion de precio de producto: " + part_code.ToString() + " a el precio de: " + price.ToString()
                queryLog = "INSERT INTO logs(user_name, event, date) VALUES ('" + username.ToString() + "', '" + logevent.ToString() + "', getDate())"
                Dataconnect.runquery(queryLog)
            End If
        Next

        If error_msg <> "" Then
            lbl_error_file.Text = "<br />Corrija los siguiente errores, el resto se ingreso con exito:<br />" + error_msg
        Else
            lbl_error_file.ForeColor = Color.Green
            lbl_error_file.Text = "<br />El archivo se cargo con exito!"
        End If

    End Sub

End Class
